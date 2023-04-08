using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[WIP] The Escort", "0.2.2.4")]
    class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public EscOptions config;
        private const string MOD_ID = "urufudoggo.theescort";
        public Plugin(){
            try{
                this.config = new EscOptions(this, base.Logger);
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
        public static readonly PlayerFeature<float> LiftHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DKM = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static readonly PlayerFeature<int> Escomet = PlayerInt("theescort/headbutt");
        public static readonly PlayerFeature<bool> Elvator = PlayerBool("theescort/elevator");
        public static readonly PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");
        public static readonly PlayerFeature<bool> HypeSys = PlayerBool("theescort/adrenaline_system");
        public static readonly PlayerFeature<float> HypeReq = PlayerFloat("theescort/stamina_req");
        public static readonly PlayerFeature<int> RR = PlayerInt("theescort/reset_rate");

        /* JSON VALUES
        ["Hyped spear damage", "Base spear damage"]
        */
        public static readonly PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");
        public static readonly PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");
        public static readonly PlayerFeature<bool> soundAhoy = PlayerBool("theescort/sounds_ahoy");

        /* JSON VALUES
        ["Min swim power (affected by viscosity of water)", "Max swim power", "deepswim X velocity", "deepswim Y velocity", "surfaceswim X velocity", "surfaceswim Y velocity"]
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


        public static SoundID Escort_SFX_Death;
        public static SoundID Escort_SFX_Flip;
        public static SoundID Escort_SFX_Roll;
        public static SoundID Escort_SFX_Lizard_Grab;
        //public static SoundID Escort_SFX_Spawn;

        //public DynamicSoundLoop escortRollin;

        // Miscellanious things
        private int slowDownDevConsole = 0;
        private bool nonArena = false;
        //public static readonly String EscName = "EscortMe";
        /*
        Log Priority:
        -1: No logs
         0: Exceptions
         1: Important things
         2: Less important things
         3: Method pings
        */
        private int logImportance = 3;

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
            On.Player.SpearStick += Escort_StickySpear;

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
                Ebug("Found Dress My Slugcat! Applying patch...", 1);
                //escPatch_DMS = true;
                Eshelp_Patch_DMS();
            }
            } catch (Exception err){
                Ebug(err, "Something happened while searching for mods!");
            }
        }

        private static void Eshelp_Patch_DMS(){
            try{// Dress My Slugcat Patch
                DressMySlugcat.SpriteDefinitions.AvailableSprites.Add(new DressMySlugcat.SpriteDefinitions.AvailableSprite{
                    Name = "MARKINGS",
                    Description = "Markings",
                    GallerySprite = "escortHipT",
                    RequiredSprites = new List<string> {"escortHeadT", "escortHipT"},
                    Slugcats = new List<string>{"EscortMe"}
                });
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

        private bool Esconfig_Build(Player self){
            try {
                if (!eCon.TryGetValue(self, out Escort e)){
                    return false;
                }
                int pal = 0;
                switch (self.playerState.playerNumber){
                    case 0:
                        pal = config.cfgBuildP1.Value;
                        break;
                    case 1:
                        pal = config.cfgBuildP2.Value;
                        break;
                    case 2:
                        pal = config.cfgBuildP3.Value;
                        break;
                    case 3:
                        pal = config.cfgBuildP4.Value;
                        break;
                }
                switch (pal){
                    // Beginner build
                    // Full Melee build ()
                    // Speedstar build (Fast (when running, crawling, etc.), and gets faster when hyped)

                    case -2:  // Deflector build
                        e.parryTech = true;
                        self.slugcatStats.runspeedFac -= 0.1f;
                        self.slugcatStats.corridorClimbSpeedFac -= 0.15f;
                        self.slugcatStats.poleClimbSpeedFac -= 0.15f;
                        self.slugcatStats.throwingSkill = 1;
                        Ebug("Deflector Build selected!", 2);
                        break;
                    case -1:  // Brawler build
                        e.combatTech = false;
                        self.slugcatStats.runspeedFac += 0.1f;
                        Ebug("Brawler Build selected!", 2);
                        break;
                    default:  // Default build
                        Ebug("Default Build selected!", 2);
                        break;
                }
                Ebug("Set build complete!", 1);
                Ebug("Movement Speed: " + self.slugcatStats.runspeedFac, 2);
                return true;
            } catch (Exception err){
                Ebug(err, "Something went wrong when setting an Escort build!");
                return false;
            }
        }


        /*
        Miscellaneous!
        */
        private void Eshelp_Log_Timer(Player self){
            if (RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                slowDownDevConsole = 0;
            } else {
                slowDownDevConsole++;
            }
        }

        private void Eshelp_Tick(Player self){
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
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
                e.parryAirLean = 9;
            }

            // Increased damage when parry tick
            if (e.parryExtras > 0){
                e.parryExtras--;
            }

            // Headbutt cooldown
            if (e.CometFrames > 0){
                e.CometFrames--;
            } else {
                e.Cometted = false;
            }

            // Invincibility Frames
            if (e.iFrames > 0){
                Ebug("IFrames: " + e.iFrames, 2);
                e.iFrames--;
            } else {
                e.ElectroParry = false;
            }

            // Smooth color/brightness transition
            if (requirement <= self.aerobicLevel && e.smoothTrans < 15){
                e.smoothTrans++;
            }
            else if (requirement > self.aerobicLevel && e.smoothTrans > 0){
                e.smoothTrans--;
            }

            // Lizard dropkick leniency
            if (e.LizDunkLean > 0 && e.LizardDunk){
                e.LizDunkLean--;
            }
            else{
                e.LizardDunk = false;
                if (e.LizDunkLean == 0){
                    e.LizGoForWalk = 0;
                }
            }

            // Lizard grab timer
            if (e.LizGoForWalk > 0){
                e.LizGoForWalk--;
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
            Ebug("Ctor Triggered!");
            orig(self, abstractCreature, world);

            if (self.slugcatStats.name.value == "EscortMe"){
                eCon.Add(self, new Escort(self));
                if (!eCon.TryGetValue(self, out Escort e)){
                    Ebug("Something happened while initializing Escort instance!", 0);
                    return;
                }
                Esconfig_Build(self);
                try {
                    Ebug("Setting silly sounds", 2);
                    e.Esclass_setSFX_roller(Escort_SFX_Roll);
                    e.Esclass_setSFX_lizgrab(Escort_SFX_Lizard_Grab);
                    Ebug("All done! Awaiting activation.", 2);
                    
                    /*
                    Color col = new Color(0.796f, 0.549f, 0.27843f);
                    e.Esclass_set_hypeLight(self, col);
                    Ebug("Setting hyped light", 2);
                    */
                    // April fools!
                    //self.setPupStatus(set: true);
                    //self.room.PlaySound(Escort_SFX_Spawn, self.mainBodyChunk);
                } catch (Exception err){
                    Ebug(err, "Error while constructing!");
                } finally {
                    Ebug("All ctor'd", 1);
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
                Ebug("Resized array", 1);
                s.sprites[e.spriteQueue] = new FSprite("escortHeadT");
                s.sprites[e.spriteQueue + 1] = new FSprite("escortHipT");
                if (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null){
                    Ebug("Oh geez. No sprites?", 0);
                }
                Ebug("Set the sprites", 1);
                self.AddToContainer(s, rCam, null);
                Ebug("Sprite init complete!", 1);
            } catch(Exception err){
                Ebug(err, "Something went wrong when initiating sprites!");
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
                    Ebug("Oh dear. Null sprites!!", 0);
                    return;
                }
                Color c = new Color(0.796f, 0.549f, 0.27843f);
                // Applying colors?
                if (s.sprites.Length > e.spriteQueue){
                    Ebug("Gone in", 2);
                    if (ModManager.CoopAvailable && self.useJollyColor){
                        Ebug("Jollymachine", 2);
                        c = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        Ebug("R: " + c.r + " G: " + c.g + " B: " + c.b);
                        Ebug("Jollymachine end", 2);
                    }
                    else if (PlayerGraphics.CustomColorsEnabled()){
                        Ebug("Custom color go brr", 2);
                        c = PlayerGraphics.CustomColorSafety(2);
                        Ebug("Custom color end", 2);
                    }
                    else{
                        Ebug("Arenasession or Singleplayer", 2);

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
                        Ebug("Arena/Single end.", 2);
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
                Ebug(err, "Something went wrong when coloring in the palette!");
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
                    Ebug("Oh shoot. Where sprites?", 0);
                    return;
                }
                if (e.spriteQueue < s.sprites.Length){
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue + 1]);
                    Ebug("Removal success.", 1);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue + 1]);
                    Ebug("Addition success.", 1);
                    s.sprites[e.spriteQueue].MoveBehindOtherNode(s.sprites[3]);
                    s.sprites[e.spriteQueue].MoveBehindOtherNode(s.sprites[9]);
                    Ebug("Restructure success.", 1);
                }
            } catch(Exception err){
                Ebug(err, "Something went wrong when adding graphics to container!");
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
                    Ebug("Oh crap. Sprites? Hello?!", 0);
                    return;
                }
                if (s.sprites.Length > e.spriteQueue){
                    // Hypelevel visual fx
                    try{
                        if (self.player != null && Esconfig_Hypable(self.player)){
                            float alphya = 1f;
                            if (requirement > self.player.aerobicLevel){
                                alphya = Mathf.Lerp((self.player.dead? 0f : 0.5f), 1f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
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
                        Ebug(err, "something went wrong when altering alpha");
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
                Ebug(err, "Something happened while trying to draw sprites!");
            }
        }


        // Implement Escort's slowed stamina increase
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            if (!Exhausion.TryGet(self, out float exhaust)){
                orig(self, f);
                return;
            }
            if (self.slugcatStats.name.value == "EscortMe"){
                // Due to the aerobic decrease found in some movements implemented in Escort, the AerobicIncrease actually does the original, and on top of that the additional to balance things out.
                orig(self, f);

                //Ebug("Aerobic Increase Triggered!");
                if (!self.slugcatStats.malnourished){
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / exhaust));
                } else {
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / (exhaust / 2)));
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
                    Ebug("Attempted to access a nulled player when updating!", 0);
                    return;
                }
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err);
                return;
            }

            if (
                !RR.TryGet(self, out int limiter) ||
                !WallJumpVal.TryGet(self, out var WJV) ||
                !NoMoreGutterWater.TryGet(self, out var theGut) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            // For slowed down dev console output
            Eshelp_Log_Timer(self);

            // Cooldown/Frames Tick
            Eshelp_Tick(self);
            
            // Just for seeing what a variable does.
            if(limiter < slowDownDevConsole){
                Ebug("Clocked.");
                Ebug(" Roll Direction: " + self.rollDirection);
                Ebug("Slide Direction:" + self.slideDirection);
                Ebug(" Flip Direction: " + self.flipDirection);
                //Ebug(self.abstractCreature.creatureTemplate.baseDamageResistance);
                //Ebug("Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                //Ebug("Normalized direction: " + self.bodyChunks[0].vel.normalized);
            }

            // vfx
            if(self != null && self.room != null){
                Esconfig_HypeReq(self);

                /*
                // Battle-hyped visual effect
                if (Esconfig_Hypable(self) && Esconfig_HypeReq(self) && self.aerobicLevel > requirement){
                    Color hypedColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    hypedColor.a = 0.8f;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, hypedColor));
                }*/

                // Charged pounces Visual Effect
                if (Esconfig_Pouncing(self)){
                    Color pounceColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);

                    if (self.superLaunchJump > 19){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 9f, 4f, 4f, 11f, pounceColor));
                    }
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.consistentDownDiagonal >= (int)WJV[4] && self.allowRoll == 15){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, pounceColor));
                    }
                }

                // Empowered damage from parry visual effect
                if (e.parryExtras > 0){
                    Color empoweredColor = new Color(1f, 0.7f, 0.35f, 0.7f);
                    //empoweredColor.a = 0.7f;
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    self.room.AddObject(new Spark(self.bodyChunks[0].pos, new Vector2(2f * (e.parryExtras%2==0? 1: -1), Mathf.Lerp(-1f, 1f, UnityEngine.Random.value)), empoweredColor, null, 9, 13));
                }
            }

            // Implement guuh wuuh
            if(self.bodyMode == Player.BodyModeIndex.Swimming){
                float superSwim = Mathf.Lerp(theGut[0], theGut[1], self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity));
                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[2] * superSwim, theGut[3] * superSwim);
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[4] * superSwim, theGut[5] * superSwim);
                }
            }

            // Check if player is grabbing a lizard
            if (Esconfig_Dunkin(self)){
                try{
                    for (int i = 0; i < self.grasps.Length; i++){
                        if (self.grasps[i] != null && self.grasps[i].grabbed is Lizard lizzie && !lizzie.dead){
                            e.LizDunkLean = 60;
                            if (e.LizGoForWalk > 0){
                                lizzie.Violence(null, null, lizzie.mainBodyChunk, null, Creature.DamageType.Electric, 0f, 40f);
                            }
                            break;
                        }
                    }
                } catch (Exception err){
                    Ebug(err, "Something went wrong when checking for lizard grasps");
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
                Ebug(err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }

            Ebug("Jump Triggered!");
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
                Ebug("FLIPERONI GO!", 2);

                if (Esconfig_SFX(self)){
                    self.room.PlaySound(Escort_SFX_Flip, e.SFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
            self.consistentDownDiagonal = 0;
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
                Ebug(err);
                return;
            }
            if (!WallJumpVal.TryGet(self, out var WJV) ||
                !eCon.TryGetValue(self, out Escort e)){
                orig(self, direction);
                return;
            }

            Ebug("Walljump Triggered!");
            bool wallJumper = Esconfig_WallJumps(self);
            bool longWallJump = (self.superLaunchJump > 19 && wallJumper);
            bool superWall = (Esconfig_Pouncing(self) && self.consistentDownDiagonal > (int)WJV[4]);
            bool superFlip = self.allowRoll == 15 && Esconfig_Pouncing(self);

            // If charge wall jump is enabled and is able to walljump, or if charge wall jump is disabled
            if ((wallJumper && self.canWallJump != 0) || !wallJumper) {
                orig(self, direction);
                float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
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
                    Ebug("Y Velocity" + self.bodyChunks[0].vel.y, 2);
                    Ebug("Y Velocity" + self.bodyChunks[1].vel.y, 2);
                    Ebug("X Velocity" + self.bodyChunks[0].vel.x, 2);
                    Ebug("X Velocity" + self.bodyChunks[1].vel.x, 2);
                }
                self.jumpBoost = 0f;

                if (superFlip && superWall){
                    self.animation = Player.AnimationIndex.Flip;
                    self.room.PlaySound((Esconfig_SFX(self)? Escort_SFX_Flip : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.SFXChunk, false, (Esconfig_SFX(self)? 1f : 2f), 0.9f);
                    self.jumpBoost += Mathf.Lerp(WJV[6], WJV[7], Mathf.InverseLerp(WJV[4], WJV[5], self.consistentDownDiagonal));
                    toPrint.SetValue("SUPERFLIP", 2);
                } else {
                    toPrint.SetValue("not so flip", 2);
                }
                Ebug("Jumpboost" + self.jumpBoost, 2);
                Ebug("CDownDir" + self.consistentDownDiagonal, 2);
                Ebug("SLaunchJump" + self.superLaunchJump, 2);
                if (self.superLaunchJump > 19){
                    self.superLaunchJump = 0;
                }
                self.canWallJump = 0;
                Ebug(toPrint, 2);
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
                Ebug(err);
                return;
            }
            if (!Esconfig_WallJumps(self)){
                return;
            }
            if (!RR.TryGet(self, out int limiter)){
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
                if(limiter < slowDownDevConsole){
                    Ebug(msg, 2);
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
                Ebug(err);
                return;
            }
            if(!Esconfig_WallJumps(self)){
                orig(self);
                return;
            }

            //Ebug("CheckInput Triggered!");
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
            } catch (Exception err){
                Ebug(err);
                return orig(self, obj);
            }
            if (!Esconfig_Heavylift(self)){
                return orig(self, obj);
            }
            if (!RR.TryGet(self, out int resetRate) ||
                !eCon.TryGetValue(self, out Escort e)){
                return orig(self, obj);
            }

            if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                if (slowDownDevConsole > resetRate){
                    Ebug("Revivify skip!", 1);
                    Ebug("Creature: " + creature.GetType(), 1);
                    Ebug("Player: " + self.GetOwnerType(), 1);
                }
                return orig(self, creature);
            }

            //Ebug("Heavycarry Triggered!");
            if (obj.TotalMass <= self.TotalMass * ratioed){
                if (ModManager.CoopAvailable && obj is Player player && player != null){
                    return !player.isSlugpup;
                }
                return false;
            }
            return orig(self, obj);
        }

        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err);
                return;
            }
            if (
                !BetterCrawl.TryGet(self, out var crawlSpeed) ||
                !BetterPoleWalk.TryGet(self, out var poleMove) ||
                !Escomet.TryGet(self, out int SetComet) ||
                !eCon.TryGetValue(self, out Escort e)
            ){
                return;
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Implement bettercrawl
            if (!e.parryTech && self.bodyMode == Player.BodyModeIndex.Crawl){
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
            else if (!e.parryTech && self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam && (self.animation == Player.AnimationIndex.StandOnBeam || self.animation == Player.AnimationIndex.HangFromBeam)){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(poleMove[0], poleMove[1], self.aerobicLevel): poleMove[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(poleMove[2], poleMove[3], self.aerobicLevel): poleMove[5]) * self.slugcatStats.runspeedFac;
            } 
            
            // Set headbutt condition
            else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb){
                if (self.slowMovementStun > 0){
                    e.CometFrames = SetComet;
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
                Ebug(err);
                return;
            }
            if (!RR.TryGet(self, out int limiter) ||
                !eCon.TryGetValue(self, out Escort e)){
                return;
            }

            //Ebug("UpdateAnimation Triggered!");
            // Infiniroll
            if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                e.RollinCount++;
                if(limiter < slowDownDevConsole){
                    Ebug("Rollin at: " + e.RollinCount, 2);
                }
                if(Esconfig_SFX(self) && e.Rollin != null){
                    e.Rollin.Volume = Mathf.InverseLerp(100f, 300f, e.RollinCount);
                }
                self.rollCounter = 0;
            }

            if (self.animation != Player.AnimationIndex.Roll){
                e.RollinCount = 0f;
            }

            // I'll find out how to implement a more lineant slide (like rivulet's slide pounces) while keeping it short (like every other slugcats) one day...
            //if (self.animation == Player.AnimationIndex.BellySlide){
                // TODO implement better slide
            //}
        }


        public bool Eshelp_ParryCondition(Creature self){
            if (self is Player player){
                if (!eCon.TryGetValue(player, out Escort e)){
                    return false;
                }
                if (e.parryTech && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll)){
                    Ebug("Parryteched condition!", 2);
                    return true;
                }
                else if (player.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0){
                    Ebug("Regular parry condition!", 2);
                    return true;
                }
                else {
                    Ebug("Not in parry condition", 2);
                    Ebug("Parry leniency: " + e.parryAirLean, 2);
                    return e.parrySlideLean > 0;
                }
            }
            return false;
        }
        public bool Eshelp_ParryCondition(Player self){ 
            if (!eCon.TryGetValue(self, out Escort e)){
                return false;
            }
            if (e.parryTech && (self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.Roll)){
                Ebug("Parryteched condition!", 2);
                return true;
            }
            else if (self.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0){
                Ebug("Regular parry condition!", 2);
                return true;
            }
            else {
                Ebug("Not in parry condition", 2);
                Ebug("Parry leniency: " + e.parryAirLean);
                return e.parrySlideLean > 0;
            }
        }

        // Implement Parryslide/midair projectile grab
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){
            try{
                if (self is Player && (self as Player).slugcatStats.name.value != "EscortMe"){
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    return;
                }
            } catch (Exception err){
                Ebug(err);
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



            Ebug("Violence Triggered!");
            // connects to the Escort's Parryslide option
            e.ParrySuccess = false;
            if (Eshelp_ParryCondition(player)){
                // Parryslide (parry module)
                Ebug("Escort attempted a Parryslide", 2);
                int direction;
                direction = player.slideDirection;

                Ebug("Is there a source? " + (source != null), 2);
                Ebug("Is there a direction & Momentum? " + (directionAndMomentum != null), 2);
                Ebug("Is there a hitChunk? " + (hitChunk != null), 2);
                Ebug("Is there a hitAppendage? " + (hitAppendage != null), 2);
                Ebug("Is there a type? " + (type != null), 2);
                Ebug("Is there damage? " + (damage > 0f), 2);
                Ebug("Is there stunBonus? " + (stunBonus > 0f), 2);

                if (source != null) {
                    Ebug("Escort is being assaulted by: " + source.owner.GetType(), 2);
                }
                Ebug("Escort parry is being checked", 1);
                if (type != null){
                    Ebug("Escort gets hurt by: " + type.value, 2);
                if (type == Creature.DamageType.Bite){
                    Ebug("Escort is getting BIT?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 35;
                        //(self as Player).WallJump(direction);
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug("Escort got out of a creature's mouth!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug("Weapons can BITE?!", 2);
                    } 
                    else {
                        Ebug("Where is Escort getting bit from?!", 2);
                    }
                } 
                else if (type == Creature.DamageType.Stab) {
                    Ebug("Escort is getting STABBED?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug("Escort parried a stabby creature?", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon) {
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug("Escort parried a stabby weapon", 2);
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
                                Ebug("Tusk unimpaled!", 2);
                                break;
                            }
                        }
                        e.ParrySuccess = true;
                        Ebug("Escort parried a generic stabby thing", 2);
                    }
                } 
                else if (type == Creature.DamageType.Blunt) {
                    Ebug("Escort is getting ROCC'ED?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug("Creatures aren't rocks...", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon){
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(weapon.firstChunk.lastPos, -vector, weapon.firstChunk.vel.magnitude);
                        damage = 0f;
                        stunBonus = stunBonus / 5f;
                        e.ParrySuccess = true;
                        Ebug("Escort bounces a blunt thing.", 2);
                    } 
                    else {
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug("Escort parried something blunt.", 2);
                    }
                } 
                else if (type == Creature.DamageType.Water) {
                    Ebug("Escort is getting Wo'oh'ed?!", 1);
                } 
                else if (type == Creature.DamageType.Explosion) {
                    Ebug("Escort is getting BLOWN UP?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug("Wait... creatures explode?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug("Escort parries an explosion from weapon?!", 2);
                    } 
                    else {
                        player.WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug("Escort parries an explosion", 2);
                    }
                } 
                else if (type == Creature.DamageType.Electric) {
                    Ebug("Escort is getting DEEP FRIED?!", 1);
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
                        Ebug("Escort somehow parried a shock from creature?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        //type = Creature.DamageType.Blunt;
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug("Escort somehow parried a shock object?!", 2);
                    } 
                    else {
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug("Escort attempted to parry a shock but why?!", 2);
                    }
                } 
                else {
                    Ebug("Escort is getting UNKNOWNED!!! RUNNN", 1);
                    if (source != null && source.owner is Creature){
                        Ebug("IT'S ALSO AN UNKNOWN CREATURE!!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug("IT'S ALSO AN UNKNOWN WEAPON!!", 2);
                    } 
                    else {
                        Ebug("WHO THE HECK KNOWS WHAT IT IS?!", 2);
                    }
                }
                }
            }

            // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
            // Visual indicator doesn't work ;-;
            if (e.ParrySuccess){
                if (e.parryTech){
                    stunBonus = 0;
                    player.Jump();
                    player.animation = Player.AnimationIndex.Flip;
                    player.mainBodyChunk.vel.y *= 1.5f;
                    player.mainBodyChunk.vel.x *= 0.15f;
                    self.room.PlaySound(SoundID.Snail_Warning_Click, self.mainBodyChunk, false, 1.6f, 0.7f);
                    e.parryExtras = 160;
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
                Ebug("Parry successful!", 1);
                e.iFrames = 6;
                e.parrySlideLean = 0;
            }
            else if (e.iFrames > 0) {
                if (e.ElectroParry){
                    damage = 0f;
                    stunBonus = stunBonus * 0.5f;
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    Ebug("Stun Resistance frame tick", 2);
                } else {
                    Ebug("Immunity frame tick", 2);
                }
            } 
            else {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                Ebug("Nothing or not possible to parry!", 1);
            }
            Ebug("Parry Check end", 1);
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
                Ebug(err);
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!ParrySlide.TryGet(self, out bool parrier)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!parrier || (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }

            Ebug("Sticky Triggered!");
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
                Ebug(err);
                return;
            }
            if (
                !RR.TryGet(self, out int resetRate) ||
                !BodySlam.TryGet(self, out float[] bodySlam) ||
                !TrampOhLean.TryGet(self, out float bounce) ||
                !Esconfig_HypeReq(self) ||
                !Esconfig_DKMulti(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            //Ebug("Collision Triggered!");
            if (slowDownDevConsole > resetRate){
                Ebug("Escort collides!");
                Ebug("Has physical object? " + otherObject != null);
                if (otherObject != null){
                    Ebug("What is it? " + otherObject.GetType());
                }
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Reimplementing the elevator... the way it was in its glory days
            if (Esconfig_Elevator(self) && otherObject is Creature && self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                self.jumpBoost += 4;
            }


            if (otherObject is Creature creature && 
                creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !(ModManager.CoopAvailable && otherObject is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){


                // Creature Trampoline (or if enabled Escort's Elevator)
                /*
                Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
                */
                if (self.animation == Player.AnimationIndex.Flip && self.bodyMode == Player.BodyModeIndex.Default && (!creature.dead || creature is Lizard)){
                    if (self.jumpBoost <= 0) {
                        self.jumpBoost = bounce;
                    }
                    if (e.parryTech){
                        try{
                            if (self.mainBodyChunk.vel.y < 6f){
                                self.mainBodyChunk.vel.y += 10f;
                            }
                            self.Violence(null, null, self.mainBodyChunk, null, Creature.DamageType.Blunt, 0f, 0f);
                        } catch (Exception err){
                            Ebug(err, "Hitting thyself failed!");
                        }
                    }
                }

                int direction;

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    creature.SetKillTag(self.abstractCreature);

                    if (e.parrySlideLean <= 0){
                        e.parrySlideLean = 4;
                    }
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,e.SFXChunk);

                    float normSlideStun = (hypedMode || e.combatTech? bodySlam[1] : bodySlam[1] * 1.5f);
                    if (hypedMode && self.aerobicLevel > requirement){
                        normSlideStun = bodySlam[1] * (e.combatTech? 1.75f : 2f);
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        creature.firstChunk, null, (e.parryExtras > 0? Creature.DamageType.Stab : Creature.DamageType.Blunt),
                        (e.parryExtras > 0? 0.4f : bodySlam[0]), normSlideStun
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
                        if (e.combatTech){
                            self.animation = Player.AnimationIndex.BellySlide;
                            self.bodyChunks[1].vel = new Vector2((float)self.slideDirection * 18f, 0f);
                            self.bodyChunks[0].vel = new Vector2((float)self.slideDirection * 18f, 5f);
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                        }
                        Ebug("Greatdadstance stunslide!", 2);
                    } else {
                        direction = self.flipDirection;
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.Flip;
                        Ebug("Stunslided!", 2);
                    }
                    }

                // Dropkick
                else if (self.animation == Player.AnimationIndex.RocketJump){
                    creature.SetKillTag(self.abstractCreature);

                    String message = "Dropkicked!";
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk);
                    
                    if (!creature.dead) {
                        DKMultiplier *= creature.TotalMass;
                    }
                    float normSlamDamage = 0.1f;
                    if (e.DropKickCD == 0){
                        normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + (e.combatTech? 0.15f : 0.27f));
                        creature.LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * (e.combatTech? 1.6f : 2f);}
                        if (e.parryTech){
                            if (e.parryExtras > 0){
                                normSlamDamage *= 3f;
                            }
                            else {
                                normSlamDamage *= 0.5f;
                            }
                        }
                        message = "Powerdropkicked!";
                    } else {
                        //multiplier *= 0.5f;
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*DKMultiplier, self.mainBodyChunk.vel.y*DKMultiplier*(e.LizardDunk?0.2f:1f))),
                        creature.firstChunk, null, ((e.parryExtras > 0 && e.DropKickCD == 0)? Creature.DamageType.Explosion : Creature.DamageType.Blunt),
                        normSlamDamage, (e.parryExtras > 0? bodySlam[1] : bodySlam[3])
                    );
                    Ebug("Dunk the lizard: " + e.LizardDunk, 2);
                    if (e.DropKickCD == 0){
                        e.LizardDunk = false;
                    }
                    if (e.parryExtras > 0){
                        e.parryExtras = 0;
                    }
                    e.DropKickCD = (self.longBellySlide? 25 : 12);
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    /*
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }*/
                    direction = -self.flipDirection;
                    self.WallJump(direction);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    Ebug(message + " Dmg: " + normSlamDamage, 2);
                    }

                else if (e.CometFrames > 0 && !e.Cometted){
                    creature.SetKillTag(self.abstractCreature);
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x*DKMultiplier, self.bodyChunks[0].vel.y*DKMultiplier)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        0f, 15f
                    );
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x*(DKMultiplier/2);
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y*(DKMultiplier/2);
                    if (self != null && self.room != null){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    }
                    Ebug("Headbutted!", 2);
                    e.Cometted = true;
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
            } catch (Exception err){
                Ebug(err);
                return;
            }
            if (!BodySlam.TryGet(self, out var bodySlam) ||
                !eCon.TryGetValue(self, out Escort e)){
                return;
            }

            Ebug("Toss Object Triggered!");
            if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead){
                if (Esconfig_SFX(self) && e.LizGet != null){
                    e.LizGet.Volume = 0f;
                }
                if (self.bodyMode == Player.BodyModeIndex.Default){
                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyChunks[1].vel.x += self.slideDirection;
                }
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
                Ebug(err);
                return;
            }
            if(
                !bonusSpear.TryGet(self, out float[] spearDmgBonuses) ||
                !Esconfig_HypeReq(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            Ebug("ThrownSpear Triggered!");
            float thrust = 7f;
            bool onPole = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
            bool doNotYeet = onPole || !e.combatTech;
            if (Esconfig_Hypable(self)){
                if (self.aerobicLevel > requirement){
                    spear.throwModeFrames = -1;
                    spear.spearDamageBonus = spearDmgBonuses[0];
                    if (self.canJump != 0 && !self.longBellySlide){
                        if (!doNotYeet){
                            self.rollCounter = 0;
                            if (self.input[0].jmp && self.input[0].thrw){
                                self.animation = Player.AnimationIndex.BellySlide;
                                self.whiplashJump = true;
                                spear.firstChunk.vel.x *= 1.8f;
                                Ebug("Spear Go!?", 2);
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
                                spear.firstChunk.vel.x *= 1.7f;
                                Ebug("Spear Go!", 2);
                            }
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                            self.standing = false;
                        }
                    }
                    spear.spearDamageBonus = spearDmgBonuses[1];
                    thrust = 5f;
                }
            } else {
                spear.spearDamageBonus = 1.25f;
            }
            if (e.parryTech){
                if (e.parryExtras > 0){
                    spear.spearDamageBonus *= 3f;
                    spear.firstChunk.vel *= 1.5f;
                    e.parryExtras = 0;
                }
                else {
                    spear.spearDamageBonus *= 0.5f;
                    spear.firstChunk.vel *= 0.75f;
                }
            }

            if (onPole) {
                thrust = 1f;
            }

            if ((self.room != null && self.room.gravity == 0f) || Mathf.Abs(spear.firstChunk.vel.x) < 1f){
                self.firstChunk.vel += spear.firstChunk.vel.normalized * thrust;
            } else {
                self.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                BodyChunk firstChunker = self.firstChunk;
                if (self.animation != Player.AnimationIndex.BellySlide){
                    firstChunker.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                }
            }
            Ebug("Speartoss! Velocity [X,Y]: [" + spear.firstChunk.vel.x + "," + spear.firstChunk.vel.y + "] Damage: " + spear.spearDamageBonus, 2);
        }

        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
                if (obj == null){
                    return orig(self, obj);
                }
            } catch (Exception err){
                Ebug(err);
                return orig(self, obj);
            }
            if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                return Player.ObjectGrabability.TwoHands;
            }
            if (!dualWielding.TryGet(self, out bool dW) ||
                !eCon.TryGetValue(self, out Escort e)){
                return orig(self, obj);
            }

            if (dW){
                if (obj is Weapon){
                    // Any weapon is dual-wieldable, including spears
                    return Player.ObjectGrabability.OneHand;
                } else if (obj is Lizard lizzie){
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
        }

        private float Escort_DeathBiteMult(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            try{
                if (!eCon.TryGetValue(self, out Escort e)){
                    return orig(self);
                }
                if (self.slugcatStats.name.value == "EscortMe"){
                    float biteMult = e.combatTech? 0.5f : 0.15f;
                    return (Eshelp_ParryCondition(self)? 5f : biteMult);
                } else {
                    return orig(self);
                }
            } catch (Exception err){
                Ebug(err, "Couldn't set deathbitemultipler!");
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

                Ebug("Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0){
                    orig(self);
                    if (self.dead && Esconfig_SFX(self)){
                        self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug("Failure.", 1);
                } else {
                    self.dead = false;
                    Ebug("Player didn't die?", 1);
                    e.ParrySuccess = false;
                }
            } catch (Exception err){
                Ebug(err, "Something happened while trying to die!");
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
                }
                Ebug("Attempting to replace some spears with Spearmaster's needles!", 2);
                for (int i = 0; i < self.abstractRoom.entities.Count; i++){
                    if (self.abstractRoom.entities[i] != null && self.abstractRoom.entities[i] is AbstractSpear spear){
                        if (UnityEngine.Random.value > 0.75f && !spear.explosive && !spear.electric){
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
    }
}