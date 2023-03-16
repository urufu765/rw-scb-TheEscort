using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;

using static SlugBase.Features.FeatureTypes;

namespace SlugTemplate
{
    [BepInPlugin(MOD_ID, "Escort n Co", "0.0.13")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "urufudoggo.theescort";

        public static readonly PlayerFeature<bool> BetterPounce = PlayerBool("theescort/better_pounce");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("theescort/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("theescort/mean_lizards");
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");
        public static readonly PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");
        public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static readonly PlayerFeature<float> CarryHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> BetterSlide = PlayerFloat("theescort/better_slide");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DropKick = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");
        public static readonly PlayerFeature<bool> Elevator = PlayerBool("theescort/elevator");


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.Jump += Player_Jump;
            On.Player.UpdateBodyMode += new On.Player.hook_UpdateBodyMode(this.Player_UpdateBodyMode);
            On.Player.UpdateAnimation += new On.Player.hook_UpdateAnimation(this.Player_UpdateAnimation);
            On.Player.Collide += new On.Player.hook_Collide(this.Player_Collision);
            On.Player.HeavyCarry += new On.Player.hook_HeavyCarry(this.Player_HeavyCarry);
            //On.Player.Die += Player_Die;
            On.Lizard.ctor += Lizard_ctor;
            On.Player.AerobicIncrease += Player_AerobicIncrease;
            On.Creature.Violence += new On.Creature.hook_Violence(this.Player_Violence);
            }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        // Implement MeanLizards
        private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if(MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }

        private void Player_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            orig(self, f);

            Exhausion.TryGet(self, out var stamina);
            if (self.slugcatStats.name.value == "EscortMe"){
                self.aerobicLevel = Mathf.Min(1f, self.aerobicLevel + (f / stamina));
            } else {
                self.aerobicLevel = Mathf.Min(1f, self.aerobicLevel + f / 9f);
            }
        }

        // Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (BetterPounce.TryGet(self, out var willPounce))
            //Debug.Log("Meow");
            {
                if (self.slugcatStats.name.value == "EscortMe"){
                    //Debug.Log("Escort Jumps!");

                    // Replace chargepounce with a sick flip
                    if (willPounce && (self.superLaunchJump >= 20 || self.simulateHoldJumpButton == 6 || self.killSuperLaunchJumpCounter > 0)){
                        Debug.Log("FLIPERONI GO!");
                        self.animation = Player.AnimationIndex.Flip;
                    }
                }
            }
        }


        // Implement Heavylifter
        private bool Player_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            orig(self, obj);
            if (self.slugcatStats.name.value == "EscortMe"){
                    CarryHeavy.TryGet(self, out var ratioed);
                    if (self.Grabability(obj) != Player.ObjectGrabability.TwoHands && obj.TotalMass <= self.TotalMass * ratioed){
                        if (ModManager.CoopAvailable){
                            Player player = obj as Player;
                            if (player != null){
                                return !player.isSlugpup;
                            }
                        }
                        return false;
                    }
                    return true;
            } else {
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
                }
                return true;
            }
        }


        // Implement Movementthings
        private void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);

            if (self.slugcatStats.name.value == "EscortMe"){
            BetterCrawl.TryGet(self, out var crawlSpeed);
            BetterPoleWalk.TryGet(self, out var poleMove);


            // Implement bettercrawl
            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                self.dynamicRunSpeed[0] = crawlSpeed[0] * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = crawlSpeed[1] * self.slugcatStats.runspeedFac;
            }

            // Implement betterpolewalk
            /*
            The hangfrombeam's speed does not get affected by dynamicrunspeed apparently so that's fun... 
            still the standonbeam works but also at the same time not as I initially thought.
            The slugcat apparently has a limit on how fast they can move on the beam while standing on it, leaning more and more foreward and getting more and more friction as a result...
            or to that degree.
            */
            else if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam && (self.animation == Player.AnimationIndex.StandOnBeam || self.animation == Player.AnimationIndex.HangFromBeam)){
                self.dynamicRunSpeed[0] = poleMove[0] * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = poleMove[1] * self.slugcatStats.runspeedFac;
            }
            }
        }


        // Implement Movementtech
        private void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self){
            orig(self);
            if (self.slugcatStats.name.value == "EscortMe"){
                float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                // Infiniroll
                if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                    self.rollCounter = 0;
                }/* else {  // Inspired from the decompiled code
                    self.rollDirection = 0;
                    self.room.PlaySound(SoundID.Slugcat_Roll_Finish, self.mainBodyChunk.pos, 1f, 1f);
                    self.animation = Player.AnimationIndex.None;
                    self.standing = (self.input[0].y > -1); 
                    return;
                }*/

                // I'll find out how to implement a more lineant slide (like rivulet's slide pounces) while keeping it short (like every other slugcats) one day...
                if (self.animation == Player.AnimationIndex.BellySlide){
                    // TODO implement better slide
                }
            }
        }


        // Implement Parryslide/midair projectile grab
        private void Player_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){

            if (self is Player){  // Check if self is player such that when class is converted it does not cause an error
                ParrySlide.TryGet((self as Player), out var enableParry);  // connects to the Escort's Parryslide option

                if ((self as Player).slugcatStats.name.value == "EscortMe" && enableParry){
                    // Parryslide (parry module)
                    Debug.Log("Escort attempted a Parryslide");
                    if ((self as Player).animation == Player.AnimationIndex.BellySlide && (type == Creature.DamageType.Bite || type == Creature.DamageType.Stab)){  // When Escort is sliding, parry bites and stabs
                        Debug.Log("Escort is being bit or stabbed");
                        if (type == Creature.DamageType.Bite && source.owner is Creature){
                            (source.owner as Creature).LoseAllGrasps();
                            (source.owner as Creature).stun = 20;
                            int direction;
                            direction = (self as Player).slideDirection;
                            (self as Player).WallJump(direction);
                            Debug.Log("Apparently Escort got released and is fine");
                        }
                        type = Creature.DamageType.Blunt;
                        Debug.Log("Escort gets hit by an invisible rock instead");
                        damage = 0; stunBonus = 0; (self as Player).newToRoomInvinsibility = 100;

                        // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
                        // Visual indicator doesn't work ;-;
                        //Color playerColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                        //RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.white;
                        //RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.black;
                        self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, self.mainBodyChunk);
                        //RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.white;
                        //RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = playerColor;
                        Debug.Log("Parry successful!");
                    }
                }
            }

            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        }


        // Implement Bodyslam
        private void Player_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);


            if (self.slugcatStats.name.value == "EscortMe"){

            BodySlam.TryGet(self, out var bodySlam);
            Elevator.TryGet(self, out var yeet);

            if (otherObject is Creature && (!ModManager.CoopAvailable || !(otherObject is Player) || RWCustom.Custom.rainWorld.options.friendlyFire)){
                float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

                // Creature Trampoline (or if enabled Escort's Elevator)
                if (self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                    if (self.jumpBoost == 0 || yeet){
                        self.jumpBoost += 4;
                    }
                }

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,self.mainBodyChunk);
                    (otherObject as Creature).SetKillTag(self.abstractCreature);
                    (otherObject as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        otherObject.firstChunk, null, Creature.DamageType.Blunt,
                        bodySlam[0], bodySlam[1]
                    );
                    int direction;
                    if (self.pickUpCandidate is Spear){  // Attempts to pickup spears (may pickup things higher in priority that are nearby)
                        self.PickupPressed();
                    }

                    direction = self.flipDirection;
                    self.WallJump(direction);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 5f, 5.5f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    self.animation = Player.AnimationIndex.Flip;

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
                    (otherObject as Creature).Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*multiplier, self.mainBodyChunk.vel.y*multiplier)),
                        otherObject.firstChunk, null, Creature.DamageType.Blunt,
                        bodySlam[2], bodySlam[3]
                    );
                    int direction;
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }
                    direction = -self.flipDirection;
                    self.WallJump(direction);
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    }
                }
            }
        }


        // Implement ExlodeOnDeath
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if(!wasDead && self.dead
                && ExplodeOnDeath.TryGet(self, out bool explode)
                && explode)
            {
                // Adapted from ScavengerBomb.Explode
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));

                room.ScreenMovement(pos, default, 1.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
            }
        }
    }
}