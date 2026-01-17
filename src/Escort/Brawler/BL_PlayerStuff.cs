using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Brawler;

public static class BL_Player
{
    /// <summary>
    /// Stops
    /// </summary>
    public static bool Esclass_BL_HeavyCarry(Player self, PhysicalObject obj)
    {
        if (obj.TotalMass <= self.TotalMass * ins.ratioed * 2 && obj is Creature)
        {
            for (int i = 0; i < 2; i++)
            {
                if (self.grasps[i] != null && self.grasps[i].grabbed != obj && self.grasps[i].grabbed is Spear && self.grasps[1 - i] != null && self.grasps[1 - i].grabbed == obj)
                {
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Whether object is a BigOneHand or not
    /// </summary>
    public static bool Esclass_BL_Grabability(Player self, PhysicalObject obj, ref Escort e, out Player.ObjectGrabability grabby)
    {
        grabby = Player.ObjectGrabability.BigOneHand;
        if (obj is Weapon w && w is Rock or Spear or ScavengerBomb)
        {
            return true;
        }
        if (obj is Creature c && !c.dead)
        {
            if (obj is JetFish || obj is Fly || obj is TubeWorm || obj is Cicada || obj is MoreSlugcats.Yeek || (obj is Player && obj == self))
            {
                return false;
            }
            if (c.Stunned && c is Lizard && ins.Esconfig_Dunkin() && !e.LizardDunk)
            {
                if (e.LizGoForWalk == 0)
                {
                    e.LizGoForWalk = 320;
                }
                e.LizardDunk = true;
            }
            grabby = Player.ObjectGrabability.OneHand;
            return true;
        }
        return false;
    }


}