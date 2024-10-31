using SlugBase.Features;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;


namespace TheEscort;
/// <summary>
/// Oh my god this is so hard coded it's making me cry... it's not like I had much time anyways. I'll fix this when I'm properly back
/// </summary>
public static class SChallengeMachine
{
    public static bool ESC_ACTIVE {
        get
        {
            return SC03_Active || SC04_Active;
        }
    }
    public static bool SC03_Starter {get; set;} = false;
    public static bool SC03_Active {get; set;} = false;
    public static bool SC04_Starter {get; set;} = false;
    public static bool SC04_Active {get; set;} = false;

    private static MoreSlugcats.FadeOut fadeOut1;
    private static FSprite[] missionProgressDot = new FSprite[4];
    private static FLabel missionProgressTxN;
    private static FLabel missionProgressTxE;
    private static FLabel missionComplete;
    private static FLabel missionCompleteSub;
    private static FSprite missionCompleteGlow;
    private static int timeFadeIn;
    private static int timeHold;
    private static int timeFadeOut;
    private static float screensizeX;
    private static float screensizeY;
    private static Color greenConfirm = new(0.5f, 0.85f, 0.5f);
    private static Color greyIncomplete = new(0.2f, 0.2f, 0.2f);

    private static FirecrackerPlant.ScareObject intimidatingAura;

#region Challenge 03: Railgunner Commits Genocide
    /// <summary>
    /// regular scavenger kill progress
    /// </summary>
    private static int killsNorm;
    /// <summary>
    /// Elite scavenger kill progress
    /// </summary>
    private static int killsElit;
    /// <summary>
    /// Scav merchant overall kill progress (out of 4)
    /// </summary>
    private static int killsProg;
    /// <summary>
    /// Elite scavenger kill counter colour
    /// </summary>
    private static Color elitKillCountdown = new(0.45f, 0.4f, 0.2f);
    /// <summary>
    /// Regular scavenger kill counter colour
    /// </summary>
    private static Color normKillCountdown = new(0.35f, 0.35f, 0.35f);
    /// <summary>
    /// Initialises the room variables at the start of a session (cycle)
    /// </summary>
    public static void SC03_SessionStart(this Room room)
    {
        room.world.game.session.creatureCommunities.SetLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, -1, 0, -10f);
        room.world.game.session.creatureCommunities.InfluenceLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, -1, 0, -10f, -1, -1);
    }

    /// <summary>
    /// Gives mission progress
    /// </summary>
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

    /// <summary>
    /// Initiates the sprites and graphics for the mission HUD
    /// </summary>
    public static void SC03_GrafixInit(HUD.HUD hud)
    {
        screensizeX = hud.rainWorld.options.ScreenSize.x;
        screensizeY = hud.rainWorld.options.ScreenSize.y;
        missionCompleteGlow = new FSprite("Futile_White")
        {
            scaleX = 50f,
            scaleY = 20f,
            x = screensizeX / 2f,
            y = screensizeY - 50f,
            alpha = 0
        };
        missionCompleteGlow.shader = hud.rainWorld.Shaders["FlatLight"];

	    missionComplete = new FLabel(RWCustom.Custom.GetDisplayFont(), "<TEXT>")
        {
            x = screensizeX / 2f + 0.2f,
            y = screensizeY + 50.2f
        };
	    missionComplete.shader = hud.rainWorld.Shaders["MenuText"];

	    missionCompleteSub = new FLabel(RWCustom.Custom.GetFont(), "<TEXT>")
        {
            x = screensizeX / 2f + 0.2f,
            y = screensizeY + 20.2f
        };
	    missionCompleteSub.shader = hud.rainWorld.Shaders["MenuText"];

        for (int i = 0; i < 4; i++)
        {
            missionProgressDot[i] = new FSprite("WormEye")
            {
                x = screensizeX / 2f + (20f * i) - 30f,
                y = screensizeY - 25f,
                scale = 1
            };
            hud.fContainers[1].AddChild(missionProgressDot[i]);
        }

        missionProgressTxN = new FLabel(RWCustom.Custom.GetDisplayFont(), "<TEXT>")
        {
            x = screensizeX / 2f - 50f,
            y = screensizeY - 25.2f
        };

        missionProgressTxE = new FLabel(RWCustom.Custom.GetDisplayFont(), "<TEXT>")
        {
            x = screensizeX / 2f + 50f,
            y = screensizeY - 25.2f
        };

        hud.fContainers[1].AddChild(missionProgressTxE);
        hud.fContainers[1].AddChild(missionProgressTxN);
        hud.fContainers[1].AddChild(missionCompleteGlow);
        hud.fContainers[1].AddChild(missionComplete);
        hud.fContainers[1].AddChild(missionCompleteSub);
    }

    /// <summary>
    /// Draws the graphics for the challenge hud
    /// </summary>
    public static void SC03_GrafixDraw(float timeStacker)
    {
        if (missionComplete is not null)
        {
            missionComplete.y = screensizeY + 50.2f - Mathf.Lerp(0, 100, Mathf.InverseLerp(0, 120, timeHold + timeFadeOut));
            missionCompleteSub.y = screensizeY + 20.2f - Mathf.Lerp(0, 100, Mathf.InverseLerp(0, 120, timeHold + timeFadeOut));
            missionCompleteGlow.alpha = 0.15f * Mathf.InverseLerp(0, 120, timeHold + timeFadeOut);
        }
    }

    /// <summary>
    /// Updates the graphics variables
    /// </summary>
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

        if (missionProgressTxE is not null)
        {
            missionProgressTxE.text = "" + (20 - killsElit);
            missionProgressTxE.color = elitKillCountdown;
            if (killsElit >= 20)
            {
                missionProgressTxE.text = "0!";
                missionProgressTxE.color = greenConfirm;
            }
            missionProgressTxN.text = "" + (150 - killsNorm);
            missionProgressTxN.color = normKillCountdown;
            if (killsNorm >= 150)
            {
                missionProgressTxN.text = "0!";
                missionProgressTxN.color = greenConfirm;
            }
            for (int i = 0; i < 4; i++)
            {
                missionProgressDot[i].color = (i < killsProg)? ((i == killsProg - 1)? Color.Lerp(greenConfirm, Color.white, Mathf.InverseLerp(0, 120, timeHold + timeFadeOut)) : greenConfirm) : greyIncomplete;
            }
        }
    }

    /// <summary>
    /// Checks challenge progress, then upon completion ends the run
    /// </summary>
    public static bool SC03_Finished(this Room room)
    {
        if (room?.world?.game?.session is StoryGameSession sgs)
        {
            int merchantsKilled = 0;
            if (sgs.saveState.miscWorldSaveData.Esave().ESC03_GW) merchantsKilled++;
            if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SH) merchantsKilled++;
            if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SI) merchantsKilled++;
            if (sgs.saveState.miscWorldSaveData.Esave().ESC03_SB) merchantsKilled++;
            killsProg = merchantsKilled;
            killsNorm = sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavKills;
            killsElit = sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaKills;
            

            if (
                killsProg == 4 &&
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
#endregion

#region Challenge 04: Speedster Speedrun
    /// <summary>
    /// Initialises the room variables at the start of a session (cycle)
    /// </summary>
    public static void SC04_SessionStart(this Room room)
    {

    }

    /// <summary>
    /// Gives mission progress
    /// </summary>
    public static void SC04_Achieve(this Room room)
    {

    }

    /// <summary>
    /// Initiates the sprites and graphics for the mission HUD
    /// </summary>
    public static void SC04_GrafixInit(HUD.HUD hud)
    {

    }

    /// <summary>
    /// Draws the graphics for the challenge hud
    /// </summary>
    public static void SC04_GrafixDraw(float timeStacker)
    {

    }

    /// <summary>
    /// Updates the graphics variables
    /// </summary>
    public static void SC04_GrafixUpda()
    {

    }

    /// <summary>
    /// Checks challenge progress, then upon completion ends the run
    /// </summary>
    public static bool SC04_Finished(this Room room)
    {
        // intimidatingAura = new FirecrackerPlant.ScareObject();
        // room.AddObject(intimidatingAura);
    }
#endregion
}
