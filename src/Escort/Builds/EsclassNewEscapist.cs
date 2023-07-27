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
        // New Escapist tweak values
        // public static readonly PlayerFeature<> escapist = Player("theescort/newescapist/");
        // public static readonly PlayerFeature<float> escapist = PlayerFloat("theescort/newescapist/");
        // public static readonly PlayerFeature<float[]> escapist = PlayerFloats("theescort/newescapist/");


        public void Esclass_NE_Tick(Player self, ref Escort e)
        {
            if (e.NEsCooldown > 0)
            {
                e.NEsCooldown--;
            }

            if (e.NEsLastInput.x > 0)
            {
                e.NEsLastInput.x--;
            }
            else if (e.NEsLastInput.x < 0)
            {
                e.NEsLastInput.x++;
            }
            if (e.NEsLastInput.y > 0)
            {
                e.NEsLastInput.y--;
            }
        
            if (e.NEsAbilityTime > 0)
            {
                e.NEsAbilityTime--;
            }
            else if (e.NEsSetCooldown > 0)
            {
                e.NEsCooldown = e.NEsSetCooldown;
                e.NEsSetCooldown = 0;
            }
        }

        public void Esclass_NE_AbsoluteTick(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }

        private void Esclass_NE_Update(Player self, ref Escort e)
        {
            // Check if player has inputted the direction for ability activation
            if (e.NEsAbilityTime == 0)  // Doubletap direction checker
            {
                // Left
                if (self.input[0].x < 0 && self.input[1].x >= 0)
                {
                    if (e.NEsLastInput.x < 0)
                    {
                        e.NEsLastInput.x = 0;
                        e.NEsAbilityTime = 200;
                    }
                    else
                    {
                        e.NEsLastInput.x = -40;
                    }
                }

                // Right
                if (self.input[0].x > 0 && self.input[1].x <= 0)
                {
                    if (e.NEsLastInput.x > 0)
                    {
                        e.NEsLastInput.x = 0;
                        e.NEsAbilityTime = 200;
                    }
                    else
                    {
                        e.NEsLastInput.x = 40;
                    }
                }

                // Up
                if (self.input[0].y > 0 && self.input[1].y <= 0)
                {
                    if (e.NEsLastInput.y > 0)
                    {
                        e.NEsLastInput.y = 0;
                        e.NEsAbilityTime = 200;
                    }
                    else
                    {
                        e.NEsLastInput.y = 40;
                    }
                }

            }
        }
    }
}
