using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Deflector;

public static class DF_FX
{
    public static void UltimateShake(Player self)
    {
        try
        {
            self.room?.ScreenMovement(null, default, 1.2f);
        }
        catch (Exception e)
        {
            Ebug(self, e, "Couldn't cause the screen to shake!");
        }
    }
}