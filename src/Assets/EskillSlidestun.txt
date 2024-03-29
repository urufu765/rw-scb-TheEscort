using BepInEx;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    public static class EskSS
    {
        /// <summary>
        /// The default option for Escorts. Makes them launch forwards for easy escape.
        /// </summary>
        public static void SS_Hurdle_Leap(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                target.Violence(self.mainBodyChunk, self.mainBodyChunk.vel / 4f, target.firstChunk, null, Creature.DamageType.Blunt, dmg, stn);
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to violence the creature!");
            }

            // Effects
            try
            {
                self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, self.mainBodyChunk);
                self.room.AddObject(
                    new ExplosionSpikes(
                        self.room, 
                        self.bodyChunks[1].pos + new Vector2(
                            0f, -self.bodyChunks[1].rad
                        ), 
                        8, 7f, 5f, 5.5f, 40f, 
                        new Color(0f, 0.35f, 1f, 0f)
                    )
                );
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to apply visual and aural FX");
            }

            // Movement
            try
            {
                int direction;
                if (self.longBellySlide)
                {
                    direction = self.rollDirection;  // Unused?
                    self.bodyChunks[1].vel.x = self.slideDirection * tossMod - 1;
                    self.bodyChunks[0].vel.x = self.slideDirection * tossMod;

                    Escort_FakeWallJump(self, boostUp:21, yankUp:10);
                    Ebug(self, "Grandstunslided!", 2);
                }
                else
                {
                    direction = self.flipDirection;
                    self.mainBodyChunk.vel.x *= tossFac;
                    if (self.rocketJumpFromBellySlide)
                    {
                        Ebug(self, "No more no control!");
                        self.rocketJumpFromBellySlide = false;
                    }

                    self.WallJump(direction);
                    self.jumpBoost += 8;
                    self.animation = Player.AnimationIndex.Flip;
                    Ebug(self, "Stunslided!", 2);
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make the player move!");
            }
        }


        /// <summary>
        /// (INCOMPLETE) Launches Escort upwards instead of horizontally, allowing much easier downthrow skillshots.
        /// </summary>
        public static void SS_Hurdle_Launch(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to affect creature when slidestunning!");
            }

            // Effects
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to create visual/aural effect when slidestunning!");
            }

            // Player Movement
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make player move when slidestunning!");
            }
        }


        /// <summary>
        /// (INCOMPLETE) Launches Escort the opposite direction from impact direction sort of like a whiplash but on steroids.
        /// </summary>
        public static void SS_Backfire(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to affect creature when slidestunning!");
            }

            // Effects
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to create visual/aural effect when slidestunning!");
            }

            // Player Movement
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make player move when slidestunning!");
            }
        }


        /// <summary>
        /// (INCOMPLETE) Transfers Escort's sliding momentum right into the creature, stopping Escort dead in their tracks and dealing damage.
        /// </summary>
        public static void SS_Body_Slam(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to affect creature when slidestunning!");
            }

            // Effects
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to create visual/aural effect when slidestunning!");
            }

            // Player Movement
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make player move when slidestunning!");
            }
        }


        /// <summary>
        /// (INCOMPLETE) Allows Escort to pass beneath the creature, before pushing off of them to slide further.
        /// </summary>
        public static void SS_Relay(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to affect creature when slidestunning!");
            }

            // Effects
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to create visual/aural effect when slidestunning!");
            }

            // Player Movement
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make player move when slidestunning!");
            }
        }


        /// <summary>
        /// (INCOMPLETE) Allows Escort to pass under a creature, making them drop everything in their hands/mouth.
        /// </summary>
        public static void SS_Tackle(this Creature target, Player self, Vector2 momNdir, float dmg, float stn, float tossMod = 18, float tossFac = 1)
        {
            // Creature Affect
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to affect creature when slidestunning!");
            }

            // Effects
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to create visual/aural effect when slidestunning!");
            }

            // Player Movement
            try
            {
                // code
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to make player move when slidestunning!");
            }
        }
    }
}