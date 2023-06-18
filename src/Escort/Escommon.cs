using BepInEx;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort
{
    /// <summary>
    /// For methods specific to Escort that are used by all builds
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {

        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
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
            if (!Esconfig_WallJumps(self))
            {
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return;
            }
            if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0)
            {
                String msg = "Nothing New";
                self.canWallJump = 0;
                if (self.input[0].jmp)
                {
                    msg = "Is touching the jump button";
                    if (self.superLaunchJump < 20)
                    {
                        self.superLaunchJump += 2;
                        if (self.Adrenaline == 1f && self.superLaunchJump < 6)
                        {
                            self.superLaunchJump = 6;
                        }
                    }
                    else
                    {
                        self.killSuperLaunchJumpCounter = 15;
                    }
                }

                if (!self.input[0].jmp && self.input[1].jmp)
                {
                    msg = "Lets go of the jump button";
                    self.wantToJump = 1;
                }
                if (e.consoleTick == 0)
                {
                    Ebug(self, msg, 2);
                }
            }
        }


        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self);
                    return;
                }
            }
            catch (Exception err)
            {
                orig(self);
                Ebug(self, err);
                return;
            }
            if (!Esconfig_WallJumps(self))
            {
                orig(self);
                return;
            }

            //Ebug(self, "CheckInput Triggered!");
            int previously = self.input[0].x;
            orig(self);

            // Undoes the input cancellation
            if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1)
            {
                if (self.input[0].x == 0)
                {
                    self.input[0].x = previously;
                }
            }
        }

        /// <summary>
        /// Immitates a wall jump but not really
        /// </summary>
        private static void Escort_FakeWallJump(Player self, int direction=-2, float boostUp=18f, float yankUp=10f, float boostLR=5f){
            self.bodyChunks[0].vel.y = boostUp;
            self.bodyChunks[1].vel.y = boostUp - 1f;
            self.bodyChunks[0].pos.y += yankUp;
            self.bodyChunks[1].pos.y += yankUp;
            if (direction != -2){
                self.bodyChunks[0].vel.x = boostLR * direction;
                self.bodyChunks[1].vel.x = (boostLR - 1) * direction;
            }
        }


        // Implement a different type of dropkick
        private void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu)
        {
            orig(self, grasp, eu);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    return;
                }
                if (!BodySlam.TryGet(self, out var bodySlam) ||
                    !eCon.TryGetValue(self, out Escort e))
                {
                    return;
                }

                Ebug(self, "Toss Object Triggered!");
                if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead)
                {
                    if (Esconfig_SFX(self) && e.LizGet != null)
                    {
                        e.LizGet.Volume = 0f;
                    }
                    if (self.bodyMode == Player.BodyModeIndex.Default && (!e.Brawler || e.BrawThrowGrab == 0))
                    {
                        self.animation = Player.AnimationIndex.RocketJump;
                        self.bodyChunks[1].vel.x += self.ThrowDirection;
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(self, err);
                return;
            }
        }


        private void Escort_Die(On.Player.orig_Die orig, Player self)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e))
                {
                    orig(self);
                    return;
                }

                Ebug(self, "Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0)
                {
                    orig(self);
                    if (self.dead && Esconfig_SFX(self) && self.room != null)
                    {
                        self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug(self, "Failure.", 1);
                }
                else
                {
                    self.dead = false;
                    Ebug(self, "Player didn't die?", 1);
                    e.ParrySuccess = false;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Something happened while trying to die!");
                orig(self);
            }
        }

        /// <summary>
        /// Makes Escort eat held items twice as fast
        /// </summary>
        /// <param name="orig">Original function call (pass the method along)</param>
        /// <param name="self">Player instance</param>
        /// <param name="eu">Even Updates</param>
        private void Escort_Eated(On.Player.orig_BiteEdibleObject orig, Player self, bool eu)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
                {
                    orig(self, eu);
                    return;
                }
                for (int a = 0; a < 2; a++)
                {
                    if (self.grasps[a] != null && self.grasps[a].grabbed is IPlayerEdible ipe && ipe.Edible)
                    {
                        if (ipe.BitesLeft > 1)
                        {
                            if (self.grasps[a].grabbed is Fly)
                            {
                                for (int b = 0; b < ipe.BitesLeft; b++)
                                {
                                    ipe.BitByPlayer(self.grasps[a], eu);
                                }
                            }
                            else
                            {
                                ipe.BitByPlayer(self.grasps[a], eu);
                            }
                        }
                        break;
                    }
                }
                orig(self, eu);
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error when eated!");
                orig(self, eu);
                return;
            }
        }


    }
}
