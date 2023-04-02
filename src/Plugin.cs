using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[WIP] The Escort", "0.1.9.17")]
    class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public EscOptions config;
        private const string MOD_ID = "urufudoggo.theescort";
        public Plugin(){
            try{
                this.config = new EscOptions(this, base.Logger);
                Plugin.instance = this;
            } catch (Exception e){
                base.Logger.LogError(e);
            }
        }


        public static readonly PlayerFeature<bool> pRTEdits = PlayerBool("playescort/realtime_edits");
        public static readonly GameFeature<bool> gRTEdits = GameBool("gameescort/realtime_edits");
        public static readonly PlayerFeature<bool> BtrPounce = PlayerBool("theescort/better_pounce");
        public static readonly GameFeature<bool> SupahMeanLizards = GameBool("theescort/mean_lizards");
        public static readonly GameFeature<bool> SuperMeanGarbageWorms = GameBool("theescort/mean_garb_worms");

        /* JSON VALUES
        ["Unhyped speed", "Hyped speed", "Malnourished unhyped", "Malnourished hyped", "Malnourished unhyped", "hype-disabled", "Malnourished hype-disabled"]
        */
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");

        /* JSON VALUES
        ["Unhyped speed", "Hyped speed", "Malnourished unhyped", "Malnourished hyped", "Malnourished unhyped", "hype-disabled", "Malnourished hype-disabled"]
        */
        public static readonly PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");

        /* JSON VALUES
        ["Stun Slide damage", "Stun Slide base stun duration", "Drop Kick base damage", "Drop Kick stun duration"]
        */
        public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static readonly PlayerFeature<float> LiftHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DKM = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static readonly PlayerFeature<int> Escomet = PlayerInt("theescort/headbutt");
        public static readonly PlayerFeature<bool> Elvator = PlayerBool("theescort/elevator");
        public static readonly PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");
        public static readonly PlayerFeature<bool> HypeSys = PlayerBool("theescort/adrenaline_system");
        public static readonly PlayerFeature<float> HypeReq = PlayerFloat("theescort/stamina_req");
        public static readonly PlayerFeature<int> RR = PlayerInt("theescort/reset_rate");

        /* JSON VALUES
        ["Hyped spear damage", "Base spear damage"]
        */
        public static readonly PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");
        public static readonly PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");
        public static readonly PlayerFeature<bool> soundAhoy = PlayerBool("theescort/sounds_ahoy");

        /* JSON VALUES
        ["Min swim power (affected by viscosity of water)", "Max swim power", "deepswim X velocity", "deepswim Y velocity", "surfaceswim X velocity", "surfaceswim Y velocity"]
        */
        public static readonly PlayerFeature<float[]> NoMoreGutterWater = PlayerFloats("theescort/guuh_wuuh");
        public static readonly PlayerFeature<bool> LWallJump = PlayerBool("theescort/long_wall_jump");

        /* JSON VALUES
        ["Head Y velocity", "Body Y velocity", "Head X velocity", "Body X velocity", "ConstantDownDiagnoal floor value", "ConstantDownDiagonal ceiling value", "min JumpBoost", "max JumpBoost"]
        */
        public static readonly PlayerFeature<float[]>
        WallJumpVal = PlayerFloats("theescort/wall_jump_val");

        public static SoundID Escort_SFX_Death;
        public static SoundID Escort_SFX_Flip;
        public static SoundID Escort_SFX_Roll;
        //public static SoundID Escort_SFX_Spawn;

        //public DynamicSoundLoop escortRollin;
        public Escort e;

        private int slowDownDevConsole = 0;
        private float requirement;
        private float DKMultiplier;
        float ratioed;


        // Patches
        private bool escPatch_revivify = false;
        //private bool escPatch_emeraldTweaks = false;


        private static void Ebug(String message){
            Debug.Log("-> Escort: " + message);
        }
        private static void Ebug(System.Object message){
            Debug.Log("-> Escort: " + message.ToString());
        }
        private static void Ebug(String[] messages){
            String message = "";
            foreach(String msg in messages){
                message += ", " + msg;
            }
            Debug.Log("-> Escort: " + message);
        }
        private static void Ebug(System.Object[] messages){
            String message = "";
            foreach(System.Object[] msg in messages){
                message += ", " + msg.ToString();
            }
            Debug.Log("-> Escort: " + message);
        }


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.RainWorld.PostModsInit += new On.RainWorld.hook_PostModsInit(this.Escort_PostInit);

            On.Lizard.ctor += new On.Lizard.hook_ctor(this.Escort_Lizard_ctor);

            On.Player.Jump += new On.Player.hook_Jump(this.Escort_Jump);
            On.Player.UpdateBodyMode += new On.Player.hook_UpdateBodyMode(this.Escort_UpdateBodyMode);
            On.Player.UpdateAnimation += new On.Player.hook_UpdateAnimation(this.Escort_UpdateAnimation);
            On.Player.Collide += new On.Player.hook_Collide(this.Escort_Collision);
            On.Player.HeavyCarry += new On.Player.hook_HeavyCarry(this.Escort_HeavyCarry);
            On.Player.AerobicIncrease += Escort_AerobicIncrease;
            On.Creature.Violence += new On.Creature.hook_Violence(this.Escort_Violence);
            On.Player.Update += new On.Player.hook_Update(this.Escort_Update);
            On.Player.ThrownSpear += new On.Player.hook_ThrownSpear(this.Escort_ThrownSpear);
            On.Player.Grabability += new On.Player.hook_Grabability(this.Escort_Grabability);
            On.Player.Die += new On.Player.hook_Die(this.Escort_Die);
            On.Player.WallJump += new On.Player.hook_WallJump(this.Escort_WallJump);
            On.Player.MovementUpdate += new On.Player.hook_MovementUpdate(this.Escort_MovementUpdate);
            On.Player.checkInput += new On.Player.hook_checkInput(this.Escort_checkInput);
            On.Player.ctor += new On.Player.hook_ctor(this.Escort_ctor);
            On.Player.DeathByBiteMultiplier += new On.Player.hook_DeathByBiteMultiplier(this.Escort_DeathBiteMult);
            On.Player.TossObject += new On.Player.hook_TossObject(this.Escort_TossObject);
            On.Player.SpearStick += new On.Player.hook_SpearStick(this.Escort_StickySpear);

            On.SlugcatStats.SpearSpawnModifier += Escort_SpearSpawnMod;
            On.SlugcatStats.SpearSpawnElectricRandomChance += Escort_EleSpearSpawnChance;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance += Escort_ExpSpearSpawnChance;
            On.SlugcatStats.getSlugcatStoryRegions += Escort_getStoryRegions;
            }




        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Escort_SFX_Death = new SoundID("Escort_Failure", true);
            Escort_SFX_Flip = new SoundID("Escort_Flip", true);
            Escort_SFX_Roll = new SoundID("Escort_Roll", true);
            //Escort_SFX_Spawn = new SoundID("Escort_Spawn", true);
            Ebug("All SFX loaded!");
            EscEnums.RegisterValues();
            MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            Ebug("All loaded!");
        }


        private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self){
            orig(self);
            try{
            if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify")){
                Ebug("Found Revivify! Applying patch...");
                escPatch_revivify = true;
            }
            } catch (Exception e){
                throw new Exception(e.Message);
            }
        }


        

        /*
        Configurations!
        */
        private bool Esconfig_Mean_Lizards(World self){
            if (!gRTEdits.TryGet(self.game, out bool RT) || !SupahMeanLizards.TryGet(self.game, out bool meanLizard)){
                return false;
            }
            if (RT){
                return meanLizard;
            } else {
                return config.cfgMeanLizards.Value;
            }
        }

        private bool Esconfig_Heavylift(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !LiftHeavy.TryGet(self, out float power)){
                ratioed = 3f;
                return false;
            }
            if (RT){
                ratioed = power;
            } else {
                ratioed = config.cfgHeavyLift.Value;
            }
            return true;
        }

        private bool Esconfig_DKMulti(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !DKM.TryGet(self, out float dk)){
                return false;
            }
            if (RT){
                DKMultiplier = dk;
            } else {
                DKMultiplier = config.cfgDKMult.Value;
            }
            return true;
        }

        private bool Esconfig_Elevator(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !Elvator.TryGet(self, out bool yeet)){
                return false;
            }
            if (RT){
                return yeet;
            } else {
                return config.cfgElevator.Value;
            }
        }

        private bool Esconfig_Hypable(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !HypeSys.TryGet(self, out bool hm)){
                return false;
            }
            if (RT){
                return hm;
            } else {
                return config.cfgHypable.Value;
            }
        }

        private bool Esconfig_HypeReq(Player self, float require=0.8f){
            if (!pRTEdits.TryGet(self, out bool RT) || !HypeReq.TryGet(self, out float req)){
                return false;
            }
            if (RT){
                requirement = req;
            } else {
                switch(config.cfgHypeReq.Value){
                    case 0:
                        requirement = -1f; break;
                    case 1:
                        requirement = 0.5f; break;
                    case 2:
                        requirement = 0.66f; break;
                    case 3:
                        requirement = 0.75f; break;
                    case 4:
                        requirement = 0.8f; break;
                    case 5:
                        requirement = 0.87f; break;
                    case 6:
                        requirement = 0.92f; break;
                    default:
                        requirement = require; break;
                };
            }
            return true;
        }

        private bool Esconfig_SFX(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !soundAhoy.TryGet(self, out bool soundFX)){
                return false;
            }
            if (RT){
                return soundFX;
            } else {
                return config.cfgSFX.Value;
            }
        }

        private bool Esconfig_WallJumps(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !LWallJump.TryGet(self, out bool wallJumper)){
                return false;
            }
            if (RT){
                return wallJumper;
            } else {
                return config.cfgLongWallJump.Value;
            }
        }

        private bool Esconfig_Pouncing(Player self){
            if (!pRTEdits.TryGet(self, out bool RT) || !BtrPounce.TryGet(self, out bool pouncing)){
                return false;
            }
            if (RT){
                return pouncing;
            } else {
                return config.cfgPounce.Value;
            }
        }

        private bool Esconfig_Dunkin(Player self){
            return config.cfgDunkin.Value;
        }

        private bool Esconfig_Build(Player self){
            try {
                switch (config.cfgBuildNum.Value){
                    case -1:  // Brawler build
                        e.combatTech = false;
                        self.slugcatStats.runspeedFac += 0.1f;
                        break;
                    default:  // Default build
                        break;
                }
                Ebug("Set build complete!");
                return true;
            } catch (Exception e){
                Ebug("Something went wrong when setting an Escort build!");
                Ebug(e.Message);
                return false;
            }
        }


        /*
        Miscellaneous!
        */
        private void Update_Refresher(Player self){
            if (RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                slowDownDevConsole = 0;
            } else {
                slowDownDevConsole++;
            }
        }

        private void Tick_Escort(Player self){
            // Dropkick damage cooldown
            if (e.DropKickCD > 0){
                e.DropKickCD--;
            }

            // Get out of centipede grasp cooldown
            // TODO: IMPLEMENT
            if (e.CentiCD > 0){
                e.CentiCD--;
            }

            // Parry leniency when triggering stunslide first (may not actually occur)
            if (e.parryLean > 0){
                e.parryLean--;
            }

            // Headbutt cooldown
            if (e.CometFrames > 0){
                e.CometFrames--;
            } else {
                e.Cometted = false;
            }

            // Invincibility Frames
            if (e.iFrames > 0){
                self.abstractCreature.creatureTemplate.baseDamageResistance = 2f;
                if (e.ElectroParry) {
                    self.abstractCreature.creatureTemplate.baseStunResistance = 3f;                
                }
                Ebug("IFrames: " + e.iFrames);
                e.iFrames--;
            } else {
                self.abstractCreature.creatureTemplate.baseDamageResistance = 1f;
                self.abstractCreature.creatureTemplate.baseStunResistance = 1f;
                e.ElectroParry = false;
            }
        }

        /*
        Escort code!
        */
        // Implement lizard aggression (edited from template)
        private void Escort_Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            
            orig(self, abstractCreature, world);

            if(Esconfig_Mean_Lizards(world))
            {
                Ebug("Lizard Ctor Triggered!");
                self.spawnDataEvil = Mathf.Max(self.spawnDataEvil, 100f);
            }
        }

        private void Escort_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            Ebug("Ctor Triggered!");
            orig(self, abstractCreature, world);
            if (self.slugcatStats.name.value == "EscortMe"){
                e = new Escort(self);
                try {
                    e.Escort_set_roller(Escort_SFX_Roll);
                    Ebug("Setting roll sound");
                    Esconfig_Build(self);
                    // April fools!
                    //self.setPupStatus(set: true);
                    //self.room.PlaySound(Escort_SFX_Spawn, self.mainBodyChunk);
                } catch (Exception e){
                    throw new Exception(e.Message);
                } finally {
                    Ebug("All ctor'd");
                }
            }
        }

        // Implement Escort's slowed stamina increase
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            if (!Exhausion.TryGet(self, out float exhaust)){
                orig(self, f);
                return;
            }
            if (self.slugcatStats.name.value == "EscortMe"){
                // Due to the aerobic decrease found in some movements implemented in Escort, the AerobicIncrease actually does the original, and on top of that the additional to balance things out.
                orig(self, f);

                //Ebug("Aerobic Increase Triggered!");
                if (!self.slugcatStats.malnourished){
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / exhaust));
                } else {
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / (exhaust / 2)));
                }
            } else {
                orig(self, f);
            }
        }

        // Implement visual effect for Battle-Hyped mode
        private void Escort_Update(On.Player.orig_Update orig, Player self, bool eu){
            orig(self, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }

            if (
                !RR.TryGet(self, out int limiter) ||
                !WallJumpVal.TryGet(self, out var WJV) ||
                !NoMoreGutterWater.TryGet(self, out var theGut)
                ){
                return;
            }

            // For slowed down dev console output
            Update_Refresher(self);

            // Cooldown/Frames Tick
            Tick_Escort(self);
            
            // Just for seeing what a variable does.
            if(limiter < slowDownDevConsole){
                Ebug("Clocked.");
                Ebug(" Roll Direction: " + self.rollDirection);
                Ebug("Slide Direction:" + self.slideDirection);
                Ebug(" Flip Direction: " + self.flipDirection);
                Ebug(self.abstractCreature.creatureTemplate.baseDamageResistance);;
                //Ebug("Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                //Ebug("Normalized direction: " + self.bodyChunks[0].vel.normalized);
            }

            // vfx
            if(self != null && self.room != null){
                // Battle-hyped visual effect
                if (Esconfig_Hypable(self) && Esconfig_HypeReq(self) && self.aerobicLevel > requirement){
                    Color hypedColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    hypedColor.a = 0.8f;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, hypedColor));
                }

                // Charged pounces Visual Effect
                if (Esconfig_Pouncing(self)){
                    Color pounceColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);

                    if (self.superLaunchJump > 19){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 9f, 4f, 4f, 11f, pounceColor));
                    }
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.consistentDownDiagonal >= (int)WJV[4]){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, pounceColor));
                    }
                }
            }

            // Implement guuh wuuh
            if(self.bodyMode == Player.BodyModeIndex.Swimming){
                float superSwim = Mathf.Lerp(theGut[0], theGut[1], self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity));
                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[2] * superSwim, theGut[3] * superSwim);
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[4] * superSwim, theGut[5] * superSwim);
                }
            }

            // Implement rolling SFX
            if (Esconfig_SFX(self)){
                if (self.animation == Player.AnimationIndex.Roll){
                    e.Rollin.Update();
                } else {
                    e.RollinCount = 0f;
                }
            }
        }

        // Implement Flip jump and less tired from jumping
        private void Escort_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }

            Ebug("Jump Triggered!");
            // Decreases aerobiclevel gained from jumping
            if (self.aerobicLevel > 0.1f){
                self.aerobicLevel -= 0.1f;
            }

            // Replace chargepounce with a sick flip
            if (
                Esconfig_Pouncing(self) && 
                (
                    self.superLaunchJump >= 19 || 
                    self.simulateHoldJumpButton == 6 || 
                    self.killSuperLaunchJumpCounter > 0
                    ) && 
                self.bodyMode == Player.BodyModeIndex.Crawl
                ){
                Ebug("FLIPERONI GO!");

                if (Esconfig_SFX(self)){
                    self.room.PlaySound(Escort_SFX_Flip, e.RollinSFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
            self.consistentDownDiagonal = 0;
        }

        private void Escort_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            if (self.bodyMode != Player.BodyModeIndex.WallClimb){
                orig(self, direction);
                return;
            }
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, direction);
                    return;
                }
            } catch (Exception err){
                orig(self, direction);
                Ebug(err.Message);
                return;
            }
            if (!WallJumpVal.TryGet(self, out var WJV)){
                orig(self, direction);
                return;
            }

            Ebug("Walljump Triggered!");
            bool wallJumper = Esconfig_WallJumps(self);
            bool longWallJump = (self.superLaunchJump > 19 && wallJumper);
            bool superWall = (Esconfig_Pouncing(self) && self.consistentDownDiagonal > (int)WJV[4]);
            bool superFlip = self.allowRoll == 15 && Esconfig_Pouncing(self);

            // If charge wall jump is enabled and is able to walljump, or if charge wall jump is disabled
            if ((wallJumper && self.canWallJump != 0) || !wallJumper) {
                orig(self, direction);
                float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                String[] toPrint = new String[3];
                toPrint.SetValue("Walls the Jump", 0);
                if (
                    self.IsTileSolid(1, 0, -1) ||
                    self.IsTileSolid(0, 0, -1) ||
                    self.bodyChunks[1].submersion > 0.1f ||
                    (
                        self.input[0].x != 0 && 
                        self.bodyChunks[0].ContactPoint.x == self.input[0].x &&
                        self.IsTileSolid(0, self.input[0].x, 0) &&
                        !self.IsTileSolid(0, self.input[0].x, 1)
                    )
                ){
                    self.bodyChunks[0].vel.y = 8f * n;
                    self.bodyChunks[1].vel.y = 7f * n;
                    self.bodyChunks[0].pos.y += 10f * Mathf.Min(1f, n);
                    self.bodyChunks[1].pos.y += 10f * Mathf.Min(1f, n);
                    toPrint.SetValue("Water", 1);
                    self.room.PlaySound(SoundID.Slugcat_Normal_Jump, e.RollinSFXChunk, false, 1f, 0.7f);
                } 
                else {
                    self.bodyChunks[0].vel.y = ((longWallJump || (superFlip && superWall))? WJV[0] : 8f) * n;
                    self.bodyChunks[1].vel.y = ((longWallJump || (superFlip && superWall))? WJV[1] : 7f) * n;
                    self.bodyChunks[0].vel.x = ((superFlip && superWall)? WJV[2] : 7f) * n * (float)direction;
                    self.bodyChunks[1].vel.x = ((superFlip && superWall)? WJV[3] : 6f) * n * (float)direction;
                    self.standing = true;
                    self.jumpStun = 8 * direction;
                    if (superWall){
                        self.room.PlaySound((self.superLaunchJump > 19? SoundID.Slugcat_Super_Jump : SoundID.Slugcat_Wall_Jump), e.RollinSFXChunk, false, 1f, 0.7f);
                    }
                    toPrint.SetValue("Not Water", 1);
                    Ebug("Y Velocity" + self.bodyChunks[0].vel.y);
                    Ebug("Y Velocity" + self.bodyChunks[1].vel.y);
                    Ebug("X Velocity" + self.bodyChunks[0].vel.x);
                    Ebug("X Velocity" + self.bodyChunks[1].vel.x);
                }
                self.jumpBoost = 0f;

                if (superFlip && superWall){
                    self.animation = Player.AnimationIndex.Flip;
                    self.room.PlaySound((Esconfig_SFX(self)? Escort_SFX_Flip : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.RollinSFXChunk, false, 1f, 0.9f);
                    self.jumpBoost += Mathf.Lerp(WJV[6], WJV[7], Mathf.InverseLerp(WJV[4], WJV[5], self.consistentDownDiagonal));
                    toPrint.SetValue("SUPERFLIP", 2);
                } else {
                    toPrint.SetValue("not so flip", 2);
                }
                Ebug("Jumpboost" + self.jumpBoost);
                Ebug("CDownDir" + self.consistentDownDiagonal);
                Ebug("SLaunchJump" + self.superLaunchJump);
                if (self.superLaunchJump > 19){
                    self.superLaunchJump = 0;
                }
                self.canWallJump = 0;
                Ebug(toPrint);
            }
        }

        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if (!Esconfig_WallJumps(self)){
                return;
            }
            if (!RR.TryGet(self, out int limiter)){
                return;
            }

            if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0){
                String msg = "Nothing New";
                self.canWallJump = 0;
                if (self.input[0].jmp){
                    msg = "Is touching the jump button";
                    if (self.superLaunchJump < 20){
                        self.superLaunchJump += 2;
                        if (self.Adrenaline == 1f && self.superLaunchJump < 6){
                            self.superLaunchJump = 6;
                        }
                    } else {
                        self.killSuperLaunchJumpCounter = 15;
                    }
                }

                if (!self.input[0].jmp && self.input[1].jmp){
                    msg = "Lets go of the jump button";
                    self.wantToJump = 1;
                }
                if(limiter < slowDownDevConsole){
                    Ebug(msg);
                }
            }
        }

        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
            } catch (Exception err){
                orig(self);
                Ebug(err.Message);
                return;
            }
            if(!Esconfig_WallJumps(self)){
                orig(self);
                return;
            }

            //Ebug("CheckInput Triggered!");
            int previously = self.input[0].x;
            orig(self);

            // Undoes the input cancellation
            if(self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1){
                if (self.input[0].x == 0){
                    self.input[0].x = previously;
                }
            }
        }

        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
            } catch (Exception err){
                Ebug(err.Message);
                return orig(self, obj);
            }
            if (!Esconfig_Heavylift(self)){
                return orig(self, obj);
            }
            if (!RR.TryGet(self, out int resetRate)){
                return orig(self, obj);
            }

            if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                if (slowDownDevConsole > resetRate){
                    Ebug("Revivify skip!");
                    Ebug("Creature: " + creature.GetType());
                    Ebug("Player: " + self.GetOwnerType());
                }
                return orig(self, creature);
            }

            //Ebug("Heavycarry Triggered!");
            if (obj.TotalMass <= self.TotalMass * ratioed){
                if (ModManager.CoopAvailable && obj is Player player && player != null){
                    return !player.isSlugpup;
                }
                return false;
            }
            return orig(self, obj);
        }

        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if (
                !BetterCrawl.TryGet(self, out var crawlSpeed) ||
                !BetterPoleWalk.TryGet(self, out var poleMove) ||
                !Escomet.TryGet(self, out int SetComet)
            ){
                return;
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Implement bettercrawl
            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) : crawlSpeed[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(crawlSpeed[2], crawlSpeed[3], self.aerobicLevel) : crawlSpeed[5]) * self.slugcatStats.runspeedFac;
            }

            // Implement betterpolewalk
            /*
            The hangfrombeam's speed does not get affected by dynamicrunspeed apparently so that's fun... 
            still the standonbeam works but also at the same time not as I initially thought.
            The slugcat apparently has a limit on how fast they can move on the beam while standing on it, leaning more and more foreward and getting more and more friction as a result...
            or to that degree.
            */
            else if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam && (self.animation == Player.AnimationIndex.StandOnBeam || self.animation == Player.AnimationIndex.HangFromBeam)){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(poleMove[0], poleMove[1], self.aerobicLevel): poleMove[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(poleMove[2], poleMove[3], self.aerobicLevel): poleMove[5]) * self.slugcatStats.runspeedFac;
            } 
            
            // Set headbutt condition
            else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb){
                if (self.slowMovementStun > 0){
                    e.CometFrames = SetComet;
                }
            }
        }

        // Implement Movementtech
        private void Escort_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if (!RR.TryGet(self, out int limiter)){
                return;
            }

            //Ebug("UpdateAnimation Triggered!");
            // Infiniroll
            if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                e.RollinCount++;
                if(limiter < slowDownDevConsole){
                    Ebug("Rollin at: " + e.RollinCount);
                }
                if(Esconfig_SFX(self) && e.Rollin != null){
                    e.Rollin.Volume = Mathf.InverseLerp(100f, 300f, e.RollinCount);
                }
                self.rollCounter = 0;
            }

            if (self.animation != Player.AnimationIndex.Roll){
                e.RollinCount = 0f;
            }

            // I'll find out how to implement a more lineant slide (like rivulet's slide pounces) while keeping it short (like every other slugcats) one day...
            //if (self.animation == Player.AnimationIndex.BellySlide){
                // TODO implement better slide
            //}
        }


        // Implement Parryslide/midair projectile grab
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){
            try{
                if (self is Player && (self as Player).slugcatStats.name.value != "EscortMe"){
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(self is not Player player){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(
                !ParrySlide.TryGet(player, out bool enableParry)
            ){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (!enableParry){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }



            Ebug("Violence Triggered!");
            // connects to the Escort's Parryslide option
            e.ParrySuccess = false;
            if ((player.animation == Player.AnimationIndex.BellySlide && player.canJump != 0) || e.parryLean > 0){
                // Parryslide (parry module)
                Ebug("Escort attempted a Parryslide");
                int direction;
                direction = player.slideDirection;

                Ebug("Is there a source? " + (source != null));
                Ebug("Is there a direction & Momentum? " + (directionAndMomentum != null));
                Ebug("Is there a hitChunk? " + (hitChunk != null));
                Ebug("Is there a hitAppendage? " + (hitAppendage != null));
                Ebug("Is there a type? " + (type != null));
                Ebug("Is there damage? " + (damage > 0f));
                Ebug("Is there stunBonus? " + (stunBonus > 0f));

                if (source != null) {
                    Ebug("Escort is being assaulted by: " + source.owner.GetType());
                }
                Ebug("Escort parry is being checked");
                if (type != null){
                    Ebug("Escort gets hurt by: " + type.value);
                if (type == Creature.DamageType.Bite){
                    Ebug("Escort is getting BIT?!");
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 35;
                        //(self as Player).WallJump(direction);
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug("Escort got out of a creature's mouth!");
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug("Weapons can BITE?!");
                    } 
                    else {
                        Ebug("Where is Escort getting bit from?!");
                    }
                } 
                else if (type == Creature.DamageType.Stab) {
                    Ebug("Escort is getting STABBED?!");
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug("Escort parried a stabby creature?");
                    } 
                    else if (source != null && source.owner is Weapon weapon) {
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug("Escort parried a stabby weapon");
                    } 
                    else {
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        bool keepLooping = true;
                        for (int a = 0; a < self.room.physicalObjects.Length; a++){
                            for (int b = 0; b < self.room.physicalObjects[a].Count; b++){
                                if (self.room.physicalObjects[a][b] is Vulture vulture && vulture.IsKing){
                                    if (vulture.kingTusks.tusks[0].impaleChunk != null && vulture.kingTusks.tusks[0].impaleChunk.owner == self){
                                        vulture.kingTusks.tusks[0].impaleChunk = null;
                                        keepLooping = false;
                                        break;
                                    } else if (vulture.kingTusks.tusks[1].impaleChunk != null && vulture.kingTusks.tusks[1].impaleChunk.owner == self){
                                        vulture.kingTusks.tusks[1].impaleChunk = null;
                                        keepLooping = false;
                                        break;
                                    }
                                }
                            }
                            if (!keepLooping){
                                Ebug("Tusk unimpaled!");
                                break;
                            }
                        }
                        e.ParrySuccess = true;
                        Ebug("Escort parried a generic stabby thing");
                    }
                } 
                else if (type == Creature.DamageType.Blunt) {
                    Ebug("Escort is getting ROCC'ED?!");
                    if (source != null && source.owner is Creature){
                        Ebug("Creatures aren't rocks...");
                    } 
                    else if (source != null && source.owner is Weapon weapon){
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(weapon.firstChunk.lastPos, -vector, weapon.firstChunk.vel.magnitude);
                        damage = 0f;
                        stunBonus = stunBonus / 5f;
                        e.ParrySuccess = true;
                        Ebug("Escort bounces a blunt thing.");
                    } 
                    else {
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug("Escort parried something blunt.");
                    }
                } 
                else if (type == Creature.DamageType.Water) {
                    Ebug("Escort is getting Wo'oh'ed?!");
                } 
                else if (type == Creature.DamageType.Explosion) {
                    Ebug("Escort is getting BLOWN UP?!");
                    if (source != null && source.owner is Creature){
                        Ebug("Wait... creatures explode?!");
                    } 
                    else if (source != null && source.owner is Weapon){
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug("Escort parries an explosion from weapon?!");
                    } 
                    else {
                        player.WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug("Escort parries an explosion");
                    }
                } 
                else if (type == Creature.DamageType.Electric) {
                    Ebug("Escort is getting DEEP FRIED?!");
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        player.Jump();
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        //(self as Player).LoseAllGrasps();
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug("Escort somehow parried a shock from creature?!");
                    } 
                    else if (source != null && source.owner is Weapon){
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        player.Jump();
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug("Escort somehow parried a shock object?!");
                    } 
                    else {
                        player.animation = Player.AnimationIndex.Flip;
                        player.Jump();
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug("Escort attempted to parry a shock but why?!");
                    }
                } 
                else {
                    Ebug("Escort is getting UNKNOWNED!!! RUNNN");
                    if (source != null && source.owner is Creature){
                        Ebug("IT'S ALSO AN UNKNOWN CREATURE!!");
                    } else if (source != null && source.owner is Weapon){
                        Ebug("IT'S ALSO AN UNKNOWN WEAPON!!");
                    } else {
                        Ebug("WHO THE HECK KNOWS WHAT IT IS?!");
                    }
                }
                }
            }

            // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
            // Visual indicator doesn't work ;-;
            if (e.ParrySuccess){
                self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                Ebug("Parry successful!");
                e.iFrames = 6;
                e.parryLean = 0;
            }
            // else if (e.iFrames > 0) {Ebug("Immunity frame tick");} 
            else {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                Ebug("Nothing or not possible to parry!");
            }
            Ebug("Parry Check end");
            return;

        }


        // Parryslide spears?!
        private bool Escort_StickySpear(On.Player.orig_SpearStick orig, Player self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, source, dmg, chunk, appPos, direction);
                }
            } catch (Exception err){
                Ebug(err.Message);
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!ParrySlide.TryGet(self, out bool parrier)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!parrier || (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }

            Ebug("Sticky Triggered!");
            return !(self.animation == Player.AnimationIndex.BellySlide);
        }


        // Implement Bodyslam
        private void Escort_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if (
                !RR.TryGet(self, out int resetRate) ||
                !BodySlam.TryGet(self, out float[] bodySlam) ||
                !TrampOhLean.TryGet(self, out float bounce) ||
                !Esconfig_HypeReq(self) ||
                !Esconfig_DKMulti(self)
                ){
                return;
            }

            //Ebug("Collision Triggered!");
            if (slowDownDevConsole > resetRate){
                Ebug("Escort collides!");
                Ebug("Has physical object? " + otherObject != null);
                if (otherObject != null){
                    Ebug("What is it? " + otherObject.GetType());
                }
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Reimplementing the elevator... the way it was in its glory days
            if (Esconfig_Elevator(self) && otherObject is Creature && self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                self.jumpBoost += 4;
            }


            if (otherObject is Creature creature && 
                creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !(ModManager.CoopAvailable && otherObject is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){


                // Creature Trampoline (or if enabled Escort's Elevator)
                /*
                Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
                */
                if (self.animation == Player.AnimationIndex.Flip && self.bodyMode == Player.BodyModeIndex.Default && (!creature.dead || creature is Lizard)){
                    if (self.jumpBoost <= 0) {
                        self.jumpBoost = bounce;
                    }
                }

                int direction;

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    creature.SetKillTag(self.abstractCreature);

                    if (e.parryLean <= 0){
                        e.parryLean = 4;
                    }
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,e.RollinSFXChunk);

                    float normSlideStun = (hypedMode || e.combatTech? bodySlam[1] : bodySlam[1] * 1.5f);
                    if (hypedMode && self.aerobicLevel > requirement){
                        normSlideStun = bodySlam[1] * (e.combatTech? 1.75f : 2f);
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        creature.firstChunk, null, Creature.DamageType.Blunt,
                        bodySlam[0], normSlideStun
                    );
                    /*
                    if (self.pickUpCandidate is Spear){  // Attempts to pickup spears (may pickup things higher in priority that are nearby)
                        self.PickupPressed();
                    }*/

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 5f, 5.5f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    if (self.longBellySlide) {
                        direction = self.rollDirection;
                        self.animation = Player.AnimationIndex.Flip;
                        self.WallJump(direction);
                        if (e.combatTech){
                            self.animation = Player.AnimationIndex.BellySlide;
                            self.bodyChunks[1].vel = new Vector2((float)self.slideDirection * 18f, 0f);
                            self.bodyChunks[0].vel = new Vector2((float)self.slideDirection * 18f, 5f);
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                        }
                        Ebug("Greatdadstance stunslide!");
                    } else {
                        direction = self.flipDirection;
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.Flip;
                        Ebug("Stunslided!");
                    }
                    }

                // Dropkick
                else if (self.animation == Player.AnimationIndex.RocketJump){
                    creature.SetKillTag(self.abstractCreature);

                    String message = "Dropkicked!";
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk);
                    
                    if (!creature.dead) {
                        DKMultiplier *= creature.TotalMass;
                    }
                    float normSlamDamage = 0.1f;
                    if (e.DropKickCD <= 5){
                        normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + (e.combatTech? 0.15f : 0.27f));
                        creature.LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * (e.combatTech? 1.6f : 2f);}
                        message = "Powerdropkicked!";
                    } else {
                        //multiplier *= 0.5f;
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*DKMultiplier, self.mainBodyChunk.vel.y*DKMultiplier*(e.LizardDunk?0.2f:1f))),
                        creature.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, bodySlam[3]
                    );
                    Ebug("Dunk the lizard: " + e.LizardDunk);
                    if (e.DropKickCD == 0){
                        e.LizardDunk = false;
                    }
                    e.DropKickCD = (self.longBellySlide? 30 : 15);
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    /*
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }*/
                    direction = -self.flipDirection;
                    self.WallJump(direction);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    Ebug(message);
                    }

                else if (e.CometFrames > 0 && !e.Cometted){
                    creature.SetKillTag(self.abstractCreature);
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x*DKMultiplier, self.bodyChunks[0].vel.y*DKMultiplier)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        0f, 15f
                    );
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x*(DKMultiplier/2);
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y*(DKMultiplier/2);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    Ebug("Headbutted!");
                    e.Cometted = true;
                }
            }
        }


        // Implement a different type of dropkick
        private void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu){
            orig(self, grasp, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if (!BodySlam.TryGet(self, out var bodySlam)){
                return;
            }

            Ebug("Toss Object Triggered!");
            if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead && self.bodyMode == Player.BodyModeIndex.Default){
                self.animation = Player.AnimationIndex.RocketJump;
                self.bodyChunks[1].vel.x += self.slideDirection;
            }
        }

        // Implement unique spearskill
        private void Escort_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear){
            orig(self, spear);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(err.Message);
                return;
            }
            if(
                !bonusSpear.TryGet(self, out float[] spearDmgBonuses) ||
                !Esconfig_HypeReq(self) ||
                !self.Malnourished
                ){
                return;
            }

            Ebug("ThrownSpear Triggered!");
            float thrust = 7f;
            bool onPole = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
            bool doNotYeet = onPole || !e.combatTech;
            if (Esconfig_Hypable(self)){
                if (self.aerobicLevel > requirement){
                    spear.throwModeFrames = -1;
                    spear.spearDamageBonus = spearDmgBonuses[0];
                    if (self.canJump != 0 && !self.longBellySlide){
                        if (!doNotYeet){
                            self.rollCounter = 0;
                            if (self.input[0].jmp && self.input[0].thrw){
                                self.animation = Player.AnimationIndex.BellySlide;
                                self.whiplashJump = true;
                                spear.firstChunk.vel.x *= 1.8f;
                                Ebug("Spear Go!?");
                            } else {
                                self.animation = Player.AnimationIndex.Roll;
                                self.standing = false;
                            }
                        }
                        thrust = 12f;
                    } else {
                        self.longBellySlide = true;
                        if (!doNotYeet){
                            self.exitBellySlideCounter = 0;
                            self.rollCounter = 0;
                            self.flipFromSlide = true;
                            self.animation = Player.AnimationIndex.BellySlide;
                        }
                        thrust = 9f;
                    }
                } else {
                    if (!doNotYeet){
                        self.rollCounter = 0;
                        if (self.canJump != 0){
                            self.whiplashJump = true;
                            if (self.animation != Player.AnimationIndex.BellySlide){
                                self.animation = Player.AnimationIndex.BellySlide;
                            }
                            if (self.input[0].jmp && self.input[0].thrw){
                                spear.firstChunk.vel.x *= 1.7f;
                                Ebug("Spear Go!");
                            }
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                            self.standing = false;
                        }
                    }
                    spear.spearDamageBonus = spearDmgBonuses[1];
                    thrust = 5f;
                }
            } else {
                spear.spearDamageBonus = 1.25f;
            }
            if (onPole) {
                thrust = 1f;
            }
            if ((self.room != null && self.room.gravity == 0f) || Mathf.Abs(spear.firstChunk.vel.x) < 1f){
                self.firstChunk.vel += spear.firstChunk.vel.normalized * thrust;
            } else {
                self.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                BodyChunk firstChunker = self.firstChunk;
                if (self.animation != Player.AnimationIndex.BellySlide){
                    firstChunker.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                }
            }
        }

        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
            } catch (Exception err){
                Ebug(err.Message);
                return orig(self, obj);
            }
            if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                return orig(self, obj);
            }
            if (!dualWielding.TryGet(self, out bool dW)){
                return orig(self, obj);
            }

            if (dW){
                if (obj is Weapon){
                    // Any weapon is dual-wieldable, including spears
                    return Player.ObjectGrabability.OneHand;
                } else if (obj is Lizard lizzie){
                    // Any lizards that are haulable (while dead) or stunned are dual-wieldable
                    if (lizzie.dead){
                        return Player.ObjectGrabability.OneHand;
                    } else if (lizzie.Stunned && Esconfig_Dunkin(self)){
                        lizzie.Violence(self.bodyChunks[1], null, obj.firstChunk, null, Creature.DamageType.Blunt, 0f, 25f);
                        e.LizardDunk = true;
                        return Player.ObjectGrabability.TwoHands;
                    }
                }
            }
            return orig(self, obj);
        }

        private float Escort_DeathBiteMult(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            try{
                if (self.slugcatStats.name.value == "EscortMe"){
                    float biteMult = e.combatTech? 0.5f : 0.15f;
                    return ((self.animation == Player.AnimationIndex.BellySlide && self.canJump != 0) || e.parryLean > 0? 5f : biteMult);
                } else {
                    return orig(self);
                }
            } catch (Exception e){
                Ebug("Couldn't set deathbitemultipler!");
                Ebug(e.Message);
                return orig(self);
            }
        }

        private void Escort_Die(On.Player.orig_Die orig, Player self){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
                Ebug("Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0){
                    orig(self);
                    if (self.dead && Esconfig_SFX(self)){
                        self.room.PlaySound(Escort_SFX_Death, e.RollinSFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug("Failure.");
                } else {
                    self.dead = false;
                    Ebug("Player didn't die?");
                    e.ParrySuccess = false;
                }
            } catch (Exception e){
                Ebug("Something happened while trying to die!");
                Ebug(e.Message);
                orig(self);
            }
        }

        private static string[] Escort_getStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
        {
            try {
                if (i.value == "EscortMe"){
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
                        "DM"
                    };
                } else {
                    return orig(i);
                }
            } catch (Exception e){
                Ebug("Something went wrong when getting story regions!");
                Ebug(e.Message);
                return orig(i);
            }
        }

        private static float Escort_ExpSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
        {
            try{
                if (index.value == "EscortMe"){
                    return 0.012f;
                } else {
                    return orig(index);
                }
            } catch (Exception e){
                Ebug("Something happened when setting exploding spear chance!");
                Ebug(e.Message);
                return orig(index);
            }
        }

        private static float Escort_EleSpearSpawnChance(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
        {   
            try{
                if (index.value == "EscortMe"){
                    return 0.078f;
                } else {
                    return orig(index);
                }
            } catch (Exception e){
                Ebug("Something happened when setting electric spear spawn chance!");
                Ebug(e.Message);
                return orig(index);
            }
        }

        private static float Escort_SpearSpawnMod(On.SlugcatStats.orig_SpearSpawnModifier orig, SlugcatStats.Name index, float originalSpearChance)
        {
            try{
                if (index.value == "EscortMe"){
                    return Mathf.Pow(originalSpearChance, 1.3f);
                } else {
                    return orig(index, originalSpearChance);
                }
            } catch (Exception e){
                Ebug("Something happened when spawning spears!");
                Ebug(e.Message);
                return orig(index, originalSpearChance);
            }
        }
    }

}