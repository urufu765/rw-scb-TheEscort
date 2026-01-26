using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Escapist.Strappable;

public class StramCloth : Strammable<AbstractPhysicalObject>
{
    public StramCloth(Player owner, AbstractPhysicalObject thing, int hand) : base(owner, thing, hand)
    {
        useAvail = false;
    }

    public override bool Activate(bool first)
    {
        return false;
    }

    public override void DoVisualShtick(Vector2 wristPos)
    {
        // TODO: uhh idk
    }

    public override bool EmergencyActivate(bool first, Creature creature)
    {
        return false;
    }
}