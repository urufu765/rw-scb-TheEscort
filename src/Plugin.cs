﻿using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[WIP] The Escort", "0.2.5.2")]
    partial class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public EscOptions config;
        private const string MOD_ID = "urufudoggo.theescort";
        public Plugin(){
            try{
                Plugin.instance = this;
            } catch (Exception e){
                base.Logger.LogError(e);
            }
        }


        public static readonly PlayerFeature<bool> pRTEdits = PlayerBool("playescort/realtime_edits");
        public static readonly GameFeature<bool> gRTEdits = GameBool("gameescort/realtime_edits");
        public static readonly PlayerFeature<bool> BtrPounce = PlayerBool("theescort/better_pounce");
        public static readonly GameFeature<bool> SupahMeanLizards = GameBool("theescort/mean_lizards");
        //public static readonly GameFeature<bool> SuperMeanGarbageWorms = GameBool("theescort/mean_garb_worms");



        /* JSON VALUES
        ["Stun Slide damage", "Stun Slide base stun duration", "Drop Kick base damage", "Drop Kick stun duration"]
        */
        public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static readonly PlayerFeature<float[]> SlideLaunchMod = PlayerFloats("theescort/slide_launch_mod");

        public static readonly PlayerFeature<float> LiftHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DKM = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static readonly PlayerFeature<int> Escomet = PlayerInt("theescort/headbutt");
        public static readonly PlayerFeature<bool> Elvator = PlayerBool("theescort/elevator");
        public static readonly PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");
        public static readonly PlayerFeature<bool> HypeSys = PlayerBool("theescort/adrenaline_system");
        public static readonly PlayerFeature<float> HypeReq = PlayerFloat("theescort/stamina_req");
        public static readonly PlayerFeature<int> CR = PlayerInt("theescort/reset_rate");

        /* JSON VALUES
        ["Hyped spear damage", "Base spear damage"]
        */
        public static readonly PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");
        public static readonly PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");
        public static readonly PlayerFeature<bool> soundAhoy = PlayerBool("theescort/sounds_ahoy");

        /* JSON VALUES
        [Soon]
        */
        public static readonly PlayerFeature<float[]> NoMoreGutterWater = PlayerFloats("theescort/guuh_wuuh");
        public static readonly PlayerFeature<bool> LWallJump = PlayerBool("theescort/long_wall_jump");

        /* JSON VALUES
        ["Head Y velocity", "Body Y velocity", "Head X velocity", "Body X velocity", "ConstantDownDiagnoal floor value", "ConstantDownDiagonal ceiling value", "min JumpBoost", "max JumpBoost"]
        */
        public static readonly PlayerFeature<float[]> WallJumpVal = PlayerFloats("theescort/wall_jump_val");

        /* JSON VALUES
        [Rotation val, X val, Y val]
        */
        public static readonly PlayerFeature<float[]>headDraw = PlayerFloats("theescort/headthing");
        public static readonly PlayerFeature<float[]>bodyDraw = PlayerFloats("theescort/bodything");
        



        public static readonly SlugcatStats.Name EscortMe = new SlugcatStats.Name("EscortMe");
        public static SoundID Escort_SFX_Death;
        public static SoundID Escort_SFX_Flip;
        public static SoundID Escort_SFX_Flip2;
        public static SoundID Escort_SFX_Flip3;
        public static SoundID Escort_SFX_Roll;
        public static SoundID Escort_SFX_Boop;
        public static SoundID Escort_SFX_Railgunner_Death;
        public static SoundID Escort_SFX_Lizard_Grab;
        public static SoundID Escort_SFX_Impact;
        public static SoundID Escort_SFX_Parry;
        public static SoundID Escort_SFX_Brawler_Shank;
        public static SoundID Escort_SFX_Pole_Bounce;
        //public static SoundID Escort_SFX_Spawn;

        //public DynamicSoundLoop escortRollin;

        // Miscellanious things
        private bool nonArena = false;
        //public static readonly String EscName = "EscortMe";
        /*
        Log Priority:
        -1: No logs
         0: Exceptions
         1: Important things
         2: Less important things
         3: Method pings
         4: Ebug errors (done by design)
        */
        private int logImportance = 4;
        private static int logRepetition = 0;
        private static String prevLog = "";

        // Escort instance stuff
        public static ConditionalWeakTable<Player, Escort> eCon = new();
        //private Escort e;
        private float requirement;
        private float DKMultiplier;
        float ratioed;


        // Patches
        private static bool escPatch_revivify = false;
        //private static bool escPatch_DMS = false;
        //private bool escPatch_emeraldTweaks = false;


        // Debug Logger (Beautified!)
        private static void Ebug(String message, int logPrio=3){
            if (logPrio <= instance.logImportance){
                if (message != prevLog){
                    if (logRepetition > 0){
                        Debug.Log("-> Escort: Previous message repeated " + logRepetition + " times: " + prevLog);
                    }
                    prevLog = message;
                    logRepetition = 0;
                    Debug.Log("-> Escort: " + message);
                } else {
                    logRepetition++;
                }
            }
        }
        private static void Ebug(System.Object message, int logPrio=3){
            if (logPrio <= instance.logImportance){
                Debug.Log("-> Escort: " + message.ToString());
            }
        }
        private static void Ebug(String[] messages, int logPrio=3, bool separated=true){
            if (logPrio <= instance.logImportance){
                if (separated){
                    String message = "";
                    foreach(String msg in messages){
                        message += ", " + msg;
                    }
                    Debug.Log("-> Escort: " + message.Substring(2));
                }
                else {
                    for(int i = 0; i < messages.Length; i++){
                        if (i == 0){
                            Debug.Log("-> Escort: " + messages[i]);
                        }
                        else{
                            Debug.Log("->         " + messages[i]);
                        }
                    }
                }
            }
        }
        private static void Ebug(System.Object[] messages, int logPrio=3, bool separated=true){
            if (logPrio <= instance.logImportance){
                if (separated){
                    String message = "";
                    foreach(String msg in messages){
                        message += ", " + msg.ToString();
                    }
                    Debug.Log("-> Escort: " + message.Substring(2));
                }
                else {
                    for(int i = 0; i < messages.Length; i++){
                        if (i == 0){
                            Debug.Log("-> Escort: " + messages[i].ToString());
                        }
                        else{
                            Debug.Log("->         " + messages[i].ToString());
                        }
                    }
                }
            }
        }
        private static void Ebug(Exception exception, String message="caught error!", int logPrio=0, bool asregular=false){
            if (logPrio <= instance.logImportance){
                if(asregular){
                    Debug.LogWarning("-> ERcORt: " + message + " => " + exception.Message);
                    if (exception.Source != null){
                        Debug.LogWarning("->       : " + exception.Source);
                    }
                }
                else{
                    Debug.LogError("-> ERcORt: " + message);
                    if (exception.Source != null){
                        Debug.LogError("->       : " + exception.Source);
                    }
                    Debug.LogException(exception);
                }
            }
        }
        private static void Ebug(Player self, String message, int logPrio=3){
            if (self == null){
                Ebug(message, logPrio);
            }
            try{
                if (logPrio <= instance.logImportance){
                    if (message != prevLog){
                        if (logRepetition > 0){
                            Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: Previous message repeated " + logRepetition + " times: " + prevLog);
                        }
                        prevLog = message;
                        logRepetition = 0;
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message);
                    } else {
                        logRepetition++;
                    }
                }
            } catch (Exception err){
                Ebug(message, logPrio);
                Ebug(err, logPrio:4, asregular:true);
            }
        }
        private static void Ebug(Player self, System.Object message, int logPrio=3){
            if (self == null){
                Ebug(message, logPrio);
            }
            try{
                if (logPrio <= instance.logImportance){
                    Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.ToString());
                }
            } catch (Exception err){
                Ebug(message, logPrio);
                Ebug(err, logPrio:4, asregular:true);
            }
        }
        private static void Ebug(Player self, String[] messages, int logPrio=3, bool separated=true){
            if (self == null){
                Ebug(messages, logPrio, separated);
            }
            try{
                if (logPrio <= instance.logImportance){
                    if (separated){
                        String message = "";
                        foreach(String msg in messages){
                            message += ", " + msg;
                        }
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2));
                    }
                    else {
                        for(int i = 0; i < messages.Length; i++){
                            if (i == 0){
                                Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i]);
                            }
                            else{
                                Debug.Log("->        [" + self.playerState.playerNumber + "]: " + messages[i]);
                            }
                        }
                    }
                }
            } catch (Exception err){
                Ebug(messages, logPrio, separated);
                Ebug(err, logPrio:4, asregular:true);
            }

        }
        private static void Ebug(Player self, System.Object[] messages, int logPrio=3, bool separated=true){
            if (self == null){
                Ebug(messages, logPrio, separated);
            }
            try{
                if (logPrio <= instance.logImportance){
                    if (separated){
                        String message = "";
                        foreach(String msg in messages){
                            message += ", " + msg.ToString();
                        }
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2));
                    }
                    else {
                        for(int i = 0; i < messages.Length; i++){
                            if (i == 0){
                                Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i].ToString());
                            }
                            else{
                                Debug.Log("->         [" + self.playerState.playerNumber + "]: " + messages[i].ToString());
                            }
                        }
                    }
                }
            } catch (Exception err){
                Ebug(messages, logPrio, separated);
                Ebug(err, logPrio:4, asregular:true);
            }
        }
        private static void Ebug(Player self, Exception exception, String message="caught error!", int logPrio=0, bool asregular=false){
            if (self == null){
                Ebug(exception, message, logPrio, asregular);
            }
            try {
                if (logPrio <= instance.logImportance){
                    if(asregular){
                        Debug.LogWarning("-> ERcORt[" + self.playerState.playerNumber + "]: " + message + " => " + exception.Message);
                        if (exception.Source != null){
                            Debug.LogWarning("->       : " + exception.Source);
                        }
                    }
                    else{
                        Debug.LogError("-> ERcORt[" + self.playerState.playerNumber + "]: " + message);
                        if (exception.Source != null){
                            Debug.LogError("->       [" + self.playerState.playerNumber + "]: " + exception.Source);
                        }
                        Debug.LogException(exception);
                    }
                }
            } catch (Exception err){
                Ebug(exception, message, logPrio, asregular);
                Ebug(err, logPrio:4, asregular:true);
            }
        }


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.RainWorld.PostModsInit += Escort_PostInit;

            On.Lizard.ctor += Escort_Lizard_ctor;
            //On.LizardAI.GiftRecieved += Escort_Lizard_Denial;

            On.Room.Loaded += Escort_Hipbone_Replacement;
            On.RoomSettings.Load += Escort_Transplant;

            On.PlayerGraphics.InitiateSprites += Escort_InitiateSprites;
            On.PlayerGraphics.ApplyPalette += Escort_ApplyPalette;
            On.PlayerGraphics.AddToContainer += Escort_AddGFXContainer;
            On.PlayerGraphics.DrawSprites += Escort_DrawSprites;

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
            On.Player.GrabUpdate += Esclass_RG_GrabUpdate;
            On.Player.BiteEdibleObject += Escort_Eated;
            On.Player.CanIPickThisUp += Esclass_RG_SpearGet;
            On.Player.TerrainImpact += Esclass_SS_Bonk;
            On.Player.IsCreatureLegalToHoldWithoutStun += Esclass_BL_Legality;

            On.Rock.HitSomething += Escort_RockHit;
            On.Rock.Thrown += Escort_RockThrow;

            On.ScavengerBomb.Thrown += Esclass_RG_BombThrow;

            On.MoreSlugcats.LillyPuck.Thrown += Esclass_RG_LillyThrow;

            On.Weapon.WeaponDeflect += Esclass_RG_AntiDeflect;

            On.SlugcatStats.SpearSpawnModifier += Escort_SpearSpawnMod;
            On.SlugcatStats.SpearSpawnElectricRandomChance += Escort_EleSpearSpawnChance;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance += Escort_ExpSpearSpawnChance;
            On.SlugcatStats.getSlugcatStoryRegions += Escort_getStoryRegions;
            }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Escort_SFX_Death = new SoundID("Escort_Failure", true);
            Escort_SFX_Flip = new SoundID("Escort_Flip", true);
            Escort_SFX_Roll = new SoundID("Escort_Roll", true);
            Escort_SFX_Boop = new SoundID("Escort_Boop", true);
            Escort_SFX_Railgunner_Death = new SoundID("Escort_Rail_Fail", true);
            Escort_SFX_Lizard_Grab = new SoundID("Escort_Liz_Grab", true);
            Escort_SFX_Impact = new SoundID("Escort_Impact", true);
            Escort_SFX_Parry = new SoundID("Escort_Parry", true);
            FAtlas aB, aH;
            Escort_SFX_Flip2 = new SoundID("Escort_Flip_More", true);
            Escort_SFX_Flip3 = new SoundID("Escort_Flip_Even_More", true);
            Escort_SFX_Brawler_Shank = new SoundID("Escort_Brawl_Shank", true);
            Escort_SFX_Pole_Bounce = new SoundID("Escort_Pole_Bounce", true);
            aB = Futile.atlasManager.LoadAtlas("atlases/escorthip");
            aH = Futile.atlasManager.LoadAtlas("atlases/escorthead");
            if (aB == null || aH == null){
                Ebug("Oh no. Sprites dead.", 0);
            }
            //Escort_SFX_Spawn = new SoundID("Escort_Spawn", true);
            Ebug("All SFX loaded!", 1);
            EscEnums.RegisterValues();  // TODO: do something with this
            this.config = new EscOptions(rainWorld);
            MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            Ebug("All loaded!", 1);
        }


        private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self){
            orig(self);

            // Look for mods...
            try{
            if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify")){
                Ebug("Found Revivify! Applying patch...", 1);
                escPatch_revivify = true;
            }
            if (ModManager.ActiveMods.Exists(mod => mod.id == "dressmyslugcat")){
                Ebug("Found Dress My Slugcat!", 1);
                //escPatch_DMS = true;
                Espatch_DMS(ModManager.ActiveMods.Find(mod => mod.id == "dressmyslugcat"));
            }
            } catch (Exception err){
                Ebug(err, "Something happened while searching for mods!");
            }
        }

        private static void Espatch_DMS(ModManager.Mod dms){
            try{// Dress My Slugcat Patch
                //if (dms.version)
                Ebug("Found DMS Version: " + dms.version, 1);
                String[] dmsVer = dms.version.Split('.');
                if(int.TryParse(dmsVer[1], out int verMin) && verMin >= 3){
                    Ebug("Applying patch!...", 1);
                    if(verMin == 3 && int.TryParse(dmsVer[2], out int verPatch) && verPatch == 0){
                        DressMySlugcat.SpriteDefinitions.AddSprite(new DressMySlugcat.SpriteDefinitions.AvailableSprite{
                            Name = "WAIT 4 NEXT DMS PATCH",
                            Description = "Please wait for next patch (will likely softcrash upon clicking on customize)",
                            GallerySprite = "escortHipT",
                            RequiredSprites = new List<string> {"escortHeadT", "escortHipT"},
                            Slugcats = new List<string>{"EscortMe"}
                        });
                    }
                    else {
                        DressMySlugcat.SpriteDefinitions.AddSprite(new DressMySlugcat.SpriteDefinitions.AvailableSprite{
                            Name = "MARKINGS",
                            Description = "Markings",
                            GallerySprite = "escortHipT",
                            RequiredSprites = new List<string> {"escortHeadT", "escortHipT"},
                            Slugcats = new List<string>{"EscortMe"}
                        });
                    }
                }
                else {
                    //Ebug(self, "Using dud patch... (update your DMS!)", 1);
                    Ebug("Using dud patch...", 1);
                    DressMySlugcat.SpriteDefinitions.AvailableSprites.Add(new DressMySlugcat.SpriteDefinitions.AvailableSprite{
                        //Name = "UPDATEYOURDMS!",
                        //Description = "Update Your DMS",
                        Name = "MARKINGS",
                        Description = "Markings",
                        GallerySprite = "escortHipT",
                        RequiredSprites = new List<string> {"escortHeadT", "escortHipT"},
                        Slugcats = new List<string>{"EscortMe"}
                    });
                }
            } catch (Exception merr){
                //escPatch_DMS = false;
                Ebug(merr, "Couldn't patch Dress Me Sluggie because...");
            }
        }
        

        /*
        Configurations!
        */
        private bool Esconfig_Mean_Lizards(World self){
            if (!gRTEdits.TryGet(self.game, out bool RT) || !SupahMeanLizards.TryGet(self.game, out bool meanLizard)){
                return false;
            }
            if (RT){
                return meanLizard;
            } else {
                return config.cfgMeanLizards.Value;
            }
        }

        private bool Esconfig_Heavylift(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !LiftHeavy.TryGet(self, out float power)){
                ratioed = 3f;
                return false;
            }
            if (RT){
                ratioed = power;
            } else {
                ratioed = config.cfgHeavyLift.Value;
            }
            return true;
        }

        private bool Esconfig_DKMulti(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !DKM.TryGet(self, out float dk)){
                return false;
            }
            if (RT){
                DKMultiplier = dk;
            } else {
                DKMultiplier = config.cfgDKMult.Value;
            }
            return true;
        }

        private bool Esconfig_Elevator(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !Elvator.TryGet(self, out bool yeet)){
                return false;
            }
            if (RT){
                return yeet;
            } else {
                return config.cfgElevator.Value;
            }
        }

        private bool Esconfig_Hypable(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !HypeSys.TryGet(self, out bool hm)){
                return false;
            }
            if (RT){
                return hm;
            } else {
                return config.cfgHypable.Value;
            }
        }

        private bool Esconfig_HypeReq(Player self, float require=0.8f){
            if (!pRTEdits.TryGet(self, out bool RT) || !HypeReq.TryGet(self, out float req)){
                return false;
            }
            if (RT){
                requirement = req;
            } else {
                switch(config.cfgHypeReq.Value){
                    case 0:
                        requirement = -1f; break;
                    case 1:
                        requirement = 0.5f; break;
                    case 2:
                        requirement = 0.66f; break;
                    case 3:
                        requirement = 0.75f; break;
                    case 4:
                        requirement = 0.8f; break;
                    case 5:
                        requirement = 0.87f; break;
                    case 6:
                        requirement = 0.92f; break;
                    default:
                        requirement = require; break;
                };
            }
            return true;
        }

        private bool Esconfig_SFX(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !soundAhoy.TryGet(self, out bool soundFX)){
                return false;
            }
            if (RT){
                return soundFX;
            } else {
                return config.cfgSFX.Value;
            }
        }

        private bool Esconfig_WallJumps(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !LWallJump.TryGet(self, out bool wallJumper)){
                return false;
            }
            if (RT){
                return wallJumper;
            } else {
                return config.cfgLongWallJump.Value;
            }
        }

        private bool Esconfig_Pouncing(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !BtrPounce.TryGet(self, out bool pouncing)){
                return false;
            }
            if (RT){
                return pouncing;
            } else {
                return config.cfgPounce.Value;
            }
        }

        private bool Esconfig_Dunkin(Player self){
            return config.cfgDunkin.Value;
        }

        private bool Esconfig_Spears(Player self){
            try {
                if (!eCon.TryGetValue(self, out Escort e)){
                    return false;
                }
                if (config.cfgSpears.Value){
                    return e.tossEscort;
                }
                return false;
            }
            catch (Exception err){
                Ebug(self, err, "Something went wrong when setting an Escort build!");
                return false;
            }
        }

        private bool Esconfig_Build(Player self){
            try {
                if (!eCon.TryGetValue(self, out Escort e)){
                    return false;
                }
                int pal = 0;
                bool help = false;
                switch (self.playerState.playerNumber){
                    case 0:
                        pal = config.cfgBuildP1.Value;
                        help = config.cfgEasyP1.Value;
                        break;
                    case 1:
                        pal = config.cfgBuildP2.Value;
                        help = config.cfgEasyP2.Value;
                        break;
                    case 2:
                        pal = config.cfgBuildP3.Value;
                        help = config.cfgEasyP3.Value;
                        break;
                    case 3:
                        pal = config.cfgBuildP4.Value;
                        help = config.cfgEasyP4.Value;
                        break;
                }
                switch (pal){
                    // Unstable build (Longer you're in battlehype, the more the explosion does. Trigger explosion on a dropkick)
                    // Stylist build (Do combos that build up to a super move)
                    // Ultrakill build (Pressing throw while there's nothing in main hand will send a grapple tongue, which if it latches onto creature, pulls Escort to eavy creatures, and light creatures to Escort. Throwing while having a rock in main hand will do melee/parry, having bomb in main hand will melee/knockback. Sliding also is fast and feet first. While midair, pressing down+jump will stomp)
                    // Stealth build (hold still or crouch to enter stealthed mode)
                    // Speedstar build (Fast, and when running for a certain amount of time, apply BOOST that also does damage on collision)
                    case -5:  // Speedstar build
                        e.Speedster = true;
                        self.slugcatStats.lungsFac -= 0.1f;
                        self.slugcatStats.bodyWeightFac += 0.1f;
                        self.slugcatStats.corridorClimbSpeedFac += 1.0f;
                        self.slugcatStats.poleClimbSpeedFac += 0.6f;
                        self.slugcatStats.runspeedFac += 0.35f;
                        Ebug(self, "Speedstar Build selected!", 2);
                        break;
                    case -4:  // Railgunner build
                        e.Railgunner = true;
                        self.slugcatStats.lungsFac = 1.3f;
                        self.slugcatStats.throwingSkill = 2;
                        self.slugcatStats.loudnessFac += 2f;
                        self.slugcatStats.generalVisibilityBonus += 1f;
                        self.slugcatStats.visualStealthInSneakMode = 0f;
                        self.slugcatStats.bodyWeightFac += 0.3f;
                        Ebug(self, "Railgunner Build selected!", 2);
                        break;
                    case -3:  // Escapist build
                        e.Escapist = true;
                        e.dualWield = false;
                        self.slugcatStats.runspeedFac += 0.1f;
                        Ebug(self, "Escapist Build selected!", 2);
                        break;
                    case -2:  // Deflector build
                        e.Deflector = true;
                        self.slugcatStats.runspeedFac = 1f;
                        self.slugcatStats.corridorClimbSpeedFac = 1f;
                        self.slugcatStats.poleClimbSpeedFac = 1f;
                        self.slugcatStats.bodyWeightFac = 1f;
                        Ebug(self, "Deflector Build selected!", 2);
                        break;
                    case -1:  // Brawler build
                        e.Brawler = true;
                        e.tossEscort = false;
                        self.slugcatStats.runspeedFac -= 0.2f;
                        self.slugcatStats.corridorClimbSpeedFac -= 0.4f;
                        self.slugcatStats.poleClimbSpeedFac -= 0.4f;
                        self.slugcatStats.throwingSkill = 1;
                        Ebug(self, "Brawler Build selected!", 2);
                        break;
                    default:  // Default build
                        Ebug(self, "Default Build selected!", 2);
                        break;
                }
                e.easyMode = help;
                if (e.easyMode){
                    Ebug(self, "Easy Mode active!");
                }
                self.slugcatStats.lungsFac += self.Malnourished? 0.15f : -0.1f;
                Ebug(self, "Set build complete!", 1);
                Ebug(self, "Movement Speed: " + self.slugcatStats.runspeedFac, 2);
                Ebug(self, "Lung capacity fac: " + self.slugcatStats.lungsFac, 2);
                return true;
            } catch (Exception err){
                Ebug(self, err, "Something went wrong when setting an Escort build!");
                return false;
            }
        }

        /*
        Escort code!
        */
        private SoundID Eshelp_SFX_Flip(){
            float r = UnityEngine.Random.value;
            switch (r){
                case var _ when r > 0.5f:
                    return Escort_SFX_Flip;
                case var _ when r > 0.3f:
                    return Escort_SFX_Flip2;
                case var _ when r > 0:
                    return Escort_SFX_Flip3;
            }
            return Escort_SFX_Flip;
        }

        // Implement lizard aggression (edited from template)
        private void Escort_Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            
            orig(self, abstractCreature, world);

            if(Esconfig_Mean_Lizards(world))
            {
                Ebug("Lizard Ctor Triggered!");
                self.spawnDataEvil = Mathf.Max(self.spawnDataEvil, 100f);
            }
        }
        /*
        private void Escort_Lizard_Denial(On.LizardAI.orig_GiftRecieved orig, LizardAI self, SocialEventRecognizer.OwnedItemOnGround giftOfferedToMe)
        {
            try{
                if (giftOfferedToMe != null && giftOfferedToMe.owner != null && giftOfferedToMe.owner is Player p){
                    if (p.slugcatStats.name.value == "EscortMe" && config.cfgMeanLizards.Value){
                        giftOfferedToMe.active = false;
                        giftOfferedToMe.offered = false;
                        giftOfferedToMe.owner = null;
                    }
                }
                orig(self, giftOfferedToMe);
            } catch (Exception err){
                orig(self, giftOfferedToMe);
                Ebug(err, "Exception when lizard likes!");
            }
        }*/


        private void Escort_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            Ebug(self, "Ctor Triggered!");
            orig(self, abstractCreature, world);

            if (self.slugcatStats.name.value == "EscortMe"){
                eCon.Add(self, new Escort(self));
                if (!eCon.TryGetValue(self, out Escort e)){
                    Ebug(self, "Something happened while initializing then accessing Escort instance!", 0);
                    return;
                }
                Esconfig_Build(self);
                try {
                    Ebug(self, "Setting silly sounds", 2);
                    e.Escat_setSFX_roller(Escort_SFX_Roll);
                    e.Escat_setSFX_lizgrab(Escort_SFX_Lizard_Grab);
                    Ebug(self, "All done! Awaiting activation.", 2);
                    
                    /*
                    Color col = new Color(0.796f, 0.549f, 0.27843f);
                    e.Esclass_set_hypeLight(self, col);
                    Ebug(self, "Setting hyped light", 2);
                    */
                    // April fools!
                    //self.setPupStatus(set: true);
                    //self.room.PlaySound(Escort_SFX_Spawn, self.mainBodyChunk);
                } catch (Exception err){
                    Ebug(self, err, "Error while constructing!");
                } finally {
                    Ebug(self, "All ctor'd", 1);
                }
            }
        }

        private void Escort_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam)
        {
            orig(self, s, rCam);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }
                e.Escat_setIndex_sprite_cue(s.sprites.Length);
                Array.Resize(ref s.sprites, s.sprites.Length + 2);
                //Ebug(self.player, "Resized array", 1);
                s.sprites[e.spriteQueue] = new FSprite("escortHeadT");
                s.sprites[e.spriteQueue + 1] = new FSprite("escortHipT");
                if (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null){
                    Ebug(self.player, "Oh geez. No sprites?", 0);
                }
                //Ebug(self.player, "Set the sprites", 1);
                self.AddToContainer(s, rCam, null);
                //Ebug(self.player, "Sprite init complete!", 1);
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when initiating sprites!");
                return;
            }
        }

        private void Escort_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, RoomPalette palette){
            orig(self, s, rCam, palette);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }

                if (e.spriteQueue == -1){
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    Ebug(self.player, "Oh dear. Null sprites!!", 0);
                    return;
                }
                Color c = new Color(0.796f, 0.549f, 0.27843f);
                // Applying colors?
                if (s.sprites.Length > e.spriteQueue){
                    //Ebug(self.player, "Gone in", 2);
                    if (ModManager.CoopAvailable && self.useJollyColor){
                        //Ebug(self.player, "Jollymachine", 2);
                        c = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        //Ebug(self.player, "R: " + c.r + " G: " + c.g + " B: " + c.b);
                        //Ebug(self.player, "Jollymachine end", 2);
                    }
                    else if (PlayerGraphics.CustomColorsEnabled()){
                        Ebug(self.player, "Custom color go brr", 2);
                        c = PlayerGraphics.CustomColorSafety(2);
                        Ebug(self.player, "Custom color end", 2);
                    }
                    else{
                        //==Ebug(self.player, "Arenasession or Singleplayer", 2);

                        if (rCam.room.game.IsArenaSession && !rCam.room.game.setupValues.arenaDefaultColors){
                            switch(self.player.playerState.playerNumber){
                                case 0:
                                    if (rCam.room.game.IsArenaSession && rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcats.MoreSlugcatsEnums.GameTypeID.Challenge && !nonArena){
                                        c = new Color(0.2f, 0.6f, 0.77f);
                                    }
                                    break;
                                case 1:
                                    c = new Color(0.26f, 0.68f, 0.21f);
                                    break;
                                case 2:
                                    c = new Color(0.55f, 0.11f, 0.55f);
                                    break;
                                case 3:
                                    c = new Color(0.91f, 0.7f, 0.9f);
                                    break;
                            }
                        }
                        Ebug(self.player, "Arena/Single end.", 2);
                    }
                }
                if (c.r <= 0.02f && c.g <= 0.02f && c.b <= 0.02f){
                    e.secretRGB = true;
                }
                for (int i = e.spriteQueue; i < s.sprites.Length; i++){
                    s.sprites[i].color = e.Escat_runit_thru_RGB(c);
                }
                if (!e.secretRGB){
                    e.hypeColor = c;
                }
                if (e.hypeLight == null || e.hypeSurround == null){
                    e.Escat_setLight_hype(self.player, e.hypeColor);
                }
                else{
                    e.hypeLight.color = c;
                    e.hypeSurround.color = c;
                }
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when coloring in the palette!");
                return;
            }
        }

        private void Escort_AddGFXContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, FContainer newContainer){
            orig(self, s, rCam, newContainer);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }
                if (e.spriteQueue == -1){
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    Ebug(self.player, "Oh shoot. Where sprites?", 0);
                    return;
                }
                if (e.spriteQueue < s.sprites.Length){
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue + 1]);
                    Ebug(self.player, "Removal success.", 1);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue + 1]);
                    Ebug(self.player, "Addition success.", 1);
                    s.sprites[e.spriteQueue].MoveBehindOtherNode(s.sprites[3]);
                    s.sprites[e.spriteQueue].MoveBehindOtherNode(s.sprites[9]);
                    //Ebug(self.player, "Restructure success.", 1);
                }
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when adding graphics to container!");
                return;
            }
        }

        private void Escort_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP){
            orig(self, s, rCam, t, camP);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!headDraw.TryGet(self.player, out var hD) || !bodyDraw.TryGet(self.player, out var bD) || !eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }
                if (e.spriteQueue == -1){
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    Ebug(self.player, "Oh crap. Sprites? Hello?!", 0);
                    return;
                }
                if (s.sprites.Length > e.spriteQueue){
                    // Hypelevel visual fx
                    try{
                        if (self.player != null && Esconfig_Hypable(self.player)){
                            float alphya = 1f;
                            if (requirement > self.player.aerobicLevel){
                                alphya = Mathf.Lerp((self.player.dead? 0f : 0.57f), 1f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                            }
                            for (int a = e.spriteQueue; a < s.sprites.Length; a++){
                                s.sprites[a].alpha = alphya;
                                s.sprites[a].color = e.Escat_runit_thru_RGB(e.hypeColor, (requirement < self.player.aerobicLevel? 8f : Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel)))) * Mathf.Lerp(1f, 1.8f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                            }
                            if (e.hypeLight != null && e.hypeSurround != null){
                                float alpine = 0f;
                                float alpite = 0f;

                                if (requirement > self.player.aerobicLevel){
                                    alpine = Mathf.Lerp(0f, 0.08f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                                    alpite = Mathf.Lerp(0f, 0.2f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                                }
                                else {
                                    alpine = 0.1f;
                                    alpite = 0.3f;
                                }
                                e.hypeLight.stayAlive = true;
                                e.hypeSurround.stayAlive = true;
                                e.hypeLight.setPos = self.player.mainBodyChunk.pos;
                                e.hypeSurround.setPos = self.player.bodyChunks[0].pos;
                                e.hypeLight.setAlpha = alpine;
                                e.hypeSurround.setAlpha = alpite;
                                if (e.secretRGB){
                                    e.hypeLight.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                    e.hypeSurround.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                }
                            }
                            else {
                                e.Escat_setLight_hype(self.player, e.hypeColor);
                            }
                        }
                    } catch (Exception err){
                        Ebug(self.player, err, "something went wrong when altering alpha");
                    }
                    s.sprites[e.spriteQueue].rotation = s.sprites[9].rotation;
                    s.sprites[e.spriteQueue + 1].rotation = s.sprites[1].rotation;
                    s.sprites[e.spriteQueue].scaleX = hD[0];
                    s.sprites[e.spriteQueue + 1].scaleX = bD[0];
                    if (self.player.animation == Player.AnimationIndex.Flip || self.player.animation == Player.AnimationIndex.Roll){
                        Vector2 vectoria = RWCustom.Custom.DegToVec(s.sprites[9].rotation) * hD[1];
                        Vector2 vectorib = RWCustom.Custom.DegToVec(s.sprites[1].rotation) * bD[1];
                        s.sprites[e.spriteQueue].x = s.sprites[9].x + vectoria.x;
                        s.sprites[e.spriteQueue].y = s.sprites[9].y + vectoria.y;
                        s.sprites[e.spriteQueue + 1].x = s.sprites[1].x + vectorib.x;
                        s.sprites[e.spriteQueue + 1].y = s.sprites[1].y + vectorib.y;
                    } else {
                        s.sprites[e.spriteQueue].x = s.sprites[9].x + hD[2];
                        s.sprites[e.spriteQueue].y = s.sprites[9].y + hD[3];
                        s.sprites[e.spriteQueue + 1].x = s.sprites[1].x + bD[2];
                        s.sprites[e.spriteQueue + 1].y = s.sprites[1].y + bD[3];
                    }
                }
                if (e.Speedster) Esclass_SS_DrawSprites(self, s, rCam, t, camP, ref e);
            } catch (Exception err){
                Ebug(self.player, err, "Something happened while trying to draw sprites!");
            }
        }


        // Check Escort's parry condition
        public bool Eshelp_ParryCondition(Creature self){
            if (self is Player player){
                if (!eCon.TryGetValue(player, out Escort e)){
                    return false;
                }
                if (e.Deflector && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll)){
                    Ebug(player, "Parryteched condition!", 2);
                    return true;
                }
                else if (player.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0){
                    Ebug(player, "Regular parry condition!", 2);
                    return true;
                }
                else {
                    Ebug(player, "Not in parry condition", 2);
                    Ebug(player, "Parry leniency: " + e.parryAirLean, 2);
                    return e.parrySlideLean > 0;
                }
            }
            return false;
        }
        public bool Eshelp_ParryCondition(Player self){ 
            if (!eCon.TryGetValue(self, out Escort e)){
                return false;
            }
            if (e.Deflector && (self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.Roll)){
                Ebug(self, "Parryteched condition!", 2);
                return true;
            }
            else if (self.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0){
                Ebug(self, "Regular parry condition!", 2);
                return true;
            }
            else {
                Ebug(self, "Not in parry condition", 2);
                Ebug(self, "Parry leniency: " + e.parryAirLean);
                return e.parrySlideLean > 0;
            }
        }

        // Secondary parry condition when dropkicking to save Escort from accidental death while trying to kick creatures
        public bool Eshelp_SavingThrow(Player self, BodyChunk offender, Creature.DamageType ouchie){
            if (!eCon.TryGetValue(self, out Escort e)){
                Ebug(self, "Saving throw failed because Scug is not Escort!", 0);
                return false;
            }
            if (!(self != null && offender != null && ouchie != null)){
                Ebug(self, "Saving throw failed due to null values!", 0);
                return false;
            }
            if (offender.owner is not Creature){
                Ebug(self, "Saving throw failed due to the offender not being a creature!", 2);
                return false;
            }
            if (e.easyKick){
                Ebug(self, "Saving throw don't work on easier dropkicks!", 2);
                return false;
            }
            // Deflector isn't allowed a saving throw because they don't need it ;)
            if (!e.Deflector){
                // For now, saving throws only apply to bites
                if (ouchie == Creature.DamageType.Bite && self.animation == Player.AnimationIndex.RocketJump){
                    Ebug(self, "Escort won a saving throw!", 2);
                    e.savingThrowed = true;
                    return true;
                }
            }
            else {
                Ebug(self, "Saving throw failed: Deflector Build Moment.", 2);
            }
            return false;
        }



        private static string[] Escort_getStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
        {
            try {
                if (i.value == "EscortMe"){
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
                        "DM"
                    };
                } else {
                    return orig(i);
                }
            } catch (Exception err){
                Ebug(err, "Something went wrong when getting story regions!");
                return orig(i);
            }
        }

        private static float Escort_ExpSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
        {
            try{
                if (!(index != null && index.value != null)){
                    Ebug("Found nulled slugcat name when getting explosive spear spawn chance!", 1);
                    return orig(index);
                }
                if (index.value == "EscortMe"){
                    return 0.012f;
                } else {
                    return orig(index);
                }
            } catch (Exception err){
                Ebug(err, "Something happened when setting exploding spear chance!");
                return orig(index);
            }
        }

        private static float Escort_EleSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
        {   
            try{
                if (!(index != null && index.value != null)){
                    Ebug("Found nulled slugcat name when getting electric spear spawn chance!", 1);
                    return orig(index);
                }
                if (index.value == "EscortMe"){
                    return 0.078f;
                } else {
                    return orig(index);
                }
            } catch (Exception err){
                Ebug(err, "Something happened when setting electric spear spawn chance!");
                return orig(index);
            }
        }

        private static float Escort_SpearSpawnMod(On.SlugcatStats.orig_SpearSpawnModifier orig, SlugcatStats.Name index, float originalSpearChance)
        {
            try{
                if (!(index != null && index.value != null)){
                    Ebug("Found nulled slugcat name when applying spear spawn chance!", 1);
                    return orig(index, originalSpearChance);
                }
                if (index.value == "EscortMe"){
                    return Mathf.Pow(originalSpearChance, 1.1f);
                } else {
                    return orig(index, originalSpearChance);
                }
            } catch (Exception err){
                Ebug(err, "Something happened when spawning spears!");
                return orig(index, originalSpearChance);
            }
        }

        private void Escort_Hipbone_Replacement(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);
            try{
                if (!(self != null && self.game != null && self.game.StoryCharacter != null && self.game.StoryCharacter.value != null)){
                    Ebug("Found nulled slugcat name when replacing spears!", 1);
                    return;
                }
                if (self.game.StoryCharacter.value != "EscortMe"){  
                    Ebug("... That's not Escort... nice try", 1);
                    return;
                }
                if (self.abstractRoom.shelter){
                    Ebug("Spear swap ignores shelters!", 1);
                    return;
                }
                Ebug("Attempting to replace some spears with Spearmaster's needles!", 2);
                int j = 0;
                for (int i = 0; i < self.abstractRoom.entities.Count; i++){
                    if (self.abstractRoom.entities[i] != null && self.abstractRoom.entities[i] is AbstractSpear spear){
                        if (UnityEngine.Random.value > 0.8f && !spear.explosive && !spear.electric){
                            self.abstractRoom.entities[i] = new AbstractSpear(spear.world, null, spear.pos, spear.ID, false){
                                needle = true
                            };
                            j++;
                        }
                    }
                }
                /*
                foreach (AbstractPhysicalObject a in self.abstractRoom.entities){
                    if (a != null && a is AbstractSpear spear){
                        if (UnityEngine.Random.value > 0.67f && !spear.explosive && !spear.electric){
                            a = new AbstractSpear(self.world, null, a.pos, a.ID, false){
                                needle = true
                            };
                        }
                    }
                }*/
                Ebug("Swapped " + j + " spears!");
            } catch (Exception err){
                Ebug(err, "Something happened while swapping spears!");
            }
        }

        private bool Escort_Transplant(On.RoomSettings.orig_Load orig, RoomSettings self, SlugcatStats.Name index)
        {
            try{
                if (index == null){
                    Ebug("Transplant failed due to nulled slugcat name!");
                    return orig(self, index);
                }
                if (self == null || self.name == null){
                    Ebug("Transplant failed due to nulled roomSettings name");
                    return orig(self, index);
                }
                if (index != EscortMe){
                    Ebug("Not Escort!", 1);
                    return orig(self, index);
                }
                Ebug("Roomsetting name: " + self.name);
                string p = WorldLoader.FindRoomFile(self.name, false, "_settings-escortme.txt");
                if (File.Exists(p)){
                    Ebug("Escort Transplanted!", 4);
                    self.filePath = p;
                } else {
                    p = WorldLoader.FindRoomFile(self.name, false, "_settings-spear.txt");
                    if (File.Exists(p)){
                        Ebug("Spearmaster Transplanted!", 4);
                        self.filePath = p;
                    } else {
                        Ebug("No Transplant, gone default", 4);
                    }
                }
                return orig(self, index);
            } catch (Exception err){
                Ebug(err, "Something happened while replacing room setting file paths!");
                return orig(self, index);
            }
        }
    }
}