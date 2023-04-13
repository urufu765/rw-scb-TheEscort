using System;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using UnityEngine;


namespace TheEscort{
    class EscOptions : OptionInterface{


        public readonly Plugin instance;
        public readonly ManualLogSource logSource;
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
        public Configurable<bool> cfgDunkin;
        public Configurable<bool> cfgSpears;
        private UIelement[] mainSet;
        private UIelement[] buildSet;
        private UIelement[] gimmickSet;
        private readonly float yoffset = 540f;
        private readonly float xoffset = 30f;
        private readonly float ypadding = 40f;
        private readonly float xpadding = 35f;
        private readonly float tpadding = 6f;
        private readonly int buildDiv = -3;
    
        public EscOptions(Plugin instance, ManualLogSource loggerSource){
            this.instance = instance;
            this.logSource = loggerSource;
            this.cfgMeanLizards = this.config.Bind<bool>("cfg_Mean_Lizards", true);
            this.cfgHeavyLift = this.config.Bind<float>("cfg_Heavy_Lift", 3f, new ConfigAcceptableRange<float>(0.01f, 1000f));
            this.cfgDKMult = this.config.Bind<float>("cfg_Drop_Kick_Multiplier", 3f, new ConfigAcceptableRange<float>(0.01f, 765f));
            this.cfgElevator = this.config.Bind<bool>("cfg_Elevator", false);
            this.cfgHypable = this.config.Bind<bool>("cfg_Hypable", true);
            this.cfgHypeReq = this.config.Bind<int>("cfg_Hype_Requirement", 3, new ConfigAcceptableRange<int>(0, 6));
            this.cfgSFX = this.config.Bind<bool>("cfg_SFX", false);
            this.cfgPounce = this.config.Bind<bool>("cfg_Pounce", true);
            this.cfgLongWallJump = this.config.Bind<bool>("cfg_Long_Wall_Jump", false);
            this.cfgBuildNum = this.config.Bind<int>("cfg_Build", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP1 = this.config.Bind<int>("cfg_Build_P1", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP2 = this.config.Bind<int>("cfg_Build_P2", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP3 = this.config.Bind<int>("cfg_Build_P3", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgBuildP4 = this.config.Bind<int>("cfg_Build_P4", 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
            this.cfgDunkin = this.config.Bind<bool>("cfg_Dunkin_Lizards", true);
            this.cfgSpears = this.config.Bind<bool>("cfg_Super_Spear", true);
        }

        public override void Initialize()
        {
            float xo = this.xoffset;
            float yo = this.yoffset;
            float xp = this.xpadding;
            float yp = this.ypadding;
            float tp = this.tpadding;
            String hypeReqText = "0=Always on<LINE>1=50% tiredness<LINE>2=66% tiredness<LINE>3=75% tiredness<LINE>4=80% tiredness<LINE>5=87% tiredness<LINE>6=92% tiredness";
            String mainText = "These options change Escort's main abilities and how they play.";
            String buildText = "These will change hidden values in certain ways to make Escort play differently!<LINE>Try each build out to see which one you vibe to the most.";
            String gimmickText = "These optional gimmicks will add features that may alter the gameplay of Escort significantly (and usually in hilarous ways)";
            String buildP = "Player<LINE>(1)   (2)   (3)   (4)";
            hypeReqText = hypeReqText.Replace("<LINE>", "" + System.Environment.NewLine);
            mainText = mainText.Replace("<LINE>", "" + System.Environment.NewLine);
            buildText = buildText.Replace("<LINE>", "" + System.Environment.NewLine);
            buildP = buildP.Replace("<LINE>", "" + System.Environment.NewLine);
            gimmickText = gimmickText.Replace("<LINE>", "" + System.Environment.NewLine);
            Color descColor = new Color(0.49f, 0.44f, 0.55f);
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
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), mainText),

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Enable (Extra) Mean Lizards"),
                new OpCheckBox(this.cfgMeanLizards, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    description = OptionInterface.Translate("When enabled, lizards will spawn at full aggression when playing Escort... the behaviour may not carry over in coop sessions. (Default=true)")
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
                new OpLabel(440f + xp - 7, yo - (yp * 6), hypeReqText),

                new OpLabel(xo + (xp * 1), yo - (yp * 7) + tp/2, "Long Wall Jump"),
                new OpCheckBox(this.cfgLongWallJump, new Vector2(xo + (xp * 0), yo - (yp * 7))){
                    description = OptionInterface.Translate("Allows Escort to do long jumps (hold jump) but on walls as well. May affect how normal wall jumping feels. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 8) + tp/2, "Flippy Pounce"),
                new OpCheckBox(this.cfgPounce, new Vector2(xo + (xp * 0), yo - (yp * 8))){
                    description = OptionInterface.Translate("Causes Escort to do a (sick) flip with long jumps, and allows Escort to do a super-wall-flip-jumpTM when holding downdiagonal when against a wall and achieve great vertical height. (Default=true)")
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
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), buildText),
                new OpLabel(xo - (tp * 3.8f), yo - (yp * 1.5f), buildP){
                    color = new Color(0.5f, 0.5f, 0.5f)
                },

                new OpSliderTick(this.cfgBuildP1, new Vector2(xo - (tp * 5), (yo + tp) - (yp * 2) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP2, new Vector2(xo - (tp * 1), (yo + tp) - (yp * 2) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP3, new Vector2(xo + (tp * 3), (yo + tp) - (yp * 2) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    min = this.buildDiv,
                    max = 0
                },
                new OpSliderTick(this.cfgBuildP4, new Vector2(xo + (tp * 7), (yo + tp) - (yp * 2) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                    min = this.buildDiv,
                    max = 0
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 2), "Default"),
                new OpLabel(xo + (xp * 2), yo - (yp * 2) - (tp * 2), " The intended way of playing Escort."){
                    color = descColor
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 3), "Brawler"),
                new OpLabel(xo + (xp * 2), yo - (yp * 3) - (tp * 2), " More powerful and consistent close combat, but reduced range efficiency."){
                    color = descColor
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 4), "Deflector"),
                new OpLabel(xo + (xp * 2), yo - (yp * 4) - (tp * 2), " Easier to parry, with empowered damage on success!... at the cost of some base stats."){
                    color = descColor
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 5), "Escapist"),
                new OpLabel(xo + (xp * 2), yo - (yp * 5) - (tp * 2), " Force out of grasps, though don't expect to be fighting much..."){
                    color = descColor
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 6), "Railgunner"),
                new OpLabel(xo + (xp * 2), yo - (yp * 6) - (tp * 2), " Embrace an unorthodox, yet powerful fighting style."){
                    color = descColor
                },
            };
            this.gimmickSet = new UIelement[]{
                new OpLabel(xo, yo, "Gimmicks", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), gimmickText),

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Enable dumb SFX"),
                new OpCheckBox(this.cfgSFX, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    description = OptionInterface.Translate("Replaces a few sound effects that on one hand will kill the immersion, but on the other hand... haha funny noises. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 3) + tp/2, "Enable Elevator"),
                new OpCheckBox(this.cfgElevator, new Vector2(xo + (xp * 0), yo - (yp * 3))){
                    description = OptionInterface.Translate("When enabled, allows Escort to touch the stars whenever they ramp off a living creature. (Default=false)")
                }
            };
            mainTab.AddItems(this.mainSet);
            buildTab.AddItems(this.buildSet);
            gimmickTab.AddItems(this.gimmickSet);
        }
    }
}