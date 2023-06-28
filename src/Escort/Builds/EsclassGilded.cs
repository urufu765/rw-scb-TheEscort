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
        }

        private void Esclass_GD_Update(Player self, ref Escort e)
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
            else if (e.GildPower > 5000)
            {
                self.Die();
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
            if (!(self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.HangFromBeam || self.bodyMode == Player.BodyModeIndex.ZeroG) && self.wantToJump > 0 && self.canJump == 0 && !e.GildFloatState)
            {
                e.Escat_float_state(self);
                self.wantToJump = 0;
            }

            // Main code
            if (e.GildLevitateLimit > 0 && self.input[0].jmp && e.GildFloatState)
            {
                self.mainBodyChunk.vel.y = Mathf.Sign(self.mainBodyChunk.vel.y) < 0? Mathf.Min(self.mainBodyChunk.vel.y + floatingSpd / 2f, 0) : Mathf.Max(self.mainBodyChunk.vel.y - floatingSpd / 2f, 0);

                self.bodyChunks[0].vel.y += levitation;
                self.bodyChunks[1].vel.y += levitation - 1f;
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
    }
}
