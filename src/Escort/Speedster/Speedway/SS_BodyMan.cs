using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster;

public static class SS_BodyStuff
{
    public static void UpdateBodyMode(Player self, ref Escort e)
    {
        float speed_Mod = (e.SpeDashNCrash? 1.5f : 0) + ((e.SpeDashNCrash? 1.5f : 1) * .1f * e.SpeGear);
        float boost_Mod = e.SpeBoosterTunneling;

        switch (self.bodyMode)
        {
            case var a when a == Player.BodyModeIndex.Default:  // ADJUSTMENT NEEDED
                break;
        }
    }
}