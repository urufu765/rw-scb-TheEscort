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
            if (e.NEsChangeRoom > 0)
            {
                e.NEsChangeRoom--;
            }
            
            if (e.NEsCooldown > 0)
            {
                e.NEsCooldown--;
            }
        }

        public void Esclass_NE_AbsoluteTick(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }

        private void Esclass_NE_Update(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }
    }
}
