using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using Menu;
using Newtonsoft.Json;
using TheEscort.VengefulLizards;

namespace TheEscort.Deflector;

public static class DF_Damage
{
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
    public static void DamageIncrease(On.PlayerSessionRecord.orig_AddKill orig, PlayerSessionRecord self, Creature victim)
    {
        try
        {
            if (victim.killTag?.realizedCreature is Player p)
            {
                if (vCon.TryGetValue(victim.killTag, out VengefulMachine ven) && p.room?.game?.session is StoryGameSession sg)
                {
                    int points = TryGetKillscorePoints(victim.Template.type);
                    if (points > 0)
                    {
                        if (!ven.IsVengefulEntity(victim.abstractCreature.ID))
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
                            ven.IsVengefulEntity(victim.abstractCreature.ID)
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

    public static void WinLoseSave(ShelterDoor self, int playerNumber, bool success, ref Escort escort)
    {
        if (success && self.room?.game?.session is StoryGameSession storyGameSession)
        {
            float bonusDamage = 0;
            if (storyGameSession.saveState.deathPersistentSaveData.karma == storyGameSession.saveState.deathPersistentSaveData.karmaCap)
            {
                bonusDamage = storyGameSession.saveState.deathPersistentSaveData.karmaCap switch
                {
                    >= 9 => 0.05f,
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


    public static float DamageMultiplier(Player player, ref Escort e, bool spendEmpower = true)
    {
        float multiplier = e.DeflDamageMult + e.DeflPerma;

        if (e.DeflPowah >= 3) DF_FX.UltimateShake(player);
        if (spendEmpower) e.DeflPowah = 0;

        return multiplier;
    }

    public static void MakeHitHurtReallyBad(Weapon self, SharedPhysics.CollisionResult result, Player p, Creature c, ref Escort e, Creature.DamageType type, float dmg = 0.01f, float stun = 0f, bool allowDead = false)
    {
        if (self is Spear or Rock or Bullet or LillyPuck or ScavengerBomb) return;  // Skip already handled cases
        if (e.DeflPowah > 0 && (allowDead || !c.dead))
        {
            float newDmg = dmg * DamageMultiplier(p, ref e);
            c.Violence(self.firstChunk, self.firstChunk.vel * self.firstChunk.mass, result.chunk, result.onAppendagePos, type, newDmg, stun);
        }
    }
}