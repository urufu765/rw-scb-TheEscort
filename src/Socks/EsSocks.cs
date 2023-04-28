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
    /// It's Socks!
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {

        private static bool unplayableSocks => ins.L().crispmunch;
        //private static bool playingSocks = false;

        private void Socks_ctor(Player self){
            self.setPupStatus(true);
        }

        private static bool Socks_hideTheSocks(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            try{
                if (i == null){
                    Ebug("Found nulled slugcat name when hiding cats!", 1);
                    return orig(i);
                }
                if (i == EscortSocks){
                    return unplayableSocks;
                }
            } catch (Exception err){
                Ebug(err, "Couldn't hide the socks in the drawer!");
            }
            return orig(i);
        }

        private void Socks_Update(On.Player.orig_Update orig, Player self, bool eu){
            ins.L().setF();
            orig(self, eu);
            try{
                if (self.slugcatStats.name != EscortSocks){
                    return;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                Ebug(self, err, "Couldn't do Socks update!");
                return;
            }
            if (!sCon.TryGetValue(self, out Socks es)){
                return;
            }
            ins.L().setF("Socks CWT");

            // Generate backpack!
            if (self.animation != Player.AnimationIndex.DeepSwim && es.backpack == null && es.Escat_clock_backpackReGen()){
                ins.L().setF("Socks Backpack Check");
                es.Escat_generate_backpack(self);
            }

            // Force respawn backpack when it dies
            if (self.grasps.Length == 3 && self.grasps[2] != null && self.grasps[2].grabbed != null && (self.grasps[2].grabbed as GrappleBackpack).dead){
                Ebug("Backpack is dead. Rip");
                self.ReleaseGrasp(2);
            }

            // Respawn backpack
            if (self.grasps.Length == 3 && self.grasps[2] == null && es.backpack != null){
                Ebug("Regenerate backpack!");
                es.Escat_kill_backpack();
                es.Escat_clock_backpackReGen(10);
            }

            // Allow the backpack to actually be used lol (change this later so that it's compatible with remap mods)
            if (self.input[0].jmp && !self.input[1].jmp){
                if (self.grasps[2] != null && self.grasps[2].grabbed is GrappleBackpack){
                    (self.grasps[2].grabbed as GrappleBackpack).JumpButton(self);
                }
            }
        }

        private void Socks_GMU(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
        {
            ins.L().setF();
            try{
                if (self.slugcatStats.name != EscortSocks){
                    orig(self, actuallyViewed, eu);
                    return;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                orig(self, actuallyViewed, eu);
                Ebug(self, err, "Couldn't do Socks GMU!");
                return;
            }
            if (!sCon.TryGetValue(self, out Socks es)){
                orig(self, actuallyViewed, eu);
                return;
            }
            ins.L().setF("Socks CWT");
            if (es.backpack != null){
                ins.L().setF("Socks Backpack Check");
                es.backpack.GraphicsModuleUpdated(actuallyViewed, eu);
            }
            orig(self, actuallyViewed, eu);
        }

        private void Socks_Mine(On.Player.orig_SlugcatGrab orig, Player self, PhysicalObject obj, int graspUsed)
        {
            ins.L().setF();
            try{
                if (self.slugcatStats.name != EscortSocks){
                    orig(self, obj, graspUsed);
                    return;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                orig(self, obj, graspUsed);
                Ebug(self, err, "Couldn't do Socks Grab Log!");
                return;
            }
            int j = 0;
            int k = self.grasps.Length;
            int l = 0;
            for (int i = 0; i < k; i++){
                if (self.grasps[i] != null){
                    j++;
                    if (i < 2) l++;
                }
            }
            Ebug("Total grasps: " + (k) + ", Used (with / without grap): " + j + "/" + l);
            orig(self, obj, graspUsed);
        }

        private bool Socks_Grabby(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            ins.L().setF();
            if (obj is GrappleBackpack){
                return false;
            }
            try{
                if (self.slugcatStats.name == EscortSocks && obj is TubeWorm){
                    return false;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                Ebug(self, err, "Couldn't do Socks Pick up check!");
                return orig(self, obj);
            }
            
            return orig(self, obj);
        }

        private void Socks_DontLoseBackpack(On.Creature.orig_LoseAllGrasps orig, Creature self)
        {
            try{
                if (!(self is Player p && p.slugcatStats.name == EscortSocks)){
                    orig(self);
                    return;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                Ebug(err, "Couldn't do Socks Backpack invincibility!");
                orig(self);
                return;
            }
            if (self.Template.grasps > 0){
                for (int i = 0; i < 2; i++){
                    self.ReleaseGrasp(i);
                }
            }
        }

        /// <summary>
        /// Kills the grapple backpack when Socks dies so other players can grab Socks. May not be 100% working as intended with Revivify
        /// </summary>
        private void Socks_Death(On.Player.orig_Die orig, Player self){
            try{
                if (self.slugcatStats.name != EscortSocks){
                    orig(self);
                    return;
                }
                ins.L().setF("Socks Check");
            } catch (Exception err){
                Ebug(self, err, "Couldn't do Socks Death!");
                orig(self);
                return;
            }
            if (self.tubeWorm != null){
                self.tubeWorm.Die();
            }
            orig(self);
        }

        /// <summary>
        /// Stops Socks from having an aneurysm whenever they climb poles.
        /// </summary>
        private float Socks_Stop_Having_An_Aneurysm(On.PlayerGraphics.PlayerObjectLooker.orig_HowInterestingIsThisObject orig, PlayerGraphics.PlayerObjectLooker self, PhysicalObject obj)
        {
            ins.L().setF();
            try{
                if (!(self != null && self.owner != null && self.owner.player != null)){
                    return orig(self, obj);
                }
                ins.L().setF("Null Check");
                if (self.owner.player.slugcatStats.name != EscortSocks){
                    return orig(self, obj);
                }
                ins.L().setF("Socks Check");
                if (obj is GrappleBackpack){
                    return 0f;
                }
                return orig(self, obj);
            } catch(Exception err){
                Ebug(err, "Something went wrong while trying to stop socks from having a laugh!");
                return orig(self, obj);
            }
        }


        private void Socks_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam)
        {
            ins.L().set();
            orig(self, s, rCam);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                ins.L().set("Null Check");
                if (self.player.slugcatStats.name != EscortSocks){
                    return;
                }
                ins.L().set("Socks Check");
                if (!sCon.TryGetValue(self.player, out Socks es)){
                    return;
                }
                ins.L().set("Socks CWT Access");
                //s.sprites[e.spriteQueue]
                for (int i = es.spriteQueue; i < es.spriteQueue + es.customSprites; i++){
                    if (s.sprites[es.spriteQueue] == null){
                        Ebug(self.player, "Oh geez. No sprites?", 0);
                    }
                }

                ins.L().set("Successful Spriting Check");
                self.AddToContainer(s, rCam, null);
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when initiating sprites!");
                return;
            }
        }

    }
}