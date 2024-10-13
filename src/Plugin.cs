﻿using BepInEx;
using Menu;
using MonoMod.Cil;
using Newtonsoft.Json;
using RWCustom;
using SlugBase;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.SMSMod;
using static UrufuCutsceneTool.CsInLogger;


/// <summary>
/// The holy grail of jankiness and somehow stable code. My very first game mod, and my first time using C#. Not my first time coding though
/// </summary>
namespace TheEscort;

[BepInPlugin(MOD_ID, "[Alpha] The Escort", "0.4")]
partial class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Static instance of the plugin class to allow public access to things that require this class
    /// </summary>
    public static Plugin ins;


    /// <summary>
    /// Configuration instance, to access all the user-set options
    /// </summary>
    public EscOptions config;

    /// <summary>
    /// Literally only used in the above section above the partial class identifier
    /// </summary>
    public const string MOD_ID = "urufudoggo.theescort";

#region Declare Features
    /// <summary>
    /// Allow json values to override internal values (Player instance)
    /// </summary>
    public static readonly PlayerFeature<bool> pRTEdits = PlayerBool("playescort/realtime_edits");

    /// <summary>
    /// Allow json values to override internal values (Game instance)
    /// </summary>
    public static readonly GameFeature<bool> gRTEdits = GameBool("gameescort/realtime_edits");

    /// <summary>
    /// Allows Escort to do the sick flip
    /// </summary>
    public static readonly PlayerFeature<bool> BtrPounce = PlayerBool("theescort/better_pounce");

    /// <summary>
    /// Angry lizards
    /// </summary>
    public static readonly GameFeature<bool> SupahMeanLizards = GameBool("theescort/mean_lizards");

    /// <summary>
    /// JSON VALUES,
    /// ["Stun Slide damage", "Stun Slide base stun duration", "Drop Kick base damage", "Drop Kick stun duration"]
    /// </summary>
    public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");

    /// <summary>
    /// Slidestun forward thrust/momentum
    /// </summary>
    public static readonly PlayerFeature<float[]> SlideLaunchMod = PlayerFloats("theescort/slide_launch_mod");

    /// <summary>
    /// Heavy lift multiplier (0.6x default, 3x Escort)
    /// </summary>
    public static readonly PlayerFeature<float> LiftHeavy = PlayerFloat("theescort/heavylifter");

    /// <summary>
    /// A bit of a wonk. Adds more adrenaline on TOP of the original adrenaline add to make achieving and maintaining battlehype easier
    /// </summary>
    public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");

    /// <summary>
    /// Dropkick knockback multiplier. Controls how strong the dropkick knockback is to those who can experience knockback.
    /// </summary>
    public static readonly PlayerFeature<float> DKM = PlayerFloat("theescort/dk_multiplier");

    /// <summary>
    /// Allow parry slide (parry by sliding on the ground)
    /// </summary>
    public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
    
    /// <summary>
    /// Amount of frames after pressing jump in a corridor where contact with another creature would count as a headbutt
    /// </summary>
    public static readonly PlayerFeature<int> Escomet = PlayerInt("theescort/headbutt");

    /// <summary>
    /// Allow Escort's elevator (ramping off creatures while pressing and holding jump launches Escort)
    /// </summary>
    public static readonly PlayerFeature<bool> Elvator = PlayerBool("theescort/elevator");
    
    public static readonly PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");

    /// <summary>
    /// Allow Escort's hype mechanic
    /// </summary>
    public static readonly PlayerFeature<bool> HypeSys = PlayerBool("theescort/adrenaline_system");

    /// <summary>
    /// Controls the minimum required adrenaline to activate Battlehype
    /// </summary>
    public static readonly PlayerFeature<float> HypeReq = PlayerFloat("theescort/stamina_req");

    /// <summary>
    /// Controls how many frames between outputs of a log that updates constantly
    /// </summary>
    public static readonly PlayerFeature<int> CR = PlayerInt("theescort/reset_rate");

    /// <summary>
    /// Controls Escort's spear damage
    /// JSON VALUES: ["Hyped spear damage", "Base spear damage"]
    /// </summary>
    public static readonly PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");

    /// <summary>
    /// Allow Escort being able to hold two spears
    /// </summary>
    public static readonly PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");

    /// <summary>
    /// Allow silly sounds
    /// </summary>
    public static readonly PlayerFeature<bool> soundAhoy = PlayerBool("theescort/sounds_ahoy");

    /// <summary>
    /// Helps adjust the gutter watter resistance on escort in a variety of on the fly
    /// </summary>
    public static readonly PlayerFeature<float[]> NoMoreGutterWater = PlayerFloats("theescort/guuh_wuuh");
    /// <summary>
    /// Do not. Tries to adapt pouncing to walls... not much success here
    /// </summary>
    public static readonly PlayerFeature<bool> LWallJump = PlayerBool("theescort/long_wall_jump");

    /* JSON VALUES
    ["Head Y velocity", "Body Y velocity", "Head X velocity", "Body X velocity", "ConstantDownDiagnoal floor value", "ConstantDownDiagonal ceiling value", "min JumpBoost", "max JumpBoost"]
    */
    public static readonly PlayerFeature<float[]> WallJumpVal = PlayerFloats("theescort/wall_jump_val");


    /// <summary>
    /// Head marking sprite scaling
    /// JSON VALUES
    /// [Rotation val, X val, Y val]
    /// </summary> 
    public static readonly PlayerFeature<float[]> headDraw = PlayerFloats("theescort/headthing");
    /// <summary>
    /// Body marking sprite scaling
    /// </summary>
    public static readonly PlayerFeature<float[]> bodyDraw = PlayerFloats("theescort/bodything");
    /// <summary>
    /// Replaces the grapple backpack.... idk why forgor
    /// </summary>
    //public static readonly GameFeature<bool> replaceGrapple = GameBool("thesocks/replacegrapple");

    /// <summary>
    /// Registers the Escort sleep scene
    /// </summary>
    public static readonly GameFeature<MenuScene.SceneID> AltSleepScene = GameExtEnum<MenuScene.SceneID>("alt_sleep_scene");
    /// <summary>
    /// Registers the Escort with Socks sleep scene
    /// </summary>
    public static readonly GameFeature<MenuScene.SceneID> AltSleepSceneDuo = GameExtEnum<MenuScene.SceneID>("alt_sleep_scene_together");
    #endregion

#region Plugin Variable Declarations
    public static readonly SlugcatStats.Name EscortMe = new("EscortMe");
    public static readonly SlugcatStats.Name EscortSocks = new("EscortSocks");
    //public static readonly SlugcatStats.Name ShadowEscort = new("EscortDummy", true);

    /// <summary>
    /// Urufu calls you a failure.
    /// </summary>
    public static SoundID Escort_SFX_Death;
    /// <summary>
    /// Urufu says "Sick Flip!"
    /// </summary>
    public static SoundID Escort_SFX_Flip;
    /// <summary>
    /// Urufu says "Cool Flip!"
    /// </summary>
    public static SoundID Escort_SFX_Flip2;
    /// <summary>
    /// Urufu says "Nice Flip!"
    /// </summary>
    public static SoundID Escort_SFX_Flip3;
    /// <summary>
    /// ...around and around and around and around and around and around and around and around and around and around and around and around and around and around and around...
    /// </summary>
    public static SoundID Escort_SFX_Roll;
    /// <summary>
    /// Uru says "Boop" in multiple pitches and inflections
    /// </summary>
    public static SoundID Escort_SFX_Boop;
    /// <summary>
    /// That Deltarune explosion sfx edited to sound extremely cheap, juuust like Rails.
    /// </summary>
    public static SoundID Escort_SFX_Railgunner_Death;
    /// <summary>
    /// Lizard, grab. Lizard, grab. Lizard, grab. Lizard, grab. Lizard,grab. Lizard, grab. Lizard, grab. Lizard, grab.
    /// </summary>
    public static SoundID Escort_SFX_Lizard_Grab;
    /// <summary>
    /// A thump made by a kick drum
    /// </summary>
    public static SoundID Escort_SFX_Impact;
    /// <summary>
    /// A high pitch bell hit
    /// </summary>
    public static SoundID Escort_SFX_Parry;
    /// <summary>
    /// A metal knife rubbing against another knife
    /// </summary>
    public static SoundID Escort_SFX_Brawler_Shank;
    /// <summary>
    /// A distorted clap
    /// </summary>
    public static SoundID Escort_SFX_Pole_Bounce;
    /// <summary>
    /// Urufu calls you fat (Rotund World exclusive!)
    /// </summary>
    public static SoundID Escort_SFX_Uhoh_Big;
    /// <summary>
    /// Urufu is being real sneaky
    /// </summary>
    public static SoundID Esconfig_SFX_Sectret;
    /// <summary>
    /// Literal SILENCE
    /// </summary>
    public static SoundID Escort_SFX_Placeholder;
    /// <summary>
    /// Bass pluck reversed (inspired by Bearhugger's incoming punch sfx from Punch-Out Wii)
    /// </summary>
    public static SoundID Escort_SFX_Gild_Stomp;

    //(Urufu announces your spawn for 2023 April Fools)
    // public static SoundID Escort_SFX_Spawn;


    // Miscellanious things

    /// <summary>
    /// Sets Escort's marking colors to the main color instead of Arena
    /// </summary>
    private readonly bool nonArena = false;  // Sets Escort's marking colors to the main color instead of Arena

    // Escort instance 
    
    /// <summary>
    /// Contains instances of the Escort class for each applicable players. ALL Escort loadouts use the same Escort class to control the loadout specific abilities.
    /// </summary>
    public static ConditionalWeakTable<Player, Escort> eCon = new();
    /// <summary>
    /// Contains instances of the Socks class for each applicable players.
    /// </summary>
    public static ConditionalWeakTable<Player, Socks> sCon = new();
    //public static ConditionalWeakTable<AbstractCreature, AbstractEscort> aCon = new();

    /// <summary>
    /// Global hype requirement setting (stores the setting from json or remix for use)
    /// </summary>
    public float hypeRequirement;

    /// <summary>
    /// Semi-global dropkick knockback intensity setting (stores the setting from json or remix for use)
    /// </summary>
    private float DKMultiplier;

    /// <summary>
    /// Mostly-global heavy carry intensity (stores the setting from json or remix for use)
    /// </summary>
    float ratioed;

    /// <summary>
    /// Global setting for whether temple guard is friendly or not
    /// </summary>
    public static bool templeGuardIsFriendly;

    /// <summary>
    /// Enables logging of player input and player position
    /// </summary>
    public static readonly bool logForCutscene = false;

    /// <summary>
    /// Global variable that prompts the code to check if pup is existing
    /// </summary>
    public static bool checkPupStatusAgain = false;

    /// <summary>
    /// Status check that checks if slugpup is allowed to exist in the campaign
    /// </summary>
    public static bool pupAvailable;

    /// <summary>
    /// Status check that checks if slugpup is alive and existing
    /// </summary> 
    public static bool pupIsAlive;

    /// <summary>
    /// Contains the list of naturally spawned needle spears
    /// </summary>
    public static List<EntityID> natrualSpears = new();


    // Patches
    
    /// <summary>
    /// Revivify patch: 1.2.0
    /// </summary>
    public static bool escPatch_revivify = false;
    /// <summary>
    /// Rotund World patch: 1.6
    /// </summary>
    public static bool escPatch_rotundness = false;
    /// <summary>
    /// Dress My Slugcat patch: 1.4
    /// </summary>
    public static bool escPatch_dms = false;
    /// <summary>
    /// Guardian patch: N/A
    /// </summary>
    public static bool escPatch_guardian = false;
    //private bool escPatch_emeraldTweaks = false;
