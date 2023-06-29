using BepInEx;
using SlugBase.Features;
using System;
using System.IO;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using RWCustom;
using IL.MoreSlugcats;

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

            if (!e.GildLockRecharge) 
            {
                e.GildRequiredPower = 0;
                e.GildPowerUsage = 0;
                if (!self.Stunned) e.GildPower++;
            }

            if (e.GildLockRecharge && e.GildReservePower < e.GildRequiredPower)
            {
                e.GildPower -= e.GildPowerUsage;
                e.GildReservePower += e.GildPowerUsage;
            }

            if (e.GildCancel && e.GildReservePower > 0)
            {
                if (e.GildReservePower > 50)
                {
                    e.GildPower += 50;
                    e.GildReservePower -= 50;
                }
                else
                {
                    e.GildPower += e.GildReservePower;
                    e.GildReservePower = 0;
                }
            }
            else if (e.GildCancel && e.GildReservePower <= 0)
            {
                e.GildCancel = false;
            }

            if (!self.dead) e.GildLockRecharge = false;
        }

        private void Esclass_GD_Update(Player self, bool eu, ref Escort e)
        {
            if (
                !gilded_float.TryGet(self, out float floatingSpd) ||
                !gilded_lev.TryGet(self, out float levitation)
                ) return;

            // Die by overpower
            if (e.GildPower > 4600)
            {
                self.Blink(5);
            }
            if (e.GildPower > 5000 && !self.dead)
            {
                self.Die();
                e.GildLockRecharge = true;
            }

            // Check empty hand
            bool hasSomething = false;
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i]?.grabbed is not null)
                {
                    hasSomething = true;
                    break;
                }
            }
            if (!hasSomething) e.GildWantToThrow = -1;


            // Throw when letting go of the held button
            if (e.GildWantToThrow != -1 && !self.input[0].thrw)
            {
                self.ThrowObject(e.GildWantToThrow, eu);
                e.GildWantToThrow = -1;
            }


            #region Temporary levitation code
            if (self.canJump > 0) e.GildLevitateLimit = 200;

            // Deactivate levitation
            if ((!self.input[0].jmp || self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.HangFromBeam || e.GildLevitateLimit == 0) && e.GildFloatState)
            {
                e.Escat_float_state(self, false);
                self.wantToJump = 0;
            }

            // Activate levitation
            if (!(self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.HangFromBeam || self.bodyMode == Player.BodyModeIndex.ZeroG) && self.wantToJump > 0 && self.canJump == 0 && !e.GildFloatState && e.GildPower > Escort.GildCheckLevitate)
            {
                e.Escat_float_state(self);
                self.wantToJump = 0;
                e.GildRequiredPower = Escort.GildCheckLevitate;
                e.GildPowerUsage = Escort.GildUseLevitate;
            }

            // Main code
            if (e.GildLevitateLimit > 0 && self.input[0].jmp && e.GildFloatState)
            {
                e.GildLockRecharge = true;
                self.mainBodyChunk.vel.y = Mathf.Sign(self.mainBodyChunk.vel.y) < 0? Mathf.Min(self.mainBodyChunk.vel.y + floatingSpd / 2f, 0) : Mathf.Max(self.mainBodyChunk.vel.y - floatingSpd / 2f, 0);

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
                if (!hasSomething && !e.GildCrush && (e.GildMoonJump < e.GildMoonJumpMax - 5 || e.GildFloatState) && self.bodyChunks[1].contactPoint.y != -1 && self.input[0].thrw && !self.input[1].thrw)
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

        private static void Esclass_GD_Breathing(Player self, float f)
        {
            self.aerobicLevel = Mathf.Min(1f, self.aerobicLevel + (f / 8.2f));
        }

        private static void Esclass_GD_Jump(Player self, ref Escort e)
        {
            if (self.standing)
            {
                e.GildMoonJump = e.GildMoonJumpMax;
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
            if (self.animation == Player.AnimationIndex.BellySlide && self.rollCounter > 8)
            {
                self.rollCounter++;
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
                if (!self.input[0].thrw)
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
