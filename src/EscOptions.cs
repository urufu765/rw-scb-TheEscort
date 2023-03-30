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
        private UIelement[] mainSet;
        private readonly float yoffset = 540f;
        private readonly float xoffset = 30f;
        private readonly float ypadding = 40f;
        private readonly float xpadding = 35f;
        private readonly float tpadding = 6f;
    
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
        }

        public override void Initialize()
        {
            float xo = this.xoffset;
            float yo = this.yoffset;
            float xp = this.xpadding;
            float yp = this.ypadding;
            float tp = this.tpadding;
            String hypeReqText = "0=Always on<LINE>1=50% tiredness<LINE>2=66% tiredness<LINE>3=75% tiredness<LINE>4=80% tiredness<LINE>5=87% tiredness<LINE>6=92% tiredness";
            hypeReqText = hypeReqText.Replace("<LINE>", "" + System.Environment.NewLine);
            base.Initialize();
            OpTab mainTab = new OpTab(this, "Main");
            this.Tabs = new OpTab[]{
                mainTab
            };


            this.mainSet = new UIelement[]{
                new OpLabel(xo, yo, "Options", true),

                new OpLabel(xo + (xp * 1), yo - (yp * 1) + tp/2, "Enable (Extra) Mean Lizards"),
                new OpCheckBox(this.cfgMeanLizards, new Vector2(xo + (xp * 0), yo - (yp * 1))){
                    description = OptionInterface.Translate("When enabled, lizards will spawn at full aggression when playing Escort... the behaviour may not carry over in coop sessions. (Default=true)")
                },

                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 2), "Lifting Power Multiplier"),
                new OpUpdown(this.cfgHeavyLift, new Vector2(xo + (xp * 0), yo - (yp * 2) - tp), 100, 2){
                    description = OptionInterface.Translate("Determines how heavy (X times Escort's weight) of an object Escort can carry before being burdened. (Default=3.0, Normal=0.6)")
                },
                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 3), "Drop Kick Multiplier"),
                new OpUpdown(this.cfgDKMult, new Vector2(xo + (xp * 0), yo - (yp * 3) - tp), 100, 2){
                    description = OptionInterface.Translate("How much knockback does Escort's drop kick cause? Keep in mind it only affects creatures that aren't resiliant to knockback. (Default=3.0)"),
                },

                new OpLabel(xo + (xp * 0) + 7f, yo - (yp * 4) - tp, "Battle-Hype Mechanic"),
                new OpCheckBox(this.cfgHypable, new Vector2(xo + (xp * 0), yo - (yp * 5) + tp/2)){
                    description = OptionInterface.Translate("Enables/disables Escort's Battle-Hype mechanic. (Default=true)")
                },
                new OpSliderTick(this.cfgHypeReq, new Vector2(xo + (xp * 1) + 7f, yo - (yp * 5)), 400 - (int)xp - 7){
                    min = 0,
                    max = 6,
                    description = OptionInterface.Translate("Determines how lenient the Battle-Hype requirements are. (Default=3)"),
                },
                new OpLabel(440f + xp - 7, yo - (yp * 5), hypeReqText),


                new OpLabel(xo + (xp * 1), yo - (yp * 6) + tp/2, "Enable dumb SFX"),
                new OpCheckBox(this.cfgSFX, new Vector2(xo + (xp * 0), yo - (yp * 6))){
                    description = OptionInterface.Translate("Replaces a few sound effects that on one hand will kill the immersion, but on the other hand... haha funny noises. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 7) + tp/2, "Enable Elevator"),
                new OpCheckBox(this.cfgElevator, new Vector2(xo + (xp * 0), yo - (yp * 7))){
                    description = OptionInterface.Translate("When enabled, allows Escort to touch the stars whenever they ramp off a living creature. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 8) + tp/2, "Long Wall Jump"),
                new OpCheckBox(this.cfgLongWallJump, new Vector2(xo + (xp * 0), yo - (yp * 8))){
                    description = OptionInterface.Translate("Allows Escort to do long jumps (hold jump) but on walls as well. May affect how normal wall jumping feels. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 9) + tp/2, "Flippy Pounce"),
                new OpCheckBox(this.cfgPounce, new Vector2(xo + (xp * 0), yo - (yp * 9))){
                    description = OptionInterface.Translate("Causes Escort to do a (sick) flip with long jumps, and allows Escort to do a super-wall-flip-jumpTM when holding downdiagonal when against a wall and achieve great vertical height. (Default=true)")
                }

            };
            mainTab.AddItems(this.mainSet);
        }
    }
}