#endregion


    /// <summary>
    /// Here's where all the hooks go... oh god why are there so many hooks?! HOW IS MY CODE STILL FUNCTIONAL AND MOSTLY COMPATIBLE WITH OTHER MODS?!
    /// </summary>
    public void OnEnable()
    {
        Logger.LogInfo("-> Escort plugin INIT!");
        try
        {
            ins = this;
        }
        catch (Exception e)
        {
            base.Logger.LogError(e);
        }
        On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
        On.RainWorld.OnModsInit += Escort_Option_Dont_Disappear_Pls_Maybe_Pretty_Please_I_will_do_anything_please;
        On.RainWorld.PostModsInit += Escort_PostInit;

        On.SaveState.setDenPosition += Escort_ChangingRoom;
        On.SaveState.SessionEnded += Escort_Reset_Values;
        
        On.Lizard.ctor += Escort_Lizard_ctor;

        On.Room.Loaded += Escort_Hipbone_Replacement;
        On.RoomSettings.Load += Escort_Transplant;

        On.PlayerGraphics.InitiateSprites += Escort_InitiateSprites;
        On.PlayerGraphics.ApplyPalette += Escort_ApplyPalette;
        On.PlayerGraphics.AddToContainer += Escort_AddGFXContainer;
        On.PlayerGraphics.DrawSprites += Escort_DrawSprites;
        On.PlayerGraphics.Update += Escort_GFXUpdate;
        On.PlayerGraphics.DefaultSlugcatColor += Escort_Colorz;
        On.RoomCamera.DrawUpdate += Escort_HUD_Draw;

        // Jolly UI
        On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.HasUniqueSprite += Escort_Jolly_Sprite;
        On.JollyCoop.JollyMenu.JollyPlayerSelector.GetPupButtonOffName += Escort_Jolly_Name;
        On.PlayerGraphics.JollyUniqueColorMenu += Escort_Please_Just_Kill_Me;
        On.JollyCoop.JollyMenu.JollySlidingMenu.ctor += EscortBuildSelectFromJollyMenu;
        On.JollyCoop.JollyMenu.JollyPlayerSelector.Update += EscortHideShowBuildCopium;
        On.JollyCoop.JollyMenu.JollySlidingMenu.UpdatePlayerSlideSelectable += EscortGrayedOutLikeAnIdiot;
        On.JollyCoop.JollyMenu.JollySetupDialog.RequestClose += EscortPleaseSaveTheGoddamnConfigs;
        //On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.Update += Escort_RGBRGBRGB_GoesBrr;

        // Arena UI
        //On.Menu.MultiplayerMenu.InitiateGameTypeSpecificButtons += Escort_Arena_Class_Changer;

        // Escort stuff
        On.Player.Jump += Escort_Jump;
        On.Player.UpdateBodyMode += Escort_UpdateBodyMode;
        On.Player.UpdateAnimation += Escort_UpdateAnimation;
        On.Player.Collide += Escort_Collision;
        On.Player.HeavyCarry += Escort_HeavyCarry;
        On.Player.AerobicIncrease += Escort_AerobicIncrease;
        On.Creature.Violence += Escort_Violence;
        On.Player.Update += Escort_Update;
        On.Player.ThrownSpear += Escort_ThrownSpear;
        On.Player.Grabability += Escort_Grabability;
        On.Player.Die += Escort_Die;
        On.Player.WallJump += Escort_WallJump;
        On.Player.MovementUpdate += Escort_MovementUpdate;
        On.Player.checkInput += Escort_checkInput;
        On.Player.ctor += Escort_ctor;
        On.Player.DeathByBiteMultiplier += Escort_GotBit;
        On.Player.TossObject += Escort_TossObject;
        On.Player.ThrowObject += Escort_ThrowObject;
        On.Player.SpearStick += Escort_StickySpear;
        On.Player.Grabbed += Esclass_EC_Grabbed;
        On.Player.GrabUpdate += Escort_GrabbyUpdate;
        On.Player.BiteEdibleObject += Escort_Eated;
        On.Player.CanIPickThisUp += Escort_SpearGet;
        On.Player.TerrainImpact += Esclass_SS_Bonk;
        On.Player.IsCreatureLegalToHoldWithoutStun += Esclass_BL_Legality;
        On.Player.Stun += Esclass_RG_Spasm;
        On.RainWorldGame.Update += Escort_AbsoluteTick;
        On.Creature.SetKillTag += Esclass_NE_CheckKiller;

        // Socks stuff
        On.PlayerGraphics.PlayerObjectLooker.HowInterestingIsThisObject += Socks_Stop_Having_An_Aneurysm;
        On.Player.Update += Socks_Update;
        On.Player.GraphicsModuleUpdated += Socks_GMU;
        On.Player.SlugcatGrab += Socks_Mine;
        On.Player.CanIPickThisUp += Socks_Grabby;
        On.Creature.LoseAllGrasps += Socks_DontLoseBackpack;
        On.Player.Die += Socks_Death;
        On.Player.Jump += Socks_Jump;
        On.Player.GrabUpdate += Socks_Legacy;

        On.Rock.HitSomething += Escort_RockHit;
        On.Rock.Thrown += Escort_RockThrow;

        On.ScavengerBomb.Thrown += Esclass_RG_BombThrow;

        On.FlareBomb.HitSomething += Escort_FlareHit;

        On.MoreSlugcats.LillyPuck.Thrown += Esclass_RG_LillyThrow;

        On.Weapon.WeaponDeflect += Esclass_RG_AntiDeflect;
        On.Weapon.HitThisObject += Esclass_NE_HitShadowscort;

        On.SlugcatStats.SpearSpawnModifier += Escort_SpearSpawnMod;
        On.SlugcatStats.SpearSpawnElectricRandomChance += Escort_EleSpearSpawnChance;
        On.SlugcatStats.SpearSpawnExplosiveRandomChance += Escort_ExpSpearSpawnChance;
        On.SlugcatStats.getSlugcatStoryRegions += Escort_getStoryRegions;
        On.SlugcatStats.HiddenOrUnplayableSlugcat += Socks_hideTheSocks;
        On.SlugcatStats.SlugcatUnlocked += Escort_Playable;
        On.SlugcatStats.SlugcatFoodMeter += Escort_differentBuildsFoodz;

        //On.Player.Update += Estest_1_Update;
        //On.Player.GrabUpdate += Estest_3_GrabUpdate;
        Escort_Conversation.Attach();
        EscortRoomScript.Attach();
        EscortHUD.Attach();
        SpearmasterNeedleDataCollectionTool.SpearmasterSpearObserver.Attach();
        On.TubeWorm.GrabbedByPlayer += GrappleBackpack.BackpackGrabbedByPlayer;
        On.TubeWorm.GrabbedByPlayer += LauncherBackpack.BackpackGrabbedByPlayer;

        On.TubeWorm.Update += Socks_Sticky_Immune;

        On.MoreSlugcats.HRGuardManager.Update += Esclass_GD_KillGuardianWithOneHit;

        On.TempleGuardAI.ThrowOutScore += Escort_Friendship;

        On.ShelterDoor.Close += StoreWinConditionData;
        On.ShelterDoor.DoorClosed += SpawnPupInShelterAtWin;

        On.Menu.SleepAndDeathScreen.AddBkgIllustration += Escort_Add_Slugpup;

        On.PlayerSessionRecord.AddKill += Esclass_DF_DamageIncrease;
    }

    /// <summary>
    /// Forces temple guardians to ignore the creatures Escort brings to the piss pool if the flag is true
    /// </summary>
    private float Escort_Friendship(On.TempleGuardAI.orig_ThrowOutScore orig, TempleGuardAI self, Tracker.CreatureRepresentation crit)
    {
        if (templeGuardIsFriendly)
        {
            return 0f;  // Guardian ignores all creatures
        }
        return orig(self, crit);  // Default behaviour
    }

    /// <summary>
    /// Originally an attempt at trying to make the remix option not die, repurposed for IL hooks and to make sure they work correctly.
    /// </summary>
    private void Escort_Option_Dont_Disappear_Pls_Maybe_Pretty_Please_I_will_do_anything_please(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (this.config is null)
            {
                MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            }
            IL.MoreSlugcats.LillyPuck.HitSomething += Escort_LillyHit;
            IL.MoreSlugcats.Bullet.HitSomething += Escort_BulletHit;
            IL.Spear.HitSomething += Escort_SpearHit;
            IL.ScavengerBomb.HitSomething += Escort_BombHit;
            // IL.FirecrackerPlant.Explode;
        }
        catch (Exception err)
        {
            Ebug(err, "Oh dear.");
        }
    }



    // Verify that all hooked functions have been checked for Escort and send the amount of times the code has been passed with checks

    /// <summary>
    /// Export the log of function uses... TODO: REMOVE!
    /// </summary>
    public void OnApplicationQuit()
    {
        try
        {
            ins.L().LetItRip();
        }
        catch (NullReferenceException ne)
        {
            Debug.LogError(">>>E>   Instance is not inited!");
            Logger.LogError(ne);
        }
        catch (Exception e)
        {
            Debug.LogError(">>>E>   Generic instance error!");
            Logger.LogError(e);
        }

    }


    // Load any resources, such as sprites or sounds

    /// <summary>
    /// Loads all the external assets for use
    /// </summary>
    private void LoadResources(RainWorld rainWorld)
    {
        ins.L().Set();

        // Sound effects! (Used mostly for silly reasons)
        Escort_SFX_Death = new SoundID("Escort_Failure", true);
        Escort_SFX_Flip = new SoundID("Escort_Flip", true);
        Escort_SFX_Roll = new SoundID("Escort_Roll", true);
        Escort_SFX_Boop = new SoundID("Escort_Boop", true);
        Escort_SFX_Railgunner_Death = new SoundID("Escort_Rail_Fail", true);
        Escort_SFX_Lizard_Grab = new SoundID("Escort_Liz_Grab", true);
        Escort_SFX_Impact = new SoundID("Escort_Impact", true);
        Escort_SFX_Parry = new SoundID("Escort_Parry", true);
        Escort_SFX_Flip2 = new SoundID("Escort_Flip_More", true);
        Escort_SFX_Flip3 = new SoundID("Escort_Flip_Even_More", true);
        Escort_SFX_Brawler_Shank = new SoundID("Escort_Brawl_Shank", true);
        Escort_SFX_Pole_Bounce = new SoundID("Escort_Pole_Bounce", true);
        Escort_SFX_Uhoh_Big = new SoundID("Escort_Rotunded", true);
        Esconfig_SFX_Sectret = new SoundID("Esconfig_Sectret", true);
        Escort_SFX_Placeholder = new SoundID("Esplaceholder", true);
        Escort_SFX_Gild_Stomp = new SoundID("Escort_Gild_Stomp", true);
        //Escort_SFX_Spawn = new SoundID("Escort_Spawn", true);

        // Custom sprites! Includes a checker to check if they loaded correctly and are not null!
        FAtlas aB, aH, hA, hB;
        aB = Futile.atlasManager.LoadAtlas("atlases/escorthip");
        aH = Futile.atlasManager.LoadAtlas("atlases/escorthead");
        hA = Futile.atlasManager.LoadAtlas("atlases/escorthuda");
        hB = Futile.atlasManager.LoadAtlas("atlases/escorthudb");
        if (aB == null || aH == null || hA == null || hB == null)
        {
            Ebug("Oh no. Sprites dead.", 0);
        }
        Ebug("All SFX loaded!", 1);
        this.config = new EscOptions(rainWorld);
        MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
        ins.L().Christmas(config.cfgSectret.Value);
        Ebug("All loaded!", 1);
    }

