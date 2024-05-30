using BepInEx;
using SlugBase.Features;
using System;
using RWCustom;
using MoreSlugcats;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Unstable tweak values
        // public static readonly PlayerFeature<> unstable = Player("theescortunstable//");
        // public static readonly PlayerFeature<float> unstable = PlayerFloat("theescort/unstable/");
        // public static readonly PlayerFeature<float[]> unstable = PlayerFloats("theescort/unstable/");


        public void Esclass_US_Tick(Player self, ref Escort e)
        {
            bool shortDash = self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG;
            // Constant tripping clock
            if (e.UnsTripTime < 40)
            {
                e.UnsTripTime++;
            }
            else
            {
                e.UnsTripTime = 0;
            }

            if (e.UnsTripping > 0)
            {
                e.UnsTripping--;
            }

            // May need revisiting
            if (self.canJump > 0 && e.UnsBlinkCount > 0 && e.UnsBlinkCD == 0 && e.UnsBlinkWindow == 0)
            {
                e.UnsBlinkCount--;
            }

            // 
            if (e.UnsBlinkFrame == 0)
            {
                if (e.UnsBlinkWindow > 0)
                {
                    Ebug(self, "Window: " + e.UnsBlinkWindow, ignoreRepetition: true);
                    e.UnsBlinkWindow--;
                }
                else
                {
                    if (e.UnsBlinking)
                    {
                        Ebug(self, "Blink window end!", ignoreRepetition: true);
                        e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                        e.UnsBlinking = false;
                    }
                }

                if (e.UnsBlinkCD > 0)
                {
                    self.Blink(5);
                    e.UnsBlinkCD--;
                }
            }

            //

            // For alternate version of blink
            if (e.UnsBlinkFrame > 0 && e.UnsBlinkFrame < (shortDash? 7 : 10))
            {
                e.UnsBlinkFrame++;
            }
            else if (e.UnsBlinkFrame >= (shortDash? 7 : 10))
            {
                e.UnsBlinkFrame = 0;
            }

            // Cooldown for melee attacks
            if (e.UnsMeleeStun > 0)
            {
                e.UnsMeleeStun--;
            }

            // Countdown to let whatever Unstable threw to return back to them upon reaching 0...
            // Default state is -1, and will only get to that state upon retrieving whatever had been thrown
            if (e.UnsMeleeGrab > 0)
            {
                e.UnsMeleeGrab--;
            }

            // Slide prevention system(?)
            if (e.UnsFuckYourSlide > 0)
            {
                e.UnsFuckYourSlide--;
            }

            // Cancels homing if homing takes too long
            if (e.UnsRockitCret is not null)
            {
                if (e.UnsRockitDur > 0)
                {
                    e.UnsRockitDur--;
                }
                else
                {
                    e.UnsRockitCret = null;
                }
            }
            else
            {
                e.UnsRockitDur = 0;
            }
        }

        private void Esclass_US_Update(Player self, ref Escort e)
        {
            bool shortDash = self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG;

            // Trippin' time!
            try
            {
                if (self.input[0].x != 0 && e.UnsTripTime == 0 && UnityEngine.Random.value >= 0.95f && !shortDash && self.standing && self.bodyChunks[1].ContactPoint.y == -1)
                {
                    Ebug(self, "Unstable fucking tripped! Laugh at 'em!");
                    self.standing = false;
                    self.bodyChunks[0].vel.x += self.rollDirection * 7f;
                    self.bodyChunks[1].vel.x -= self.rollDirection * 5f;
                    self.bodyChunks[0].vel.y -= 3f;
                    e.UnsTripping = 120;
                    if (UnityEngine.Random.value > 0.7f)
                    {
                        self.Stun(40);
                        self.LoseAllGrasps();
                    }
                    else
                    {
                        self.Stun(30);
                    }
                }
                if (e.UnsTripping > 0 && self.bodyMode == Player.BodyModeIndex.Crawl && self.animation == Player.AnimationIndex.None && self.bodyChunks[0].ContactPoint.y == -1 && self.room is not null)
                {
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk);
                    e.UnsTripping = 0;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Unstable tripping failed!");
            }

            try
            {
                if (e.UnsBlinkFrame > 0)
                {
                    Ebug(self, "Frame: " + e.UnsBlinkFrame, ignoreRepetition: true);
                    if (Esclass_US_Dash2(self, in e.UnsBlinkFrame, e.UnsBlinkDir, e.UnsBlinkDir == (0, 1) || e.UnsBlinkNoDir, e.UnsBlinkDifDir))
                    {
                        e.UnsBlinkNoDir = false;
                        e.UnsBlinkFrame = 0;
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Unstable movin failed!");
            }


            try
            {
                if (e.UnsMeleeGrab == 0 && e.UnsMeleeWeapon.Count > 0 && e.UnsMeleeUsed >= 0 && self.grasps[e.UnsMeleeUsed] == null)
                {
                    if (e.UnsMeleeWeapon.Peek() is null)
                    {
                        e.UnsMeleeWeapon.Clear();
                        return;
                    }
                    Ebug(self, "Unstable Weapon Mode was: " + e.UnsMeleeWeapon.Peek().mode);
                    if (self.room is not null && e.UnsMeleeWeapon.Peek().mode == Weapon.Mode.StuckInCreature)
                    {
                        self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, e.SFXChunk);
                    }

                    // Make Unstable be able to try tossing/throwing the explosive after they active it(no cooldown plz)
                    if (self.room is not null && e.UnsMeleeWeapon.Peek().mode != Weapon.Mode.StuckInWall)  // TODO: Also have it such that they pull it out if they have that dang remix option enabled
                    {
                        if (e.UnsMeleeWeapon.Peek() is ScavengerBomb or SingularityBomb or ExplosiveSpear)
                        {
                            Ebug(self, "Unstable has explosives and is trying desperately to throw it away!");
                        }
                        else
                        {
                            e.UnsMeleeStun = 15;
                        }
                        self.SlugcatGrab(e.UnsMeleeWeapon.Pop(), e.UnsMeleeUsed);
                    }
                    else
                    {
                        e.UnsMeleeWeapon.Pop();
                    }
                    e.UnsMeleeUsed = -1;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Unstable melee failed!");
            }
        }


        private void Esclass_US_MovementUpdate(Player self, ref Escort e)
        {
            // Midair jump
            if (e.UnsBlinkCD == 0 && self.input[0].jmp && !self.input[1].jmp && self.bodyChunks[0].ContactPoint.y != -1 && self.bodyChunks[1].ContactPoint.y != -1)
            {
                Ebug(self, "Midjump!", ignoreRepetition: true);
                Esclass_US_MidJump(self, e);
            }
            if (e.UnsBlinkCD == 0 && e.UnsBlinkWindow > 0 && self.input[0].thrw && !self.input[1].thrw)
            {
                Ebug(self, "HOMING MISSILE!", ignoreRepetition: true);
                e.UnsRockitCret = Esclass_US_RockitKick(self, e);
            }

            // Might want to deal with walljumps too later on
        }


        /// <summary>
        /// Calculates whether Unstable can blink. Guaranteed for the first two blinks.
        /// </summary>
        private static bool Esclass_US_CanBlinkYes(int n)
        {
            if (n < 3) return true;
            return UnityEngine.Random.value < Mathf.Pow(0.5f, n - 2);
        }


        /// <summary>
        /// Replaces the slugcat jump with a blink instead. Dunno the behaviour of jumps so this code will assume that the jump function will not always run when jump is pressed midair. Only meant to be used as the first blink
        /// It has come to my attention that this jump should replace the player jump since this is essentially how the player basically makes a jump.
        /// </summary>
        private bool Esclass_US_Jump(Player self, ref Escort e)
        {
            bool shortDash = self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG;

            // If in zero G, let the midjump handle it
            if (shortDash)
            {
                Esclass_US_MidJump(self, e);
                return true;
            }

            // Allow player to get off deer antlers (Needs more details)
            if (self.animation == Player.AnimationIndex.AntlerClimb)
            {
                self.animation = Player.AnimationIndex.Flip;
                self.playerInAntlers.playerDisconnected = true;
                self.playerInAntlers = null;
            }

            // Allow the player to slide lol by calling orig
            if (self.animation == Player.AnimationIndex.DownOnFours && self.bodyChunks[1].ContactPoint.y < 0 && self.input[0].downDiagonal == self.flipDirection)
            {
                return false;
            }

            // Flipping out of a roll or slide jump
            if (self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Roll)
            {
                self.animation = Player.AnimationIndex.Flip;
            }

            // If dash is available
            if (e.UnsBlinkCD == 0 && e.UnsBlinkFrame == 0 && (!e.UnsBlinking || e.UnsBlinkWindow > 0))
            {
                // May need to reevaluate
                // The cooldown needs to be turned on when the blink window reaches 0 after blink is used or the player fails the blink check
                e.UnsBlinkCount++;
                e.UnsBlinkWindow = 40;
                e.UnsBlinkDir = (0, 1);
                e.UnsBlinking = true;
                if (self.room is not null)
                {
                    // Change sound later to something more suitable
                    self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, 0.75f, 0.95f);
                    self.room.AddObject(new Explosion.ExplosionLight(self.mainBodyChunk.pos, 90f, 0.7f, 4, Color.gray));
                }
                e.UnsBlinkFrame++;  // Alt implementation
                //Esclass_US_Dash(self, true);


                if (!Esclass_US_CanBlinkYes(e.UnsBlinkCount))
                {
                    e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                    e.UnsBlinkCount = 0;
                    e.UnsBlinking = false;
                }
            }

            // Otherwise, do a miniature hop
            else if (e.UnsBlinkCD > 0)
            {
                Esclass_US_MiniHop(self);
            }
            
            return true;
        }


        /// <summary>
        /// Makes Unstable do a miniature hop that does barely anything... to be expanded upon so it's fully featured like a normal hop
        /// </summary>
        private void Esclass_US_MiniHop(Player self)
        {
            if (self.animation == Player.AnimationIndex.ClimbOnBeam)
            {
                self.jumpBoost = 0f;
                if (self.input[0].x != 0)
                {
                    self.animation = Player.AnimationIndex.None;
                    self.bodyChunks[0].vel.y = 2.5f;
                    self.bodyChunks[1].vel.y = 2f;
                    self.bodyChunks[0].vel.x = 2f * (float)self.flipDirection;
                    self.bodyChunks[1].vel.x = 1.5f * (float)self.flipDirection;
                    self.room.PlaySound(SoundID.Slugcat_From_Vertical_Pole_Jump, self.mainBodyChunk, false, 0.95f, 1f);
                    return;
                }
                if (self.input[0].y <= 0)
                {
                    self.animation = Player.AnimationIndex.None;
                    self.bodyChunks[0].vel.y = 1f;
                    if (self.input[0].y > -1)
                    {
                        self.bodyChunks[0].vel.x = 1f * (float)self.flipDirection;
                    }
                    self.room.PlaySound(SoundID.Slugcat_From_Vertical_Pole_Jump, self.mainBodyChunk, false, 0.25f, 1f);
                    return;
                }
                if (self.slowMovementStun < 1 && self.slideUpPole < 1)
                {
                    self.Blink(7);
                    self.bodyChunks[0].pos.y += 1.25f;
                    self.bodyChunks[1].pos.y += 1.25f;
                    self.bodyChunks[0].vel.y += 1f;
                    self.bodyChunks[1].vel.y += 1f;
                    self.slideUpPole = 17;
                    self.room.PlaySound(SoundID.Slugcat_From_Vertical_Pole_Jump, self.mainBodyChunk, false, 0.75f, 1f);
                    return;
                }
            }
            self.bodyChunks[0].vel.y = 2f;
            self.bodyChunks[1].vel.y = 1.5f;
            self.bodyChunks[0].vel.x = 1f * (float)self.flipDirection;
            self.bodyChunks[1].vel.x = -1.5f * (float)self.flipDirection;
            self.jumpBoost = 9f;
            self.room.PlaySound(SoundID.Slugcat_Normal_Jump, self.mainBodyChunk, false, 0.85f, 1f);
        }


        /// <summary>
        /// Handles the midair jumps or zeroG jumps
        /// May need to have an extra condition made in MovementUpdate or something for midair jumps
        /// Also I just realized this doesn't account for if the player is not holding a direction and they attenpt to press jump
        /// </summary>
        private void Esclass_US_MidJump(Player self, Escort e)
        {
            if (e.UnsBlinkCD == 0 && e.UnsBlinkFrame == 0 && (!e.UnsBlinking || e.UnsBlinkWindow > 0))
            {
                e.UnsBlinkCount++;
                e.UnsBlinkWindow = 40;
                e.UnsBlinkDifDir = false;
                e.UnsBlinkNoDir = self.input[0].x == 0 && self.input[0].y == 0;
                e.UnsBlinking = true;
                if (e.UnsBlinkDir != (self.input[0].x, self.input[0].y))
                {
                    e.UnsBlinkDir = (self.input[0].x, self.input[0].y);
                    e.UnsBlinkDifDir = true;
                }
                if (self.room is not null)
                {
                    // Change sound later to something more suitable
                    self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, 0.75f, 0.95f);
                    self.room.AddObject(new Explosion.ExplosionLight(self.mainBodyChunk.pos, 90f, 0.7f, 4, Color.white));
                }
                e.UnsBlinkFrame++;  // Alt implementation
                //Esclass_US_Dash(self, false, e.UnsBlinkDifDir);


                if (!Esclass_US_CanBlinkYes(e.UnsBlinkCount))
                {
                    e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                    e.UnsBlinkCount = 0;
                    e.UnsBlinking = false;
                }
            }
        }


        /// <summary>
        /// Checks terrain collisions for Unstable. Works semi-similarly to new escapist's except missing a few comparisons. TODO make sure the previous position also matches and the velocity resets also reset previous velocity
        /// </summary>
        public static void Esclass_US_Dash(Player self, bool upOnly = false, bool changeDir = false)
        {
            for (int i = 1; i < 10; i++)
            {
                // Calculate positions
                float xCom = 0;
                float yCom = 0;
                float xMov = 20 * self.input[0].x;
                float yMov = 20 * self.input[0].y;

                // Checks a tile ahead to see if it's possible to move to that space
                if (upOnly)
                {
                    yCom = self.bodyChunks[1].pos.y + 20;
                }
                else
                {
                    xCom = self.bodyChunks[1].pos.x + xMov;
                    yCom = self.bodyChunks[1].pos.y + yMov;
                }
                
                var tPos = self.room.GetTilePosition(new(xCom, yCom));
                Room.Tile rt = self.room.GetTile(tPos);

                // Allow Unstable to automatically grab onto the pole if they are holding up or down and they go towards the pole. Also stops all momentum so the slugcat doesn't go flying off the pole.
                if (self.input[0].y != 0 && rt.verticalBeam)
                {
                    self.bodyChunks[0].pos.x += xMov;
                    self.bodyChunks[1].pos.x += xMov;
                    self.bodyChunks[0].pos.y += yMov;
                    self.bodyChunks[1].pos.y += yMov;
                    self.bodyChunks[0].vel *= 0;
                    self.bodyChunks[1].vel *= 0;
                    self.dropGrabTile = tPos;
                    self.animation = Player.AnimationIndex.ClimbOnBeam;
                    self.bodyMode = Player.BodyModeIndex.ClimbingOnBeam;
                    return;
                }

                // Hit a wall?!
                if (rt.Solid)
                {
                    return;
                }

                // If no obstacle, move!
                if (i == 1 && changeDir)  // destroy all momentum if direction had been changed
                {
                    self.bodyChunks[0].vel *= 0;
                    self.bodyChunks[1].vel *= 0;
                }
                if (upOnly)
                {
                    self.bodyChunks[0].pos.y += 20;
                    self.bodyChunks[1].pos.y += 20;
                    self.bodyChunks[0].vel.y += 1.02f;
                    self.bodyChunks[1].vel.y += 1;
                }
                else
                {
                    self.bodyChunks[0].pos.x += xMov;
                    self.bodyChunks[1].pos.x += xMov;
                    self.bodyChunks[0].pos.y += yMov;
                    self.bodyChunks[1].pos.y += yMov;
                    self.bodyChunks[0].vel.x += 1 * self.input[0].x;
                    self.bodyChunks[1].vel.x += 1 * self.input[0].x;
                    self.bodyChunks[0].vel.y += 1 * self.input[0].y;
                    self.bodyChunks[1].vel.y += 1 * self.input[0].y;
                }
            }
        }

        /// <summary>
        /// Alternate implementation Returns true if end of blink
        /// </summary>
        public static bool Esclass_US_Dash2(Player self, in int frame, (int x, int y) dir, bool upOnly = false, bool changeDir = false)
        {
            // Calculate positions
            float xCom = self.bodyChunks[0].pos.x;
            float yCom = self.bodyChunks[0].pos.y;
            float xMov = 20 * dir.x;
            float yMov = 20 * dir.y;

            // Checks a tile ahead to see if it's possible to move to that space
            if (upOnly)
            {
                yCom += 20;
            }
            else
            {
                xCom += xMov;
                yCom += yMov;
            }

            IntVector2 tPos = self.room.GetTilePosition(new(xCom, yCom));
            Room.Tile rt = self.room.GetTile(tPos);
            // Room.Tile bt = self.room.GetTile(new Vector2(self.bodyChunks[0].pos.x, self.bodyChunks[0].pos.y));
            // Ebug(self, "Wall previous? " + bt.Solid + bt.Terrain + ", And now? " + rt.Solid + rt.Terrain, ignoreRepetition: true);



            // Allow Unstable to automatically grab onto the pole if they are holding up or down and they go towards the pole. Also stops all momentum so the slugcat doesn't go flying off the pole.
            if (self.input[0].y != 0 && rt.verticalBeam)
            {
                Ebug(self, "Cling to pole!");
                self.bodyChunks[0].pos.x += xMov;
                self.bodyChunks[1].pos.x += xMov;
                self.bodyChunks[0].pos.y += yMov;
                self.bodyChunks[1].pos.y += yMov;
                self.bodyChunks[0].vel *= 0;
                self.bodyChunks[1].vel *= 0;
                self.dropGrabTile = tPos;
                self.animation = Player.AnimationIndex.ClimbOnBeam;
                self.bodyMode = Player.BodyModeIndex.ClimbingOnBeam;
                return true;
            }

            // Hit a wall?!
            if (rt.Solid)
            {
                Ebug(self, "WALL!");
                return true;
            }

            // If no obstacle, move!
            if (frame == 1 && changeDir)  // destroy all momentum if direction had been changed
            {
                self.bodyChunks[0].vel *= 0;
                self.bodyChunks[1].vel *= 0;
            }

            float velT = 10;
            bool shortDash = self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG;
            if (!shortDash)
            {
                self.bodyChunks[0].vel.y += 1.4f;
                self.bodyChunks[1].vel.y += 1.4f;
            }


            if (upOnly)
            {
                self.bodyChunks[0].pos.y += 20;
                self.bodyChunks[1].pos.y += 20;
                // Though I could get away with increasing the velocity every frame, having it increase only at the end seemed funner
                if (frame == (shortDash? 6 : 9))
                {
                    self.bodyChunks[0].vel.y += velT + 1f;
                    self.bodyChunks[1].vel.y += velT;
                }
            }
            else
            {
                self.bodyChunks[0].pos.x += xMov;
                self.bodyChunks[1].pos.x += xMov;
                self.bodyChunks[0].pos.y += yMov;
                self.bodyChunks[1].pos.y += yMov;
                if (frame == (shortDash? 6 : 9))
                {
                    self.bodyChunks[0].vel.x += velT * dir.x;
                    self.bodyChunks[1].vel.x += velT * dir.x;
                    self.bodyChunks[0].vel.y += velT * dir.y;
                    self.bodyChunks[1].vel.y += velT * dir.y;
                }
            }
            return false;
        }

        private bool Esclass_US_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
        {
            if (self.grasps[grasp]?.grabbed is Weapon w)  // Only accepts throwing weapons. Normal stuff that will be just tossed may not need to be percentaged
            {
                float v = UnityEngine.Random.value;
                if (v > 0.5f)  // regular throw (50%)
                {
                    return false;  // does orig
                }
                else if (v > 0.2f)  // melee (30%)
                {
                    if (e.UnsMeleeWeapon.Count > 0)
                    {
                        e.UnsMeleeWeapon.Pop();
                    }
                    e.UnsMeleeWeapon.Push(w);
                    e.UnsMeleeGrab = 4;  // Do throw for 4 frames
                    e.UnsMeleeUsed = grasp;
                    return false;
                }
                else  // pathetic toss (20%)
                {
                    self.TossObject(grasp, eu);
                    Esclass_GD_ReplicateThrowBodyPhysics(self, grasp);
                    self.dontGrabStuff = 15;
                    self.ReleaseGrasp(grasp);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Homes into the nearest creature or nearest in selected direction and does a KICK
        /// </summary>
        private Creature Esclass_US_RockitKick(Player self, Escort e)
        {
            bool directional = false;
            float maxR = 100;
            Creature targit = null;

            if (self.input[0].x != 0 || self.input[0].y != 0)
            {
                directional = true;
                maxR = 150;
            }

            // General creature finder
            if (!directional)
            {
                try
                {
                    float closest = maxR;
                    foreach (UpdatableAndDeletable thing in self.room.updateList)
                    {
                        if (thing is Creature cret && cret != self && Custom.DistLess(cret.firstChunk.pos, self.mainBodyChunk.pos, closest))
                        {
                            closest = Custom.Dist(cret.firstChunk.pos, self.mainBodyChunk.pos);
                            targit = cret;
                            Ebug(self, "Found someone at " + closest, ignoreRepetition: true);
                        }
                    }
                }
                catch (Exception err)
                {
                    Ebug(err, "Something happened whilst trying to home into a creature!");
                }
            }
            else
            {
                try
                {
                    float closist = maxR;
                    foreach (UpdatableAndDeletable uad in self.room.updateList)
                    {
                        if (uad is Creature crit && crit != self && self.ConeDetection(crit.firstChunk.pos, closist, InputToDeg(self.input[0]), 15))
                        {
                            closist = Custom.Dist(crit.firstChunk.pos, self.mainBodyChunk.pos);
                            targit = crit;
                            Ebug(self, "Found someone at " + closist + ", angle: " + Custom.VecToDeg(Custom.DirVec(self.firstChunk.pos, crit.firstChunk.pos)), ignoreRepetition: true);
                        }
                    }
                }
                catch (Exception err)
                {
                    Ebug(err, "Something happened whilst trying to home into a creature directionwise!");
                }
            }

            // // Home towards creature!
            // if (targit is not null)
            // {
            // }
            return targit;
        }

    }
}
