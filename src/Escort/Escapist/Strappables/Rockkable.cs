using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Escapist.Strappable;

public class StramRock : Strammable<AbstractPhysicalObject>
{
    public StramRock(Player owner, AbstractPhysicalObject thing, int hand) : base(owner, thing, hand)
    {
    }

    public override bool Activate(bool first)
    {
        if (useAvail && owner?.room is Room r)
        {
            Vector2 wristPos = default;
            if (owner.graphicsModule is PlayerGraphics pg && pg.hands?[hand] is SlugcatHand sch)
            {
                wristPos = Vector2.Lerp(sch.pos, pg.head.pos, .1f);
            }
            //TODO: use scavengerbomb fragments instead
            r.AddObject(new ExplosiveSpear.SpearFragment(wristPos, Custom.RNV() * Mathf.Lerp(2, 4, UnityEngine.Random.value)));
            // r.PlaySound(SoundID.Slugcat_Lay_Down_Object, owner.mainBodyChunk);
            useAvail = false;
            NullReady = true;
            return true;
        }
        return false;
    }

    public override void DoVisualShtick(Vector2 wristPos)
    {
        throw new NotImplementedException();
    }

    public override bool EmergencyActivate(bool first, Creature creature)
    {
        return false;
    }
}