#region Mod Patches
    /// <summary>
    /// Checks if specific mods are enabled, simply flip the flag or apply patches when needed
    /// </summary>
    private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        ins.L().Set();
        orig(self);

        // Look for mods...
        try
        {
            // Revivify
            if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify"))
            {
                ins.L().Set("Patch: Revivify");
                Ebug("Found Revivify! Applying patch...", 1);
                escPatch_revivify = true;
            }

            // Dress my slugcat
            if (ModManager.ActiveMods.Exists(mod => mod.id == "dressmyslugcat"))
            {
                ins.L().Set("Patch: DressMySlugcat");
                Ebug("Found Dress My Slugcat!", 1);
                ModManager.Mod DMS_Mod = ModManager.ActiveMods.Find(mod => mod.id == "dressmyslugcat");
                //escPatch_DMS = true;
                Ebug("Found DMS Version: " + DMS_Mod.version, 1);
                string[] dmsVer = DMS_Mod.version.Split('.');

                // Newer than 1.3?
                if (int.TryParse(dmsVer[0], out int verMaj) && verMaj >= 1 && int.TryParse(dmsVer[1], out int verMin) && verMin >= 3)
                {
                    Ebug("Applying patch!...", 1);
                    Espatch_DMS(verMaj, verMin);
                }
                else
                {
                    Ebug("Applying dud patch...", 1);
                    Espatch_DMS();
                }
                escPatch_dms = true;
                Ebug("Patched: " + escPatch_dms, 4);
            }

            // Rotund world
            if (ModManager.ActiveMods.Exists(mod => mod.id == "willowwisp.bellyplus"))
            {
                ins.L().Set("Patch: Rotund World");
                Ebug("Found Rotund World! Applying custom patch...", 1);
                escPatch_rotundness = true;
            }

            // Guardian (Used for uploading exception outputs)
            if (ModManager.ActiveMods.Exists(mod => mod.id == "vigaro.guardian"))
            {
                Ebug("Found Guardian! Applying patch...", 1);
                escPatch_guardian = true;
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened while searching for mods!");
        }
    }

    /// <summary>
    /// Dress My Slugcat patch for applying custom sprites
    /// </summary>
    private static void Espatch_DMS(int verMaj, int verMin)
    {
        try
        {
            // DMS 1.3 has a bug that basically causes the game to lag out, thus if someone is using this version they should just NOT
            if (verMaj == 1 && verMin == 3)
            {
                DressMySlugcat.SpriteDefinitions.AddSprite(new DressMySlugcat.SpriteDefinitions.AvailableSprite
                {
                    Name = "WHY DO YOU HAVE THIS PATCH",
                    Description = "Please update DMS",
                    GallerySprite = "escortHipT",
                    RequiredSprites = new List<string> { "escortHeadT", "escortHipT" },
                    Slugcats = new List<string> { "EscortMe" }
                });
            }
            else  // Anything above 1.3 is fair game (No incompatibilities reported as of yet)
            {
                DressMySlugcat.SpriteDefinitions.AddSprite(new DressMySlugcat.SpriteDefinitions.AvailableSprite
                {
                    Name = "MARKINGS",
                    Description = "Markings",
                    GallerySprite = "escortHipT",
                    RequiredSprites = new List<string> { "escortHeadT", "escortHipT" },
                    Slugcats = new List<string> { "EscortMe" }
                });
            }
        }
        catch (Exception merr)
        {
            Ebug(merr, "Couldn't patch Dress Me Sluggie because...");
        }
    }

    /// <summary>
    /// Legacy Dress My Slugcat patch for versions that do not have compatibility with custom sprites
    /// </summary>
    private static void Espatch_DMS()
    {
        // Versions below 1.3 do not support swapping custom sprites
        try
        {
            Ebug("Using dud patch...", 1);
            DressMySlugcat.SpriteDefinitions.AvailableSprites.Add(new DressMySlugcat.SpriteDefinitions.AvailableSprite
            {
                Name = "MARKINGS",
                Description = "Markings",
                GallerySprite = "escortHipT",
                RequiredSprites = new List<string> { "escortHeadT", "escortHipT" },
                Slugcats = new List<string> { "EscortMe" }
            });
        }
        catch (Exception merr)
        {
            Ebug(merr, "Couldn't patch Dress Me Sluggie because...");
        }
    }
#endregion


