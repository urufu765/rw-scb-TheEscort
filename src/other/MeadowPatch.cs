using MonoMod.RuntimeDetour;
using RainMeadow;
using System;
using System.Reflection;
using static TheEscort.Eshelp;

namespace TheEscort.Patches;

public static class EPatchMeadow
{
    private static int _clock;
    public static bool Clock
    {
        get
        {
            if (_clock > 19) _clock = 0;
            return _clock++ == 1;
        }
    }
    public static void Initialize()
    {
        try
        {
            _ = new Hook(
                typeof(StoryGameMode).GetMethod(nameof(StoryGameMode.LoadWorldAs), BindingFlags.Instance | BindingFlags.Public),
                typeof(EPatchMeadow).GetMethod(nameof(FixTheWorldState), BindingFlags.Static | BindingFlags.Public)
            );
            _ = new Hook(
                typeof(StoryGameMode).GetMethod(nameof(StoryGameMode.LoadWorldIn), BindingFlags.Instance | BindingFlags.Public),
                typeof(EPatchMeadow).GetMethod(nameof(FixTheTimeline), BindingFlags.Static | BindingFlags.Public)
            );
        }
        catch (Exception err)
        {
            Ebug(err, "Hook not work!");
        }
    }

    public static bool IsOnline()
    {
        try
        {
            return OnlineManager.lobby is not null;
        }
        catch (Exception err)
        {
            Ebug(err, "Online check bad!");
        }
        return false;
    }

    public static SlugcatStats.Timeline FixTheTimeline(Func<StoryGameMode, RainWorldGame, SlugcatStats.Timeline> orig, StoryGameMode self, RainWorldGame game)
    {
        if (Eshelp_IsNull(self.currentCampaign, false))
        {
            // if (Clock)
            // {
            //     Ebug("What's the timeline? " + (game?.TimelinePoint?.value??"I don't know!"));
            // }
            return SlugcatStats.Timeline.Spear;
            // return Plugin.EscortMeTime;
        }
        return orig(self, game);
    }

    /// <summary>
    /// wait wtf THIS RUN EVERY FRAME?!
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    /// <param name="game"></param>
    /// <returns></returns>
    public static SlugcatStats.Name FixTheWorldState(Func<StoryGameMode, RainWorldGame, SlugcatStats.Name> orig, StoryGameMode self, RainWorldGame game)
    {
        // Ebug("Loaded world state");
        if (Eshelp_IsNull(self.currentCampaign, false))
        {
            // Ebug("What's the worldstate? " + (self.currentCampaign.value??"I don't know!"));
            // return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;
            return Plugin.EscortMe;
        }
        return orig(self, game);
    }

    public static void AddOnlineEscortData(Player player)
    {
        if (player?.abstractPhysicalObject?.GetOnlineObject() is OnlinePhysicalObject opo)
        {
            if (opo.TryGetData<EscortOnlineData>(out _)) return;
            opo.AddData(new EscortOnlineData());
        }
    }
}