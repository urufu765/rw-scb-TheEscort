using System;
using System.Globalization;
using Menu.Remix;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;


namespace SlugTemplate{
    public class EscortOptionInterface: OptionInterface{
        private OpLabel currentPresetLabel;
        private OpSimpleButton standardPresetBtn;
        private OpSimpleButton derailedPresetBtn;
        private OpSimpleButton gladiatorPresetBtn;


        public override void Initialize()
        {
            base.Initialize();
            this.Tabs = new OpTab[]{
                new OpTab(this, OptionInterface.Translate("Presets")),
                new OpTab(this, OptionInterface.Translate("Main")),
                new OpTab(this, OptionInterface.Translate("Gimmicks"))
            };

            OpLabel opLabel1 = new OpLabel(
                new Vector2(150f, 520f), new Vector2(300f, 30f), 
                base.mod.LocalizedName, 
                FLabelAlignment.Center, true, null);
            this.Tabs[0].AddItems(new UIelement[]{opLabel1});

            OpLabel opLabel2 = new OpLabel(
                new Vector2(150f, 420f), new Vector2(300f, 30f), 
                Custom.ReplaceLineDelimeters(
                    OptionInterface.Translate("Select a preset!") + 
                    Environment.NewLine + 
                    OptionInterface.Translate("Use these to reset the options to default... either into Intended Gameplay way, or the MEME way.")), 
                FLabelAlignment.Center, false, null);
            this.Tabs[0].AddItems(new UIelement[]{opLabel2});
            
            this.standardPresetBtn = new OpSimpleButton(
                new Vector2(250f, 360f), new Vector2(120f, 30f), OptionInterface.Translate("Standard")){
                    description = OptionInterface.Translate(
                        "The standard configuration of The Escort; the intended way they're supposed to be played."
                    )};
            this.standardPresetBtn.OnClick += this.PresetStandard;
            this.Tabs[0].AddItems(new UIelement[]{this.standardPresetBtn});

            this.derailedPresetBtn = new OpSimpleButton(
                new Vector2(250f, 320f), new Vector2(120f, 30f), OptionInterface.Translate("Derailed")){
                    description = OptionInterface.Translate(
                        "The MEME configuration of The Escort, aimed to deliver a massive joke of an experience."
                    )};
            this.derailedPresetBtn.OnClick += this.PresetDerailed;
            this.Tabs[0].AddItems(new UIelement[]{this.derailedPresetBtn});

            // Apply this button once tuning their abilities based on feedback is possible.
            this.gladiatorPresetBtn = new OpSimpleButton(
                new Vector2(250f, 280f), new Vector2(120f, 30f), OptionInterface.Translate("Gladiator")){
                description = OptionInterface.Translate(
                        "Tune Escort's powers purely for PvP purposes, reducing power in some parts of their kit, while dialing up other bits."
                    )};
            this.gladiatorPresetBtn.OnClick += this.PresetGladiator;
            this.Tabs[0].AddItems(new UIelement[]{this.gladiatorPresetBtn});

            ConfigurableBase[] array = new ConfigurableBase[]{
            };
        }
    }
}