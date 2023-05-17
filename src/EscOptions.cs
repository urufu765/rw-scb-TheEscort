using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using System;
using UnityEngine;
using static TheEscort.Eshelp;


namespace TheEscort
{
    /*
    class EscCusOpt : Dialog, SelectOneButton.SelectOneButtonOwner
    {
        public MenuIllustration title;
        public SimpleButton cancelButton;
        public float leftAnchor, rightAnchor;
        public bool opening, closing;
        public float movementCounter;
        public SelectOneButton[] topicButtons;
        public MenuLabel pageLabel;
        public ManualPage currentTopicPage;
        public int index, pageNumber;
        public String currentTopic;
        public Dictionary<string, int> topics;
        public float sin;
        public bool firstView;
        public float lastAlpha;
        public float currentAlpha;
        public float uAlpha;
        public float targetAlpha;
        public float globalOffX;
        public float contentOffX;
        public float wrapTextMargin;

        public EscCusOpt(ProcessManager manager, Dictionary<string, int> topics, MenuObject owner) : base(manager)
        {
            float[] screenOffsets = RWCustom.Custom.GetScreenOffsets();
            leftAnchor = screenOffsets[0]; rightAnchor = screenOffsets[1];
            this.topics = topics;
            pages[0].pos = new Vector2(0.01f, 0f);
            pages[0].pos.y += 2000f;
            pages.Add(new Page(this, owner, "CLASS", 1));
            pages[1].pos = new Vector2(520.01f, 155f);
            pages[1].pos.y += 2155f;

        }

        public int GetCurrentlySelectedOfSeries(string series)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentlySelectedOfSeries(string series, int to)
        {
            throw new NotImplementedException();
        }
    }*/

    class EscOptions : OptionInterface
    {


        //public readonly Plugin instance;
        public readonly RainWorld rainworld;
        public Configurable<bool> cfgMeanLizards;
        public Configurable<bool> cfgVengefulLizards;
        // public Configurable<bool> cfgMeanGarbWorms;
        public Configurable<float> cfgHeavyLift;
        public Configurable<float> cfgDKMult;
        public Configurable<bool> cfgElevator;
        public Configurable<bool> cfgHypable;
        public Configurable<int> cfgHypeReq;
        public Configurable<bool> cfgSFX;
        public Configurable<bool> cfgPounce;
        public Configurable<bool> cfgLongWallJump;
        public Configurable<int> cfgBuildNum;  // LEGACY!
        public Configurable<int> cfgBuildP1, cfgBuildP2, cfgBuildP3, cfgBuildP4;  // LEGACY #2
        public Configurable<int>[] cfgBuild;
        public Configurable<bool> cfgEasyP1, cfgEasyP2, cfgEasyP3, cfgEasyP4;  // LEGACY #2
        public Configurable<bool>[] cfgEasy;
        public Configurable<bool> cfgCustomP1, cfgCustomP2, cfgCustomP3, cfgCustomP4;
        public Configurable<bool> cfgDunkin;
        public Configurable<bool> cfgSpears;
        public Configurable<bool> cfgDKAnimation;
        public Configurable<bool> cfgNoticeHype;
        public Configurable<bool> cfgNoticeEmpower;
        public Configurable<bool> cfgFunnyDeflSlide;
        public Configurable<bool> cfgPoleBounce;
        public Configurable<bool> cfgOldSpeedster;
        public Configurable<bool> cfgDeveloperMode;
        public Configurable<int> cfgSecret;
        public Configurable<bool> cfgSectret;
        private OpTextBox secretText;
        //private OpCheckBox hypableBtn;
        //private OpSliderTick hypedSlide;
        private OpCheckBox hypeableBox;
        private OpSliderTick hypeableTick;
        private OpLabel[] hypeableText;
        private UIelement[] mainSet;
        private UIelement[] buildSet, buildTitle, buildText, buildShadow;
        //private OpCheckBox buildEasyP1, buildEasyP2, buildEasyP3, buildEasyP4;
        public OpCheckBox[] buildEasy;
        //private OpSliderTick buildP1, buildP2, buildP3, buildP4;
        public OpSliderTick[] buildPlayer;
        private UIelement[] gimmickSet;
        private UIelement[] accessibleSet;
        private Color[] buildColors;
        private Color p1Color, p2Color, p3Color, p4Color;
        private readonly float yoffset = 560f;
        private readonly float xoffset = 30f;
        private readonly float ypadding = 40f;
        private readonly float xpadding = 35f;
        private readonly float tpadding = 6f;
        public readonly int buildDiv = -6;
        public readonly Color easyColor = new(0.42f, 0.75f, 0.5f);
        private static readonly string VERSION = "0.2.8.2";
        private readonly Configurable<string> cfgVersion;
        private static string HelloWorld {
            get{
                return Swapper("New in version " + VERSION + ":<LINE><LINE>" +
                "Nerfed railgunner CD (so that you can actually use the boosted damage and actually have risk of blowing up). Nerfed brawler punch (gave more cooldown), buffed shank, and added alternative shank mode. Buffed Deflector's speed to hunter speed instead of survivor speed. Added version notes (and -what's new- page). Minor code tweaks.");
            }
        }

