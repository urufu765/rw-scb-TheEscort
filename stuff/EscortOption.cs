using System;
using System.Collections.Generic;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;


namespace SlugTemplate{
    public class EscortOption{
        private const string MOD_ID = "urufudoggo.theescort";

        public static PlayerFeature<bool> BetterPounce = PlayerBool("theescort/better_pounce");
        public static GameFeature<bool> SuperMeanLizards = GameBool("theescort/mean_lizards");
        public static GameFeature<bool> SuperMeanGarbageWorms = GameBool("theescort/mean_garb_worms");
        public static PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");
        public static PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");
        public static PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static PlayerFeature<float> CarryHeavy = PlayerFloat("theescort/heavylifter");
        public static PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static PlayerFeature<float> DropKick = PlayerFloat("theescort/dk_multiplier");
        public static PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static PlayerFeature<bool> Elevator = PlayerBool("theescort/elevator");
        public static PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");
        public static PlayerFeature<bool> Hyped = PlayerBool("theescort/adrenaline_system");
        public static PlayerFeature<float> StaReq = PlayerFloat("theescort/stamina_req");
        public static PlayerFeature<int> RR = PlayerInt("theescort/reset_rate");
        public static PlayerFeature<bool> ParryTest = PlayerBool("theescort/parry_test");
        public static PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");
        public static PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");
        public static PlayerFeature<bool> soundsAhoy = PlayerBool("theescort/sounds_ahoy");

        public static void OnInit(){
            OptionInterface optionInterface = new EscortOptionInterface();

        }
    }
}