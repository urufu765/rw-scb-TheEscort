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
        // Unstable tweak values
        // public static readonly PlayerFeature<> unstable = Player("theescortunstable//");
        // public static readonly PlayerFeature<float> unstable = PlayerFloat("theescort/unstable/");
        // public static readonly PlayerFeature<float[]> unstable = PlayerFloats("theescort/unstable/");


        public void Esclass_US_Tick(Player self, ref Escort e)
        {
            if (e.UnsTripTime < 40)
            {
                e.UnsTripTime++;
            }
            else
            {
                e.UnsTripTime = 0;
            }

            if (self.canJump > 0 && e.UnsBlinkCount > 0)
            {
                e.UnsBlinkCount--;
            }

            if (e.UnsBlinkWindow > 0)
            {
                e.UnsBlinkWindow--;
            }
        }

        private void Esclass_US_Update(Player self, ref Escort e)
        {
            // Trippin' time!
            if (e.UnsTripTime == 0 && UnityEngine.Random.value > 0.9f && self.bodyMode != Player.BodyModeIndex.ZeroG)
            {
                Ebug(self, "Unstable fucking tripped! Laugh at 'em!");
                self.standing = false;
                self.bodyChunks[0].vel.x += self.rollDirection * 7f;
                self.bodyChunks[1].vel.x -= self.rollDirection * 5f;
                self.bodyChunks[0].vel.y -= 3f;
                self.Stun(20);
                if (UnityEngine.Random.value > 0.7f)
                {
                    self.LoseAllGrasps();
                }
            }
        }


        /// <summary>
        /// Calculates whether Unstable can blink. Guaranteed for the first two blinks.
        /// </summary>
        private static bool Esclass_US_CanBlinkYes(int n)
        {
            if (n < 3) return true;
            return UnityEngine.Random.value < Mathf.Pow(0.5f, n - 2);
        }
    }
}
