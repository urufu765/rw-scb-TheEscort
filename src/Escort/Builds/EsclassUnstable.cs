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


        /// <summary>
        /// Replaces the slugcat jump with a blink instead. Dunno the behaviour of jumps so this code will assume that the jump function will not always run when jump is pressed midair.
        /// </summary>
        private void Esclass_US_Jump(Player self, Escort e)
        {
            throw NotImplementedException;
        }


        /// <summary>
        /// Handles the midair jumps
        /// </summary>
        public static void Esclass_US_MidJump(Player self, Escort e)
        {
            throw NotImplementedException;
        }


        /// <summary>
        /// Checks terrain collisions for Unstable. Works semi-similarly to new escapist's except missing a few comparisons.
        /// </summary>
        public static float Esclass_US_Dash(Player self)
        {
            float dashDistance = 0;
            for (int i = 1; i < 10; i++)
            {
                // Calculate positions
                float xCom = self.bodyChunks[1].pos.x + 20 * i * self.input[0].x;
                float yCom = self.bodyChunks[1].pos.y + 20 * i * self.input[0].y;
                
                IntVector2 tPos = self.room.GetTilePosition(new(xCom, yCom));
                Room.Tile rt = self.room.GetTile(tPos);



            }
        }
    }
}
