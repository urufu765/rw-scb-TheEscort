using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// Implementing an artificer parry... electric style!
        /// </summary>
        private void Estest_1_Update(On.Player.orig_Update orig, Player self, bool eu){    
            orig(self, eu);
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            // Parry
            e.UpdateParryCD(self);

            // Air input
            bool airParry = (self.input[0].pckp && !self.input[1].pckp) && self.wantToJump > 0;
            
            // Ground input
            int tolerance = 3;
            bool gParryLeanPckp = false, gParryLeanJmp = false;
            if (!airParry){
                for (int i = 0; i <= tolerance; i++){
                    if (self.input[i].pckp){
                        gParryLeanPckp = i < tolerance;
                    }
                    if (self.input[i].jmp){
                        gParryLeanJmp = i < tolerance;
                    }
                }
            }
            bool groundParry = self.input[0].y < 0 && self.bodyChunks[1].contactPoint.y < 0 && gParryLeanJmp && gParryLeanPckp;
            if (self.Consious && (airParry || groundParry)){
                Debug.Log("Input - Air: " + airParry);
                Debug.Log("Input - Ground: " + groundParry);
                e.ThundahParry(self);
            }

            // Sparking when close to death VFX
            if (self.room != null && e.parryNum > e.parryMax - 5){
                self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.white, null, 4, 8));
            }
        }

        /// <summary>
        /// Implementing not dying by falling into a deathpit
        /// </summary>
        private void Estest_2_Update(Player self){    
            if (self.GetCat().updateTimer > 0){
                self.GetCat().updateTimer--;
            }
            else{
                self.GetCat().updateTimer = 40;
                // Every 1 second, copy the positions of the bodychunk if the player is standing on ground
                if (self.bodyChunks[1].contactPoint.y == -1){
                    self.GetCat().setSafe(self.bodyChunks);
                    Ebug(self, "Set safe platform");
                }
            }

            if (self.room != null){
                // Shift detection of deathpit depending on the room
                float deathPit = 0f - self.bodyChunks[0].restrictInRoomRange + 1f;

                // Change detection range to match with game code
                if (self.bodyChunks[0].restrictInRoomRange == self.bodyChunks[0].defaultRestrictInRoomRange){
                    deathPit = (self.bodyMode != Player.BodyModeIndex.WallClimb)? Mathf.Max(deathPit, -470f) : Mathf.Max(deathPit, -220f);
                } else {
                    deathPit += 30f;
                }

                // Prevent death.
                if (self.bodyChunks[0].pos.y < deathPit && (!self.room.water || self.room.waterInverted || self.room.defaultWaterLevel < -10) && (!self.Template.canFly || self.Stunned || self.dead) && self.playerState.foodInStomach >= 2){
                    self.SubtractFood(2);
                    Ebug(self, "No die!");
                    // Set position
                    for (int x = 0; x < self.bodyChunks.Length; x++){
                        self.bodyChunks[x].pos = self.GetCat().safeBodyChunksPos[x];
                        self.bodyChunks[x].vel *= 0.1f;
                    }
                    // Play sound
                    self.room.PlaySound(SoundID.HUD_Karma_Reinforce_Bump, self.mainBodyChunk, false, 1f, 1.6f);
                }
            }
        }

        /// <summary>
        /// Implementing healing an injured creature the player grabs
        /// </summary>
        private void Estest_3_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            // Slugcat check
            try{
                // Null slugcat check (you never know)
                if (!(self != null && self.slugcatStats != null && self.slugcatStats.name != null)){
                    Debug.Log("Attempted to access a nulled player when updating!");
                    return;
                }
                // Custom Scug check
                if (self.slugcatStats.name != EscortMe){
                    return;
                }
                // CWT access just to check for if character is for testing purposes (legacy code go brr)
                if (!eCon.TryGetValue(self, out Escort e)){
                    return;
                }
                if (!e.EsTest){
                    return;
                }
            } catch (Exception err){
                Debug.LogException(err);  // LOG!
                return;
            }
            // When player presses the pickup button after grabbing a creature (the self.noPickUpOnRelease prevents the player from applying the medic thing on the same button press as when they picked up the creature)
            if (self.input[0].pckp && !self.input[1].pckp && self.noPickUpOnRelease < 1){
                Debug.Log("Attempt at medic");
                for (int i = 0; i < self.grasps.Length; i++){
                    if (self.grasps[i] != null && self.grasps[i].grabbed != null && self.grasps[i].grabbed is Creature c){
                        Debug.Log("Found injured creature!");
                        // Creature health check (assumes their health is 1 or their healthstate is based on 0%-100%)
                        if ((c.abstractCreature.state as HealthState).health > 0 && (c.abstractCreature.state as HealthState).health < 1){
                            Debug.Log("Holding injured creature!");
                            //replace this with item usage... I'm using food pips for now
                            if (self.playerState.foodInStomach >= 1){
                                Debug.Log("Apply medic!");  // LOG!
                                self.SubtractFood(1);  // remove food

                                // Apply health 
                                (c.abstractCreature.state as HealthState).health = Mathf.Min(1f, (c.abstractCreature.state as HealthState).health + 0.2f); // Regenerate 0.2f (20%) of creature health (and prevent it from going higher than 100% health using .Min())
                            }
                        }
                    }
                }
            }
        }


    }
}