#region Escort Legacy Configurations
    // TODO: Replace or make it better by using out keyword
    /*
    Configurations!
    */

    /// <summary>
    /// Vengeful Lizards: After a certain number of lizard kills, spawns a group of lizards that will hunt you down
    /// </summary>
    private bool Esconfig_Vengeful_Lizards()
    {
        return config.cfgVengefulLizards.Value;
    }

    /// <summary>
    /// Mean Lizards: Just tries to make the lizards more aggressive
    /// </summary>
    private bool Esconfig_Mean_Lizards(World self)
    {
        if (!gRTEdits.TryGet(self.game, out bool RT) || !SupahMeanLizards.TryGet(self.game, out bool meanLizard))
        {
            return false;
        }
        if (RT)
        {
            return meanLizard;
        }
        else
        {
            return config.cfgMeanLizards.Value;
        }
    }

    /// <summary>
    /// HeavyLift: Sets the (Escort) global max weight for one handed handling
    /// </summary>
    private bool Esconfig_Heavylift(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !LiftHeavy.TryGet(self, out float power))
        {
            ratioed = 3f;
            return false;
        }
        if (RT)
        {
            ratioed = power;
        }
        else
        {
            ratioed = config.cfgHeavyLift.Value;
        }
        return true;
    }

    /// <summary>
    /// DropKick Multiplier: Knockback strength (for those who can experience knockback)
    /// </summary>
    private bool Esconfig_DKMulti(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !DKM.TryGet(self, out float dk))
        {
            return false;
        }
        if (RT)
        {
            DKMultiplier = dk;
        }
        else
        {
            DKMultiplier = config.cfgDKMult.Value;
        }
        return true;
    }

    /// <summary>
    /// Elevator: Allows Escort to take flight by holding jump and coming in contact with another creature
    /// </summary>
    private bool Esconfig_Elevator(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !Elvator.TryGet(self, out bool yeet))
        {
            return false;
        }
        if (RT)
        {
            return yeet;
        }
        else
        {
            return config.cfgElevator.Value;
        }
    }

    /// <summary>
    /// Hypeable: Enables/disable battlehype mechanic
    /// </summary>
    private bool Esconfig_Hypable(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !HypeSys.TryGet(self, out bool hm))
        {
            return false;
        }
        if (RT)
        {
            return hm;
        }
        else
        {
            return config.cfgHypable.Value;
        }
    }

    /// <summary>
    /// Hype requirement: Sets the minimum adrenaline needed before entering battlehype
    /// </summary>
    public bool Esconfig_HypeReq(Player self, float require = 0.8f)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !HypeReq.TryGet(self, out float req))
        {
            return false;
        }
        if (RT)
        {
            hypeRequirement = req;
        }
        else
        {
            hypeRequirement = config.cfgHypeReq.Value switch
            {
                0 => -1f,
                1 => 0.5f,
                2 => 0.66f,
                3 => 0.75f,
                4 => 0.8f,
                5 => 0.87f,
                6 => 0.92f,
                _ => require,
            };
            ;
        }
        return true;
    }

    /// <summary>
    /// SFX: Allows silly (usually unnecessary) sound effects to "enhance" the user experience
    /// </summary>
    private bool Esconfig_SFX(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !soundAhoy.TryGet(self, out bool soundFX))
        {
            return false;
        }
        if (RT)
        {
            return soundFX;
        }
        else
        {
            return config.cfgSFX.Value;
        }
    }

    /// <summary>
    /// LongWallJump: Enables/disables the ability to long press jump while hanging onto the wall to perform a high wall jump... Funky to use so disabled by default
    /// </summary>
    private bool Esconfig_WallJumps(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !LWallJump.TryGet(self, out bool wallJumper))
        {
            return false;
        }
        if (RT)
        {
            return wallJumper;
        }
        else
        {
            return config.cfgLongWallJump.Value;
        }
    }

    /// <summary>
    /// Better Pounce: Upon being enabled, causes Escort to do a sick flip whenever they pounce (by long-pressing jump)
    /// </summary>
    private bool Esconfig_Pouncing(Player self)
    {
        if (!pRTEdits.TryGet(self, out bool RT) || !BtrPounce.TryGet(self, out bool pouncing))
        {
            return false;
        }
        if (RT)
        {
            return pouncing;
        }
        else
        {
            return config.cfgPounce.Value;
        }
    }

    /// <summary>
    /// This appears to not have a slugbase json version. It's done like this just to make the settings look matching to a certain degree. Controls whether Escort can dunk on lizards.
    /// </summary>
    private bool Esconfig_Dunkin()
    {
        return config.cfgDunkin.Value;
    }

    /// <summary>
    /// Applies the setting for spear tricks (that send Escort rolling or flyin'). Some builds have this forced off by setting the value of e.tossEscort to false.
    /// </summary>
    private bool Esconfig_Spears(Player self)
    {
        try
        {
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return false;
            }
            if (config.cfgSpears.Value)
            {
                return e.tossEscort;
            }
            return false;
        }
        catch (Exception err)
        {
            Ebug(self, err, "Something went wrong when setting an Escort build!");
            return false;
        }
    }

    /// <summary>
    /// Applies the configured build based on the option (COMING SOON or based on the campaign ID)
    /// </summary>
    private bool Esconfig_Build(Player self, int forceBuild = 1)
    {
        try
        {
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return false;
            }
            // Get build ID from configuration
            int pal = config.cfgBuild[self.playerState.playerNumber].Value;
            if (forceBuild != 1)
            {
                pal = forceBuild;
            }

            // Story campaign expansion skips
            if (self.slugcatStats?.name?.value is not null)
            {
                pal = self.slugcatStats.name.value switch 
                {
                    "EscortBriish" => -1,
                    "EscortGamer" => -2,
                    "EscortHax" => -3,
                    "EscortRizzgayer" => -4,
                    "EscortCheese" => -5,
                    "EscortDrip" => -6,
                    _ => pal
                };
            }
            // Fix this by turning it off for expedition or add multiplier or somethingidk (Future me here; HUH? GUH? WUH?)

            switch (pal)
            {
                case -99:  // Testing build
                    e.EsTest = true;
                    break;
                case -98:  // New Escapist build (Testing, obsolete)
                    e.NewEscapist = true;
                    Ebug(self, "New Escapist Build selected!", 2);
                    self.slugcatStats.visualStealthInSneakMode = 1;
                    self.spearOnBack = new Player.SpearOnBack(self);
                    break;
                case -9:  // Power test build
                    if (self?.room?.game is null)
                    {
                        Ebug(self, "NULL GAME!", 1);
                    }
                    else if (!self.room.game.rainWorld.progression.miscProgressionData.Esave().achieveEscort_Bare_Fists)
                    {
                        goto default;
                    }
                    break;
                case -8:  // Unstable test build
                    // IF locked, don't let player play as Unstable
                    if (self?.room?.game is null)
                    {
                        Ebug(self, "NULL GAME!", 1);
                    }
                    /*
                    else if (!self.room.game.rainWorld.progression.miscProgressionData.Esave().beaten_Escort)
                    {
                        goto default;
                    }
                    */
                    e.Unstable = true;
                    Ebug(self, "Unstable (WIP) Build selected!", 2);
                    self.slugcatStats.runspeedFac += 0.45f;
                    self.slugcatStats.poleClimbSpeedFac += 0.4f;
                    self.slugcatStats.corridorClimbSpeedFac += 0.55f;
                    self.slugcatStats.lungsFac += 0.5f;
                    break;
                case -7:
                    e.Barbarian = true;
                    Ebug(self, "Barbarian (WIP) Build selected!", 2);
                    self.slugcatStats.bodyWeightFac += 0.95f;
                    break;
                case -6:  // Gilded build
                    e.Gilded = true;
                    if (config.cfgSectretBuild.Value) e.acidSwim = 0.2f;
                    self.slugcatStats.bodyWeightFac = 1f;
                    self.slugcatStats.lungsFac += 0.3f;
                    self.slugcatStats.runspeedFac = 0.9f;
                    self.slugcatStats.corridorClimbSpeedFac -= 0.35f;
                    self.slugcatStats.poleClimbSpeedFac -= 0.7f;
                    self.slugcatStats.bodyWeightFac -= 0.15f;
                    Ebug(self, "Gilded Build selected!", 2);
                    break;
                case -5:  // Speedstar build
                    e.Speedster = true;
                    e.SpeOldSpeed = config.cfgOldSpeedster.Value;
                    e.SpeMaxGear = config.cfgSpeedsterGears.Value;
                    if (!e.SpeOldSpeed && self.room?.game?.session is StoryGameSession speedsterSession)
                    {
                        Ebug(self, "Get Speedster save!");
                        if (speedsterSession.saveState.miscWorldSaveData.Esave().SpeChargeStore.TryGetValue(self.playerState.playerNumber, out int charging))
                        {
                            e.SpeCharge = Math.Min(charging, e.SpeMaxGear);
                        }
                        //Ebug(self, "Misc: " + JsonConvert.SerializeObject(speedsterSession.saveState.miscWorldSaveData.Esave()));
                    }
                    self.slugcatStats.lungsFac += 0.3f;
                    self.slugcatStats.bodyWeightFac += 0.1f;
                    self.slugcatStats.poleClimbSpeedFac += 0.6f;
                    self.slugcatStats.corridorClimbSpeedFac += 0.8f;
                    self.slugcatStats.runspeedFac += 0.35f;
                    Ebug(self, "Speedstar Build selected!", 2);
                    break;
                case -4:  // Railgunner build
                    e.Railgunner = true;
                    e.acidSwim = 0.3f;
                    e.RailFrail = self.Malnourished || e.escortArena;
                    self.slugcatStats.lungsFac += 0.7f;
                    self.slugcatStats.throwingSkill = 2;
                    self.slugcatStats.loudnessFac += 2f;
                    self.slugcatStats.generalVisibilityBonus += 1f;
                    self.slugcatStats.visualStealthInSneakMode = 0f;
                    self.slugcatStats.bodyWeightFac += 0.3f;
                    Ebug(self, "Railgunner Build selected!", 2);
                    break;
                case -3:  // Escapist build
                    if (config.cfgOldEscapist.Value)
                    {
                        e.Escapist = true;
                        e.dualWield = false;
                        self.slugcatStats.runspeedFac += 0.1f;
                        self.slugcatStats.lungsFac += 0.2f;
                        self.slugcatStats.bodyWeightFac -= 0.15f;
                        Ebug(self, "Old Escapist Build selected!", 2);
                    }
                    else
                    {
                        e.NewEscapist = true;
                        self.slugcatStats.visualStealthInSneakMode += 1;
                        self.slugcatStats.lungsFac += 0.2f;
                        self.slugcatStats.bodyWeightFac -= 0.15f;
                        self.slugcatStats.throwingSkill = 1;
                        self.spearOnBack = new Player.SpearOnBack(self);
                        Ebug(self, "New Escapist Build selected!", 2);
                    }
                    break;
                case -2:  // Deflector build
                    e.Deflector = true;
                    if (self.room?.game?.session is StoryGameSession deflectorSession)
                    {
                        Ebug(self, "Get Deflector save!");
                        if (deflectorSession.saveState.miscWorldSaveData.Esave().DeflPermaDamage.TryGetValue(self.playerState.playerNumber, out float permaDamage))
                        {
                            if (permaDamage > e.DeflPerma)
                            {
                                e.DeflPerma = permaDamage;
                                DeflInitSharedPerma = permaDamage;
                            }
                        }
                        else
                        {
                            Ebug(self, "Couldn't find deflector save!");
                        }
                    }

                    self.slugcatStats.runspeedFac += 0.1f;
                    self.slugcatStats.lungsFac += 0.2f;
                    self.slugcatStats.bodyWeightFac += 0.12f;
                    self.slugcatStats.throwingSkill = 1;
                    Ebug(self, "Deflector Build selected!", 2);
                    break;
                case -1:  // Brawler build
                    e.Brawler = true;
                    e.tossEscort = false;
                    self.slugcatStats.runspeedFac -= 0.1f;
                    self.slugcatStats.lungsFac += 0.2f;
                    self.slugcatStats.corridorClimbSpeedFac -= 0.4f;
                    self.slugcatStats.poleClimbSpeedFac -= 0.4f;
                    self.slugcatStats.throwingSkill = 1;
                    Ebug(self, "Brawler Build selected!", 2);
                    break;
                default:  // Default build
                    Ebug(self, "Default Build selected!", 2);
                    e.isDefault = true;
                    self.slugcatStats.lungsFac -= 0.2f;
                    break;
            }

            // Determine if "Easier mode" is turned on by retrieving the setting
            e.easyMode = config.cfgEasy[self.playerState.playerNumber].Value;
            if (e.easyMode)
            {
                Ebug(self, "Easy Mode active!");
            }

            // Reduce underwater breath if malnourished
            self.slugcatStats.lungsFac += self.Malnourished ? 0.15f : 0f;
            self.buoyancy -= 0.05f;
            if (config.cfgDeflecterSharedPool.Value)
            {
                e.DeflPerma = DeflInitSharedPerma;
            }

            Ebug(self, "Set build complete!", 1);
            Ebug(self, new string[]{
                $"Weightfac: {self.slugcatStats.bodyWeightFac}",
                $"Movespeed: {self.slugcatStats.runspeedFac}",
                $"Lungcapfc: {self.slugcatStats.lungsFac}",
                $"Corridors: {self.slugcatStats.corridorClimbSpeedFac}",
                $"Poleclimb: {self.slugcatStats.poleClimbSpeedFac}",
                $"Throwskil: {self.slugcatStats.throwingSkill}",
                $"Loudnessf: {self.slugcatStats.loudnessFac}",
                $"Stealthyf: {self.slugcatStats.visualStealthInSneakMode}",
                $"Visibilit: {self.slugcatStats.generalVisibilityBonus}"
            }, 2);
            return true;
        }
        catch (Exception err)
        {
            Ebug(self, err, "Something went wrong when setting an Escort build!");
            return false;
        }
    }
