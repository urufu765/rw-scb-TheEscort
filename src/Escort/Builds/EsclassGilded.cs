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
        public static readonly PlayerFeature<float> gilded_float = PlayerFloat("theescort/gilded/float_speed");
        public static readonly PlayerFeature<float> gilded_lev = PlayerFloat("theescort/gilded/levitation");


        public void Esclass_GD_Tick(Player self, ref Escort e)
        {
        }

        private void Esclass_GD_Update(Player self, ref Escort e)
        {
            if (
                !gilded_float.TryGet(self, out float floatingSpd) ||
                !gilded_lev.TryGet(self, out float levitation)
                ) return;
            if (self.wantToJump > 0 && e.float_state){
                e.Escat_float_state(self, false);
                self.wantToJump = 0;
            }
            else if (self.wantToJump > 0 && self.canJump > 0 && !e.float_state){
                e.Escat_float_state(self);
                self.wantToJump = 0;
            }
            if (e.float_state){
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
                //self.standing = true;

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
        }
    }
}
