using SlugBase.Features;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;


namespace TheEscort;
public static class SChallengeMachine
{
    public static bool ESC_ACTIVE {
        get
        {
            return SC03_Active;
        }
    }
    public static bool SC03_Starter {get; set;} = false;
    public static bool SC03_Active {get; set;} = false;

    private static MoreSlugcats.FadeOut fadeOut1;
    private static FLabel missionComplete;
    private static FLabel missionCompleteSub;
    private static FSprite missionCompleteGlow;
    private static int timeFadeIn;
    private static int timeHold;
    private static int timeFadeOut;
    private static float screensize;


    public static void SC03_SessionStart(this Room room)
    {
        room.world.game.session.creatureCommunities.SetLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, -1, 0, -10f);
        room.world.game.session.creatureCommunities.InfluenceLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, -1, 0, -10f, -1, -1);
    }

    public static void SC03_Achieve(this Room room, bool merchant)
    {
        if (room?.world?.game?.session is StoryGameSession sgs && room.world.region is not null)
        {
            if (merchant)
            {
                switch (room.world.region.name)
                {
                    case "GW":
                        if (sgs.saveState.miscWorldSaveData.Esave().ESC03_GW) break;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft--;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_GW = true;
                        timeFadeIn = 40;
                        missionComplete.text = "Garbage Wastes Merchant Downed!";
                        missionCompleteSub.text = $"{sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft}/4 to go!";
                        Ebug("GW Complete!");
                        break;
                    case "SH":
                        if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SH) break;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft--;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_SH = true;
                        timeFadeIn = 40;
                        missionComplete.text = "Shaded Citadel Merchant Downed!";
                        missionCompleteSub.text = $"{sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft}/4 to go!";
                        Ebug("SH Complete!");
                        break;
                    case "SI":
                        if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SI) break;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft--;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_SI = true;
                        timeFadeIn = 40;
                        missionComplete.text = "Sky Islands Merchant Downed!";
                        missionCompleteSub.text = $"{sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft}/4 to go!";
                        Ebug("SI Complete!");
                        break;
                    case "SB":
                        if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SB) break;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft--;
                        sgs.saveState.miscWorldSaveData.Esave().ESC03_SB = true;
                        timeFadeIn = 40;
                        missionComplete.text = "Subterranian Merchant Downed!";
                        missionCompleteSub.text = $"{sgs.saveState.miscWorldSaveData.Esave().ESC03_ObjLeft}/4 to go!";
                        Ebug("SB Complete");
                        break;
                }
            }
            else
            {
                if (sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaKills >= 20 && !sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaWin)
                {
                    timeFadeIn = 40;
                    missionComplete.text = "20 Elite Scavengers Felled!";
                    missionCompleteSub.text = "Wow! You did it!";
                    sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaWin = true;
                }
                if (sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavKills >= 150 && !sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavWin)
                {
                    timeFadeIn = 40;
                    missionComplete.text = "150 Scavengers Felled!";
                    missionCompleteSub.text = "Yippee!!!";
                    sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavWin = true;
                }
            }
        }
    }

    public static void SC03_GrafixInit(HUD.HUD hud)
    {
        screensize = hud.rainWorld.options.ScreenSize.y;
        missionCompleteGlow = new FSprite("Futile_White");
        missionCompleteGlow.shader = hud.rainWorld.Shaders["FlatLight"];
        missionCompleteGlow.scaleX = 50f;
        missionCompleteGlow.scaleY = 20f;
        missionCompleteGlow.x = hud.rainWorld.options.ScreenSize.x / 2f;
	    missionCompleteGlow.y = hud.rainWorld.options.ScreenSize.y - 50f;
        missionCompleteGlow.alpha = 0;
	    missionComplete = new FLabel(RWCustom.Custom.GetDisplayFont(), "<TEXT>");
	    missionComplete.shader = hud.rainWorld.Shaders["MenuText"];
	    missionComplete.x = hud.rainWorld.options.ScreenSize.x / 2f + 0.2f;
	    missionComplete.y = screensize + 50.2f;
        missionComplete.alpha = 0;
	    missionCompleteSub = new FLabel(RWCustom.Custom.GetFont(), "<TEXT>");
	    missionCompleteSub.shader = hud.rainWorld.Shaders["MenuText"];
	    missionCompleteSub.x = hud.rainWorld.options.ScreenSize.x / 2f + 0.2f;
	    missionCompleteSub.y = screensize + 20.2f;
        missionCompleteSub.alpha = 0;
        hud.fContainers[1].AddChild(missionCompleteGlow);
        hud.fContainers[1].AddChild(missionComplete);
        hud.fContainers[1].AddChild(missionCompleteSub);
    }


    public static void SC03_GrafixDraw(float timeStacker)
    {
        if (missionComplete is not null)
        {
            missionComplete.y = screensize + 50.2f - Mathf.Lerp(0, 100, Mathf.InverseLerp(0, 120, timeHold + timeFadeOut));
            missionCompleteSub.y = screensize + 20.2f - Mathf.Lerp(0, 100, Mathf.InverseLerp(0, 120, timeHold + timeFadeOut));
            missionCompleteGlow.alpha = 0.15f * Mathf.InverseLerp(0, 120, timeHold + timeFadeOut);
        }
    }

    public static void SC03_GrafixUpda()
    {
        if (timeFadeIn > 0)
        {
            timeFadeIn--;
            timeHold += 3;
        }
        else if (timeHold > 0)
        {
            timeHold--;
            timeFadeOut++;
        }
        else if (timeFadeOut > 0)
        {
            timeFadeOut -= 3;
        }
        else
        {
            timeFadeIn = timeHold = timeFadeOut = 0;
        }
    }

    public static bool SC03_Finished(this Room room)
    {
        if (room?.world?.game?.session is StoryGameSession sgs)
        {
            if (
                sgs.saveState.miscWorldSaveData.Esave().ESC03_GW &&
                sgs.saveState.miscWorldSaveData.Esave().ESC03_SH &&
                sgs.saveState.miscWorldSaveData.Esave().ESC03_SI &&
                sgs.saveState.miscWorldSaveData.Esave().ESC03_SB && 
                fadeOut1 is null
            )
            {
                fadeOut1 = new MoreSlugcats.FadeOut(room, new Color(0.525f, 0.8f, 0.8f), 80, false);
                room.AddObject(fadeOut1);
                return false;
            }
            if (
                fadeOut1 is not null &&
                fadeOut1.IsDoneFading()
            )
            {
                room.game.GoToRedsGameOver();
                if (sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavWin) sgs.saveState.progression.miscProgressionData.Esave().achieveEschallenge_Challenge03a = true;
                if (sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaWin) sgs.saveState.progression.miscProgressionData.Esave().achieveEschallenge_Challenge03b = true;
                sgs.saveState.progression.miscProgressionData.Esave().achieveEschallenge_Challenge03 = true;
                return true;
            }
        }
        return false;
    }
}
