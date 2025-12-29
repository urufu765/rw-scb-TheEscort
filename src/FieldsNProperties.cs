using System.Linq;
using BepInEx;
using BepInEx.Logging;
using Menu;
using SlugBase;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort;

partial class Plugin : BaseUnityPlugin
{
    static Plugin()
    {
        LR = BepInEx.Logging.Logger.Sources.FirstOrDefault(l => l.SourceName == "EBUGGER") as ManualLogSource ?? BepInEx.Logging.Logger.CreateLogSource("EBUGGER");
        EscortMe = new("EscortMe");
        EscortSocks = new("EscortSocks");
        EscortMeTime = new("EscortMe");
        EscortSocksTime = new("EscortSocks");

        eCon = new();
        sCon = new();
        natrualSpears = [];

        escPatch_revivify = false;
        escPatch_rotundness = false;
        escPatch_dms = false;

        AltSleepScene = GameExtEnum<MenuScene.SceneID>("alt_sleep_scene");
        AltSleepSceneDuo = GameExtEnum<MenuScene.SceneID>("alt_sleep_scene_together");
        pRTEdits = PlayerBool("playescort/realtime_edits");
        gRTEdits = GameBool("gameescort/realtime_edits");
        BtrPounce = PlayerBool("theescort/better_pounce");
        SupahMeanLizards = GameBool("theescort/mean_lizards");
        BodySlam = PlayerFloats("theescort/body_slam");
        SlideLaunchMod = PlayerFloats("theescort/slide_launch_mod");
        LiftHeavy = PlayerFloat("theescort/heavylifter");
        Exhausion = PlayerFloat("theescort/exhausion");
        DKM = PlayerFloat("theescort/dk_multiplier");
        ParrySlide = PlayerBool("theescort/parry_slide");
        Escomet = PlayerInt("theescort/headbutt");
        Elvator = PlayerBool("theescort/elevator");
        TrampOhLean = PlayerFloat("theescort/trampoline");
        HypeSys = PlayerBool("theescort/adrenaline_system");
        HypeReq = PlayerFloat("theescort/stamina_req");
        CR = PlayerInt("theescort/reset_rate");
        bonusSpear = PlayerFloats("theescort/spear_damage");
        dualWielding = PlayerBool("theescort/dual_wield");
        soundAhoy = PlayerBool("theescort/sounds_ahoy");
        NoMoreGutterWater = PlayerFloats("theescort/guuh_wuuh");
        LWallJump = PlayerBool("theescort/long_wall_jump");
        WallJumpVal = PlayerFloats("theescort/wall_jump_val");
        headDraw = PlayerFloats("theescort/headthing");
        bodyDraw = PlayerFloats("theescort/bodything");

        boxMove = GameFloats("boxbox");

        InstaHype = PlayerBool("theescort/insta_hype");
        BetterCrawl = PlayerFloats("theescort/better_crawl");
        BetterPoleWalk = PlayerFloats("theescort/better_polewalk");
        BetterPoleHang = PlayerFloats("theescort/better_polehang");
        RollCage = PlayerFloats("theescort/rollfly");

        barbarianDisallowOversizedLuggage = PlayerBool("theescort/barbarian/nooverlug");

        brawlerSlideLaunchFac = PlayerFloat("theescort/brawler/slide_launch_fac");
        brawlerDKHypeDmg = PlayerFloat("theescort/brawler/dk_h_dmg");
        brawlerSpearVelFac = PlayerFloats("theescort/brawler/spear_vel_fac");
        brawlerSpearDmgFac = PlayerFloats("theescort/brawler/spear_dmg_fac");
        brawlerSpearThrust = PlayerFloat("theescort/brawler/spear_thrust");
        brawlerSpearShankY = PlayerFloats("theescort/brawler/spear_shank");
        brawlerRockHeight = PlayerFloat("theescort/brawler/rock_height");

        deflectorSlideDmg = PlayerFloat("theescort/deflector/slide_dmg");
        deflectorSlideLaunchFac = PlayerFloat("theescort/deflector/slide_launch_fac");
        deflectorSlideLaunchMod = PlayerFloat("theescort/deflector/slide_launch_mod");
        deflectorDKHypeDmg = PlayerFloats("theescort/deflector/dk_h_dmg");
        deflectorSpearVelFac = PlayerFloats("theescort/deflector/spear_vel_fac");
        deflectorSpearDmgFac = PlayerFloats("theescort/deflector/spear_dmg_fac");

        escapistSlideLaunchMod = PlayerFloat("theescort/escapist/slide_launch_mod");
        escapistSlideLaunchFac = PlayerFloat("theescort/escapist/slide_launch_fac");
        escapistSpearVelFac = PlayerFloat("theescort/escapist/spear_vel_fac");
        escapistSpearDmgFac = PlayerFloat("theescort/escapist/spear_dmg_fac");
        escapistNoGrab = PlayerInts("theescort/escapist/no_grab");
        escapistCD = PlayerInt("theescort/escapist/cd");
        escapistColor = PlayerFloat("theescort/escapist/color");

        gilded_float = PlayerFloat("theescort/gilded/float_speed");
        gilded_lev = PlayerFloat("theescort/gilded/levitation");
        gilded_jet = PlayerFloat("theescort/gilded/jetplane");
        gilded_radius = PlayerFloat("theescort/gilded/pipradius");
        gilded_position = PlayerFloats("theescort/gilded/pipposition");

        railgunSpearVelFac = PlayerFloats("theescort/railgunner/spear_vel_fac");
        railgunSpearDmgFac = PlayerFloats("theescort/railgunner/spear_dmg_fac");
        railgunSpearThrust = PlayerFloats("theescort/railgunner/spear_thrust");
        railgunRockVelFac = PlayerFloat("theescort/railgunner/rock_vel_fac");
        railgunLillyVelFac = PlayerFloat("theescort/railgunner/lilly_vel_fac");
        railgunBombVelFac = PlayerFloat("theescort/railgunner/bomb_vel_fac");
        railgunRockThrust = PlayerFloats("theescort/railgunner/rock_thrust");
        railgunRecoil = PlayerFloat("theescort/railgunner/recoil_fac");
        railgunRecoilMod = PlayerFloats("theescort/railgunner/recoil_mod");
        railgunRecoilDelay = PlayerInt("theescort/railgunner/recoil_delay");

        CustomShader = PlayerString("theescort/speedster/custom_shader");
        speedsterPolewow = PlayerFloats("theescort/speedster/pole_rise");

        selectionable = new() {
            {  0, "Guardian" },
            { -1, "Brawler" },
            { -2, "Deflector" },
            { -3, "Escapist" },
            { -4, "Railgunner" },
            { -5, "Speedster" },
            { -6, "Gilded" }
        };
        escortRGBTick = new float[4];
        escortRGBStore = new Color[4];
        Ebug("-> Static fields load complete");
    }
}

