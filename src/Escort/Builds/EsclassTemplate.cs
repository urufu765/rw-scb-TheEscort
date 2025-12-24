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
        // public static readonly PlayerFeature<> escapist = Player("theescort//");
        // public static readonly PlayerFeature<float> escapist = PlayerFloat("theescort//");
        // public static readonly PlayerFeature<float[]> escapist = PlayerFloats("theescort//");


        public static void Esclass__Tick(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }

        public static void Esclass__Update(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }
    }
}
