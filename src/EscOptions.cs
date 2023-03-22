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
        private UIelement[] mainSet;
    
        public EscOptions(Plugin instance, ManualLogSource loggerSource){
            this.instance = instance;
            this.logSource = loggerSource;
            this.cfgMeanLizards = this.config.Bind<bool>("cfg_Mean_Lizards", true);
            this.cfgHeavyLift = this.config.Bind<float>("cfg_Heavy_Lift", 3f, new ConfigAcceptableRange<float>(0.3f, 100f));
            this.cfgDKMult = this.config.Bind<float>("cfg_Drop_Kick_Multiplier", 3f, new ConfigAcceptableRange<float>(0.5f, 100f));
            this.cfgElevator = this.config.Bind<bool>("cfg_Elevator", false);
            this.cfgHypable = this.config.Bind<bool>("cfg_Hypable", true);
            this.cfgHypeReq = this.config.Bind<int>("cfg_Hype_Requirement", 3, new ConfigAcceptableRange<float>(0f, 1f));
            this.cfgSFX = this.config.Bind<bool>("cfg_SFX", true);
        }

        public override void Initialize()
        {
            base.Initialize();
            OpTab opTab = new OpTab(this, "Options");
            this.Tabs = new OpTab[]{
                opTab
            };
            this.mainSet = new UIelement[]{
                new OpLabel(30f, 260f, "Options", true),
                new OpCheckBox(this.cfgMeanLizards, new Vector2(30f, 230f)),
                new OpFloatSlider(this.cfgHeavyLift, new Vector2(30f, 200f), 100){
                    min = 0.3f,
                    max = 100f
                },
                new OpFloatSlider(this.cfgDKMult, new Vector2(30f, 170f), 100){
                    min = 0.5f,
                    max = 100f
                },
                new OpCheckBox(this.cfgHypable, new Vector2(30f, 140f)),
                new OpSliderTick(this.cfgHypeReq, new Vector2(60f, 140f), 70){
                    min = 0,
                    max = 6
                },
                new OpCheckBox(this.cfgSFX, new Vector2(30f, 110f)),
                new OpCheckBox(this.cfgElevator, new Vector2(30f, 80f))
            };
            opTab.AddItems(this.mainSet);
        }
    }
}