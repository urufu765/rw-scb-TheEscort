using BepInEx;
using RWCustom;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using Newtonsoft;
using Newtonsoft.Json;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        public static readonly PlayerFeature<string> CustomShader = PlayerString("theescort/speedster/custom_shader");
        public static readonly PlayerFeature<float[]> speedsterPolewow = PlayerFloats("theescort/speedster/pole_rise");

        public void Esclass_SS_Tick(Player self, ref Escort e)
        {
            if (e.SpeTrailTick > 0)
            {
                e.SpeTrailTick--;
            }
            if (e.SpeBonk > 0)
            {
                e.SpeBonk--;
            }
            if (e.SpeSpeedin > 0 && !e.SpeOldSpeed)
            {
                e.SpeSpeedin--;
                if (self.input[0].x != 0 && e.SpeNitros > 0)
                {
                    e.SpeNitros--;
                }
                else if (self.input[0].x == 0 && e.SpeNitros < e.SpeGear * 2)
                {
                    e.SpeNitros += 2;
                }
            }
            else
            {
                e.SpeNitros = 0;
            }
        }

        private void Esclass_SS_DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
        {
            try
            {
                if (e.SpeTrailTick == 0 && e.SpeDashNCrash && self != null && self.player != null && self.owner != null && self.owner.room != null && self.player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut)
                {
                    e.Escat_addTrail(rCam, s, (int)Mathf.Lerp(20, 40, self.player.Adrenaline), (int)Mathf.Lerp(10, 20, self.player.Adrenaline));
                    //e.Escat_addSpeTrail(s, rCam, (int)Mathf.Lerp(20, 40, self.player.Adrenaline), (int)Mathf.Lerp(10, 20, self.player.Adrenaline));
                    e.SpeTrailTick = 2;
                }
                //e.Escat_showTrail();
                // foreach(var speedTrail in e.SpeTrail2)
                // {
                //     speedTrail.DrawSprites(s, rCam, t, camP);
                // }
            }
            catch (Exception err)
            {
                Ebug(self.player, err, "Speedster Draw Sprite failed!");
            }
        }

        private void Esclass_SS_Update(Player self, ref Escort e)
        {
            if (!(self != null && self.mainBodyChunk != null))
            {
                return;
            }
            if (e.SpeOldSpeed)
            {
                if (e.SpeBonk == 1)
                {
                    self.Stun(e.SpeSecretSpeed ? 100 : 60);
                }
                if (self.Stunned)
                {
                    e.SpeSpeedin = 0;
                }
                if (e.SpeSpeedin < 20)
                {
                    self.slugcatStats.throwingSkill = 0;
                    e.SpeDashNCrash = false;
                    e.SpeSecretSpeed = false;
                    e.SpeExtraSpe = 0;
                }
                else if (e.SpeSpeedin >= 240)
                {
                    if (!e.SpeDashNCrash && self.room != null)
                    {
                        self.slugcatStats.throwingSkill = 1;
                        for (int i = 0; i < 5; i++)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(10 * Mathf.Sign(self.bodyChunks[1].vel.x), 0), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                        }
                        self.room.PlaySound(SoundID.Weapon_Skid, e.SFXChunk, false, 0.7f, 0.5f);
                        self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, e.SFXChunk, false, 0.3f, 7f);
                    }
                    e.SpeDashNCrash = true;
                    e.SpeExtraSpe += 2;
                }
                else
                {
                    e.SpeExtraSpe--;
                }
                if (e.SpeExtraSpe > 480)
                {
                    if (!e.SpeSecretSpeed && self.room != null)
                    {
                        self.slugcatStats.throwingSkill = 2;
                        for (int i = 0; i < 10; i++)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(10 * Mathf.Sign(self.bodyChunks[1].vel.x), 0), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                        }
                        self.room.PlaySound(SoundID.Weapon_Skid, e.SFXChunk, false, 0.74f, 1.5f);
                        self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, e.SFXChunk, false, 0.32f, 8.7f);
                    }
                    e.SpeSecretSpeed = true;
                }
                if (!e.SpeDashNCrash)
                {
                    float v = Mathf.Abs(self.mainBodyChunk.vel.x);
                    if (self.bodyMode == Player.BodyModeIndex.CorridorClimb)
                    {
                        v = Mathf.Max(v, Mathf.Abs(self.mainBodyChunk.vel.y));
                    }
                    switch (v)
                    {
                        case var _ when v > 13f:
                            e.SpeSpeedin += 10;
                            break;
                        case var _ when v > 9f:
                            e.SpeSpeedin += 4;
                            break;
                        case var _ when v > 5.5f:
                            e.SpeSpeedin++;
                            break;
                        default:
                            if (Mathf.Abs(self.mainBodyChunk.vel.y) <= 4.5f)
                            {
                                e.SpeSpeedin--;
                            }
                            break;
                    }
                }
                else
                {
                    if (Mathf.Abs(self.mainBodyChunk.vel.x) < 7 && Mathf.Abs(self.mainBodyChunk.vel.y) < 6)
                    {
                        e.SpeSpeedin--;
                    }
                    if (Mathf.Abs(self.mainBodyChunk.vel.x) > 9 || Mathf.Abs(self.mainBodyChunk.vel.y) > 8)
                    {
                        e.SpeSpeedin++;
                    }
                    if (self.slowMovementStun > 5)
                    {
                        self.slowMovementStun = 5;
                    }
                }
                e.SpeSpeedin = RWCustom.Custom.IntClamp(e.SpeSpeedin, 0, 320);
            }

            else
            {  // New speedway
                if (e.SpeBonk == 1)
                {
                    self.Stun(40 * e.SpeGear);
                    if (e.SpeDashNCrash && e.SpeGear > 0)
                    {
                        e.SpeGear--;
                        e.SpeSpeedin = Math.Min(0, e.SpeSpeedin - 80)
                    }
                }
                if (self.Stunned)
                {
                    e.SpeBuildup = 0f;
                }
                // Buildup speed
                if (!e.SpeDashNCrash)
                {
                    e.SpeGain = -1f;
                    if (Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)) > 2f)
                    {
                        // Going fast builds up charge
                        if (self.input[0].AnyDirectionalInput)
                        {
                            e.SpeGain += 1f + 3f * Mathf.InverseLerp(5f, 17f, Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)));
                        }

                        // Doing things while moving builds up charge
                        /*
                        switch (self.animation)
                        {
                            case var value when value == Player.AnimationIndex.Flip:
                                e.SpeGain++;
                                break;
                            case var value when value == Player.AnimationIndex.BellySlide:
                                e.SpeGain += 5;
                                break;
                            case var value when value == Player.AnimationIndex.Roll:
                                e.SpeGain += 1.1f;
                                break;
                            case var value when value == Player.AnimationIndex.StandOnBeam:
                                e.SpeGain += 0.7f;
                                break;
                            case var value when value == Player.AnimationIndex.SurfaceSwim:
                                e.SpeGain += 1.2f;
                                break;
                            case var value when value == Player.AnimationIndex.RocketJump:
                                e.SpeGain += 2f;
                                break;
                        }
                        */
                        e.SpeGain += self.animation switch {
                            var value when value == Player.AnimationIndex.Flip => 1,
                            var value when value == Player.AnimationIndex.BellySlide => e.CustomKeybindEnabled? 1.3f : 5,
                            var value when value == Player.AnimationIndex.Roll => 1.1f,
                            var value when value == Player.AnimationIndex.StandOnBeam => 0.7f,
                            var value when value == Player.AnimationIndex.SurfaceSwim => 1.2f,
                            var value when value == Player.AnimationIndex.RocketJump => 2,
                            _ => 0
                        };

                        // Double the gain when in hyped
                        if (e.SpeGain > 0f && self.aerobicLevel > hypeRequirement)
                        {
                            e.SpeGain *= 2;
                        }

                        // VFX
                        if (self.room != null && e.SpeCharge > 0)
                        {
                            if (self.bodyChunks[1].contactPoint.y == -1)
                            {
                                self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(0, -5f), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 4, 8));
                            }
                            else
                            {
                                //self.room.AddObject(new CollectToken.TokenSpark(self.bodyChunks[1].pos + new Vector2(0, -5f), new Vector2(-1f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, false));

                            }
                        }
                    }

                    // Add charge storage
                    if (e.SpeBuildup > 239 && e.SpeCharge < e.SpeMaxGear)
                    {
                        Ebug(self, "Charge! " + e.SpeCharge + " => " + (e.SpeCharge + 1));
                        if (self.room != null)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(-10 * Mathf.Sign(self.bodyChunks[1].vel.x), -5), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                            }
                            self.room.PlaySound(SoundID.Weapon_Skid, e.SFXChunk, false, 0.74f, 0.5f + 0.15f * e.SpeCharge);
                            self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, e.SFXChunk, false, 0.32f, 6f + 0.5f * e.SpeCharge);
                        }
                        e.SpeCharge++;
                        e.SpeBuildup = 0f;
                    }

                    // Clamp
                    e.SpeBuildup = Mathf.Clamp(e.SpeBuildup + e.SpeGain, 0, 240);
                }
                // Use speed
                else
                {
                    if (self.slowMovementStun > 5)
                    {
                        self.slowMovementStun = 5;
                    }
                    if (e.SpeSpeedin == 0)
                    {
                        e.SpeGear = 0;
                        e.SpeDashNCrash = false;
                    }
                }
            }
        }

        private void Esclass_SS_UpdateBodyMode(Player self, ref Escort e)
        {
            float n = 1f;
            if (e.SpeOldSpeed)
            {
                n = e.SpeSecretSpeed ? 2.5f : 1f;
            }
            else
            {
                n += 0.6f * e.SpeGear;
            }

            if (e.SpeDashNCrash)
            {
                self.dynamicRunSpeed[0] += 3f * n;
                self.dynamicRunSpeed[1] += 3f * n;
            }
        }

        private void Esclass_SS_UpdateAnimation(Player self, ref Escort e)
        {
            if (!speedsterPolewow.TryGet(self, out float[] poleR)) return;
            float n = 0.8f;
            if (e.SpeOldSpeed)
            {
                n = e.SpeSecretSpeed ? 1.6f : 0.8f;
            }
            else
            {
                n += 0.45f * e.SpeGear;
            }
            if (self.animation == Player.AnimationIndex.BellySlide)
            {
                int initReq = 2;
                int rollCount = 4;
                if (e.SpeOldSpeed)
                {
                    if (e.SpeDashNCrash) 
                    {
                        initReq = 3;
                        rollCount = 4;
                    }
                    if (e.SpeSecretSpeed)
                    {
                        initReq = 5;
                        rollCount = 8;
                    }
                }
                else
                {
                    int toAdd = e.SpeGear switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 3,
                        _ => 4
                    };
                    initReq += toAdd;
                    rollCount += toAdd;
                }
                if (self.initSlideCounter < initReq)
                {
                    self.initSlideCounter += initReq;
                }
                if (self.rollCounter < rollCount)
                {
                    self.rollCounter += rollCount;
                }
                if (self.rollCounter >= 9 && e.SpeRollCounter < rollCount)
                {
                    self.rollCounter--;
                    e.SpeRollCounter++;
                }
            }
            else
            {
                e.SpeRollCounter = 0;
            }

            if (e.SpeDashNCrash)
            {
                if (self.animation == Player.AnimationIndex.Roll)
                {
                    self.mainBodyChunk.vel.x += 1f * self.input[0].x * n;
                }
                if (self.animation == Player.AnimationIndex.BellySlide)
                {
                    self.bodyChunks[0].vel.x += Mathf.Sign(self.bodyChunks[0].vel.x) * n * 2.5f;
                    self.bodyChunks[1].vel.x += Mathf.Sign(self.bodyChunks[1].vel.x) * n * 2.3f;
                    self.bodyChunks[0].vel.y *= 0.9f;
                    self.bodyChunks[1].vel.y *= 0.75f;
                }
                if (self.animation == Player.AnimationIndex.HangFromBeam)
                {
                    self.bodyChunks[0].vel.x += self.input[0].x * n * 0.5f;
                    self.bodyChunks[1].vel.x += self.input[0].x * n * 0.5f;
                }
                if (self.animation == Player.AnimationIndex.StandOnBeam)
                {
                    self.bodyChunks[0].vel.x += self.input[0].x * n * 1.1f;
                    self.bodyChunks[1].vel.x += self.input[0].x * n * 1.1f;
                }
                if (self.animation == Player.AnimationIndex.ClimbOnBeam)
                {
                    if (self.input[0].y > 0)
                    {
                        self.bodyChunks[0].vel.y += poleR[0] * self.input[0].y * n * 1.3f;
                    }
                    else
                    {
                        self.bodyChunks[1].vel.y += poleR[1] * self.input[0].y * n * 1.3f;
                    }
                }
                if (self.animation == Player.AnimationIndex.RocketJump && self.allowRoll == 0)
                {
                    //self.dynamicRunSpeed[0] += 2f * n;
                    //self.dynamicRunSpeed[1] += 2f * n;
                    self.bodyChunks[0].vel.x += self.input[0].x * n * 3;
                    self.bodyChunks[1].vel.x += self.input[0].x * n * 3;
                }
            }
            else if (!e.SpeOldSpeed)  // Trigger speed ability
            {
                bool condition = e.CustomKeybindEnabled? Input.GetKey(e.CustomKeybind) : (self.animation == Player.AnimationIndex.BellySlide && !e.slideFromSpear && self.rollCounter > 10);
                if (e.SpeCharge > 0 && condition)
                {
                    e.SpeGear = e.SpeCharge - 1;
                    self.slugcatStats.throwingSkill = 1;
                    if (e.SpeGear > 2)
                    {
                        self.slugcatStats.throwingSkill = 2;
                    }
                    e.SpeDashNCrash = true;
                    e.SpeCharge = 0;
                    e.SpeBuildup = 0;
                    e.SpeSpeedin = 200 + 80 * e.SpeGear;  // Made the math simple
                    e.SpeNitros = e.SpeGear * 2;
                    //e.SpeSpeedin = 200 + 60 * (int)Math.Pow(2, e.SpeGear);
                    e.SpeExtraSpe = e.SpeSpeedin;
                    if (self.room != null)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(-10 * Mathf.Sign(self.bodyChunks[1].vel.x), 0), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                        }
                        self.room.PlaySound(SoundID.Firecracker_Bang, e.SFXChunk, false, 0.5f, 1.5f + 0.2f * e.SpeGear);
                    }
                }
            }
        }

        private void Esclass_SS_MovementUpdate(Player self, ref Escort e)
        {
            // Nitros boost
            if (!e.SpeOldSpeed && e.SpeDashNCrash && e.SpeNitros > 0)
            {
                self.bodyChunks[0].vel.x += (float)(self.input[0].x * e.SpeNitros);
                self.bodyChunks[1].vel.x += (float)(self.input[0].x * e.SpeNitros);
            }
        }


        private void Esclass_SS_Jump(Player self, ref Escort e)
        {
            float n = 0.8f;
            if (e.SpeOldSpeed)
            {
                n = e.SpeSecretSpeed ? 1.3f : 0.8f;
            }
            else
            {
                n += 0.35f * e.SpeGear;
            }
            if (e.SpeDashNCrash)
            {
                Ebug(self, "Speedster Jump!");
                /*
                self.bodyChunks[0].vel.y += 6f;
                self.bodyChunks[1].vel.y += 5f;
                self.bodyChunks[0].vel.x += 3f * (float)self.flipDirection;
                self.bodyChunks[1].vel.x += 2f * (float)self.flipDirection;
                */
                /*
                if (self.animation == Player.AnimationIndex.None)
                {
                    self.jumpBoost += 1f * n;
                }
                if (self.animation == Player.AnimationIndex.Flip)
                {
                    self.jumpBoost += 4f * n;
                }
                if (self.animation == Player.AnimationIndex.Roll)
                {
                    self.jumpBoost += 8f * n;
                }
                if (self.animation == Player.AnimationIndex.SurfaceSwim)
                {
                    self.jumpBoost += 2f * n;
                }
                if (self.animation == Player.AnimationIndex.StandOnBeam)
                {
                    self.jumpBoost += 4f * n;
                }
                */

                self.jumpBoost += self.animation switch {
                    var value when value == Player.AnimationIndex.None => 1 * n,
                    var value when value == Player.AnimationIndex.Flip => 4 * n,
                    var value when value == Player.AnimationIndex.Roll => 8 * n,
                    var value when value == Player.AnimationIndex.SurfaceSwim => 2 * n,
                    var value when value == Player.AnimationIndex.StandOnBeam => 4 * n,
                    _ => 0,
                };
            }
        }

        private void Esclass_SS_Collision(Player self, Creature creature, ref Escort e)
        {
            if (e.SpeDashNCrash && !creature.dead && Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)) > 10f)
            {
                bool checkSlide = self.animation == Player.AnimationIndex.BellySlide;
                if (e.SpeOldSpeed)
                {
                    creature.SetKillTag(self.abstractCreature);
                    creature.LoseAllGrasps();
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x * DKMultiplier, self.bodyChunks[0].vel.y * DKMultiplier)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        Mathf.Lerp(
                            0.1f, e.SpeSecretSpeed ? 2.5f : 1f, Mathf.InverseLerp(
                                4f, 16f, Mathf.Max(
                                    Mathf.Abs(self.mainBodyChunk.vel.x),
                                    Mathf.Abs(self.mainBodyChunk.vel.y)
                                )
                            )
                        ), e.SpeSecretSpeed ? 80f : 45f);
                    if (self.room != null)
                    {
                        self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk, false, 2.3f, 1.2f);
                        self.room.PlaySound(Escort_SFX_Impact, e.SFXChunk);
                    }
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x * DKMultiplier * (creature.TotalMass * (checkSlide? 1f : 0.5f));
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y * DKMultiplier * (creature.TotalMass * (checkSlide? 1f : 0.5f));
                    //self.WallJump(-self.flipDirection);
                    if (!checkSlide)
                    {
                        self.bodyChunks[0].vel.x *= -(e.SpeSecretSpeed ? 2.5f : 1.5f);
                        self.bodyChunks[1].vel.x *= -(e.SpeSecretSpeed ? 1.5f : 1f);
                        self.Stun(e.SpeSecretSpeed ? 160 : 60);
                    }
                }
                else
                {
                    float slamDam = 1f + 0.6f * e.SpeGear;
                    float slamStun = 45f + 15f * e.SpeGear;
                    if (e.isChunko)
                    {
                        slamDam *= self.TotalMass / e.originalMass;
                        slamStun *= self.TotalMass / e.originalMass;
                    }
                    creature.SetKillTag(self.abstractCreature);
                    creature.LoseAllGrasps();
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x * DKMultiplier, self.bodyChunks[0].vel.y * DKMultiplier)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        Mathf.Lerp(
                            0.1f, slamDam, Mathf.InverseLerp(
                                4f, 16f, Mathf.Max(
                                    Mathf.Abs(self.mainBodyChunk.vel.x),
                                    Mathf.Abs(self.mainBodyChunk.vel.y)
                                )
                            )
                        ), slamStun);
                    if (self.room != null)
                    {
                        self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk, false, 2.3f, 1.2f);
                        self.room.PlaySound(Escort_SFX_Impact, e.SFXChunk);
                    }
                    float velocityX = self.bodyChunks[0].vel.x * DKMultiplier * (creature.TotalMass * (checkSlide? 0.35f : 0.5f));
                    float velocityY = self.bodyChunks[0].vel.y * DKMultiplier * (creature.TotalMass * (self.bodyChunks[0].vel.y > 0? 1.25f : 0.5f));
                    if (e.isChunko)
                    {
                        velocityX *= self.TotalMass / e.originalMass;
                        velocityY *= self.TotalMass / e.originalMass;
                    }
                    creature.firstChunk.vel.x += velocityX;
                    creature.firstChunk.vel.y += velocityY;
                    //self.WallJump(-self.flipDirection);
                    if (!checkSlide)
                    {
                        self.bodyChunks[0].vel.x *= -(1.5f + 0.3f * e.SpeGear);
                        self.bodyChunks[1].vel.x *= -(1f + 0.2f * e.SpeGear);
                        self.Stun((int)(slamStun * 1.5f));
                    }
                    else
                    {
                        self.bodyChunks[0].vel.x *= -0.01f;
                        self.bodyChunks[1].vel.x *= -0.025f;
                    }
                }
            }
        }

        private void Esclass_SS_Bonk(On.Player.orig_TerrainImpact orig, Player self, int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            orig(self, chunk, direction, speed, firstContact);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error in Speester TerrainImpact");
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return;
            }
            if (!self.dead && e.SpeDashNCrash)
            {
                float limit = 14f;
                if (e.SpeOldSpeed && e.SpeSecretSpeed)
                {
                    limit = 16f;
                }
                else
                {
                    limit += 1.5f * e.SpeGear;
                }
                if (firstContact && speed > limit && direction.x != 0 && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.animation != Player.AnimationIndex.Flip && self.animation != Player.AnimationIndex.BellySlide)
                {
                    self.room?.PlaySound(e.SpeSecretSpeed ? SoundID.Slugcat_Terrain_Impact_Hard : SoundID.Slugcat_Terrain_Impact_Medium, e.SFXChunk);
                    e.SpeBonk = 5;
                }
            }
            if (MoreSlugcats.MMF.cfgWallpounce.Value && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.wantToJump > 0 && direction.x != 0 && chunk == 0 && speed > 7f && self.input[0].x == direction.x && !self.standing)
            {
                self.bodyChunks[0].vel = new Vector2(direction.x * -25f, 10f);
                self.bodyChunks[1].vel = new Vector2(direction.x * -25f, 10f);
                if (e.SpeOldSpeed)
                {
                    float extraHeight = 5f;
                    if (e.SpeDashNCrash)
                    {
                        extraHeight = 10f;
                    }
                    if (e.SpeSecretSpeed)
                    {
                        extraHeight = 20f;
                    }
                    self.bodyChunks[0].vel.y += extraHeight + 1f;
                    self.bodyChunks[1].vel.y += extraHeight;
                }
                else
                {
                    self.bodyChunks[0].vel.y += 6f + 4f * e.SpeGear;
                    self.bodyChunks[1].vel.y += 5f + 4f * e.SpeGear;
                }
                self.jumpStun = 15 * -direction.x;
            }
        }


        public static void Esclass_SS_WinLoseSave(ShelterDoor self, int playerNumber, bool success, ref Escort escort)
        {
            if (self.room?.game?.session is StoryGameSession storyGameSession)
            {
                storyGameSession.saveState.miscWorldSaveData.Esave().SpeChargeStore[playerNumber] = success? escort.SpeCharge : 0;
                storyGameSession.saveState.miscWorldSaveData.Esave().SpeChargeStore.TryGetValue(playerNumber, out int charging);
                Ebug("Saved successfully to " + playerNumber + ": " + charging);
                if (escort.shelterSaveComplete <= 1)
                {
                    Ebug("Misc: " + JsonConvert.SerializeObject(storyGameSession.saveState.miscWorldSaveData.Esave()), 1, true);
                }
            }
        }
    }
}