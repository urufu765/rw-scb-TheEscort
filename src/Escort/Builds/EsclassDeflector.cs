using BepInEx;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Deflector tweak values
        // public static readonly PlayerFeature<> deflector = Player("theescort/deflector/");
        // public static readonly PlayerFeature<float> deflector = PlayerFloat("theescort/deflector/");
        // public static readonly PlayerFeature<float[]> deflector = PlayerFloats("theescort/deflector/");
        public static readonly PlayerFeature<float> deflectorSlideDmg = PlayerFloat("theescort/deflector/slide_dmg");
        public static readonly PlayerFeature<float> deflectorSlideLaunchFac = PlayerFloat("theescort/deflector/slide_launch_fac");
        public static readonly PlayerFeature<float> deflectorSlideLaunchMod = PlayerFloat("theescort/deflector/slide_launch_mod");
        public static readonly PlayerFeature<float[]> deflectorDKHypeDmg = PlayerFloats("theescort/deflector/dk_h_dmg");
        public static readonly PlayerFeature<float[]> deflectorSpearVelFac = PlayerFloats("theescort/deflector/spear_vel_fac");
        public static readonly PlayerFeature<float[]> deflectorSpearDmgFac = PlayerFloats("theescort/deflector/spear_dmg_fac");


        public void Esclass_DF_Tick(Player self, ref Escort e)
        {
            // Increased damage when parry tick
            if (e.DeflAmpTimer > 0)
            {
                e.DeflAmpTimer--;
            }
            else {
                e.DeflPowah = 0;
            }

            // Sound FX cooldown
            if (e.DeflSFXcd > 0)
            {
                e.DeflSFXcd--;
            }

            if (self.rollCounter > 1)
            {
                e.DeflSlideCom++;
            }
            else
            {
                e.DeflSlideCom = 0;
            }
        }

        private void Esclass_DF_Update(Player self, ref Escort e)
        {
            // VFX
            if (self != null && self.room != null)
            {
                if (e.DeflAmpTimer > 0)
                {
                    self.slugcatStats.runspeedFac = e.DeflPowah switch
                    {
                        3 => 1.4f,
                        2 => 1.3f,
                        _ => 1.2f,
                    };
                    self.slugcatStats.throwingSkill = e.DeflPowah > 0? 1 : 0;

                    // Empowered damage from parry visual effect
                    Color empoweredColor = new Color(0.69f, 0.55f, 0.9f);
                    //Color empoweredColor = new Color(1f, 0.7f, 0.35f, 0.7f);
                    //empoweredColor.a = 0.7f;
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    if (!config.cfgNoticeEmpower.Value)
                    {
                        if (e.DeflPowah == 1 || e.DeflPowah == 3){
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer % 2 == 0 ? 10 : -10), 0), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                        }
                        if (e.DeflPowah == 2){
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer % 2 == 0 ? 10 : -10), 3), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer % 2 == 0 ? 10 : -10), -3), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                        }
                        if (e.DeflPowah == 3){
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer % 2 == 0 ? 10 : -10), 6), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2((e.DeflAmpTimer % 2 == 0 ? 10 : -10), -6), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));

                        }
                    }
                    else
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 15f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        if (e.DeflPowah <= 2){
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 23f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        }
                        if (e.DeflPowah <= 3){
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 31f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        }
                    }
                }
            }
        }

        private bool Esclass_DF_StickySpear(Player self)
        {
            return !(
                self.animation == Player.AnimationIndex.BellySlide ||
                self.animation == Player.AnimationIndex.Roll ||
                self.animation == Player.AnimationIndex.Flip
            );
        }

        private void Esclass_DF_UpdateAnimation(Player self, ref Escort e)
        {
            if (self.animation == Player.AnimationIndex.BellySlide)
            {
                e.DeflSlideKick = true;
                if (self.rollCounter < 8)
                {
                    self.rollCounter += 9;
                }
                if (self.initSlideCounter < 3)
                {
                    self.initSlideCounter += 3;
                }
                int da = 32;
                int db = 18;
                if (config.cfgFunnyDeflSlide.Value)
                {
                    da = 46;
                    db = 22;
                }
                if (e.DeflSlideCom < (self.longBellySlide ? da : db) && self.rollCounter > 12 && self.rollCounter < 15)
                {
                    self.rollCounter--;
                    //self.exitBellySlideCounter--;
                }
                self.mainBodyChunk.vel.x *= Mathf.Lerp(1.1f, (self.longBellySlide ? 1.33f : 1.3f), Mathf.InverseLerp(0, (self.longBellySlide ? 20 : 10), e.DeflSlideCom));
            }
            else if (e.DeflSlideKick && self.animation == Player.AnimationIndex.RocketJump)
            {
                self.mainBodyChunk.vel.x *= 1.4f;
                self.mainBodyChunk.vel.y *= 0.9f;
                e.DeflSlideKick = false;
            }
            else
            {
                e.DeflSlideKick = false;
            }

        }

        private void Esclass_DF_ThrownSpear(Player self, Spear spear, ref Escort e)
        {
            if (
                !deflectorSpearVelFac.TryGet(self, out float[] dSpearVel) ||
                !deflectorSpearDmgFac.TryGet(self, out float[] dSpearDmg)
            )
            {
                return;
            }
            try
            {
                if (e.DeflAmpTimer > 0)
                {
                    spear.spearDamageBonus *= dSpearDmg[0];
                    if (e.DeflPowah == 2) spear.spearDamageBonus += 1.5f;
                    if (e.DeflPowah == 3) {
                        spear.spearDamageBonus = 1000000f;
                        self.room?.ScreenMovement(null, default, 1.2f);
                    }
                    spear.firstChunk.vel *= dSpearVel[0];
                    e.DeflPowah = 0;
                    e.DeflAmpTimer = 0;
                }
                else
                {
                    e.DeflPowah = 0;
                    spear.spearDamageBonus = dSpearDmg[1];
                    spear.firstChunk.vel *= dSpearVel[1];
                }
            }
            catch (Exception err)
            {
                Ebug(self, err, "Error while applying Deflector-specific speartoss");
            }
        }
    }
}
