using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace TheEscort
{
    [BepInPlugin(MOD_ID, "Escort n Co", "0.1.4.4")]
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
        public static readonly PlayerFeature<float[]> guuhWuuh = PlayerFloats("theescort/guuh_wuuh");

        private static readonly String dMe = "<EscortMe> ";

        public static SoundID EscortDeath;
        public static SoundID EscortFlip;
        public static SoundID EscortRoll;
        //public DynamicSoundLoop escortRollin;

        private int slowDownDevConsole = 0;


        private bool escPatch_revivify = false;
        

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += new On.RainWorld.hook_OnModsInit(this.RainWorld_OnModsInit);
            On.RainWorld.PostModsInit += Escort_PostInit;

            On.Player.Jump += Escort_Jump;
            On.Player.UpdateBodyMode += new On.Player.hook_UpdateBodyMode(this.Escort_UpdateBodyMode);
            On.Player.UpdateAnimation += new On.Player.hook_UpdateAnimation(this.Escort_UpdateAnimation);
            On.Player.Collide += new On.Player.hook_Collide(this.Escort_Collision);
            On.Player.HeavyCarry += new On.Player.hook_HeavyCarry(this.Escort_HeavyCarry);
            On.Lizard.ctor += new On.Lizard.hook_ctor(this.Escort_Lizard_ctor);
            //On.Player.ctor += new On.Player.hook_ctor(this.Escort_ctor);
            On.Player.AerobicIncrease += Escort_AerobicIncrease;
            On.Creature.Violence += new On.Creature.hook_Violence(this.Escort_Violence);
            On.Player.Update += new On.Player.hook_Update(this.Escort_Update);
            On.Player.ThrownSpear += new On.Player.hook_ThrownSpear(this.Escort_ThrownSpear);
            On.Player.Grabability += new On.Player.hook_Grabability(this.Escort_Grabability);
            On.Player.Die += new On.Player.hook_Die(this.Escort_Die);
            //On.Water.Update += new On.Water.hook_Update(this.Escort_Water_Update);
            }
            
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            EscortDeath = new SoundID("Escort_Failure", true);
            EscortFlip = new SoundID("Escort_Flip", true);
            EscortRoll = new SoundID("Escort_Roll", true);
        }


        private void Escort_PostInit(On.RainWorld.orig_PostModsInit orig, RainWorld self){
            orig(self);
            if (ModManager.ActiveMods.Exists(mod => mod.id == "revivify")){
                Debug.Log(dMe + "Found Revivify! Applying patch...");
                escPatch_revivify = true;
            }
        }


        private void Update_Refresher(Player self){
            if (RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                slowDownDevConsole = 0;
            } else {
                slowDownDevConsole++;
            }
        }


        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self){
            orig(self);
            Extras.WrapInit(LoadResources);
            EscEnums.RegisterValues();
            MachineConnector.SetRegisteredOI("urufudoggo.theescort", this.config);
            
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
/*
        private void Escort_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world){
            escort = new Escort(self);
            orig(self, abstractCreature, world);
        }
*/
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
            
            // Just for seeing what a variable does.
            if(RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                Debug.Log(dMe + "Clocked.");
            }


            if(Hyped.TryGet(self, out bool hypedMode) && StaReq.TryGet(self, out float requirement)){
                if (hypedMode && self.aerobicLevel > requirement){
                    Color playerColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    playerColor.a = 0.8f;

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, playerColor));
                }
            }

            // Implement guuh wuuh
            if(guuhWuuh.TryGet(self, out var theGut) && self.bodyMode == Player.BodyModeIndex.Swimming){
                float superSwim = Mathf.Lerp(theGut[0], theGut[1], self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity));
                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[2] * superSwim, theGut[3] * superSwim);
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        theGut[4] * superSwim, theGut[5] * superSwim);
                }
            }

            /*
            if (soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn){
                if (this.escortRollin == null){
                    this.escortRollin = new ChunkDynamicSoundLoop(self.mainBodyChunk);
                    this.escortRollin.sound = EscortRoll;
                    Debug.Log(dMe + "Sound Initialized.");
                } else {
                    if (self.animation == Player.AnimationIndex.Roll){
                        this.escortRollin.Update();
                    }
                    if(limiter < slowDownDevConsole){
                        Debug.Log(dMe + "Updated... with: " + EscortRoll != null);
                        Debug.Log(dMe + "Updated... with: " + EscortRoll != null);
                        Debug.Log(dMe + "Updated... with: " + escortRollin.sound != null);
                    }
                }
            }*/
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
                        willPounce && (self.superLaunchJump >= 20 || self.simulateHoldJumpButton == 6 || self.killSuperLaunchJumpCounter > 0)
                        ){
                        Debug.Log(dMe + "FLIPERONI GO!");
                        Room room = self.room;
                        room.PlaySound(EscortFlip, self.mainBodyChunk.pos);
                        self.animation = Player.AnimationIndex.Flip;
                    }
                }
            }
        }


        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            if (CarryHeavy.TryGet(self, out var ratioed)){
                if (self.Grabability(obj) != Player.ObjectGrabability.TwoHands && obj.TotalMass <= self.TotalMass * ratioed){
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
                        EscEnums.rollin++;
                        if(RR.TryGet(self, out int limiter) && limiter < slowDownDevConsole){
                            Debug.Log(dMe + "ROLLIN at:" + EscEnums.rollin++);
                        }

                        //this.escortRollin.Volume = Mathf.InverseLerp(120f, 360f, EscEnums.rollin);
                        self.rollCounter = 0;
                    } else {
                        //this.escortRollin.Volume = 0f;
                        EscEnums.rollin = 0f;
                    }
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
                ParryTest.TryGet((self as Player), out bool standParry) &&
                (standParry? (self as Player).bodyMode == Player.BodyModeIndex.Crawl : (self as Player).animation == Player.AnimationIndex.BellySlide)){

                    // Parryslide (parry module)
                    bool parrySuccess = false;
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
                            damage = damage / 5f;
                            stunBonus = 0f;
                            parrySuccess = true;
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
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort parried a stabby creature?");
                        } else if (source != null && source.owner is Weapon) {
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            (source.owner as Weapon).WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                            damage = 0f;
                            type = Creature.DamageType.Blunt;
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort parried a stabby weapon");
                        } else {
                            damage = damage / 5f;
                            type = Creature.DamageType.Blunt;
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort parried a generic stabby thing");
                        }
                    } else if (type == Creature.DamageType.Blunt) {
                        Debug.Log(dMe + "Escort is getting ROCC'ED?!");
                        if (source != null && source.owner is Creature){
                            Debug.Log(dMe + "Creatures aren't rocks...");
                        } else if (source != null && source.owner is Weapon){
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            (source.owner as Weapon).WeaponDeflect(source.owner.firstChunk.lastPos, -vector, source.owner.firstChunk.vel.magnitude);
                            damage = damage / 7f;
                            stunBonus = stunBonus / 5f;
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort bounces a blunt thing.");
                        } else {
                            damage = damage / 4f;
                            stunBonus = 0f;
                            parrySuccess = true;
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
                            damage = damage / 15f;
                            stunBonus = stunBonus / 2f;
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort parries an explosion from weapon?!");
                        } else {
                            (self as Player).WallJump(direction);
                            (self as Player).animation = Player.AnimationIndex.Roll;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = stunBonus / 2f;
                            parrySuccess = true;
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
                            damage = damage / 10f;
                            stunBonus = stunBonus / 2f;
                            (self as Player).LoseAllGrasps();
                            parrySuccess = true;
                            Debug.Log(dMe + "Escort somehow parried a shock from creature?!");
                        } else if (source != null && source.owner is Weapon){
                            (self as Player).WallJump(direction);
                            (self as Player).animation = Player.AnimationIndex.Flip;
                            type = Creature.DamageType.Blunt;
                            damage = damage / 10f;
                            stunBonus = stunBonus / 2f;
                            parrySuccess = true;
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

                    // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
                    // Visual indicator doesn't work ;-;
                    if (parrySuccess){

                        self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                        Debug.Log(dMe + "Parry successful!");

                    } else {
                        Debug.Log(dMe + "... nor an object");

                    }
                    Debug.Log(dMe + "Parry Check end");

                }
            }
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

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
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,self.mainBodyChunk);
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
                    self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk);
                    
                    DropKick.TryGet(self, out var multiplier);
                    if (!(otherObject as Creature).dead) {
                        multiplier *= (otherObject as Creature).TotalMass;
                    }
                    (otherObject as Creature).SetKillTag(self.abstractCreature);
                    (otherObject as Creature).LoseAllGrasps();
                    float normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + 0.15f);
                    if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * 1.6f;}
                    (otherObject as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*multiplier, self.mainBodyChunk.vel.y*multiplier)),
                        otherObject.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, bodySlam[3]
                    );
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
                    Debug.Log(dMe + "Dropkicked!");
                    }
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
                    firstChunker.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                }
            }
        }

        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            if (dualWielding.TryGet(self, out bool dW) && dW){
                if (obj is Weapon){
                    // Any weapon is dual-wieldable, including spears
                    return Player.ObjectGrabability.OneHand;
                } else if (obj is Lizard && (obj as Lizard).dead){
                    // Any lizards that are haulable are dual-wieldable
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

        private void Escort_Die(On.Player.orig_Die orig, Player self){
            orig(self);
            Room room = self.room;
            if (self.dead && soundsAhoy.TryGet(self, out bool sfxOn) && sfxOn){
                room.PlaySound(EscortDeath, self.mainBodyChunk.pos);
                //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
            }
            Debug.Log(dMe + "Failure.");

        }
    }

}