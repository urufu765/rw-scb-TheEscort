using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;

using static SlugBase.Features.FeatureTypes;

namespace SlugTemplate
{
    [BepInPlugin(MOD_ID, "Escort n Co", "0.0.11.3")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "urufudoggo.theescort";

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("theescort/super_jump");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("theescort/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("theescort/mean_lizards");
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");
        public static readonly PlayerFeature<float[]> BodySlam = PlayerFloats("theescort/body_slam");
        public static readonly PlayerFeature<float> CarryHeavy = PlayerFloat("theescort/heavylifter");
        public static readonly PlayerFeature<float> BetterSlide = PlayerFloat("theescort/better_slide");
        public static readonly PlayerFeature<float> Exhausion = PlayerFloat("theescort/exhausion");
        public static readonly PlayerFeature<float> DropKick = PlayerFloat("theescort/dk_multiplier");
        public static readonly PlayerFeature<bool> ParrySlide = PlayerBool("theescort/parry_slide");


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            //On.Player.Jump += Player_Jump;
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

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
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
            // Implement bettercrawl
            BetterCrawl.TryGet(self, out var crawlSpeed);


            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                self.dynamicRunSpeed[0] = crawlSpeed[0] * self.slugcatStats.runspeedFac;
                self.dynamicRunSpeed[1] = crawlSpeed[1] * self.slugcatStats.runspeedFac;
            }
            }
        }


        // Implement Movementtech
        private void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self){
            orig(self);
            if (self.slugcatStats.name.value == "EscortMe"){
                float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                if (self.animation == Player.AnimationIndex.Roll){
                    self.rollCounter = 0;
                }
                if (self.animation == Player.AnimationIndex.BellySlide){
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
                        Color playerColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                        RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.white;
                        RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.black;
                        self.room.PlaySound(SoundID.Lizard_Head_Shield_Deflect, self.mainBodyChunk);
                        RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = Color.white;
                        RainWorld.PlayerObjectBodyColors[(self.State as PlayerState).playerNumber] = playerColor;
                        Debug.Log("Parry successful!");
                    }
                }  // After testing, the parry is successful, but nothing happens and the Escort just dies... maybe the "self" loses the pointer to the specific slugcat and thus doesn't affect it? Or maybe once the conversion is done it's done as read-only?
            }

            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        }


        // Implement Bodyslam
        private void Player_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);


            if (self.slugcatStats.name.value == "EscortMe"){

            BodySlam.TryGet(self, out var bodySlam);

            if (otherObject is Creature && (!ModManager.CoopAvailable || !(otherObject is Player) || !RWCustom.Custom.rainWorld.options.friendlyFire)){
                float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

                // Creature Trampoline
                if (self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                    self.jumpBoost += 4;
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