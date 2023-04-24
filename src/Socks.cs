using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using TheEscort;

namespace TheEscort
{
    public static class Socklass
    {
        public class Socks{
            public bool coopMode;
            //public MoreSlugcats.SlugNPCAI sAI;
            public readonly int sID = 3118;

            public Socks(Player self){
                this.coopMode = false;
            }
        }

        private static readonly ConditionalWeakTable<Player, Socks> CWT = new();
        public static Socks Get(this Player player) => CWT.GetValue(player, _ => new(player));
    }
}