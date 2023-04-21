using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace TheEscort{
    public partial class Escort{
        public bool EsTest = false;
        public int parryCD = 0;
        public int parryNum = 0;
        public int parryMax = 10;
        public int parryClock = 240;

        public void UpdateParryCD(Player self){
            if (this.parryCD > 0){
                this.parryCD--;
            }
            if (!self.Stunned && this.parryNum > 0){
                if (this.parryClock > 0){
                    this.parryClock--;
                }
                else if (this.parryClock == 0){
                    this.parryNum--;
                    this.parryClock = 240;
                }
            }
        }

        public void ThundahParry(Player self){
            // Nullcheck
            if (!(self != null && self.room != null && self.room.physicalObjects != null)){
                Debug.LogError("Null check!");
                return;
            }

            if (this.parryCD == 0){
                // Set cooldown
                this.parryCD = 40;
                this.parryNum++;

                // Ranges (for easy value change without needing to look through the code ;) )
                float weaponRange = 300f;  // Artificer default
                float creatureRange = 200f;  // Artificer default
                float maxStunDuration = 280f;  // Each second is 40 ticks

                // Colors
                Color explosionColor = new Color(0.7f, 1f, 1f);
                Color sparkColor = RWCustom.Custom.HSL2RGB(UnityEngine.Random.Range(0.55f, 0.7f), UnityEngine.Random.Range(0.8f, 1f), UnityEngine.Random.Range(0.3f, 0.6f));

                // Parry code
                Vector2 pos = self.firstChunk.pos;
                List<Weapon> weaponList = new List<Weapon>();

                for (int i = 0; i < self.room.physicalObjects.Length; i++){
                    for (int j = 0; j < self.room.physicalObjects[i].Count; j++){
                        // Weapon checks (to see if they're in range)
                        if (
                            self.room.physicalObjects[i][j] is Weapon weapon && 
                            weapon.mode == Weapon.Mode.Thrown && 
                            RWCustom.Custom.Dist(pos, weapon.firstChunk.pos) < weaponRange
                        ){
                            weaponList.Add(weapon);
                        }

                        // Creature checks

                        // Check if selfhit, a player hit (if friendlyfire is enabled) or creature hit
                        bool hitCreature = !ModManager.CoopAvailable || RWCustom.Custom.rainWorld.options.friendlyFire || !(self.room.physicalObjects[i][j] is Player p) || p.isNPC;
                        if (!(self.room.physicalObjects[i][j] is Creature c0 && c0 != self && hitCreature)){
                            continue;
                        }

                        Creature c = self.room.physicalObjects[i][j] as Creature;

                        // Check if creature is too far from stun distance
                        if (
                            !(RWCustom.Custom.Dist(pos, c.firstChunk.pos) < creatureRange) ||
                            (
                                !(RWCustom.Custom.Dist(pos, c.firstChunk.pos) < 60f) && 
                                !self.room.VisualContact(self.abstractCreature.pos, c.abstractCreature.pos)
                            )
                        ) {
                            continue;
                        }

                        // Apply parry effect to creature
                        self.room.socialEventRecognizer.WeaponAttack(null, self, c, true);
                        c.SetKillTag(self.abstractCreature);
                        //c.Stun(80);
                        //c.firstChunk.vel = RWCustom.Custom.DegToVec(RWCustom.Custom.AimFromOneVectorToAnother(pos, c.firstChunk.pos)) * 30f;
                        c.Violence(
                            self.firstChunk, 
                            RWCustom.Custom.DegToVec(RWCustom.Custom.AimFromOneVectorToAnother(pos, c.firstChunk.pos)) * 30f, 
                            c.firstChunk, 
                            null, 
                            Creature.DamageType.Electric, 
                            0.1f,
                            (maxStunDuration * (1 - (RWCustom.Custom.Dist(pos, c.firstChunk.pos) / creatureRange)) * Mathf.Lerp(c.Template.baseStunResistance, 1f, 0.5f))
                        );
                        self.room.AddObject(new CreatureSpasmer(c, allowDead: false, c.stun));

                        if (c is TentaclePlant){
                            for (int k = 0; k < c.grasps.Length; k++){
                                c.ReleaseGrasp(k);
                            }
                        }
                    }
                }

                // Apply parry effect to weapons
                foreach (Weapon w in weaponList){
                    w.ChangeMode(Weapon.Mode.Free);
                    w.firstChunk.vel = RWCustom.Custom.DegToVec(RWCustom.Custom.AimFromOneVectorToAnother(pos, w.firstChunk.pos)) * 20f;
                    w.SetRandomSpin();
                }

                // And stun yourself.
                if (this.parryNum >= this.parryMax){
                    DeathUponThee(self);
                } else {
                    self.Stun(80);
                    self.room.AddObject(new CreatureSpasmer(self, allowDead: false, 60));
                }


                // VFX
                self.room.AddObject(new ShockWave(pos, 200f, 0.2f, 10));
                self.room.AddObject(new Explosion.ExplosionLight(pos, 200f, 1f, 6, explosionColor));
                self.room.AddObject(new ZapCoil.ZapFlash(pos, 25f));
                for (int m = 0; m < 10; m++){
                    Vector2 v = RWCustom.Custom.RNV();
                    self.room.AddObject(new Spark(
                        pos + v * UnityEngine.Random.value * 20f,
                        v * Mathf.Lerp(6f, 18f, UnityEngine.Random.value),
                        sparkColor, null, 4, 18
                    ));
                }

                // SFX
                self.room.PlaySound(SoundID.Fire_Spear_Pop, pos);
                self.room.PlaySound(SoundID.Firecracker_Bang, pos);
                self.room.PlaySound(SoundID.Zapper_Zap, pos, 0.95f, 0.9f + 0.25f * UnityEngine.Random.value);
                self.room.InGameNoise(new Noise.InGameNoise(pos, 800f, self, 1f));
            }
        }


        public void DeathUponThee(Player self){
            Vector2 v = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            self.room.AddObject(new SootMark(self.room, v, 120f, bigSprite:true));
            self.room.AddObject(new Explosion(self.room, self, v, 8, 500f, 60f, 1f, 360f, 0.4f, self, 0.05f, 120f, 0f));
            self.room.ScreenMovement(v, default(Vector2), 1.5f);
            self.room.PlaySound(SoundID.Bomb_Explode, self.mainBodyChunk);
            self.Die();
        }

        public bool thing (Player self, Vector2 direction){
            if (self.dead) return true;

            else if (self.standing){
                if (Mathf.Abs(self.firstChunk.vel.x) < 2f){
                    return true;
                }
                else {
                    if (Mathf.Sign(direction.x) == Mathf.Sign(self.firstChunk.vel.x) && Mathf.Abs(direction.x) > 2f){
                        return false;
                    }
                    return true;
                }
            
            }

            return false;
        }
    }
}