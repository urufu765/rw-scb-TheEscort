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
    /// <summary>
    /// For methods specific to Escort that are used by all builds
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {
        

        /// <summary>
        /// Modifies <c>Player.aerobicLevel</c> by reducing it's value by 0.1f if the aerobic level > 0.1f, and also uses the same check as ingame for pouncing to replace it with a sick flip.
        /// </summary>
        /// <remarks>
        /// The sick flip may need to be moved elsewhere to ensure it works with mods like Simplified Movesets.
        /// </remarks>
        private void Escort_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }

            //Ebug(self, "Jump Triggered!");
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
                Ebug(self, "FLIPERONI GO!", 2);

                if (Esconfig_SFX(self)){
                    self.room.PlaySound(Escort_SFX_Flip, e.SFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
        }


        /// <summary>
        /// Changes how walljumps work so Escort can have wall longpounce and super wall flip
        /// </summary>
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
                Ebug(self, err);
                return;
            }
            if (!WallJumpVal.TryGet(self, out var WJV) ||
                !eCon.TryGetValue(self, out Escort e)){
                orig(self, direction);
                return;
            }

            Ebug(self, "Walljump Triggered!");
            bool wallJumper = Esconfig_WallJumps(self);
            bool longWallJump = (self.superLaunchJump > 19 && wallJumper);
            bool superWall = (Esconfig_Pouncing(self) && e.superWallFlip > (int)WJV[4]);
            bool superFlip = self.allowRoll == 15 && Esconfig_Pouncing(self);

            // If charge wall jump is enabled and is able to walljump, or if charge wall jump is disabled
            if ((wallJumper && self.canWallJump != 0) || !wallJumper) {
                orig(self, direction);
                float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline) * (e.savingThrowed? 0.7f : 1f);
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
                    self.room.PlaySound(SoundID.Slugcat_Normal_Jump, e.SFXChunk, false, 1f, 0.7f);
                } 
                else {
                    self.bodyChunks[0].vel.y = ((longWallJump || (superFlip && superWall))? WJV[0] : 8f) * n;
                    self.bodyChunks[1].vel.y = ((longWallJump || (superFlip && superWall))? WJV[1] : 7f) * n;
                    self.bodyChunks[0].vel.x = ((superFlip && superWall)? WJV[2] : 7f) * n * (float)direction;
                    self.bodyChunks[1].vel.x = ((superFlip && superWall)? WJV[3] : 6f) * n * (float)direction;
                    self.standing = true;
                    self.jumpStun = 8 * direction;
                    if (superWall){
                        self.room.PlaySound((self.superLaunchJump > 19? SoundID.Slugcat_Super_Jump : SoundID.Slugcat_Wall_Jump), e.SFXChunk, false, 1f, 0.7f);
                    }
                    toPrint.SetValue("Not Water", 1);
                    Ebug(self, "Y Velocity" + self.bodyChunks[0].vel.y, 2);
                    Ebug(self, "Y Velocity" + self.bodyChunks[1].vel.y, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[0].vel.x, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[1].vel.x, 2);
                }
                self.jumpBoost = 0f;

                if (superFlip && superWall){
                    self.animation = Player.AnimationIndex.Flip;
                    self.room.PlaySound((Esconfig_SFX(self)? Escort_SFX_Flip : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.SFXChunk, false, (Esconfig_SFX(self)? 1f : 1.4f), 0.9f);
                    self.jumpBoost += Mathf.Lerp(WJV[6], WJV[7], Mathf.InverseLerp(WJV[4], WJV[5], e.superWallFlip));
                    toPrint.SetValue("SUPERFLIP", 2);
                } else {
                    toPrint.SetValue("not so flip", 2);
                }
                Ebug(self, "Jumpboost" + self.jumpBoost, 2);
                Ebug(self, "SWallFlip" + e.superWallFlip, 2);
                Ebug(self, "SLaunchJump" + self.superLaunchJump, 2);
                if (self.superLaunchJump > 19){
                    self.superLaunchJump = 0;
                }
                self.canWallJump = 0;
                Ebug(self, toPrint, 2);
            }
        }


        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!Esconfig_WallJumps(self)){
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
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
                if(e.consoleTick == 0){
                    Ebug(self, msg, 2);
                }
            }
        }


        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
            } catch (Exception err){
                orig(self);
                Ebug(self, err);
                return;
            }
            if(!Esconfig_WallJumps(self)){
                orig(self);
                return;
            }

            //Ebug(self, "CheckInput Triggered!");
            int previously = self.input[0].x;
            orig(self);

            // Undoes the input cancellation
            if(self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1){
                if (self.input[0].x == 0){
                    self.input[0].x = previously;
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
                if (!BodySlam.TryGet(self, out var bodySlam) ||
                    !eCon.TryGetValue(self, out Escort e)){
                    return;
                }

                Ebug(self, "Toss Object Triggered!");
                if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead){
                    if (Esconfig_SFX(self) && e.LizGet != null){
                        e.LizGet.Volume = 0f;
                    }
                    if (self.bodyMode == Player.BodyModeIndex.Default){
                        self.animation = Player.AnimationIndex.RocketJump;
                        self.bodyChunks[1].vel.x += self.ThrowDirection;
                    }
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
        }


        private void Escort_Die(On.Player.orig_Die orig, Player self){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    orig(self);
                    return;
                }

                Ebug(self, "Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0){
                    orig(self);
                    if (self.dead && Esconfig_SFX(self) && self.room != null){
                        self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug(self, "Failure.", 1);
                } else {
                    self.dead = false;
                    Ebug(self, "Player didn't die?", 1);
                    e.ParrySuccess = false;
                }
            } catch (Exception err){
                Ebug(self, err, "Something happened while trying to die!");
                orig(self);
            }
        }

        private void Escort_Eated(On.Player.orig_BiteEdibleObject orig, Player self, bool eu)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, eu);
                    return;
                }
                for (int a = 0; a < 2; a++){
                    if (self.grasps[a] != null && self.grasps[a].grabbed is IPlayerEdible ipe && ipe.Edible){
                        if (ipe.BitesLeft > 1){
                            if (self.grasps[a].grabbed is Fly){
                                for (int b = 0; b < ipe.BitesLeft; b++){
                                    ipe.BitByPlayer(self.grasps[a], eu);
                                }
                            } else {
                                ipe.BitByPlayer(self.grasps[a], eu);
                            }
                        }
                        break;
                    }
                }
                orig(self, eu);
            } catch (Exception err){
                Ebug(self, err, "Error when eated!");
                orig(self, eu);
                return;
            }
        }


    }
}
