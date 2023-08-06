using BepInEx;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using Newtonsoft.Json;
using static UrufuCutsceneTool.CsInLogger;

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
        

        private int filenum;

        private void Esclass_Tick(Player self)
        {
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return;
            }

            // Build-specific ticks
            if (e.Brawler) Esclass_BL_Tick(self, ref e);
            if (e.Deflector) Esclass_DF_Tick(self, ref e);
            if (e.Escapist) Esclass_EC_Tick(self, ref e);
            if (e.NewEscapist) Esclass_NE_Tick(self, ref e);
            if (e.Railgunner) Esclass_RG_Tick(self, ref e);
            if (e.Speedster) Esclass_SS_Tick(self, ref e);
            if (e.Gilded) Esclass_GD_Tick(self, ref e);

            // Dropkick damage cooldown
            if (e.DropKickCD > 0)
            {
                e.DropKickCD--;
            }

            // Parry leniency when triggering stunslide first (may not actually occur)
            if (e.parrySlideLean > 0)
            {
                e.parrySlideLean--;
            }

            // Parry leniency when not touching the ground
            if (e.parryAirLean > 0 && self.canJump == 0)
            {
                e.parryAirLean--;
            }
            else if (self.canJump != 0)
            {
                e.parryAirLean = 20;
            }

            // Headbutt cooldown
            if (e.CometFrames > 0)
            {
                e.CometFrames--;
            }
            else
            {
                e.Cometted = false;
            }

            // Invincibility Frames
            if (e.iFrames > 0)
            {
                Ebug(self, "IFrames: " + e.iFrames, 2);
                e.iFrames--;
            }
            else
            {
                e.ElectroParry = false;
                e.savingThrowed = false;
            }

            // Smooth color/brightness transition
            if (hypeRequirement <= self.aerobicLevel && e.smoothTrans < 15)
            {
                e.smoothTrans++;
            }
            else if (hypeRequirement > self.aerobicLevel && e.smoothTrans > 0)
            {
                e.smoothTrans--;
            }

            // Lizard dropkick leniency
            if (e.LizDunkLean > 0)
            {
                e.LizDunkLean--;
            }

            // Lizard grab timer
            if (e.LizGoForWalk > 0)
            {
                e.LizGoForWalk--;
            }
            else
            {
                e.LizGrabCount = 0;
            }

            // Super Wall Flip
            if (self.input[0].x != 0 && self.input[0].y != 0)
            {
                if (e.superWallFlip < 60)
                {
                    e.superWallFlip++;
                }
            }
            else if (self.input[0].x == 0 || self.input[0].y == 0)
            {
                if (e.superWallFlip > 3)
                {
                    e.superWallFlip -= 3;
                }
                else if (e.superWallFlip > 0)
                {
                    e.superWallFlip--;
                }
            }

            // Vengeful lizard location updater clock
            if (e.lizzieVengenceClock > 0)
            {
                e.lizzieVengenceClock--;
            }
            else
            {
                e.lizzieVengenceClock = 400;
            }
            if (e.lizzieVengenceTick > 0)
            {
                e.lizzieVengenceTick--;
            }
            else
            {
                e.lizzieVengenceTick = 40;
            }

            // Escort touching acid cooldown
            if (e.acidRepetitionGuard > 0)
            {
                e.acidRepetitionGuard--;
            }

            // Escort vertical pole penalty
            if (e.verticalPoleFail > 0)
            {
                e.verticalPoleFail--;
            }
            if (self.bodyChunks[1].contactPoint.y < 0)
            {
                e.verticalPoleTech = false;
                e.verticalPoleToggle = false;
            }

            if (e.offendingRemoval > 0)
            {
                e.offendingRemoval--;
            }

            if (e.tryFindingPup > 0)
            {
                e.tryFindingPup--;
            }
        }


        /// <summary>
        /// Does a variety of things, from test logs, ticking the Escort variables, general VFX, lizard grab, easy mode, drug use, looping sfx.
        /// </summary>
        private void Escort_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    Ebug(self, "Attempted to access a nulled player when updating!", 0);
                    return;
                }
                // ONE OF THESE WILL TELL ME WHY THE FUCK THE SCUG'S CONTROLLER GETS YEETED OUT THE WINDOW I SWEAR TO FUCKING GOD
                if (false && (bool)!self.room?.game?.paused){
                    Ebug(new object[]{
                             "X: ", self.input[0].x,
                           "| Y: ", self.input[0].y,
                         "| Jmp: ", self.input[0].jmp,
                          "| JS: ", self.jumpStun,
                          "| FC: ", self.freezeControls
                    });
                }

                if (logForCutscene && self.playerState.playerNumber == 0)
                {
                    this.GetCSIL().Capture(self);
                    if (Input.GetKeyDown("-"))
                    {
                        this.GetCSIL().Release($"cutscene_input_log{filenum}");
                        filenum++;
                    }
                }
                //self.jumpStun = 0;
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return;
            }

            if (
                !WallJumpVal.TryGet(self, out float[] WJV) ||
                !eCon.TryGetValue(self, out Escort e) ||
                !InstaHype.TryGet(self, out bool inhy)
                )
            {
                return;
            }
            if (inhy)
            {
                self.aerobicLevel = 0.95f;
            }

            // Cooldown/Frames Tick
            Esclass_Tick(self);
            if (e.Brawler) Esclass_BL_Update(self, ref e);
            if (e.Deflector) Esclass_DF_Update(self, ref e);
            if (e.Escapist) Esclass_EC_Update(self, ref e);
            if (e.NewEscapist) Esclass_NE_Update(self, ref e);
            if (e.Railgunner) Esclass_RG_Update(self, ref e);
            if (e.Speedster) Esclass_SS_Update(self, ref e);
            if (e.Gilded) Esclass_GD_Update(self, ref e);
            //if (e.EsTest) Estest_2_Update(self);


            // Secret color tick
            if (e.secretRGB) {
                e.Escat_runit_thru_RGB(
                    e.hypeColor, 
                    hypeRequirement < self.aerobicLevel ? 8f: Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, hypeRequirement, self.aerobicLevel))
                );
            }

            // Just for seeing what a variable does.
            try
            {
                if (false && CR.TryGet(self, out int limiter)){
                    // Console ticker
                    if (e.consoleTick > limiter)
                    {
                        e.consoleTick = 0;
                    }
                    else
                    {
                        e.consoleTick++;
                    }

                    if (e.consoleTick == 0)
                    {
                        Ebug(self, "Clocked.");
                        if (e.Gilded)
                        {
                            Ebug(self, "Power: " + e.GildPower);
                            Ebug(self, "SPowr: " + e.GildStartPower);
                            Ebug(self, "Resrv: " + e.GildReservePower);
                            Ebug(self, "Requi: " + e.GildRequiredPower);
                            Ebug(self, "Float: " + e.GildLevitateLimit);
                            Ebug(self, "WTThr: " + e.GildWantToThrow);
                            Ebug(self, "LOCKY: " + e.GildLockRecharge);
                            Ebug(self, "CANCL: " + e.GildCancel);
                        }
                        Ebug(self, "X: " + self.mainBodyChunk.pos.x);
                        Ebug(self, "Y: " + self.mainBodyChunk.pos.y);
                        #if false
                        Ebug(self, "X Velocity: " + self.mainBodyChunk.vel.x);
                        Ebug(self, "Y Velocity: " + self.mainBodyChunk.vel.y);
                        Ebug(self, "Dynamic Move Speed: [" + self.dynamicRunSpeed[0] + ", " + self.dynamicRunSpeed[1] + "]");
                        Ebug(self, "[Roll, Slide, Flip, Throw] Direction: [" + self.rollDirection + ", " + self.slideDirection + ", " + self.flipDirection + ", " + self.ThrowDirection + "]");
                        Ebug(self, "Rotation [x,y]: [" + self.mainBodyChunk.Rotation.x + ", " + self.mainBodyChunk.Rotation.y + "]");
                        Ebug(self, "Lizard Grab Counter: " + e.LizGrabCount);
                        Ebug(self, "How big?: " + self.TotalMass / e.originalMass);
                        if (e.Brawler)
                        {
                            Ebug(self, "Shankmode: " + e.BrawShankMode);
                        }
                        if (e.Deflector)
                        {
                            Ebug(self, "Empowered: " + e.DeflAmpTimer);
                        }
                        if (e.Escapist)
                        {
                            Ebug(self, "Ungrasp Left: " + e.EscUnGraspTime);
                        }
                        if (e.Railgunner)
                        {
                            Ebug(self, "  DoubleRock: " + e.RailDoubleRock);
                            Ebug(self, " DoubleSpear: " + e.RailDoubleSpear);
                            //Ebug(self, "  DoubleBomb: " + e.RailDoubleBomb);
                            //Ebug(self, "DoubleFlower: " + e.RailDoubleFlower);
                        }
                        if (e.Speedster)
                        {
                            Ebug(self, "Speeding Tickets: " + e.SpeSpeedin);
                            Ebug(self, "Speeding Tickets 2: " + e.SpeBuildup);
                            Ebug(self, "Charge/Gear: " + e.SpeCharge + "/" + e.SpeGear);
                            //Ebug(self, "Gravity: " + self.gravity);
                            //Ebug(self, "Customgrav: " + self.customPlayerGravity);
                            //Ebug(self, "FrictionAir: " + self.airFriction);
                            //Ebug(self, "FrictionWar: " + self.waterFriction);
                            //Ebug(self, "FrictionSur: " + self.surfaceFriction);
                            //Ebug(self, "FrictionMush: " + self.mushroomEffect);
                            //Ebug(self, "Timesinceincorr: " + self.timeSinceInCorridorMode);
                            //Ebug(self, "VerticalCorrSlideCount: " + self.verticalCorridorSlideCounter);
                            //Ebug(self, "HorizontalCorrSlideCount: " + self.horizontalCorridorSlideCounter);
                        }
                        #endif
                    }
                    //Ebug(self, self.abstractCreature.creatureTemplate.baseDamageResistance);
                    //Ebug(self, "Perpendicularvector: " + RWCustom.Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos));
                    //Ebug(self, "Normalized direction: " + self.bodyChunks[0].vel.normalized);
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Caught error when updating and console logging");
            }

            // Vengeful Lizards
            if (self != null && self.room != null && self.room.game != null && self.room.game.IsStorySession && Esconfig_Vengeful_Lizards())
            {
                if (e.playerKillCount < self.SessionRecord.kills.Count)
                {
                    int a = 0;
                    foreach (PlayerSessionRecord.KillRecord killz in self.SessionRecord.kills)
                    {
                        if (killz.lizard) a++;
                    }
                    if (e.lizzieVengenceCount < a)
                    {
                        e.lizzieVengenceCount = a;
                    }
                    e.playerKillCount = self.SessionRecord.kills.Count;
                }
                e.Escat_ping_lizards(self);
            }

            // vfx
            if (self != null && self.room != null)
            {
                Esconfig_HypeReq(self);

                // Battle-hyped visual effect
                if (config.cfgNoticeHype.Value && Esconfig_Hypable(self) && Esconfig_HypeReq(self) && self.aerobicLevel > hypeRequirement)
                {
                    Color hypedColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);
                    hypedColor.a = 0.8f;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 1, 11f, 8f, 11f, 15f, hypedColor));
                }

                // Charged pounces Visual Effect
                if (Esconfig_Pouncing(self))
                {
                    Color pounceColor = PlayerGraphics.SlugcatColor((self.State as PlayerState).slugcatCharacter);

                    if (self.superLaunchJump > 19)
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 9f, 4f, 4f, 11f, pounceColor));
                    }
                    if (self.bodyMode == Player.BodyModeIndex.WallClimb && e.superWallFlip >= (int)WJV[4] && self.allowRoll == 15)
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 2, 10f, 4f, 11f, 4f, pounceColor));
                    }
                }
            }

            // Check if player is grabbing a lizard
            if (Esconfig_Dunkin())
            {
                try
                {
                    if (e.LizDunkLean == 0)
                    {
                        e.LizardDunk = false;
                    }
                    for (int i = 0; i < self.grasps.Length; i++)
                    {
                        if (self.grasps[i] != null && self.grasps[i].grabbed is Lizard lizzie && !lizzie.dead)
                        {
                            e.LizDunkLean = 20;
                            if (!e.LizardDunk)
                            {
                                e.LizGrabCount++;
                            }
                            if (e.LizGoForWalk > 0 && e.LizGrabCount < 4)
                            {
                                self.grasps[i].pacifying = true;
                                lizzie.Violence(null, null, lizzie.mainBodyChunk, null, Creature.DamageType.Electric, 0f, 40f);
                            }
                            else
                            {
                                self.grasps[i].pacifying = false;
                            }
                            e.LizardDunk = true;
                            break;
                        }
                    }
                }
                catch (Exception err)
                {
                    Ebug(self, err, "Something went wrong when checking for lizard grasps");
                }
            }

            // Implement drug use
            if (hypeRequirement >= 0 && self.aerobicLevel < Mathf.Lerp(0f, hypeRequirement + 0.01f, self.Adrenaline))
            {
                self.aerobicLevel = Mathf.Lerp(0f, hypeRequirement + 0.01f, self.Adrenaline);
            }

            // Implement Easy Mode
            if (e.easyMode && (self.wantToJump > 0 && self.input[0].pckp) && self.input[0].x != 0)
            {
                self.animation = Player.AnimationIndex.RocketJump;
                self.wantToPickUp = 0;
                e.easyKick = true;
            }

            // Implement looping SFX
            if (Esconfig_SFX(self))
            {
                if (self.animation == Player.AnimationIndex.Roll && e.Rollin != null)
                {
                    e.Rollin.Update();
                }
                else
                {
                    e.RollinCount = 0f;
                }
                if (e.LizGet != null)
                {
                    e.LizGet.Volume = (e.LizardDunk ? 1f : 0f);
                    e.LizGet.Update();
                }
            }

            // optional Pole Tech
            if ((config.cfgPoleBounce.Value || e.isDefault) && self.input[0].jmp && !self.input[1].jmp)
            {
                // Normally rivulet check
                try
                {
                    bool flipperoni = self.animation == Player.AnimationIndex.Flip;
                    bool kickeroni = self.animation == Player.AnimationIndex.RocketJump;
                    bool rollaunch = self.animation == Player.AnimationIndex.Roll;
                    if (
                        Mathf.Abs(self.bodyChunks[1].lastPos.y - self.bodyChunks[1].pos.y) < 35f &&
                        self.bodyChunks[1].vel.y < 9f &&
                        (
                            self.animation == Player.AnimationIndex.None ||
                            flipperoni || kickeroni || rollaunch
                        ) &&
                        self.bodyMode == Player.BodyModeIndex.Default &&
                        self.bodyChunks[1].contactPoint.y == 0 &&
                        self.bodyChunks[1].contactPoint.x == 0 &&
                        self.bodyChunks[0].contactPoint.y == 0 &&
                        self.bodyChunks[0].contactPoint.x == 0 &&
                        (!self.IsTileSolid(1, -1, 0) && !self.IsTileSolid(1, 1, 0) ||
                        !self.IsTileSolid(1, 0, -1) && !self.IsTileSolid(1, 0, 1)
                        )
                    )
                    {   
                        // Horizontal pole skip
                        if (
                            (
                                self.room.GetTile(self.bodyChunks[1].pos).horizontalBeam ||
                                self.room.GetTile(new Vector2(self.bodyChunks[1].pos.x, self.bodyChunks[1].pos.y - 10f)).horizontalBeam
                            ) &&
                            Mathf.Min(Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].pos.y), Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].lastPos.y)
                            ) < 22.5f &&
                            self.input[0].y <= 0 &&
                            self.poleSkipPenalty < 1
                        )
                        {
                            Ebug(self, "Horizontal Poletech condition", ignoreRepetition: true);
                            if (flipperoni)
                            {
                                self.bodyChunks[0].vel.y = 7f;
                                self.bodyChunks[1].vel.y = 6f;
                                self.bodyChunks[0].vel.x *= 1.08f;
                                self.bodyChunks[1].vel.x *= 1.04f;
                                self.jumpBoost += 8f;
                            }
                            else if (kickeroni)
                            {
                                self.bodyChunks[0].vel.y = 6f;
                                self.bodyChunks[1].vel.y = 5f;
                                self.bodyChunks[0].vel.x *= 1.1f;
                                self.bodyChunks[1].vel.x *= 1.2f;
                                self.jumpBoost += 7f;
                            }
                            else if (rollaunch)
                            {
                                self.bodyChunks[0].vel.y = 14f;
                                self.bodyChunks[1].vel.y = 13f;
                                self.bodyChunks[0].vel.x *= 1.3f;
                                self.bodyChunks[1].vel.x *= 1.45f;
                                self.jumpBoost += 9f;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.y = 5f;
                                self.bodyChunks[1].vel.y = 4f;
                                self.jumpBoost += 6f;
                            }
                            if (self.animation != Player.AnimationIndex.None)
                            {
                                for (int num8 = 0; num8 < (rollaunch ? 14 : 6); num8++)
                                {
                                    self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos + new Vector2(self.mainBodyChunk.rad * self.mainBodyChunk.vel.x, 0f), Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.mainBodyChunk.vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                                }
                                self.room.PlaySound(Escort_SFX_Pole_Bounce, e.SFXChunk, false, 1f, rollaunch ? 0.5f : 1f);
                            }
                            else
                            {
                                self.room.PlaySound(SoundID.Slugcat_From_Horizontal_Pole_Jump, e.SFXChunk);
                            }
                            float n = self.room.MiddleOfTile(self.bodyChunks[1].pos).y + 5f - self.bodyChunks[1].pos.y;
                            self.bodyChunks[1].pos.y += n;
                            self.bodyChunks[0].pos.y += n;
                            self.poleSkipPenalty = 3;
                            self.wantToJump = 0;
                            self.canJump = 0;
                            e.verticalPoleFail = 10;
                        }
                        // Vertical pole skip
                        else if (
                            true &&
                            e.isDefault &&
                            //e.verticalPoleFail == 0 &&
                            (
                                self.room.GetTile(self.bodyChunks[1].pos).verticalBeam ||
                                self.room.GetTile(new Vector2(self.bodyChunks[1].pos.x, self.bodyChunks[1].pos.y - 10f)).verticalBeam) &&
                                Mathf.Min(Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].pos.y), Mathf.Abs(self.room.MiddleOfTile(self.bodyChunks[1].pos).y - self.bodyChunks[1].lastPos.y)
                            ) < 22.5f &&
                            self.input[0].y <= 0 &&
                            self.poleSkipPenalty < 1 &&
                            ((e.verticalPoleToggle? (self.bodyChunks[0].pos.y > self.bodyChunks[1].pos.y) : (self.bodyChunks[0].pos.y < self.bodyChunks[1].pos.y)) || !e.verticalPoleTech)
                        )
                        {
                            e.verticalPoleTech = true;
                            Ebug(self, "Vertical Poletech Condition", ignoreRepetition: true);
                            if (flipperoni)
                            {
                                self.bodyChunks[0].vel.y = e.verticalPoleToggle? 6f : 8f;
                                self.bodyChunks[1].vel.y = e.verticalPoleToggle? 8f : 6f;
                                self.bodyChunks[0].vel.x *= e.verticalPoleToggle? 0.28f: 0.24f;
                                self.bodyChunks[1].vel.x *= e.verticalPoleToggle? 0.24f: 0.28f;
                                self.jumpBoost += 8f;
                                e.verticalPoleToggle = !e.verticalPoleToggle;
                            }
                            else if (kickeroni)
                            {
                                self.bodyChunks[0].vel.y = 6f;
                                self.bodyChunks[1].vel.y = 4.5f;
                                self.bodyChunks[0].vel.x *= -2.5f;
                                self.bodyChunks[1].vel.x *= -1.5f;
                                self.jumpBoost += 7f;
                            }
                            else if (rollaunch)
                            {
                                self.bodyChunks[0].vel.y = 14f;
                                self.bodyChunks[1].vel.y = 13f;
                                self.bodyChunks[0].vel.x *= 0.1f;
                                self.bodyChunks[1].vel.x *= 0f;
                                self.jumpBoost += 9f;
                                self.animation = Player.AnimationIndex.Flip;
                                e.verticalPoleToggle = true;
                            }
                            else
                            {
                                e.verticalPoleFail = 40;
                            }
                            if (self.animation != Player.AnimationIndex.None)
                            {
                                for (int num8 = 0; num8 < (rollaunch ? 14 : 6); num8++)
                                {
                                    self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos + new Vector2(self.mainBodyChunk.rad * self.mainBodyChunk.vel.x, 0f), Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.mainBodyChunk.vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                                }
                                self.room.PlaySound(Escort_SFX_Pole_Bounce, e.SFXChunk, false, 1f, rollaunch ? 0.5f : 1f);
                            }
                            else
                            {
                                self.room.PlaySound(SoundID.Slugcat_From_Vertical_Pole_Jump, e.SFXChunk);
                            }
                            float n = self.room.MiddleOfTile(self.bodyChunks[1].pos).y + 5f - self.bodyChunks[1].pos.y;
                            self.bodyChunks[1].pos.y += n;
                            self.bodyChunks[0].pos.y += n;
                            self.poleSkipPenalty = 3;
                            self.wantToJump = 0;
                            self.canJump = 0;
                        }
                        else
                        {
                            e.verticalPoleFail = 30;
                            self.poleSkipPenalty = 5;
                            self.wantToJump = 5;
                        }
                    }
                    else
                    {
                        //e.verticalPoleFail = 80;
                        self.wantToJump = 5;
                    }
                }
                catch (Exception err)
                {
                    Ebug(self, err, "Pole bounce failed!");
                }
            }

            // Rotund world rotundness check then SFX!
            if (escPatch_rotundness)
            {
                if (!e.isChunko && self.TotalMass / e.originalMass > 1.4f && self.room != null)
                {
                    if (Esconfig_SFX(self))
                    {
                        Ebug("Play rotund sound!");
                        self.room.PlaySound(Escort_SFX_Uhoh_Big, e.SFXChunk);
                    }
                    e.isChunko = true;
                }
                else if (e.isChunko && self.TotalMass / e.originalMass < 1.4f)
                {
                    e.isChunko = false;
                }
            }

            // Delayed Kingvulture tusk removal so it doesn't exception
            if (e.offendingKingTusk.Count > 0 && e.offendingRemoval <= 0 && e.offendingKTtusk != -1)
            {
                try
                {
                    if (e.offendingKingTusk.Peek()?.kingTusks?.tusks[e.offendingKTtusk]?.impaleChunk?.owner is Player)
                    {
                        e.offendingKingTusk.Pop().kingTusks.tusks[e.offendingKTtusk].impaleChunk = null;
                        e.offendingKTtusk = -1;
                    }
                    else
                    {
                        e.offendingKingTusk.Pop();
                        e.offendingKTtusk = -1;
                        Ebug("King vulture tusk no longer has an impale owner?", 1, true);
                    }
                }
                catch (Exception ex)
                {
                    Ebug(ex, "Couldn't remove king vulture tusk from player!");
                }
            }

            // Make guardian passive
            if (self.room?.game?.session is StoryGameSession s)
            {
                if (s.saveState.deathPersistentSaveData.karmaCap >= 9 && self?.room?.world?.region is not null && self.room.world.region.name == "SB")
                {
                    templeGuardIsFriendly = true;
                    Ebug("Templeguard is friendly!");
                }
            }


            // Currently pup is only supported in story mode
            if (self.playerState.playerNumber == 0 && self.room?.game?.session is StoryGameSession sgs)
            {
                // Debug
                if (self.room.game.devToolsActive && (self.room.game.cameras[0].room == self.room || !ModManager.CoopAvailable) && self.playerState.playerNumber == 0 && Input.GetKeyDown("="))
                {
                    if (e.SocksAliveAndHappy is null)
                    {
                        sgs.saveState.miscWorldSaveData.Esave().HackPupSpawn = true;
                        SpawnThePup(ref e, self.room, self.coord, self.abstractCreature.ID);
                        e.SocksAliveAndHappy.mainBodyChunk.pos = e.SocksAliveAndHappy.mainBodyChunk.lastPos = new Vector2(Futile.mousePosition.x, Futile.mousePosition.y) + self.room.game.cameras[0].pos;

                        Ebug("DEBUG SOCKS on", 2, true);
                    }
                    else
                    {
                        sgs.saveState.miscWorldSaveData.Esave().HackPupSpawn = false;
                        e.SocksAliveAndHappy.Destroy();
                        Ebug("DEBUG SOCKS off", 2, true);
                    }
                }

                // Regular stuff
                if (e.isDefault || config.cfgAllBuildsGetPup.Value || sgs.saveState.miscWorldSaveData.Esave().HackPupSpawn || self.room.game.rainWorld.ExpeditionMode)
                {
                    if (checkPupStatusAgain)
                    {
                        e.tryFindingPup = 20;
                        checkPupStatusAgain = false;
                    }
                    if (e.isDefault && config.cfgAllBuildsGetPup.Value && sgs.saveState.cycleNumber > 0 && !pupAvailable)
                    {
                        pupAvailable = sgs.saveState.miscWorldSaveData.Esave().EscortPupEncountered = true;
                    }

                    // Spawn in shelter on cycle 0 for expedition
                    if (e.SocksAliveAndHappy is null && !e.expeditionSpawnPup && ModManager.Expedition && sgs.saveState.cycleNumber == 0 && self.room.game.rainWorld.ExpeditionMode && self.room.world is not null)
                    {
                        SpawnThePup(ref e, self.room, self.coord, self.abstractCreature.ID);
                        e.expeditionSpawnPup = true;
                        Ebug("Socks has been added to expedition!", 1, true);
                    }

                    if (e.SocksAliveAndHappy is null && !e.cheatedSpawnPup && config.cfgAllBuildsGetPup.Value && sgs.saveState.cycleNumber == 0 && !e.isDefault)
                    {
                        SpawnThePup(ref e, self.room, self.coord, self.abstractCreature.ID);
                        Ebug("Socks has been added to an Escort with the power of options!", 1, true);
                        pupAvailable = sgs.saveState.miscWorldSaveData.Esave().EscortPupEncountered = true;
                        e.cheatedSpawnPup = true;
                    }

                    // Socks (impostor) check
                    if (e.SocksAliveAndHappy is null && pupAvailable)
                    {
                        if (e.tryFindingPup > 0 && TryFindThePup(self.room, out AbstractCreature ac))
                        {
                            e.socksAbstract = ac;
                            Ebug($"Socks has been found with {e.tryFindingPup} tries remaining!", 1, true);
                        }
                        else
                        {
                            // schedule pup to respawn next cycle if reinforced
                            sgs.saveState.miscWorldSaveData.Esave().RespawnPupReady = sgs.saveState.deathPersistentSaveData.reinforcedKarma;
                        }
                    }

                    // Show tutorial when Socks respawns
                    if (self.room?.game?.session is StoryGameSession storyGameSession && storyGameSession.saveState.deathPersistentSaveData.Etut(EscortTutorial.EscortPupRespawned))
                    {
                        Ebug("Show socks respawn tutorial!", 1, true);
                        self.room.game.cameras[0].hud.textPrompt.AddMessage(RWCustom.Custom.rainWorld.inGameTranslator.Translate("At the cost of a karma flower, the slugpup respawns!"), 40, 300, true, true);
                        storyGameSession.saveState.deathPersistentSaveData.Etut(EscortTutorial.EscortPupRespawnedNotify, true);
                        storyGameSession.saveState.deathPersistentSaveData.Etut(EscortTutorial.EscortPupRespawned, false);
                    }
                }
            }

            // Update tracker
            //e.Escat_Update_Ring_Trackers();
        }



        // Implement Movementtech
        private void Escort_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self)
        {
            orig(self);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return;
            }
            if (e.Deflector) Esclass_DF_UpdateAnimation(self, ref e);
            if (e.Gilded) Esclass_GD_UpdateAnimation(self);

            //Ebug(self, "UpdateAnimation Triggered!");

            // Infiniroll
            try
            {
                if (self.animation == Player.AnimationIndex.Roll && !((self.input[0].y > -1 && self.input[0].downDiagonal == 0) || self.input[0].x == -self.rollDirection))
                {
                    e.RollinCount++;
                    if (e.consoleTick == 0)
                    {
                        Ebug(self, "Rollin at: " + e.RollinCount, 2);
                    }
                    if (Esconfig_SFX(self) && e.Rollin != null)
                    {
                        e.Rollin.Volume = Mathf.InverseLerp(80f, 240f, e.RollinCount);
                    }
                    self.rollCounter = e.Brawler? 15: 0;
                    self.mainBodyChunk.vel.x *= Mathf.Lerp(1, 1.25f, Mathf.InverseLerp(0, 120f, e.RollinCount)) * (e.Gilded? 0.75f : 1f);
                }

                if (self.animation != Player.AnimationIndex.Roll)
                {
                    e.RollinCount = 0f;
                }
            }
            catch (NullReferenceException nerr)
            {
                Ebug(self, nerr, "Caught null error in updateAnimation!");
            }
            // Implement dropkick animation
            if (self.animation == Player.AnimationIndex.RocketJump && config.cfgDKAnimation.Value)
            {
                Vector2 n = self.bodyChunks[0].vel.normalized;
                self.bodyChunks[0].vel -= n * 2;
                self.bodyChunks[1].vel += n * 2;
                self.bodyChunks[0].vel.y += 0.05f;
                self.bodyChunks[1].vel.y += 0.1f;
            }
            // Implement frontslide animation (not working right)
            if (false && self.animation == Player.AnimationIndex.BellySlide && config.cfgDKAnimation.Value && !self.longBellySlide)
            {
                if (self.rollCounter < 6)
                {
                    self.bodyChunks[1].vel.x += 9.1f * self.rollDirection;
                    self.bodyChunks[0].vel.x -= 9.1f * self.rollDirection;
                }

                float sliding = 18.1f * self.rollDirection * Mathf.Sin(self.rollCounter / 15f) * (float)Math.PI;
                self.bodyChunks[0].vel.x -= sliding;
                self.bodyChunks[1].vel.x += sliding;
            }

            if (e.easyMode && e.easyKick)
            {
                if (self.animation == Player.AnimationIndex.RocketJump && self.input[0].x == 0 && self.input[1].x == 0 && self.input[2].x == 0)
                {
                    self.animation = Player.AnimationIndex.None;
                }
                if (self.animation != Player.AnimationIndex.RocketJump)
                {
                    e.easyKick = false;
                }
            }
            if (e.Speedster) Esclass_SS_UpdateAnimation(self, ref e);


            if (e.slideFromSpear && self.animation != Player.AnimationIndex.BellySlide)
            {
                e.slideFromSpear = false;
            }
        }

        // Implement Movementthings
        private void Escort_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return;
                }
            }
            catch (Exception err)
            {
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
            )
            {
                return;
            }

            bool hypedMode = Esconfig_Hypable(self);

            float movementSlow = Mathf.Lerp(1, 4, Mathf.InverseLerp(0, 16, self.slowMovementStun));

            // Implement bettercrawl
            if (self.bodyMode == Player.BodyModeIndex.Crawl)
            {
                if (!e.Gilded && hypedMode)
                {
                    self.dynamicRunSpeed[0] = Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) * self.slugcatStats.runspeedFac / movementSlow;
                    self.dynamicRunSpeed[1] = Mathf.Lerp(crawlSpeed[0], crawlSpeed[1], self.aerobicLevel) * self.slugcatStats.runspeedFac / movementSlow;
                }
                else
                {
                    self.dynamicRunSpeed[0] = crawlSpeed[2] * self.slugcatStats.runspeedFac / movementSlow;
                    self.dynamicRunSpeed[1] = crawlSpeed[2] * self.slugcatStats.runspeedFac / movementSlow;
                }
            }

            // Implement betterpolewalk
            /*
            The hangfrombeam's speed does not get affected by dynamicrunspeed apparently so that's fun...
            still the standonbeam works but also at the same time not as I initially thought.
            The slugcat apparently has a limit on how fast they can move on the beam while standing on it, leaning more and more foreward and getting more and more friction as a result...
            or to that degree.
            */
            else if (!e.Gilded && self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
            {
                if (hypedMode)
                {
                    if (self.animation == Player.AnimationIndex.StandOnBeam)
                    {
                        self.dynamicRunSpeed[0] = Mathf.Lerp(poleWalk[0], poleWalk[1], self.aerobicLevel) * self.slugcatStats.runspeedFac / movementSlow;
                        self.dynamicRunSpeed[1] = Mathf.Lerp(poleWalk[0], poleWalk[1], self.aerobicLevel) * self.slugcatStats.runspeedFac / movementSlow;
                        self.bodyChunks[1].vel.x += Mathf.Lerp(0, poleWalk[3], self.aerobicLevel) * self.input[0].x / movementSlow;
                    }
                    else if (self.animation == Player.AnimationIndex.HangFromBeam)
                    {
                        self.bodyChunks[0].vel.x += Mathf.Lerp(poleHang[0], poleHang[1], self.aerobicLevel) * self.input[0].x / movementSlow;
                        self.bodyChunks[1].vel.x += Mathf.Lerp(poleHang[2], poleHang[3], self.aerobicLevel) * self.input[0].x / movementSlow;
                    }
                }
                else
                {
                    if (self.animation == Player.AnimationIndex.StandOnBeam)
                    {
                        self.dynamicRunSpeed[0] = poleWalk[2] * self.slugcatStats.runspeedFac / movementSlow;
                        self.dynamicRunSpeed[1] = poleWalk[2] * self.slugcatStats.poleClimbSpeedFac / movementSlow;
                    }
                    else if (self.animation == Player.AnimationIndex.HangFromBeam)
                    {
                        self.bodyChunks[0].vel.x += poleHang[4] * self.input[0].x / movementSlow;
                        self.bodyChunks[1].vel.x += poleHang[5] * self.input[0].x / movementSlow;
                    }
                }
            }

            // Set headbutt condition
            else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb)
            {
                if (self.slowMovementStun > 0)
                {
                    e.CometFrames = SetComet;
                }
            }

            // Implement GuuhWuuh (& Default Escort better swimming ability)
            if (self.bodyMode == Player.BodyModeIndex.Swimming)
            {
                e.viscoDance = self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.WaterViscosity);
                if (self.room?.waterObject is not null)
                {
                    e.viscoColor = self.room.waterObject.palette.waterColor1;   
                }

                if (self.animation == Player.AnimationIndex.DeepSwim)
                {
                    if (e.isDefault) self.waterFriction = 0.98f;
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[0], (float)Math.Pow(e.viscoDance, theGut[6])),
                        Mathf.Lerp(1f, self.mainBodyChunk.vel.y > 0 ? theGut[1] : theGut[2], (float)Math.Pow(e.viscoDance, theGut[7])));
                }
                else if (self.animation == Player.AnimationIndex.SurfaceSwim)
                {                    
                    if (e.isDefault) self.waterFriction = Mathf.Lerp(0.98f, 1f, Mathf.InverseLerp(0, 5, self.waterJumpDelay));
                    self.mainBodyChunk.vel *= new Vector2(
                        Mathf.Lerp(1f, theGut[3], (float)Math.Pow(e.viscoDance, theGut[8])),
                        Mathf.Lerp(1f, self.mainBodyChunk.vel.y > 0 ? theGut[4] : theGut[5], (float)Math.Pow(e.viscoDance, theGut[9])));
                    self.dynamicRunSpeed[0] += Mathf.Lerp(theGut[10], theGut[11], (float)Math.Pow(e.viscoDance, theGut[12]));
                }
            }

            if (e.Speedster) Esclass_SS_UpdateBodyMode(self, ref e);
        }

        /// <summary>
        /// Implements code that makes Escort drop something live if it grabs them. TODO.
        /// <summary>
        private static void Escort_GrabbyUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self, eu);
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Grab update!");
                orig(self, eu);
                return;
            }
            if (
                !eCon.TryGetValue(self, out Escort e)
                )
            {
                orig(self, eu);
                return;
            }
            /* Eat meat faster?
            int n = 0;
            if ((self.grasps[0] == null || !(self.grasps[0].grabbed is Creature)) && self.grasps[1] != null && self.grasps[1].grabbed is Creature){
                n = 1;
            }
            if (self.input[0].pckp && self.grasps[n] != null && self.grasps[n].grabbed is Creature && self.CanEatMeat(self.grasps[n].grabbed as Creature) && (self.grasps[n].grabbed as Creature).Template.meatPoints > 1){
                //self.EatMeatUpdate(n);
            }*/
            orig(self, eu);
            try
            {
                if (e.Railgunner) Esclass_RG_GrabUpdate(self, ref e);
            }
            catch (Exception err)
            {
                Ebug(err, "Railgunner grabby fail!");
            }
            try
            {
                if (e.Gilded) Esclass_GD_GrabUpdate(self, eu, ref e);
            }
            catch (Exception err)
            {
                Ebug(err, "Gilded grabby fail!");
            }
        }


        /// <summary>
        /// Implement Escort's slowed stamina increase. Due to the aerobic decrease found in some movements implemented in Escort, the AerobicIncrease actually does the original, and on top of that the additional to balance things out.
        /// </summary>
        private void Escort_AerobicIncrease(On.Player.orig_AerobicIncrease orig, Player self, float f)
        {
            if (
                !Exhausion.TryGet(self, out float exhaust) ||
                !eCon.TryGetValue(self, out Escort e)
                )
            {
                orig(self, f);
                return;
            }
            if (Eshelp_IsMe(self.slugcatStats.name, false))
            {
                if (e.Gilded && !self.slugcatStats.malnourished){
                    Esclass_GD_Breathing(self, f);
                    return;
                }


                if (!e.Escapist)
                {
                    orig(self, f);
                }

                //Ebug(self, "Aerobic Increase Triggered!");
                if (!self.slugcatStats.malnourished)
                {
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + ((e.Escapist ? f * 2 : f) / exhaust));
                }
                else
                {
                    self.aerobicLevel = Mathf.Min(1.1f, self.aerobicLevel + (f / (exhaust * 2)));
                }
            }
            else
            {
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
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return;
            }

            //Ebug(self, "Jump Triggered!");
            // Decreases aerobiclevel gained from jumping
            if (self.aerobicLevel > 0.1f)
            {
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
                )
            {
                Ebug(self, "FLIPERONI GO!", 2);

                if (Esconfig_SFX(self) && !e.kickFlip)
                {
                    self.room.PlaySound(Eshelp_SFX_Flip(), e.SFXChunk);
                }
                self.animation = Player.AnimationIndex.Flip;
            }
            if (e.Speedster) Esclass_SS_Jump(self, ref e);
            if (e.Gilded) Esclass_GD_Jump(self, ref e);
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
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self, direction);
                    return;
                }
            }
            catch (Exception err)
            {
                orig(self, direction);
                Ebug(self, err);
                return;
            }
            if (!WallJumpVal.TryGet(self, out var WJV) ||
                !eCon.TryGetValue(self, out Escort e))
            {
                orig(self, direction);
                return;
            }

            Ebug(self, "Walljump Triggered!");
            bool wallJumper = Esconfig_WallJumps(self);
            bool longWallJump = (self.superLaunchJump > 19 && wallJumper);
            bool superWall = (Esconfig_Pouncing(self) && e.superWallFlip > (int)WJV[4]);
            bool superFlip = self.allowRoll == 15 && Esconfig_Pouncing(self);

            // If charge wall jump is enabled and is able to walljump, or if charge wall jump is disabled
            if ((wallJumper && self.canWallJump != 0) || !wallJumper && !e.kickFlip)
            {
                orig(self, direction);
                float n = Mathf.Lerp(1f, 1.15f, self.Adrenaline) * (e.savingThrowed ? 0.7f : 1f);
                float m = 1f;
                if (e.Speedster && e.SpeDashNCrash)
                {
                    m = (e.SpeSecretSpeed ? 2f : 0.66f);
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
                )
                {
                    self.bodyChunks[0].vel.y = 8f * n;
                    self.bodyChunks[1].vel.y = 7f * n;
                    self.bodyChunks[0].pos.y += 10f * Mathf.Min(1f, n);
                    self.bodyChunks[1].pos.y += 10f * Mathf.Min(1f, n);
                    toPrint.SetValue("Water", 1);
                    self.room.PlaySound(SoundID.Slugcat_Normal_Jump, e.SFXChunk, false, 1f, 0.7f);
                }
                else
                {
                    self.bodyChunks[0].vel.y = ((longWallJump || (superFlip && superWall)) ? WJV[0] : 8f) * n;
                    self.bodyChunks[1].vel.y = ((longWallJump || (superFlip && superWall)) ? WJV[1] : 7f) * n;
                    self.bodyChunks[0].vel.x = ((superFlip && superWall) ? WJV[2] * m : 7f) * n * direction;
                    self.bodyChunks[1].vel.x = ((superFlip && superWall) ? WJV[3] * m : 6f) * n * direction;
                    self.standing = true;
                    self.jumpStun = 8 * direction;
                    if (superWall)
                    {
                        self.room.PlaySound((self.superLaunchJump > 19 ? SoundID.Slugcat_Super_Jump : SoundID.Slugcat_Wall_Jump), e.SFXChunk, false, 1f, 0.7f);
                    }
                    toPrint.SetValue("Not Water", 1);
                    Ebug(self, "Y Velocity" + self.bodyChunks[0].vel.y, 2);
                    Ebug(self, "Y Velocity" + self.bodyChunks[1].vel.y, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[0].vel.x, 2);
                    Ebug(self, "X Velocity" + self.bodyChunks[1].vel.x, 2);
                }
                self.jumpBoost = 0f;

                if (superFlip && superWall)
                {
                    self.animation = Player.AnimationIndex.Flip;
                    self.room.PlaySound((Esconfig_SFX(self) ? Eshelp_SFX_Flip() : SoundID.Slugcat_Sectret_Super_Wall_Jump), e.SFXChunk, false, (Esconfig_SFX(self) ? 1f : 1.4f), 0.9f);
                    self.jumpBoost += Mathf.Lerp(WJV[6], WJV[7], Mathf.InverseLerp(WJV[4], WJV[5], e.superWallFlip));
                    toPrint.SetValue("SUPERFLIP", 2);
                }
                else
                {
                    toPrint.SetValue("not so flip", 2);
                }
                Ebug(self, "Jumpboost" + self.jumpBoost, 2);
                Ebug(self, "SWallFlip" + e.superWallFlip, 2);
                Ebug(self, "SLaunchJump" + self.superLaunchJump, 2);
                if (self.superLaunchJump > 19)
                {
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
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return orig(self, source, dmg, chunk, appPos, direction);
                }
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!ParrySlide.TryGet(self, out bool parrier) ||
                !eCon.TryGetValue(self, out Escort e))
            {
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            if (!parrier || (ModManager.CoopAvailable && source.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
            {
                return orig(self, source, dmg, chunk, appPos, direction);
            }
            Ebug(self, "Sticky Triggered!");
            if (e.Deflector) return Esclass_DF_StickySpear(self);
            return !(self.animation == Player.AnimationIndex.BellySlide);
        }


        // Implement Heavylifter
        private bool Escort_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj)
        {
            try {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return orig(self, obj);
                }
                if (!Esconfig_Heavylift(self))
                {
                    return orig(self, obj);
                }
                if (!eCon.TryGetValue(self, out Escort e))
                {
                    return orig(self, obj);
                }

                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead)
                {
                    if (e.consoleTick == 0)
                    {
                        Ebug(self, "Revivify skip!", 1);
                        Ebug(self, "Creature: " + creature.GetType(), 1);
                        Ebug(self, "Player: " + self.GetOwnerType(), 1);
                    }
                    return orig(self, creature);
                }


                if (e.Brawler && Esclass_BL_HeavyCarry(self, obj))
                {
                    //return false;
                }

                //Ebug(self, "Heavycarry Triggered!");
                if (obj.TotalMass <= self.TotalMass * ratioed)
                {
                    if (ModManager.CoopAvailable && obj is Player player && player != null)
                    {
                        return !player.isSlugpup;
                    }
                    if (obj is Creature c && c is not Lizard && !c.dead)
                    {
                        return orig(self, obj);
                    }
                    return false;
                }
                return orig(self, obj);
            }
            
            catch (Exception err) {
                Ebug(self, err);
                return orig(self, obj);
            }
        }

        private void Escort_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self, grasp, eu);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e) ||
                    !NoMoreGutterWater.TryGet(self, out float[] theGut))  // why is this here, investigate.
                {
                    orig(self, grasp, eu);
                    return;
                }
                if (e.Brawler && Esclass_BL_ThrowObject(orig, self, grasp, eu, ref e)) return;
                if (e.Railgunner && Esclass_RG_ThrowObject(orig, self, grasp, eu, ref e)) return;
                if (e.Gilded && Esclass_GD_ThrowObject(orig, self, grasp, eu, ref e)) return;
            }
            catch (Exception err)
            {
                Ebug(self, err, "Throwing object error!");
                orig(self, grasp, eu);
                return;
            }
            orig(self, grasp, eu);
        }


        private Player.ObjectGrabability Escort_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return orig(self, obj);
                }
                if (obj == null)
                {
                    return orig(self, obj);
                }
                if (escPatch_revivify && obj is Creature creature && (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || creature is Player) && creature.dead)
                {
                    return Player.ObjectGrabability.TwoHands;
                }
                if (!dualWielding.TryGet(self, out bool dW) ||
                    !eCon.TryGetValue(self, out Escort e))
                {
                    return orig(self, obj);
                }

                if (e.NewEscapist && obj is Weapon)
                {
                    return Player.ObjectGrabability.BigOneHand;
                }

                if (dW && e.dualWield)
                {
                    if (obj is Weapon)
                    {
                        // Any weapon is dual-wieldable, including spears
                        return Player.ObjectGrabability.OneHand;
                    }
                    if (e.Brawler && Esclass_BL_Grabability(self, obj, ref e))
                    {
                        return Player.ObjectGrabability.BigOneHand;
                    }
                    if (obj is Lizard lizzie)
                    {
                        // Any lizards that are haulable (while dead) or stunned are dual-wieldable
                        if (lizzie.dead)
                        {
                            return Player.ObjectGrabability.OneHand;
                        }
                        else if (lizzie.Stunned && Esconfig_Dunkin() && !e.LizardDunk)
                        {
                            if (e.LizGoForWalk == 0)
                            {
                                e.LizGoForWalk = 320;
                            }
                            if (!Esconfig_SFX(self))
                            {
                                self.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, self.mainBodyChunk);
                            }
                            e.LizardDunk = true;
                            return Player.ObjectGrabability.TwoHands;
                        }
                    }
                    if (obj is Creature c && c.TotalMass <= self.TotalMass * ratioed && c.dead)
                    {
                        return Player.ObjectGrabability.BigOneHand;
                    }
                }
                return orig(self, obj);
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return orig(self, obj);
            }
        }

        private float Escort_GotBit(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            try
            {
                if (!eCon.TryGetValue(self, out Escort e))
                {
                    return orig(self);
                }
                if (Eshelp_IsMe(self.slugcatStats.name, false))
                {
                    float biteMult = 0.5f;
                    if (e.Brawler)
                    {
                        biteMult -= 0.35f;
                    }
                    if (e.Railgunner)
                    {
                        biteMult = self.Malnourished? 10000f : 0.75f;
                    }
                    if (e.Escapist)
                    {
                        biteMult -= 0.15f;
                    }
                    if (e.NewEscapist)
                    {
                        biteMult -= 0.45f;
                    }
                    if (Eshelp_ParryCondition(self) || (!e.Deflector && self.animation == Player.AnimationIndex.RocketJump))
                    {
                        biteMult = 10000f;
                    }
                    Ebug(self, "Lizard bites with multiplier: " + biteMult);
                    return biteMult;
                }
                else
                {
                    return orig(self);
                }
            }
            catch (NullReferenceException nerr)
            {
                Ebug(self, nerr, "Found null while setting deathbitemultiplier!");
                return orig(self);
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't set deathbitemultipler!");
                return orig(self);
            }
        }


        // Implement Parryslide
        private void Escort_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
            try
            {
                if (self is Player p && Eshelp_IsMe(p.slugcatStats.name))
                {
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug((self as Player), err);
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (self is not Player player)
            {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (
                !ParrySlide.TryGet(player, out bool enableParry) ||
                !eCon.TryGetValue(player, out Escort e)
            )
            {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            if (!enableParry)
            {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }



            Ebug(player, "Violence Triggered!");
            // connects to the Escort's Parryslide option
            e.ParrySuccess = false;
            if (e.Railgunner && e.RailIReady && type != null && type == Creature.DamageType.Explosion)
            {
                if (e.iFrames == 0)
                {
                    e.ParrySuccess = true;
                }
                stunBonus = 0;
            }
            if (!ins.L().Vegetable)
            {
                e.ParrySuccess = true;
            }
            if (Eshelp_ParryCondition(player))
            {
                // Parryslide (parry module)
                Ebug(player, "Escort attempted a Parryslide", 2);
                int direction;
                direction = player.slideDirection;

                Ebug(player, "Is there a source? " + (source != null), 2);
                Ebug(player, "Is there a direction & Momentum? " + (directionAndMomentum != null), 2);
                Ebug(player, "Is there a hitChunk? " + (hitChunk != null), 2);
                Ebug(player, "Is there a hitAppendage? " + (hitAppendage != null), 2);
                Ebug(player, "Is there a type? " + (type != null), 2);
                Ebug(player, $"Is there damage? {damage > 0f} | {damage}", 2);
                Ebug(player, $"Is there stunBonus? {stunBonus > 0f} | {stunBonus}", 2);

                if (source != null)
                {
                    Ebug(player, "Escort is being assaulted by: " + source.owner.GetType(), 2);
                }
                Ebug(player, "Escort parry is being checked", 1);
                if (type != null)
                {
                    Ebug(player, "Escort gets hurt by: " + type.value, 2);
                    if (type == Creature.DamageType.Bite)
                    {
                        Ebug(player, "Escort is getting BIT?!", 1);
                        if (source != null && source.owner is Creature creature)
                        {
                            creature.LoseAllGrasps();
                            creature.stun = 35;
                            //(self as Player).WallJump(direction);
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus = 0f;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort got out of a creature's mouth!", 2);
                        }
                        else if (source != null && source.owner is Weapon)
                        {
                            Ebug(player, "Weapons can BITE?!", 2);
                        }
                        else
                        {
                            Ebug(player, "Where is Escort getting bit from?!", 2);
                        }
                    }
                    else if (type == Creature.DamageType.Stab)
                    {
                        Ebug(player, "Escort is getting STABBED?!", 1);
                        if (source != null && source.owner is Creature creature)
                        {
                            creature.LoseAllGrasps();
                            creature.stun = 20;
                            damage = 0f;
                            stunBonus *= 1.5f;
                            type = Creature.DamageType.Blunt;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parried a stabby creature?", 2);
                        }
                        else if (source != null && source.owner is Weapon weapon)
                        {
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            weapon.WeaponDeflect(-source.owner.firstChunk.lastPos, vector, source.owner.firstChunk.vel.magnitude);
                            damage = 0f;
                            type = Creature.DamageType.Blunt;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parried a stabby weapon", 2);
                        }
                        else
                        {
                            damage = 0f;
                            type = Creature.DamageType.Blunt;
                            bool foundOffender = false;
                            for (int a = 0; a < self.room.physicalObjects.Length; a++)
                            {
                                for (int b = 0; b < self.room.physicalObjects[a].Count; b++)
                                {
                                    if (self.room.physicalObjects[a][b] is Vulture vulture && vulture.IsKing)
                                    {
                                        for (int c = 0; c < vulture.kingTusks.tusks.Length; c++)
                                        {
                                            if (vulture.kingTusks.tusks[c].impaleChunk?.owner is Player p && p == self)
                                            {
                                                foundOffender = true;
                                                if (e.offendingKingTusk.Count > 0)
                                                {
                                                    e.offendingKingTusk.Pop();
                                                }
                                                e.offendingKingTusk.Push(vulture);
                                                e.offendingKTtusk = c;
                                                e.offendingRemoval = 2;
                                                goto getOut;
                                            }
                                        }
                                    }
                                }
                            }
                            getOut:
                            if (foundOffender)
                            {
                                Ebug(player, "Tusk unimpaled!", 2);
                                //Debug.LogError("-> Escort: Please pay no attention to this! This is how Escort parry works (on King Tusks)!");
                            }
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parried a generic stabby thing", 2);
                        }
                    }
                    else if (type == Creature.DamageType.Blunt)
                    {
                        Ebug(player, "Escort is getting ROCC'ED?!", 1);
                        if (source != null && source.owner is Creature)
                        {
                            Ebug(player, "Creatures aren't rocks...", 2);
                        }
                        else if (source != null && source.owner is Weapon weapon)
                        {
                            Vector2 vector = RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f);
                            weapon.WeaponDeflect(weapon.firstChunk.lastPos, -vector, weapon.firstChunk.vel.magnitude);
                            damage = 0f;
                            stunBonus /= 5f;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort bounces a blunt thing.", 2);
                        }
                        else
                        {
                            damage = 0f;
                            stunBonus = 0f;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parried something blunt.", 2);
                        }
                    }
                    else if (type == Creature.DamageType.Water)
                    {
                        Ebug(player, "Escort is getting Wo'oh'ed?!", 1);
                    }
                    else if (type == Creature.DamageType.Explosion)
                    {
                        Ebug(player, "Escort is getting BLOWN UP?!", 1);
                        if (source != null && source.owner is Creature)
                        {
                            Ebug(player, "Wait... creatures explode?!", 2);
                        }
                        else if (source != null && source.owner is Weapon)
                        {
                            player.animation = Player.AnimationIndex.Flip;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus *= 1.5f;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parries an explosion from weapon?!", 2);
                        }
                        else
                        {
                            player.WallJump(direction);
                            player.animation = Player.AnimationIndex.Flip;
                            type = Creature.DamageType.Blunt;
                            damage = 0f;
                            stunBonus *= 1.5f;
                            e.ParrySuccess = true;
                            Ebug(player, "Escort parries an explosion", 2);
                        }
                    }
                    else if (type == Creature.DamageType.Electric)
                    {
                        Ebug(player, "Escort is getting DEEP FRIED?!", 1);
                        if (source != null && source.owner is Creature creature)
                        {
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
                        else if (source != null && source.owner is Weapon)
                        {
                            //(self as Player).WallJump(direction);
                            player.animation = Player.AnimationIndex.Flip;
                            //player.Jump();

                            //type = Creature.DamageType.Blunt;
                            damage = 0f;
                            e.ParrySuccess = true;
                            e.ElectroParry = true;
                            Ebug(player, "Escort somehow parried a shock object?!", 2);
                        }
                        else
                        {
                            player.animation = Player.AnimationIndex.Flip;
                            //player.Jump();

                            damage = 0f;
                            e.ParrySuccess = true;
                            e.ElectroParry = true;
                            Ebug(player, "Escort attempted to parry a shock but why?!", 2);
                        }
                    }
                    else
                    {
                        Ebug(player, "Escort is getting UNKNOWNED!!! RUNNN", 1);
                        if (source != null && source.owner is Creature)
                        {
                            Ebug(player, "IT'S ALSO AN UNKNOWN CREATURE!!", 2);
                        }
                        else if (source != null && source.owner is Weapon)
                        {
                            Ebug(player, "IT'S ALSO AN UNKNOWN WEAPON!!", 2);
                        }
                        else
                        {
                            Ebug(player, "WHO THE HECK KNOWS WHAT IT IS?!", 2);
                        }
                    }
                }
            }
            else if (Eshelp_SavingThrow(player, source, type))
            {
                e.ParrySuccess = true;
                (source.owner as Creature).LoseAllGrasps();
                type = Creature.DamageType.Blunt;
                damage = 0f;
                stunBonus = 0f;
            }
            // Auralvisual indicator: Manual white flickering effect? I'd be surprised if this works as intended
            // Visual indicator doesn't work ;-;
            if (e.ParrySuccess)
            {
                if (player.abstractCreature.world.game.IsArenaSession && !e.DeflTrampoline)
                {
                    player.abstractCreature.world.game.GetArenaGameSession.arenaSitting.players[0].parries++;
                }
                if (e.Deflector)
                {
                    player.Jump();
                    player.animation = Player.AnimationIndex.Flip;
                    player.mainBodyChunk.vel.y *= 1.5f;
                    player.mainBodyChunk.vel.x *= 0.15f;
                    if (e.DeflTrampoline)
                    {
                        if (e.DeflPowah < 1)
                        {
                            e.DeflPowah = 1;
                        }
                        if (e.DeflSFXcd == 0)
                        {
                            self.room.PlaySound(SoundID.Snail_Warning_Click, self.mainBodyChunk, false, 1.6f, 0.65f);
                            e.DeflSFXcd = 8;
                        }
                    }
                    else
                    {
                        if (e.DeflPowah < 3)
                        {
                            e.DeflPowah++;
                        }
                        self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                        self.room.PlaySound(Escort_SFX_Parry, e.SFXChunk);
                    }
                    e.DeflAmpTimer = e.DeflPowah switch {
                        1 => 200,
                        2 => 400,
                        3 => 800,
                        _ => 0
                    };
                    e.DeflTrampoline = false;
                }
                else
                {
                    self.room.PlaySound(SoundID.Spear_Fragment_Bounce, self.mainBodyChunk);
                    self.room.PlaySound(Escort_SFX_Parry, e.SFXChunk);
                }
                if (self != null && self.room != null && self.mainBodyChunk != null)
                {
                    for (int c = 0; c < 7; c++)
                    {
                        self.room.AddObject(new WaterDrip(self.bodyChunks[1].pos + new Vector2(self.mainBodyChunk.rad * self.bodyChunks[1].vel.x, 0f), RWCustom.Custom.DegToVec(UnityEngine.Random.value * 180f * (0f - self.bodyChunks[1].vel.x)) * Mathf.Lerp(10f, 17f, UnityEngine.Random.value), waterColor: false));
                        //self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.grey, null, 3, 6));
                        //self.room.AddObject(new WaterDrip(self.mainBodyChunk.pos, self.mainBodyChunk.vel * Mathf.Lerp(1f, 4f, UnityEngine.Random.value) * player.flipDirection + RWCustom.Custom.RNV() * UnityEngine.Random.value * 9f, false));
                    }
                }
                Ebug(player, "Parry successful!", 1);
                e.iFrames = 6;
                if (e.Deflector)
                {
                    e.iFrames = 8;
                }
                e.parrySlideLean = 0;
                if (e.Railgunner && e.RailIReady)
                {
                    if (e.RailBombJump)
                    {
                        self.mainBodyChunk.vel.x = 0;
                    }
                    self.stun = 0;
                }
                self.AllGraspsLetGoOfThisObject(false);
            }
            else if (e.iFrames > 0)
            {
                if (e.Railgunner && e.RailIReady)
                {
                    self.stun = 0;
                    if (e.RailBombJump)
                    {
                        self.mainBodyChunk.vel.x = 0;
                    }
                    if (e.iFrames <= 0)
                    {
                        e.RailIReady = false;
                        e.RailBombJump = false;
                    }
                }
                if (e.ElectroParry)
                {
                    damage = 0f;
                    stunBonus *= 0.5f;
                    orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                    Ebug(player, "Stun Resistance frame tick", 2);
                }
                else
                {
                    if (e.Railgunner && e.RailIReady)
                    {
                        e.RailIReady = false;
                        e.RailBombJump = false;
                    }
                    Ebug(player, "Immunity frame tick", 2);
                }
            }
            else
            {
                orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                Ebug(player, "Nothing or not possible to parry!", 1);
            }
            Ebug(player, "Parry Check end", 1);
            return;

        }

        private bool Escort_SpearGet(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return orig(self, obj);
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Grab update!");
                return orig(self, obj);
            }
            if (
                !eCon.TryGetValue(self, out Escort e)
                )
            {
                return orig(self, obj);
            }
            if (e.Railgunner && Esclass_RG_SpearGet(obj))
            {
                return true;
            }
            if (obj != null && obj is Weapon w && !(ModManager.CoopAvailable && w.thrownBy is Player && !RWCustom.Custom.rainWorld.options.friendlyFire) && w.mode == Weapon.Mode.Thrown)
            {
                Ebug(self, "Hehe, yoink!");
                if (self.input[0].pckp && !self.input[1].pckp)
                {
                    w.mode = Weapon.Mode.Free;
                }
                return true;
            }
            return orig(self, obj);
        }


        private void StoreWinConditionData(On.ShelterDoor.orig_Close orig, ShelterDoor self)
        {
            orig(self);
            try{
                List<AbstractCreature> playersInThisGame = self.room.game.Players;
                List<AbstractCreature> playersToProgressOrWin = self.room.game.PlayersToProgressOrWin;
                List<AbstractCreature> shelterPlayers = (from x in self.room.physicalObjects.SelectMany((List<PhysicalObject> x) => x).OfType<Player>() select x.abstractCreature).ToList<AbstractCreature>();
                foreach (AbstractCreature abstractPlayer in playersInThisGame)
                {
                    if (abstractPlayer.realizedCreature is Player player && eCon.TryGetValue(player, out Escort escort))
                    {
                        if (escort.shelterSaveComplete < 2)
                        {
                            int playerNumber = (abstractPlayer.state as PlayerState).playerNumber;
                            bool inShelter = shelterPlayers.Contains(abstractPlayer);
                            bool isWinner = playersToProgressOrWin.Contains(abstractPlayer);
                            bool notDead = !player.dead;

                            if (inShelter && notDead && isWinner)
                            {
                                //Ebug(player, "Is successful!", ignoreRepetition: true);
                                // do things when successful
                            }
                            else
                            {
                                //Ebug(player, "Is failure!", ignoreRepetition: true);
                                // do things when failure
                            }
                            Ebug(player, $"Shelter: {inShelter} | Winner: {isWinner} | Dead: {!notDead}", ignoreRepetition: true);

                            // Other builds
                            if (escort.Deflector) Esclass_DF_WinLoseSave(self, playerNumber, notDead || RWCustom.Custom.rainWorld.options.jollyDifficulty == Options.JollyDifficulty.EASY, ref escort);
                            if (escort.Speedster) Esclass_SS_WinLoseSave(self, playerNumber, isWinner && notDead, ref escort);
                            escort.shelterSaveComplete++;
                        }

                        if (escort.NewEscapist)
                        {
                            escort.NEsShadowPlayer?.GoAwayShadow();
                            escort.NEsShelterCloseTime = true;
                        }
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null encountered when determining win condition!");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic error when saving win condition");
            }
        }

    }
}
