using BepInEx;
using SlugBase.Features;
using System;
using RWCustom;
using MoreSlugcats;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Escapist tweak values
        // public static readonly PlayerFeature<> barbarian = Player("theescort/barbarian/");
        // public static readonly PlayerFeature<float> barbarian = PlayerFloat("theescort/barbarian/");
        // public static readonly PlayerFeature<float[]> barbarian = PlayerFloats("theescort/barbarian/");


        public void Esclass_BB_Tick(Player self, ref Escort e)
        {
            // Resets wiggle count of Barbarian is not holding onto a player
            if (!e.BarFkingCretin && e.BarWiggle > 0)
            {
                e.BarWiggle = 0;
            }

            // Checks whether the grasp is held creature
            e.BarFkingCretin = e.BarCretin = false;
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i].grabbed is Creature)
                {
                    e.BarWhichCretin[i] = e.BarCretin = true;
                    if (self.grasps[i].grabbed is Player)
                    {
                        e.BarFkingCretin = true;
                    }
                }
                else
                {
                    e.BarWhichCretin[i] = false;
                }
            }

            // Increase shield delay count if creature held and grab held
            // TODO: Disable item swallowing
            if (e.BarCretin && self.input[0].pckp)
            {
                if (e.BarShieldDelay < 20)
                {
                    e.BarShieldDelay++;
                }
            }
            else
            {
                e.BarShieldDelay = 0;
            }

            // Shielding status check
            e.BarShieldState = 0;
            if (e.BarShieldDelay >= 20)
            {
                if (e.BarWhichCretin[0]) e.BarShieldState = -1;
                else if (e.BarWhichCretin[1]) e.BarShieldState = 1;
            }
        }

        private void Esclass_BB_Update(Player self, ref Escort e)
        {
            
        }
    }
}