        // Jolly Coop button stuff don't worry about it
        public OpSimpleButton[] jollyEscortBuilds;
        public OpSimpleButton[] jollyEscortEasies;
        //public bool[] jollyEasierState = new bool[4];

        // Arena button stuff
        //public OpSimpleButton[] arenaEscortBuilds;
        //public OpSimpleButton[] arenaEscortEasies;

        public EscOptions(RainWorld rainworld)
        {
            this.rainworld = rainworld;
            this.cfgMeanLizards = this.config.Bind<bool>("cfg_Mean_Lizards", false);
            this.cfgVengefulLizards = this.config.Bind<bool>("cfg_Vengeful_Lizards", false);
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
            this.cfgBuild = new Configurable<int>[4];
            this.cfgEasy = new Configurable<bool>[4];
            for (int x = 0; x < this.cfgBuild.Length; x++){
                this.cfgBuild[x] = this.config.Bind<int>("cfg_Build_Player" + x, 0, new ConfigAcceptableRange<int>(this.buildDiv, 0));
                this.cfgEasy[x] = this.config.Bind<bool>("cfg_Easy_Player" + x, false);
            }
            this.cfgEasyP1 = this.config.Bind<bool>("cfg_Easy_P1", false);
            this.cfgEasyP2 = this.config.Bind<bool>("cfg_Easy_P2", false);
            this.cfgEasyP3 = this.config.Bind<bool>("cfg_Easy_P3", false);
            this.cfgEasyP4 = this.config.Bind<bool>("cfg_Easy_P4", false);
            this.cfgCustomP1 = this.config.Bind<bool>("cfg_Custom_P1", false);
            this.cfgCustomP2 = this.config.Bind<bool>("cfg_Custom_P2", false);
            this.cfgCustomP3 = this.config.Bind<bool>("cfg_Custom_P3", false);
            this.cfgCustomP4 = this.config.Bind<bool>("cfg_Custom_P4", false);
            this.cfgDunkin = this.config.Bind<bool>("cfg_Dunkin_Lizards", true);
            this.cfgSpears = this.config.Bind<bool>("cfg_Super_Spear", true);
            this.cfgNoticeHype = this.config.Bind<bool>("cfg_Noticeable_Hype", false);
            this.cfgNoticeEmpower = this.config.Bind<bool>("cfg_Noticeable_Empower", false);
            this.cfgFunnyDeflSlide = this.config.Bind<bool>("cfg_Funny_Deflector_Slide", false);
            this.cfgPoleBounce = this.config.Bind<bool>("cfg_Pole_Bounce", false);
            this.cfgOldSpeedster = this.config.Bind<bool>("cfg_Old_Speedster", false);
            this.cfgDeveloperMode = this.config.Bind<bool>("cfg_Dev_Log_Mode", false);
            this.cfgDeveloperMode.OnChange += LongDevLogChange;
            this.cfgSecret = this.config.Bind<int>("cfg_EscSecret", 765, new ConfigAcceptableRange<int>(0, 99999));
            this.cfgSectret = this.config.Bind<bool>("cfg_EscSectret", false);
            //this.cfgSecret.OnChange += inputSecret;
            this.buildEasy = new OpCheckBox[4];
            this.buildPlayer = new OpSliderTick[4];
            this.cfgVersion = this.config.Bind<string>("cfg_Escort_Version", VERSION);
        }

