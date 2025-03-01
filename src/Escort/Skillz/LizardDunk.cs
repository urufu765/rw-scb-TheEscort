using BepInEx;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        /// <summary><list type="bullet">
        /// <item>0: X Power Multiplier</item>
        /// <item>1: Y Power Multiplier</item>
        /// <item>2: Self X0 Multiplier</item>
        /// <item>3: Self Y0 Multiplier</item>
        /// <item>4: Self X1 Multiplier</item>
        /// <item>5: Self Y1 Multiplier</item>
        /// </list></summary>
        public static readonly PlayerFeature<float[]> LizDunker = PlayerFloats("theescort/lizard_dunker");


        /// <summary>
        /// Determines whether to use type 1 or type two for calculating lizard dunk velocity
        /// <list type="bullet">
        /// <item>Type 1: Uses ingame violence velocity calculation</item>
        /// <item>Type 2: Adds velocity manually</item>
        /// </list>
        /// </summary>
        public static readonly PlayerFeature<bool> LizDunkType2 = PlayerBool("theescort/lizard_dunk_type2");


        public void LizardDunk(Player self, int grasp, Escort e)
        {
            if (!LizDunker.TryGet(self, out float[] oreo) ||
                !LizDunkType2.TryGet(self, out bool dunkType2))  // why is this here, investigate.
            {
                throw new System.InvalidOperationException("LizardDunker or LizDunkType2 not found");
            }

            if (self.grasps[grasp]?.grabbed is Lizard lizzie && Esconfig_Dunkin() && !e.Brawler && self.canJump == 0)
            {
                if (self.Grabability(lizzie) == Player.ObjectGrabability.OneHand || self.Grabability(lizzie) == Player.ObjectGrabability.BigOneHand)
                {
                    Ebug("Lizard dunk!");
                    Ebug($"X0:{self.bodyChunks[0].vel.x} Y0:{self.bodyChunks[0].vel.y}");
                    Ebug($"X1:{self.bodyChunks[1].vel.x} Y1:{self.bodyChunks[1].vel.y}");
                    Vector2 pow = new(oreo[0] * self.ThrowDirection, oreo[1]);
                    if (self.input[0].y != 0)
                    {
                        pow = new(oreo[0] * self.ThrowDirection, oreo[1] * self.input[0].y);
                    }
                    if (dunkType2)
                    {
                        lizzie.Violence(self.mainBodyChunk, null, lizzie.mainBodyChunk, null, Creature.DamageType.Blunt, 0.5f, 60f);
                        lizzie.mainBodyChunk.vel.x += pow.x;
                        lizzie.mainBodyChunk.vel.y += pow.y;
                    }
                    else
                    {
                        lizzie.Violence(self.mainBodyChunk, pow, lizzie.mainBodyChunk, null, Creature.DamageType.Blunt, 0.5f, 60f);
                    }
                    self.bodyChunks[0].vel.x += oreo[2] * pow.x;
                    self.bodyChunks[0].vel.y += oreo[3] * pow.y;
                    self.bodyChunks[1].vel.x += oreo[4] * pow.x;
                    self.bodyChunks[1].vel.y += oreo[5] * pow.y;
                    Ebug("After:");
                    Ebug($"X0:{self.bodyChunks[0].vel.x} Y0:{self.bodyChunks[0].vel.y}");
                    Ebug($"X1:{self.bodyChunks[1].vel.x} Y1:{self.bodyChunks[1].vel.y}");
                }
                if (!Esconfig_SFX(self) || true)
                {
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk, false, 0.8f, 0.85f);
                }
            }
        }
    }
}