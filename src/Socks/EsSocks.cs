using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;

namespace TheEscort
{
    /// <summary>
    /// It's Socks!
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {
        private static readonly bool unplayableSocks = true;
        //private static bool playingSocks = false;

        private void Socks_ctor(Player self){
            self.setPupStatus(true);
        }

        private static bool Socks_hideTheSocks(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            try{
                if (i == null){
                    Ebug("Found nulled slugcat name when hiding cats!", 1);
                    return orig(i);
                }
                if (i == EscortSocks){
                    return unplayableSocks;
                }
            } catch (Exception err){
                Ebug(err, "Couldn't hide the socks in the drawer!");
            }
            return orig(i);
        }
    }
}