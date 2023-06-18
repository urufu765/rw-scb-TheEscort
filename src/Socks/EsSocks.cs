using BepInEx;
using IL.MoreSlugcats;
using System;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort
{
    /// <summary>
    /// It's Socks!
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {

        private static bool UnplayableSocks => ins.L().Crispmunch;
        //private static bool playingSocks = false;

        private void Socks_ctor(Player self)
        {
            self.setPupStatus(true);
        }

        private static bool Socks_hideTheSocks(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            try
            {
                if (i == null)
                {
                    Ebug("Found nulled slugcat name when hiding cats!", 1);
                    return orig(i);
                }
                if (i == EscortSocks)
                {
                    return UnplayableSocks;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Couldn't hide the socks in the drawer!");
            }
            return orig(i);
        }

        private void Socks_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            ins.L().SetF();
            orig(self, eu);
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    return;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't do Socks update!");
                return;
            }
            if (!sCon.TryGetValue(self, out Socks es))
            {
                return;
            }
            ins.L().SetF("Socks CWT");

            self.slugcatStats.runspeedFac = es.Escat_socks_runspd(self.Malnourished, self.aerobicLevel, self.Adrenaline);
            self.slugcatStats.poleClimbSpeedFac = es.Escat_socks_climbspd(self.Malnourished, self.aerobicLevel, self.Adrenaline);
            self.slugcatStats.corridorClimbSpeedFac = es.Escat_socks_corrspd(self.Malnourished, self.aerobicLevel, self.Adrenaline);
            self.slugcatStats.throwingSkill = es.Escat_socks_skillz(self.Malnourished, self.aerobicLevel - self.Adrenaline);
            if (self.aerobicLevel > 0.99f && !self.exhausted)
            {
                self.exhausted = true;
            }
            if (self.aerobicLevel < 0.34f && self.exhausted)
            {
                self.exhausted = false;
            }


            // Generate backpack!
            if (self.animation != Player.AnimationIndex.DeepSwim && es.backpack == null && es.Escat_clock_backpackReGen())
            {
                ins.L().SetF("Socks Backpack Check");
                es.Escat_generate_backpack(self);
            }

            // Force respawn backpack when it dies
            if (self.grasps.Length == 3 && self.grasps[2] != null && self.grasps[2].grabbed != null && (self.grasps[2].grabbed as TubeWorm).dead)
            {
                Ebug("Backpack is dead. Rip");
                self.ReleaseGrasp(2);
            }

            // Respawn backpack
            if (self.grasps.Length == 3 && self.grasps[2] == null && es.backpack != null)
            {
                Ebug("Regenerate backpack!");
                es.Escat_kill_backpack();
                es.Escat_clock_backpackReGen(10);
            }

            // Swap backpacks! (disabled for now)
            if (false && (self.bodyMode == Player.BodyModeIndex.ZeroG || self.standing || self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam))
            {
                es.Escat_swap_backpack(self);
            }

            // Allow the backpack to actually be used lol (change this later so that it's compatible with remap mods)
            if (self.input[0].jmp && !self.input[1].jmp)
            {
                if (self.grasps[2] != null && self.grasps[2].grabbed is GrappleBackpack)
                {
                    (self.grasps[2].grabbed as GrappleBackpack).JumpButton(self);
                }
                if (self.grasps[2] != null && self.grasps[2].grabbed is LauncherBackpack)
                {
                    (self.grasps[2].grabbed as LauncherBackpack).JumpButton(self);
                }

            }
        }

        private void Socks_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    return;
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't do Socks update!");
                return;
            }
            self.aerobicLevel = Mathf.Min(0, self.aerobicLevel - 0.03f);
        }

        private void Socks_GMU(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
        {
            ins.L().SetF();
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    orig(self, actuallyViewed, eu);
                    return;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                orig(self, actuallyViewed, eu);
                Ebug(self, err, "Couldn't do Socks GMU!");
                return;
            }
            if (!sCon.TryGetValue(self, out Socks es))
            {
                orig(self, actuallyViewed, eu);
                return;
            }
            ins.L().SetF("Socks CWT");
            if (es.backpack != null)
            {
                ins.L().SetF("Socks Backpack Check");
                es.backpack.GraphicsModuleUpdated(eu);
            }
            orig(self, actuallyViewed, eu);
        }

        private void Socks_Mine(On.Player.orig_SlugcatGrab orig, Player self, PhysicalObject obj, int graspUsed)
        {
            ins.L().SetF();
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    orig(self, obj, graspUsed);
                    return;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                orig(self, obj, graspUsed);
                Ebug(self, err, "Couldn't do Socks Grab Log!");
                return;
            }
            int j = 0;
            int k = self.grasps.Length;
            int l = 0;
            for (int i = 0; i < k; i++)
            {
                if (self.grasps[i] != null)
                {
                    j++;
                    if (i < 2) l++;
                }
            }
            Ebug("Total grasps: " + (k) + ", Used (with / without grap): " + j + "/" + l);
            orig(self, obj, graspUsed);
        }

        private bool Socks_Grabby(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            ins.L().SetF();
            if (obj is GrappleBackpack)
            {
                return false;
            }
            try
            {
                if (self.slugcatStats.name == EscortSocks && obj is TubeWorm)
                {
                    return false;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't do Socks Pick up check!");
                return orig(self, obj);
            }

            return orig(self, obj);
        }

        private void Socks_DontLoseBackpack(On.Creature.orig_LoseAllGrasps orig, Creature self)
        {
            try
            {
                if (!(self is Player p && p.slugcatStats.name == EscortSocks))
                {
                    orig(self);
                    return;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                Ebug(err, "Couldn't do Socks Backpack invincibility!");
                orig(self);
                return;
            }
            if (self.Template.grasps > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    self.ReleaseGrasp(i);
                }
            }
        }

        /// <summary>
        /// Kills the grapple backpack when Socks dies so other players can grab Socks. May not be 100% working as intended with Revivify
        /// </summary>
        private void Socks_Death(On.Player.orig_Die orig, Player self)
        {
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    orig(self);
                    return;
                }
                ins.L().SetF("Socks Check");
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't do Socks Death!");
                orig(self);
                return;
            }
            self.tubeWorm?.Die();
            orig(self);
        }

        /// <summary>
        /// Stops Socks from having an aneurysm whenever they climb poles.
        /// </summary>
        private float Socks_Stop_Having_An_Aneurysm(On.PlayerGraphics.PlayerObjectLooker.orig_HowInterestingIsThisObject orig, PlayerGraphics.PlayerObjectLooker self, PhysicalObject obj)
        {
            ins.L().SetF();
            try
            {
                if (!(self != null && self.owner != null && self.owner.player != null))
                {
                    return orig(self, obj);
                }
                ins.L().SetF("Null Check");
                if (self.owner.player.slugcatStats.name != EscortSocks)
                {
                    return orig(self, obj);
                }
                ins.L().SetF("Socks Check");
                if (obj is GrappleBackpack || obj is LauncherBackpack)
                {
                    return 0f;
                }
                return orig(self, obj);
            }
            catch (Exception err)
            {
                Ebug(err, "Something went wrong while trying to stop socks from having a laugh!");
                return orig(self, obj);
            }
        }


        private void Socks_Sticky_Immune(On.TubeWorm.orig_Update orig, TubeWorm self, bool eu)
        {
            orig(self, eu);
            try
            {
                for (int i = 0; i < self.tongues.Length; i++)
                {
                    if (self.tongues[i].mode == TubeWorm.Tongue.Mode.AttachedToObject && self.tongues[i].attachedChunk != null && self.tongues[i].attachedChunk.owner is Player)
                    {
                        Player p = self.tongues[i].attachedChunk.owner as Player;
                        if (p.slugcatStats.name == EscortSocks)
                        {
                            self.tongues[i].Release();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Oh no Tubeworm vengence!");
            }
        }


        private void Socks_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam)
        {
            ins.L().Set();
            orig(self, s, rCam);
            try
            {
                if (!(self != null && self.player != null))
                {
                    return;
                }
                ins.L().Set("Null Check");
                if (self.player.slugcatStats.name != EscortSocks)
                {
                    return;
                }
                ins.L().Set("Socks Check");
                if (!sCon.TryGetValue(self.player, out Socks es))
                {
                    return;
                }
                ins.L().Set("Socks CWT Access");
                //s.sprites[e.spriteQueue]
                for (int i = es.spriteQueue; i < es.spriteQueue + es.customSprites; i++)
                {
                    if (s.sprites[es.spriteQueue] == null)
                    {
                        Ebug(self.player, "Oh geez. No sprites?", 0);
                    }
                }

                ins.L().Set("Successful Spriting Check");
                self.AddToContainer(s, rCam, null);
            }
            catch (Exception err)
            {
                Ebug(self.player, err, "Something went wrong when initiating sprites!");
                return;
            }
        }


        /// <summary>
        /// Allows Socks to use the grapple worm with legacy controls
        /// </summary>
        /// <param name="orig">Original Function call</param>
        /// <param name="self">Player instance</param>
        /// <param name="eu">Even Update</param>
        private void Socks_Legacy(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            try
            {
                if (self.slugcatStats.name != EscortSocks)
                {
                    return;
                }

                // Checks for the third grasp which Socks will definitely have. Controls will be similar to Saint with legacy tongue controls.
                if (self.wantToThrow > 0 && ModManager.MMF && MoreSlugcats.MMF.cfgOldTongue.Value && self.grasps[0] is null && self.grasps[1] is null && self.grasps.Length == 3 && self.grasps[2]?.grabbed is TubeWorm)
                {
                    self.ThrowObject(2, eu);
                    self.wantToThrow = 0;
                }
            }
            catch (NullReferenceException nerr)
            {
                Ebug(self, nerr, "Null exception while doing socks grab update!");
            }
            catch (Exception err)
            {
                Ebug(self, err, "Couldn't do Socks Grab Update!");
            }
        }


    }
}