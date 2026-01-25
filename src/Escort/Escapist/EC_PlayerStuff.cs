using System;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;

namespace TheEscort.Escapist;

public static class EC_Player
{
    public static bool ICantSwallowThisButICanDoSomethingElse(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
    {
        if (Eshelp_IsNull(self?.slugcatStats?.name))
        {
            goto origEnd;
        }
        if (!eCon.TryGetValue(self, out Escort e))
        {
            goto origEnd;
        }

        if (e.Escapist)
        {
            return false;
        }

        origEnd:
        return orig(self, testObj);
    }
}