#endregion

#region Escort New Configurations
    /// <summary>
    /// Intended to adjust the force of the slidestun launch, on the fence about implementing it.
    /// </summary>
    private bool Esconfig_Launch(Player self, out float value, string type="spear"){
        value = 0;
        if (!pRTEdits.TryGet(self, out bool RT) ||
            !SlideLaunchMod.TryGet(self, out float[] launcher)) return false;
        value = type switch
        {
            "horizontal" => RT ? launcher[0] : config.cfgEscLaunchH.Value,
            "vertical" => RT ? launcher[0] : config.cfgEscLaunchV.Value,
            _ => RT ? launcher[0] : config.cfgEscLaunchSH.Value,
        };
        Esconfig_Launch(self, out float thing); // Not implemented yet
        return thing == 0;
    }
#endregion

    /*
    Escort code!
    */

    /// <summary>
    /// Mixes up the escort sick flip sound effects! Or returns a silent thing if configured to do so
    /// </summary>
    /// <returns>a random flip sound effect or silent thing</returns>
    private SoundID Eshelp_SFX_Flip()
    {
        if (config.cfgNoMoreFlips.Value) return Escort_SFX_Placeholder;
        float r = UnityEngine.Random.value;
        return r switch
        {
            > 0.5f => Escort_SFX_Flip,
            > 0.3f => Escort_SFX_Flip2,
            > 0.01f => Escort_SFX_Flip3,
            _ => Escort_SFX_Railgunner_Death,
        };
    }

    /// <summary>
    /// Implement lizard aggression (edited from template)... don't know if this even does anything.
    /// </summary>
    private void Escort_Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
    {
        ins.L().SetF();
        orig(self, abstractCreature, world);

        if (Esconfig_Mean_Lizards(world))
        {
            ins.L().SetF(true);
            Ebug("Lizard Ctor Triggered!");
            self.spawnDataEvil = Mathf.Max(self.spawnDataEvil, 100f);
        }
    }

    /// <summary>
    /// Upon creation of the player, also initialize the Escort variables and turn the slugcat into an Escort.
    /// </summary>
    private void Escort_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        ins.L().Set();  // ignore this
        Ebug("Ctor Triggered!");
        // Let the game create the player first
        orig(self, abstractCreature, world);

        // Checks if player is an Escort
        if (Eshelp_IsMe(self.slugcatStats.name, false))
        {
            ins.L().Set("Escort Check");

            eCon.Add(self, new Escort(self));  // Create a new Escort class for that player instance

            // Check if the Escort instance has been correctly created and stored in the CWT
            if (!eCon.TryGetValue(self, out Escort e))
            {
                Ebug(self, "Something happened while initializing then accessing Escort instance!", 0);
                return;
            }

            if (world?.game?.session is ArenaGameSession) e.escortArena = true;  // Set arena mode to true (Affects a few different things, hover over to see details)

            // Slugpup code
            if (world?.game?.session is StoryGameSession s)
            {
                // Checks if Escort has encountered the pup
                pupAvailable = s.saveState.miscWorldSaveData.Esave().EscortPupEncountered;
                Ebug(self, $"Pup available? {pupAvailable}", 1, true);

                // Set the Socks variant on first encounter
                if (s.saveState.miscWorldSaveData.Esave().EscortPupCampaignID == 0)
                {
                    s.saveState.miscWorldSaveData.Esave().EscortPupCampaignID = UnityEngine.Random.value switch
                    {
                        > 0.2f => Escort.SocksID,
                        > 0.01f => Escort.DemonSocksID,
                        _ => Escort.SpeedSocksID
                    };
                }
                e.PupCampaignID = s.saveState.miscWorldSaveData.Esave().EscortPupCampaignID;
            }
            Esconfig_Build(self, Challenge_Presetter(self.room, ref e));  // Set build

            // Override settings for challenge setter
            Challenge_Postsetter(self.room, ref e);


            e.Escat_Add_Ring_Trackers(self);  // Add the trackers that will track variables for the HUD to use

            e.originalMass = 0.7f * self.slugcatStats.bodyWeightFac;  // Calculates the original mass to compare to most current mass (Rotund World)

            logImportance = config.cfgLogImportance.Value;  // Commented out for ALPHA TESTING

            try  // Initialize and set up SFX that play on loop
            {
                Ebug(self, "Setting silly sounds", 2);
                e.Escat_setSFX_roller(Escort_SFX_Roll);
                e.Escat_setSFX_lizgrab(Escort_SFX_Lizard_Grab);
                Ebug(self, "All done! Awaiting activation.", 2);

                // 2023 April fools!
                //self.setPupStatus(set: true);
                //self.room.PlaySound(Escort_SFX_Spawn, self.mainBodyChunk);
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error while constructing!");
            }
            finally
            {
                Ebug(self, "All ctor'd", 1);
            }
        }

        // Socks ctor (Socks Campaign Socks, not to be confused with Escort Campaign Socks)
        if (self.slugcatStats.name == EscortSocks)
        {
            ins.L().Set("Socks Check");
            sCon.Add(self, new Socks(self));  // Add to CWT
            if (!sCon.TryGetValue(self, out Socks es))
            {  // Check if instance has been added correctly
                Ebug(self, "Something happened while initializing then accessing Socks instance!", 0);
                return;
            }
            Socks_ctor(self);  // Additional ctor stuff
            es.SockWorld = world;
            try  // Initialize grapple backpack
            {
                Creature.Grasp[] tempGrasps = self.grasps;
                Array.Resize(ref tempGrasps, self.grasps.Length + 1);
                self.grasps = tempGrasps;
                //es.Escat_kill_backpack();
                //es.Escat_generate_backpack(self);
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error while constructing!");
            }
        }
    }

    /// <summary>
    /// Sets the build for the challenge
    /// </summary>
    /// <param name="room"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private int Challenge_Presetter(Room room, ref Escort e)
    {
        if (room?.game?.session is not StoryGameSession)
        {
            return 0;
        }
        if (SChallengeMachine.SC03_Starter && (room.game.GetStorySession.saveState.cycleNumber == 0 || room.game.GetStorySession.saveState.miscWorldSaveData.Esave().ESC03_START))
        {
            room.game.GetStorySession.saveState.miscWorldSaveData.Esave().ESC03_START = true;
            SChallengeMachine.SC03_Active = true;
            return -4;
        }
        if (SChallengeMachine.SC04_Starter && (room.game.GetStorySession.saveState.cycleNumber == 0 || room.game.GetStorySession.saveState.miscWorldSaveData.Esave().ESC04_START))
        return 0;
    }

    /// <summary>
    /// Configures the rest of the restrictions for the challenge
    /// </summary>
    /// <param name="room"></param>
    /// <param name="e"></param>
    private void Challenge_Postsetter(Room room, ref Escort e)
    {
        if (room?.game?.session is not StoryGameSession)
        {
            return;
        }

        if (SChallengeMachine.SC03_Active)
        {
            room.SC03_SessionStart();
            e.RailgunLimit = 10;
        }
    }


    /// <summary>
    /// For when Esclass updates need to happen outside of Player update
    /// </summary>
    private void Escort_AbsoluteTick(On.RainWorldGame.orig_Update orig, RainWorldGame self)
    {
        orig(self);
        try
        {
            if (!self.paused)
            {
                foreach(var x in self.Players)
                {
                    if (x.realizedCreature is Player player && eCon.TryGetValue(player, out Escort escort))
                    {
                        // Updates the Escort property trackers outside player update to allow the tracker to continue checking for updates
                        escort.Escat_Update_Ring_Trackers();

                        // Speedster's afterimage
                        if (escort.Speedster)
                        {
                            escort.Escat_showTrail();
                        }

                        // New Escapist's afterimage?
                        if (escort.NewEscapist)
                        {
                            //escort.Escat_NE_ShowTrail();
                            //Esclass_NE_AbsoluteTick(player, ref escort);
                        }
                        if (player.playerState.playerNumber == 0 && SChallengeMachine.ESC_ACTIVE)
                        {
                            SChallengeMachine.SC03_GrafixUpda();
                        }
                    }
                }
            }
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Nulled when trying to tick absolute clocks");
        }
        catch (ArgumentNullException ane)
        {
            Ebug(ane, "Argument nulled when trying to find an escort in a CWT");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when ticking absolute clocks");
        }
    }


    /// <summary>
    /// Each build gets different food requirements!
    /// </summary>
    private IntVector2 Escort_differentBuildsFoodz(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
    {
        try
        {
            if (Eshelp_IsMe(slugcat)) return orig(slugcat);
            IntVector2 foodReq = config.cfgBuild[0].Value switch{
                -7 => new(14, UnityEngine.Random.Range(1, 14)),  // Unstable? (Replace with Barbarian!)
                -6 => config.cfgSectretBuild.Value? new(10, 6) : new(14, 8),  // Gilded
                -5 => new(14, 10),  // Speedster
                -4 => new(14, 7),  // Railgunner
                -3 => config.cfgOldEscapist.Value? new(11, 7) : new(10, 9),  // Escapist (TODO: Don't forget to flip this!)
                -2 => new(14, 8),  // Deflector
                -1 => new(14, 12),  // Brawler
                _  => new(14, 9)  // Default and unspecified.
            };
            if (config.cfgEasy[0].Value && foodReq.y > 3) 
            {
                foodReq.y -= 3;  // Reduce food requirement upon easier mode triggered
            }
            return foodReq;
        } 
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null error when setting food meters. Shouldn't be happening.");
            return orig(slugcat);
        }
        catch (Exception err) 
        {
            Ebug(err, "Generic exception when setting food meter.");
            return orig(slugcat);
        }
    }


    /// <summary>
    /// Resets/sets some saved values upon end of cycle/session
    /// </summary>
    private void Escort_Reset_Values(On.SaveState.orig_SessionEnded orig, SaveState self, RainWorldGame game, bool survived, bool newMalnourished)
    {
        try
        {
            Ebug($"Savestate SEB: {JsonConvert.SerializeObject(self.miscWorldSaveData.Esave())}", 1, true);
        }
        catch (Exception err)
        {
            Ebug(err, "Error on trying to print savestate before session ended");
        }

        if (logForCutscene)
        {
            this.GetCSIL().Release();
        }

        // Respawn pup so karma reinforcement go away
        if (survived && self.miscWorldSaveData.Esave().RespawnPupReady)
        {
            self.miscWorldSaveData.Esave().RespawnPupReady = false;

            // Show tutorial message saying that the pup has been revived
            if (!self.deathPersistentSaveData.Etut(EscortTutorial.EscortPupRespawnedNotify))
            {
                self.deathPersistentSaveData.Etut(EscortTutorial.EscortPupRespawned, true);
            }

            // Remove reinforced karma
            self.deathPersistentSaveData.reinforcedKarma = false;
        }

        // Respawn pup if karma flower is used instead
        if (survived && self.miscWorldSaveData.Esave().AltRespawnReady)
        {
            self.miscWorldSaveData.Esave().AltRespawnReady = false;

            // Show alternative tutorial message saying that the pup has been revived
            if (!self.deathPersistentSaveData.Etut(EscortTutorial.EscortAltPupRespawnedNotify))
            {
                self.deathPersistentSaveData.Etut(EscortTutorial.EscortAltPupRespawned, true);
            }
        }

        orig(self, game, survived, newMalnourished);

        DeflSharedPerma = 0;  // Reset shared pool (Will be replaced with saved value from individual deflector)
        try
        {
            Ebug($"Savestate SEA: {JsonConvert.SerializeObject(self.miscWorldSaveData.Esave())}", 1, true);
        }
        catch (Exception err)
        {
            Ebug(err, "Error on trying to print savestate after session ended");
        }
    }

    /// <summary>
    /// Spawns slugpup in shelter on a successful end of cycle upon the door being closed. Also saves naturally spawned needle spears so they don't get trashed.
    /// </summary>
    private void SpawnPupInShelterAtWin(On.ShelterDoor.orig_DoorClosed orig, ShelterDoor self)
    {
        try
        {
            if (self?.room is null)  // Room exist?
            {
                orig(self);
                return;
            }

            bool winCondition = true;  // Checks if the players are in a win condition
            if (ModManager.CoopAvailable)  // Coop condition
            {
                List<PhysicalObject> listOfShelterers = (
                    from x in self.room.physicalObjects.SelectMany((List<PhysicalObject> y) => y)
                    where x is Player
                    select x).ToList();

                winCondition = listOfShelterers.Count >= self.room.game.PlayersToProgressOrWin.Count;
            }
            else  // Solo play condition
            {
                foreach (AbstractCreature abstractPlayer in self.room.game.Players)
                {
                    if (!abstractPlayer.state.alive)
                    {
                        winCondition = false;
                    }
                }
            }

            if (winCondition && self.room?.game?.session is StoryGameSession s)
            {
                // Save naturally spawned needle spears
                if (self.room.game.StoryCharacter.value == "EscortMe")
                {
                    Escort_SaveShelterSpears(self.room);
                }
                
                Player focus = null;  // Focuses on just the first player. Fuck you other players ;)
                foreach (AbstractCreature abstractCreature in self.room.game.Players)
                {
                    if (abstractCreature?.realizedCreature is Player player && player.playerState.playerNumber == 0)
                    {
                        focus = player;
                        break;
                    }
                }
                if (eCon.TryGetValue(focus, out Escort e))
                {
                    // If slugpup respawn state
                    if (s.saveState.miscWorldSaveData.Esave().RespawnPupReady || s.saveState.miscWorldSaveData.Esave().AltRespawnReady)
                    {
                        // Make the slugpup like you
                        float like, tempLike;
                        like = s.saveState.miscWorldSaveData.Esave().EscortPupLike;
                        tempLike = s.saveState.miscWorldSaveData.Esave().EscortPupTempLike;
                        if (like == -1) like = 1;
                        if (tempLike == -1) tempLike = 1;
                        

                        if (TryFindThePup(self.room, out AbstractCreature ac))
                        {
                            if (ac.state.dead)  // There's probably a better way but for now this will suffice
                            {
                                // Pup exists in the room but is dead so must be respawned!
                                ac.Destroy();  // Might be redundant as .Destory() is done in the method below.
                                SpawnThePup(ref e, self.room, self.room.LocalCoordinateOfNode(0), focus.abstractCreature.ID, like, tempLike);
                                Ebug("Socks has revived from dead!", 1, true);
                            }
                            else
                            {
                                // Pup exists in the room and is well and alive!
                                Ebug("Socks has made it!", 1, true);
                            }
                        }
                        else if (e.socksAbstract?.realizedCreature is not null)
                        {
                            // Pup exists somewhere in the world so must just be respawned!
                            SpawnThePup(ref e, self.room, self.room.LocalCoordinateOfNode(0), focus.abstractCreature.ID, like, tempLike);
                            Ebug("Socks has been brought back from somewhere in the world back to Escort's embrace!", 1, true);
                        }
                        else
                        {
                            // Pup no longer exists so must be recreated and respawned!
                            SpawnThePup(ref e, self.room, self.room.LocalCoordinateOfNode(0));
                            Ebug("Hello Socks!", 1, true);
                        }
                    }
                    bool flag = TryFindThePup(self.room, out _);  // Verify slugpup revived and exists
                    s.saveState.miscWorldSaveData.Esave().SocksIsAlive = flag;
                    pupIsAlive = flag;
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to respawn slugpup!");
        }
        orig(self);
    }


    /// <summary>
    /// Spawns the slugpup
    /// </summary>
    /// <param name="escort">Escort instance</param>
    /// <param name="room">Room to spawn in</param>
    /// <param name="worldCoordinate">Coordinates to spawn at</param>
    /// <param name="iD">ID of player that the slugpup will like</param>
    public static void SpawnThePup(ref Escort escort, Room room, WorldCoordinate worldCoordinate, EntityID? iD = null, float like = 1, float tempLike = 1)
    {
        escort.socksAbstract?.Destroy();  // Remove traces of slugpup if exists
        int socksID = escort.PupCampaignID;  // Retrieves Socks type
        escort.socksAbstract = new AbstractCreature(room.world, StaticWorld.GetCreatureTemplate("Slugpup"), null, worldCoordinate, new(-1, socksID));  // Make new SocksAbstract
        room.abstractRoom.AddEntity(escort.socksAbstract);  // Add the socks to room
        escort.socksAbstract.RealizeInRoom();  // Make socks real
        if (iD is not null)  // Try to make slugpup like you
        {
            escort.socksAbstract.state.socialMemory.GetOrInitiateRelationship(iD.Value).InfluenceLike(like);
            escort.socksAbstract.state.socialMemory.GetOrInitiateRelationship(iD.Value).InfluenceTempLike(tempLike);
        }
        escort.SocksAliveAndHappy.Stun(100);  // Fuck Socks.
        Ebug("Spawn socks (unofficially)!", 1, true);
    }


    /// <summary>
    /// Hijacks the sleep scene and adds the slugpup if slugpup is alive and well
    /// </summary>
    private void Escort_Add_Slugpup(On.Menu.SleepAndDeathScreen.orig_AddBkgIllustration orig, Menu.SleepAndDeathScreen self)
    {
        MenuScene.SceneID newScene = null;
        SlugcatStats.Name name;
        // Find name of slugcat
        if (self.manager.currentMainLoop is RainWorldGame)
            name = (self.manager.currentMainLoop as RainWorldGame).StoryCharacter;
        else
            name = self.manager.rainWorld.progression.PlayingAsSlugcat;

        // Check if slugcat is Escort
        if (Eshelp_IsMe(name, true))
        {
            Ebug("Not an escort!", 1);
            orig(self);
            return;
        }

        // Grabs the completed scene depending on 
        if (SlugBaseCharacter.TryGet(name, out var chara))
        {
            if (self.IsSleepScreen) 
            {
                try
                {
                    if (pupIsAlive)
                    {
                        Ebug("Socks is alive!", 1, true);
                        if (AltSleepSceneDuo.TryGet(chara, out MenuScene.SceneID sleepTogether))
                        {
                            newScene = sleepTogether;
                        }
                    }
                    else
                    {
                        Ebug("Socks is dead!", 1, true);
                        if (AltSleepScene.TryGet(chara, out MenuScene.SceneID sleepAlone))
                        {
                            newScene = sleepAlone;
                        }
                    }
                }
                catch (Exception err)
                {
                    Ebug(err, "Something happened while trying to show sleep screen!");
                }
            }
        }

        // Set Escort sleep screen!
        if(newScene != null && newScene.Index != -1)
        {
            self.scene = new InteractiveMenuScene(self, self.pages[0], newScene);
            self.pages[0].subObjects.Add(self.scene);
            return;
        }
        else
            orig(self);        
    }



    /// <summary>
    /// Locates the pup
    /// </summary>
    /// <param name="room">Room to look in</param>
    /// <param name="abstractSocks">The abstractcreature of Socks</param>
    /// <returns>If the pup exists in the same room or not</returns>
    private bool TryFindThePup(Room room, out AbstractCreature abstractSocks)
    {
        abstractSocks = null;
        foreach (UpdatableAndDeletable thing in room.updateList)
        {
            if (thing is Player player && player.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && player.abstractCreature.ID.number is Escort.SocksID or Escort.DemonSocksID or Escort.SpeedSocksID)
            {
                abstractSocks = player.abstractCreature;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Check Escort's parry condition (LEGACY)
    /// </summary>
    /// <param name="self">A creature</param>
    /// <returns>false if not a player or Escort, true if in parry condition</returns>
    public bool Eshelp_ParryCondition(Creature self)
    {
        if (self is Player player)
        {
            if (!eCon.TryGetValue(player, out Escort e))
            {
                return false;
            }

            // Deflector extra parry check
            if (e.Deflector && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll))
            {
                Ebug(player, "Parryteched condition!", 2);
                return true;
            }

            // Regular parry check
            else if (player.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0)
            {
                Ebug(player, "Regular parry condition!", 2);
                return true;
            }

            // Not in parry condition
            else
            {
                Ebug(player, "Not in parry condition", 2);
                Ebug(player, "Parry leniency: " + e.parryAirLean, 2);
                return e.parrySlideLean > 0;
            }
        }
        return false;
    }

    /// <summary>
    /// Check Escort's parry condition
    /// </summary>
    /// <param name="self">Player</param>
    /// <returns>false if player is not escort, true if in parry condition</returns>
    public bool Eshelp_ParryCondition(Player self)
    {
        // If not escort, why bother?
        if (!eCon.TryGetValue(self, out Escort e))
        {
            return false;
        }

        // Deflector extra parry check
        if (e.Deflector && (self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.Roll))
        {
            Ebug(self, "Parryteched condition!", 2);
            return true;
        }

        // New Escapist hidden parry tech check
        if (e.NewEscapist && e.NEsAbility > 0 && (self.animation == Player.AnimationIndex.Flip))
        {
            Ebug(self, "New Escapist trickz parry condition!", 2);
            return true;
        }

        // Regular parry check
        else if (self.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0)
        {
            Ebug(self, "Regular parry condition!", 2);
            return true;
        }

        // Not in parry condition
        else
        {
            Ebug(self, "Not in parry condition", 2);
            Ebug(self, "Parry leniency: " + e.parryAirLean);
            return e.parrySlideLean > 0;
        }
    }

    /// <summary>
    /// Secondary parry condition when dropkicking to save Escort from accidental death while trying to kick creatures
    /// </summary>
    public bool Eshelp_SavingThrow(Player self, BodyChunk offender, Creature.DamageType ouchie)
    {
        // Escort check
        if (!eCon.TryGetValue(self, out Escort e))
        {
            Ebug(self, "Saving throw failed because Scug is not Escort!", 0);
            return false;
        }

        // Null check
        if (self is null || offender is null || ouchie is null)
        {
            Ebug(self, "Saving throw failed due to null values!", 0);
            return false;
        }

        // Checks whether the attacker is a creature
        if (offender.owner is not Creature)
        {
            Ebug(self, "Saving throw failed due to the offender not being a creature!", 2);
            return false;
        }

        // Checks whether easier mode is on
        if (e.easyKick)
        {
            Ebug(self, "Saving throw don't work on easier dropkicks!", 2);
            return false;
        }

        // Deflector isn't allowed a saving throw because they don't need it ;)
        if (!e.Deflector)
        {
            // For now, saving throws only apply to bites
            if (ouchie == Creature.DamageType.Bite && self.animation == Player.AnimationIndex.RocketJump)
            {
                Ebug(self, "Escort won a saving throw!", 2);
                e.savingThrowed = true;
                return true;
            }
        }

        // Fuck you, get rekt
        else
        {
            Ebug(self, "Saving throw failed: Deflector Build Moment.", 2);
        }
        return false;
    }


    /// <summary>
    /// Probably used when swapping backpacks... unused due to sudden and unknown null exception caused by the other backpack (Move to Socks' stuff)
    /// </summary>
    // private void Backpack_Realize(On.AbstractCreature.orig_Realize orig, AbstractCreature self)
    // {
    //     orig(self);
    //     try
    //     {
    //         if (self?.world?.game is null || (replaceGrapple is not null && replaceGrapple.TryGet(self.world.game, out bool rG) && !rG))
    //         {
    //             return;
    //         }
    //         if (self.Room is not null && self.Room.shelter && self.realizedCreature is not null && self.realizedCreature is TubeWorm && self.realizedCreature is not GrappleBackpack)
    //         {
    //             Ebug("Replaced Grapple with Backpack!");
    //             self.realizedCreature.Destroy();
    //             self.realizedCreature = new GrappleBackpack(self, self.world);
    //         }
    //     }
    //     catch (Exception err)
    //     {
    //         Ebug(err, "Something happened while replacing Tubeworm with GrappleBackpack!");
    //     }
    // }


    /// <summary>
    /// Changes the spawn location of Escort. Compatible with Expedition random spawns
    /// </summary>
    private void Escort_ChangingRoom(On.SaveState.orig_setDenPosition orig, SaveState self){
        orig(self);
        Ebug("Changing room 2!");
        Ebug(self.denPosition);

        // Ignores build-specific spawn for Expedition
        if (
            (ModManager.MSC && self.progression.rainWorld.safariMode) ||
            (self.progression.rainWorld.setup.startMap != "") ||
            (ModManager.Expedition && self.progression.rainWorld.ExpeditionMode)
        )
        {
            return;
        }
        
        if(self.saveStateNumber == EscortMe){
            self.denPosition = config.cfgBuild[0].Value switch {
                0 => "CC_SUMP02",  // Default
                -1 => "SU_A02",  // Brawler
                //-2 => "SI_C03",  // Deflector
                -2 => "HI_A14",  // Deflector NEW
                -3 => config.cfgOldEscapist.Value? "DM_LEG02" : "SB_B04",  // Escapist
                -4 => "GW_C02_PAST",  // Railgunner
                -5 => "LF_E03",  // Speedster
                -6 => config.cfgSectretBuild.Value? "HR_C01" : "CC_A10",  // Gilded
                -7 => "SS_A18",  // Unstable (now Barbarian, replace!)
                _ => "SB_C09"  // Unspecified
            };
            if (SChallengeMachine.SC03_Starter) self.denPosition = "CC_S05";
            Ebug("It's time OwO");
            Ebug(self.denPosition);
        }
    }

    /// <summary>
    /// Returns whether the character should be playable or not. It hides the Socks
    /// </summary>
    private static bool Escort_Playable(On.SlugcatStats.orig_SlugcatUnlocked orig, SlugcatStats.Name i, RainWorld rainWorld)
    {
        ins.L().Set();
        try
        {
            if (i is null)
            {
                Ebug("Found nulled slugcat name when checking if slugcat is unlocked or not!", 1);
                return orig(i, rainWorld);
            }
            ins.L().Set("Null Check");
            if (i == EscortMe)
            {
                ins.L().Set("Escort Check");
                return true;
                // return rainWorld.progression.miscProgressionData.beaten_SpearMaster;
            }
            if (i == EscortSocks)
            {
                ins.L().Set("Socks Check");
                // TODO: Find a way to check if Escort has been beaten or not (DONE! All that has to be done is to actually make Socks stable)
                return !UnplayableSocks;
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened when setting whether slugcat is playable or not!");
        }
        return orig(i, rainWorld);
    }


    /// <summary>
    /// I don't know what this does but this does something
    /// </summary>
    private static string[] Escort_getStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
    {
        ins.L().Set();
        try
        {
            if (i is null)
            {
                Ebug("Found nulled cat when searching for regions!");
                return orig(i);
            }
            ins.L().Set("Null Check");
            if (i == EscortMe)
            {
                ins.L().Set("Escort Check");
                return new string[]{
                    "SU",
                    "HI",
                    "DS",
                    "CC",
                    "GW",
                    "SH",
                    "VS",
                    "LM",
                    "SI",
                    "LF",
                    "UW",
                    "SS",
                    "SB",
                    "DM",
                    "OE"
                };
            }
            if (i == EscortSocks)
            {
                ins.L().Set("Socks Check");
                return new string[]
                {
                    "SU",
                    "HI",
                    "DS",
                    "CC",
                    "GW",
                    "SH",
                    "VS",
                    "LM",
                    "SI",
                    "LF",
                    "UW",
                    "SS",
                    "SB",
                    "OE"
                };
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something went wrong when getting story regions!");
        }
        return orig(i);
    }

    /// <summary>
    /// Modifies the explosive spear chance
    /// </summary>
    private static float Escort_ExpSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
    {
        ins.L().SetF();
        try
        {
            if (index is null)
            {
                Ebug("Found nulled slugcat name when getting explosive spear spawn chance!", 1);
                return orig(index);
            }
            ins.L().SetF("Null Check");
            if (index == EscortMe)
            {
                ins.L().SetF("Escort Check");
                return 0.012f;
            }
            if (index == EscortSocks)
            {
                ins.L().SetF("Socks Check");
                return 0.01f;
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened when setting exploding spear chance!");
        }
        return orig(index);
    }

    /// <summary>
    /// Modifes the electrical spear spawn chance
    /// </summary>
    private static float Escort_EleSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
    {
        ins.L().SetF();
        try
        {
            if (index is null)
            {
                Ebug("Found nulled slugcat name when getting electric spear spawn chance!", 1);
                return orig(index);
            }
            ins.L().SetF("Null Check");
            if (index == EscortMe)
            {
                ins.L().SetF("Escort Check");
                return 0.078f;
            }
            if (index == EscortSocks)
            {
                ins.L().SetF("Socks Check");
                return 0.03f;
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened when setting electric spear spawn chance!");
        }
        return orig(index);
    }

    /// <summary>
    /// Modifies the generic spear spawn chance
    /// </summary>
    private static float Escort_SpearSpawnMod(On.SlugcatStats.orig_SpearSpawnModifier orig, SlugcatStats.Name index, float originalSpearChance)
    {
        ins.L().SetF();
        try
        {
            if (index == null)
            {
                Ebug("Found nulled slugcat name when applying spear spawn chance!", 1);
                return orig(index, originalSpearChance);
            }
            ins.L().SetF("Null Check");
            if (index == EscortMe)
            {
                ins.L().SetF("Escort Check");
                return Mathf.Pow(originalSpearChance, 1.1f);
            }
            if (index == EscortSocks)
            {
                ins.L().SetF("Socks Check");
                return Mathf.Pow(originalSpearChance, 0.83f);
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened when spawning spears!");
        }
        return orig(index, originalSpearChance);
    }

    /// <summary>
    /// Replaces some regular spawned spears with Spearmaster needle versions
    /// </summary>
    private void Escort_Hipbone_Replacement(On.Room.orig_Loaded orig, Room self)
    {
        ins.L().SetF();
        orig(self);
        try
        {
            if (!(self != null && self.game != null && self.game.StoryCharacter != null && self.game.StoryCharacter.value != null))
            {
                Ebug("Found nulled slugcat name when replacing spears!", 1);
                return;
            }
            ins.L().SetF("Null Check");
            if (self.game.StoryCharacter.value != "EscortMe")
            {
                Ebug("... That's not Escort... nice try", 1);
                return;
            }
            ins.L().SetF("Escort Check");
            
            bool shelterGotPerson = false;

            // Shelter check
            if (self.abstractRoom.shelter)
            {
                Ebug("Spear swap ignores shelters!... unless QoL unfixer!", 1);
                // Though this means the game checks the room twice (and thus loops twice), it only applies to shelters so it shouldn't impact the performance too much.
                for (int i = 0; i < self.abstractRoom.entities.Count; i++)
                {
                    if (self.abstractRoom.entities[i] is AbstractCreature ac && self.game.Players.Contains(ac))
                    {
                        // Find the shelter that contains the player, to determine this is indeed the starting shelter. At one point I could have all the needle spears be saved but that's only if we got enough performance
                        shelterGotPerson = true;
                        Ebug("Player shelter!", 1);
                        break;
                    }
                }
                if (!shelterGotPerson)  // If shelter that the players don't spawn in
                {
                    return;
                }
            }
            Ebug("Attempting to replace some spears with Spearmaster's needles!", 2);
            int j = 0;  // Numbers of spears swapped
            float chance = 0.2f;  // Swap chance (default)

            // Get room-specific swap chance
            if (self.world?.region is not null && self.abstractRoom is not null)
            {
                chance = SMSM(self.world.region.name, self.abstractRoom.name);
            }

            // Replace them spears!
            for (int i = 0; i < self.abstractRoom.entities.Count; i++)
            {
                // Find a regular spear
                if (self.abstractRoom.entities[i] != null && self.abstractRoom.entities[i] is AbstractSpear spear && !spear.explosive && !spear.electric)
                {

                    if (
                        (shelterGotPerson && self.world?.game?.session is StoryGameSession s && s.saveState.miscWorldSaveData.Esave().SpearsToRemake > 0) || 
                        (!shelterGotPerson && UnityEngine.Random.value < chance)
                    )
                    {
                        // Convert a spear into a needle
                        self.abstractRoom.entities[i] = new AbstractSpear(spear.world, null, spear.pos, spear.ID, false)
                        {
                            needle = true
                        };

                        natrualSpears.Add(spear.ID);  // Add spear id of the naturally converted needles so they may be saved
                        if (shelterGotPerson)
                        {
                            // Tick down the number of spears to remake
                            (self.world.game.session as StoryGameSession).saveState.miscWorldSaveData.Esave().SpearsToRemake--;
                        }
                        j++;
                    }
                }
            }
            Ebug("Swapped " + j + " spears!");
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened while swapping spears!");
        }
    }

    /// <summary>
    /// Loads custom room settings when playing Escort campaign
    /// </summary>
    private bool Escort_Transplant(On.RoomSettings.orig_Load orig, RoomSettings self, SlugcatStats.Name index)
    {
        ins.L().SetF();
        try
        {
            if (index is null)
            {
                Ebug("Transplant failed due to nulled slugcat name!");
                return orig(self, index);
            }
            ins.L().SetF("Null Check");
            if (self is null || self.name is null)
            {
                Ebug("Transplant failed due to nulled roomSettings name");
                return orig(self, index);
            }
            ins.L().SetF("Roomsetting presence Check");
            if (index == EscortMe)
            {
                ins.L().SetF("Escort Check");
                Ebug("Roomsetting name: " + self.name);
                string p = WorldLoader.FindRoomFile(self.name, false, "_settings-escortme.txt");
                if (File.Exists(p))
                {
                    Ebug("Escort Transplanted!", 4);
                    self.filePath = p;
                }
                else
                {
                    p = WorldLoader.FindRoomFile(self.name, false, "_settings-spear.txt");
                    if (File.Exists(p))
                    {
                        Ebug("Spearmaster Transplanted!", 4);
                        self.filePath = p;
                    }
                    else
                    {
                        Ebug("No Transplant, gone default", 4);
                    }
                }
            }
            if (index == EscortSocks)
            {
                ins.L().SetF("Socks Check");
                Ebug("Roomsetting name: " + self.name);
                string p = WorldLoader.FindRoomFile(self.name, false, "_settings-escortsocks.txt");
                if (File.Exists(p))
                {
                    Ebug("Socks Transplanted!", 4);
                    self.filePath = p;
                }
                else
                {
                    p = WorldLoader.FindRoomFile(self.name, false, "_settings-artificer.txt");
                    if (File.Exists(p))
                    {
                        Ebug("Artificer Transplanted!", 4);
                        self.filePath = p;
                    }
                    else
                    {
                        Ebug("No Transplant, gone default", 4);
                    }
                }

            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened while replacing room setting file paths!");
        }
        bool theOriginal = orig(self, index);
        try
        {
            if (index is null)
            {
                Ebug("Voidmelter failed due to nulled slugcat name!");
                return orig(self, index);
            }
            if (self is null || self.name is null)
            {
                Ebug("Voidmelter failed due to nulled roomSettings name");
                return orig(self, index);
            }
            if (index == EscortMe)
            {
                foreach(RoomSettings.RoomEffect effect in self.effects)
                {
                    if (effect.type == RoomSettings.RoomEffect.Type.VoidMelt)
                    {
                        effect.amount *= 0.75f;
                        Ebug("Voidmelt effectiveness reduced by 1/4!");
                    }
                    
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Something happened while reducing voidmelt effect!");
        }
        return theOriginal;
    }

    /// <summary>
    /// Undoes the QoL fix of spearmaster spears disappearing from shelters by replacing them with regular spears before save.. assumes the game will save spear IDs
    /// </summary>
    private void Escort_SaveShelterSpears(Room room)
    {
        if (room.world?.game?.session is StoryGameSession s)
        {
            // Retrieve the remake spear data from save
            int remakeSpears = s.saveState.miscWorldSaveData.Esave().SpearsToRemake;

            // Clear spears to be remade if there is left over for whatever reason
            if (remakeSpears > 0)
            {
                remakeSpears = 0;
                Ebug("Why is there a remainder?! CALL DEATHPITS!", 1);
            }


            // Find needle spears in shelter and replace it with regular spears
            for (int i = 0; i < room.abstractRoom.entities.Count; i++)
            {
                if (room.abstractRoom.entities[i] != null && room.abstractRoom.entities[i] is AbstractSpear spear && spear.needle && natrualSpears.Contains(spear.ID))
                {
                    remakeSpears++;
                    room.abstractRoom.entities[i] = new AbstractSpear(spear.world, null, spear.pos, spear.ID, false);
                    spear.realizedObject?.Destroy();
                    (room.abstractRoom.entities[i] as AbstractSpear).RealizeInRoom();
                }
            }

            // Save remake spear data
            s.saveState.miscWorldSaveData.Esave().SpearsToRemake = remakeSpears;

            // Clear list of spears spawned by natural causes
            natrualSpears.Clear();
        }
        else
        {
            Ebug("Failed to find savestate!", 0);
        }
    }
}
