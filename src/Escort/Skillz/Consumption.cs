using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace TheEscort
{
    public static class Consumption
    {
        public static void Escort_EatMeatFaster(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchLdfld<Player>(nameof(Player.eatMeat))
                );
            }
            catch (Exception e)
            {
                Ebug(e, "Couldn't find player.eatmeat! IL match 1 failed");
                throw new Exception("IL match 1 failed", e);
            }
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchLdcI4(15)
                );
            }
            catch (Exception e)
            {
                Ebug(e, "Couldn't find the 15 value thing! IL match 2 failed");
                throw new Exception("IL match 2 failed", e);
            }
            Ebug("EatMeatFaster identified point of interest", 0, true);



            try
            {
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate(
                    (int original, Player self) =>
                    {
                        if (Eshelp_IsMe(self.slugcatStats.name, false))
                        {
                            return 7;
                        }
                        return original;
                    }
                );
            }
            catch (Exception e)
            {
                Ebug(e, "Emitting failed!");
                throw new Exception("IL emit failed", e);
            }
        }
    }
}