using System;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using UnityEngine;


namespace TheEscort{
    class EscOptions : OptionInterface{


        //public readonly Plugin instance;
        public readonly RainWorld rainworld;
        public Configurable<bool> cfgMeanLizards;
        // public Configurable<bool> cfgMeanGarbWorms;
        public Configurable<float> cfgHeavyLift;
        public Configurable<float> cfgDKMult;
        public Configurable<bool> cfgElevator;
        public Configurable<bool> cfgHypable;
        public Configurable<int> cfgHypeReq;
        public Configurable<bool> cfgSFX;
        public Configurable<bool> cfgPounce;
        public Configurable<bool> cfgLongWallJump;
        public Configurable<int> cfgBuildNum;
        public Configurable<int> cfgBuildP1;
        public Configurable<int> cfgBuildP2;
        public Configurable<int> cfgBuildP3;
        public Configurable<int> cfgBuildP4;
        public Configurable<bool> cfgEasyP1;
        public Configurable<bool> cfgEasyP2;
        public Configurable<bool> cfgEasyP3;
        public Configurable<bool> cfgEasyP4;
        public Configurable<bool> cfgDunkin;
        public Configurable<bool> cfgSpears;
        public Configurable<bool> cfgDKAnimation;
        private UIelement[] mainSet;
        private UIelement[] buildSet;
        private UIelement[] gimmickSet;
        private readonly float yoffset = 560f;
        private readonly float xoffset = 30f;
        private readonly float ypadding = 40f;
        private readonly float xpadding = 35f;
        private readonly float tpadding = 6f;
        private readonly int buildDiv = -4;
    
