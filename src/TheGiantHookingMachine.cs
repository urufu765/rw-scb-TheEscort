using BepInEx;
using System;
using static TheEscort.Eshelp;

namespace TheEscort;

partial class Plugin : BaseUnityPlugin
{
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
            IL.Player.EatMeatUpdate += Consumption.Escort_EatMeatFaster;
            IL.Player.GrabUpdate += Esclass_CQ_DontRegurgutato;
            // IL.FirecrackerPlant.Explode;
        }
        catch (Exception err)
        {
            Ebug(err, "Oh dear.");
        }
    }
}
