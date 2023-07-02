using BepInEx;
using SlugBase.Features;
using System;
using System.IO;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
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


        public void Esclass_GD_Tick(Player self, ref Escort e)
        {
            if (e.GildLevitateLimit > 0 && e.GildFloatState)
            {
                e.GildLevitateLimit--;
            }

            if (e.GildMoonJump > 0)
            {
                e.GildMoonJump--;
            }
            
            if (e.GildCrushCooldown > 0)
            {
                e.GildCrushCooldown--;
            }

            if (e.GildLevitateCooldown > 0)
            {
                e.GildLevitateCooldown--;
            }

            if (!e.GildLockRecharge) 
            {
                e.GildRequiredPower = 0;
                e.GildPowerUsage = 0;
                if (!self.Stunned) e.GildPower++;
                e.GildStartPower = e.GildPower;
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
            if (!self.dead) e.GildLockRecharge = false;

            // if (e.secretRGB) e.Escat_RGB_firespear();
        }

        private void Esclass_GD_Update(Player self, ref Escort e)
        {
            if (
                !gilded_float.TryGet(self, out float floatingSpd) ||
                !gilded_lev.TryGet(self, out float levitation)
                ) return;

            // Die by overpower
            if (e.GildPower > 4600 && !self.dead)
            {
                self.Blink(5);
                Eshelp_Player_Shaker(self, Mathf.Lerp(0.3f, 1.3f, Mathf.InverseLerp(4600, 5000, e.GildPower)));
                self.aerobicLevel = Mathf.Max(self.aerobicLevel, Mathf.InverseLerp(4600, 5000, e.GildPower));
            }
            if (e.GildPower > 5000 && !self.dead)
            {
                self.Die();
                self.room?.AddObject(new CreatureSpasmer(self, true, 120));
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
            if (self.canJump > 0) e.GildLevitateLimit = 120;

            // Deactivate levitation
            if ((
                !self.input[0].jmp || 
                self.animation == Player.AnimationIndex.ClimbOnBeam || 
                self.animation == Player.AnimationIndex.HangFromBeam || 
                e.GildLevitateLimit == 0 || 
                self.Stunned || 
                self.bodyChunks[1].contactPoint.y == -1 ||
                e.GildPower <= Escort.GildUseLevitate ||
                e.GildCrush
                ) && e.GildFloatState)
            {
                e.Escat_float_state(self, false);
                self.wantToJump = 0;
                e.GildReservePower = 0;
            }

            // Activate levitation
            if (
                !(
                    self.animation == Player.AnimationIndex.ClimbOnBeam || 
                    self.animation == Player.AnimationIndex.HangFromBeam || 
                    self.bodyMode == Player.BodyModeIndex.ZeroG
                ) && 
                self.wantToJump > 0 && 
                self.canJump == 0 && 
                !e.GildFloatState && 
                !e.GildCrush &&
                e.GildPower >= Escort.GildUseLevitate &&
                self.bodyChunks[1].contactPoint.y != -1 &&
                (
                    e.GildLevitateCooldown <= 0 || 
                    self.input[0].jmp && !self.input[1].jmp
                )
            )
            {
                e.Escat_float_state(self);
                self.wantToJump = 0;
                e.GildRequiredPower = Escort.GildCheckLevitate;
                e.GildPowerUsage = Escort.GildUseLevitate;
                e.GildCrushCooldown = 10;
            }

            // Main code
            if (e.GildLevitateLimit > 0 && e.GildPower > Escort.GildUseLevitate && self.input[0].jmp && e.GildFloatState)
            {
                e.GildLockRecharge = true;
                self.mainBodyChunk.vel.y = self.mainBodyChunk.vel.y < 0? Mathf.Min(self.mainBodyChunk.vel.y + floatingSpd, 0) : Mathf.Max(self.mainBodyChunk.vel.y - floatingSpd, 0);
                self.airFriction = 0.8f;
                self.standing = false;
                self.buoyancy = 0f;
                self.bodyChunks[0].vel.y += levitation;
                self.bodyChunks[1].vel.y += levitation - 1f;
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
                if (!hasSomething && !e.GildCrush && (e.GildMoonJump < e.GildMoonJumpMax - 5 || e.GildFloatState) && self.bodyChunks[1].contactPoint.y != -1 && self.input[0].thrw && !self.input[1].thrw && e.GildCrushCooldown == 0)
                {
                    e.GildCrush = true;
                    e.GildMoonJump = 0;
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
                else  
                {  // Flight downwards
                    self.impactTreshhold = 200f;
                    self.bodyChunks[1].vel.y -= 3f;
                }
            }
            #endregion
        }


        private static void Esclass_GD_GrabUpdate(Player self, bool eu, ref Escort e)
        {
            if (e.GildWantToThrow != -1)
            {
                // Throw when letting go of the held button
                if (!self.input[0].thrw)
                {
                    self.ThrowObject(e.GildWantToThrow, eu);
                    e.GildCancel = true;
                    e.GildWantToThrow = -1;
                }
                else
                {
                    if (self.grasps[e.GildWantToThrow]?.grabbed is null) return;
                    if (self.grasps[e.GildWantToThrow].grabbed is Rock r && e.GildStartPower >= Escort.GildCheckCraftFirebomb)
                    {
                        e.GildRequiredPower = Escort.GildCheckCraftFirebomb;
                        e.GildPowerUsage = Escort.GildUseCraftFirebomb;
                        try
                        {
                            if (e.GildReservePower >= e.GildRequiredPower)
                            {
                                Vector2 posi = r.firstChunk.pos;
                                WorldCoordinate wPos = r.abstractPhysicalObject.pos;
                                Color.RGBToHSV(e.hypeColor, out float hue, out float sat, out float vib);
                                Ebug(self, "Rock init");
                                self.ReleaseGrasp(e.GildWantToThrow);
                                Ebug(self, "throwaway");
                                r.Destroy();
                                Ebug(self, "Destroy");
                                FireEgg.AbstractBugEgg apo = new(self.abstractCreature.world, null, wPos, self.room.game.GetNewID(), hue + 0.5f);
                                self.room.abstractRoom.AddEntity(apo);
                                apo.RealizeInRoom();
                                apo.realizedObject.firstChunk.HardSetPosition(posi);
                                self.room.PlaySound(SoundID.Water_Nut_Swell, posi);
                                e.GildWantToThrow = -1;
                                e.GildReservePower = 0;
                                return;
                            }
                            else
                            {
                                e.GildLockRecharge = true;
                                r.vibrate = e.GildReservePower * 20 / Escort.GildCheckCraftFirebomb;
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
                    if (self.grasps[e.GildWantToThrow].grabbed is Spear s && !s.bugSpear && e.GildStartPower >= Escort.GildCheckCraftFirespear)
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
                                Color.RGBToHSV(e.hypeColor, out float hue, out float sat, out float vib);
                                self.ReleaseGrasp(e.GildWantToThrow);
                                s.Destroy();
                                AbstractSpear apo = new(self.abstractCreature.world, null, wPos, self.room.game.GetNewID(), false, hue + 0.5f);
                                self.room.abstractRoom.AddEntity(apo);
                                apo.RealizeInRoom();
                                self.room.PlaySound(SoundID.Fire_Spear_Pop, posi, 0.7f, 1f);
                                self.SlugcatGrab(apo.realizedObject, e.GildWantToThrow);

                                // Doesn't work
                                #if false
                                if (e.secretRGB)
                                {
                                    e.GildRainbowFirespear.Add(apo.realizedObject as Spear);
                                }
                                #endif
                                e.GildWantToThrow = -1;
                                e.GildReservePower = 0;
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
            if (e.GildCrush){
                creature.SetKillTag(self.abstractCreature);
                creature.LoseAllGrasps();
                float dam = Mathf.Lerp(0, 5, Mathf.InverseLerp(0, 50, Mathf.Abs(self.bodyChunks[0].vel.y)));
                creature.Violence(
                    self.bodyChunks[1], 
                    new Vector2?(new Vector2(self.bodyChunks[1].vel.x, self.bodyChunks[1].vel.y * -1 * DKMultiplier)),
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
                if (self.rollCounter < 8)
                {
                    self.rollCounter = 8;
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
                self.grasps[grasp].grabbed is Rock || 
                self.grasps[grasp].grabbed is Spear spear && !spear.bugSpear || 
                self.grasps[grasp].grabbed is MoreSlugcats.LillyPuck
            )
            {
                if (!self.input[0].thrw || self.grasps[grasp].grabbed is MoreSlugcats.LillyPuck)
                {
                    self.TossObject(grasp, eu);
                    Esclass_GD_ReplicateThrowBodyPhysics(self, grasp);
                    self.dontGrabStuff = 15;
                    self.ReleaseGrasp(grasp);
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
    }
}
