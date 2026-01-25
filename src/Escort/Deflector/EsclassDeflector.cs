using BepInEx;
using Menu;
using Newtonsoft.Json;
using SlugBase.Features;
using System;
using System.Linq;
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
        public static readonly PlayerFeature<float> deflectorSlideDmg;
        public static readonly PlayerFeature<float> deflectorSlideLaunchFac;
        public static readonly PlayerFeature<float> deflectorSlideLaunchMod;
        public static readonly PlayerFeature<float[]> deflectorDKHypeDmg;
        public static readonly PlayerFeature<float[]> deflectorSpearVelFac;
        public static readonly PlayerFeature<float[]> deflectorSpearDmgFac;

        public static float DeflSharedPerma;
        public static float DeflInitSharedPerma;

        public static void Esclass_DF_Tick(Player self, ref Escort e)
        {
            // Increased damage when parry tick
            if (e.DeflAmpTimer > 0)
            {
                e.DeflAmpTimer--;
            }
            else
            {
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

            if (self.room?.game?.session is ArenaGameSession ags)
            {
                e.DeflPerma = 0.01f * ags.ScoreOfPlayer(self, false);
            }

            if (e.DeflParryCD > 0)
            {
                e.DeflParryCD--;
            }

            bool holding = e.DeflBonusParry > 0 && (e.CustomKeybindEnabled? e.CustomInput[0] : self.input[0].jmp);

            if (e.DeflAerialParry > 0)
            {
                if (e.DeflAerialParry < 10)
                {
                    if (holding)
                    {
                        e.DeflBonusParry--;
                        e.DeflAerialParry++;
                    }
                    else
                    {
                        e.DeflBonusParry = 0;
                    }
                }
                e.DeflAerialParry--;
            }
            else if (e.DeflAerialParry < -1)
            {
                e.DeflAerialParry++;
            }

            if (e.DeflSwimParry > 0)
            {
                if (e.DeflSwimParry < 10)
                {
                    if (holding)
                    {
                        e.DeflBonusParry--;
                        e.DeflSwimParry++;
                    }
                    else
                    {
                        e.DeflBonusParry = 0;
                    }
                }
                e.DeflSwimParry--;
            }
            else if (e.DeflSwimParry == 0)
            {
                e.DeflSwimParry -= Escort.DeflSwimCD - Escort.DeflSwimWindow - e.DeflBonusWindow;
            }
            else if (e.DeflSwimParry < -1)
            {
                if (self.waterJumpDelay > -e.DeflSwimParry)
                {
                    e.DeflSwimParry = -self.waterJumpDelay;
                }
                else
                {
                    e.DeflSwimParry++;
                }
            }            
            
            if (e.DeflCorridorParry > 0)
            {
                e.DeflCorridorParry--;
            }
            else if (e.DeflCorridorParry == 0)
            {
                e.DeflCorridorParry -= Escort.DeflCorridorCD - Escort.DeflCorridorWindow;
            }
            else if (e.DeflCorridorParry < -1)
            {
                if (self.slowMovementStun > -e.DeflCorridorParry)
                {
                    e.DeflCorridorParry = -self.slowMovementStun;
                }
                else
                {
                    e.DeflCorridorParry++;
                }
            }

            if (e.DeflZeroGParry > 0)
            {
                if (e.DeflZeroGParry < 10)
                {
                    if (holding)
                    {
                        e.DeflBonusParry--;
                        e.DeflZeroGParry++;
                    }
                    else
                    {
                        e.DeflBonusParry = 0;
                    }
                }
                e.DeflZeroGParry--;
            }
            else if (e.DeflZeroGParry == 0)
            {
                e.DeflZeroGParry -= Escort.DeflZeroGWait;
            }
            else if (e.DeflZeroGParry < -1)
            {
                e.DeflZeroGParry++;
            }
        }

        public static void Esclass_DF_Update(Player self, ref Escort e)
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
                    self.slugcatStats.throwingSkill = e.DeflPowah > 0 ? 2 : 0;

                    // Empowered damage from parry visual effect
                    Color empoweredColor = new Color(0.69f, 0.55f, 0.9f);
                    //Color empoweredColor = new Color(1f, 0.7f, 0.35f, 0.7f);
                    //empoweredColor.a = 0.7f;
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    if (!ins.config.cfgNoticeEmpower.Value)
                    {
                        if (e.DeflPowah == 1 || e.DeflPowah == 3)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2(e.DeflAmpTimer % 2 == 0 ? 10 : -10, 0), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                        }
                        if (e.DeflPowah == 2)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2(e.DeflAmpTimer % 2 == 0 ? 10 : -10, 3), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2(e.DeflAmpTimer % 2 == 0 ? 10 : -10, -3), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                        }
                        if (e.DeflPowah == 3)
                        {
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2(e.DeflAmpTimer % 2 == 0 ? 10 : -10, 6), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));
                            self.room.AddObject(new Spark(self.bodyChunks[0].pos + new Vector2(e.DeflAmpTimer % 2 == 0 ? 10 : -10, -6), new Vector2(2f * (e.DeflAmpTimer % 2 == 0 ? 1 : -1), 0), empoweredColor, null, 9, 13));

                        }
                    }
                    else
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 15f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        if (e.DeflPowah <= 2)
                        {
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 23f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        }
                        if (e.DeflPowah <= 3)
                        {
                            self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[0].pos, 5, Mathf.Lerp(10f, 31f, Mathf.InverseLerp(0, 20, e.DeflAmpTimer % 20)), 1.5f, 24f, 3.5f, empoweredColor * 1.5f));
                        }
                    }
                }
            }

            //Ebug(self, $"Val {e.DeflAerialParry}");
            if (self.bodyChunks.Any(b => b.ContactPoint.x != 0 || b.ContactPoint.y != 0))
            {
                if (e.DeflAerialParry >= 0)
                {
                    Ebug(self, "AerialParry restored!");
                }
                if (e.DeflAerialWasStanding is not null)
                {
                    self.standing = e.DeflAerialWasStanding.Value;
                }
                e.DeflAerialWasStanding = null;
                e.DeflAerialParry = -Escort.DeflAerialWait;
            }

        }
    }
}
