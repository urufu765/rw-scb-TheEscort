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
    public static class CreatureControlling
    {
        public static void OverrideSafariControlInputUpdate(On.Creature.orig_SafariControlInputUpdate orig, Creature self, int playerIndex)
        {
            if (self.abstractCreature.controlled && self.room is not null && self.freezeControls)
            {
                self.lastInputWithDiagonals = self.inputWithDiagonals;
                self.lastInputWithoutDiagonals = self.inputWithoutDiagonals;
            }
            orig(self, playerIndex);
        }
    }
}