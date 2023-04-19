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
        // Escapist tweak values
        // public static readonly PlayerFeature<> escapist = Player("theescort/escapist/");
        // public static readonly PlayerFeature<float> escapist = PlayerFloat("theescort/escapist/");
        // public static readonly PlayerFeature<float[]> escapist = PlayerFloats("theescort/escapist/");
        public static readonly PlayerFeature<float> escapistSlideLaunchMod = PlayerFloat("theescort/escapist/slide_launch_mod");
        public static readonly PlayerFeature<float> escapistSlideLaunchFac = PlayerFloat("theescort/escapist/slide_launch_fac");
        public static readonly PlayerFeature<float> escapistSpearVelFac= PlayerFloat("theescort/escapist/spear_vel_fac");
        public static readonly PlayerFeature<float> escapistSpearDmgFac= PlayerFloat("theescort/escapist/spear_dmg_fac");
        public static readonly PlayerFeature<int[]> escapistNoGrab = PlayerInts("theescort/escapist/no_grab");
        public static readonly PlayerFeature<int> escapistCD = PlayerInt("theescort/escapist/cd");
        public static readonly PlayerFeature<float> escapistColor = PlayerFloat("theescort/escapist/color");

        public void Esclass_EC_Tick(Player self, ref Escort e){
            if (e.EscDangerExtend < 10){
                e.EscDangerExtend++;
                if (self.dangerGraspTime > 3 && self.dangerGraspTime < 29){
                    self.dangerGraspTime--;
                }
            } else {
                if (e.EscUnGraspTime > 4 && e.EscUnGraspTime < e.EscUnGraspLimit){
                    e.EscUnGraspTime++;
                }
                e.EscDangerExtend = 0;
            }

            if (e.EscUnGraspTime > 0 && !self.dead && e.EscUnGraspCD == 0){
                Player.InputPackage iP = RWInput.PlayerInput(self.playerState.playerNumber, self.room.game.rainWorld);
                if (iP.thrw){
                    e.EscUnGraspTime--;
                }
            }

            if (e.EscUnGraspCD > 0 && self.stun < 5){
                e.EscUnGraspCD--;
            }
        }

        private void Esclass_EC_Update(Player self, ref Escort e){
            if (
                !escapistColor.TryGet(self, out float eC) ||
                !escapistCD.TryGet(self, out int esCD) ||
                !escapistNoGrab.TryGet(self, out int[] esNoGrab)
            ){
                return;
            }
            // VFX
            if(self != null && self.room != null){
                // Escapist escape progress VFX
                if (e.EscUnGraspCD == 0 && e.EscUnGraspLimit > 0){
                    Color escapistColor = new Color(0.42f, 0.75f, 0.1f);
                    // Progress fill ring
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 16, 10f, 3f, 20f, Mathf.Lerp(2, 16, 1 - Mathf.InverseLerp(0, e.EscUnGraspLimit, e.EscUnGraspTime)), escapistColor * Mathf.Lerp(0.4f, 1f, 1 - Mathf.InverseLerp(0, e.EscUnGraspLimit, e.EscUnGraspTime)) * eC));

                    // Outer ring
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 12, 25f, 2f, 32f, 2f, escapistColor * eC));

                    // Inner ring
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 8, 10f, 2f, 24f, 2f, escapistColor * eC));
                }
            }

            // Implement Escapist's getaway
            try{
                if (e.EscDangerGrasp == null){
                    e.EscUnGraspLimit = 0;
                    e.EscUnGraspTime = 0;
                }
                else if (e.EscDangerGrasp.discontinued){
                    e.EscDangerGrasp = null;
                }
                else if (e.EscUnGraspTime == 0){
                    Ebug(self, "Attempted to take off grabber", 2);
                    e.EscDangerGrasp.grabber.LoseAllGrasps();
                    e.EscUnGraspLimit = 0;
                    self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Duck_Pop, e.SFXChunk, false, 0.9f, 1.3f);
                    self.cantBeGrabbedCounter = esNoGrab[1];
                    e.EscDangerGrasp = null;
                    e.EscUnGraspCD = esCD;
                }

                if (e.EscUnGraspCD > 0){
                    self.bodyChunks[0].vel.x *= 1f - Mathf.Lerp(0f, 0.22f, Mathf.InverseLerp(0f, 120f, (float)(e.EscUnGraspCD)));
                    self.bodyChunks[1].vel.x *= 1f - Mathf.Lerp(0f, 0.26f, Mathf.InverseLerp(0f, 120f, (float)(e.EscUnGraspCD)));
                    self.Blink(5);
                }
            } catch (Exception err){
                Ebug(self, err, "UPDATE: Escapist's getaway error!");
            }

        }


        private void Esclass_EC_ThrownSpear(Player self, Spear spear){
            if (!escapistSpearVelFac.TryGet(self, out float eSpearVel) ||
                !escapistSpearDmgFac.TryGet(self, out float eSpearDmg)){
                return;
            }
            spear.firstChunk.vel *= eSpearVel;
            spear.spearDamageBonus *= eSpearDmg;
        }


        private void Esclass_EC_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp){
            orig(self, grasp);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if(
                    !eCon.TryGetValue(self, out Escort e)
                    ){
                    return;
                }

                if (e.Escapist){
                    e.EscUnGraspLimit = 120;
                    if (grasp.grabber is Lizard){
                        e.EscUnGraspLimit = 80;
                    }
                    else if (grasp.grabber is Vulture){
                        e.EscUnGraspLimit = 60;
                    }
                    else if (grasp.grabber is BigSpider){
                        e.EscUnGraspLimit = 150;
                    }
                    else if (grasp.grabber is DropBug){
                        e.EscUnGraspLimit = 90;
                    }
                    else if (grasp.grabber is Centipede){
                        e.EscUnGraspLimit = 40;
                    }
                    e.EscDangerGrasp = grasp;
                    e.EscUnGraspTime = e.EscUnGraspLimit;
                }

            } catch (Exception err){
                Ebug(self, err);
                return;
            }

        }
    }
}
