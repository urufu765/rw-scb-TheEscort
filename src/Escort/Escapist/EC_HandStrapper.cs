using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Escapist;

public static class EC_HandStrapper
{
    public static void GrabUpdate(Player self, bool eu, ref Escort e)
    {
        for (int i = 0; i < e.EscStrammables.Length; i++)
        {
            if (e.EscStrammables[i] is IStrammable strammable)
            {
                strammable.Update(eu);
            }
        }
    }
}
