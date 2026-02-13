using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster.Old;

public static class OldSpeedster
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
    }

    public static void Update(Player self, ref Escort e)
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
            e.SpeSpeedin += v switch
            {
                > 13f => 10,
                > 9f => 4,
                > 5.5f => 1,
                var _ when Mathf.Abs(self.mainBodyChunk.vel.y) <= 4.5f => -1,
                _ => 0
            };
            // switch (v)
            // {
            //     case var _ when v > 13f:
            //         e.SpeSpeedin += 10;
            //         break;
            //     case var _ when v > 9f:
            //         e.SpeSpeedin += 4;
            //         break;
            //     case var _ when v > 5.5f:
            //         e.SpeSpeedin++;
            //         break;
            //     default:
            //         if (Mathf.Abs(self.mainBodyChunk.vel.y) <= 4.5f)
            //         {
            //             e.SpeSpeedin--;
            //         }
            //         break;
            // }
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


    public static void UpdateBodyMode(Player self, ref Escort e)
    {
        float n = e.SpeSecretSpeed ? 2.5f : 1f;

        if (e.SpeDashNCrash)
        {
            self.dynamicRunSpeed[0] += 3f * n;
            self.dynamicRunSpeed[1] += 3f * n;
        }
    }


    public static void UpdateAnimation(Player self, ref Escort e)
    {
        float n = e.SpeSecretSpeed ? 1.6f : 0.8f;
        /*  # I'll come back to this
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
        */

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
                //self.dynamicRunSpeed[0] += 2f * n;
                //self.dynamicRunSpeed[1] += 2f * n;
                self.bodyChunks[0].vel.x += self.input[0].x * n * 3;
                self.bodyChunks[1].vel.x += self.input[0].x * n * 3;
            }
        }
    }


    public static void Jump(Player self, ref Escort e)
    {
        float n = e.SpeSecretSpeed ? 1.3f : 0.8f;
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
            creature.SetKillTag(self.abstractCreature);
            creature.LoseAllGrasps();
            creature.Violence(
                self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x * ins.DKMultiplier, self.bodyChunks[0].vel.y * ins.DKMultiplier)),
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
            creature.firstChunk.vel.x = self.bodyChunks[0].vel.x * ins.DKMultiplier * (creature.TotalMass * (checkSlide ? 1f : 0.5f));
            creature.firstChunk.vel.y = self.bodyChunks[0].vel.y * ins.DKMultiplier * (creature.TotalMass * (checkSlide ? 1f : 0.5f));
            //self.WallJump(-self.flipDirection);
            if (!checkSlide)
            {
                self.bodyChunks[0].vel.x *= -(e.SpeSecretSpeed ? 2.5f : 1.5f);
                self.bodyChunks[1].vel.x *= -(e.SpeSecretSpeed ? 1.5f : 1f);
                self.Stun(e.SpeSecretSpeed ? 160 : 60);
            }
        }
    }


    public static void Bonk(Player self, int chunk, IntVector2 direction, float speed, bool firstContact, ref Escort e)
    {
        // if (speed > 35)
        // {
        //     Ebug("Terrain Impact Speed @ " + speed, LogLevel.INFO, ignoreRepetition: true);
        // }
        if (!self.dead && e.SpeDashNCrash)
        {
            float limit = e.SpeSecretSpeed? 16f : 14f;
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
            self.jumpStun = 15 * -direction.x;
        }
    }


    public static void DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
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

}