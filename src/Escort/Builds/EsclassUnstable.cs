using BepInEx;
using SlugBase.Features;
using System;
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
            // Constant tripping clock
            if (e.UnsTripTime < 40)
            {
                e.UnsTripTime++;
            }
            else
            {
                e.UnsTripTime = 0;
            }

            // May need revisiting
            if (self.canJump > 0 && e.UnsBlinkCount > 0 && e.UnsBlinkCD == 0 && e.UnsBlinkWindow == 0)
            {
                e.UnsBlinkCount--;
            }

            // 
            if (e.UnsBlinkWindow > 0 && e.UnsBlinkFrame == 0)
            {
                e.UnsBlinkWindow--;
            }
            else
            {
                if (e.UnsBlinking)
                {
                    e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                    e.UnsBlinking = false;
                }
            }

            //
            if (e.UnsBlinkCD > 0 && e.UnsBlinkFrame == 0)
            {
                e.UnsBlinkCD--;
            }

            // For alternate version of blink
            if (e.UnsBlinkFrame > 0 && e.UnsBlinkFrame < 10)
            {
                e.UnsBlinkFrame++;
            }
            else if (e.UnsBlinkFrame >= 10)
            {
                e.UnsBlinkFrame = 0;  // Redundancy never hurts
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
        }

        private void Esclass_US_Update(Player self, ref Escort e)
        {
            // Trippin' time!
            if (e.UnsTripTime == 0 && UnityEngine.Random.value > 0.9f && self.bodyMode != Player.BodyModeIndex.ZeroG && self.standing && self.bodyChunks[1].ContactPoint.y == -1)
            {
                Ebug(self, "Unstable fucking tripped! Laugh at 'em!");
                self.standing = false;
                self.bodyChunks[0].vel.x += self.rollDirection * 7f;
                self.bodyChunks[1].vel.x -= self.rollDirection * 5f;
                self.bodyChunks[0].vel.y -= 3f;
                self.Stun(20);
                if (UnityEngine.Random.value > 0.7f)
                {
                    self.LoseAllGrasps();
                }
            }

            if (e.UnsBlinkFrame > 0)
            {
                if (Esclass_US_Dash2(self, in e.UnsBlinkFrame, e.UnsBlinkDir, e.UnsBlinkDir == (0, 1) || e.UnsBlinkNoDir, e.UnsBlinkDifDir))
                {
                    e.UnsBlinkNoDir = false;
                    e.UnsBlinkFrame = 0;  // Redundancy never hurts
                }
            }

            if (e.UnsMeleeGrab == 0 && e.UnsMeleeUsed >= 0 && self.grasps[e.UnsMeleeUsed] is null)
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


        private static void Esclass_US_MovementUpdate(Player self, ref Escort e)
        {
            // Midair jump
            if (e.UnsBlinkCD == 0 && self.wantToJump > 0 && self.canJump == 0 && self.bodyChunks[0].ContactPoint.y != -1 && self.bodyChunks[1].ContactPoint.y != -1)
            {
                Esclass_US_MidJump(self, e);
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
        private void Esclass_US_Jump(Player self, ref Escort e)
        {
            if (self.bodyMode == Player.BodyModeIndex.ZeroG)
            {
                Esclass_US_MidJump(self, e);
                return;
            }
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
            }
            if (!Esclass_US_CanBlinkYes(e.UnsBlinkCount))
            {
                e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                e.UnsBlinking = false;
            }
        }


        /// <summary>
        /// Handles the midair jumps or zeroG jumps
        /// May need to have an extra condition made in MovementUpdate or something for midair jumps
        /// Also I just realized this doesn't account for if the player is not holding a direction and they attenpt to press jump
        /// </summary>
        public static void Esclass_US_MidJump(Player self, Escort e)
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
            }
            if (!Esclass_US_CanBlinkYes(e.UnsBlinkCount))
            {
                e.UnsBlinkCD = self.Malnourished ? 120 : 80;
                e.UnsBlinking = false;
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
            float xCom = 0;
            float yCom = 0;
            float xMov = 20 * dir.x;
            float yMov = 20 * dir.y;

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
                return true;
            }

            // Hit a wall?!
            if (rt.Solid)
            {
                return true;
            }

            // If no obstacle, move!
            if (frame == 1 && changeDir)  // destroy all momentum if direction had been changed
            {
                self.bodyChunks[0].vel *= 0;
                self.bodyChunks[1].vel *= 0;
            }
            if (upOnly)
            {
                self.bodyChunks[0].pos.y += 20;
                self.bodyChunks[1].pos.y += 20;
                // Though I could get away with increasing the velocity every frame, having it increase only at the end seemed funner
                if (frame == 9)
                {
                    self.bodyChunks[0].vel.y += 11f;
                    self.bodyChunks[1].vel.y += 10;
                }
            }
            else
            {
                self.bodyChunks[0].pos.x += xMov;
                self.bodyChunks[1].pos.x += xMov;
                self.bodyChunks[0].pos.y += yMov;
                self.bodyChunks[1].pos.y += yMov;
                if (frame == 9)
                {
                    self.bodyChunks[0].vel.x += 10 * dir.x;
                    self.bodyChunks[1].vel.x += 10 * dir.x;
                    self.bodyChunks[0].vel.y += 10 * dir.y;
                    self.bodyChunks[1].vel.y += 10 * dir.y;
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
                    e.UnsMeleeGrab = 3;  // Do throw for 3 frames (may need to do 5 incase it doesn't travel far enough)
                    e.UnsMeleeUsed = grasp;
                    return true;
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
    }
}
