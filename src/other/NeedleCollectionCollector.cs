using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using MoreSlugcats;
using RWCustom;
using TheEscort;
using BepInEx;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;
using static SpearmasterNeedleDataCollectionTool.NeedleLogger;

namespace SpearmasterNeedleDataCollectionTool;
public class SpearmasterSpearObserver
{
    public static readonly GameFeature<bool> LogSpears = GameBool("theescort/spearlogger");

    private static ConditionalWeakTable<Plugin, NeedleMe> nL = new();

    public void Attach()
    {
        On.Player.ctor += SMSO_AttachToSpear;
        On.SaveState.SessionEnded += SMSO_PrintValues;
        //On.Player.GrabUpdate += SMSO_GetSpear;
        On.Spear.Spear_makeNeedle += SMSO_MakeSpear;
        On.Player.ReleaseGrasp += SMSO_DropThrowSpear;
    }

    private void SMSO_AttachToSpear(On.Player.orig_ctor orig, Player self, AbstractCreature ac, World world)
    {
        orig(self, ac, world);
        try
        {
            if (LogSpears.TryGet(world.game, out bool l) && l && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStats.Name.Spear && world?.game?.session is StoryGameSession s)
            {
                nL.Add(ins, new NeedleMe(s.saveState.cycleNumber));
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Spearmaster data collection attach went wrong!");
        }
    }

    private void SMSO_PrintValues(On.SaveState.orig_SessionEnded orig, SaveState self, RainWorldGame game, bool survived, bool newMalnourished)
    {
        try
        {
            if (nL.TryGetValue(ins, out NeedleMe n))
            {
                n.Release(survived);
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Spearmaster data collection print went wrong!");
        }
        orig(self, game, survived, newMalnourished);
    }

    private void SMSO_MakeSpear(On.Spear.orig_Spear_makeNeedle orig, Spear self, int type, bool active)
    {
        orig(self, type, active);
        try
        {
            if (active && self.grabbedBy[0] != null && self.grabbedBy[0].grabber is Player p && p.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && nL.TryGetValue(ins, out NeedleMe n))
            {
                n.Capture(in p, isCreate: true);
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Spearmaster data collection make spear went wrong!");
        }
    }

    private void SMSO_GetSpear(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        try
        {
            if (LogSpears.TryGet(self.room.game, out bool l) && l && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && nL.TryGetValue(ins, out NeedleMe n))
            {

            }
        }
        catch (Exception err)
        {
            Ebug(err, "Spearmaster data collection get spear went wrong!");
        }
        orig(self, eu);
    }

    private void SMSO_DropThrowSpear(On.Player.orig_ReleaseGrasp orig, Player self, int grasp)
    {
        try
        {
            if (LogSpears.TryGet(self.room.game, out bool l) && l && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && self.grasps[grasp] != null && self.grasps[grasp].grabbed is Spear s && nL.TryGetValue(ins, out NeedleMe n) && s.spearmasterNeedle)
            {
                if (s.mode == Weapon.Mode.Free)  // Dropped
                {
                    n.Capture(in self, isDrop: true);
                }
                if (s.mode == Weapon.Mode.Thrown)  // Thrown
                {
                    n.Capture(in self, isThrow: true);
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Spearmaster data collection drop/throw spear went wrong!");
        }
        orig(self, grasp);
    }
}