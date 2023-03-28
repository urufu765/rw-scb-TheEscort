using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "[WIP] The Escort", "0.1.6.2")]
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

        public static readonly PlayerFeature<bool> BetterPounce = PlayerBool("theescort/better_pounce");
        public static readonly GameFeature<bool> SuperMeanLizards = GameBool("theescort/mean_lizards");
        public static readonly GameFeature<bool> SuperMeanGarbageWorms = GameBool("theescort/mean_garb_worms");
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");
        public static readonly PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");
        public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static readonly PlayerFeature<float> CarryHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DropKick = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static readonly PlayerFeature<bool> Elevator = PlayerBool("theescort/elevator");
        public static readonly PlayerFeature<float> TrampOhLean = PlayerFloat("theescort/trampoline");
        public static readonly PlayerFeature<bool> Hyped = PlayerBool("theescort/adrenaline_system");
        public static readonly PlayerFeature<float> StaReq = PlayerFloat("theescort/stamina_req");
        public static readonly PlayerFeature<int> RR = PlayerInt("theescort/reset_rate");
        public static readonly PlayerFeature<bool> ParryTest = PlayerBool("theescort/parry_test");
        public static readonly PlayerFeature<float[]> bonusSpear = PlayerFloats("theescort/spear_damage");
        public static readonly PlayerFeature<bool> dualWielding = PlayerBool("theescort/dual_wield");
        public static readonly PlayerFeature<bool> soundsAhoy = PlayerBool("theescort/sounds_ahoy");
        public static readonly PlayerFeature<float[]> NoMoreGutterWater = PlayerFloats("theescort/guuh_wuuh");
        public static readonly PlayerFeature<bool> LongWallJump = PlayerBool("theescort/long_wall_jump");
        public static readonly PlayerFeature<float[]>
        WallJumpVal = PlayerFloats("theescort/wall_jump_val");

        private static readonly String dMe = "<EscortMe> ";

        public static SoundID Escort_SFX_Death;
        public static SoundID Escort_SFX_Flip;
        public static SoundID Escort_SFX_Roll;
        //public DynamicSoundLoop escortRollin;
        public Escort escort;

        private int slowDownDevConsole = 0;

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
            //On.Player.ctor += new On.Player.hook_ctor(this.Escort_ctor);
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

            //On.Water.Update += new On.Water.hook_Update(this.Escort_Water_Update);
            }




        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Escort_SFX_Death = new SoundID("Escort_Failure", true);
            Escort_SFX_Flip = new SoundID("Escort_Flip", true);
            Escort_SFX_Roll = new SoundID("Escort_Roll", true);
            Ebug("All SFX loaded!");
            EscEnums.RegisterValues();
            //MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            Ebug("All loaded!");
        }


        private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self){
            orig(self);
            try{
            if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify")){
                Debug.Log(dMe + "Found Revivify! Applying patch...");
                Ebug("... just kidding, there's no proper patch yet...");
                escPatch_revivify = true;
            }
            } catch (Exception e){
                throw new Exception(e.Message);
            }
        }


        private void Update_Refresher(Player self){
            if (RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                slowDownDevConsole = 0;
            } else {
                slowDownDevConsole++;
            }
        }


        // Implement lizard aggression (edited from template)
        private void Escort_Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if(SuperMeanLizards.TryGet(world.game, out bool meanness) && meanness)
            {
                self.spawnDataEvil = Mathf.Max(self.spawnDataEvil, 100f);
            }
        }
        private void Escort_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            escort = new Escort(self);
            try {
                escort.Escort_set_roller(Escort_SFX_Roll);
                Ebug("Setting roll sound");
            } catch (Exception e){
                throw new Exception(e.Message);
            } finally {
                Ebug("All ctor'd");
            }
        }

        // Implement Escort's slowed stamina increase
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            orig(self, f);

            if (Exhausion.TryGet(self, out var exhaust)){
                if (!self.slugcatStats.malnourished){
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / exhaust));
                } else {
                    self.aerobicLevel = Mathf.Min(2f, self.aerobicLevel + (f / (exhaust / 2)));
                }
            } else {
                self.aerobicLevel = Mathf.Min(1f, self.aerobicLevel + f / 9f);
            }
        }

        // Implement visual effect for Battle-Hyped mode
        private void Escort_Update(On.Player.orig_Update orig, Player self, bool eu){
            orig(self, eu);

            // For slowed down dev console output
            Update_Refresher(self);


            // Cooldown Refresh
            if (escort.EscortDropKickCooldown > 0){
                escort.EscortDropKickCooldown--;
            }
            if (escort.EscortCentipedeCooldown > 0){
                escort.EscortCentipedeCooldown--;
            }
            
            // Just for seeing what a variable does.
            if(RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                //Debug.Log(dMe + "Clocked.");
                //Ebug(" Roll Direction: " + self.rollDirection);
                //Ebug("Slide Direction:" + self.slideDirection);
                //Ebug(" Flip Direction: " + self.flipDirection);
                //Ebug("Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                //Ebug("Normalized direction: " + self.bodyChunks[0].vel.normalized);
                }

            // vfx
            if(Hyped.TryGet(self, out bool hypedMode) && StaReq.TryGet(self, out float requirement) && LongWallJump.TryGet(self, out bool wallJumper) && WallJumpVal.TryGet(self, out var WJV)){
                if (hypedMode && self.aerobicLevel > requirement){
                    Color hypedColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    hypedColor.a = 0.8f;

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, hypedColor));
                }
                if (wallJumper){
                    if (self.superLaunchJump > 19){
                        Color superColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 9f, 4f, 4f, 11f, superColor));
                    }
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.consistentDownDiagonal >= (int)WJV[4]){
                        Color flipColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, flipColor));
                    }
                    
                }
            }

            // Implement guuh wuuh
            if(NoMoreGutterWater.TryGet(self, out var theGut) && self.bodyMode == Player.BodyModeIndex.Swimming){
                float superSwim = Mathf.Lerp(theGut[0], theGut[1], self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity));
                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[2] * superSwim, theGut[3] * superSwim);
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[4] * superSwim, theGut[5] * superSwim);
                }
            }

            if (soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn){
                /*
                if (this.escortRollin == null){
                    this.escortRollin = new ChunkDynamicSoundLoop(self.mainBodyChunk);
                    this.escortRollin.sound = Escort_SFX_Roll;
                    this.escortRollin.Volume = 0f;
                    Debug.Log(dMe + "Sound Initialized.");
                } else {
                    if (self.animation == Player.AnimationIndex.Roll){
                        this.escortRollin.Update();
                    } else {
                        escort.EscortRollinCounter = 0f;
                    }
                }*/
                if (self.animation == Player.AnimationIndex.Roll){
                    escort.EscortRoller.Update();
                } else {
                    escort.EscortRollinCounter = 0f;
                }

            }
        }

        // Implement Flip jump and less tired from jumping
        private void Escort_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (BetterPounce.TryGet(self, out var willPounce))
            {
                if (self.slugcatStats.name.value == "EscortMe"){
                    if (self.aerobicLevel > 0.1f){
                        self.aerobicLevel -= 0.1f;
                    }
                    
                    if (self.animation != Player.AnimationIndex.BellySlide && self.longBellySlide){
                        self.longBellySlide = false;
                    }

                    // Replace chargepounce with a sick flip
                    if (
                        willPounce && (self.superLaunchJump >= 19 || self.simulateHoldJumpButton == 6 || self.killSuperLaunchJumpCounter > 0) && self.bodyMode == Player.BodyModeIndex.Crawl
                        ){
                        Debug.Log(dMe + "FLIPERONI GO!");
                        if (soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn){
                            self.room.PlaySound(Escort_SFX_Flip, escort.EscortSoundBodyChunk);
                        }
                        self.animation = Player.AnimationIndex.Flip;
                    }

                    self.consistentDownDiagonal = 0;
                }
            }
        }


        private void Escort_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            if (self.bodyMode == Player.BodyModeIndex.WallClimb){
            if (LongWallJump.TryGet(self, out bool wallJumper) && WallJumpVal.TryGet(self, out var WJV) && soundsAhoy.TryGet(self, out bool sfxOn) && BetterPounce.TryGet(self, out bool willPounce)){
                if ((wallJumper && self.canWallJump != 0) || !wallJumper) {
                    orig(self, direction);
                    float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                    String[] toPrint = new String[3];
                    bool superFlip = self.allowRoll == 15 && willPounce;
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
                        self.room.PlaySound(SoundID.Slugcat_Normal_Jump, escort.EscortSoundBodyChunk, false, 1f, 0.7f);


                    } else {
                        self.bodyChunks[0].vel.y = ((self.superLaunchJump > 19 || self.consistentDownDiagonal > (int)WJV[4])? WJV[0] : 8f) * n;
                        self.bodyChunks[1].vel.y = ((self.superLaunchJump > 19 || self.consistentDownDiagonal > (int)WJV[4])? WJV[1] : 7f) * n;
                        self.bodyChunks[0].vel.x = ((superFlip && self.consistentDownDiagonal >= (int)WJV[4])? WJV[2] : 7f) * n * (float)direction;
                        self.bodyChunks[1].vel.x = ((superFlip && self.consistentDownDiagonal >= (int)WJV[4])? WJV[3] : 6f) * n * (float)direction;
                        self.standing = true;
                        self.jumpStun = 8 * direction;
                        if (self.consistentDownDiagonal < (float)WJV[4]){
                            self.room.PlaySound((self.superLaunchJump > 19? SoundID.Slugcat_Super_Jump : SoundID.Slugcat_Wall_Jump), escort.EscortSoundBodyChunk, false, 1f, 0.7f);
                        }
                        toPrint.SetValue("Not Water", 1);
                        Ebug("Y Velocity" + self.bodyChunks[0].vel.y);
                        Ebug("Y Velocity" + self.bodyChunks[1].vel.y);
                        Ebug("X Velocity" + self.bodyChunks[0].vel.x);
                        Ebug("X Velocity" + self.bodyChunks[1].vel.x);
                    }
                    self.jumpBoost = 0f;
                    if (superFlip && self.consistentDownDiagonal >= (int)WJV[4]){
                        self.animation = Player.AnimationIndex.Flip;
                        self.room.PlaySound((sfxOn? Escort_SFX_Flip : SoundID.Slugcat_Sectret_Super_Wall_Jump), escort.EscortSoundBodyChunk, false, 1f, 0.9f);
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
            } else {
                orig(self, direction);
                Ebug("Default behaviour");
            }
        }

        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);

            if (LongWallJump.TryGet(self, out bool wallJumper) && wallJumper && self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0){
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
                if(RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                Ebug(msg);
            }

            }

        }

        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            if(LongWallJump.TryGet(self, out bool wallJumper) && wallJumper){
                int previously = self.input[0].x;
                orig(self);

            // Undoes the input cancellation
                if(self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1){
                    if (self.input[0].x == 0){
                        self.input[0].x = previously;
                    }
                }
            } else {
                orig(self);
            }
        }




        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            if (CarryHeavy.TryGet(self, out var ratioed)){
                if (obj.TotalMass <= self.TotalMass * ratioed){
                    if (escPatch_revivify && obj is Creature && (obj as Creature).abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && (obj as Creature).dead) {
                        return orig(self, obj);
                    }
                    if (ModManager.CoopAvailable){
                        Player player = obj as Player;
                        if (player != null){
                            return !player.isSlugpup;
                        }
                    }
                    return false;
                }
                return orig(self, obj);
            } 
            
            // for some reason if I don't copy and paste the thing, the other scugs get affected...
            else {
                /*
                if (self.Grabability(obj) == Player.ObjectGrabability.Drag)
            {
                return true;
            }
                if (self.Grabability(obj) != Player.ObjectGrabability.TwoHands && obj.TotalMass <= self.TotalMass * 0.6f)
                {
                    if (ModManager.CoopAvailable)
                    {
                        Player player = obj as Player;
                        if (player != null)
                        {
                            return !player.isSlugpup;
                        }
                    }
                    return false;
                }*/
                return orig(self, obj);
            }
        }


        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);

            if (BetterCrawl.TryGet(self, out var crawlSpeed) && BetterPoleWalk.TryGet(self, out var poleMove) && Hyped.TryGet(self, out bool hypedMode)){

            // Implement bettercrawl
            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                self.dynamicRunSpeed[0] = (hypedMode? Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) : crawlSpeed[4]) * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = (hypedMode? Mathf.Lerp(crawlSpeed[2], crawlSpeed[3], self.aerobicLevel) : crawlSpeed[5]) * self.slugcatStats.runspeedFac;
                //Debug.Log("Escort's Speed:" + self.dynamicRunSpeed[0]);
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
            }
        }


        // Implement Movementtech
        private void Escort_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self){
            orig(self);
            if (self.slugcatStats.name.value == "EscortMe"){
                // Infiniroll
                if (self.animation == Player.AnimationIndex.Roll){
                    if (!((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                        escort.EscortRollinCounter++;
                        if(RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                            Debug.Log(dMe + "ROLLIN at:" + escort.EscortRollinCounter);
                        }
                        if(soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn && escort.EscortRoller != null){
                            escort.EscortRoller.Volume = Mathf.InverseLerp(100f, 300f, escort.EscortRollinCounter);
                        }
                        self.rollCounter = 0;
                    }
                } /*else if (self.animation == Player.AnimationIndex.RocketJump) {
                    self.bodyMode = Player.BodyModeIndex.Default;
                    self.standing = false;
                    self.bodyChunks[1].vel
                }*/

                if (self.animation != Player.AnimationIndex.Roll){
                    escort.EscortRollinCounter = 0f;
                }

                // I'll find out how to implement a more lineant slide (like rivulet's slide pounces) while keeping it short (like every other slugcats) one day...
                //if (self.animation == Player.AnimationIndex.BellySlide){
                    // TODO implement better slide
                //}
            }
        }


        // Implement Parryslide/midair projectile grab
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){

            if (self is Player){  // Check if self is player such that when class is converted it does not cause an error

                // connects to the Escort's Parryslide option
                if (ParrySlide.TryGet((self as Player), out bool enableParry) && enableParry && 
                ParryTest.TryGet((self as Player), out bool standParry)){
                escort.ParrySuccess = false;
                if (standParry? (self as Player).bodyMode == Player.BodyModeIndex.Crawl : (self as Player).animation == Player.AnimationIndex.BellySlide){
                    // Parryslide (parry module)
                    Debug.Log(dMe + "Escort attempted a Parryslide");
                    int direction;
                    if (self is Player){
                        direction = (self as Player).slideDirection;
                    } else {
                        direction = 0;
                        throw new Exception("Self is not player!");
                    }
                    Debug.Log(dMe + "Is there an instance? " + (self != null));
                    Debug.Log(dMe + "Is there a source? " + (source != null));
                    Debug.Log(dMe + "Is there a direction & Momentum? " + (directionAndMomentum != null));
                    Debug.Log(dMe + "Is there a hitChunk? " + (hitChunk != null));
                    Debug.Log(dMe + "Is there a hitAppendage? " + (hitAppendage != null));
                    Debug.Log(dMe + "Is there a type? " + (type != null));
                    Debug.Log(dMe + "Is there damage? " + (damage > 0f));
                    Debug.Log(dMe + "Is there stunBonus? " + (stunBonus > 0f));

                    if (source != null) {
                        Debug.Log(dMe + "Escort is being assaulted by: " + source.owner.GetType());
                    } if (type != null) {
                        Debug.Log(dMe + "Escort gets hurt by: " + type.value);
                    }

                    Debug.Log(dMe + "Escort parry is being checked");
                    if (type != null){
                    if (type == Creature.DamageType.Bite){
                        Debug.Log(dMe + "Escort is getting BIT?!");
                        if (source != null && source.owner is Creature){
                            (source.owner as Creature).LoseAllGrasps();
                            (source.owner as Creature).stun = 35;
                            (self as Player).WallJump(direction);
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = 0f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort got out of a creature's mouth!");
                        } else if (source != null && source.owner is Weapon){
                            Debug.Log(dMe + "Weapons can BITE?!");
                        } else {
                            Debug.Log(dMe + "Where is Escort getting bit from?!");
                        }
                    } else if (type == Creature.DamageType.Stab) {
                        Debug.Log(dMe + "Escort is getting STABBED?!");
                        if (source != null && source.owner is Creature){
                            (source.owner as Creature).LoseAllGrasps();
                            (source.owner as Creature).stun = 20;
                            damage = 0f;
                            stunBonus = 0f;
                            type = Creature.DamageType.Blunt;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parried a stabby creature?");
                        } else if (source != null && source.owner is Weapon) {
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            (source.owner as Weapon).WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                            damage = 0f;
                            type = Creature.DamageType.Blunt;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parried a stabby weapon");
                        } else {
                            damage = 0f;
                            type = Creature.DamageType.Blunt;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parried a generic stabby thing");
                        }
                    } else if (type == Creature.DamageType.Blunt) {
                        Debug.Log(dMe + "Escort is getting ROCC'ED?!");
                        if (source != null && source.owner is Creature){
                            Debug.Log(dMe + "Creatures aren't rocks...");
                        } else if (source != null && source.owner is Weapon){
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            (source.owner as Weapon).WeaponDeflect(source.owner.firstChunk.lastPos, -vector, source.owner.firstChunk.vel.magnitude);
                            damage = 0f;
                            stunBonus = stunBonus / 5f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort bounces a blunt thing.");
                        } else {
                            damage = 0f;
                            stunBonus = 0f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parried something blunt.");
                        }
                    } else if (type == Creature.DamageType.Water) {
                        Debug.Log(dMe + "Escort is getting Wo'oh'ed?!");
                    } else if (type == Creature.DamageType.Explosion) {
                        Debug.Log(dMe + "Escort is getting BLOWN UP?!");
                        if (source != null && source.owner is Creature){
                            Debug.Log(dMe + "Wait... creatures explode?!");
                        } else if (source != null && source.owner is Weapon){
                            (self as Player).animation = Player.AnimationIndex.Roll;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = stunBonus / 2f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parries an explosion from weapon?!");
                        } else {
                            (self as Player).WallJump(direction);
                            (self as Player).animation = Player.AnimationIndex.Roll;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = stunBonus / 2f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort parries an explosion");
                        }
                    } else if (type == Creature.DamageType.Electric) {
                        Debug.Log(dMe + "Escort is getting DEEP FRIED?!");
                        if (source != null && source.owner is Creature){
                            (source.owner as Creature).LoseAllGrasps();
                            (source.owner as Creature).stun = 20;
                            (self as Player).WallJump(direction);
                            (self as Player).animation = Player.AnimationIndex.Flip;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = stunBonus / 2f;
                            (self as Player).LoseAllGrasps();
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort somehow parried a shock from creature?!");
                        } else if (source != null && source.owner is Weapon){
                            (self as Player).WallJump(direction);
                            (self as Player).animation = Player.AnimationIndex.Flip;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = stunBonus / 2f;
                            escort.ParrySuccess = true;
                            Debug.Log(dMe + "Escort somehow parried a shock object?!");
                        } else {
                            Debug.Log(dMe + "Escort attempted to parry a shock but why?!");
                        }
                    } else {
                        Debug.Log(dMe + "Escort is getting UNKNOWNED!!! RUNNN");
                        if (source != null && source.owner is Creature){
                            Debug.Log(dMe + "IT'S ALSO AN UNKNOWN CREATURE!!");
                        } else if (source != null && source.owner is Weapon){
                            Debug.Log(dMe + "IT'S ALSO AN UNKNOWN WEAPON!!");
                        } else {
                            Debug.Log(dMe + "WHO THE HECK KNOWS WHAT IT IS?!");
                        }
                    }
                    }
                    }

                    // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
                    // Visual indicator doesn't work ;-;
                    if (escort.ParrySuccess){
                        self.AllGraspsLetGoOfThisObject(true);
                        self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                        Debug.Log(dMe + "Parry successful!");

                    } else {
                        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                        Debug.Log(dMe + "... nor an object");
                    }
                    Debug.Log(dMe + "Parry Check end");
                    return;
                }
            }

        }


        // Parryslide spears?!
        private bool Escort_StickySpear(On.Player.orig_SpearStick orig, Player self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            if (ParrySlide.TryGet(self, out bool parrier) && parrier) {
                if (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire){
                    return orig(self, source, dmg, chunk, appPos, direction);
                }
                return !(self.animation == Player.AnimationIndex.BellySlide);
            } else{
                return orig(self, source, dmg, chunk, appPos, direction);
            }
        }


        // Implement Bodyslam
        private void Escort_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);


            if (self.slugcatStats.name.value == "EscortMe"){
            if (RR.TryGet(self, out int resetRate) && slowDownDevConsole > resetRate){
                Debug.Log(dMe + "Escort collides!");
                Debug.Log(dMe + "Has physical object? " + otherObject != null);
                if (otherObject != null){
                    Debug.Log(dMe + "What is it? " + otherObject.GetType());
                }
            }

            BodySlam.TryGet(self, out float[] bodySlam);
            Elevator.TryGet(self, out bool yeet);
            TrampOhLean.TryGet(self, out float bounce);
            Hyped.TryGet(self, out bool hypedMode);
            StaReq.TryGet(self, out float requirement);

            if (otherObject is Creature && 
                (otherObject as Creature).abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && (otherObject as Creature).abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && ((otherObject as Creature).abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Slugcat || RWCustom.Custom.rainWorld.options.friendlyFire)){


                // Creature Trampoline (or if enabled Escort's Elevator)
                /*
                Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
                */
                if (self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                    if (yeet){
                        self.jumpBoost += 4;
                    } else if (self.jumpBoost <= 0) {
                        self.jumpBoost = bounce;
                    } else if (self.jumpBoost >= bounce && self.jumpBoost >= 1f){
                        self.jumpBoost--;  // not sure if this does anything...
                    }
                }

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,escort.EscortSoundBodyChunk);
                    (otherObject as Creature).SetKillTag(self.abstractCreature);
                    float normSlideStun = bodySlam[1];
                    if (hypedMode && self.aerobicLevel > requirement){
                        normSlideStun = bodySlam[1] * 1.75f;
                    }
                    (otherObject as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        otherObject.firstChunk, null, Creature.DamageType.Blunt,
                        bodySlam[0], normSlideStun
                    );
                    int direction;
                    /*
                    if (self.pickUpCandidate is Spear){  // Attempts to pickup spears (may pickup things higher in priority that are nearby)
                        self.PickupPressed();
                    }*/

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 5f, 5.5f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    if (self.longBellySlide) {
                        direction = self.rollDirection;
                        self.animation = Player.AnimationIndex.Flip;
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.BellySlide;
                        self.bodyChunks[1].vel = new Vector2((float)self.slideDirection * 18f, 0f);
                        self.bodyChunks[0].vel = new Vector2((float)self.slideDirection * 18f, 5f);
                        Debug.Log(dMe + "Greatdadstance stunslide!");
                    } else {
                        direction = self.flipDirection;
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.Flip;
                        Debug.Log(dMe + "Stunslided!");
                    }
                    }

                // Dropkick
                else if (self.animation == Player.AnimationIndex.RocketJump){
                    String message = "Dropkicked!";
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk);
                    
                    DropKick.TryGet(self, out var multiplier);
                    if (!(otherObject as Creature).dead) {
                        multiplier *= (otherObject as Creature).TotalMass;
                    }
                    (otherObject as Creature).SetKillTag(self.abstractCreature);
                    float normSlamDamage = 0.1f;
                    if (escort.EscortDropKickCooldown <= 5){
                        normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + 0.15f);
                        (otherObject as Creature).LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * 1.6f;}
                        message = "Powerdropkicked!";
                    } else {
                        //multiplier *= 0.5f;
                    }
                    (otherObject as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*multiplier, self.mainBodyChunk.vel.y*multiplier*(escort.LizardDunk?0.2f:1f))),
                        otherObject.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, bodySlam[3]
                    );
                    Ebug(escort.LizardDunk);

                    escort.EscortDropKickCooldown = (self.longBellySlide? 30 : 15);
                    escort.LizardDunk = false;
                    int direction;
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
                }
            }
        }


        // Implement a different type of dropkick
        private void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu){
            orig(self, grasp, eu);
            if (BodySlam.TryGet(self, out var bodySlam))
            {            
                if (self.grasps[grasp].grabbed is Lizard && !(self.grasps[grasp].grabbed as Lizard).dead && self.bodyMode == Player.BodyModeIndex.Default){
                    /*(self.grasps[grasp].grabbed as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        self.grasps[grasp].grabbed.firstChunk, null, Creature.DamageType.Blunt,
                        bodySlam[2], bodySlam[3]
                    );*/
                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyChunks[1].vel.x += self.slideDirection;
                }
            }        
        }



        // Implement unique spearskill
        private void Escort_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear){
            orig(self, spear);
            if (bonusSpear.TryGet(self, out float[] spearDmgBonuses) && Hyped.TryGet(self, out bool hypedMode) && StaReq.TryGet(self, out float requirement) && !self.Malnourished){
                float thrust = 7f;
                bool doNotYeet = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
                if (hypedMode){
                    if (self.aerobicLevel > requirement){
                        spear.spearDamageBonus = spearDmgBonuses[0];
                        if (self.canJump != 0 && !self.longBellySlide){
                            if (!doNotYeet){
                                self.rollCounter = 0;
                                self.animation = Player.AnimationIndex.Roll;
                                self.standing = false;
                            }
                            thrust = 12f;
                        } else {
                            self.longBellySlide = true;
                            if (!doNotYeet){
                                self.exitBellySlideCounter = 0;
                                self.rollCounter = 0;
                                self.animation = Player.AnimationIndex.BellySlide;
                            }
                            thrust = 9f;
                        }
                    } else {
                        if (!doNotYeet){
                            if (self.canJump != 0){
                                self.rollCounter = 0;
                                self.whiplashJump = true;
                                self.animation = Player.AnimationIndex.BellySlide;
                            } else {
                                self.rollCounter = 0;
                                self.animation = Player.AnimationIndex.Flip;
                            }
                        }
                        spear.spearDamageBonus = spearDmgBonuses[1];
                        thrust = 5f;
                    }
                } else {
                    spear.spearDamageBonus = 1.25f;
                }
                if (doNotYeet) {
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
        }

        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            if (dualWielding.TryGet(self, out bool dW) && dW){
                if (obj is Weapon){
                    // Any weapon is dual-wieldable, including spears
                    return Player.ObjectGrabability.OneHand;
                } else if (obj is Lizard && (obj as Lizard).Stunned){
                    // Any lizards that are haulable (while dead) or stunned are dual-wieldable
                    if (!(obj as Lizard).dead){
                        (obj as Lizard).Violence(self.bodyChunks[1], null, obj.firstChunk, null, Creature.DamageType.Blunt, 0f, 35f);
                        escort.LizardDunk = true;
                        return Player.ObjectGrabability.TwoHands;
                    }
                    return Player.ObjectGrabability.OneHand;
                } else if (escPatch_revivify && obj is Creature && (obj as Creature).abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && (obj as Creature).dead) {
                    return orig(self, obj);
                } else {
                    // Do default behaviour
                    return orig(self, obj);
                }
            } else {
                return orig(self, obj);
            }
        }


        private float Escort_DeathBiteMult(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            if (self.slugcatStats.name.value == "EscortMe"){
                return 0.3f;
            } else {
                return orig(self);
            }
        }


        private void Escort_Die(On.Player.orig_Die orig, Player self){
            if (!escort.ParrySuccess){
                orig(self);
                if (self.dead && soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn){
                    self.room.PlaySound(Escort_SFX_Death, escort.EscortSoundBodyChunk);
                    //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                }
                Debug.Log(dMe + "Failure.");
                Ebug("Death by: " + ((self.killTag!=null)? self.killTag.ToString():"Unknown reasons"));
            } else {
                self.dead = false;
                Ebug("Player didn't die?");
                escort.ParrySuccess = false;
            }
            return;
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