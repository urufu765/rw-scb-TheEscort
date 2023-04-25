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
        private void Esclass_Test_Update(Player self){    
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            // Parry
            e.UpdateParryCD(self);
            if (self.Consious && (self.canJump > 0 || self.wantToJump > 0) && self.input[0].pckp && (self.input[0].y < 0 || self.bodyMode == Player.BodyModeIndex.Crawl)){
                e.ThundahParry(self);
            }

            // Sparking when close to death VFX
            if (self.room != null && e.parryNum > e.parryMax - 5){
                self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.white, null, 4, 8));
            }
        }


        private void Esclass_Test2_Update(Player self){    
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

    }
}