        private static string Swapper(string text, string with = "")
        {
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

            Color tempColor = new(0.5f, 0.5f, 0.55f);
            Color descColor = new(0.53f, 0.48f, 0.59f);

            p1Color = new Color(1f, 1f, 1f);
            p2Color = new Color(1f, 1f, 0.451f);
            p3Color = new Color(1f, 0.451f, 0.451f);
            p4Color = new Color(0.09f, 0.1373f, 0.306f);
            //Color p4Color = RWCustom.Custom.HSL2RGB(0.63055557f, 0.54f, 0.2f);

            Color bShadow = new Color(0.1f, 0.1f, 0.1f);
            Color bDefault = new Color(0.75f, 0.75f, 0.75f);
            Color bBrawler = new Color(0.8f, 0.4f, 0.6f);
            Color bDeflector = new Color(0.69f, 0.55f, 0.9f);
            Color bEscapist = new Color(0.42f, 0.75f, 0.1f);
            Color bRailgunner = new Color(0.5f, 0.85f, 0.78f);
            Color bSpeedster = new Color(0.76f, 0.78f, 0f);
            Color bGilded = new Color(0.796f, 0.549f, 0.27843f);
            Color bUltKill = new Color(0.7f, 0.2f, 0.2f);
            this.buildColors = new Color[]{
                bDefault, bBrawler, bDeflector, bEscapist, bRailgunner, bSpeedster, bGilded
            };
            // I'm so done with this shit, may we never remotely reach 1.5k


            this.secretText = new OpTextBox(this.cfgSecret, new Vector2(xo + (xp * 14f), yo - (yp * 14)), 60)
            {
                description = OptionInterface.Translate("Hmm? What's this?"),
                colorEdge = new Color(0.9294f, 0.898f, 0.98f, 0.55f),
                colorFill = new Color(0.1843f, 0.1843f, 0.1843f, 0.55f),
                colorText = new Color(0.9294f, 0.898f, 0.98f, 0.55f),
                maxLength = 5
            };
            this.secretText.OnValueChanged += InputTheSecret;
            /*
            this.hypableBtn = new OpCheckBox(this.cfgHypable, new Vector2(xo + (xp * 0), yo - (yp * 6) + tp/2)){
                description = OptionInterface.Translate("Enables/disables Escort's Battle-Hype mechanic. (Default=true)")
            };
            this.hypedSlide = new OpSliderTick(this.cfgHypeReq, new Vector2(xo + (xp * 1) + 7f, yo - (yp * 6)), 400 - (int)xp - 7){
                min = 0,
                max = 6,
                description = OptionInterface.Translate("Determines how lenient the Battle-Hype requirements are. (Default=3)"),
            };
            //this.hypableBtn.OnDeactivate += setTheHype;
            //this.hypableBtn.OnReactivate += killTheHype;
            */
            this.hypeableBox = new OpCheckBox(this.cfgHypable, new Vector2(xo + (xp * 0), yo - (yp * 7) + tp / 2))
            {
                description = OptionInterface.Translate("Enables/disables Escort's Battle-Hype mechanic. (Default=true)")
            };
            this.hypeableBox.OnValueChanged += ToggleDisableHyped;
            this.hypeableTick = new OpSliderTick(this.cfgHypeReq, new Vector2(xo + (xp * 1) + 7f, yo - (yp * 7)), 400 - (int)xp - 7)
            {
                min = 0,
                max = 6,
                description = OptionInterface.Translate("Determines how lenient the Battle-Hype requirements are. (Default=3)"),
            };
            this.hypeableTick.OnValueUpdate += RunItThruHyped;
            this.hypeableText = new OpLabel[7];
            for (int i = 0; i < this.hypeableText.Length; i++)
            {
                this.hypeableText[i] = new OpLabel(440f + xp - tp, yo - (yp * 6) - (tp * 2 * i), i switch
                {
                    1 => "1=50% tiredness",
                    2 => "2=66% tiredness",
                    3 => "3=75% tiredness",
                    4 => "4=80% tiredness",
                    5 => "5=87% tiredness",
                    6 => "6=92% tiredness",
                    _ => "0=Always on"
                })
                {
                    color = this.cfgHypeReq.Value == i && !this.hypeableTick.greyedOut ? Menu.MenuColorEffect.rgbMediumGrey : Menu.MenuColorEffect.rgbDarkGrey
                };
            }



            /*
            this.buildP1 = new OpSliderTick(this.cfgBuildP1, new Vector2(xo - (tp * 5), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                colorLine = p1Color*0.8f,
                colorEdge = p1Color*0.9f,
                min = this.buildDiv,
                max = 0
            };
            //this.buildP1.OnFocusGet += viewBuild;
            this.buildP2 = new OpSliderTick(this.cfgBuildP2, new Vector2(xo - (tp * 1), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                colorLine = p2Color*0.8f,
                colorEdge = p2Color*0.9f,
                min = this.buildDiv,
                max = 0
            };
            this.buildP3 = new OpSliderTick(this.cfgBuildP3, new Vector2(xo + (tp * 3), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                colorLine = p3Color*0.9f,
                colorEdge = p3Color,
                min = this.buildDiv,
                max = 0
            };
            this.buildP4 = new OpSliderTick(this.cfgBuildP4, new Vector2(xo + (tp * 7), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true){
                colorLine = p4Color*2.4f,
                colorEdge = p4Color*2.8f,
                min = this.buildDiv,
                max = 0
            };*/

            bool catBeat = rainworld.progression.miscProgressionData.redUnlocked;
            base.Initialize();
            OpTab mainTab = new(this, "Main");
            OpTab buildTab = new(this, "Builds");
            OpTab gimmickTab = new(this, "Gimmicks");
            OpTab accessibilityTab = new(this, "Accessibility");
            this.Tabs = new OpTab[]{
                mainTab,
                buildTab,
                gimmickTab,
                accessibilityTab
            };


            this.mainSet = new UIelement[]{
                new OpLabel(xo, yo, "Options", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), Swapper("These options change Escort's main abilities and how they play.<LINE>You can find the easy mode and builds in BUILDS and additional funny shenanigans in GIMMICKS")){
                    color = descColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Enable (Extra) Mean Lizards"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgMeanLizards, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    description = Translate("When enabled, lizards will spawn at full aggression when playing Escort (and target Escort upon sight)... the behaviour may not carry over in coop sessions. (Default=false)"),
                    colorEdge = tempColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 3) + tp/2, "Enable Vengeful Lizards [Beta]"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgVengefulLizards, new Vector2(xo + (xp * 0), yo - (yp * 3))){
                    description = Translate("Upon killing too many lizards, lizards may begin hunting Escort down. (Default=false)"),
                    colorEdge = tempColor
                },

                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 4), "Lifting Power Multiplier"),
                new OpUpdown(this.cfgHeavyLift, new Vector2(xo + (xp * 0), yo - (yp * 4) - tp), 100, 2){
                    description = Translate("Determines how heavy (X times Escort's weight) of an object Escort can carry before being burdened. (Default=3.0, Normal=0.6)")
                },
                new OpLabel(xo + (xp * 3) + 7f, yo - (yp * 5), "Drop Kick Multiplier"),
                new OpUpdown(this.cfgDKMult, new Vector2(xo + (xp * 0), yo - (yp * 5) - tp), 100, 2){
                    description = Translate("How much knockback does Escort's drop kick cause? Keep in mind it only affects creatures that aren't resiliant to knockback. (Default=3.0)"),
                },

                new OpLabel(xo + (xp * 0) + 7f, yo - (yp * 6) - tp, "Battle-Hype Mechanic"),
                //hypableBtn, hypedSlide,
                this.hypeableBox,
                this.hypeableTick,
                this.hypeableText[0], this.hypeableText[1], this.hypeableText[2], this.hypeableText[3], this.hypeableText[4], this.hypeableText[5], this.hypeableText[6],
                //new OpLabel(440f + xp - 7, yo - (yp * 7), swapper("0=Always on<LINE>1=50% tiredness<LINE>2=66% tiredness<LINE>3=75% tiredness<LINE>4=80% tiredness<LINE>5=87% tiredness<LINE>6=92% tiredness")),

                new OpLabel(xo + (xp * 1), yo - (yp * 8) + tp/2, "Long Wall Jump"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgLongWallJump, new Vector2(xo + (xp * 0), yo - (yp * 8))){
                    description = Translate("Allows Escort to do long jumps (hold jump) but on walls as well. May affect how normal wall jumping feels. (Default=false)"),
                    colorEdge = tempColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 9) + tp/2, "Flippy Pounce"),
                new OpCheckBox(this.cfgPounce, new Vector2(xo + (xp * 0), yo - (yp * 9))){
                    description = Translate("Causes Escort to do a (sick) flip with long jumps, and allows Escort to do a super-wall-flip-jumpTM when holding diagonal against a wall and pressing jump to achieve great vertical height. (Default=true)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 10) + tp/2, "Grab Stunned Lizards"),
                new OpCheckBox(this.cfgDunkin, new Vector2(xo + (xp * 0), yo - (yp * 10))){
                    description = Translate("Allows Escort to grab stunned lizards and cause violence on them. (Default=true)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 11) + tp/2, "Super Spear"),
                new OpCheckBox(this.cfgSpears, new Vector2(xo + (xp * 0), yo - (yp * 11))){
                    description = Translate("Does additional movement tech when throwing spears. (Default=true)")
                }
            };

            this.buildTitle = new OpLabel[]{
                new OpLabel(xo + (xp * 2), yo - (yp * 2.5f) - (tp * 1.3f), "Default {***__}", true){
                    color = bDefault * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 3.5f) - (tp * 1.3f), "Brawler {***__}", true){
                    color = bBrawler * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 4.5f) - (tp * 1.3f), "Deflector {*****}", true){
                    color = bDeflector * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 5.5f) - (tp * 1.3f), "Escapist {*____}", true){
                    color = bEscapist * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 6.5f) - (tp * 1.3f), "Railgunner {****_}", true){
                    color = bRailgunner * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 7.5f) - (tp * 1.3f), "Speedster {**___}", true){
                    color = bSpeedster * 0.75f
                },
                new OpLabel(xo + (xp * 2), yo - (yp * 8.5f) - (tp * 1.3f), "Gilded {Coming Soon!}", true){
                    color = bGilded * 0.75f
                },
            };
            this.buildText = new UIelement[]{
                new OpLabel(xo + (xp * 2), yo - (yp * 2.5f) - (tp * 2.1f), "  The intended way of playing Escort."){
                    color = bDefault
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 3.5f) - (tp * 2.1f), "  More powerful and consistent close-combat, but reduced range efficiency."){
                    color = bBrawler
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 4.5f) - (tp * 2.1f), "  Easier to parry, with empowered damage on success!... at the cost of some base stats."){
                    color = bDeflector
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 5.5f) - (tp * 2.1f), "  Force out of grasps, though don't expect to be fighting much..."){
                    color = bEscapist
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 6.5f) - (tp * 2.1f), Swapper("  With the aid of <REPLACE>, dual-wield for extreme results!", catBeat? "the void" : "mysterious forces")){
                    color = bRailgunner
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 7.5f) - (tp * 2.1f), "  Sometimes you just gotta hit the bricks... and sometimes you just gotta go fast."){
                    color = bSpeedster
                },

                new OpLabel(xo + (xp * 2), yo - (yp * 8.5f) - (tp * 2.1f), Swapper("  Closer to attunement like he wanted, grasp the powers of <REPLACE>.", catBeat? "the guardians" : "mysterious entities")){
                    color = bGilded
                },
            };
            this.buildShadow = new UIelement[this.buildText.Length];
            for (int i = 0; i < this.buildShadow.Length; i++)
            {
                this.buildShadow[i] = new OpLabel(this.buildText[i].PosX - 1f, this.buildText[i].PosY, this.buildText[i].description)
                {
                    color = bShadow
                };
            }
            // Checkbox/buildtick and it's fancy functions
            for (int j = 0; j < this.buildEasy.Length; j++)
            {
                this.buildEasy[j] = new OpCheckBox(
                    /*
                    j switch
                    {
                        0 => this.cfgEasyP1,
                        1 => this.cfgEasyP2,
                        2 => this.cfgEasyP3,
                        _ => this.cfgEasyP4
                    }*/
                    this.cfgEasy[j]
                    , new Vector2(xoffset - (tpadding * (5 - 4 * j)) + 3f, yoffset - 3f - (ypadding * 2))
                )
                {
                    description = OptionInterface.Translate(j switch
                    {
                        0 => "[P1] ",
                        1 => "[P2] ",
                        2 => "[P3] ",
                        _ => "[P4] "
                    } + "Enable the ability to dropkick by pressing Jump + Grab while midair."),
                    colorEdge = easyColor,
                    colorFill = j switch
                    {
                        0 => p1Color * 0.45f,
                        1 => p2Color * 0.45f,
                        2 => p3Color * 0.7f,
                        _ => p4Color
                    },
                };
                // this.buildEasy[j].SetValueBool(this.jollyEasierState[j]);
                // this.buildEasy[j].SetValueBool(
                //     j switch {
                //         0 => this.cfgEasyP1.Value,
                //         1 => this.cfgEasyP2.Value,
                //         2 => this.cfgEasyP3.Value,
                //         _ => this.cfgEasyP4.Value
                //     }
                // );
                // try {
                //     this.buildEasy.SetValue(j switch {
                //         0 => this.cfgEasyP1.Value,
                //         1 => this.cfgEasyP2.Value,
                //         2 => this.cfgEasyP3.Value,
                //         _ => this.cfgEasyP4.Value
                //     }, j);
                // } catch (Exception err){
                //     Ebug(err, "Oh no not my remix menu");
                // }
                    /*
                this.buildPlayer[j] = new OpSliderTick(
                    j switch
                    {
                        0 => this.cfgBuildP1,
                        1 => this.cfgBuildP2,
                        2 => this.cfgBuildP3,
                        _ => this.cfgBuildP4
                    }, new Vector2((xo - (tp * (5 - 4 * j))), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true
                )
                {
                    value = j switch
                    {
                        0 => ValueConverter.ConvertToString(this.cfgBuildP1.Value, this.cfgBuildP1.settingType),
                        1 => ValueConverter.ConvertToString(this.cfgBuildP2.Value, this.cfgBuildP2.settingType),
                        2 => ValueConverter.ConvertToString(this.cfgBuildP3.Value, this.cfgBuildP3.settingType),
                        _ => ValueConverter.ConvertToString(this.cfgBuildP4.Value, this.cfgBuildP4.settingType)
                    },
                };
                    */
                this.buildPlayer[j] = new OpSliderTick(this.cfgBuild[j], new Vector2((xo - (tp * (5 - 4 * j))), (yo + tp) - (yp * 2.5f) + (yp * buildDiv)), (int)(yp * -buildDiv), true);
                (this.buildPlayer[j].colorLine, this.buildPlayer[j].colorEdge) = j switch
                {
                    0 => (p1Color * 0.8f, p1Color * 0.9f),
                    1 => (p2Color * 0.8f, p2Color * 0.9f),
                    2 => (p3Color * 0.9f, p3Color),
                    _ => (p4Color * 2.4f, p4Color * 2.8f)
                };
                this.buildEasy[j].OnValueChanged += (UIconfig config, string value, string oldValue) =>
                {
                    int target = -1;
                    for (int s = 0; s < this.buildEasy.Length; s++)
                    {
                        if (this.buildEasy[s].cfgEntry.BoundUIconfig == config)
                        {
                            target = s;
                            break;
                        }
                    }
                    if (target == -1)
                    {
                        Ebug("Config index not found!");
                        return;
                    }
                    this.buildPlayer[target].colorFill = value == "true" ? easyColor * 0.5f : Menu.MenuColorEffect.rgbBlack;
                };
                /*
                this.buildPlayer[j].OnValueUpdate += (UIconfig config, string value, string oldValue) =>
                {  // Only starts applying once the slider is moved
                    int target = -1;
                    for (int s = 0; s < this.buildPlayer.Length; s++)
                    {
                        if (this.buildPlayer[s].cfgEntry.BoundUIconfig == config)
                        {
                            target = s;
                            break;
                        }
                    }
                    if (target == -1)
                    {
                        Ebug("Config index not found!");
                        return;
                    }
                    if (this.buildPlayer.Length < 4)
                    {
                        Ebug("Index error?! " + this.buildPlayer.Length + "  Target: " + target);
                        return;
                    }
                    DoTheBuildColorThing(target, value);
                };

                this.buildPlayer[j].OnHeld += (bool isHeld) =>
                {  // Only applies on selecting/deselecting the slider
                    if (!isHeld)
                    {
                        for (int m = 0; m < this.buildTitle.Length; m++)
                        {
                            (this.buildTitle[m] as OpLabel).color = this.buildColors[m] * 0.75f;
                            (this.buildText[m] as OpLabel).color = this.buildColors[m];
                        }
                    }
                    else
                    {
                        int target = -1;
                        for (int s = 0; s < this.buildPlayer.Length; s++)
                        {
                            if (this.buildPlayer[s].held)
                            {
                                target = s;
                                break;
                            }
                        }
                        if (target == -1)
                        {
                            Ebug("Config index not found!");
                            return;
                        }
                        if (this.buildPlayer.Length < 4)
                        {
                            Ebug("Index error?! " + this.buildPlayer.Length + "  Target: " + target);
                            return;
                        }
                        DoTheBuildColorThing(target, this.buildPlayer[target].value);
                    }
                };
                */
            }
            this.buildSet = new UIelement[]{
                new OpLabel(xo, yo, "Builds", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), Swapper("These will change hidden values in certain ways to make Escort play differently!<LINE>Try each build out to see which one you vibe to the most.")){
                    color = descColor
                },

                new OpLabel(xo + (xp * 2) + (tp * 1.5f), yo - 3f - (yp * 2) + tp/2, "Easier Mode"){
                    color = easyColor
                },
                this.buildEasy[0],
                this.buildEasy[1],
                this.buildEasy[2],
                this.buildEasy[3],

                new OpLabel(xo - (tp * 3.8f), yo + 3f - (yp * 1.5f), "(1)   (2)   (3)   (4) <-PLAYER #"){
                    color = new Color(0.5f, 0.5f, 0.5f)
                },

                // Sliders
                this.buildPlayer[0],
                this.buildPlayer[1],
                this.buildPlayer[2],
                this.buildPlayer[3],
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
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 5) + tp/2, "Super-Extended Deflector Slide"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgFunnyDeflSlide, new Vector2(xo + (xp * 0), yo - (yp * 5))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Makes Deflector Escort's slides last much longer. (Default=false)"),
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 6) + tp/2, "Pole Tech."){
                    color = tempColor
                },
                new OpCheckBox(this.cfgPoleBounce, new Vector2(xo + (xp * 0), yo - (yp * 6))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Because someone requested it. Not quite Rivulet, but something along the lines ;). (Default=false)"),
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 7) + tp/2, "Old Speedster Mechanics"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgOldSpeedster, new Vector2(xo + (xp * 0), yo - (yp * 7))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Reverts Speedster Escort Mechanics to before the revamp. (Default=false)"),
                },


                secretText
            };
            this.accessibleSet = new UIelement[]{
                new OpLabel(xo, yo, "Accessibility", true),
                new OpLabelLong(new Vector2(xo, yo - (yp * 2)), new Vector2(500f, yp * 2), "Change how some visual effects are displayed for easier understanding of what's going on."){
                    color = descColor
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 2) + tp/2, "Stronger Battle-hyped Indicator"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgNoticeHype, new Vector2(xo + (xp * 0), yo - (yp * 2))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Adds a visual effect to show more clearly whether Escort is in the battle-hyped state or not. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 3) + tp/2, "Different Empowered VFX for Deflector Build"){
                    color = tempColor
                },
                new OpCheckBox(this.cfgNoticeEmpower, new Vector2(xo + (xp * 0), yo - (yp * 3))){
                    colorEdge = tempColor,
                    description = OptionInterface.Translate("Changes the empowered vfx for Deflector Escort from sparks to pulsing circle. (Default=false)")
                },

                new OpLabel(xo + (xp * 1), yo - (yp * 4) + tp/2, "Keep Track of All Methods (for modders)"){
                    color = tempColor * 0.75f
                },
                new OpCheckBox(this.cfgDeveloperMode, new Vector2(xo + (xp * 0), yo - (yp * 4))){
                    colorEdge = tempColor * 0.75f,
                    description = OptionInterface.Translate("For other modders: Also keep track of frequently updated methods (May cause a bit of lag). (Default=false)"),
                    greyedOut = true
                },
            };
            mainTab.AddItems(this.mainSet);
            buildTab.AddItems(this.buildSet);
            buildTab.AddItems(this.buildTitle);
            buildTab.AddItems(this.buildShadow);
            buildTab.AddItems(this.buildText);
            gimmickTab.AddItems(this.gimmickSet);
            accessibilityTab.AddItems(this.accessibleSet);
            if (cfgVersion.Value != VERSION){
                ConfigConnector.CreateDialogBoxNotify(HelloWorld);
                cfgVersion.Value = VERSION;
            }
        }

        /*
        public override void Update()
        {
            this.cfgDeveloperMode.OnChange += longDevLogChange;
            base.Update();
        }*/

        private void LongDevLogChange()
        {
            if (this.cfgDeveloperMode.Value)
            {
                Plugin.ins.L().TurnOnLog();
            }
            else
            {
                Plugin.ins.L().TurnOffLog();
            }
        }
        /*
                private void inputSecret()
                {
                    int num = (int)this.yoffset * (int)this.tpadding - ((int)this.xoffset / 2) * (int)this.ypadding + ((int)this.tpadding - 1) * ((int)this.xoffset + (int)this.xpadding) + 33;
                    string[] insult = new string[1];
                    Action[] doThing = new Action[1]{
                        makeSomeNoiseEsconfig
                    };
                    insult[0] = "Ur not my mum.";
                    switch(UnityEngine.Random.Range(0, 5)){
                        case 1: insult[0] = "F#@k off."; break;
                        case 2: insult[0] = "Skill issue."; break;
                        case 3: insult[0] = "I don't care."; break;
                        case 4: insult[0] = "Shut the f$&k up."; break;
                    }
                    if (this.cfgSecret.Value == num){
                        if (!this.cfgSectret.Value){
                            this.cfgSectret.Value = true;
                            ConfigConnector.CreateDialogBoxMultibutton(
                                swapper(
                                    "     ...though never intent...     <LINE> ...the pup escapes containment... <LINE>  ...careful out there, yeah?...   "
                                ), insult, doThing
                            );
                        }
                        Plugin.ins.L().christmas(this.cfgSectret.Value);
                    }
                    else {
                        this.cfgSectret.Value = false;
                        Plugin.ins.L().christmas();
                        try{
                            if (Plugin.Esconfig_SFX_Sectret != null){
                                ConfigContainer.PlaySound(Plugin.Esconfig_SFX_Sectret);
                            }
                        } catch (Exception err){
                            Debug.LogError("Couldn't play sound!");
                            Debug.LogException(err);
                        }
                    }
                }
        */
        private void InputTheSecret(UIconfig config, string value, string oldValue)
        {
            int num = (int)this.yoffset * (int)this.tpadding - ((int)this.xoffset / 2) * (int)this.ypadding + ((int)this.tpadding - 1) * ((int)this.xoffset + (int)this.xpadding) + 33;
            string notNum = num.ToString();
            string[] insult = new string[1];
            Action[] doThing = new Action[1]{
                MakeSomeNoiseEsconfig
            };
            insult[0] = "Ur not my mum.";
            switch (UnityEngine.Random.Range(0, 5))
            {
                case 1: insult[0] = "F#@k off."; break;
                case 2: insult[0] = "Skill issue."; break;
                case 3: insult[0] = "I don't care."; break;
                case 4: insult[0] = "Shut the f$&k up."; break;
            }
            if (value == notNum)
            {
                if (!this.cfgSectret.Value && this.cfgSecret.Value != num)
                {
                    this.cfgSectret.Value = true;
                    ConfigConnector.CreateDialogBoxMultibutton(
                        Swapper(
                            "     ...though never intent...     <LINE> ...the pup escapes containment... <LINE>  ...careful out there, yeah?...   "
                        ), insult, doThing
                    );
                }
                Plugin.ins.L().Christmas(this.cfgSectret.Value);
            }
            else
            {
                this.cfgSectret.Value = false;
                Plugin.ins.L().Christmas();
                try
                {
                    if (Plugin.Esconfig_SFX_Sectret != null)
                    {
                        ConfigContainer.PlaySound(Plugin.Esconfig_SFX_Sectret);
                    }
                }
                catch (Exception err)
                {
                    Debug.LogError("Couldn't play sound!");
                    Debug.LogException(err);
                }
            }
        }



        private void MakeSomeNoiseEsconfig()
        {
            ConfigContainer.PlaySound(SoundID.MENU_Next_Slugcat, 0, 1, 0.6f);
        }

        private void ToggleDisableHyped(UIconfig config, string value, string oldValue)
        {
            if (value == "true")
            {
                int.TryParse(this.hypeableTick.value, out int i);
                this.hypeableTick.greyedOut = false;
                for (int j = 0; j < this.hypeableText.Length; j++)
                {
                    if (i == j)
                    {
                        this.hypeableText[j].color = Menu.MenuColorEffect.rgbMediumGrey;
                    }
                    else
                    {
                        this.hypeableText[j].color = Menu.MenuColorEffect.rgbDarkGrey;
                    }
                }
            }
            else
            {
                this.hypeableTick.greyedOut = true;
                foreach (OpLabel l in this.hypeableText)
                {
                    l.color = Menu.MenuColorEffect.rgbVeryDarkGrey;
                }
            }
        }

        private void RunItThruHyped(UIconfig config, string value, string oldValue)
        {
            int.TryParse(value, out int n);
            int.TryParse(oldValue, out int o);
            this.hypeableText[n].color = Menu.MenuColorEffect.rgbMediumGrey;
            this.hypeableText[o].color = Menu.MenuColorEffect.rgbDarkGrey;
        }

        private void DoTheBuildColorThing(int index, string value)
        {
            if (!int.TryParse(value, out int r))
            {
                Ebug("Couldn't get value of Player Build!", 0);
                return;
            }
            try
            {
                for (int k = 0; k < this.buildTitle.Length; k++)
                {
                    if (k == -r)
                    {
                        (this.buildTitle[k] as OpLabel).color = index switch
                        {
                            0 => this.buildColors[k] * (this.p1Color * 0.8f),
                            1 => this.buildColors[k] * (this.p2Color * 0.8f),
                            2 => this.buildColors[k] * (this.p3Color * 0.9f),
                            _ => this.buildColors[k] * (this.p4Color * 2.4f)
                        };
                        (this.buildText[k] as OpLabel).color = this.buildColors[k];
                    }
                    else
                    {
                        (this.buildTitle[k] as OpLabel).color = Menu.MenuColorEffect.rgbDarkGrey;
                        (this.buildText[k] as OpLabel).color = Menu.MenuColorEffect.rgbMediumGrey;
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Error on Focus Gain");
            }
        }
    }
}