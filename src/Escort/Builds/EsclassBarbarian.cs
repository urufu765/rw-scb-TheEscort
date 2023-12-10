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
            throw new NotImplementedException();
        }

        private void Esclass_BB_Update(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }
    }
}
