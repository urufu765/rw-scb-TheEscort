using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using Newtonsoft.Json;

namespace TheEscort.Speedster.Old;

public static class RacingSpeedster
{
    public static void Tick(Player self, ref Escort e)
    {
        if (e.SpeTrailTick > 0)
        {
            e.SpeTrailTick--;
        }
        if (e.SpeBonk > 0)
        {
            e.SpeBonk--;
        }

        if (e.SpeSpeedin > 0 || e.SpeDashNCrash)
        {
            e.SpeSpeedin--;
            if (self.input[0].x != 0 && e.SpeNitrosX > 0)
            {
                e.SpeNitrosX--;
            }
            else if (self.input[0].x == 0 && e.SpeNitrosX < e.SpeGear * 2)
            {
                e.SpeNitrosX += 2;
            }
        }
        else
        {
            e.SpeNitrosX = 0;
        }
    }


    public static void Update(Player self, ref Escort e)
    {
        if (self?.bodyChunks is null) return;
        if (e.SpeBonk == 1)
        {
            self.Stun(40 * e.SpeGear);
            if (e.SpeDashNCrash && e.SpeGear > 0)
            {
                e.SpeGear--;
                e.SpeSpeedin = Math.Max(0, e.SpeSpeedin - 80);
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
                bool nerfSlide = e.CustomKeybindEnabled || !ins.config.cfgDisableSpeedsterSlide.Value;
                e.SpeGain += self.animation switch
                {
                    var value when value == Player.AnimationIndex.Flip => 1,
                    var value when value == Player.AnimationIndex.BellySlide => nerfSlide ? 1.3f : 5,
                    var value when value == Player.AnimationIndex.Roll => 1.1f,
                    var value when value == Player.AnimationIndex.StandOnBeam => 0.7f,
                    var value when value == Player.AnimationIndex.SurfaceSwim => 1.2f,
                    var value when value == Player.AnimationIndex.RocketJump => 2,
                    _ => 0
                };

                // Double the gain when in hyped
                if (e.SpeGain > 0f && self.aerobicLevel > ins.hypeRequirement)
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
                SS_Fx.Gfx_Sparkle_Boom(self.room, self, e.SpeColor);
                SS_Fx.Sfx_Sparkle_Click(self.room, e.SFXChunk, e.SpeCharge);
                // if (self.room != null)
                // {
                //     for (int i = 0; i < 10; i++)
                //     {
                //         self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(-10 * Mathf.Sign(self.bodyChunks[1].vel.x), -5), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                //     }
                //     self.room.PlaySound(SoundID.Weapon_Skid, e.SFXChunk, false, 0.74f, 0.5f + 0.15f * e.SpeCharge);
                //     self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, e.SFXChunk, false, 0.32f, 6f + 0.5f * e.SpeCharge);
                // }
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
            if (e.SpeSpeedin <= 0)
            {
                e.SpeGear = 0;
                e.SpeDashNCrash = false;
            }
        }
    }


    public static void UpdateBodyMode(Player self, ref Escort e)
    {
        float n = 1f;
        float p = 0f;  // Passive speed
        n += 0.6f * e.SpeGear;
        p += 0.15f * e.SpeCharge;

        if (e.SpeDashNCrash)
        {
            self.dynamicRunSpeed[0] += 3f * n;
            self.dynamicRunSpeed[1] += 3f * n;
        }
        else
        {
            self.dynamicRunSpeed[0] += p;
            self.dynamicRunSpeed[1] += p;
        }
    }


    public static void UpdateAnimation(Player self, ref Escort e)
    {
        float n = .8f + (0.45f * e.SpeGear);

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
                    self.bodyChunks[0].vel.y += .8f * self.input[0].y * n * 1.3f;
                }
                else
                {
                    self.bodyChunks[1].vel.y += .65f * self.input[0].y * n * 1.3f;
                }
            }
            if (self.animation == Player.AnimationIndex.RocketJump && self.allowRoll == 0)
            {
                self.bodyChunks[0].vel.x += self.input[0].x * n * 3;
                self.bodyChunks[1].vel.x += self.input[0].x * n * 3;
            }
        }
        else  // Trigger speed ability
        {
            bool condition;
            if (e.CustomKeybindEnabled)
            {
                condition = Input.GetKey(e.CustomKeybind);
            }
            else
            {
                condition = self.input[0].spec;
            }

            if (!ins.config.cfgDisableSpeedsterSlide.Value && self.animation == Player.AnimationIndex.BellySlide && !e.slideFromSpear && self.rollCounter > 10)
            {
                condition = true;
            }

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
                e.SpeNitrosX = e.SpeGear * 2;
                //e.SpeSpeedin = 200 + 60 * (int)Math.Pow(2, e.SpeGear);
                e.SpeExtraSpe = e.SpeSpeedin;
                SS_Fx.Gfx_Sparkle_Boom(self.room, self, e.SpeColor, 0);
                SS_Fx.Sfx_Click_Bang(self.room, e.SFXChunk, e.SpeGear);
                // if (self.room != null)
                // {
                //     for (int i = 0; i < 10; i++)
                //     {
                //         self.room.AddObject(new Spark(self.bodyChunks[1].pos + new Vector2(-10 * Mathf.Sign(self.bodyChunks[1].vel.x), 0), new Vector2(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)), e.SpeColor, null, 20, 40));
                //     }
                //     self.room.PlaySound(SoundID.Firecracker_Bang, e.SFXChunk, false, 0.5f, 1.5f + 0.2f * e.SpeGear);
                // }
            }
        }
    }


    public static void MovementUpdate(Player self, ref Escort e)
    {
        // Nitros boost
        if (e.SpeDashNCrash && e.SpeNitrosX > 0)
        {
            if (self.bodyMode == Player.BodyModeIndex.Stand ||
            self.bodyMode == Player.BodyModeIndex.Crawl)
            {
                self.bodyChunks[0].vel.x += self.input[0].x * e.SpeNitrosX * .85f;
                self.bodyChunks[1].vel.x += self.input[0].x * e.SpeNitrosX * .85f;
            }
            if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.CorridorClimb || self.bodyMode == Player.BodyModeIndex.Swimming)
            {
                self.bodyChunks[0].vel.x += self.input[0].x * e.SpeNitrosX * .7f;
                self.bodyChunks[1].vel.x += self.input[0].x * e.SpeNitrosX * .7f;
                self.bodyChunks[0].vel.y += self.input[0].y * e.SpeNitrosY * .7f;
                self.bodyChunks[1].vel.y += self.input[0].y * e.SpeNitrosY * .7f;
            }
        }

        // Slide up walls!
        // if (self.bodyMode == Player.BodyModeIndex.WallClimb && e.SpeResimo > 0 && e.SpeMomentumJump is Vector2 momentum)
        // {
        //     Ebug(self, $"{momentum}|{e.SpeResimo}");
        //     if (e.SpeResimo > 4) e.SpeResimo = 4;
        //     self.bodyChunks[0].vel.y += momentum.y * ((10 + e.SpeGear) / 6f);
        //     self.bodyChunks[1].vel.y += momentum.y * ((10 + e.SpeGear) / 6f);
        // }
    }


    public static void Jump(Player self, ref Escort e)
    {
        float n = 0.8f + (0.35f * e.SpeGear);
        if (e.SpeDashNCrash)
        {
            Ebug(self, "Speedster Jump!");
            e.SpeResimo = 40;
            Vector2 velocity = self.mainBodyChunk.vel;
            if (self.bodyMode != Player.BodyModeIndex.ZeroG && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam)
            {
                // velocity.x *= .25f;
                // velocity.y *= 3f;
            }
            e.SpeMomentumJump = velocity.normalized;

            self.jumpBoost += self.animation switch
            {
                var value when value == Player.AnimationIndex.None => 1 * n,
                var value when value == Player.AnimationIndex.Flip => 4 * n,
                var value when value == Player.AnimationIndex.Roll => 8 * n,
                var value when value == Player.AnimationIndex.SurfaceSwim => 2 * n,
                var value when value == Player.AnimationIndex.StandOnBeam => 4 * n,
                _ => 0,
            };
        }
    }


    public static void Collision(Player self, Creature creature, ref Escort e)
    {
        if (e.SpeDashNCrash && !creature.dead && Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)) > 10f)
        {
            bool checkSlide = self.animation == Player.AnimationIndex.BellySlide;
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
                self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x * ins.DKMultiplier, self.bodyChunks[0].vel.y * ins.DKMultiplier)),
                creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                Mathf.Lerp(
                    0.1f, slamDam, Mathf.InverseLerp(
                        4f, 16f, Mathf.Max(
                            Mathf.Abs(self.mainBodyChunk.vel.x),
                            Mathf.Abs(self.mainBodyChunk.vel.y)
                        )
                    )
                ), slamStun);
            SS_Fx.Sfx_Whamo(self.room, e.SFXChunk);
            // if (self.room != null)
            // {
            //     self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk, false, 2.3f, 1.2f);
            //     self.room.PlaySound(Escort_SFX_Impact, e.SFXChunk);
            // }
            float velocityX = self.bodyChunks[0].vel.x * ins.DKMultiplier * (creature.TotalMass * (checkSlide ? 0.35f : 0.5f));
            float velocityY = self.bodyChunks[0].vel.y * ins.DKMultiplier * (creature.TotalMass * (self.bodyChunks[0].vel.y > 0 ? 1.25f : 0.5f));
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


    public static void Bonk(Player self, int chunk, IntVector2 direction, float speed, bool firstContact, ref Escort e)
    {
        if (!self.dead && e.SpeDashNCrash)
        {
            float limit = 14f + (1.5f * e.SpeGear);
            if (firstContact && speed > limit && direction.x != 0 && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.animation != Player.AnimationIndex.Flip && self.animation != Player.AnimationIndex.BellySlide)
            {
                self.room?.PlaySound(SoundID.Slugcat_Terrain_Impact_Medium, e.SFXChunk);
                e.SpeBonk = 5;
            }
        }
        if (MMF.cfgWallpounce.Value && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.wantToJump > 0 && direction.x != 0 && chunk == 0 && speed > 7f && self.input[0].x == direction.x && !self.standing)
        {
            self.bodyChunks[0].vel = new Vector2(direction.x * -25f, 10f);
            self.bodyChunks[1].vel = new Vector2(direction.x * -25f, 10f);
            self.bodyChunks[0].vel.y += 6f + 4f * e.SpeGear;
            self.bodyChunks[1].vel.y += 5f + 4f * e.SpeGear;
            self.jumpStun = 15 * -direction.x;
        }
    }


    public static void WinLoseSave(ShelterDoor self, int playerNumber, bool success, ref Escort escort)
    {
        if (self.room?.game?.session is StoryGameSession storyGameSession)
        {
            storyGameSession.saveState.miscWorldSaveData.Esave().SpeChargeStore[playerNumber] = success ? escort.SpeCharge : 0;
            storyGameSession.saveState.miscWorldSaveData.Esave().SpeChargeStore.TryGetValue(playerNumber, out int charging);
            Ebug("Saved successfully to " + playerNumber + ": " + charging, LogLevel.MESSAGE);
            if (escort.shelterSaveComplete <= 1)
            {
                Ebug("Misc: " + JsonConvert.SerializeObject(storyGameSession.saveState.miscWorldSaveData.Esave()), ignoreRepetition: true);
            }
        }
    }


    public static void DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
    {
        try
        {
            if (e.SpeTrailTick == 0 && e.SpeDashNCrash && self?.player?.room is not null && self.player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut)
            {
                e.Escat_addTrail(rCam, s, (int)Mathf.Lerp(20, 40, self.player.Adrenaline), (int)Mathf.Lerp(10, 20, self.player.Adrenaline));
                e.SpeTrailTick = 2;
            }
        }
        catch (Exception err)
        {
            Ebug(self.player, err, "Speedster Draw Sprite failed!");
        }
    }
}