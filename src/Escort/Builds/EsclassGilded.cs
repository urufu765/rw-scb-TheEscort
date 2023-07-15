using BepInEx;
using SlugBase.Features;
using System;
using System.IO;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.EscortTutorial;
using RWCustom;
using MoreSlugcats;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Gilded tweak values
        // public static readonly PlayerFeature<> gilded = Player("theescort/gilded/");
        // public static readonly PlayerFeature<float> gilded = PlayerFloat("theescort/gilded/");
        // public static readonly PlayerFeature<float[]> gilded = PlayerFloats("theescort/gilded/");
        public static readonly PlayerFeature<float> gilded_float = PlayerFloat("theescort/gilded/float_speed");
        public static readonly PlayerFeature<float> gilded_lev = PlayerFloat("theescort/gilded/levitation");
        public static readonly PlayerFeature<float> gilded_jet = PlayerFloat("theescort/gilded/jetplane");
        public static readonly PlayerFeature<float> gilded_radius = PlayerFloat("theescort/gilded/pipradius");
        public static readonly PlayerFeature<float[]> gilded_position = PlayerFloats("theescort/gilded/pipposition");

        public void Esclass_GD_Tick(Player self, ref Escort e)
        {
            // // Shortcut fix for crafting
            // if (!e.GildLockRecharge && !e.GildCancel && !e.GildClearReserve && e.GildReservePower > 0 && (e.GildWantToThrow != -1 || e.GildAlsoPop))
            // {
            //     e.GildLockRecharge = true;
            // }

            if (e.GildClearReserve)
            {
                e.GildReservePower = 0;
            }


            if (e.GildLevitateLimit > 0 && e.GildFloatState && !config.cfgSectretBuild.Value)
            {
                e.GildLevitateLimit -= e.GildPowerUsage;
            }

            if (e.GildMoonJump > 0)
            {
                e.GildMoonJump--;
            }
            
            if (e.GildCrushCooldown > 0)
            {
                e.GildCrushCooldown--;
            }

            if (e.GildCrushTime > 0)
            {
                e.GildCrushTime--;
            }

            if (e.GildLevitateCooldown > 0)
            {
                e.GildLevitateCooldown--;
            }

            if (e.GildJetPackVFX > 0)
            {
                e.GildJetPackVFX--;
            }

            if (e.GildInstaCreate > 0)
            {
                e.GildInstaCreate--;
            }

            if (!e.GildLockRecharge) 
            {
                e.GildRequiredPower = 0;
                e.GildPowerUsage = 0;
                if (!self.Stunned) e.GildPower += e.GildPower < 2000? 2 : 1;
                if (e.GildReservePower == 0) e.GildStartPower = e.GildPower;
            }

            if (e.GildLockRecharge && e.GildReservePower < e.GildRequiredPower)
            {
                e.GildPower -= e.GildPowerUsage;
                e.GildReservePower += e.GildPowerUsage;
            }

            if (e.GildCancel)
            {
                if (e.GildReservePower > 100)
                {
                    e.GildPower += 100;
                    e.GildReservePower -= 100;
                }
                else if (e.GildReservePower > 0)
                {
                    e.GildPower += e.GildReservePower;
                    e.GildReservePower = 0;
                }
                else
                {
                    e.GildCancel = false;
                }
            }

            e.GildLockRecharge = self.dead;

            if (!self.input[0].thrw || e.GildClearReserve) e.GildAlsoPop = false;

            if (e.GildClearReserve) e.GildClearReserve = false;

            // if (e.secretRGB) e.Escat_RGB_firespear();
            if (!e.GildOverpowered)
            {
                e.GildOverpowered = ins.config.cfgSectretBuild.Value;

                if (self.room?.game?.session is StoryGameSession && self.room.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap == 9)
                {
                    e.GildOverpowered = true;
                }
            }
        }

        private void Esclass_GD_Update(Player self, ref Escort e)
        {
            if (
                !gilded_float.TryGet(self, out float floatingSpd) ||
                !gilded_lev.TryGet(self, out float levitation) || 
                !gilded_jet.TryGet(self, out float jetSize)
                ) return;

            // Die by overpower
            if (e.GildPower > e.GildPowerMax - 800 && !self.dead)
            {
                self.Blink(5);
                Eshelp_Player_Shaker(self, 0.7f * Mathf.InverseLerp(e.GildPowerMax - 800, e.GildPowerMax, e.GildPower));
                self.aerobicLevel = Mathf.Max(self.aerobicLevel, Mathf.InverseLerp(e.GildPowerMax - 800, e.GildPowerMax, e.GildPower));
            }
            if (e.GildPower > e.GildPowerMax && !self.dead)
            {
                self.Die();
                self.room?.AddObject(new CreatureSpasmer(self, true, 120));
                self.room?.PlaySound(SoundID.Firecracker_Bang, e.SFXChunk, false, 1f, 0.75f + UnityEngine.Random.value);
                self.room?.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, e.SFXChunk, false, 0.8f, 0.3f + UnityEngine.Random.value * 0.5f);
                for (int i = 0; i < 15; i++)
                {
                    self.room?.AddObject(new Spark(self.mainBodyChunk.pos, Custom.RNV() * UnityEngine.Random.value * 40f, e.hypeColor, null, 30, 120));
                    if (i < 10)
                    {
                        self.room?.AddObject(new WaterDrip(self.mainBodyChunk.pos, Custom.RNV() * UnityEngine.Random.value * 40f, false));
                    }
                }
                e.GildLockRecharge = true;
            }

            // Check empty hand
            bool hasSomething = false;
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i]?.grabbed is not null)
                {
                    hasSomething = true;
                    e.GildCrushCooldown = 3;
                    break;
                }
            }
            if (!hasSomething) e.GildWantToThrow = -1;


            #region Temporary levitation code
            if (self.canJump > 0 && self.canWallJump == 0)
            {
                if (self.room?.game?.session is StoryGameSession)
                {
                    e.GildLevitateLimit = 120 * (self.room.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap + 1);
                }
                else 
                {
                    e.GildLevitateLimit = 480;
                }
            }

            // Deactivate levitation
            if ((
                !self.input[0].jmp || 
                self.animation == Player.AnimationIndex.ClimbOnBeam || 
                self.animation == Player.AnimationIndex.HangFromBeam || 
                self.bodyMode == Player.BodyModeIndex.CorridorClimb ||
                e.GildLevitateLimit == 0 || 
                self.Stunned || 
                self.bodyChunks[1].contactPoint.y == -1 ||
                e.GildPower <= Escort.GildUseLevitate ||
                e.GildCrush
                ) && e.GildFloatState)
            {
                e.Escat_float_state(self, false);
                self.standing = !(self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump);
                self.wantToJump = 0;
                e.GildReservePower = 0;
            }

            // Activate levitation
            if (
                !(
                    self.animation == Player.AnimationIndex.ClimbOnBeam || 
                    self.animation == Player.AnimationIndex.HangFromBeam || 
                    self.bodyMode == Player.BodyModeIndex.ZeroG ||
                    self.bodyMode == Player.BodyModeIndex.CorridorClimb
                ) && 
                self.wantToJump > 0 && 
                self.canJump == 0 && 
                !e.GildFloatState && 
                !e.GildCrush &&
                e.GildPower >= Escort.GildUseLevitate &&
                e.GildReservePower < 10 &&
                self.bodyChunks[1].contactPoint.y != -1 &&
                (
                    e.GildLevitateCooldown <= 0 || 
                    self.input[0].jmp && !self.input[1].jmp
                )
            )
            {
                e.Escat_float_state(self);
                self.wantToJump = 0;
                e.GildRequiredPower = config.cfgSectretBuild.Value? e.GildStartPower : e.GildLevitateLimit;
                e.GildPowerUsage = Escort.GildUseLevitate;
                e.GildCrushCooldown = 10;
            }

            // Main code
            // TODO: Allow simultaneous usage of power, e.g. float while making a spear.
            if (e.GildLevitateLimit > 0 && e.GildPower > Escort.GildUseLevitate && self.input[0].jmp && e.GildFloatState)
            {
                e.GildLockRecharge = true;
                self.mainBodyChunk.vel.y = self.mainBodyChunk.vel.y < 0? Mathf.Min(self.mainBodyChunk.vel.y + floatingSpd, 0) : Mathf.Max(self.mainBodyChunk.vel.y - floatingSpd, 0);
                self.airFriction = 0.8f;
                self.standing = false;
                self.buoyancy = 0f;
                self.bodyChunks[0].vel.y += levitation;
                self.bodyChunks[1].vel.y += levitation - 1f;
                if (Esconfig_SFX(self) && e.GildJetPackVFX == 0)
                {
                    e.GildJetPackVFX += UnityEngine.Random.Range(5, 21);
                    /*
                    self.room?.AddObject(new MoreSlugcats.VoidParticle(self.bodyChunks[1].pos + new Vector2(-4, 0), new Vector2(5f * self.input[0].x, (-30f + e.GildJetPackVFX)/5), 20f));
                    self.room?.AddObject(new MoreSlugcats.VoidParticle(self.bodyChunks[1].pos + new Vector2(4, 0), new Vector2(5f * self.input[0].x, (-30f + e.GildJetPackVFX)/5), 20f));
                    */
                    self.room?.AddObject(new Explosion.FlashingSmoke(self.bodyChunks[1].pos + new Vector2(-4, 0), new Vector2(-5f * self.input[0].x, (-30f + e.GildJetPackVFX)/4), jetSize, e.hypeColor, Color.black, 30));
                    self.room?.AddObject(new Explosion.FlashingSmoke(self.bodyChunks[1].pos + new Vector2(4, 0), new Vector2(-5f * self.input[0].x, (-30f + e.GildJetPackVFX)/4), jetSize, e.hypeColor, Color.black, 30));

                }
            }
            #endregion


            #region Moonjump & Crush
            if (!(self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG || self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam))
            {
                // Moonjump
                if (!e.GildCrush)
                {
                    self.bodyChunks[0].vel.y += Mathf.Lerp(
                        0, 
                        levitation * (self.animation == Player.AnimationIndex.Flip? 1.5f : 1f), 
                        Mathf.InverseLerp(0, e.GildMoonJumpMax, e.GildMoonJump)
                    );
                }

                // Crush part 1
                if (!hasSomething && !e.GildCrush && (e.GildMoonJump < e.GildMoonJumpMax - 5 || e.GildFloatState) && self.bodyChunks[1].contactPoint.y != -1 && self.input[0].thrw && !self.input[1].thrw && e.GildCrushCooldown == 0 && self.dontGrabStuff == 0)
                {
                    e.GildCrush = true;
                    e.GildMoonJump = 0;
                    e.GildCrushTime = 30;
                    self.room?.PlaySound(Escort_SFX_Gild_Stomp, e.SFXChunk);
                }
            }

            // Crush part 2
            if (e.GildCrush)
            {
                // Successful stomp
                if (self.bodyChunks[1].contactPoint.y == -1 || self.bodyChunks[1].contactPoint.x != 0)
                {  // Land on surface/creature
                    self.bodyChunks[0].vel.y = Mathf.Max(self.bodyChunks[0].vel.y, 0);
                    self.bodyChunks[1].vel.y = Mathf.Max(self.bodyChunks[1].vel.y, 1);
                    self.impactTreshhold = 1f;
                    e.GildCrush = false;
                    self.room?.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk);
                }
                else if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG)
                {  // Land on water/zeroG/pole
                    self.bodyChunks[0].vel.y /= 10;
                    self.bodyChunks[1].vel.y /= 10;
                    self.impactTreshhold = 1f;
                    e.GildCrush = false;
                }
                else if (e.GildCrushTime > 0)
                {
                    self.bodyChunks[0].vel.y = levitation;
                    self.bodyChunks[1].vel.y = levitation;
                    self.bodyChunks[0].vel.x /= 2;
                    self.bodyChunks[1].vel.x /= 2;
                }
                else  
                {  // Flight downwards
                    self.impactTreshhold = 200f;
                    self.bodyChunks[1].vel.y -= 4f;
                }
            }
            #endregion
        }


        private static void Esclass_GD_GrabUpdate(Player self, bool eu, ref Escort e)
        {
            // Throw when letting go of the held button
            if (!self.input[0].thrw && e.GildWantToThrow != -1)
            {
                self.ThrowObject(e.GildWantToThrow, eu);
                e.GildCancel = true;
                e.GildWantToThrow = -1;
            }
            else if (e.GildWantToThrow != -1 || e.GildAlsoPop)
            {
                int grabby = e.GildAlsoPop? 1 : e.GildWantToThrow;
                if (self.grasps[grabby]?.grabbed is null) return;
                if ((self.grasps[grabby].grabbed is Rock && e.GildStartPower >= Escort.GildCheckCraftFirebomb * 2) || (self.grasps[grabby].grabbed is ScavengerBomb && e.GildStartPower >= Escort.GildCheckCraftFirebomb))
                {
                    e.GildRequiredPower = Escort.GildCheckCraftFirebomb * (self.grasps[grabby].grabbed is Rock? 2 : 1);
                    e.GildPowerUsage = Escort.GildUseCraftFirebomb;
                    try
                    {
                        if (e.GildReservePower >= e.GildRequiredPower)
                        {
                            Vector2 posi = new();
                            WorldCoordinate wPos = new(); 
                            Rock r = null;
                            ScavengerBomb b = null;
                            if (self.grasps[grabby].grabbed is Rock) 
                            {
                                r = self.grasps[grabby].grabbed as Rock;
                                posi = r.firstChunk.pos;
                                wPos = r.abstractPhysicalObject.pos;
                            }
                            else
                            {
                                b = self.grasps[grabby].grabbed as ScavengerBomb;
                                posi = b.firstChunk.pos;
                                wPos = b.abstractPhysicalObject.pos;
                            }
                            Color.RGBToHSV(e.hypeColor, out float hue, out float _, out float _);
                            Ebug(self, "Rock init");
                            self.ReleaseGrasp(grabby);
                            Ebug(self, "throwaway");
                            r?.Destroy();
                            b?.Destroy();
                            Ebug(self, "Destroy");
                            FireEgg.AbstractBugEgg apo = new(self.abstractCreature.world, null, wPos, self.room.game.GetNewID(), hue + 0.5f);
                            self.room.abstractRoom.AddEntity(apo);
                            apo.RealizeInRoom();
                            apo.realizedObject.firstChunk.HardSetPosition(posi);
                            self.room?.PlaySound(SoundID.Water_Nut_Swell, posi);
                            self.room?.PlaySound(SoundID.Fire_Spear_Pop, posi, 0.5f, 1.1f);
                            self.room?.AddObject(new Explosion.ExplosionLight(posi, 200f, 0.7f, 7, e.hypeColor));
                            self.room?.AddObject(new ExplosionSpikes(self.room, posi, 8, 15f, 9f, 5f, 90f, e.hypeColor));
                            if (e.GildInstaCreate > 0) 
                            {
                                self.SlugcatGrab(apo.realizedObject, grabby);
                            }

                            if (
                                e.GildWantToThrow == 0 && 
                                self.grasps[1]?.grabbed is not null && 
                                !e.GildAlsoPop &&
                                (
                                    self.grasps[1].grabbed is Rock && e.GildPower >= Escort.GildCheckCraftFirebomb * 2 ||
                                    self.grasps[1].grabbed is ScavengerBomb && e.GildPower >= Escort.GildCheckCraftFirebomb || 
                                    self.grasps[1].grabbed is Spear sp && !sp.bugSpear && e.GildPower >= Escort.GildCheckCraftFirespear
                                )
                            ) 
                            {
                                e.GildAlsoPop = true;
                                e.GildClearReserve = true;
                            }
                            e.GildWantToThrow = -1;
                            e.GildReservePower = 0;
                            self.Blink(10);
                            return;
                        }
                        else
                        {
                            e.GildLockRecharge = true;
                            if (self.grasps[grabby].grabbed is Rock r) 
                            {
                                r.vibrate = e.GildReservePower * 20 / Escort.GildCheckCraftFirebomb;
                            }
                            else if (self.grasps[grabby].grabbed is ScavengerBomb b)
                            {
                                b.vibrate = e.GildReservePower * 20 / Escort.GildCheckCraftFirebomb;
                            }
                        }
                    } 
                    catch (NullReferenceException nre)
                    {
                        Ebug(self, nre, "Null when charging a rock!");
                    }
                    catch (Exception err) {
                        Ebug(self, err, "Generic exception when charging a rock!");
                    }
                }
                if (self.grasps[grabby].grabbed is Spear s && !s.bugSpear && e.GildStartPower >= Escort.GildCheckCraftFirespear)
                {
                    e.GildRequiredPower = Escort.GildCheckCraftFirespear;
                    e.GildPowerUsage = Escort.GildUseCraftFirespear;
                    try
                    {
                        if (e.GildReservePower >= e.GildRequiredPower)
                        {
                            Vector2 posi = s.firstChunk.pos;
                            WorldCoordinate wPos = s.abstractPhysicalObject.pos;
                            //float hue = Mathf.Lerp(0.35f, 0.6f, Custom.ClampedRandomVariation(0.5f, 0.5f, 2f));
                            Color.RGBToHSV(e.hypeColor, out float hue, out float _, out float _);
                            self.ReleaseGrasp(grabby);
                            s.Destroy();
                            AbstractSpear apo = new(self.abstractCreature.world, null, wPos, self.room.game.GetNewID(), false, hue + 0.5f);
                            self.room.abstractRoom.AddEntity(apo);
                            apo.RealizeInRoom();
                            self.room?.PlaySound(SoundID.Fire_Spear_Pop, posi, 0.7f, 1f);
                            self.room?.AddObject(new Explosion.ExplosionLight(posi, 200f, 0.7f, 7, e.hypeColor));
                            self.room?.AddObject(new ExplosionSpikes(self.room, posi, 10, 15f, 9f, 5f, 90f, e.hypeColor));
                            self.SlugcatGrab(apo.realizedObject, grabby);

                            // Doesn't work
                            #if false
                            if (e.secretRGB)
                            {
                                e.GildRainbowFirespear.Add(apo.realizedObject as Spear);
                            }
                            #endif
                            if (
                                e.GildWantToThrow == 0 && 
                                self.grasps[1]?.grabbed is not null && 
                                !e.GildAlsoPop &&
                                (
                                    self.grasps[1].grabbed is Rock && e.GildPower >= Escort.GildCheckCraftFirebomb * 2 ||
                                    self.grasps[1].grabbed is ScavengerBomb && e.GildPower >= Escort.GildCheckCraftFirebomb || 
                                    self.grasps[1].grabbed is Spear sp && !sp.bugSpear && e.GildPower >= Escort.GildCheckCraftFirespear
                                )
                            ) 
                            {
                                e.GildAlsoPop = true;
                                e.GildClearReserve = true;
                            }
                            e.GildWantToThrow = -1;
                            e.GildReservePower = 0;
                            self.Blink(10);
                            return;
                        }
                        else
                        {
                            e.GildLockRecharge = true;
                            s.vibrate = e.GildReservePower * 20 / Escort.GildCheckCraftFirespear;
                        }
                    } 
                    catch (NullReferenceException nre)
                    {
                        Ebug(self, nre, "Null when charging a spear!");
                    }
                    catch (Exception err) {
                        Ebug(self, err, "Generic exception when charging a spear!");
                    }

                }
                if (e.GildOverpowered && (self.grasps[grabby].grabbed is FireEgg || self.grasps[grabby].grabbed is Spear spr && spr.bugSpear) && e.GildStartPower >= Escort.GildCheckCraftSingularity)
                {
                    e.GildRequiredPower = Escort.GildCheckCraftSingularity;
                    e.GildPowerUsage = Escort.GildUseCraftSingularity;
                    try
                    {
                        if (e.GildReservePower >= e.GildRequiredPower)
                        {
                            Vector2 posi = new();
                            WorldCoordinate wPos = new(); 
                            Spear spear = null;
                            FireEgg fireEgg = null;
                            if (self.grasps[grabby].grabbed is Spear) 
                            {
                                spear = self.grasps[grabby].grabbed as Spear;
                                posi = spear.firstChunk.pos;
                                wPos = spear.abstractPhysicalObject.pos;
                            }
                            else
                            {
                                fireEgg = self.grasps[grabby].grabbed as FireEgg;
                                posi = fireEgg.firstChunk.pos;
                                wPos = fireEgg.abstractPhysicalObject.pos;
                            }
                            Color.RGBToHSV(e.hypeColor, out float hue, out float _, out float _);
                            Ebug(self, "Singularity init");
                            self.ReleaseGrasp(grabby);
                            Ebug(self, "throwaway");
                            spear?.Destroy();
                            fireEgg?.Destroy();
                            Ebug(self, "Destroy");
                            AbstractPhysicalObject apo = new(self.abstractCreature.world, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null, wPos, self.room.game.GetNewID());
                            self.room.abstractRoom.AddEntity(apo);
                            apo.RealizeInRoom();
                            apo.realizedObject.firstChunk.HardSetPosition(posi);
                            self.room?.PlaySound(SoundID.Water_Nut_Swell, posi);
                            self.room?.PlaySound(SoundID.Fire_Spear_Pop, posi, 0.5f, 1.1f);
                            self.room?.AddObject(new Explosion.ExplosionLight(posi, 200f, 0.7f, 7, e.hypeColor));
                            self.room?.AddObject(new ExplosionSpikes(self.room, posi, 8, 15f, 9f, 5f, 90f, e.hypeColor));
                            self.SlugcatGrab(apo.realizedObject, grabby);
                            e.GildWantToThrow = -1;
                            e.GildReservePower = 0;
                            self.Blink(10);
                            return;
                        }
                        else
                        {
                            e.GildLockRecharge = true;
                            if (self.grasps[grabby].grabbed is FireEgg fe) 
                            {
                                fe.firstChunk.vel += new Vector2(
                                    Mathf.Lerp(0, UnityEngine.Random.Range(-3f, 3f), e.GildReservePower / Escort.GildCheckCraftSingularity), 
                                    Mathf.Lerp(0, UnityEngine.Random.Range(-3f, 3f), e.GildReservePower / Escort.GildCheckCraftSingularity)
                                );
                            }
                            else if (self.grasps[grabby].grabbed is Spear spearie)
                            {
                                spearie.vibrate = e.GildReservePower * 20 / Escort.GildCheckCraftFirebomb;
                            }
                        }
                    } 
                    catch (NullReferenceException nre)
                    {
                        Ebug(self, nre, "Null when charging a rock!");
                    }
                    catch (Exception err) {
                        Ebug(self, err, "Generic exception when charging a rock!");
                    }
                }

            }
        }


        private static void Esclass_GD_Breathing(Player self, float f)
        {
            self.aerobicLevel = Mathf.Min(1f, self.aerobicLevel + (f / 10f));
        }

        private static void Esclass_GD_Jump(Player self, ref Escort e)
        {
            if (self.standing)
            {
                e.GildMoonJump = e.GildMoonJumpMax;
                e.GildLevitateCooldown = 5;
            }
        }

        /// <summary>
        /// Crush a creature
        /// </summary>
        private void Esclass_GD_Collision(Player self, Creature creature, ref Escort e)
        {
            if (e.GildCrush && e.GildCrushTime == 0){
                creature.SetKillTag(self.abstractCreature);
                creature.LoseAllGrasps();
                float dam = Mathf.Lerp(0, 5, Mathf.InverseLerp(0, 35, Mathf.Abs(self.bodyChunks[0].vel.y)));
                creature.Violence(
                    self.bodyChunks[1], 
                    new Vector2(self.bodyChunks[1].vel.x, self.bodyChunks[1].vel.y * -1 * DKMultiplier),
                    creature.mainBodyChunk, null,
                    Creature.DamageType.Blunt,
                    dam,
                    30
                );
                self.room?.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk, false, 1f, 1.1f);
                self.room?.PlaySound(Escort_SFX_Impact, e.SFXChunk);
                self.bodyChunks[0].vel.y = Mathf.Max(self.bodyChunks[0].vel.y, 0);
                self.bodyChunks[1].vel.y = Mathf.Max(self.bodyChunks[1].vel.y, 1);
                self.impactTreshhold = 1f;
                e.GildCrush = false;
                e.GildCrushReady = false;
                Ebug(self, "Stomp! Damage: " + dam);
            }
        }

        /// <summary>
        /// Half the duration of the slide because fuck you.
        /// </summary>
        private void Esclass_GD_UpdateAnimation(Player self)
        {
            if (self.animation == Player.AnimationIndex.BellySlide)
            {
                if (self.rollCounter < 7)
                {
                    self.rollCounter = 7;
                }
                if (self.initSlideCounter < 3)
                {
                    self.initSlideCounter = 3;
                }
            }

        }

        /// <summary>
        /// Prevent player from throwing a rock or spear when they tap the throw button, instead throwing on letting go of the button such that holding the throw button lets the player craft a firebomb or firespear. If let go, make Gilded toss object instead.
        /// </summary>
        private bool Esclass_GD_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort escort)
        {
            if (self.grasps[grasp]?.grabbed is null) return false;

            if (
                self.grasps[grasp].grabbed is Rock or ScavengerBomb || 
                self.grasps[grasp].grabbed is Spear spear && (escort.GildOverpowered || !spear.bugSpear) || 
                self.grasps[grasp].grabbed is MoreSlugcats.LillyPuck ||
                escort.GildOverpowered && self.grasps[grasp].grabbed is MoreSlugcats.FireEgg
            )
            {
                if (!self.input[0].thrw || self.grasps[grasp].grabbed is MoreSlugcats.LillyPuck)
                {
                    if (self.grasps[grasp].grabbed is ScavengerBomb || (escort.GildOverpowered && (self.grasps[grasp].grabbed is FireEgg || self.grasps[grasp].grabbed is Spear sp && sp.bugSpear)))
                    {
                        orig(self, grasp, eu);  // Regular throw
                    }
                    else
                    {
                        self.TossObject(grasp, eu);
                        Esclass_GD_ReplicateThrowBodyPhysics(self, grasp);
                        self.dontGrabStuff = 15;
                        self.ReleaseGrasp(grasp);
                    }
                }
                else
                {
                    escort.GildWantToThrow = grasp;
                }
                return true;
            }
            return false;
        }

        private static void Esclass_GD_ReplicateThrowBodyPhysics(Player self, int grasp)
        {
            IntVector2 throwDir = new(self.ThrowDirection, 0);
            bool upwardsEnabled = ModManager.MMF && MoreSlugcats.MMF.cfgUpwardsSpearThrow.Value;
            if (
                self.animation == Player.AnimationIndex.Flip &&
                (self.input[0].y < 0 || (upwardsEnabled && self.input[0].y != 0)) &&
                self.input[0].x == 0
            )
            {
                throwDir = new(0, upwardsEnabled? self.input[0].y : -1);
            }
            if (upwardsEnabled && self.bodyMode == Player.BodyModeIndex.ZeroG && self.input[0].y != 0)
            {
                throwDir = new(0, self.input[0].y);
            }


            if (
                self.animation == Player.AnimationIndex.BellySlide && 
                self.rollCounter > 8 &&
                self.rollCounter < 15 &&
                throwDir.x == -self.rollDirection &&
                !self.longBellySlide
            )
            {
                self.grasps[grasp].grabbed.firstChunk.vel.y += 4;
                (self.grasps[grasp].grabbed as Weapon).changeDirCounter = 0;
            }

            if (self.animation == Player.AnimationIndex.ClimbOnBeam && ModManager.MMF && MoreSlugcats.MMF.cfgClimbingGrip.Value)
            {
                self.bodyChunks[0].vel += throwDir.ToVector2() * 2f;
                self.bodyChunks[1].vel -= throwDir.ToVector2() * 8f;
            }
            else
            {
                self.bodyChunks[0].vel += throwDir.ToVector2() * 8f;
                self.bodyChunks[1].vel -= throwDir.ToVector2() * 4f;
            }

            if (self.graphicsModule is PlayerGraphics playerGraphics)
            {
                playerGraphics.ThrowObject(grasp, self.grasps[grasp].grabbed);
            }
            self.Blink(15);
        }


        /// <summary>
        /// Initiates the pips that will be shown for the amount of power left
        /// </summary>
        private static void Esclass_GD_InitiateSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera roomCamera, ref Escort escort)
        {
            try
            {
                escort.Escat_setIndex_sprite_cue(ref escort.GildPowerPipsIndex, s.sprites.Length);
                Ebug("Set cue for Gilded sprites");
                Array.Resize(ref s.sprites, s.sprites.Length + escort.GildPowerPipsMax);
                for (int i = 0; i < escort.GildPowerPipsMax; i++)
                {
                    s.sprites[escort.GildPowerPipsIndex + i] = new FSprite("WormEye");
                }
                Ebug(self.player, "Applied the PIPS");
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null reference when initiating Gilded sprites");
            }
            catch (IndexOutOfRangeException ioore)
            {
                Ebug(ioore, "Index out of bounds when initiating Gilded sprites");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when initiating Gilded sprites");
            }
        }

        /// <summary>
        /// Puts the power pips in the hud layer
        /// </summary>
        private static void Esclass_GD_AddTaCantaina(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera r, ref Escort escort)
        {
            try
            {
                if (escort.GildPowerPipsIndex + escort.GildPowerPipsMax <= s.sprites.Length)
                {
                    for (int i = escort.GildPowerPipsIndex; i < escort.GildPowerPipsIndex + escort.GildPowerPipsMax; i++)
                    {
                        r.ReturnFContainer("Foreground").RemoveChild(s.sprites[i]);
                        r.ReturnFContainer("HUD2").AddChild(s.sprites[i]);
                    }
                    Ebug(self.player, "Power pips relocated");
                }
                else
                {
                    Ebug(self.player, "Uh oh, something went wrong while allocating power pips");
                }
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null reference when containering Gilded sprites");
            }
            catch (IndexOutOfRangeException ioore)
            {
                Ebug(ioore, "Index out of bounds when containering Gilded sprites");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when containering Gilded sprites");
            }

        }

        /// <summary>
        /// Draws the power pips
        /// </summary>
        private static void Esclass_GD_DrawPipSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera roomCamera, float timeStacker, Vector2 cameraPos, ref Escort escort)
        {
            if (
                !gilded_radius.TryGet(self.player, out float pipRad) ||
                !gilded_position.TryGet(self.player, out float[] pipPos)
            ) return;
            try
            {
                if (escort.GildPowerPipsIndex + escort.GildPowerPipsMax <= s.sprites.Length)
                {
                    float division = escort.GildPowerMax / (float)escort.GildPowerPipsMax;
                    float minReq = escort.GildStartPower - escort.GildRequiredPower;
                    for (int i = 0; i < escort.GildPowerPipsMax; i++)
                    {
                        // Visibility
                        float preR = i > 0? division * (float)(i - 1) : 0;
                        float minR = division * (float)i;
                        float maxR = division * (float)(i + 1);
                        s.sprites[escort.GildPowerPipsIndex + i].scale = Mathf.InverseLerp(minR, maxR, escort.GildPower);
                        /*
                        s.sprites[escort.GildPowerPipsIndex + i].scale = (float)escort.GildPower switch {
                            var a when a <= minR => 0f,
                            var a when a >= maxR => 1f,
                            _ => (escort.GildPower - minR) / division
                        };*/

                        // Color
                        if (escort.GildRequiredPower != 0 && maxR >= minReq)
                        {
                            s.sprites[escort.GildPowerPipsIndex + i].color = Color.white;
                        }
                        else 
                        {
                            Color wawa = new(escort.hypeColor.r, escort.hypeColor.g, escort.hypeColor.b, 1f);
                            s.sprites[escort.GildPowerPipsIndex + i].color = wawa;
                        }
                        s.sprites[escort.GildPowerPipsIndex + i].alpha = 1f;

                        // Location
                        Vector2 loc = new Vector2(s.sprites[3].x + pipPos[0], s.sprites[3].y + pipPos[1]) + Custom.rotateVectorDeg(Vector2.one * pipRad, i * (360f / escort.GildPowerPipsMax));
                        s.sprites[escort.GildPowerPipsIndex + i].x = loc.x;
                        s.sprites[escort.GildPowerPipsIndex + i].y = loc.y;
                    }

                }
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null reference when drawing Gilded sprites");
            }
            catch (IndexOutOfRangeException ioore)
            {
                Ebug(ioore, "Index out of bounds when drawing Gilded sprites");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when drawing Gilded sprites");
            }

        }


        /// <summary>
        /// Let Gilded survive the acid pools, based on the hype meter. Recharges a bit of gilded power.
        /// </summary>
        private static void Esclass_GD_Die(ref Escort escort)
        {
            if (escort.GildReservePower > 0 && !escort.GildFloatState) 
            {
                escort.GildReservePower = escort.GildRequiredPower;
                escort.GildInstaCreate = 5;
            }
            else if (escort.acidRepetitionGuard == 0)
            {
                escort.GildPower += Math.Min(800, (escort.GildPowerMax - escort.GildPower) / 2);
            }
        }

        /// <summary>
        /// Kills a guardian in rubicon with just one singularity bomb so it isn't required for them to source and craft three just to get past.
        /// </summary>
        private void Esclass_GD_KillGuardianWithOneHit(On.MoreSlugcats.HRGuardManager.orig_Update orig, MoreSlugcats.HRGuardManager self, bool eu)
        {
            try
            {
                // Unsure if this will work properly in jolly coop
                if (self.myGuard is not null && self.myPlayer is not null && self.room?.game?.cameras[0]?.followAbstractCreature?.Room is not null && self.room.game.cameras[0].followAbstractCreature.Room == self.room.abstractRoom)
                {
                    if (eCon.TryGetValue(self.myPlayer, out Escort escort) && escort.Gilded)
                    {
                        Ebug(self.myPlayer, "Found Gilded!", 4);
                        if (self.myGuard.state.dead && self.myGuard.realizedCreature.killTag == self.myPlayer.abstractCreature && self.hitsToKill > 1)
                        {
                            Ebug(self.myPlayer, "Set killrequirement to 1");
                            self.hitsToKill = 1;
                        }

                        if (self.triggered && self.room?.game?.session is StoryGameSession storyGameSession && !storyGameSession.saveState.deathPersistentSaveData.Etut(GildKillGuardian))
                        {
                            self.room.game.cameras[0].hud.textPrompt.AddMessage(Swapper(Custom.rainWorld.inGameTranslator.Translate("gilded_tut_guardian")), 20, 300, true, true);
                            storyGameSession.saveState.deathPersistentSaveData.Etut(GildKillGuardian, true);
                        }
                    }
                }
                orig(self, eu);
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null error when killing guardian with one hit");
                orig(self, eu);
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when killing guardian with one hit");
                orig(self, eu);
            }
        }

    }

}
