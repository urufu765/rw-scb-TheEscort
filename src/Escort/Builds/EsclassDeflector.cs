using BepInEx;
using Menu;
using Newtonsoft.Json;
using SlugBase.Features;
using System;
using TheEscort.VengefulLizards;
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

            if (self.room?.game?.session is ArenaGameSession ags)
            {
                e.DeflPerma = 0.01f * ags.ScoreOfPlayer(self, false);
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
                    self.slugcatStats.throwingSkill = e.DeflPowah > 0? 1 : 0;

                    // Empowered damage from parry visual effect
                    Color empoweredColor = new Color(0.69f, 0.55f, 0.9f);
                    //Color empoweredColor = new Color(1f, 0.7f, 0.35f, 0.7f);
                    //empoweredColor.a = 0.7f;
                    //self.room.AddObject(new MoreSlugcats.VoidParticle(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value, 5f));
                    if (!ins.config.cfgNoticeEmpower.Value)
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

        public static bool Esclass_DF_StickySpear(Player self)
        {
            return !(
                self.animation == Player.AnimationIndex.BellySlide ||
                self.animation == Player.AnimationIndex.Roll ||
                self.animation == Player.AnimationIndex.Flip
            );
        }

        public static void Esclass_DF_UpdateAnimation(Player self, ref Escort e)
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
                if (ins.config.cfgFunnyDeflSlide.Value)
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

    public static int TryGetKillscorePoints(CreatureTemplate.Type type)
    {
        int points = StoryGameStatisticsScreen.GetNonSandboxKillscore(type);
        if (points <= 0)
        {
            if (themCreatureScores is null) Expedition.ChallengeTools.GenerateCreatureScores(ref themCreatureScores);
            if (themCreatureScores.TryGetValue(type.value, out var score))
            {
                points = score;
            }
        }
        return points;
    }
    public static void Esclass_DF_DamageIncrease(On.PlayerSessionRecord.orig_AddKill orig, PlayerSessionRecord self, Creature victim)
    {
        try
        {
            if (victim.killTag?.realizedCreature is Player p)
            {
                if (vCon.TryGetValue(victim.killTag, out VengefulLizardManager ven) && p.room?.game?.session is StoryGameSession sg)
                {
                    int points = TryGetKillscorePoints(victim.Template.type);
                    if (points > 0)
                    {
                        if (!ven.IsVengeanceLizard(victim.abstractCreature.ID))
                        {
                            sg.saveState.miscWorldSaveData.Esave().VengeancePoints += points;
                        }
                        ven.TryVengeance(
                            victim.killTag,
                            sg.saveState.deathPersistentSaveData.karma,
                            sg.saveState.deathPersistentSaveData.reinforcedKarma,
                            sg.saveState.deathPersistentSaveData.karmaCap,
                            sg.saveState.miscWorldSaveData.Esave().VengeancePoints,
                            sg.saveState.cycleNumber,
                            ven.IsVengeanceLizard(victim.abstractCreature.ID)
                        );
                    }
                }
                if (eCon.TryGetValue(p, out Escort escort))
                {
                    if (p.room?.game?.session is StoryGameSession sgs)
                    {
                        if (escort.Deflector)
                        {
                            escort.DeflPerma += TryGetKillscorePoints(victim.Template.type) * 0.001f;
                            if (p.room?.abstractRoom is not null && p.room.abstractRoom.shelter)
                            {
                                escort.shelterSaveComplete = 0;
                            }
                        }
                        // Challenge mode stuff
                        if (SChallengeMachine.SC03_Active && victim.killTag?.realizedCreature is Player && victim.abstractCreature?.abstractAI is not null && victim is Scavenger s)
                        {
                            Ebug("Scav kill!");
                            if (
                                escort.challenge03InView == s)
                            {
                                victim.room.SC03_Achieve(true);
                            }
                            if (s.Elite)
                            {
                                sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaKills++;
                            }
                            else
                            {
                                sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavKills++;
                            }
                            Ebug("Elites:" + sgs.saveState.miscWorldSaveData.Esave().ESC03_EScaKills);
                            Ebug("Normal:" + sgs.saveState.miscWorldSaveData.Esave().ESC03_ScavKills);
                            victim.room.SC03_Achieve(false);
                        }
                    }
                    // else if (p.room?.game?.session is ArenaGameSession arenaGameSession)
                    // {
                    //     int i = MultiplayerUnlocks.SandboxUnlockForSymbolData(CreatureSymbol.SymbolDataFromCreature(victim.abstractCreature)).index;
                    //     if (i >= 0)
                    //     {
                    //         escort.DeflPerma += arenaGameSession.arenaSitting.gameTypeSetup.killScores[i] * 0.01f;
                    //     }
                    // }
                    Ebug(p, "Damage: " + escort.DeflPerma, ignoreRepetition: true);
                }
            }
            orig(self, victim);
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Permadamage increase failed due to null!");
            orig(self, victim);
        }
        catch (Exception err)
        {
            Ebug(err, "Permadamage increase failed due to generic error!");
            orig(self, victim);
        }
    }

        public static void Esclass_DF_WinLoseSave(ShelterDoor self, int playerNumber, bool success, ref Escort escort)
        {
            if (success && self.room?.game?.session is StoryGameSession storyGameSession)
            {
                float bonusDamage = 0;
                if (storyGameSession.saveState.deathPersistentSaveData.karma == storyGameSession.saveState.deathPersistentSaveData.karmaCap)
                {
                    bonusDamage = storyGameSession.saveState.deathPersistentSaveData.karmaCap switch 
                    {
                        9 => 0.05f,
                        8 => 0.03f,
                        7 => 0.02f,
                        6 => 0.01f,
                        4 => 0.004f,
                        3 => 0.002f,
                        _ => 0
                    };
                }
                storyGameSession.saveState.miscWorldSaveData.Esave().DeflPermaDamage[playerNumber] = escort.DeflPerma + bonusDamage;
                if (escort.shelterSaveComplete <= 2)
                {
                    Ebug("Misc: " + JsonConvert.SerializeObject(storyGameSession.saveState.miscWorldSaveData.Esave()), ignoreRepetition: true);
                }

            }
        }

        public static void Esclass_DF_UltimatePower(Player self)
        {
            try
            {
                self.room?.ScreenMovement(null, default, 1.2f);
            }
            catch (Exception e)
            {
                Ebug(self, e, "Couldn't cause the screen to shake!");
            }
        }
    }
}
