using BepInEx;
using MonoMod.Cil;
using Newtonsoft.Json;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[Beta] The Escort", "0.2.12.1")]
    partial class Plugin : BaseUnityPlugin
    {
        public static Plugin ins;
        public EscOptions config;
        public const string MOD_ID = "urufudoggo.theescort";

        /*
        public Plugin()
        {
            Debug.Log("-> Escort Plugin Init!");
            try
            {
                Plugin.ins = this;
            }
            catch (Exception e)
            {
                base.Logger.LogError(e);
            }
        }*/

#region Declare Features
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
        public static readonly PlayerFeature<float[]> headDraw = PlayerFloats("theescort/headthing");
        public static readonly PlayerFeature<float[]> bodyDraw = PlayerFloats("theescort/bodything");

        public static readonly GameFeature<bool> replaceGrapple = GameBool("thesocks/replacegrapple");
        #endregion

#region Plugin Variable Declarations
        public static readonly SlugcatStats.Name EscortMe = new("EscortMe");
        public static readonly SlugcatStats.Name EscortSocks = new("EscortSocks");
        //public static readonly SlugcatStats.Name ShadowEscort = new("EscortDummy", true);


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
        public static SoundID Escort_SFX_Uhoh_Big;
        public static SoundID Esconfig_SFX_Sectret;
        public static SoundID Escort_SFX_Placeholder;
        public static SoundID Escort_SFX_Gild_Stomp;
        //public static SoundID Escort_SFX_Spawn;

        //public DynamicSoundLoop escortRollin;

        // Miscellanious things
        private readonly bool nonArena = false;  // Sets Escort's marking colors to the main color instead of Arena
        //public static readonly String EscName = "EscortMe";

        // Escort instance stuff
        public static ConditionalWeakTable<Player, Escort> eCon = new();
        public static ConditionalWeakTable<Player, Socks> sCon = new();
        //public static ConditionalWeakTable<AbstractCreature, AbstractEscort> aCon = new();
        //private Escort e;
        public float hypeRequirement;
        private float DKMultiplier;
        float ratioed;
        public static bool templeGuardIsFriendly;


        // Patches
        public static bool escPatch_revivify = false;
        public static bool escPatch_rotundness = false;
        public static bool escPatch_dms = false;
        public static bool escPatch_guardian = false;
        //private static bool escPatch_DMS = false;
        //private bool escPatch_emeraldTweaks = false;
#endregion


        // Debug Logger (Beautified!)

        // Add hooks
        public void OnEnable()
        {
            Debug.Log("-> Escort plugin INIT!");
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

            //On.RainWorldGame.ctor += EscortChangingRoom;

            On.SaveState.setDenPosition += Escort_ChangingRoom;
            //On.SaveState.SessionEnded += Escort_RespawnPup;
            
            //On.SaveState.GetStoryDenPosition += 

            //IL.AbstractCreature.Realize += Backpack_ILRealize;
            //On.AbstractCreature.Realize += Backpack_Realize;
            //On.StaticWorld.InitCustomTemplates += Custom_Stuff;

            On.Lizard.ctor += Escort_Lizard_ctor;
            //On.LizardAI.GiftRecieved += Escort_Lizard_Denial;

            On.Room.Loaded += Escort_Hipbone_Replacement;
            On.RoomSettings.Load += Escort_Transplant;

            On.PlayerGraphics.InitiateSprites += Escort_InitiateSprites;
            On.PlayerGraphics.ApplyPalette += Escort_ApplyPalette;
            On.PlayerGraphics.AddToContainer += Escort_AddGFXContainer;
            On.PlayerGraphics.DrawSprites += Escort_DrawSprites;
            On.PlayerGraphics.Update += Escort_GFXUpdate;
            On.PlayerGraphics.DefaultSlugcatColor += Escort_Colorz;
            //On.HUD.HudPart.Draw += Escort_HUD_Draw;
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
            //On.RoomCamera.DrawUpdate += Escort_AdditionalDraw;
            //On.Player.GrabUpdate += Esclass_RG_GrabUpdate;


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

            //On.MoreSlugcats.LillyPuck.HitSomething += Escort_LillyHit;
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
            //On.SlugcatStats.getSlugcatTimelineOrder += Escort_Time;

            //On.PlayerGraphics.PopulateJollyColorArray += ReJollyCoop.PopulateTheJollyMan;
            //,
            //{"name": "Glow", "story": "ffffff"}

            //On.Player.Update += Estest_1_Update;
            //On.Player.GrabUpdate += Estest_3_GrabUpdate;
            Escort_Conversation.Attach();
            EscortRoomScript.Attach();
            EscortHUD.Attach();
            On.TubeWorm.GrabbedByPlayer += GrappleBackpack.BackpackGrabbedByPlayer;
            On.TubeWorm.GrabbedByPlayer += LauncherBackpack.BackpackGrabbedByPlayer;

            On.TubeWorm.Update += Socks_Sticky_Immune;
            //ReJollyCoop.Hooker();

            On.MoreSlugcats.HRGuardManager.Update += Esclass_GD_KillGuardianWithOneHit;

            On.TempleGuardAI.ThrowOutScore += Escort_Friendship;

            On.ShelterDoor.Close += StoreWinConditionData;

            On.PlayerSessionRecord.AddKill += Esclass_DF_DamageIncrease;

            On.SaveState.SessionEnded += Escort_Reset_Values;

            //On.SaveState.SessionEnded += StoreSaveDataOnFinish;
        }

        private void Escort_RespawnPup(On.SaveState.orig_SessionEnded orig, SaveState self, RainWorldGame game, bool survived, bool newMalnourished)
        {
            if (survived && self.miscWorldSaveData.Esave().RespawnPupReady)
            {
                self.miscWorldSaveData.Esave().RespawnPupReady = false;
                self.deathPersistentSaveData.reinforcedKarma = false;
            }
            orig(self, game, survived, newMalnourished);
        }

        private float Escort_Friendship(On.TempleGuardAI.orig_ThrowOutScore orig, TempleGuardAI self, Tracker.CreatureRepresentation crit)
        {
            if (templeGuardIsFriendly)
            {
                return 0f;
            }
            return orig(self, crit);
        }

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
        private void LoadResources(RainWorld rainWorld)
        {
            ins.L().Set();
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
            FAtlas aB, aH, hA, hB;
            aB = Futile.atlasManager.LoadAtlas("atlases/escorthip");
            aH = Futile.atlasManager.LoadAtlas("atlases/escorthead");
            hA = Futile.atlasManager.LoadAtlas("atlases/escorthuda");
            hB = Futile.atlasManager.LoadAtlas("atlases/escorthudb");
            if (aB == null || aH == null || hA == null || hB == null)
            {
                Ebug("Oh no. Sprites dead.", 0);
            }
            //Escort_SFX_Spawn = new SoundID("Escort_Spawn", true);
            Ebug("All SFX loaded!", 1);
            EscEnums.RegisterValues();  // TODO: do something with this
            this.config = new EscOptions(rainWorld);
            MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            ins.L().Christmas(config.cfgSectret.Value);
            Ebug("All loaded!", 1);
        }

#region Mod Patches
        private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            ins.L().Set();
            orig(self);

            // Look for mods...
            try
            {
                if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify"))
                {
                    ins.L().Set("Patch: Revivify");
                    Ebug("Found Revivify! Applying patch...", 1);
                    escPatch_revivify = true;
                }
                if (ModManager.ActiveMods.Exists(mod => mod.id == "dressmyslugcat"))
                {
                    ins.L().Set("Patch: DressMySlugcat");
                    Ebug("Found Dress My Slugcat!", 1);
                    ModManager.Mod DMS_Mod = ModManager.ActiveMods.Find(mod => mod.id == "dressmyslugcat");
                    //escPatch_DMS = true;
                    Ebug("Found DMS Version: " + DMS_Mod.version, 1);
                    string[] dmsVer = DMS_Mod.version.Split('.');
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
                if (ModManager.ActiveMods.Exists(mod => mod.id == "willowwisp.bellyplus"))
                {
                    ins.L().Set("Patch: Rotund World");
                    Ebug("Found Rotund World! Applying custom patch...", 1);
                    escPatch_rotundness = true;
                }
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

        private static void Espatch_DMS(int verMaj, int verMin)
        {
            try
            {// Dress My Slugcat Patch
                //if (dms.version)

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
                else
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
                //escPatch_DMS = false;
                Ebug(merr, "Couldn't patch Dress Me Sluggie because...");
            }
        }

        // Legacy
        private static void Espatch_DMS()
        {
            try
            {// Dress My Slugcat Patch
                //if (dms.version)
                Ebug("Using dud patch...", 1);
                DressMySlugcat.SpriteDefinitions.AvailableSprites.Add(new DressMySlugcat.SpriteDefinitions.AvailableSprite
                {
                    //Name = "UPDATEYOURDMS!",
                    //Description = "Update Your DMS",
                    Name = "MARKINGS",
                    Description = "Markings",
                    GallerySprite = "escortHipT",
                    RequiredSprites = new List<string> { "escortHeadT", "escortHipT" },
                    Slugcats = new List<string> { "EscortMe" }
                });
            }
            catch (Exception merr)
            {
                //escPatch_DMS = false;
                Ebug(merr, "Couldn't patch Dress Me Sluggie because...");
            }
        }

        //Ebug(self, "Using dud patch... (update your DMS!)", 1);
#endregion


#region Escort Legacy Configurations
        // TODO: Replace or make it better by using out keyword
        /*
        Configurations!
        */
        private bool Esconfig_Vengeful_Lizards()
        {
            return config.cfgVengefulLizards.Value;
        }

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

        private bool Esconfig_Dunkin()
        {
            return config.cfgDunkin.Value;
        }

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

        private bool Esconfig_Build(Player self)
        {
            try
            {
                if (!eCon.TryGetValue(self, out Escort e))
                {
                    return false;
                }
                //int pal = 0;
                //bool help = false;
                /*
                (int pal, bool help) = self.playerState.playerNumber switch {
                    0 => (config.cfgBuildP1.Value, config.cfgEasyP1.Value),
                    1 => (config.cfgBuildP2.Value, config.cfgEasyP2.Value),
                    2 => (config.cfgBuildP3.Value, config.cfgEasyP3.Value),
                    _ => (config.cfgBuildP4.Value, config.cfgEasyP4.Value)
                };
                */
                int pal = config.cfgBuild[self.playerState.playerNumber].Value;

                // Story expansion skips
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
                // Fix this by turning it off for expedition or add multiplier or somethingidk

                //int maximumPips = self.slugcatStats.maxFood;
                //int minimumPips = self.slugcatStats.foodToHibernate;
                switch (pal)
                {
                    // Unstable build (Longer you're in battlehype, the more the explosion does. Trigger explosion on a dropkick)
                    // Stylist build (Do combos that build up to a super move)
                    // Super build (Pressing throw while there's nothing in main hand will send a grapple tongue, which if it latches onto creature, pulls Escort to eavy creatures, and light creatures to Escort. Throwing while having a rock in main hand will do melee/parry, having bomb in main hand will melee/knockback. Sliding also is fast and feet first. While midair, pressing down+jump will stomp)
                    // Stealth build (hold still or crouch to enter stealthed mode)
                    case -8:  // Testing build
                        e.EsTest = true;
                        break;
                    case -7:  // New Escapist build (Testing, obsolete)
                        e.NewEscapist = true;
                        Ebug(self, "New Escapist Build selected!", 2);
                        self.slugcatStats.visualStealthInSneakMode = 1;
                        self.spearOnBack = new Player.SpearOnBack(self);
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
                        //maximumPips -= 4;
                        //minimumPips -= 3;
                        Ebug(self, "Gilded Build selected!", 2);
                        break;
                    case -5:  // Speedstar build
                        e.Speedster = true;
                        e.SpeOldSpeed = config.cfgOldSpeedster.Value;
                        if (!e.SpeOldSpeed && self.room?.game?.session is StoryGameSession speedsterSession)
                        {
                            Ebug(self, "Get Speedster save!");
                            if (speedsterSession.saveState.miscWorldSaveData.Esave().SpeChargeStore.TryGetValue(self.playerState.playerNumber, out int charging))
                            {
                                e.SpeCharge = charging;
                            }
                            //Ebug(self, "Misc: " + JsonConvert.SerializeObject(speedsterSession.saveState.miscWorldSaveData.Esave()));
                        }
                        self.slugcatStats.lungsFac += 0.3f;
                        self.slugcatStats.bodyWeightFac += 0.1f;
                        //self.slugcatStats.corridorClimbSpeedFac += 1.0f;
                        self.slugcatStats.poleClimbSpeedFac += 0.6f;
                        self.slugcatStats.corridorClimbSpeedFac += 0.8f;
                        self.slugcatStats.runspeedFac += 0.35f;
                        // Frictions do nothing lol
                        self.airFriction -= 0.5f;
                        self.waterFriction -= 0.5f;
                        self.surfaceFriction -= 0.5f;
                        //minimumPips += 1;
                        Ebug(self, "Speedstar Build selected!", 2);
                        break;
                    case -4:  // Railgunner build
                        e.Railgunner = true;
                        e.acidSwim = 0.3f;
                        self.slugcatStats.lungsFac += 0.7f;
                        self.slugcatStats.throwingSkill = 2;
                        self.slugcatStats.loudnessFac += 2f;
                        self.slugcatStats.generalVisibilityBonus += 1f;
                        self.slugcatStats.visualStealthInSneakMode = 0f;
                        self.slugcatStats.bodyWeightFac += 0.3f;
                        //minimumPips -= 2;
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
                        //minimumPips -= 3;
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
                        //minimumPips -= 1;
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
                        //minimumPips += 4;
                        Ebug(self, "Brawler Build selected!", 2);
                        break;
                    default:  // Default build
                        Ebug(self, "Default Build selected!", 2);
                        e.isDefault = true;
                        self.slugcatStats.lungsFac -= 0.2f;
                        break;
                }
                e.easyMode = config.cfgEasy[self.playerState.playerNumber].Value;
                if (e.easyMode)
                {
                    Ebug(self, "Easy Mode active!");
                }
                self.slugcatStats.lungsFac += self.Malnourished ? 0.15f : 0f;
                self.buoyancy -= 0.05f;
                if (config.cfgDeflecterSharedPool.Value)
                {
                    e.DeflPerma = DeflInitSharedPerma;
                }

#if false
                if (self.room?.game?.StoryCharacter == EscortMe){
                    Ebug(self, "Session is Escort!", 1);
                    self.slugcatStats.maxFood = maximumPips;
                    if (!self.Malnourished){
                        self.slugcatStats.foodToHibernate = minimumPips;
                    }
                }
#endif
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

        // Implement lizard aggression (edited from template)
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
            ins.L().Set();
            Ebug("Ctor Triggered!");
            orig(self, abstractCreature, world);
            if (Eshelp_IsMe(self.slugcatStats.name, false))
            {
                ins.L().Set("Escort Check");
                eCon.Add(self, new Escort(self));
                if (!eCon.TryGetValue(self, out Escort e))
                {
                    Ebug(self, "Something happened while initializing then accessing Escort instance!", 0);
                    return;
                }
                if (world?.game?.session is ArenaGameSession) e.escortArena = true;
                Esconfig_Build(self);
                e.Escat_Add_Ring_Trackers(self);
                e.originalMass = 0.7f * self.slugcatStats.bodyWeightFac;
                logImportance = config.cfgLogImportance.Value;
                try
                {
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
                    //Ebug(new NullReferenceException(), "Test");
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
            if (self.slugcatStats.name == EscortSocks)
            {
                ins.L().Set("Socks Check");
                sCon.Add(self, new Socks(self));
                if (!sCon.TryGetValue(self, out Socks es))
                {
                    Ebug(self, "Something happened while initializing then accessing Socks instance!", 0);
                    return;
                }
                Socks_ctor(self);
                es.SockWorld = world;
                try
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
                            escort.Escat_Update_Ring_Trackers();
                            if (escort.Speedster)
                            {
                                escort.Escat_showTrail();
                            }
                            if (escort.NewEscapist)
                            {
                                //escort.Escat_NE_ShowTrail();
                                //Esclass_NE_AbsoluteTick(player, ref escort);
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

        private void Escort_AdditionalDraw(On.RoomCamera.orig_DrawUpdate orig, RoomCamera self, float timeStacker, float timeSpeed)
        {
            orig(self, timeStacker, timeSpeed);
            try
            {
                foreach (AbstractCreature abstractCreature in self.game.Players)
                {
                    if (abstractCreature.realizedCreature is Player player && eCon.TryGetValue(player, out Escort escort))
                    {
                        //e.Escat_showTrail();
                        //foreach(var speedTrail in escort.SpeTrail2)
                        //{
                            //speedTrail.Draw(self, timeStacker, self.CamPos);
                        //}
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Nulled when trying to update graphics for miscellanious stuff");
            }
            catch (ArgumentNullException ane)
            {
                Ebug(ane, "Argument nulled when trying to find an escort in a CWT");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when trying to update graphics for miscellanious stuff");
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
                return config.cfgBuild[0].Value switch{
                    -6 => config.cfgSectretBuild.Value? new(10, 6) : new(14, 8),  // Gilded
                    -5 => new(14, 10),  // Speedster
                    -4 => new(14, 7),  // Railgunner
                    -3 => config.cfgOldEscapist.Value? new(11, 7) : new(10, 9),  // Escapist (TODO: Don't forget to flip this!)
                    -2 => new(14, 8),  // Deflector
                    -1 => new(14, 12),  // Brawler
                    _  => new(14, 9)  // Default and unspecified.
                };

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
            orig(self, game, survived, newMalnourished);
            DeflSharedPerma = 0;
            try
            {
                Ebug($"Savestate SEA: {JsonConvert.SerializeObject(self.miscWorldSaveData.Esave())}", 1, true);
            }
            catch (Exception err)
            {
                Ebug(err, "Error on trying to print savestate after session ended");
            }
        }



#region Escort Has A Public Github Repo You Know
        protected void Escort_Has_A_Public_Github_Repo_Lol(Player self)
        {
            Purgitory:
            try
            {
                Escort_Has_A_Public_Github_Repo_Lol(self);
                goto Purgitory;
            }
            catch (Exception err)
            {
                Ebug(self, err);
                RedundantlyTrue(true);
                RedundantlyFalse(false);
            }
            finally
            {
                Ebug(What_Will_This_Do());
                이건_아무겄도_안함();
            }
            throw new NotImplementedException();
        }

        protected bool RedundantlyTrue(bool? thing = true)
        {
            if (thing == null)
            {
                return true;
            }

            thing = true;
            bool? b = true;
            if ((bool)thing)
            {
                thing = true;
                b = thing.ToString() == "True" || (true.ToString() == "True");
            }
            else if (true)
            {
                thing = true;
            }
            if (b == thing)
            {
                thing = b;
                b = thing;
            }
            if ((bool)thing && (bool)b)
            {
                switch (true)
                {
                    case false:
                    case var value when value == false:
                    case true:
                        break;
                    default:
                }
                if (1 == 0)
                {
                }
                return true;
            }
            return true;
        }

        protected bool RedundantlyFalse(bool thing = true)
        {
            if (thing)
            {
                thing = false;
            }
            else if (thing)
            {
                thing = false;
            }
            if (!true) { }
            while (thing)
            {
                for (int a = 0; a < 1000; a++)
                {
                    for (int b = 0; a < 10000; b++)
                    {
                        bool?[] secretThing = new bool?[] { true, false, false, false, false, false, false, false };
                        for (int c = 0; b < 100000; c++)
                        {
                            for (int d = 0; c < 1000000; d++)
                            {
                                for (int e = 0; d < 0; a++)
                                {
                                    if (e == 0)
                                    {
                                        foreach (bool f in secretThing.Select(v => (bool)v))
                                        {
                                            if (f)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool? someThing = false;
            bool anotherThing = someThing switch
            {
                true => false,
                false => false,
                _ => false
            };
            if (true)
            {
                if (false || (false && true) || false || (true && true))
                {
                    anotherThing = false;
                    someThing = anotherThing;
                    someThing = false;
                }
            }
            return false;
        }

        protected void 이건_아무겄도_안함()
        {
            bool 모함 = false;
            Ebug(모함);
        }


        protected int What_Will_This_Do(bool? startHere = true, int w = 0, int r = 0)
        {
            if (r >= 123)
            {
                return r;
            }
            int x, y, z = w;
            profoundLeap:
            x = y = z;
            notsoProfoundLeap:
            y++;
            profoundlyNotLeap:
            if (startHere is null)
            {
                if (w >= 777)
                {
                    return 0;
                }
            }
            else if ((bool)startHere)
            {
                goto one;
            }
            else if (startHere is bool)
            {
                What_Will_This_Do(true, r: r + 1);
            }
            else
            {
                goto abandonment;
            }
            one:
            bool[] g = new bool[] { false, true, true, false, true, false, false, true, false, true, true };
            foreach (bool f in g)
            {
                if (x >= g.Length)
                {
                    w = UnityEngine.Random.Range(0, g.Length);
                    startHere = false;
                }
                if (f)
                {
                    goto helloThere;
                }
                x++;
                if ((startHere.Value && g.Reverse().ToArray()[x] is true) || g[w])
                {
                    goto somethingIndeed;
                }
            }
            helloThere:
            if (z == 0)
            {
                z++;
                goto profoundLeap;
            }
            else
            {
                z = y;
            }
            if (y < 10)
            {
                x += y;
                goto notsoProfoundLeap;
            }
            somethingIndeed:
            switch (x) 
            {
                case > 1984:
                    return y;
                case > 100:
                    goto sanctuary;
                case 69:
                    y += z;
                    break;
                case > 10:
                    y = 0;
                    goto helloThere;
                case > 0:
                    z = x * y;
                    goto case 69;
                default:
                    goto one;
            }
            goto profoundlyNotLeap;
            abandonment:
            return What_Will_This_Do(null, x * y * -z, r + 2);
            sanctuary:
            return What_Will_This_Do(startHere, z, r + 3);
        }
#endregion


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
                if (e.Deflector && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll))
                {
                    Ebug(player, "Parryteched condition!", 2);
                    return true;
                }
                else if (player.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0)
                {
                    Ebug(player, "Regular parry condition!", 2);
                    return true;
                }
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
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return false;
            }
            if (e.Deflector && (self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.Roll))
            {
                Ebug(self, "Parryteched condition!", 2);
                return true;
            }
            if (e.NewEscapist && e.NEsAbility > 0 && (self.animation == Player.AnimationIndex.Flip))
            {
                Ebug(self, "New Escapist trickz parry condition!", 2);
                return true;
            }
            else if (self.animation == Player.AnimationIndex.BellySlide && e.parryAirLean > 0)
            {
                Ebug(self, "Regular parry condition!", 2);
                return true;
            }
            else
            {
                Ebug(self, "Not in parry condition", 2);
                Ebug(self, "Parry leniency: " + e.parryAirLean);
                return e.parrySlideLean > 0;
            }
        }

        // Secondary parry condition when dropkicking to save Escort from accidental death while trying to kick creatures
        public bool Eshelp_SavingThrow(Player self, BodyChunk offender, Creature.DamageType ouchie)
        {
            if (!eCon.TryGetValue(self, out Escort e))
            {
                Ebug(self, "Saving throw failed because Scug is not Escort!", 0);
                return false;
            }
            if (self is null || offender is null || ouchie is null)
            {
                Ebug(self, "Saving throw failed due to null values!", 0);
                return false;
            }
            if (offender.owner is not Creature)
            {
                Ebug(self, "Saving throw failed due to the offender not being a creature!", 2);
                return false;
            }
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
            else
            {
                Ebug(self, "Saving throw failed: Deflector Build Moment.", 2);
            }
            return false;
        }


        private void Backpack_ILRealize(ILContext il)
        {
            var cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After,
                i => i.MatchLdarg(0),
                i => i.MatchLdarg(0),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<AbstractWorldEntity>("world"),
                i => i.MatchNewobj<TubeWorm>(),
                i => i.MatchCall<AbstractCreature>("set_realizedCreature")
            ))
            {

            }
            cursor.EmitDelegate<Action<CreatureTemplate>>(
                (cb) =>
                {
                    if (cb.type == GrappleBackpack.GrapplingPack)
                    {
                        throw new NotImplementedException();
                    }
                }
            );
        }

        private void Backpack_Realize(On.AbstractCreature.orig_Realize orig, AbstractCreature self)
        {
            orig(self);
            try
            {
                if (self?.world?.game is null || (replaceGrapple is not null && replaceGrapple.TryGet(self.world.game, out bool rG) && !rG))
                {
                    return;
                }
                if (self.Room is not null && self.Room.shelter && self.realizedCreature is not null && self.realizedCreature is TubeWorm && self.realizedCreature is not GrappleBackpack)
                {
                    Ebug("Replaced Grapple with Backpack!");
                    self.realizedCreature.Destroy();
                    self.realizedCreature = new GrappleBackpack(self, self.world);
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Something happened while replacing Tubeworm with GrappleBackpack!");
            }
        }

        private void Custom_Stuff(On.StaticWorld.orig_InitCustomTemplates orig)
        {
            orig();
        }


        private void EscortChangingRoom(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            orig(self, manager);
            Ebug("Changing room!");
            Ebug(self.startingRoom);
            if (self.StoryCharacter == EscortMe){
                self.startingRoom = config.cfgBuild[0].Value switch {
                    0 => "CC_SUMP02", // Default
                    -1 => "SU_A02",  // Brawler
                    -2 => "SI_C03",  // Deflector
                    -3 => config.cfgOldEscapist.Value? "DM_LEG02" : "SB_B04",  // Escapist
                    -4 => "GW_C02_PAST",  // Railgunner
                    -5 => "LF_E03",  // Speedster
                    -6 => config.cfgSectretBuild.Value? "HR_C01" : "CC_A10",  // Gilded
                    _ => "SB_C09"  // Unspecified
                };
                Ebug("It's time UwU");
                Ebug(self.startingRoom);
            }
        }

        private void Escort_ChangingRoom(On.SaveState.orig_setDenPosition orig, SaveState self){
            orig(self);
            Ebug("Changing room 2!");
            Ebug(self.denPosition);
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
                    -2 => "SI_C03",  // Deflector
                    -3 => config.cfgOldEscapist.Value? "DM_LEG02" : "SB_B04",  // Escapist
                    -4 => "GW_C02_PAST",  // Railgunner
                    -5 => "LF_E03",  // Speedster
                    -6 => config.cfgSectretBuild.Value? "HR_C01" : "CC_A10",  // Gilded
                    _ => "SB_C09"  // Unspecified
                };
                Ebug("It's time OwO");
                Ebug(self.denPosition);
            }
        }


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
                    // TODO: Find a way to check if Escort has been beaten or not
                    return !UnplayableSocks;
                    // return rainWorld.progression.miscProgressionData.beaten_SpearMaster;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Something happened when setting whether slugcat is playable or not!");
            }
            return orig(i, rainWorld);
        }

        private static SlugcatStats.Name[] Escort_Time(On.SlugcatStats.orig_getSlugcatTimelineOrder orig)
        {
            ins.L().Set();
            SlugcatStats.Name[] timeline = orig();
            try
            {
                SlugcatStats.Name[] newTimeline = new SlugcatStats.Name[timeline.Length + 1];
                int j = 0;
                for (int i = 0; i < timeline.Length; i++)
                {
                    if (timeline[i] == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear)
                    {
                        ins.L().Set("Escort Check");
                        newTimeline[j] = timeline[i];
                        j++;
                        newTimeline[j] = EscortMe;
                    }
                    else if (timeline[i] == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                    {
                        ins.L().Set("Socks Check");
                        newTimeline[j] = EscortSocks;
                        j++;
                        newTimeline[j] = timeline[i];
                    }
                    else
                    {
                        newTimeline[j] = timeline[i];
                    }
                    j++;
                }
                return newTimeline;
            }
            catch (Exception err)
            {
                Ebug(err, "Couldn't set timeline!");
            }
            return timeline;
        }

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
                if (self.abstractRoom.shelter)
                {
                    Ebug("Spear swap ignores shelters!", 1);
                    return;
                }
                ins.L().SetF("Is not shelter");
                Ebug("Attempting to replace some spears with Spearmaster's needles!", 2);
                int j = 0;
                for (int i = 0; i < self.abstractRoom.entities.Count; i++)
                {
                    if (self.abstractRoom.entities[i] != null && self.abstractRoom.entities[i] is AbstractSpear spear)
                    {
                        if (UnityEngine.Random.value > 0.8f && !spear.explosive && !spear.electric)
                        {
                            self.abstractRoom.entities[i] = new AbstractSpear(spear.world, null, spear.pos, spear.ID, false)
                            {
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
            }
            catch (Exception err)
            {
                Ebug(err, "Something happened while swapping spears!");
            }
        }

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

    }
}