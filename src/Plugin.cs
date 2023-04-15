using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[WIP] The Escort", "0.2.4")]
    class Plugin : BaseUnityPlugin
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
        public static readonly GameFeature<bool> SuperMeanGarbageWorms = GameBool("theescort/mean_garb_worms");

        /* JSON VALUES
        ["Unhyped speed", "Hyped speed", "Malnourished unhyped", "Malnourished hyped", "Malnourished unhyped", "hype-disabled", "Malnourished hype-disabled"]
        */
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");

        /* JSON VALUES
        ["Unhyped speed", "Hyped speed", "Malnourished unhyped", "Malnourished hyped", "Malnourished unhyped", "hype-disabled", "Malnourished hype-disabled"]
        */
        public static readonly PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");

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
        public static readonly PlayerFeature<float[]>
        WallJumpVal = PlayerFloats("theescort/wall_jump_val");

        /* JSON VALUES
        [Rotation val, X val, Y val]
        */
        public static readonly PlayerFeature<float[]>headDraw = PlayerFloats("theescort/headthing");
        public static readonly PlayerFeature<float[]>bodyDraw = PlayerFloats("theescort/bodything");
        
        // Brawler tweak values
        // public static readonly PlayerFeature<> brawler = Player("theescort/brawler/");
        // public static readonly PlayerFeature<float> brawler = PlayerFloat("theescort/brawler/");
        // public static readonly PlayerFeature<float[]> brawler = PlayerFloats("theescort/brawler/");
        public static readonly PlayerFeature<float> brawlerSlideLaunchFac = PlayerFloat("theescort/brawler/slide_launch_fac");
        public static readonly PlayerFeature<float> brawlerDKHypeDmg = PlayerFloat("theescort/brawler/dk_h_dmg");
        public static readonly PlayerFeature<float[]> brawlerSpearVelFac = PlayerFloats("theescort/brawler/spear_vel_fac");
        public static readonly PlayerFeature<float[]> brawlerSpearDmgFac = PlayerFloats("theescort/brawler/spear_dmg_fac");
        public static readonly PlayerFeature<float> brawlerSpearThrust = PlayerFloat("theescort/brawler/spear_thrust");
        public static readonly PlayerFeature<float[]> brawlerSpearShankY = PlayerFloats("theescort/brawler/spear_shank");

        // Deflector tweak values
        // public static readonly PlayerFeature<> deflector = Player("theescort/deflector/");
        // public static readonly PlayerFeature<float> deflector = PlayerFloat("theescort/deflector/");
        // public static readonly PlayerFeature<float[]> deflector = PlayerFloats("theescort/deflector/");
        public static readonly PlayerFeature<float> deflectorSlideDmg = PlayerFloat("theescort/deflector/slide_dmg");
        public static readonly PlayerFeature<float> deflectorSlideLaunchFac = PlayerFloat("theescort/deflector/slide_launch_fac");
        public static readonly PlayerFeature<float> deflectorSlideLaunchMod = PlayerFloat("theescort/deflector/slide_launch_mod");
        public static readonly PlayerFeature<float[]> deflectorDKHypeDmg = PlayerFloats("theescort/deflector/dk_h_dmg");
        public static readonly PlayerFeature<float[]> deflectorSpearVelFac= PlayerFloats("theescort/deflector/spear_vel_fac");
        public static readonly PlayerFeature<float[]> deflectorSpearDmgFac = PlayerFloats("theescort/deflector/spear_dmg_fac");

        // Escapist tweak values
        // public static readonly PlayerFeature<> escapist = Player("theescort/escapist/");
        // public static readonly PlayerFeature<float> escapist = PlayerFloat("theescort/escapist/");
        // public static readonly PlayerFeature<float[]> escapist = PlayerFloats("theescort/escapist/");
        public static readonly PlayerFeature<float> escapistSlideLaunchMod = PlayerFloat("theescort/escapist/slide_launch_mod");
        public static readonly PlayerFeature<float> escapistSlideLaunchFac = PlayerFloat("theescort/escapist/slide_launch_fac");
        public static readonly PlayerFeature<float> escapistSpearVelFac= PlayerFloat("theescort/escapist/spear_vel_fac");
        public static readonly PlayerFeature<int[]> escapistNoGrab = PlayerInts("theescort/escapist/no_grab");
        public static readonly PlayerFeature<int> escapistCD = PlayerInt("theescort/escapist/cd");
        public static readonly PlayerFeature<float> escapistColor = PlayerFloat("theescort/escapist/color");

        // Railgunner tweak values
        // public static readonly PlayerFeature<> railgun = Player("theescort/railgunner/");
        // public static readonly PlayerFeature<float> railgun = PlayerFloat("theescort/railgunner/");
        // public static readonly PlayerFeature<float[]> railgun = PlayerFloats("theescort/railgunner/");
        public static readonly PlayerFeature<float[]> railgunSpearVelFac = PlayerFloats("theescort/railgunner/spear_vel_fac");
        public static readonly PlayerFeature<float> railgunSpearDmgFac = PlayerFloat("theescort/railgunner/spear_dmg_fac");
        public static readonly PlayerFeature<float[]> railgunSpearThrust = PlayerFloats("theescort/railgunner/spear_thrust");
        public static readonly PlayerFeature<float> railgunRockVelFac = PlayerFloat("theescort/railgunner/rock_vel_fac");
        public static readonly PlayerFeature<float> railgunLillyVelFac = PlayerFloat("theescort/railgunner/lilly_vel_fac");
        public static readonly PlayerFeature<float> railgunBombVelFac = PlayerFloat("theescort/railgunner/bomb_vel_fac");
        public static readonly PlayerFeature<float[]> railgunRockThrust = PlayerFloats("theescort/railgunner/rock_thrust");



        public static SoundID Escort_SFX_Death;
        public static SoundID Escort_SFX_Flip;
        public static SoundID Escort_SFX_Roll;
        public static SoundID Escort_SFX_Boop;
        public static SoundID Escort_SFX_Railgunner_Death;
        public static SoundID Escort_SFX_Lizard_Grab;
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
                Debug.Log("-> Escort: " + message);
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
                    Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message);
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

            On.Room.Loaded += Escort_Hip_Replacement;

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
            On.Player.DeathByBiteMultiplier += Escort_DeathBiteMult;
            On.Player.TossObject += Escort_TossObject;
            On.Player.ThrowObject += Escort_ThrowObject;
            On.Player.SpearStick += Escort_StickySpear;
            On.Player.Grabbed += Escort_Grabbed;
            On.Player.GrabUpdate += Escort_GrabUpdate;
            On.Player.BiteEdibleObject += Escort_Eated;

            On.Rock.HitSomething += Escort_RockHit;
            On.Rock.Thrown += Escort_RockThrow;

            On.ScavengerBomb.Thrown += Escort_BombThrow;

            On.MoreSlugcats.LillyPuck.Thrown += Escort_LillyThrow;

            On.Weapon.WeaponDeflect += Escort_AntiDeflect;

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
            FAtlas aB, aH;
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
                    /*
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
                    */
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
                    // Speedstar build (Fast, and when running for a certain amount of time, apply BOOST that also does damage on collision)
                    // Unstable build (Longer you're in battlehype, the more the explosion does. Trigger explosion on a dropkick)
                    // Stylist build (Do combos that build up to a super move)
                    // Stealth build (hold still or crouch to enter stealthed mode)
                    case -5:  // Speedstar build
                        e.Speedstar = true;
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
        Miscellaneous!
        */
        private void Eshelp_Tick(Player self){
            if (!eCon.TryGetValue(self, out Escort e) || !CR.TryGet(self, out int limiter)){
                return;
            }

            if (e.consoleTick > limiter){
                e.consoleTick = 0;
            }
            else {
                e.consoleTick++;
            }

            // Dropkick damage cooldown
            if (e.DropKickCD > 0){
                e.DropKickCD--;
            }

            // Get out of centipede grasp cooldown
            // TODO: IMPLEMENT
            if (e.CentiCD > 0){
                e.CentiCD--;
            }

            // Parry leniency when triggering stunslide first (may not actually occur)
            if (e.parrySlideLean > 0){
                e.parrySlideLean--;
            }

            // Parry leniency when not touching the ground
            if (e.parryAirLean > 0 && self.canJump == 0){
                e.parryAirLean--;
            } else if (self.canJump != 0){
                e.parryAirLean = 20;
            }

            // Build-specific ticks
            if (e.Deflector){
                // Increased damage when parry tick
                if (e.DeflAmpTimer > 0){
                    e.DeflAmpTimer--;
                }

                // Sound FX cooldown
                if (e.DeflSFXcd > 0){
                    e.DeflSFXcd--;
                }

                if (self.rollCounter > 1){
                    e.DeflSlideCom++;
                } else {
                    e.DeflSlideCom = 0;
                }
            }

            if (e.Escapist){
                if (e.EscDangerExtend < 10){
                    e.EscDangerExtend++;
                    if (self.dangerGraspTime > 3 && self.dangerGraspTime < 29){
                        self.dangerGraspTime--;
                    }
                } else {
                    if (e.EscUnGraspTime > 4 && e.EscUnGraspTime < e.EscUnGraspLimit){
                        e.EscUnGraspTime++;
                    }
                    e.EscDangerExtend = 0;
                }

                if (e.EscUnGraspTime > 0 && !self.dead && e.EscUnGraspCD == 0){
                    Player.InputPackage iP = RWInput.PlayerInput(self.playerState.playerNumber, self.room.game.rainWorld);
                    if (iP.thrw){
                        e.EscUnGraspTime--;
                    }
                }

                if (e.EscUnGraspCD > 0 && self.stun < 5){
                    e.EscUnGraspCD--;
                }
            }

            if (e.Railgunner){
                if (e.RailWeaping > 0){
                    e.RailWeaping--;
                }
                if (e.RailGaussed > 0){
                    e.RailGaussed--;
                } else {
                    e.RailIReady = false;
                }
                if (e.RailgunCD > 0){
                    e.RailgunCD--;
                } else {
                    e.RailgunUse = 0;
                }
            }


            // Headbutt cooldown
            if (e.CometFrames > 0){
                e.CometFrames--;
            } else {
                e.Cometted = false;
            }

            // Invincibility Frames
            if (e.iFrames > 0){
                Ebug(self, "IFrames: " + e.iFrames, 2);
                e.iFrames--;
            } else {
                e.ElectroParry = false;
                e.savingThrowed = false;
            }

            // Smooth color/brightness transition
            if (requirement <= self.aerobicLevel && e.smoothTrans < 15){
                e.smoothTrans++;
            }
            else if (requirement > self.aerobicLevel && e.smoothTrans > 0){
                e.smoothTrans--;
            }

            // Lizard dropkick leniency
            if (e.LizDunkLean > 0){
                e.LizDunkLean--;
            }

            // Lizard grab timer
            if (e.LizGoForWalk > 0){
                e.LizGoForWalk--;
            } else {
                e.LizGrabCount = 0;
            }

            // Super Wall Flip
            if (self.input[0].x != 0 && self.input[0].y != 0){
                if (e.superWallFlip < 60){
                    e.superWallFlip++;
                }
            } else if (self.input[0].x == 0 || self.input[0].y == 0){
                if (e.superWallFlip > 3){
                    e.superWallFlip -= 3;
                } else if (e.superWallFlip > 0){
                    e.superWallFlip--;
                }
            }
        }

        /*
        Escort code!
        */
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
                    e.Esclass_setSFX_roller(Escort_SFX_Roll);
                    e.Esclass_setSFX_lizgrab(Escort_SFX_Lizard_Grab);
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
                e.Esclass_setIndex_sprite_cue(s.sprites.Length);
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
                    s.sprites[i].color = e.Esclass_runit_thru_RGB(c);
                }
                if (!e.secretRGB){
                    e.hypeColor = c;
                }
                if (e.hypeLight == null || e.hypeSurround == null){
                    e.Esclass_setLight_hype(self.player, e.hypeColor);
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
                                s.sprites[a].color = e.Esclass_runit_thru_RGB(e.hypeColor, (requirement < self.player.aerobicLevel? 8f : Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel)))) * Mathf.Lerp(1f, 1.8f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
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
                                e.Esclass_setLight_hype(self.player, e.hypeColor);
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
            } catch (Exception err){
                Ebug(self.player, err, "Something happened while trying to draw sprites!");
            }
        }


        // Implement Escort's slowed stamina increase
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            if (
                !Exhausion.TryGet(self, out float exhaust) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                orig(self, f);
                return;
            }
            if (self.slugcatStats.name.value == "EscortMe"){
                // Due to the aerobic decrease found in some movements implemented in Escort, the AerobicIncrease actually does the original, and on top of that the additional to balance things out.
                if (!e.Escapist){
                    orig(self, f);
                }

                //Ebug(self, "Aerobic Increase Triggered!");
                if (!self.slugcatStats.malnourished){
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + ((e.Escapist? f * 2 : f) / exhaust));
                } else {
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + (f / (exhaust * 2)));
                }
            } else {
                orig(self, f);
            }
        }

        // Implement visual effect for Battle-Hyped mode
        private void Escort_Update(On.Player.orig_Update orig, Player self, bool eu){
            orig(self, eu);
            try{
                if (!(self != null && self.slugcatStats != null && self.slugcatStats.name != null && self.slugcatStats.name.value != null)){
                    Ebug(self, "Attempted to access a nulled player when updating!", 0);
                    return;
                }
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }

            if (
                !WallJumpVal.TryGet(self, out float[] WJV) ||
                !escapistCD.TryGet(self, out int esCD) ||
                !escapistNoGrab.TryGet(self, out int[] esNoGrab) ||
                !escapistColor.TryGet(self, out float eC) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            // Cooldown/Frames Tick
            Eshelp_Tick(self);
            
            // Just for seeing what a variable does.
            try{
                if(e.consoleTick == 0){
                    Ebug(self, "Clocked.");
                    Ebug(self, "[Roll, Slide, Flip, Throw] Direction: [" + self.rollDirection + ", " + self.slideDirection + ", " + self.flipDirection + ", " + self.ThrowDirection + "]");
                    Ebug(self, "Rotation [x,y]: [" + self.mainBodyChunk.Rotation.x + ", " + self.mainBodyChunk.Rotation.y + "]");
                    Ebug(self, "Lizard Grab Counter: " + e.LizGrabCount);
                    if (e.Brawler){
                        Ebug(self, "Shankmode: " + e.BrawShankMode);
                    }
                    if (e.Deflector){
                        Ebug(self, "Empowered: " + e.DeflAmpTimer);
                    }
                    if (e.Escapist){
                        Ebug(self, "Ungrasp Left: " + e.EscUnGraspTime);
                    }
                    if (e.Railgunner){
                        Ebug(self, "  DoubleRock: " + e.RailDoubleRock);
                        Ebug(self, " DoubleSpear: " + e.RailDoubleSpear);
                        //Ebug(self, "  DoubleBomb: " + e.RailDoubleBomb);
                        //Ebug(self, "DoubleFlower: " + e.RailDoubleFlower);
                    }
                }
                //Ebug(self, self.abstractCreature.creatureTemplate.baseDamageResistance);
                //Ebug(self, "Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                //Ebug(self, "Normalized direction: " + self.bodyChunks[0].vel.normalized);
            } catch (Exception err){
                Ebug(self, err, "Caught error when updating and console logging");
            }

            // vfx
            if(self != null && self.room != null){
                Esconfig_HypeReq(self);

                // Battle-hyped visual effect
                if (config.cfgNoticeHype.Value && Esconfig_Hypable(self) && Esconfig_HypeReq(self) && self.aerobicLevel > requirement){
                    Color hypedColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    hypedColor.a = 0.8f;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, hypedColor));
                }

                // Charged pounces Visual Effect
                if (Esconfig_Pouncing(self)){
                    Color pounceColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);

                    if (self.superLaunchJump > 19){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 9f, 4f, 4f, 11f, pounceColor));
                    }
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && e.superWallFlip >= (int)WJV[4] && self.allowRoll == 15){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, pounceColor));
                    }
                }

                // Empowered damage from parry visual effect
                if (e.Deflector && e.DeflAmpTimer > 0){
                    Color empoweredColor = new Color(0.69f, 0.55f, 0.9f);
                    //Color empoweredColor = new Color(1f, 0.7f, 0.35f, 0.7f);
                    //empoweredColor.a = 0.7f;
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    if (!config.cfgNoticeEmpower.Value){
                        self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer%2==0? 10: -10), 0), new Vector2(2f * (e.DeflAmpTimer%2==0? 1: -1), Mathf.Lerp(-1f, 1f, UnityEngine.Random.value)), empoweredColor, null, 9, 13));
                    } else {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 8, Mathf.Lerp(10f, 20f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer%20)), 2f, 24f, 3f, empoweredColor * 1.5f));
                    }
                }
                // Escapist escape 
                if (e.Escapist && e.EscUnGraspCD == 0 && e.EscUnGraspLimit > 0){
                    Color escapistColor = new Color(0.8f, 0.8f, 0.5f);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 16, 10f, 3f, 20f, Mathf.Lerp(2, 16, 1 - Mathf.InverseLerp(0, e.EscUnGraspLimit, e.EscUnGraspTime)), escapistColor * Mathf.Lerp(0.4f, 1f, 1 - Mathf.InverseLerp(0, e.EscUnGraspLimit, e.EscUnGraspTime)) * eC));
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 12, 25f, 2f, 32f, 2f, escapistColor * eC));
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 8, 10f, 2f, 24f, 2f, escapistColor * eC));
                }

                // Railgunner cooldown timer
                if (e.Railgunner && e.RailgunCD > 0){
                    Color railgunColor = new Color(0.5f, 0.85f, 0.78f);
                    float r = UnityEngine.Random.Range(-1, 1);
                    for (int i = 0; i < (e.RailgunUse >= e.RailgunLimit - 3? 3 : 1); i++){
                        Vector2 v = RWCustom.Custom.RNV() * (r == 0? 0.1f : r);
                        self.room.AddObject(new Spark(self.mainBodyChunk.pos + 15f * v, v, Color.Lerp(railgunColor * 0.5f, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit - 3, e.RailgunUse)), null, (e.RailgunUse >= e.RailgunLimit - 3? 8 : 6), (e.RailgunUse >= e.RailgunLimit - 3? 16 : 10)));
                    }
                    /*
                    self.room.AddObject(new Explosion.FlashingSmoke(self.bodyChunks[0].pos, self.mainBodyChunk.vel + new Vector2(0, 1), 1f, Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)), Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 6)));
                    */
                    /*
                    self.room.AddObject(new Explosion.ExplosionSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * 1.5f * UnityEngine.Random.value, 1f){
                        colorA = Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)),
                        colorB = Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD))
                    });*/
                    //Smoke.FireSmoke s = new Smoke.FireSmoke(self.room);
                    //self.room.AddObject(s);
                    //s.EmitSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 5));
                }

            }

            // Implement drug use
            if (requirement >= 0 && self.aerobicLevel < Mathf.Lerp(0f, requirement + 0.01f, self.Adrenaline)){
                self.aerobicLevel = Mathf.Lerp(0f, requirement + 0.01f, self.Adrenaline);
            }

            // Check if player is grabbing a lizard
            if (Esconfig_Dunkin(self)){
                try{
                    if (e.LizDunkLean == 0){
                        e.LizardDunk = false;
                    }
                    for (int i = 0; i < self.grasps.Length; i++){
                        if (self.grasps[i] != null && self.grasps[i].grabbed is Lizard lizzie && !lizzie.dead){

                            e.LizDunkLean = 20;
                            if (!e.LizardDunk){
                                e.LizGrabCount++;
                            }
                            if (e.LizGoForWalk > 0 && e.LizGrabCount < 4){
                                self.grasps[i].pacifying = true;
                                lizzie.Violence(null, null, lizzie.mainBodyChunk, null, Creature.DamageType.Electric, 0f, 40f);
                            } else {
                                self.grasps[i].pacifying = false;
                            }
                            e.LizardDunk = true;
                            break;
                        }
                    }
                } catch (Exception err){
                    Ebug(self, err, "Something went wrong when checking for lizard grasps");
                }
            }

            // Implement Easy Mode
            if (e.easyMode && (self.wantToJump > 0 && self.input[0].pckp) && self.input[0].x != 0){
                self.animation = Player.AnimationIndex.RocketJump;
                self.wantToPickUp = 0;
                e.easyKick = true;
            }

            // Implement Escapist's getaway
            if (e.Escapist){
                try{
                    if (e.EscDangerGrasp == null){
                        e.EscUnGraspLimit = 0;
                        e.EscUnGraspTime = 0;
                    }
                    else if (e.EscDangerGrasp.discontinued){
                        e.EscDangerGrasp = null;
                    }
                    else if (e.EscUnGraspTime == 0){
                        Ebug(self, "Attempted to take off grabber", 2);
                        e.EscDangerGrasp.grabber.LoseAllGrasps();
                        e.EscUnGraspLimit = 0;
                        self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Duck_Pop, e.SFXChunk, false, 0.9f, 1.3f);
                        self.cantBeGrabbedCounter = esNoGrab[1];
                        e.EscDangerGrasp = null;
                        e.EscUnGraspCD = esCD;
                    }

                    if (e.EscUnGraspCD > 0){
                        self.bodyChunks[0].vel.x *= 1f - Mathf.Lerp(0f, 0.22f, Mathf.InverseLerp(0f, 120f, (float)(e.EscUnGraspCD)));
                        self.bodyChunks[1].vel.x *= 1f - Mathf.Lerp(0f, 0.26f, Mathf.InverseLerp(0f, 120f, (float)(e.EscUnGraspCD)));
                        self.Blink(5);
                    }
                } catch (Exception err){
                    Ebug(self, err, "UPDATE: Escapist's getaway error!");
                }
            }

            // Implement looping SFX
            if (Esconfig_SFX(self)){
                if (self.animation == Player.AnimationIndex.Roll && e.Rollin != null){
                    e.Rollin.Update();
                } else {
                    e.RollinCount = 0f;
                }
                if (e.LizGet != null){
                    e.LizGet.Volume = (e.LizardDunk? 1f : 0f);
                    e.LizGet.Update();
                }
            }
        }

        // Implement Flip jump and less tired from jumping
        private void Escort_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }

            //Ebug(self, "Jump Triggered!");
            // Decreases aerobiclevel gained from jumping
            if (self.aerobicLevel > 0.1f){
                self.aerobicLevel -= 0.1f;
            }

            // Replace chargepounce with a sick flip
            if (
                Esconfig_Pouncing(self) && 
                (
                    self.superLaunchJump >= 19 || 
                    self.simulateHoldJumpButton == 6 || 
                    self.killSuperLaunchJumpCounter > 0
                    ) && 
                self.bodyMode == Player.BodyModeIndex.Crawl
                ){
                Ebug(self, "FLIPERONI GO!", 2);

                if (Esconfig_SFX(self)){
                    self.room.PlaySound(Escort_SFX_Flip, e.SFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
        }

        private void Escort_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            if (self.bodyMode != Player.BodyModeIndex.WallClimb){
                orig(self, direction);
                return;
            }
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, direction);
                    return;
                }
            } catch (Exception err){
                orig(self, direction);
                Ebug(self, err);
                return;
            }
            if (!WallJumpVal.TryGet(self, out var WJV) ||
                !eCon.TryGetValue(self, out Escort e)){
                orig(self, direction);
                return;
            }

            Ebug(self, "Walljump Triggered!");
            bool wallJumper = Esconfig_WallJumps(self);
            bool longWallJump = (self.superLaunchJump > 19 && wallJumper);
            bool superWall = (Esconfig_Pouncing(self) && e.superWallFlip > (int)WJV[4]);
            bool superFlip = self.allowRoll == 15 && Esconfig_Pouncing(self);

            // If charge wall jump is enabled and is able to walljump, or if charge wall jump is disabled
            if ((wallJumper && self.canWallJump != 0) || !wallJumper) {
                orig(self, direction);
                float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline) * (e.savingThrowed? 0.7f : 1f);
                String[] toPrint = new String[3];
                toPrint.SetValue("Walls the Jump", 0);
                if (
                    self.IsTileSolid(1, 0, -1) ||
                    self.IsTileSolid(0, 0, -1) ||
                    self.bodyChunks[1].submersion > 0.1f ||
                    (
                        self.input[0].x != 0 && 
                        self.bodyChunks[0].ContactPoint.x == self.input[0].x &&
                        self.IsTileSolid(0, self.input[0].x, 0) &&
                        !self.IsTileSolid(0, self.input[0].x, 1)
                    )
                ){
                    self.bodyChunks[0].vel.y = 8f * n;
                    self.bodyChunks[1].vel.y = 7f * n;
                    self.bodyChunks[0].pos.y += 10f * Mathf.Min(1f, n);
                    self.bodyChunks[1].pos.y += 10f * Mathf.Min(1f, n);
                    toPrint.SetValue("Water", 1);
                    self.room.PlaySound(SoundID.Slugcat_Normal_Jump, e.SFXChunk, false, 1f, 0.7f);
                } 
                else {
                    self.bodyChunks[0].vel.y = ((longWallJump || (superFlip && superWall))? WJV[0] : 8f) * n;
                    self.bodyChunks[1].vel.y = ((longWallJump || (superFlip && superWall))? WJV[1] : 7f) * n;
                    self.bodyChunks[0].vel.x = ((superFlip && superWall)? WJV[2] : 7f) * n * (float)direction;
                    self.bodyChunks[1].vel.x = ((superFlip && superWall)? WJV[3] : 6f) * n * (float)direction;
                    self.standing = true;
                    self.jumpStun = 8 * direction;
                    if (superWall){
                        self.room.PlaySound((self.superLaunchJump > 19? SoundID.Slugcat_Super_Jump : SoundID.Slugcat_Wall_Jump), e.SFXChunk, false, 1f, 0.7f);
                    }
                    toPrint.SetValue("Not Water", 1);
                    Ebug(self, "Y Velocity" + self.bodyChunks[0].vel.y, 2);
                    Ebug(self, "Y Velocity" + self.bodyChunks[1].vel.y, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[0].vel.x, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[1].vel.x, 2);
                }
                self.jumpBoost = 0f;

                if (superFlip && superWall){
                    self.animation = Player.AnimationIndex.Flip;
                    self.room.PlaySound((Esconfig_SFX(self)? Escort_SFX_Flip : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.SFXChunk, false, (Esconfig_SFX(self)? 1f : 1.4f), 0.9f);
                    self.jumpBoost += Mathf.Lerp(WJV[6], WJV[7], Mathf.InverseLerp(WJV[4], WJV[5], e.superWallFlip));
                    toPrint.SetValue("SUPERFLIP", 2);
                } else {
                    toPrint.SetValue("not so flip", 2);
                }
                Ebug(self, "Jumpboost" + self.jumpBoost, 2);
                Ebug(self, "SWallFlip" + e.superWallFlip, 2);
                Ebug(self, "SLaunchJump" + self.superLaunchJump, 2);
                if (self.superLaunchJump > 19){
                    self.superLaunchJump = 0;
                }
                self.canWallJump = 0;
                Ebug(self, toPrint, 2);
            }
        }

        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!Esconfig_WallJumps(self)){
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0){
                String msg = "Nothing New";
                self.canWallJump = 0;
                if (self.input[0].jmp){
                    msg = "Is touching the jump button";
                    if (self.superLaunchJump < 20){
                        self.superLaunchJump += 2;
                        if (self.Adrenaline == 1f && self.superLaunchJump < 6){
                            self.superLaunchJump = 6;
                        }
                    } else {
                        self.killSuperLaunchJumpCounter = 15;
                    }
                }

                if (!self.input[0].jmp && self.input[1].jmp){
                    msg = "Lets go of the jump button";
                    self.wantToJump = 1;
                }
                if(e.consoleTick == 0){
                    Ebug(self, msg, 2);
                }
            }
        }

        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
            } catch (Exception err){
                orig(self);
                Ebug(self, err);
                return;
            }
            if(!Esconfig_WallJumps(self)){
                orig(self);
                return;
            }

            //Ebug(self, "CheckInput Triggered!");
            int previously = self.input[0].x;
            orig(self);

            // Undoes the input cancellation
            if(self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1){
                if (self.input[0].x == 0){
                    self.input[0].x = previously;
                }
            }
        }

        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
                if (!Esconfig_Heavylift(self)){
                    return orig(self, obj);
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    return orig(self, obj);
                }

                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                    if (e.consoleTick == 0){
                        Ebug(self, "Revivify skip!", 1);
                        Ebug(self, "Creature: " + creature.GetType(), 1);
                        Ebug(self, "Player: " + self.GetOwnerType(), 1);
                    }
                    return orig(self, creature);
                }

                //Ebug(self, "Heavycarry Triggered!");
                if (obj.TotalMass <= self.TotalMass * (e.Brawler? ratioed*2 : ratioed)){
                    if (ModManager.CoopAvailable && obj is Player player && player != null && !e.Brawler){
                        return !player.isSlugpup;
                    }
                    if (obj is Creature c && c is not Lizard && !c.dead){
                        return orig(self, obj);
                    }
                    return false;
                }
                return orig(self, obj);
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, obj);
            }
        }

        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (
                !BetterCrawl.TryGet(self, out var crawlSpeed) ||
                !BetterPoleWalk.TryGet(self, out var poleMove) ||
                !Escomet.TryGet(self, out int SetComet) ||
                !NoMoreGutterWater.TryGet(self, out float[] theGut) ||
                !eCon.TryGetValue(self, out Escort e)
            ){
                return;
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Implement bettercrawl
            if (!e.Deflector && self.bodyMode == Player.BodyModeIndex.Crawl){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) : crawlSpeed[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(crawlSpeed[2], crawlSpeed[3], self.aerobicLevel) : crawlSpeed[5]) * self.slugcatStats.runspeedFac;
            }

            // Implement betterpolewalk
            /*
            The hangfrombeam's speed does not get affected by dynamicrunspeed apparently so that's fun... 
            still the standonbeam works but also at the same time not as I initially thought.
            The slugcat apparently has a limit on how fast they can move on the beam while standing on it, leaning more and more foreward and getting more and more friction as a result...
            or to that degree.
            */
            else if (!e.Deflector && self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam && (self.animation == Player.AnimationIndex.StandOnBeam || self.animation == Player.AnimationIndex.HangFromBeam)){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(poleMove[0], poleMove[1], self.aerobicLevel): poleMove[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(poleMove[2], poleMove[3], self.aerobicLevel): poleMove[5]) * self.slugcatStats.runspeedFac;
            } 
            
            // Set headbutt condition
            else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb){
                if (self.slowMovementStun > 0){
                    e.CometFrames = SetComet;
                }
            }

            // Implement GuuhWuuh
            if(self.bodyMode == Player.BodyModeIndex.Swimming){
                float viscoDance = self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity);

                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[0], (float)Math.Pow(viscoDance, theGut[6])), 
                        Mathf.Lerp(1f, (self.mainBodyChunk.vel.y > 0? theGut[1] : theGut[2]), (float)Math.Pow(viscoDance, theGut[7])));
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[3], (float)Math.Pow(viscoDance, theGut[8])), 
                        Mathf.Lerp(1f, (self.mainBodyChunk.vel.y > 0? theGut[4] : theGut[5]), (float)Math.Pow(viscoDance, theGut[9])));
                }
            }
        }

        // Implement Movementtech
        private void Escort_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }

            //Ebug(self, "UpdateAnimation Triggered!");
            // Infiniroll
            if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                e.RollinCount++;
                if(e.consoleTick == 0){
                    Ebug(self, "Rollin at: " + e.RollinCount, 2);
                }
                if(Esconfig_SFX(self) && e.Rollin != null){
                    e.Rollin.Volume = Mathf.InverseLerp(80f, 240f, e.RollinCount);
                }
                self.rollCounter = 0;
                self.mainBodyChunk.vel.x *= Mathf.Lerp(1, 1.25f, Mathf.InverseLerp(0, 120f, e.RollinCount));
            }

            if (self.animation != Player.AnimationIndex.Roll){
                e.RollinCount = 0f;
            }

            // Implement dropkick animation
            if(self.animation == Player.AnimationIndex.RocketJump && config.cfgDKAnimation.Value){
                Vector2 n = self.bodyChunks[0].vel.normalized;
                self.bodyChunks[0].vel -= n * 2;
                self.bodyChunks[1].vel += n * 2;
                self.bodyChunks[0].vel.y += 0.05f;
                self.bodyChunks[1].vel.y += 0.1f;
            }
            if(e.easyMode && e.easyKick){
                if (self.animation == Player.AnimationIndex.RocketJump && self.input[0].x == 0 && self.input[1].x == 0 && self.input[2].x == 0){
                    self.animation = Player.AnimationIndex.None;
                }
                if (self.animation != Player.AnimationIndex.RocketJump){
                    e.easyKick = false;
                }
            } 

            if (e.Deflector){
                if (self.animation == Player.AnimationIndex.BellySlide){
                    e.DeflSlideKick = true;
                    if (self.rollCounter < 8){
                        self.rollCounter += 9;
                    }
                    if (self.initSlideCounter < 3){
                        self.initSlideCounter += 3;
                    }
                    if (e.DeflSlideCom < (self.longBellySlide?51:20) && self.rollCounter > 12 && self.rollCounter < 15){
                        self.rollCounter--;
                        //self.exitBellySlideCounter--;
                    }
                    self.mainBodyChunk.vel.x *= Mathf.Lerp(1.1f, 1.3f, Mathf.InverseLerp(0, 10, e.DeflSlideCom));
                }
                else if (e.DeflSlideKick && self.animation == Player.AnimationIndex.RocketJump){
                    self.mainBodyChunk.vel.x *= 1.4f;
                    self.mainBodyChunk.vel.y *= 0.95f;
                    e.DeflSlideKick = false;
                } 
                else {
                    e.DeflSlideKick = false;
                }
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


        // Implement Parryslide/midair projectile grab
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){
            try{
                if (self is Player && (self as Player).slugcatStats.name.value != "EscortMe"){
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    return;
                }
            } catch (Exception err){
                Ebug((self as Player), err);
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(self is not Player player){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(
                !ParrySlide.TryGet(player, out bool enableParry) ||
                !eCon.TryGetValue(player, out Escort e)
            ){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (!enableParry){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }



            Ebug(player, "Violence Triggered!");
            // connects to the Escort's Parryslide option
            e.ParrySuccess = false;
            if (e.Railgunner && e.RailIReady && type != null && type == Creature.DamageType.Explosion){
                if (e.iFrames == 0){
                    e.ParrySuccess = true;
                }
                stunBonus = 0;
            }
            if (Eshelp_ParryCondition(player)){
                // Parryslide (parry module)
                Ebug(player, "Escort attempted a Parryslide", 2);
                int direction;
                direction = player.slideDirection;

                Ebug(player, "Is there a source? " + (source != null), 2);
                Ebug(player, "Is there a direction & Momentum? " + (directionAndMomentum != null), 2);
                Ebug(player, "Is there a hitChunk? " + (hitChunk != null), 2);
                Ebug(player, "Is there a hitAppendage? " + (hitAppendage != null), 2);
                Ebug(player, "Is there a type? " + (type != null), 2);
                Ebug(player, "Is there damage? " + (damage > 0f), 2);
                Ebug(player, "Is there stunBonus? " + (stunBonus > 0f), 2);

                if (source != null) {
                    Ebug(player, "Escort is being assaulted by: " + source.owner.GetType(), 2);
                }
                Ebug(player, "Escort parry is being checked", 1);
                if (type != null){
                    Ebug(player, "Escort gets hurt by: " + type.value, 2);
                if (type == Creature.DamageType.Bite){
                    Ebug(player, "Escort is getting BIT?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 35;
                        //(self as Player).WallJump(direction);
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort got out of a creature's mouth!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug(player, "Weapons can BITE?!", 2);
                    } 
                    else {
                        Ebug(player, "Where is Escort getting bit from?!", 2);
                    }
                } 
                else if (type == Creature.DamageType.Stab) {
                    Ebug(player, "Escort is getting STABBED?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a stabby creature?", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon) {
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a stabby weapon", 2);
                    } 
                    else {
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        bool keepLooping = true;
                        for (int a = 0; a < self.room.physicalObjects.Length; a++){
                            for (int b = 0; b < self.room.physicalObjects[a].Count; b++){
                                if (self.room.physicalObjects[a][b] is Vulture vulture && vulture.IsKing){
                                    if (vulture.kingTusks.tusks[0].impaleChunk != null && vulture.kingTusks.tusks[0].impaleChunk.owner == self){
                                        vulture.kingTusks.tusks[0].impaleChunk = null;
                                        keepLooping = false;
                                        break;
                                    } else if (vulture.kingTusks.tusks[1].impaleChunk != null && vulture.kingTusks.tusks[1].impaleChunk.owner == self){
                                        vulture.kingTusks.tusks[1].impaleChunk = null;
                                        keepLooping = false;
                                        break;
                                    }
                                }
                            }
                            if (!keepLooping){
                                Ebug(player, "Tusk unimpaled!", 2);
                                Debug.LogError("-> Escort: Please pay no attention to this! This is how Escort parry works (on King Tusks)!");
                                break;
                            }
                        }
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a generic stabby thing", 2);
                    }
                } 
                else if (type == Creature.DamageType.Blunt) {
                    Ebug(player, "Escort is getting ROCC'ED?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "Creatures aren't rocks...", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon){
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(weapon.firstChunk.lastPos, -vector, weapon.firstChunk.vel.magnitude);
                        damage = 0f;
                        stunBonus = stunBonus / 5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort bounces a blunt thing.", 2);
                    } 
                    else {
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried something blunt.", 2);
                    }
                } 
                else if (type == Creature.DamageType.Water) {
                    Ebug(player, "Escort is getting Wo'oh'ed?!", 1);
                } 
                else if (type == Creature.DamageType.Explosion) {
                    Ebug(player, "Escort is getting BLOWN UP?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "Wait... creatures explode?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parries an explosion from weapon?!", 2);
                    } 
                    else {
                        player.WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parries an explosion", 2);
                    }
                } 
                else if (type == Creature.DamageType.Electric) {
                    Ebug(player, "Escort is getting DEEP FRIED?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        //type = Creature.DamageType.Blunt;
                        damage = 0f;
                        //(self as Player).LoseAllGrasps();
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        // NOTE TO SELF: Centipede parry goes here
                        Ebug(player, "Escort somehow parried a shock from creature?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        //type = Creature.DamageType.Blunt;
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug(player, "Escort somehow parried a shock object?!", 2);
                    } 
                    else {
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug(player, "Escort attempted to parry a shock but why?!", 2);
                    }
                } 
                else {
                    Ebug(player, "Escort is getting UNKNOWNED!!! RUNNN", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "IT'S ALSO AN UNKNOWN CREATURE!!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug(player, "IT'S ALSO AN UNKNOWN WEAPON!!", 2);
                    } 
                    else {
                        Ebug(player, "WHO THE HECK KNOWS WHAT IT IS?!", 2);
                    }
                }
                }
            }
            else if (Eshelp_SavingThrow(player, source, type)){
                e.ParrySuccess = true;
                (source.owner as Creature).LoseAllGrasps();
                type = Creature.DamageType.Blunt;
                damage = 0f;
                stunBonus = 0f;
            }
            // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
            // Visual indicator doesn't work ;-;
            if (e.ParrySuccess){
                if (e.Deflector){
                    stunBonus = 0;
                    player.Jump();
                    player.animation = Player.AnimationIndex.Flip;
                    player.mainBodyChunk.vel.y *= 1.5f;
                    player.mainBodyChunk.vel.x *= 0.15f;
                    if (e.DeflSFXcd == 0){
                        self.room.PlaySound(SoundID.Snail_Warning_Click, self.mainBodyChunk, false, 1.6f, 0.65f);
                        e.DeflSFXcd = 9;
                    }
                    e.DeflAmpTimer = 160;
                }
                else {
                    self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                }
                if (self != null && self.room != null && self.mainBodyChunk != null){
                    for (int c = 0; c < 7; c++){
                        self.room.AddObject(new WaterDrip(self.bodyChunks[1].pos + new Vector2(self.mainBodyChunk.rad * self.bodyChunks[1].vel.x, 0f), RWCustom.Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.bodyChunks[1].vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                        //self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.grey, null, 3, 6));
                        //self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos, self.mainBodyChunk.vel * Mathf.Lerp(1f, 4f, UnityEngine.Random.value) * player.flipDirection + RWCustom.Custom.RNV() * UnityEngine.Random.value * 9f, false));
                    }
                }
                Ebug(player, "Parry successful!", 1);
                e.iFrames = 6;
                e.parrySlideLean = 0;
                if (e.Railgunner && e.RailIReady){
                    self.stun = 0;
                }
            }
            else if (e.iFrames > 0) {
                if (e.Railgunner && e.RailIReady){
                    self.stun = 0;
                    if (e.iFrames <= 0){
                        e.RailIReady = false;
                    }
                }
                if (e.ElectroParry){
                    damage = 0f;
                    stunBonus = stunBonus * 0.5f;
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    Ebug(player, "Stun Resistance frame tick", 2);
                } else {
                    if (e.Railgunner && e.RailIReady){
                        e.RailIReady = false;
                    }
                    Ebug(player, "Immunity frame tick", 2);
                }
            } 
            else {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                Ebug(player, "Nothing or not possible to parry!", 1);
            }
            Ebug(player, "Parry Check end", 1);
            return;

        }


        // Parryslide spears?!
        private bool Escort_StickySpear(On.Player.orig_SpearStick orig, Player self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, source, dmg, chunk, appPos, direction);
                }
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!ParrySlide.TryGet(self, out bool parrier)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!parrier || (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }

            Ebug(self, "Sticky Triggered!");
            return !(self.animation == Player.AnimationIndex.BellySlide);
        }


        // Implement Bodyslam
        private void Escort_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (
                !BodySlam.TryGet(self, out float[] bodySlam) ||
                !TrampOhLean.TryGet(self, out float bounce) ||
                !SlideLaunchMod.TryGet(self, out float[] slideMod) ||
                !brawlerSlideLaunchFac.TryGet(self, out float bSlideFac) ||
                !brawlerDKHypeDmg.TryGet(self, out float bDKHDmg) ||
                !deflectorSlideLaunchMod.TryGet(self, out float dSlideMod) ||
                !deflectorSlideDmg.TryGet(self, out float dSlideDmg) ||
                !deflectorSlideLaunchFac.TryGet(self, out float dSlideFac) ||
                !deflectorDKHypeDmg.TryGet(self, out float[] dDKHDmg) ||
                !escapistSlideLaunchMod.TryGet(self, out float eSlideMod) ||
                !escapistSlideLaunchFac.TryGet(self, out float eSlideFac) ||
                !escapistNoGrab.TryGet(self, out int[] esNoGrab) ||
                !Esconfig_HypeReq(self) ||
                !Esconfig_DKMulti(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            //Ebug(self, "Collision Triggered!");
            try{
                if (e.consoleTick == 0){
                    Ebug(self, "Escort collides!");
                    Ebug(self, "Has physical object? " + otherObject != null);
                    if (otherObject != null){
                        Ebug(self, "What is it? " + otherObject.GetType());
                    }
                }
            } catch (Exception err){
                Ebug(self, err, "Error when printing collision stuff");
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Reimplementing the elevator... the way it was in its glory days
            if (Esconfig_Elevator(self) && otherObject is Creature && self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                self.jumpBoost += 4;
            }


            if (otherObject is Creature creature && 
                creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !(ModManager.CoopAvailable && otherObject is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                
                if (e.Escapist && self.aerobicLevel > 0.02f){
                    self.aerobicLevel -= 0.01f;
                }


                // Creature Trampoline (or if enabled Escort's Elevator)
                /*
                Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
                */
                if ((self.animation == Player.AnimationIndex.Flip || (e.Escapist && self.animation == Player.AnimationIndex.None)) && self.bodyMode == Player.BodyModeIndex.Default && (!creature.dead || creature is Lizard)){
                    self.slideCounter = 0;
                    if (self.jumpBoost <= 0) {
                        self.jumpBoost = (self.animation == Player.AnimationIndex.None? bounce * 1.5f: bounce);
                    }
                    if (e.Escapist && self.cantBeGrabbedCounter < esNoGrab[1] && self.animation == Player.AnimationIndex.Flip){
                        self.cantBeGrabbedCounter += esNoGrab[0];
                    }
                    if (e.Deflector){
                        try{
                            if (self.mainBodyChunk.vel.y < 6f){
                                self.mainBodyChunk.vel.y += 10f;
                            }
                            self.Violence(null, null, self.mainBodyChunk, null, Creature.DamageType.Blunt, 0f, 0f);
                        } catch (Exception err){
                            Ebug(self, err, "Hitting thyself failed!");
                        }
                    }
                }

                int direction;

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    try{

                    creature.SetKillTag(self.abstractCreature);

                    if (e.parrySlideLean <= 0){
                        e.parrySlideLean = 4;
                    }
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,e.SFXChunk);

                    float normSlideStun = (hypedMode || e.Brawler? bodySlam[1] * 1.5f : bodySlam[1]);
                    if (hypedMode && self.aerobicLevel > requirement){
                        normSlideStun = bodySlam[1] * (e.Brawler? 2f : 1.75f);
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        creature.firstChunk, null, (e.DeflAmpTimer > 0? Creature.DamageType.Stab : Creature.DamageType.Blunt),
                        (e.DeflAmpTimer > 0? dSlideDmg : bodySlam[0]), normSlideStun
                    );
                    /*
                    if (self.pickUpCandidate is Spear){  // Attempts to pickup spears (may pickup things higher in priority that are nearby)
                        self.PickupPressed();
                    }*/

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 5f, 5.5f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    if (self.longBellySlide) {
                        direction = self.rollDirection;
                        self.animation = Player.AnimationIndex.Flip;
                        self.WallJump(direction);
                        if (Esconfig_Spears(self)){
                            float tossModifier = slideMod[0];
                            if (e.Deflector){
                                tossModifier = dSlideMod;
                            }
                            else if (e.Escapist){
                                tossModifier = eSlideMod;
                            }
                            self.animation = Player.AnimationIndex.BellySlide;
                            self.bodyChunks[1].vel = new Vector2((float)self.slideDirection * tossModifier, slideMod[1]);
                            self.bodyChunks[0].vel = new Vector2((float)self.slideDirection * tossModifier, slideMod[2]);
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                        }
                        Ebug(self, "Greatdadstance stunslide!", 2);
                    } else {
                        direction = self.flipDirection;
                        if (e.Brawler){
                            self.mainBodyChunk.vel.x *= bSlideFac;
                        }
                        if (e.Deflector){
                            self.mainBodyChunk.vel *= dSlideFac;
                        }
                        if (e.Escapist){
                            self.mainBodyChunk.vel.y *= eSlideFac;
                        }
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.Flip;
                        Ebug(self, "Stunslided!", 2);
                    }
                    } catch (Exception err){
                        Ebug(self, err, "Error while Slidestunning!");
                    }
                }

                // Dropkick
                else if (self.animation == Player.AnimationIndex.RocketJump){
                    try{

                    creature.SetKillTag(self.abstractCreature);

                    String message = (e.easyKick? "Easykicked!" : "Dropkicked!");
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk, false, 1f, 1.15f);
                    
                    if (!creature.dead) {
                        DKMultiplier *= creature.TotalMass;
                    }
                    float normSlamDamage = (e.easyKick? 0.001f : 0.05f);
                    if (e.DropKickCD == 0){
                        normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + (e.Brawler? 0.27f : 0.15f));
                        creature.LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * (e.Brawler? bDKHDmg : 1.6f);}
                        if (e.Deflector){
                            if (e.DeflAmpTimer > 0){
                                normSlamDamage *= dDKHDmg[0];
                            }
                            else {
                                normSlamDamage *= dDKHDmg[1];
                            }
                        }
                        message = "Powerdropkicked!";
                    } else {
                        //multiplier *= 0.5f;
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*DKMultiplier, self.mainBodyChunk.vel.y*DKMultiplier*(e.LizardDunk?0.2f:1f))),
                        creature.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, (e.DeflAmpTimer > 0? bodySlam[1] : bodySlam[3])
                    );
                    Ebug(self, "Dunk the lizard: " + e.LizardDunk, 2);
                    if (e.DropKickCD == 0){
                        e.LizardDunk = false;
                    }
                    if (e.DeflAmpTimer > 0){
                        e.DeflAmpTimer = 0;
                    }
                    e.DropKickCD = (e.easyKick? 40 : 15);
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    /*
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }*/
                    direction = -self.flipDirection;
                    self.WallJump(direction);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    Ebug(self, message + " Dmg: " + normSlamDamage, 2);
                    } catch (Exception err){
                        Ebug(self, err, "Error when dropkicking!");
                    }
                    e.savingThrowed = false;
                }

                else if (e.CometFrames > 0 && !e.Cometted){
                    try{
                    creature.SetKillTag(self.abstractCreature);
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x*(DKMultiplier*0.5f)*creature.TotalMass, self.bodyChunks[0].vel.y*(DKMultiplier*0.5f)*creature.TotalMass)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        0f, 15f
                    );
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x*(DKMultiplier*0.5f)*creature.TotalMass;
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y*(DKMultiplier*0.5f)*creature.TotalMass;
                    if (self != null && self.room != null){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    }
                    Ebug(self, "Headbutted!", 2);
                    if (self.room != null){
                        if (Esconfig_SFX(self)){
                            self.room.PlaySound(Escort_SFX_Boop, e.SFXChunk);
                        }
                        self.room.PlaySound(SoundID.Slugcat_Floor_Impact_Standard, e.SFXChunk, false, 0.75f, 1.3f);
                    }
                    e.Cometted = true;
                    } catch (Exception err){
                        Ebug(self, err, "Error when headbutting!");
                    }
                }
            }
        }


        // Implement a different type of dropkick
        private void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu){
            orig(self, grasp, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!BodySlam.TryGet(self, out var bodySlam) ||
                    !eCon.TryGetValue(self, out Escort e)){
                    return;
                }

                Ebug(self, "Toss Object Triggered!");
                if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead){
                    if (Esconfig_SFX(self) && e.LizGet != null){
                        e.LizGet.Volume = 0f;
                    }
                    if (self.bodyMode == Player.BodyModeIndex.Default){
                        self.animation = Player.AnimationIndex.RocketJump;
                        self.bodyChunks[1].vel.x += self.ThrowDirection;
                    }
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
        }

        // Implement unique spearskill
        private void Escort_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear){
            orig(self, spear);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (
                !bonusSpear.TryGet(self, out float[] spearDmgBonuses) ||
                !brawlerSpearVelFac.TryGet(self, out float[] bSpearVel) ||
                !brawlerSpearDmgFac.TryGet(self, out float[] bSpearDmg) ||
                !brawlerSpearThrust.TryGet(self, out float bSpearThr) ||
                !brawlerSpearShankY.TryGet(self, out float[] bSpearY) ||
                !deflectorSpearVelFac.TryGet(self, out float[] dSpearVel) ||
                !deflectorSpearDmgFac.TryGet(self, out float[] dSpearDmg) ||
                !escapistSpearVelFac.TryGet(self, out float eSpearVel) ||
                !railgunSpearVelFac.TryGet(self, out float[] rSpearVel) ||
                !railgunSpearDmgFac.TryGet(self, out float rSpearDmg) ||
                !railgunSpearThrust.TryGet(self, out float[] rSpearThr) ||
                !Esconfig_HypeReq(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            Ebug(self, "ThrownSpear Triggered!");
            float thrust = 7f;
            bool onPole = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
            bool doNotYeet = onPole || !Esconfig_Spears(self) || e.RailDoubleSpear;
            try{
            if (Esconfig_Hypable(self)){
                if (self.aerobicLevel > requirement){
                    spear.throwModeFrames = -1;
                    spear.spearDamageBonus *= spearDmgBonuses[0];
                    if (self.canJump != 0 && !self.longBellySlide){
                        if (!doNotYeet){
                            self.rollCounter = 0;
                            if (self.input[0].jmp && self.input[0].thrw){
                                self.animation = Player.AnimationIndex.BellySlide;
                                self.whiplashJump = true;
                                spear.firstChunk.vel.x *= 1.7f;
                                Ebug(self, "Spear Go!?", 2);
                            } else {
                                self.animation = Player.AnimationIndex.Roll;
                                self.standing = false;
                            }
                        }
                        thrust = 12f;
                    } else {
                        self.longBellySlide = true;
                        if (!doNotYeet){
                            self.exitBellySlideCounter = 0;
                            self.rollCounter = 0;
                            self.flipFromSlide = true;
                            self.animation = Player.AnimationIndex.BellySlide;
                        }
                        thrust = 9f;
                    }
                } else {
                    if (!doNotYeet){
                        self.rollCounter = 0;
                        if (self.canJump != 0){
                            self.whiplashJump = true;
                            if (self.animation != Player.AnimationIndex.BellySlide){
                                self.animation = Player.AnimationIndex.BellySlide;
                            }
                            if (self.input[0].jmp && self.input[0].thrw){
                                spear.firstChunk.vel.x *= 1.6f;
                                Ebug(self, "Spear Go!", 2);
                            }
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                            self.standing = false;
                        }
                    }
                    spear.spearDamageBonus *= spearDmgBonuses[1];
                    thrust = 5f;
                }
            } else {
                spear.spearDamageBonus *= 1.25f;
            }
            } catch (Exception err){
                Ebug(self, err, "Error while setting additional spear effects!");
            }
            if (e.Brawler){
                try{
                    spear.spearDamageBonus *= bSpearDmg[0];
                    if (self.bodyMode == Player.BodyModeIndex.Crawl){
                        spear.firstChunk.vel.x *= bSpearVel[0];
                    }
                    else if (self.bodyMode == Player.BodyModeIndex.Stand){
                        spear.firstChunk.vel.x *= bSpearVel[1];
                    } else {
                        spear.firstChunk.vel.x *= bSpearVel[2];
                    }
                    thrust *= 0.5f;
                    if (e.BrawShankMode){
                        //spear.throwDir = new RWCustom.IntVector2(0, -1);
                        spear.firstChunk.vel = e.BrawShankDir;
                        //spear.firstChunk.vel.y = -(Math.Abs(spear.firstChunk.vel.y)) * bSpearY[0];
                        //spear.firstChunk.pos += new Vector2(0f, bSpearY[1]);
                        spear.firstChunk.vel *= bSpearY[0];
                        //spear.doNotTumbleAtLowSpeed = true;
                        e.BrawShankMode = false;
                        spear.spearDamageBonus = bSpearDmg[1];
                        if (self.room != null){
                            self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, e.SFXChunk, false, 1f, 2f);
                        }
                    }
                } catch (Exception err){
                    Ebug(self, err, "Error while applying Brawler-specific speartoss");
                }
            }
            if (e.Deflector){
                try{
                    if (e.DeflAmpTimer > 0){
                        spear.spearDamageBonus *= dSpearDmg[0];
                        spear.firstChunk.vel *= dSpearVel[0];
                        e.DeflAmpTimer = 0;
                    }
                    else {
                        spear.spearDamageBonus = dSpearDmg[1];
                        spear.firstChunk.vel *= dSpearVel[1];
                    }
                } catch (Exception err){
                    Ebug(self, err, "Error while applying Deflector-specific speartoss");
                }
            }
            if (e.Escapist){
                spear.firstChunk.vel *= eSpearVel;
            }
            if (e.Railgunner){
                try{
                    thrust = 5f;
                    if (e.RailDoubleSpear){
                        if (!e.RailFirstWeaped){
                            spear.firstChunk.vel *= rSpearVel[0];
                            e.RailFirstWeaper = spear.firstChunk.vel;
                            e.RailFirstWeaped = true;
                        }
                        else {
                            spear.firstChunk.vel = e.RailFirstWeaper;
                            e.RailFirstWeaped = false;
                            //e.BarbDoubleSpear = false;
                            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                                thrust *= rSpearThr[0];
                            } else if (self.bodyMode == Player.BodyModeIndex.Stand) {
                                thrust *= rSpearThr[1];
                            } else {
                                thrust *= rSpearThr[2];
                            }
                        }
                        spear.spearDamageBonus *= rSpearDmg;
                        if (!onPole){
                            self.standing = false;
                        }
                    } else {
                        thrust *= rSpearThr[3];
                        spear.firstChunk.vel *= rSpearVel[1];
                    }
                } catch (Exception err){
                    Ebug(self, err, "Error while applying Railgunner-specific spearthrow");
                }
            }
            if (onPole && !e.Railgunner) {
                thrust = 1f;
            }

            try{
                BodyChunk firstChunker = self.firstChunk;
                if ((self.room != null && self.room.gravity == 0f) || (Mathf.Abs(spear.firstChunk.vel.x) < 1f && Mathf.Abs(spear.firstChunk.vel.y) < 1f)){
                    self.firstChunk.vel += spear.firstChunk.vel.normalized * Math.Abs(thrust);
                } else {
                    if (Esconfig_Spears(self)){
                        self.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                    }
                    if (self.animation != Player.AnimationIndex.BellySlide){
                        if (e.Railgunner && spear.throwDir.x == 0){
                            if (spear.throwDir.y == 1){
                                self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.4f;
                            } else if (spear.throwDir.y == -1){
                                self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.65f;
                            } else {
                                self.firstChunk.vel += spear.firstChunk.vel.normalized * thrust;
                            }
                        } else {
                            self.firstChunk.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                        }
                    }
                }
            } catch (Exception err){
                Ebug(self, err, "Error while adjusting the player thrust");
            }            
            Ebug(self, "Speartoss! Velocity [X,Y]: [" + spear.firstChunk.vel.x + "," + spear.firstChunk.vel.y + "] Damage: " + spear.spearDamageBonus, 2);
        }

        private void Escort_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, grasp, eu);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    orig(self, grasp, eu);
                    return;
                }
                if (e.Brawler && (self.grasps[grasp] != null && self.grasps[1 - grasp] != null)){
                    for (int j = 0; j < 2; j++){
                        if (self.grasps[j].grabbed is Spear s &&
                        self.grasps[1 - j].grabbed is Creature cs){
                            if (cs.dead || cs.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || cs.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && cs is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                                break;
                            }
                            Creature c = cs;
                            c.firstChunk.vel.y += 1f;
                            orig(self, 1 - j, eu);
                            
                            //s.alwaysStickInWalls = false;
                            //if (c.mainBodyChunk != null){
                            //    s.meleeHitChunk = c.mainBodyChunk;
                            //}
                            
                            //s.firstChunk.pos = self.mainBodyChunk.pos + new Vector2(0f, 80f);
                            s.firstChunk.vel = new Vector2(c.mainBodyChunk.pos.x - s.firstChunk.pos.x, c.mainBodyChunk.pos.y - s.firstChunk.pos.y).normalized;
                            //Vector2 v = (c.firstChunk.pos - s.firstChunk.pos).normalized * 3f;
                            e.BrawShankDir = s.firstChunk.vel;
                            Ebug(self, "Throw Spear at Creature!");
                            e.BrawShankMode = true;
                            orig(self, j, eu);
                            return;
                        }
                    }
                }

                if (e.Railgunner && (e.RailDoubleSpear || e.RailDoubleRock || e.RailDoubleBomb || e.RailDoubleLilly)){
                    self.standing = false;
                    if (self.Malnourished){
                        self.Stun(20 * e.RailgunUse);
                        e.RailgunUse++;
                    }
                    Vector2 p = new Vector2();
                    Vector2 v = new Vector2();
                    if (self.grasps[grasp] != null && self.grasps[grasp].grabbed is Weapon){
                        p = self.grasps[grasp].grabbed.firstChunk.pos;
                        v = self.grasps[grasp].grabbed.firstChunk.vel;
                    }
                    Weapon w = self.grasps[grasp].grabbed as Weapon;
                    orig(self, grasp, eu);
                    self.grasps[1 - grasp].grabbed.firstChunk.pos = p;
                    //self.grasps[1].grabbed.firstChunk.vel = v;
                    orig(self, 1 - grasp, eu);

                    if (self.room != null){
                        Color c = new Color(0.5f, 0.85f, 0.78f);
                        Smoke.FireSmoke s = new Smoke.FireSmoke(self.room);
                        self.room.AddObject(s);
                        for (int i = 0; i < 6; i++)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[1].pos + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(2f, 7f, UnityEngine.Random.value) * 6, c, null, 10, 170));
                            s.EmitSmoke(self.bodyChunks[1].pos + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, self.mainBodyChunk.vel + v * UnityEngine.Random.value * -10f, c, 12);
                        }
                        self.room.AddObject(new Explosion.ExplosionLight(p, 90f, 0.7f, 4, c));
                        if (e.Esclass_Railgunner_Death(self, self.room)){
                            if (Esconfig_SFX(self)){
                                self.room.PlaySound(Escort_SFX_Railgunner_Death, e.SFXChunk);
                            }
                            return;
                        }
                        self.room.PlaySound(e.RailgunUse >= e.RailgunLimit - 3? SoundID.Cyan_Lizard_Powerful_Jump : SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, 0.8f, Mathf.Lerp(1.15f, 2f, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)));

                        // (v * UnityEngine.Random.value * -0.5f + RWCustom.Custom.RNV() * Math.Abs(v.x * w.throwDir.x + v.y * w.throwDir.y)) * -1f
                        // self.room.ScreenMovement(self.mainBodyChunk.pos, self.mainBodyChunk.vel * 0.02f, Mathf.Max(Mathf.Max(self.mainBodyChunk.vel.x, self.mainBodyChunk.vel.y) * 0.05f, 0f));
                    }
                    e.RailGaussed = 60;
                    int addition = 0;
                    if (e.RailDoubleRock){
                        addition = 1;
                    } else if (e.RailDoubleLilly){
                        addition = 2;
                    } else if (e.RailDoubleSpear){
                        addition = 3;
                    } else if (e.RailDoubleBomb){
                        addition = 4;
                    }
                    if (e.RailgunCD == 0){
                        e.RailgunCD = 200;
                    } else {
                        e.RailgunCD += 40 * addition;
                    }
                    if (e.RailgunCD > 400){
                        e.RailgunCD = 400;
                    }
                    e.RailgunUse += addition;
                    return;
                }
            } catch (Exception err){
                Ebug(self, err, "Throwing object error!");
                orig(self, grasp, eu);
                return;
            }
            orig(self, grasp, eu);
        }

        // Implement rock throws
        private bool Escort_RockHit(On.Rock.orig_HitSomething orig, Rock self, SharedPhysics.CollisionResult result, bool eu){
            try{
                if (self.thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        return orig(self, result, eu);
                    }
                    if (!eCon.TryGetValue(p, out Escort e)){
                        return orig(self, result, eu);
                    }
                    //self.canBeHitByWeapons = true;
                    if (result.obj == null){
                        return false;
                    }
                    self.vibrate = 20;
                    self.ChangeMode(Weapon.Mode.Free);
                    if (result.obj is Creature c){
                        float stunBonus = 60f;
                        if (ModManager.MMF && MoreSlugcats.MMF.cfgIncreaseStuns.Value && (c is Cicada || c is LanternMouse || (ModManager.MSC && c is MoreSlugcats.Yeek))){
                            stunBonus = 105f;
                        }
                        if (ModManager.MSC && self.room.game.IsArenaSession && self.room.game.GetArenaGameSession.chMeta != null){
                            stunBonus = 105f;
                        }
                        c.Violence(self.firstChunk, self.firstChunk.vel * (e.RailDoubleRock? Math.Max(result.chunk.mass*0.75f, self.firstChunk.mass) : self.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, e.Railgunner? (e.RailDoubleRock? 0.15f : 0.2f): (e.Escapist? 0.1f : 0.02f), (e.Brawler? stunBonus *= 1.5f : stunBonus));
                    }
                    else if (result.chunk != null){
                        result.chunk.vel += self.firstChunk.vel * self.firstChunk.mass / result.chunk.mass;
                    }
                    else if (result.onAppendagePos != null){
                        (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
                    }
                    self.firstChunk.vel = self.firstChunk.vel * -0.5f + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude;
                    self.room.PlaySound(SoundID.Rock_Hit_Creature, self.firstChunk);
                    if (result.chunk != null)
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, result.chunk.pos + RWCustom.Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
                    }
                    self.SetRandomSpin();
                    return true;
                }

                return orig(self, result, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Exception in Rockhit!");
                return orig(self, result, eu);
            }
        }

        private void Escort_RockThrow(On.Rock.orig_Thrown orig, Rock self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu){
            try{
                if (thrownBy == null){
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e) || 
                        !railgunRockVelFac.TryGet(p, out float rRockVel) ||
                        !railgunRockThrust.TryGet(p, out float[] rRockThr)){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (e.Escapist){
                        frc *= 0.75f;
                    }
                    if (e.Railgunner){
                        //float thruster = 5f;
                        if (e.RailDoubleRock){
                            if (!e.RailFirstWeaped){
                                e.RailFirstWeaper = self.firstChunk.vel;
                                //self.canBeHitByWeapons = false;
                                e.RailFirstWeaped = true;
                            }
                            else {
                                self.firstChunk.vel = e.RailFirstWeaper;
                                e.RailFirstWeaped = false;
                                /*
                                if (p.bodyMode == Player.BodyModeIndex.Crawl){
                                    thruster *= rRockThr[0];
                                } else if (p.bodyMode == Player.BodyModeIndex.Stand) {
                                    thruster *= rRockThr[1];
                                } else {
                                    thruster *= rRockThr[2];
                                }
                                */
                                //e.RailDoubleRock = false;
                            }
                            frc *= rRockVel;
                        }
                        else{
                            //thruster *= rRockThr[3];
                            frc *= 1.5f;
                        }
                        /*
                        BodyChunk firstChunker = p.firstChunk;
                        if (!(p.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || p.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut)){
                            if ((self.room != null && self.room.gravity == 0f) || (Mathf.Abs(self.firstChunk.vel.x) < 1f && Mathf.Abs(self.firstChunk.vel.y) < 1f)){
                                //p.firstChunk.vel += RWCustom.Custom.IntVector2ToVector2(throwDir) * rRockThr[0];
                                p.firstChunk.vel += self.firstChunk.vel.normalized * Math.Abs(thruster);
                            } else {
                                if (Esconfig_Spears(p)){
                                    p.rollDirection = (int)Mathf.Sign(self.firstChunk.vel.x);
                                }
                                if (p.animation != Player.AnimationIndex.BellySlide){
                                    p.firstChunk.vel.x = firstChunker.vel.x + Mathf.Sign(self.firstChunk.vel.x * firstChunker.vel.x) * thruster;
                                }
                            }
                        }
                        */
                    }
                }

                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Error in Rockthrow!");
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }

        }

        private void Escort_LillyThrow(On.MoreSlugcats.LillyPuck.orig_Thrown orig, MoreSlugcats.LillyPuck self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {
            try{
                if (thrownBy == null){
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e) ||
                        !railgunLillyVelFac.TryGet(p, out float rLillyVel)){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (e.Railgunner){
                        //float thruster = 5f;
                        if (e.RailDoubleLilly){
                            if (!e.RailFirstWeaped){
                                self.firstChunk.vel.x *= (float)Math.Abs(self.throwDir.x);
                                self.firstChunk.vel.y *= (float)Math.Abs(self.throwDir.y);
                                e.RailFirstWeaper = self.firstChunk.vel;
                                //self.canBeHitByWeapons = false;
                                e.RailFirstWeaped = true;
                            }
                            else {
                                self.firstChunk.vel = e.RailFirstWeaper;
                                e.RailFirstWeaped = false;
                            }
                            frc *= rLillyVel;
                        }
                    }
                }
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Error in Lillythrow!");
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }
        }

        private void Escort_BombThrow(On.ScavengerBomb.orig_Thrown orig, ScavengerBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {
            try{
                if (thrownBy == null){
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e) ||
                        !railgunBombVelFac.TryGet(p, out float rBombVel)){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (e.Railgunner){
                        //float thruster = 5f;
                        if (e.RailDoubleBomb){
                            if (!e.RailFirstWeaped){
                                e.RailFirstWeaper = self.firstChunk.vel;
                                //self.canBeHitByWeapons = false;
                                e.RailFirstWeaped = true;
                            }
                            else {
                                self.firstChunk.vel = e.RailFirstWeaper;
                                e.RailFirstWeaped = false;
                            }
                            self.canBeHitByWeapons = false;
                            if (p.input[0].y == 0){
                                self.floorBounceFrames += 20;
                            }
                            e.RailIReady = true;
                            frc *= rBombVel;
                        }
                    }

                }
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Error in Lillythrow!");
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }
        }

        private void Escort_AntiDeflect(On.Weapon.orig_WeaponDeflect orig, Weapon self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed){
            try{
                if (self.thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        orig(self, inbetweenPos, deflectDir, bounceSpeed);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e)){
                        orig(self, inbetweenPos, deflectDir, bounceSpeed);
                        return;
                    }
                    if (e.Railgunner && (e.RailDoubleRock || e.RailDoubleSpear || e.RailDoubleLilly || e.RailDoubleBomb || (e.RailGaussed > 0 && self.thrownBy == e.RailThrower))){
                        Ebug(p, "NO DEFLECTING");
                        return;
                    }

                }
                orig(self, inbetweenPos, deflectDir, bounceSpeed);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Weapon Anti-Deflect failed!");
                orig(self, inbetweenPos, deflectDir, bounceSpeed);
            }
        }

        private void Escort_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp){
            orig(self, grasp);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if(
                    !eCon.TryGetValue(self, out Escort e)
                    ){
                    return;
                }

                if (e.Escapist){
                    e.EscUnGraspLimit = 120;
                    if (grasp.grabber is Lizard){
                        e.EscUnGraspLimit = 80;
                    }
                    else if (grasp.grabber is Vulture){
                        e.EscUnGraspLimit = 60;
                    }
                    else if (grasp.grabber is BigSpider){
                        e.EscUnGraspLimit = 150;
                    }
                    else if (grasp.grabber is DropBug){
                        e.EscUnGraspLimit = 90;
                    }
                    else if (grasp.grabber is Centipede){
                        e.EscUnGraspLimit = 40;
                    }
                    e.EscDangerGrasp = grasp;
                    e.EscUnGraspTime = e.EscUnGraspLimit;
                }

            } catch (Exception err){
                Ebug(self, err);
                return;
            }

        }


        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
                if (obj == null){
                    return orig(self, obj);
                }
                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                    return Player.ObjectGrabability.TwoHands;
                }
                if (!dualWielding.TryGet(self, out bool dW) ||
                    !eCon.TryGetValue(self, out Escort e)){
                    return orig(self, obj);
                }

                if (dW && e.dualWield){
                    if (obj is Weapon){
                        // Any weapon is dual-wieldable, including spears
                        return Player.ObjectGrabability.OneHand;
                    }
                    if (e.Brawler){
                        for (int i = 0; i < 2; i++){
                            if (
                                self.grasps[i] != null &&
                                self.grasps[i].grabbed != obj &&
                                self.grasps[i].grabbed is Spear &&
                                obj is Creature c && !c.dead &&
                                c.Stunned){
                                if (c is Lizard l && Esconfig_Dunkin(self) && !e.LizardDunk){
                                    if (e.LizGoForWalk == 0){
                                        e.LizGoForWalk = 320;
                                    }
                                    e.LizardDunk = true;
                                }
                                return Player.ObjectGrabability.OneHand;
                            }
                        }
                    }
                    if (obj is Lizard lizzie){
                        // Any lizards that are haulable (while dead) or stunned are dual-wieldable
                        if (lizzie.dead){
                            return Player.ObjectGrabability.OneHand;
                        } else if (lizzie.Stunned && Esconfig_Dunkin(self) && !e.LizardDunk){
                            if (e.LizGoForWalk == 0){
                                e.LizGoForWalk = 320;
                            }
                            if (!Esconfig_SFX(self)) {
                                self.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, self.mainBodyChunk);
                            }
                            e.LizardDunk = true;
                            return Player.ObjectGrabability.TwoHands;
                        }
                    }
                }
                return orig(self, obj);
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, obj);
            }
        }

        private void Escort_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, eu);
                    return;
                }
            } catch (Exception err){
                Ebug(self, err, "Grab update!");
                orig(self, eu);
                return;
            }
            if(
                !eCon.TryGetValue(self, out Escort e)
                ){
                orig(self, eu);
                return;
            }
            /* Eat meat faster?
            int n = 0;
            if ((self.grasps[0] == null || !(self.grasps[0].grabbed is Creature)) && self.grasps[1] != null && self.grasps[1].grabbed is Creature){
                n = 1;
            }
            if (self.input[0].pckp && self.grasps[n] != null && self.grasps[n].grabbed is Creature && self.CanEatMeat(self.grasps[n].grabbed as Creature) && (self.grasps[n].grabbed as Creature).Template.meatPoints > 1){
                //self.EatMeatUpdate(n);
            }*/
            orig(self, eu);
            if (e.Railgunner){
                if (e.RailWeaping == 0){
                    e.RailDoubleSpear = false;
                    e.RailDoubleRock = false;
                    e.RailDoubleLilly = false;
                    e.RailDoubleBomb = false;
                }
                for (int b = 0; b < 2; b++){
                    if (self.grasps[b] == null){
                        return;
                    }
                }
                if (self.grasps[0].grabbed is Spear && self.grasps[1].grabbed is Spear){
                    e.RailDoubleSpear = true;
                    e.RailWeaping = 4;
                }
                else if (self.grasps[0].grabbed is Rock && self.grasps[1].grabbed is Rock){
                    e.RailDoubleRock = true;
                    e.RailWeaping = 4;
                }
                else if (self.grasps[0].grabbed is MoreSlugcats.LillyPuck && self.grasps[1].grabbed is MoreSlugcats.LillyPuck){
                    e.RailDoubleLilly = true;
                    e.RailWeaping = 4;
                }
                else if (self.grasps[0].grabbed is ScavengerBomb && self.grasps[1].grabbed is ScavengerBomb){
                    e.RailDoubleBomb = true;
                    e.RailWeaping = 4;
                }
            }
        }

        private float Escort_DeathBiteMult(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            try{
                if (!eCon.TryGetValue(self, out Escort e)){
                    return orig(self);
                }
                if (self.slugcatStats.name.value == "EscortMe"){
                    float biteMult = 0.5f;
                    if (e.Brawler){
                        biteMult -= 0.35f;
                    }
                    if (e.Railgunner){
                        biteMult += 0.35f;
                    }
                    if (e.Escapist){
                        biteMult -= 0.15f;
                    }
                    if (Eshelp_ParryCondition(self) || (!e.Deflector && self.animation == Player.AnimationIndex.RocketJump)){
                        biteMult = 5f;
                    }
                    Ebug(self, "Lizard bites with multiplier: " + biteMult);
                    return biteMult;
                } else {
                    return orig(self);
                }
            } catch (Exception err){
                Ebug(self, err, "Couldn't set deathbitemultipler!");
                return orig(self);
            }
        }

        private void Escort_Die(On.Player.orig_Die orig, Player self){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    orig(self);
                    return;
                }

                Ebug(self, "Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0){
                    orig(self);
                    if (self.dead && Esconfig_SFX(self) && self.room != null){
                        self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug(self, "Failure.", 1);
                } else {
                    self.dead = false;
                    Ebug(self, "Player didn't die?", 1);
                    e.ParrySuccess = false;
                }
            } catch (Exception err){
                Ebug(self, err, "Something happened while trying to die!");
                orig(self);
            }
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

        private void Escort_Hip_Replacement(On.Room.orig_Loaded orig, Room self)
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
                for (int i = 0; i < self.abstractRoom.entities.Count; i++){
                    if (self.abstractRoom.entities[i] != null && self.abstractRoom.entities[i] is AbstractSpear spear){
                        if (UnityEngine.Random.value > 0.8f && !spear.explosive && !spear.electric){
                            self.abstractRoom.entities[i] = new AbstractSpear(spear.world, null, spear.pos, spear.ID, false){
                                needle = true
                            };
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
                
            } catch (Exception err){
                Ebug(err, "Something happened while swapping spears!");
            }
        }
    
        private void Escort_Eated(On.Player.orig_BiteEdibleObject orig, Player self, bool eu)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, eu);
                    return;
                }
                for (int a = 0; a < 2; a++){
                    if (self.grasps[a] != null && self.grasps[a].grabbed is IPlayerEdible ipe && ipe.Edible){
                        if (ipe.BitesLeft > 1){
                            if (self.grasps[a].grabbed is Fly){
                                for (int b = 0; b < ipe.BitesLeft; b++){
                                    ipe.BitByPlayer(self.grasps[a], eu);
                                }
                            } else {
                                ipe.BitByPlayer(self.grasps[a], eu);
                            }
                        }
                        break;
                    }
                }
                orig(self, eu);
            } catch (Exception err){
                Ebug(self, err, "Error when eated!");
                orig(self, eu);
                return;
            }
        }


    }
}