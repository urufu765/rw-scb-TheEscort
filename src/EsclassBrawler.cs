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
        // Brawler tweak values
        // public static readonly PlayerFeature<> brawler = Player("theescort/brawler/");
        // public static readonly PlayerFeature<float> brawler = PlayerFloat("theescort/brawler/");
        // public static readonly PlayerFeature<float[]> brawler = PlayerFloats("theescort/brawler/");
        public static readonly PlayerFeature<float> brawlerSlideLaunchFac = PlayerFloat("theescort/brawler/slide_launch_fac");
        public static readonly PlayerFeature<float> brawlerDKHypeDmg = PlayerFloat("theescort/brawler/dk_h_dmg");
        public static readonly PlayerFeature<float[]> brawlerSpearVelFac = PlayerFloats("theescort/brawler/spear_vel_fac");
        public static readonly PlayerFeature<float[]> brawlerSpearDmgFac = PlayerFloats("theescort/brawler/spear_dmg_fac");
        public static readonly PlayerFeature<float> brawlerSpearThrust = PlayerFloat("theescort/brawler/spear_thrust");
        public static readonly PlayerFeature<float[]> brawlerSpearShankY = PlayerFloats("theescort/brawler/spear_shank");


        private bool Esclass_Braw_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj){
            if (obj.TotalMass <= self.TotalMass * ratioed*2){
                if (obj is Creature c && c is not Lizard && !c.dead){
                    return orig(self, obj);
                }
                return false;
            }
            return orig(self, obj);
        }


        private void Esclass_Braw_ThrownSpear(Player self, Spear spear, ref Escort e, ref float thrust){
            if (
                !brawlerSpearVelFac.TryGet(self, out float[] bSpearVel) ||
                !brawlerSpearDmgFac.TryGet(self, out float[] bSpearDmg) ||
                !brawlerSpearThrust.TryGet(self, out float bSpearThr) ||
                !brawlerSpearShankY.TryGet(self, out float[] bSpearY)
            ){
                return;
            }
            try{
                spear.spearDamageBonus *= bSpearDmg[0];
                if (self.bodyMode == Player.BodyModeIndex.Crawl){
                    spear.firstChunk.vel.x *= bSpearVel[0];
                }
                else if (self.bodyMode == Player.BodyModeIndex.Stand){
                    spear.firstChunk.vel.x *= bSpearVel[1];
                } else {
                    spear.firstChunk.vel.x *= bSpearVel[2];
                }
                thrust *= 0.5f;
                if (e.BrawShankMode){
                    //spear.throwDir = new RWCustom.IntVector2(0, -1);
                    spear.firstChunk.vel = e.BrawShankDir;
                    //spear.firstChunk.vel.y = -(Math.Abs(spear.firstChunk.vel.y)) * bSpearY[0];
                    //spear.firstChunk.pos += new Vector2(0f, bSpearY[1]);
                    spear.firstChunk.vel *= bSpearY[0];
                    //spear.doNotTumbleAtLowSpeed = true;
                    e.BrawShankMode = false;
                    spear.spearDamageBonus = bSpearDmg[1];
                    if (self.room != null){
                        self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, e.SFXChunk, false, 1f, 2f);
                    }
                }
            } catch (Exception err){
                Ebug(self, err, "Error while applying Brawler-specific speartoss");
            }
        }


        private void Esclass_Braw_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e){
            if (!(self.grasps[grasp] != null && self.grasps[1 - grasp] != null)){
                return;
            }
            for (int j = 0; j < 2; j++){
                if (self.grasps[j].grabbed is Spear s &&
                self.grasps[1 - j].grabbed is Creature cs){
                    if (cs.dead || cs.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || cs.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && cs is Player && !RWCustom.Custom.rainWorld.options.friendlyFire)){
                        break;
                    }
                    Creature c = cs;
                    c.firstChunk.vel.y += 1f;
                    orig(self, 1 - j, eu);
                    
                    //s.alwaysStickInWalls = false;
                    //if (c.mainBodyChunk != null){
                    //    s.meleeHitChunk = c.mainBodyChunk;
                    //}
                    
                    //s.firstChunk.pos = self.mainBodyChunk.pos + new Vector2(0f, 80f);
                    s.firstChunk.vel = new Vector2(c.mainBodyChunk.pos.x - s.firstChunk.pos.x, c.mainBodyChunk.pos.y - s.firstChunk.pos.y).normalized;
                    //Vector2 v = (c.firstChunk.pos - s.firstChunk.pos).normalized * 3f;
                    e.BrawShankDir = s.firstChunk.vel;
                    Ebug(self, "Throw Spear at Creature!");
                    e.BrawShankMode = true;
                    orig(self, j, eu);
                    return;
                }
            }
        }


        private bool Esclass_Braw_Grabability(Player self, PhysicalObject obj, ref Escort e){
            for (int i = 0; i < 2; i++){
                if (
                    self.grasps[i] != null &&
                    self.grasps[i].grabbed != obj &&
                    self.grasps[i].grabbed is Spear &&
                    obj is Creature c && !c.dead &&
                    c.Stunned){
                    if (c is Lizard l && Esconfig_Dunkin(self) && !e.LizardDunk){
                        if (e.LizGoForWalk == 0){
                            e.LizGoForWalk = 320;
                        }
                        e.LizardDunk = true;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