        public EscOptions(RainWorld rainworld){
            this.rainworld = rainworld;
            this.cfgMeanLizards = this.config.Bind<bool>("cfg_Mean_Lizards", false);
            this.cfgHeavyLift = this.config.Bind<float>("cfg_Heavy_Lift", 3f, new ConfigAcceptableRange<float>(0.01f, 1000f));
            this.cfgDKMult = this.config.Bind<float>("cfg_Drop_Kick_Multiplier", 3f, new ConfigAcceptableRange<float>(0.01f, 765f));
            this.cfgElevator = this.config.Bind<bool>("cfg_Elevator", false);
            this.cfgHypable = this.config.Bind<bool>("cfg_Hypable", true);
            this.cfgHypeReq = this.config.Bind<int>("cfg_Hype_Requirement", 3, new ConfigAcceptableRange<int>(0, 6));
            this.cfgSFX = this.config.Bind<bool>("cfg_SFX", false);
            this.cfgPounce = this.config.Bind<bool>("cfg_Pounce", true);
            this.cfgLongWallJump = this.config.Bind<bool>("cfg_Long_Wall_Jump", false);
            this.cfgDKAnimation = this.config.Bind<bool>("cfg_Drop_Kick_Animation", true);
            this.cfgBuildNum = this.config.Bind<int>("cfg_Build", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP1 = this.config.Bind<int>("cfg_Build_P1", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP2 = this.config.Bind<int>("cfg_Build_P2", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP3 = this.config.Bind<int>("cfg_Build_P3", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP4 = this.config.Bind<int>("cfg_Build_P4", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgEasyP1 = this.config.Bind<bool>("cfg_Easy_P1", false);
            this.cfgEasyP2 = this.config.Bind<bool>("cfg_Easy_P2", false);
            this.cfgEasyP3 = this.config.Bind<bool>("cfg_Easy_P3", false);
            this.cfgEasyP4 = this.config.Bind<bool>("cfg_Easy_P4", false);
            this.cfgDunkin = this.config.Bind<bool>("cfg_Dunkin_Lizards", true);
            this.cfgSpears = this.config.Bind<bool>("cfg_Super_Spear", true);
        }

        private static string swapper(string text, string with=""){
            text = text.Replace("<LINE>", System.Environment.NewLine);
            text = text.Replace("<REPLACE>", with);
            return text;
        }

        public override void Initialize()
        {
            float xo = this.xoffset;
            float yo = this.yoffset;
            float xp = this.xpadding;
            float yp = this.ypadding;
            float tp = this.tpadding;

            Color tempColor = new Color(0.5f, 0.5f, 0.55f);
            Color descColor = new Color(0.53f, 0.48f, 0.59f);
            Color easyColor = new Color(0.42f, 0.75f, 0.5f);

            Color p1Color = new Color(1f, 1f, 1f);
            Color p2Color = new Color(1f, 1f, 23f / 51f);
            Color p3Color = new Color(1f, 23f / 51f, 23f / 51f);
            Color p4Color = RWCustom.Custom.HSL2RGB(0.63055557f, 0.54f, 0.2f);

            Color bShadow = new Color(0.1f, 0.1f, 0.1f);
            Color bDefault = new Color(0.75f, 0.75f, 0.75f);
            Color bBrawler = new Color(0.8f, 0.6f, 0.6f);
            Color bDeflector = new Color(0.69f, 0.55f, 0.9f);
            Color bEscapist = new Color(0.8f, 0.8f, 0.5f);
            Color bRailgunner = new Color(0.5f, 0.85f, 0.78f);

            bool catBeat = rainworld.progression.miscProgressionData.redUnlocked;
            string easyText = "Enable the ability to dropkick by pressing Jump + Grab while midair.";
            base.Initialize();
            OpTab mainTab = new OpTab(this, "Main");
            OpTab buildTab = new OpTab(this, "Builds");
            OpTab gimmickTab = new OpTab(this, "Gimmicks");
            this.Tabs = new OpTab[]{
                mainTab,
                buildTab,
                gimmickTab
            };


            this.mainSet = new UIelement[]{
                new OpLabel(xo, yo, "Options", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), swapper("These options change Escort's main abilities and how they play.<LINE>You can find the easy mode and builds in BUILDS and additional funny shenanigans in GIMMICKS")){
                    color = descColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Enable (Extra) Mean Lizards"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgMeanLizards, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    description = OptionInterface.Translate("When enabled, lizards will spawn at full aggression when playing Escort... the behaviour may not carry over in coop sessions. (Default=true)"),
                    colorEdge = tempColor
                },

                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 3), "Lifting Power Multiplier"),
                new OpUpdown(this.cfgHeavyLift, new Vector2(xo + (xp * 0), yo - (yp * 3) - tp), 100, 2){
                    description = OptionInterface.Translate("Determines how heavy (X times Escort's weight) of an object Escort can carry before being burdened. (Default=3.0, Normal=0.6)")
                },
                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 4), "Drop Kick Multiplier"),
                new OpUpdown(this.cfgDKMult, new Vector2(xo + (xp * 0), yo - (yp * 4) - tp), 100, 2){
                    description = OptionInterface.Translate("How much knockback does Escort's drop kick cause? Keep in mind it only affects creatures that aren't resiliant to knockback. (Default=3.0)"),
                },

                new OpLabel(xo + (xp * 0) + 7f, yo - (yp * 5) - tp, "Battle-Hype Mechanic"),
                new OpCheckBox(this.cfgHypable, new Vector2(xo + (xp * 0), yo - (yp * 6) + tp/2)){
                    description = OptionInterface.Translate("Enables/disables Escort's Battle-Hype mechanic. (Default=true)")
                },
                new OpSliderTick(this.cfgHypeReq, new Vector2(xo + (xp * 1) + 7f, yo - (yp * 6)), 400 - (int)xp - 7){
                    min = 0,
                    max = 6,
                    description = OptionInterface.Translate("Determines how lenient the Battle-Hype requirements are. (Default=3)"),
                },
                new OpLabel(440f + xp - 7, yo - (yp * 6), swapper("0=Always on<LINE>1=50% tiredness<LINE>2=66% tiredness<LINE>3=75% tiredness<LINE>4=80% tiredness<LINE>5=87% tiredness<LINE>6=92% tiredness")),

                new OpLabel(xo + (xp * 1), yo - (yp * 7) + tp/2, "Long Wall Jump"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgLongWallJump, new Vector2(xo + (xp * 0), yo - (yp * 7))){
                    description = OptionInterface.Translate("Allows Escort to do long jumps (hold jump) but on walls as well. May affect how normal wall jumping feels. (Default=false)"),
                    colorEdge = tempColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 8) + tp/2, "Flippy Pounce"),
                new OpCheckBox(this.cfgPounce, new Vector2(xo + (xp * 0), yo - (yp * 8))){
                    description = OptionInterface.Translate("Causes Escort to do a (sick) flip with long jumps, and allows Escort to do a super-wall-flip-jumpTM when holding diagonal against a wall and pressing jump to achieve great vertical height. (Default=true)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 9) + tp/2, "Grab Stunned Lizards"),
                new OpCheckBox(this.cfgDunkin, new Vector2(xo + (xp * 0), yo - (yp * 9))){
                    description = OptionInterface.Translate("Allows Escort to grab stunned lizards and cause violence on them. (Default=true)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 10) + tp/2, "Super Spear"),
                new OpCheckBox(this.cfgSpears, new Vector2(xo + (xp * 0), yo - (yp * 10))){
                    description = OptionInterface.Translate("Does additional movement tech when throwing spears. (Default=true)")
                }
            };
            this.buildSet = new UIelement[]{
                new OpLabel(xo, yo, "Builds", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), swapper("These will change hidden values in certain ways to make Escort play differently!<LINE>Try each build out to see which one you vibe to the most.")){
                    color = descColor
                },

                new OpLabel(xo + (xp * 2) + (tp * 1.5f), yo - 3f - (yp * 2) + tp/2, "Easier Mode"){
                    color = easyColor
                },
                new OpCheckBox(this.cfgEasyP1, new Vector2(xo - (tp * 5) + 3f, yo - 3f - (yp * 2))){
                    description = OptionInterface.Translate("[P1] " + easyText),
                    colorEdge = easyColor,
                    colorFill = p1Color * 0.45f
                },
                new OpCheckBox(this.cfgEasyP2, new Vector2(xo - (tp * 1) + 3f, yo - 3f - (yp * 2))){
                    description = OptionInterface.Translate("[P2] " + easyText),
                    colorEdge = easyColor,
                    colorFill = p2Color * 0.45f
                },
                new OpCheckBox(this.cfgEasyP3, new Vector2(xo + (tp * 3) + 3f, yo - 3f - (yp * 2))){
                    description = OptionInterface.Translate("[P3] " + easyText),
                    colorEdge = easyColor,
                    colorFill = p3Color * 0.7f
                },
                new OpCheckBox(this.cfgEasyP4, new Vector2(xo + (tp * 7) + 3f, yo - 3f - (yp * 2))){
                    description = OptionInterface.Translate("[P4] " + easyText),
                    colorEdge = easyColor,
                    colorFill = p4Color
                },

                new OpLabel(xo - (tp * 3.8f), yo + 3f - (yp * 1.5f), "(1)   (2)   (3)   (4) <-PLAYER #"){
                    color = new Color(0.5f, 0.5f, 0.5f)
                },

                new OpSliderTick(this.cfgBuildP1, new Vector2(xo - (tp * 5), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    colorLine = p1Color*0.8f,
                    colorEdge = p1Color*0.9f,
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP2, new Vector2(xo - (tp * 1), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    colorLine = p2Color*0.8f,
                    colorEdge = p2Color*0.9f,
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP3, new Vector2(xo + (tp * 3), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    colorLine = p3Color*0.9f,
                    colorEdge = p3Color,
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP4, new Vector2(xo + (tp * 7), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    colorLine = p4Color*2.4f,
                    colorEdge = p4Color*2.8f,
                    min = this.buildDiv,
                    max = 0
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 2.5f) - (tp * 1.3f), "Default", true){
                    color = bDefault * 0.75f
                },
                new OpLabel(xo + (xp * 2) - 1f, yo - (yp * 2.5f) - (tp * 2.1f) + 0.7f, "  The intended way of playing Escort."){
                    color = bShadow
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 2.5f) - (tp * 2.1f), "  The intended way of playing Escort."){
                    color = bDefault
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 3.5f) - (tp * 1.3f), "Brawler", true){
                    color = bBrawler * 0.75f
                },
                new OpLabel(xo + (xp * 2) - 1f, yo - (yp * 3.5f) - (tp * 2.1f) + 0.7f, "  More powerful and consistent close-combat, but reduced range efficiency."){
                    color = bShadow
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 3.5f) - (tp * 2.1f), "  More powerful and consistent close-combat, but reduced range efficiency."){
                    color = bBrawler
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 4.5f) - (tp * 1.3f), "Deflector", true){
                    color = bDeflector * 0.75f
                },
                new OpLabel(xo + (xp * 2) - 1f, yo - (yp * 4.5f) - (tp * 2.1f) + 0.7f, "  Easier to parry, with empowered damage on success!... at the cost of some base stats."){
                    color = bShadow
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 4.5f) - (tp * 2.1f), "  Easier to parry, with empowered damage on success!... at the cost of some base stats."){
                    color = bDeflector
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 5.5f) - (tp * 1.3f), "Escapist", true){
                    color = bEscapist * 0.75f
                },
                new OpLabel(xo + (xp * 2) - 1f, yo - (yp * 5.5f) - (tp * 2.1f) + 0.7f, "  Force out of grasps, though don't expect to be fighting much..."){
                    color = bShadow
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 5.5f) - (tp * 2.1f), "  Force out of grasps, though don't expect to be fighting much..."){
                    color = bEscapist
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 6.5f) - (tp * 1.3f), "Railgunner", true){
                    color = bRailgunner * 0.75f
                },
                new OpLabel(xo + (xp * 2) - 1f, yo - (yp * 6.5f) - (tp * 2.1f) + 0.7f, swapper("  With the aid of <REPLACE>, dual-wield for extreme results!", catBeat? "the void" : "mysterious forces")){
                    color = bShadow
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 6.5f) - (tp * 2.1f), swapper("  With the aid of <REPLACE>, dual-wield for extreme results!", catBeat? "the void" : "mysterious forces")){
                    color = bRailgunner
                },
            };
            this.gimmickSet = new UIelement[]{
                new OpLabel(xo, yo, "Gimmicks", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), "These optional gimmicks will add features that may alter the gameplay of Escort significantly (and usually in hilarous ways)"){
                    color = descColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Enable dumb SFX"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgSFX, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Replaces a few sound effects that on one hand will kill the immersion, but on the other hand... haha funny noises. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 3) + tp/2, "Enable Elevator"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgElevator, new Vector2(xo + (xp * 0), yo - (yp * 3))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("When enabled, allows Escort to touch the stars whenever they ramp off a living creature. (Default=false)"),
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 4) + tp/2, "Proper Dropkick Animation"),
                new OpCheckBox(this.cfgDKAnimation, new Vector2(xo + (xp * 0), yo - (yp * 4))){
                    description = OptionInterface.Translate("During dropkicks, Escort turns their body in midair to make dropkicks look like dropkick dropkicks. (Default=true)"),
                }
            };
            mainTab.AddItems(this.mainSet);
            buildTab.AddItems(this.buildSet);
            gimmickTab.AddItems(this.gimmickSet);
        }
    }
}