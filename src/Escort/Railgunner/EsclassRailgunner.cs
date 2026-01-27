using BepInEx;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using TheEscort.Railgunner;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort;

partial class Plugin : BaseUnityPlugin
{
    // Railgunner tweak values
    // public static readonly PlayerFeature<> railgun = Player("theescort/railgunner/");
    // public static readonly PlayerFeature<float> railgun = PlayerFloat("theescort/railgunner/");
    // public static readonly PlayerFeature<float[]> railgun = PlayerFloats("theescort/railgunner/");

    public static void Esclass_RG_Tick(Player self, ref Escort e)
    {
        if (e.RailWeaping > 0)
        {
            e.RailWeaping--;
        }

        if (e.RailGaussed > 0)
        {
            e.RailGaussed--;
        }

        if (e.RailIFrame > 0)
        {
            e.RailIFrame--;
        }
        else
        {
            e.RailBombJump = false;
        }

        if (e.RailgunCD > 0)
        {
            if (self.bodyMode != Player.BodyModeIndex.Stunned)
            {
                e.RailgunCD--;
            }
        }
        else
        {
            e.RailgunUse = 0;
        }

        if (e.RailRecoilLag > 0)  // Recoil lag
        {
            e.RailRecoilLag--;
        }

        if (e.RailLaserBlinkClock > 15)
        {
            e.RailLaserBlinkClock = 0;
            e.RailLaserBlink = !e.RailLaserBlink;
        }
        else
        {
            e.RailLaserBlinkClock++;
        }

        // 1 second delay per check
        if (e.RailTargetClock > 0)
        {
            e.RailTargetClock--;
        }

        if (e.RailLaserDimmer < Escort.RailLaserDimmerDuration)
        {
            e.RailLaserDimmer++;
        }

        if (e.RailSparkling > 0)
        {
            e.RailSparkling--;
        }

        if (e.RailLastReset > 0)
        {
            e.RailLastReset--;
        }
        else if (e.RailLastReset == 0)
        {
            e.Escat_RG_ResetSpearValues();
            e.RailLastReset = -1;
        }
    }

    public static void Esclass_RG_Update(Player self, ref Escort e)
    {
        // VFX
        if (self != null && self.room != null)
        {
            // Railgunner cooldown timer
            if (e.RailgunCD > 0 && e.RailSparkling == 0)
            {
                Color railgunColor = new(0.5f, 0.85f, 0.78f);
                Color railgunBrightColor = new(0.55f, 0.95f, 0.85f);
                float r = UnityEngine.Random.Range(-1, 1);
                // TODO: reduce amount of sparks
                for (int i = 0; i < (int)Custom.LerpMap(e.RailgunUse, 0, e.RailgunLimit, 1, 4); i++)
                {
                    Vector2 v = Custom.RNV() * (r == 0 ? 0.1f : r);
                    self.room.AddObject(
                        new Spark(
                            self.mainBodyChunk.pos + 15f * v.normalized,
                            v,
                            Color.Lerp(
                                railgunColor,
                                e.RailgunUse >= e.RailgunLimit ? Color.white : railgunBrightColor,
                                Mathf.InverseLerp(0, 3, i)
                            ),
                            null,
                            e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 8 : 6,
                            e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 16 : 12
                        )
                    );

                }
                e.RailSparkling = (int)Mathf.Lerp(2, 10, UnityEngine.Random.value);
                /*
                self.room.AddObject(new Explosion.FlashingSmoke(self.bodyChunks[0].pos, self.mainBodyChunk.vel + new Vector2(0, 1), 1f, Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)), Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 6)));
                */
                /*
                self.room.AddObject(new Explosion.ExplosionSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * 1.5f * UnityEngine.Random.value, 1f){
                    colorA = Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)),
                    colorB = Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD))
                });*/
                //Smoke.FireSmoke s = new Smoke.FireSmoke(self.room);
                //self.room.AddObject(s);
                //s.EmitSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 5));
            }

        }

        // Do recoil
        if (e.RailRecoilLag == 0)
        {
            e.RailRecoilLag = -1;
            // 0.7f, 1.5f, 0.4f, 0.75f, 1.5f
            RG_Fx.Recoil(self, e.RailLastThrowDir, 50, e.RailFrail);
        }


        // Creature check every 1 second
        // if (e.RailTargetClock == 0)
        // {
        //     e.RailTargetAcquired = Esclass_RG_Spotter(self);
        // }


        // Auto-escape out of danger grasp if overcharged
        if (self.dangerGraspTime == 29 && (!e.RailFrail || UnityEngine.Random.value <= ((float)e.RailgunUse / e.RailgunLimit)))
        {
            self.dangerGrasp.grabber.LoseAllGrasps();
            self.cantBeGrabbedCounter = 40;
            if (!e.RailFrail)
            {
                e.Escat_RG_SetGlassMode(true);
                RG_Fx.InnerSplosion(self, 40 * e.RailgunUse);
                RG_Shocker.StunWave(self, 30 * e.RailgunUse, 0.4f, 120);
                e.RailgunUse = e.RailgunCD = 0;
            }
            else
            {
                RG_Shocker.StunWave(self, 40 * e.RailgunUse, 0.6f, 120);
                RG_Fx.InnerSplosion(self, 50 * e.RailgunUse, true);
                e.RailgunUse = e.RailgunCD = 0;
            }
        }

        if (ModManager.MSC && e.RailZap.Count > 0 && Custom.rainWorld.options.quality != Options.Quality.LOW)
        {
            float railRatio = (float)e.RailgunUse / e.RailgunLimit;
            for (int i = 0; i < e.RailZap.Count; i++)
            {
                e.RailZap[i].Update(self, railRatio);
            }
            e.RailZap.RemoveAll(a => a.IsDead());
        }
    }
}
