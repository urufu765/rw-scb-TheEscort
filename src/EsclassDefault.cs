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
    /// For methods that need to be split. Default is the launchpad.
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {
        public static readonly PlayerFeature<bool> InstaHype = PlayerBool("theescort/insta_hype");

        /// <summary><list type="bullet">
        /// <item>0: Min speed</item>
        /// <item>1: Max speed</item>
        /// <item>2: Non-hyped speed</item>
        /// </list></summary>
        public static readonly PlayerFeature<float[]> BetterCrawl = PlayerFloats("theescort/better_crawl");

        /// <summary><list type="bullet">
        /// <item>0: Min speed</item>
        /// <item>1: Max speed</item>
        /// <item>2: Non-hyped speed</item>
        /// </list></summary>
        public static readonly PlayerFeature<float[]> BetterPoleWalk = PlayerFloats("theescort/better_polewalk");

        /// <summary><list type="bullet">
        /// <item>0: Head min speed</item>
        /// <item>1: Head max speed</item>
        /// <item>2: Body min speed</item>
        /// <item>3: Body max speed</item>
        /// <item>4: Non-hyped head speed</item>
        /// <item>5: Non-hyped body speed</item>
        /// </list></summary>
        public static readonly PlayerFeature<float[]> BetterPoleHang = PlayerFloats("theescort/better_polehang");


        private void Esclass_Tick(Player self){
            if (!eCon.TryGetValue(self, out Escort e) || !CR.TryGet(self, out int limiter)){
                return;
            }

            // Build-specific ticks
            if (e.Brawler) Esclass_BL_Tick(self, ref e);
            if (e.Deflector) Esclass_DF_Tick(self, ref e);
            if (e.Escapist) Esclass_EC_Tick(self, ref e);
            if (e.Railgunner) Esclass_RG_Tick(self, ref e);
            if (e.Speedster) Esclass_SS_Tick(self, ref e);


            // Console ticker
            if (e.consoleTick > limiter){
                e.consoleTick = 0;
            }
            else {
                e.consoleTick++;
            }

            // Dropkick damage cooldown
            if (e.DropKickCD > 0){
                e.DropKickCD--;
            }

            // Parry leniency when triggering stunslide first (may not actually occur)
            if (e.parrySlideLean > 0){
                e.parrySlideLean--;
            }

            // Parry leniency when not touching the ground
            if (e.parryAirLean > 0 && self.canJump == 0){
                e.parryAirLean--;
            } else if (self.canJump != 0){
                e.parryAirLean = 20;
            }


            // Headbutt cooldown
            if (e.CometFrames > 0){
                e.CometFrames--;
            } else {
                e.Cometted = false;
            }

            // Invincibility Frames
            if (e.iFrames > 0){
                Ebug(self, "IFrames: " + e.iFrames, 2);
                e.iFrames--;
            } else {
                e.ElectroParry = false;
                e.savingThrowed = false;
            }

            // Smooth color/brightness transition
            if (requirement <= self.aerobicLevel && e.smoothTrans < 15){
                e.smoothTrans++;
            }
            else if (requirement > self.aerobicLevel && e.smoothTrans > 0){
                e.smoothTrans--;
            }

            // Lizard dropkick leniency
            if (e.LizDunkLean > 0){
                e.LizDunkLean--;
            }

            // Lizard grab timer
            if (e.LizGoForWalk > 0){
                e.LizGoForWalk--;
            } else {
                e.LizGrabCount = 0;
            }

            // Super Wall Flip
            if (self.input[0].x != 0 && self.input[0].y != 0){
                if (e.superWallFlip < 60){
                    e.superWallFlip++;
                }
            } else if (self.input[0].x == 0 || self.input[0].y == 0){
                if (e.superWallFlip > 3){
                    e.superWallFlip -= 3;
                } else if (e.superWallFlip > 0){
                    e.superWallFlip--;
                }
            }
        }


        /// <summary>
        /// Does a variety of things, from test logs, ticking the Escort variables, general VFX, lizard grab, easy mode, drug use, looping sfx.
        /// </summary>
        private void Escort_Update(On.Player.orig_Update orig, Player self, bool eu){
            orig(self, eu);
            try{
                if (!(self != null && self.slugcatStats != null && self.slugcatStats.name != null && self.slugcatStats.name.value != null)){
                    Ebug(self, "Attempted to access a nulled player when updating!", 0);
                    return;
                }
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }

            if (
                !WallJumpVal.TryGet(self, out float[] WJV) ||
                !eCon.TryGetValue(self, out Escort e) ||
                !InstaHype.TryGet(self, out bool inhy)
                ){
                return;
            }
            if (inhy){
                self.aerobicLevel = 0.95f;
            }

            // Cooldown/Frames Tick
            Esclass_Tick(self);
            if (e.Brawler) Esclass_BL_Update(self, ref e);
            if (e.Deflector) Esclass_DF_Update(self, ref e);
            if (e.Escapist) Esclass_EC_Update(self, ref e);
            if (e.Railgunner) Esclass_RG_Update(self, ref e);
            if (e.Speedster) Esclass_SS_Update(self, ref e);
            //if (e.EsTest) Esclass_Test_Update(self);

            // Just for seeing what a variable does.
            try{
                if(e.consoleTick == 0){
                    Ebug(self, "Clocked.");
                    Ebug(self, "X Velocity: " + self.mainBodyChunk.vel.x);
                    Ebug(self, "Y Velocity: " + self.mainBodyChunk.vel.y);
                    Ebug(self, "Dynamic Move Speed: [" + self.dynamicRunSpeed[0] + ", " + self.dynamicRunSpeed[1] + "]");
                    Ebug(self, "[Roll, Slide, Flip, Throw] Direction: [" + self.rollDirection + ", " + self.slideDirection + ", " + self.flipDirection + ", " + self.ThrowDirection + "]");
                    Ebug(self, "Rotation [x,y]: [" + self.mainBodyChunk.Rotation.x + ", " + self.mainBodyChunk.Rotation.y + "]");
                    Ebug(self, "Lizard Grab Counter: " + e.LizGrabCount);
                    Ebug(self, "How big?: " + self.TotalMass / e.originalMass);
                    if (e.Brawler){
                        Ebug(self, "Shankmode: " + e.BrawShankMode);
                    }
                    if (e.Deflector){
                        Ebug(self, "Empowered: " + e.DeflAmpTimer);
                    }
                    if (e.Escapist){
                        Ebug(self, "Ungrasp Left: " + e.EscUnGraspTime);
                    }
                    if (e.Railgunner){
                        Ebug(self, "  DoubleRock: " + e.RailDoubleRock);
                        Ebug(self, " DoubleSpear: " + e.RailDoubleSpear);
                        //Ebug(self, "  DoubleBomb: " + e.RailDoubleBomb);
                        //Ebug(self, "DoubleFlower: " + e.RailDoubleFlower);
                    }
                    if (e.Speedster){
                        Ebug(self, "Speeding Tickets: " + e.SpeSpeedin);
                        Ebug(self, "Speeding Tickets 2: " + e.SpeBuildup);
                        Ebug(self, "Charge/Gear: " + e.SpeCharge + "/" + e.SpeGear);
                        Ebug(self, "Gravity: " + self.gravity);
                        Ebug(self, "Customgrav: " + self.customPlayerGravity);
                        Ebug(self, "FrictionAir: " + self.airFriction);
                        Ebug(self, "FrictionWar: " + self.waterFriction);
                        Ebug(self, "FrictionSur: " + self.surfaceFriction);
                        Ebug(self, "FrictionMush: " + self.mushroomEffect);
                        Ebug(self, "Timesinceincorr: " + self.timeSinceInCorridorMode);
                        Ebug(self, "VerticalCorrSlideCount: " + self.verticalCorridorSlideCounter);
                        Ebug(self, "HorizontalCorrSlideCount: " + self.horizontalCorridorSlideCounter);
                    }
                }
                //Ebug(self, self.abstractCreature.creatureTemplate.baseDamageResistance);
                //Ebug(self, "Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                //Ebug(self, "Normalized direction: " + self.bodyChunks[0].vel.normalized);
            } catch (Exception err){
                Ebug(self, err, "Caught error when updating and console logging");
            }

            // vfx
            if(self != null && self.room != null){
                Esconfig_HypeReq(self);

                // Battle-hyped visual effect
                if (config.cfgNoticeHype.Value && Esconfig_Hypable(self) && Esconfig_HypeReq(self) && self.aerobicLevel > requirement){
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
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && e.superWallFlip >= (int)WJV[4] && self.allowRoll == 15){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, pounceColor));
                    }
                }
            }

            // Check if player is grabbing a lizard
            if (Esconfig_Dunkin(self)){
                try{
                    if (e.LizDunkLean == 0){
                        e.LizardDunk = false;
                    }
                    for (int i = 0; i < self.grasps.Length; i++){
                        if (self.grasps[i] != null && self.grasps[i].grabbed is Lizard lizzie && !lizzie.dead){
                            e.LizDunkLean = 20;
                            if (!e.LizardDunk){
                                e.LizGrabCount++;
                            }
                            if (e.LizGoForWalk > 0 && e.LizGrabCount < 4){
                                self.grasps[i].pacifying = true;
                                lizzie.Violence(null, null, lizzie.mainBodyChunk, null, Creature.DamageType.Electric, 0f, 40f);
                            } else {
                                self.grasps[i].pacifying = false;
                            }
                            e.LizardDunk = true;
                            break;
                        }
                    }
                } catch (Exception err){
                    Ebug(self, err, "Something went wrong when checking for lizard grasps");
                }
            }

            // Implement drug use
            if (requirement >= 0 && self.aerobicLevel < Mathf.Lerp(0f, requirement + 0.01f, self.Adrenaline)){
                self.aerobicLevel = Mathf.Lerp(0f, requirement + 0.01f, self.Adrenaline);
            }

            // Implement Easy Mode
            if (e.easyMode && (self.wantToJump > 0 && self.input[0].pckp) && self.input[0].x != 0){
                self.animation = Player.AnimationIndex.RocketJump;
                self.wantToPickUp = 0;
                e.easyKick = true;
            }

            // Implement looping SFX
            if (Esconfig_SFX(self)){
                if (self.animation == Player.AnimationIndex.Roll && e.Rollin != null){
                    e.Rollin.Update();
                } else {
                    e.RollinCount = 0f;
                }
                if (e.LizGet != null){
                    e.LizGet.Volume = (e.LizardDunk? 1f : 0f);
                    e.LizGet.Update();
                }
            }

            // optional Pole Tech
            if (config.cfgPoleBounce.Value && self.input[0].jmp && !self.input[1].jmp){
                // Normally rivulet check
                try {
                    bool flipperoni = self.animation == Player.AnimationIndex.Flip ||
                            self.animation == Player.AnimationIndex.Roll;
                    bool kickeroni = self.animation == Player.AnimationIndex.RocketJump;
                    if (
                        Mathf.Abs(self.bodyChunks[1].lastPos.y - self.bodyChunks[1].pos.y) < 35f && 
                        self.bodyChunks[1].vel.y < 9f &&
                        (
                            self.animation == Player.AnimationIndex.None ||
                            flipperoni || kickeroni
                        ) &&
                        self.bodyMode == Player.BodyModeIndex.Default &&
                        self.bodyChunks[1].contactPoint.y == 0 &&
                        self.bodyChunks[1].contactPoint.x == 0 &&
                        self.bodyChunks[0].contactPoint.y == 0 &&
                        self.bodyChunks[0].contactPoint.x == 0 &&
                        !self.IsTileSolid(1, -1, 0) && !self.IsTileSolid(1, 1, 0)
                    ) {
                        if (
                            (
                                self.room.GetTile(self.bodyChunks[1].pos).horizontalBeam || 
                                self.room.GetTile(new Vector2(self.bodyChunks[1].pos.x, self.bodyChunks[1].pos.y - 10f)).horizontalBeam) && 
                                Mathf.Min(Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].pos.y), Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].lastPos.y)
                            ) < 22.5f &&
                            self.input[0].y <= 0 &&
                            self.poleSkipPenalty < 1
                        ){
                            if (flipperoni){
                                self.bodyChunks[0].vel.y = 7f;
                                self.bodyChunks[1].vel.y = 6f;
                                self.bodyChunks[0].vel.x *= 1.08f;
                                self.bodyChunks[1].vel.x *= 1.04f;
                                self.jumpBoost += 8f;
                            }
                            else if (kickeroni){
                                self.bodyChunks[0].vel.y = 6f;
                                self.bodyChunks[1].vel.y = 5f;
                                self.bodyChunks[0].vel.x *= 1.1f;
                                self.bodyChunks[1].vel.x *= 1.2f;
                                self.jumpBoost += 7f;
                            }
                            else{
                                self.bodyChunks[0].vel.y = 5f;
                                self.bodyChunks[1].vel.y = 4f;
                                self.jumpBoost += 6f;
                            }
                            if (self.animation != Player.AnimationIndex.None){
                                for (int num8 = 0; num8 < 7; num8++){
                                    self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos + new Vector2(self.mainBodyChunk.rad * self.mainBodyChunk.vel.x, 0f), Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.mainBodyChunk.vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                                }
                                self.room.PlaySound(Escort_SFX_Pole_Bounce, e.SFXChunk);
                            } else {
                                self.room.PlaySound(SoundID.Slugcat_From_Horizontal_Pole_Jump, e.SFXChunk);
                            }
                            float n = self.room.MiddleOfTile(self.bodyChunks[1].pos).y + 5f - self.bodyChunks[1].pos.y;
                            self.bodyChunks[1].pos.y += n;
                            self.bodyChunks[0].pos.y += n;
                            self.poleSkipPenalty = 3;
                            self.wantToJump = 0;
                            self.canJump = 0;
                        } else {
                            self.poleSkipPenalty = 5;
                            self.wantToJump = 5;
                        }
                    } else {
                        self.wantToJump = 5;
                    }
                } catch (Exception err){
                    Ebug(self, err, "Pole bounce failed!");
                }
            }

            // Rotund world rotundness check then SFX!
            if (escPatch_rotundness){
                if (!e.isChunko && self.TotalMass / e.originalMass > 1.4f && self.room != null){
                    if (Esconfig_SFX(self)){
                        Ebug("Play rotund sound!");
                        self.room.PlaySound(Escort_SFX_Uhoh_Big, e.SFXChunk);
                    }
                    e.isChunko = true;
                } else if (e.isChunko && self.TotalMass / e.originalMass < 1.4f){
                    e.isChunko = false;
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
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            if (e.Deflector) Esclass_DF_UpdateAnimation(self, ref e);

            //Ebug(self, "UpdateAnimation Triggered!");
            // Infiniroll
            if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection)){
                e.RollinCount++;
                if(e.consoleTick == 0){
                    Ebug(self, "Rollin at: " + e.RollinCount, 2);
                }
                if(Esconfig_SFX(self) && e.Rollin != null){
                    e.Rollin.Volume = Mathf.InverseLerp(80f, 240f, e.RollinCount);
                }
                self.rollCounter = 0;
                self.mainBodyChunk.vel.x *= Mathf.Lerp(1, 1.25f, Mathf.InverseLerp(0, 120f, e.RollinCount));
            }

            if (self.animation != Player.AnimationIndex.Roll){
                e.RollinCount = 0f;
            }

            // Implement dropkick animation
            if(self.animation == Player.AnimationIndex.RocketJump && config.cfgDKAnimation.Value){
                Vector2 n = self.bodyChunks[0].vel.normalized;
                self.bodyChunks[0].vel -= n * 2;
                self.bodyChunks[1].vel += n * 2;
                self.bodyChunks[0].vel.y += 0.05f;
                self.bodyChunks[1].vel.y += 0.1f;
            }
            if(e.easyMode && e.easyKick){
                if (self.animation == Player.AnimationIndex.RocketJump && self.input[0].x == 0 && self.input[1].x == 0 && self.input[2].x == 0){
                    self.animation = Player.AnimationIndex.None;
                }
                if (self.animation != Player.AnimationIndex.RocketJump){
                    e.easyKick = false;
                }
            }
            if (e.Speedster) Esclass_SS_UpdateAnimation(self, ref e);


            if (e.slideFromSpear && self.animation != Player.AnimationIndex.BellySlide){
                e.slideFromSpear = false;
            }
        }

        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self){
            orig(self);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (
                !BetterCrawl.TryGet(self, out var crawlSpeed) ||
                !BetterPoleWalk.TryGet(self, out var poleWalk) ||
                !BetterPoleHang.TryGet(self, out var poleHang) ||
                !Escomet.TryGet(self, out int SetComet) ||
                !NoMoreGutterWater.TryGet(self, out float[] theGut) ||
                !eCon.TryGetValue(self, out Escort e)
            ){
                return;
            }

            bool hypedMode = Esconfig_Hypable(self);
            

            // Implement bettercrawl
            if (self.bodyMode == Player.BodyModeIndex.Crawl){
                if (!e.Deflector && hypedMode){
                    self.dynamicRunSpeed[0] = Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) * self.slugcatStats.runspeedFac;
                    self.dynamicRunSpeed[1] = Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) * self.slugcatStats.runspeedFac;
                } else {
                    self.dynamicRunSpeed[0] = crawlSpeed[2] * self.slugcatStats.runspeedFac;
                    self.dynamicRunSpeed[1] = crawlSpeed[2] * self.slugcatStats.runspeedFac;
                }
            }

            // Implement betterpolewalk
            /*
            The hangfrombeam's speed does not get affected by dynamicrunspeed apparently so that's fun... 
            still the standonbeam works but also at the same time not as I initially thought.
            The slugcat apparently has a limit on how fast they can move on the beam while standing on it, leaning more and more foreward and getting more and more friction as a result...
            or to that degree.
            */
            else if (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam){
                if (!e.Deflector && hypedMode){
                    if (self.animation == Player.AnimationIndex.StandOnBeam){
                        self.dynamicRunSpeed[0] = Mathf.Lerp(poleWalk[0], poleWalk[1], self.aerobicLevel) * self.slugcatStats.runspeedFac;
                        self.dynamicRunSpeed[1] = Mathf.Lerp(poleWalk[0], poleWalk[1], self.aerobicLevel) * self.slugcatStats.runspeedFac;
                        self.bodyChunks[1].vel.x += Mathf.Lerp(0, poleWalk[3], self.aerobicLevel) * self.input[0].x;
                    }
                    else if (self.animation == Player.AnimationIndex.HangFromBeam){
                        self.bodyChunks[0].vel.x += Mathf.Lerp(poleHang[0], poleHang[1], self.aerobicLevel) * self.input[0].x;
                        self.bodyChunks[1].vel.x += Mathf.Lerp(poleHang[2], poleHang[3], self.aerobicLevel) * self.input[0].x;
                    }
                } else {
                    if (self.animation == Player.AnimationIndex.StandOnBeam){
                        self.dynamicRunSpeed[0] = poleWalk[2] * self.slugcatStats.runspeedFac;
                        self.dynamicRunSpeed[1] = poleWalk[2] * self.slugcatStats.poleClimbSpeedFac;
                    }
                    else if (self.animation == Player.AnimationIndex.HangFromBeam){
                        self.bodyChunks[0].vel.x += poleHang[4] * self.input[0].x;
                        self.bodyChunks[1].vel.x += poleHang[5] * self.input[0].x;
                    }
                }
            } 
            
            // Set headbutt condition
            else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb){
                if (self.slowMovementStun > 0){
                    e.CometFrames = SetComet;
                }
            }

            // Implement GuuhWuuh
            if(self.bodyMode == Player.BodyModeIndex.Swimming){
                float viscoDance = self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity);

                if (self.animation == Player.AnimationIndex.DeepSwim){
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[0], (float)Math.Pow(viscoDance, theGut[6])), 
                        Mathf.Lerp(1f, (self.mainBodyChunk.vel.y > 0? theGut[1] : theGut[2]), (float)Math.Pow(viscoDance, theGut[7])));
                } else if (self.animation == Player.AnimationIndex.SurfaceSwim) {
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[3], (float)Math.Pow(viscoDance, theGut[8])), 
                        Mathf.Lerp(1f, (self.mainBodyChunk.vel.y > 0? theGut[4] : theGut[5]), (float)Math.Pow(viscoDance, theGut[9])));
                    self.dynamicRunSpeed[0] += Mathf.Lerp(theGut[10], theGut[11], (float)Math.Pow(viscoDance, theGut[12]));
                }
            }

            if (e.Speedster) Esclass_SS_UpdateBodyMode(self, ref e);
        }

        /// <summary>
        /// Implement Escort's slowed stamina increase
        /// </summary>
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f){
            if (
                !Exhausion.TryGet(self, out float exhaust) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                orig(self, f);
                return;
            }
            if (self.slugcatStats.name.value == "EscortMe"){
                // Due to the aerobic decrease found in some movements implemented in Escort, the AerobicIncrease actually does the original, and on top of that the additional to balance things out.
                if (!e.Escapist){
                    orig(self, f);
                }

                //Ebug(self, "Aerobic Increase Triggered!");
                if (!self.slugcatStats.malnourished){
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + ((e.Escapist? f * 2 : f) / exhaust));
                } else {
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + (f / (exhaust * 2)));
                }
            } else {
                orig(self, f);
            }
        }


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

                if (Esconfig_SFX(self) && !e.kickFlip){
                    self.room.PlaySound(Eshelp_SFX_Flip(), e.SFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
            if (e.Speedster) Esclass_SS_Jump(self, ref e);
        }

        /// <summary>
        /// Changes how walljumps work so Escort can have wall longpounce and super wall flip
        /// </summary>
        private void Escort_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            /*
            if (self.bodyMode != Player.BodyModeIndex.WallClimb){
                orig(self, direction);
                return;
            }*/
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
                float m = 1f;
                if (e.Speedster && e.SpeDashNCrash){
                    m = (e.SpeSecretSpeed? 2f : 0.66f);
                }
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
                    self.bodyChunks[0].vel.x = ((superFlip && superWall)? WJV[2] * m : 7f) * n * (float)direction;
                    self.bodyChunks[1].vel.x = ((superFlip && superWall)? WJV[3] * m : 6f) * n * (float)direction;
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
                    self.room.PlaySound((Esconfig_SFX(self)? Eshelp_SFX_Flip() : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.SFXChunk, false, (Esconfig_SFX(self)? 1f : 1.4f), 0.9f);
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
        /// Unsticking spears component of parryslide.
        /// </summary>
        private bool Escort_StickySpear(On.Player.orig_SpearStick orig, Player self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, source, dmg, chunk, appPos, direction);
                }
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!ParrySlide.TryGet(self, out bool parrier) ||
                !eCon.TryGetValue(self, out Escort e)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!parrier || (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            Ebug(self, "Sticky Triggered!");
            if (e.Deflector) return Esclass_DF_StickySpear(self);
            return !(self.animation == Player.AnimationIndex.BellySlide);
        }


        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
                if (!Esconfig_Heavylift(self)){
                    return orig(self, obj);
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    return orig(self, obj);
                }

                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                    if (e.consoleTick == 0){
                        Ebug(self, "Revivify skip!", 1);
                        Ebug(self, "Creature: " + creature.GetType(), 1);
                        Ebug(self, "Player: " + self.GetOwnerType(), 1);
                    }
                    return orig(self, creature);
                }


                if (e.Brawler && Esclass_BL_HeavyCarry(self, obj)){
                    //return false;
                }

                //Ebug(self, "Heavycarry Triggered!");
                if (obj.TotalMass <= self.TotalMass * ratioed){
                    if (ModManager.CoopAvailable && obj is Player player && player != null){
                        return !player.isSlugpup;
                    }
                    if (obj is Creature c && c is not Lizard && !c.dead){
                        return orig(self, obj);
                    }
                    return false;
                }
                return orig(self, obj);
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, obj);
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
                Ebug(self, err);
                return;
            }
            if (
                !bonusSpear.TryGet(self, out float[] spearDmgBonuses) ||
                !Esconfig_HypeReq(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            Ebug(self, "ThrownSpear Triggered!");
            float thrust = 7f;
            bool onPole = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
            bool doNotYeet = onPole || !Esconfig_Spears(self) || e.RailDoubleSpear;
            try{
                if (self.slugcatStats.throwingSkill == 0 && !e.Speedster){
                    spear.spearDamageBonus = 1;
                }
                if (Esconfig_Hypable(self)){
                    if (self.aerobicLevel > requirement){
                        spear.throwModeFrames = -1;
                        if (!e.Railgunner){
                            spear.spearDamageBonus *= spearDmgBonuses[0];
                        }
                        if (self.canJump != 0 && !self.longBellySlide){
                            if (!doNotYeet){
                                self.rollCounter = 0;
                                if (self.input[0].jmp && self.input[0].thrw){
                                    self.animation = Player.AnimationIndex.BellySlide;
                                    e.slideFromSpear = true;
                                    self.whiplashJump = true;
                                    spear.firstChunk.vel.x *= 1.7f;
                                    Ebug(self, "Spear Go!?", 2);
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
                                e.slideFromSpear = true;
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
                                    e.slideFromSpear = true;
                                }
                                if (self.input[0].jmp && self.input[0].thrw){
                                    spear.firstChunk.vel.x *= 1.6f;
                                    Ebug(self, "Spear Go!", 2);
                                }
                            } else {
                                self.animation = Player.AnimationIndex.Flip;
                                self.standing = false;
                            }
                        }
                        spear.spearDamageBonus *= spearDmgBonuses[1];
                        thrust = 5f;
                    }
                } else {
                    spear.spearDamageBonus *= 1.25f;
                }
            } catch (Exception err){
                Ebug(self, err, "Error while setting additional spear effects!");
            }
            if (e.Brawler) Esclass_BL_ThrownSpear(self, spear, ref e, ref thrust);
            if (e.Deflector) Esclass_DF_ThrownSpear(self, spear, ref e);
            if (e.Escapist) Esclass_EC_ThrownSpear(self, spear);
            if (e.Railgunner) Esclass_RG_ThrownSpear(self, spear, onPole, ref e, ref thrust);
            if (onPole && !e.Railgunner || self.bodyMode == Player.BodyModeIndex.Crawl) {
                thrust = 1f;
            }

            try{
                BodyChunk firstChunker = self.firstChunk;
                if ((self.room != null && self.room.gravity == 0f) || (Mathf.Abs(spear.firstChunk.vel.x) < 1f && Mathf.Abs(spear.firstChunk.vel.y) < 1f)){
                    self.firstChunk.vel += spear.firstChunk.vel.normalized * Math.Abs(thrust);
                } else {
                    if (Esconfig_Spears(self)){
                        self.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                    }
                    if (self.bodyMode == Player.BodyModeIndex.CorridorClimb){
                        spear.throwDir = new IntVector2((int)(self.mainBodyChunk.Rotation.x * 2), (int)(self.mainBodyChunk.Rotation.y * 2));
                        if (spear.throwDir.y != 0){
                            spear.firstChunk.vel.y = Mathf.Abs(spear.firstChunk.vel.x) * spear.throwDir.y;
                            spear.firstChunk.vel.x = 0;
                            thrust *= 0.25f;
                        } else {
                            thrust *= 0.5f;
                        }
                    }
                    if (self.animation != Player.AnimationIndex.BellySlide){
                        if (spear.throwDir.x == 0){
                            if (e.Railgunner){
                                if (spear.throwDir.y == 1){
                                    self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.4f;
                                } else if (spear.throwDir.y == -1){
                                    self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.65f;
                                } else {
                                    self.firstChunk.vel += spear.firstChunk.vel.normalized * thrust;
                                }
                            } else {
                                self.firstChunk.vel.y = firstChunker.vel.y + Mathf.Sign(spear.firstChunk.vel.y) * thrust;
                            }
                        } else {
                            self.firstChunk.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                        }
                    }
                }
            } catch (Exception err){
                Ebug(self, err, "Error while adjusting the player thrust");
            }            
            Ebug(self, "Speartoss! Velocity [X,Y]: [" + spear.firstChunk.vel.x + "," + spear.firstChunk.vel.y + "] Damage: " + spear.spearDamageBonus, 2);
        }

        private void Escort_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, grasp, eu);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e) ||
                    !NoMoreGutterWater.TryGet(self, out float[] theGut)){
                    orig(self, grasp, eu);
                    return;
                }
                if (e.Brawler && Esclass_BL_ThrowObject(orig, self, grasp, eu, ref e)) return;
                if (e.Railgunner && Esclass_RG_ThrowObject(orig, self, grasp, eu, ref e)) return;
            } catch (Exception err){
                Ebug(self, err, "Throwing object error!");
                orig(self, grasp, eu);
                return;
            }
            orig(self, grasp, eu);
        }

        // Implement rock throws
        private bool Escort_RockHit(On.Rock.orig_HitSomething orig, Rock self, SharedPhysics.CollisionResult result, bool eu){
            try{
                if (self.thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        return orig(self, result, eu);
                    }
                    if (!eCon.TryGetValue(p, out Escort e)){
                        return orig(self, result, eu);
                    }
                    //self.canBeHitByWeapons = true;
                    if (result.obj == null){
                        return false;
                    }
                    self.vibrate = 20;
                    self.ChangeMode(Weapon.Mode.Free);
                    if (result.obj is Creature c){
                        float stunBonus = 60f;
                        if (ModManager.MMF && MoreSlugcats.MMF.cfgIncreaseStuns.Value && (c is Cicada || c is LanternMouse || (ModManager.MSC && c is MoreSlugcats.Yeek))){
                            stunBonus = 105f;
                        }
                        if (ModManager.MSC && self.room.game.IsArenaSession && self.room.game.GetArenaGameSession.chMeta != null){
                            stunBonus = 105f;
                        }
                        c.Violence(self.firstChunk, self.firstChunk.vel * (e.RailDoubleRock? Math.Max(result.chunk.mass*0.75f, self.firstChunk.mass) : self.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, e.Railgunner? (e.RailDoubleRock? 0.15f : 0.2f): (e.Escapist? 0.1f : 0.02f), (e.Brawler? stunBonus *= 1.5f : stunBonus));
                    }
                    else if (result.chunk != null){
                        result.chunk.vel += self.firstChunk.vel * self.firstChunk.mass / result.chunk.mass;
                    }
                    else if (result.onAppendagePos != null){
                        (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
                    }
                    self.firstChunk.vel = self.firstChunk.vel * -0.5f + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude;
                    self.room.PlaySound(SoundID.Rock_Hit_Creature, self.firstChunk);
                    if (result.chunk != null)
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, result.chunk.pos + RWCustom.Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
                    }
                    self.SetRandomSpin();
                    return true;
                }

                return orig(self, result, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Exception in Rockhit!");
                return orig(self, result, eu);
            }
        }

        private void Escort_RockThrow(On.Rock.orig_Thrown orig, Rock self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu){
            try{
                if (thrownBy == null){
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (thrownBy is Player p){
                    if (p.slugcatStats.name.value != "EscortMe"){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e)){
                        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                        return;
                    }
                    if (e.Escapist){
                        frc *= 0.75f;
                    }
                    if (e.Railgunner) Esclass_RG_RockThrow(self, frc, p, ref e);
                }
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            } catch (Exception err){
                Ebug(self.thrownBy as Player, err, "Error in Rockthrow!");
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }

        }

        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
                if (obj == null){
                    return orig(self, obj);
                }
                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead) {
                    return Player.ObjectGrabability.TwoHands;
                }
                if (!dualWielding.TryGet(self, out bool dW) ||
                    !eCon.TryGetValue(self, out Escort e)){
                    return orig(self, obj);
                }

                if (dW && e.dualWield){
                    if (obj is Weapon){
                        // Any weapon is dual-wieldable, including spears
                        return Player.ObjectGrabability.OneHand;
                    }
                    if (e.Brawler && Esclass_BL_Grabability(self, obj, ref e)){
                        return Player.ObjectGrabability.BigOneHand;
                    }
                    if (obj is Lizard lizzie){
                        // Any lizards that are haulable (while dead) or stunned are dual-wieldable
                        if (lizzie.dead){
                            return Player.ObjectGrabability.OneHand;
                        } else if (lizzie.Stunned && Esconfig_Dunkin(self) && !e.LizardDunk){
                            if (e.LizGoForWalk == 0){
                                e.LizGoForWalk = 320;
                            }
                            if (!Esconfig_SFX(self)) {
                                self.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, self.mainBodyChunk);
                            }
                            e.LizardDunk = true;
                            return Player.ObjectGrabability.TwoHands;
                        }
                    }
                    if (obj is Creature c && c.TotalMass <= self.TotalMass * ratioed && c.dead){
                        return Player.ObjectGrabability.BigOneHand;
                    }
                }
                return orig(self, obj);
            } catch (Exception err){
                Ebug(self, err);
                return orig(self, obj);
            }
        }

        private float Escort_GotBit(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            try{
                if (!eCon.TryGetValue(self, out Escort e)){
                    return orig(self);
                }
                if (self.slugcatStats.name.value == "EscortMe"){
                    float biteMult = 0.5f;
                    if (e.Brawler){
                        biteMult -= 0.35f;
                    }
                    if (e.Railgunner){
                        biteMult += 0.35f;
                    }
                    if (e.Escapist){
                        biteMult -= 0.15f;
                    }
                    if (Eshelp_ParryCondition(self) || (!e.Deflector && self.animation == Player.AnimationIndex.RocketJump)){
                        biteMult = 5f;
                    }
                    Ebug(self, "Lizard bites with multiplier: " + biteMult);
                    return biteMult;
                } else {
                    return orig(self);
                }
            } catch (Exception err){
                Ebug(self, err, "Couldn't set deathbitemultipler!");
                return orig(self);
            }
        }


        // Implement Parryslide/midair projectile grab
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus){
            try{
                if (self is Player && (self as Player).slugcatStats.name.value != "EscortMe"){
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    return;
                }
            } catch (Exception err){
                Ebug((self as Player), err);
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(self is not Player player){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if(
                !ParrySlide.TryGet(player, out bool enableParry) ||
                !eCon.TryGetValue(player, out Escort e)
            ){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (!enableParry){
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }



            Ebug(player, "Violence Triggered!");
            // connects to the Escort's Parryslide option
            e.ParrySuccess = false;
            if (e.Railgunner && e.RailIReady && type != null && type == Creature.DamageType.Explosion){
                if (e.iFrames == 0){
                    e.ParrySuccess = true;
                }
                stunBonus = 0;
            }
            if (Eshelp_ParryCondition(player)){
                // Parryslide (parry module)
                Ebug(player, "Escort attempted a Parryslide", 2);
                int direction;
                direction = player.slideDirection;

                Ebug(player, "Is there a source? " + (source != null), 2);
                Ebug(player, "Is there a direction & Momentum? " + (directionAndMomentum != null), 2);
                Ebug(player, "Is there a hitChunk? " + (hitChunk != null), 2);
                Ebug(player, "Is there a hitAppendage? " + (hitAppendage != null), 2);
                Ebug(player, "Is there a type? " + (type != null), 2);
                Ebug(player, "Is there damage? " + (damage > 0f), 2);
                Ebug(player, "Is there stunBonus? " + (stunBonus > 0f), 2);

                if (source != null) {
                    Ebug(player, "Escort is being assaulted by: " + source.owner.GetType(), 2);
                }
                Ebug(player, "Escort parry is being checked", 1);
                if (type != null){
                    Ebug(player, "Escort gets hurt by: " + type.value, 2);
                if (type == Creature.DamageType.Bite){
                    Ebug(player, "Escort is getting BIT?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 35;
                        //(self as Player).WallJump(direction);
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort got out of a creature's mouth!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug(player, "Weapons can BITE?!", 2);
                    } 
                    else {
                        Ebug(player, "Where is Escort getting bit from?!", 2);
                    }
                } 
                else if (type == Creature.DamageType.Stab) {
                    Ebug(player, "Escort is getting STABBED?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a stabby creature?", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon) {
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                        damage = 0f;
                        type = Creature.DamageType.Blunt;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a stabby weapon", 2);
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
                                Ebug(player, "Tusk unimpaled!", 2);
                                Debug.LogError("-> Escort: Please pay no attention to this! This is how Escort parry works (on King Tusks)!");
                                break;
                            }
                        }
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried a generic stabby thing", 2);
                    }
                } 
                else if (type == Creature.DamageType.Blunt) {
                    Ebug(player, "Escort is getting ROCC'ED?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "Creatures aren't rocks...", 2);
                    } 
                    else if (source != null && source.owner is Weapon weapon){
                        Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                        weapon.WeaponDeflect(weapon.firstChunk.lastPos, -vector, weapon.firstChunk.vel.magnitude);
                        damage = 0f;
                        stunBonus = stunBonus / 5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort bounces a blunt thing.", 2);
                    } 
                    else {
                        damage = 0f;
                        stunBonus = 0f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parried something blunt.", 2);
                    }
                } 
                else if (type == Creature.DamageType.Water) {
                    Ebug(player, "Escort is getting Wo'oh'ed?!", 1);
                } 
                else if (type == Creature.DamageType.Explosion) {
                    Ebug(player, "Escort is getting BLOWN UP?!", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "Wait... creatures explode?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parries an explosion from weapon?!", 2);
                    } 
                    else {
                        player.WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        type = Creature.DamageType.Blunt;
                        damage = 0f;
                        stunBonus = stunBonus * 1.5f;
                        e.ParrySuccess = true;
                        Ebug(player, "Escort parries an explosion", 2);
                    }
                } 
                else if (type == Creature.DamageType.Electric) {
                    Ebug(player, "Escort is getting DEEP FRIED?!", 1);
                    if (source != null && source.owner is Creature creature){
                        creature.LoseAllGrasps();
                        creature.stun = 20;
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        //type = Creature.DamageType.Blunt;
                        damage = 0f;
                        //(self as Player).LoseAllGrasps();
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        // NOTE TO SELF: Centipede parry goes here
                        Ebug(player, "Escort somehow parried a shock from creature?!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        //(self as Player).WallJump(direction);
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        //type = Creature.DamageType.Blunt;
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug(player, "Escort somehow parried a shock object?!", 2);
                    } 
                    else {
                        player.animation = Player.AnimationIndex.Flip;
                        //player.Jump();
                        
                        damage = 0f;
                        e.ParrySuccess = true;
                        e.ElectroParry = true;
                        Ebug(player, "Escort attempted to parry a shock but why?!", 2);
                    }
                } 
                else {
                    Ebug(player, "Escort is getting UNKNOWNED!!! RUNNN", 1);
                    if (source != null && source.owner is Creature){
                        Ebug(player, "IT'S ALSO AN UNKNOWN CREATURE!!", 2);
                    } 
                    else if (source != null && source.owner is Weapon){
                        Ebug(player, "IT'S ALSO AN UNKNOWN WEAPON!!", 2);
                    } 
                    else {
                        Ebug(player, "WHO THE HECK KNOWS WHAT IT IS?!", 2);
                    }
                }
                }
            }
            else if (Eshelp_SavingThrow(player, source, type)){
                e.ParrySuccess = true;
                (source.owner as Creature).LoseAllGrasps();
                type = Creature.DamageType.Blunt;
                damage = 0f;
                stunBonus = 0f;
            }
            // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
            // Visual indicator doesn't work ;-;
            if (e.ParrySuccess){
                if (player.abstractCreature.world.game.IsArenaSession && !e.DeflTrampoline){
                    player.abstractCreature.world.game.GetArenaGameSession.arenaSitting.players[0].parries++;
                }
                if (e.Deflector){
                    stunBonus = 0;
                    player.Jump();
                    player.animation = Player.AnimationIndex.Flip;
                    player.mainBodyChunk.vel.y *= 1.5f;
                    player.mainBodyChunk.vel.x *= 0.15f;
                    if (e.DeflSFXcd == 0){
                        self.room.PlaySound(SoundID.Snail_Warning_Click, self.mainBodyChunk, false, 1.6f, 0.65f);
                        e.DeflSFXcd = 9;
                    }
                    e.DeflAmpTimer = 160;
                    e.DeflTrampoline = false;
                }
                else {
                    self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                    self.room.PlaySound(Escort_SFX_Parry, e.SFXChunk);
                }
                if (self != null && self.room != null && self.mainBodyChunk != null){
                    for (int c = 0; c < 7; c++){
                        self.room.AddObject(new WaterDrip(self.bodyChunks[1].pos + new Vector2(self.mainBodyChunk.rad * self.bodyChunks[1].vel.x, 0f), RWCustom.Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.bodyChunks[1].vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                        //self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.grey, null, 3, 6));
                        //self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos, self.mainBodyChunk.vel * Mathf.Lerp(1f, 4f, UnityEngine.Random.value) * player.flipDirection + RWCustom.Custom.RNV() * UnityEngine.Random.value * 9f, false));
                    }
                }
                Ebug(player, "Parry successful!", 1);
                e.iFrames = 6;
                e.parrySlideLean = 0;
                if (e.Railgunner && e.RailIReady){
                    if (e.RailBombJump){
                        self.mainBodyChunk.vel.x = 0;
                    }
                    self.stun = 0;
                }
                self.AllGraspsLetGoOfThisObject(false);
            }
            else if (e.iFrames > 0) {
                if (e.Railgunner && e.RailIReady){
                    self.stun = 0;
                    if (e.RailBombJump){
                        self.mainBodyChunk.vel.x = 0;
                    }
                    if (e.iFrames <= 0){
                        e.RailIReady = false;
                        e.RailBombJump = false;
                    }
                }
                if (e.ElectroParry){
                    damage = 0f;
                    stunBonus = stunBonus * 0.5f;
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    Ebug(player, "Stun Resistance frame tick", 2);
                } else {
                    if (e.Railgunner && e.RailIReady){
                        e.RailIReady = false;
                        e.RailBombJump = false;
                    }
                    Ebug(player, "Immunity frame tick", 2);
                }
            } 
            else {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                Ebug(player, "Nothing or not possible to parry!", 1);
            }
            Ebug(player, "Parry Check end", 1);
            return;

        }


        // Implement Bodyslam
        private void Escort_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk){
            orig(self, otherObject, myChunk, otherChunk);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (
                !BodySlam.TryGet(self, out float[] bodySlam) ||
                !TrampOhLean.TryGet(self, out float bounce) ||
                !SlideLaunchMod.TryGet(self, out float[] slideMod) ||
                !brawlerSlideLaunchFac.TryGet(self, out float bSlideFac) ||
                !brawlerDKHypeDmg.TryGet(self, out float bDKHDmg) ||
                !deflectorSlideLaunchMod.TryGet(self, out float dSlideMod) ||
                !deflectorSlideDmg.TryGet(self, out float dSlideDmg) ||
                !deflectorSlideLaunchFac.TryGet(self, out float dSlideFac) ||
                !deflectorDKHypeDmg.TryGet(self, out float[] dDKHDmg) ||
                !escapistSlideLaunchMod.TryGet(self, out float eSlideMod) ||
                !escapistSlideLaunchFac.TryGet(self, out float eSlideFac) ||
                !escapistNoGrab.TryGet(self, out int[] esNoGrab) ||
                !Esconfig_HypeReq(self) ||
                !Esconfig_DKMulti(self) ||
                !eCon.TryGetValue(self, out Escort e)
                ){
                return;
            }

            //Ebug(self, "Collision Triggered!");
            try{
                if (e.consoleTick == 0){
                    Ebug(self, "Escort collides!");
                    Ebug(self, "Has physical object? " + otherObject != null);
                    if (otherObject != null){
                        Ebug(self, "What is it? " + otherObject.GetType());
                    }
                }
            } catch (Exception err){
                Ebug(self, err, "Error when printing collision stuff");
            }

            bool hypedMode = Esconfig_Hypable(self);

            // Reimplementing the elevator... the way it was in its glory days
            if (Esconfig_Elevator(self) && otherObject is Creature && self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead){
                self.jumpBoost += 4;
            }


            if (otherObject is Creature creature && 
                creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !(ModManager.CoopAvailable && otherObject is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                
                if (e.Escapist && self.aerobicLevel > 0.02f){
                    self.aerobicLevel -= 0.01f;
                }
                if (e.Speedster) Esclass_SS_Collision(self, creature, ref e);

                // Creature Trampoline (or if enabled Escort's Elevator)
                /*
                Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
                */
                if ((self.animation == Player.AnimationIndex.Flip || (e.Escapist && self.animation == Player.AnimationIndex.None)) && self.bodyMode == Player.BodyModeIndex.Default && (!creature.dead || creature is Lizard)){
                    self.slideCounter = 0;
                    if (self.jumpBoost <= 0) {
                        self.jumpBoost = (self.animation == Player.AnimationIndex.None? bounce * 1.5f: bounce);
                    }
                    if (e.Escapist && self.cantBeGrabbedCounter < esNoGrab[1] && self.animation == Player.AnimationIndex.Flip){
                        self.cantBeGrabbedCounter += esNoGrab[0];
                    }
                    if (e.Deflector){
                        try{
                            if (self.mainBodyChunk.vel.y < 6f){
                                self.mainBodyChunk.vel.y += 10f;
                            }
                            e.DeflTrampoline = true;
                            self.Violence(null, null, self.mainBodyChunk, null, Creature.DamageType.Blunt, 0f, 0f);
                        } catch (Exception err){
                            Ebug(self, err, "Hitting thyself failed!");
                        }
                    }
                }

                int direction;

                // Parryslide (stun module)
                if (self.animation == Player.AnimationIndex.BellySlide){
                    try{

                    creature.SetKillTag(self.abstractCreature);

                    if (e.parrySlideLean <= 0){
                        e.parrySlideLean = 4;
                    }
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard,e.SFXChunk);

                    float normSlideStun = (hypedMode || e.Brawler? bodySlam[1] * 1.5f : bodySlam[1]);
                    if (hypedMode && self.aerobicLevel > requirement){
                        normSlideStun = bodySlam[1] * (e.Brawler? 2f : 1.75f);
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x/4f, self.mainBodyChunk.vel.y/4f)),
                        creature.firstChunk, null, (e.DeflAmpTimer > 0? Creature.DamageType.Stab : Creature.DamageType.Blunt),
                        (e.DeflAmpTimer > 0? dSlideDmg : bodySlam[0]), normSlideStun
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
                        if (Esconfig_Spears(self)){
                            float tossModifier = slideMod[0];
                            if (e.Deflector){
                                tossModifier = dSlideMod;
                            }
                            else if (e.Escapist){
                                tossModifier = eSlideMod;
                            }
                            self.animation = Player.AnimationIndex.BellySlide;
                            self.bodyChunks[1].vel = new Vector2((float)self.slideDirection * tossModifier, slideMod[1]);
                            self.bodyChunks[0].vel = new Vector2((float)self.slideDirection * tossModifier, slideMod[2]);
                        } else {
                            self.animation = Player.AnimationIndex.Flip;
                        }
                        Ebug(self, "Greatdadstance stunslide!", 2);
                    } else {
                        direction = self.flipDirection;
                        if (e.Brawler){
                            self.mainBodyChunk.vel.x *= bSlideFac;
                        }
                        if (e.Deflector){
                            self.mainBodyChunk.vel *= dSlideFac;
                        }
                        if (e.Escapist){
                            self.mainBodyChunk.vel.y *= eSlideFac;
                        }
                        self.WallJump(direction);
                        self.animation = Player.AnimationIndex.Flip;
                        Ebug(self, "Stunslided!", 2);
                    }
                    } catch (Exception err){
                        Ebug(self, err, "Error while Slidestunning!");
                    }
                }

                // Dropkick
                else if (self.animation == Player.AnimationIndex.RocketJump){
                    try{

                    creature.SetKillTag(self.abstractCreature);

                    String message = (e.easyKick? "Easykicked!" : "Dropkicked!");
                    
                    if (!creature.dead) {
                        DKMultiplier *= creature.TotalMass;
                    }
                    float normSlamDamage = (e.easyKick? 0.001f : 0.05f);
                    if (e.DropKickCD == 0){
                        self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk, false, 1f, 0.65f);
                        normSlamDamage = (hypedMode ? bodySlam[2] : bodySlam[2] + (e.Brawler? 0.27f : 0.15f));
                        creature.LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > requirement) {normSlamDamage = bodySlam[2] * (e.Brawler? bDKHDmg : 1.6f);}
                        if (e.Deflector){
                            if (e.DeflAmpTimer > 0){
                                normSlamDamage *= dDKHDmg[0];
                            }
                            else {
                                normSlamDamage *= dDKHDmg[1];
                            }
                        }
                        message = "Powerdropkicked!";
                    } else {
                        self.room.PlaySound(SoundID.Big_Needle_Worm_Bounce_Terrain, self.mainBodyChunk, false, 1f, 0.9f);
                    }
                    creature.Violence(
                        self.mainBodyChunk, new Vector2?(new Vector2(self.mainBodyChunk.vel.x*DKMultiplier, self.mainBodyChunk.vel.y*DKMultiplier*(e.LizardDunk?0.2f:1f))),
                        creature.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, (e.DeflAmpTimer > 0? bodySlam[1] : bodySlam[3])
                    );
                    Ebug(self, "Dunk the lizard: " + e.LizardDunk, 2);
                    if (e.DropKickCD == 0){
                        e.LizardDunk = false;
                    }
                    if (e.DeflAmpTimer > 0){
                        e.DeflAmpTimer = 0;
                    }
                    e.DropKickCD = (e.easyKick? 40 : 15);
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    /*
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }*/
                    direction = -self.flipDirection;
                    e.kickFlip = true;
                    self.WallJump(direction);
                    e.kickFlip = false;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    Ebug(self, message + " Dmg: " + normSlamDamage, 2);
                    } catch (Exception err){
                        Ebug(self, err, "Error when dropkicking!");
                    }
                    e.savingThrowed = false;
                }

                // Headbutt
                else if (e.CometFrames > 0 && !e.Cometted){
                    try{
                    creature.SetKillTag(self.abstractCreature);
                    creature.Violence(
                        self.bodyChunks[0], new Vector2?(new Vector2(self.bodyChunks[0].vel.x*(DKMultiplier*0.5f)*creature.TotalMass, self.bodyChunks[0].vel.y*(DKMultiplier*0.5f)*creature.TotalMass)),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        0f, 15f
                    );
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x*(DKMultiplier*0.5f)*creature.TotalMass;
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y*(DKMultiplier*0.5f)*creature.TotalMass;
                    if (self != null && self.room != null){
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    }
                    Ebug(self, "Headbutted!", 2);
                    if (self.room != null){
                        if (Esconfig_SFX(self)){
                            self.room.PlaySound(Escort_SFX_Boop, e.SFXChunk);
                        }
                        self.room.PlaySound(SoundID.Slugcat_Floor_Impact_Standard, e.SFXChunk, false, 0.75f, 1.3f);
                    }
                    e.Cometted = true;
                    } catch (Exception err){
                        Ebug(self, err, "Error when headbutting!");
                    }
                }

            }
        }


        private bool Escort_SpearGet(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return orig(self, obj);
                }
            } catch (Exception err){
                Ebug(self, err, "Grab update!");
                return orig(self, obj);
            }
            if(
                !eCon.TryGetValue(self, out Escort e)
                ){
                return orig(self, obj);
            }
            if (e.Railgunner && Esclass_RG_SpearGet(obj)){
                return true;
            }
            if (obj != null && obj is Weapon w && !(ModManager.CoopAvailable && w.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire) && w.mode == Weapon.Mode.Thrown){
                Ebug(self, "Hehe, yoink!");
                return true;
            }
            return orig(self, obj);
        }
    }
}
