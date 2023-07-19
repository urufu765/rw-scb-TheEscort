using BepInEx;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Escapist tweak values
        // public static readonly PlayerFeature<> gilded = Player("theescort/gilded/");
        // public static readonly PlayerFeature<float> gilded = PlayerFloat("theescort/gilded/");
        // public static readonly PlayerFeature<float[]> gilded = PlayerFloats("theescort/gilded/");

        public void Esclass_GDo_Tick(Player self, ref Escort e)
        {
            if (e.GildFloatFloat > 0){
                e.GildFloatFloat--;
            }

            if (e.GildMoonJump > 0){
                e.GildMoonJump--;
            }

            if (self.input[0].jmp && e.GildCrushTime < 20 && !e.GildCrush){
                e.GildCrushTime++;
            }
            else if (!self.input[0].jmp && e.GildCrushTime > 0){
                e.GildCrushTime--;
            }

            if (!self.Stunned && e.GildPower < 5000)
            {
                e.GildPower++;
            }
        }

        private void Esclass_GDo_Update(Player self, ref Escort e)
        {
            if (
                !gilded_float.TryGet(self, out float floatingSpd) ||
                !gilded_lev.TryGet(self, out float levitation)
                ) return;

            if (self.aerobicLevel > 4){
                self.Blink(5);
            }
            if (self.standing) e.GildCrushReady = true;

            if (!(self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG || self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)){
                // Moon jump
                if (!e.GildCrush){
                    self.bodyChunks[0].vel.y += Mathf.Lerp(0, levitation * (self.animation == Player.AnimationIndex.Flip? 1.5f : 1), Mathf.InverseLerp(0, e.GildMoonJumpMax, e.GildMoonJump));
                }

                // Crush
                if (e.GildCrushTime == 20 && !e.GildCrush && e.GildCrushReady && self.bodyChunks[1].contactPoint.y != -1 && e.GildMoonJump < e.GildMoonJumpMax -5){
                    e.GildCrushTime = 0;
                    e.GildCrush = true;
                    e.GildMoonJump = 0;
                }
            }
            if (e.GildCrush){
                // Code to have Escort stomp on some zonkerdoodles
                if (self.bodyChunks[1].contactPoint.y == -1 || self.bodyChunks[1].contactPoint.x != 0 || self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG || self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam){
                    self.bodyChunks[0].vel.y = Mathf.Max(self.bodyChunks[0].vel.y, 0);
                    self.bodyChunks[1].vel.y = Mathf.Max(self.bodyChunks[1].vel.y, 1);
                    self.impactTreshhold = 1f;
                    e.GildCrush = false;
                    self.room?.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk);

                }
                else {
                    self.impactTreshhold = 200f;
                    self.bodyChunks[1].vel.y -= 3f;
                }
            }

            // Levitation
            bool levitate = false;  // Disabled for now
            if (levitate && (self.wantToJump > 0 || self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.HangFromBeam) && e.GildFloatState){
                Ebug(self, "Jump");
                e.Escat_float_state(self, false);
                self.wantToJump = 0;
            }
            else if (levitate && self.input[0].jmp && !self.input[1].jmp && self.canJump > 0 && !(self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.HangFromBeam) && !e.GildFloatState){
                Ebug(self, "Jump Higher");
                e.Escat_float_state(self);
                self.wantToJump = 0;
            }
            if (levitate && e.GildFloatState){
                self.buoyancy = 0f;
                bool swimmer = self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG;
                if (self.animation != Player.AnimationIndex.Flip){
                    self.animation = Player.AnimationIndex.None;
                }
                self.bodyMode = Player.BodyModeIndex.Default;
                if (self.dead || self.stun >= 10){
                    e.Escat_float_state(self, false);
                }
                //self.gravity = 0;
                self.airFriction = 0.8f;
                self.standing = false;

                // Move
                if (self.input[0].x != 0){
                    self.bodyChunks[0].vel.x = Mathf.Clamp(self.bodyChunks[0].vel.x + floatingSpd * self.input[0].x, -10f, 10f);
                    self.bodyChunks[1].vel.x = Mathf.Clamp(self.bodyChunks[1].vel.x + (floatingSpd - 1) * self.input[0].x, -9f, 9f);
                }
                else {
                    self.mainBodyChunk.vel.x = Mathf.Sign(self.mainBodyChunk.vel.x) < 0? Mathf.Min(self.mainBodyChunk.vel.x + floatingSpd / 2f, 0) : Mathf.Max(self.mainBodyChunk.vel.x - floatingSpd / 2f, 0);
                }
                if (self.input[0].y != 0){
                    self.bodyChunks[0].vel.y = Mathf.Clamp(self.bodyChunks[0].vel.y + floatingSpd * self.input[0].y, -10f, 10f);
                    self.bodyChunks[1].vel.y = Mathf.Clamp(self.bodyChunks[1].vel.y + (floatingSpd - 1) * self.input[0].y, -9f, 9f);
                }
                else {
                    self.mainBodyChunk.vel.y = Mathf.Sign(self.mainBodyChunk.vel.y) < 0? Mathf.Min(self.mainBodyChunk.vel.y + floatingSpd / 2f, 0) : Mathf.Max(self.mainBodyChunk.vel.y - floatingSpd / 2f, 0);
                }


                // Hover
                if (!swimmer){
                    self.bodyChunks[0].vel.y += levitation;
                    self.bodyChunks[1].vel.y += levitation - 1f;
                }
            }

            // Death upon reaching too high of a hype level
            if (e.GildPower > 5000) { self.Die(); }
        }


        private static void Esclass_GDo_Breathing(Player self, float f){
            if (
                self.animation == Player.AnimationIndex.BellySlide && 
                self.aerobicLevel > 0.5f && 
                self.aerobicLevel < 1f
            ) { self.aerobicLevel = 1f; }
            self.aerobicLevel = Mathf.Min(5.5f, self.aerobicLevel + (f / (self.aerobicLevel > 1? 10 : 8)));
        }

        private static void Esclass_GDo_Jump(Player self, ref Escort e){
            if (self.standing){
                e.GildMoonJump = e.GildMoonJumpMax;
            }
        }

        private void Esclass_GDo_Collision(Player self, Creature creature, ref Escort e){
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
    }
}
