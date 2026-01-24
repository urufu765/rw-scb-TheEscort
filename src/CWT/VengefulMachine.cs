using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;

public class VengefulMachine
{
    public List<VengefulCreatureManager> Vengefuls { get; set; }
    public int VengefulHerd { get; set; }
    public int VengefulHerdBase { get; }
    public int VengefulUpdate { get; set; }
    public int pauseUpdate;
    public int VengefulSpawn { get; set; }
    public string VengefulDifficulty { get; set; }
    public int vengefulHunted;
    public bool Online { get; set; }
    public bool StoryTime { get; set; }
    public string LastRegion { get; set; }
    public int VengefulWait { get; set; }
    public int DeadCount { get; set; }
    public bool SurpriseMe {get;set;}
    public bool TrySaved {get;set;}

    public VengefulMachine(bool storyMode, bool online, string difficulty, int cycleCount)
    {
        Vengefuls = [];
        VengefulDifficulty = difficulty;
        Online = online;
        StoryTime = storyMode;
        VengefulHerdBase = VengefulHerd = (difficulty, cycleCount) switch
        {
            (string dif, int cyc) when dif is "hard" => Mathf.FloorToInt(cyc / 100f),
            ("medium", > 99) => UnityEngine.Random.Range(1, 4),
            _ => 0
        };
        VengefulUpdate = 0;
        vengefulHunted = 0;
        LastRegion = "";
        VengefulWait = 1200;
        DeadCount = 0;
        SurpriseMe = false;
        TrySaved = false;
        pauseUpdate = 0;
    }

    public void UpdateVengeance(AbstractCreature self)
    {
        if (StoryTime && self?.Room?.world?.game?.paused == false)
        {
            if (Vengefuls is not null && VengefulDifficulty is "hard" or "unfair")
            {
                foreach(VengefulCyan cyan in Vengefuls.OfType<VengefulCyan>())
                {
                    if (cyan.RealMe is Lizard lizard)
                    {
                        lizard.biteDelay = 0;
                        lizard.stun = 0;
                    }
                }
            }
            VengefulUpdate++;
            if (pauseUpdate > 0) pauseUpdate--;
            if (VengefulWait > 0) VengefulWait--;
            if (VengefulUpdate >= VengeSetTack) VengefulUpdate = 0;

            if (self?.state?.dead == true)
            {
                DeadCount++;
            }
            else
            {
                DeadCount = 0;
            }

            // Make them disappear if target dead
            if (DeadCount > 80)
            {
                for (int i = 0; i < Vengefuls.Count; i++)
                {
                    if (Vengefuls[i].iWantDie && Vengefuls[i]?.RealMe is null)
                    {
                        Vengefuls.RemoveAt(i);
                        continue;
                    }
                    if (Vengefuls[i]?.RealMe?.room is Room r)
                    {
                        Vengefuls[i].DieAnimation(r);
                    }
                    Vengefuls[i].Destroy();
                }
                if (VengefulUpdate == 0 && DeadCount < 120)
                {
                    Ebug($"Vengeful clear due to dead player! {DeadCount}");
                }
                pauseUpdate = VengefulWait = 400;
            }

            // Make them disappear if target is in an oracle room
            if (VengefulUpdate == 0 && self?.realizedCreature?.room?.physicalObjects is not null && self.realizedCreature.room.physicalObjects.SelectMany(a => a).OfType<Oracle>().Any())
            {
                for (int i = 0; i < Vengefuls.Count; i++)
                {
                    if (Vengefuls[i].iWantDie && Vengefuls[i]?.RealMe is null)
                    {
                        Vengefuls.RemoveAt(i);
                        continue;
                    }
                    if (Vengefuls[i]?.RealMe?.room is Room r)
                    {
                        Vengefuls[i].DieAnimation(r);
                    }
                    Vengefuls[i].Destroy();
                }
                pauseUpdate = 400;
            }

            // Make them untracked if target switched regions
            if (self?.Room?.world?.region?.name != LastRegion)
            {
                LastRegion = self.Room.world.region.name;
                foreach (VengefulCreatureManager vcm in Vengefuls)
                {
                    if (vcm?.RealMe?.room is not null)
                    {
                        vcm.DieAnimation(vcm.RealMe.room);
                    }
                    vcm.Destroy();
                }
                Vengefuls.Clear();
                Ebug($"Vengeful clear due to region change!");
                pauseUpdate = 400;
                VengefulWait = 1200;
            }

            if (VengefulWait == 0 && VengefulUpdate == 0 && Vengefuls.Count < VengefulHerd && self?.realizedCreature?.room?.game?.world is not null)
            {
                if (SurpriseMe)
                {
                    Vengefuls.Add(UnityEngine.Random.value switch
                    {
                        float x when x < .3f && ModManager.MSC => new VengefulTrain(self, self.realizedCreature.room.game, Online),
                        < .8f => new VengefulRed(self, self.realizedCreature.room.game, Online),
                        _ => new VengefulRedipede(self, self.realizedCreature.room.game, Online)
                    });
                    SurpriseMe = false;
                }
                else
                {
                    // Vengefuls.Add(new VengefulCyan(self, self.realizedCreature.room.game, Online));
                    Vengefuls.Add(new VengefulCyan(self, self.realizedCreature.room.game, Online));
                }
                VengefulWait = VengeSetNextSpawn;
                Ebug($"Spawned {Vengefuls.Count}/{VengefulHerd} Vengeance creature!");
            }

            if (pauseUpdate > 0) return;

            for (int i = VengefulUpdate; i < Vengefuls.Count; i += VengeSetTack)
            {
                if (Vengefuls[i].iAmReadyToBeAdded)
                {
                    Vengefuls[i].Spawn();
                }
                if (!(Vengefuls[i].iWantToRespawn || Vengefuls[i].iAmReadyToBeAdded) && Vengefuls[i]?.RealMe is not null && Vengefuls[i].RealMe.dead)
                {
                    Vengefuls[i].KillAndRespawn(VengeSetRespawn, VengefulDifficulty != "unfair");
                    continue;
                }
                if (Vengefuls[i].iWantDie)
                {
                    Vengefuls.RemoveAt(i);
                    VengefulHerd--;
                    continue;
                }
                Vengefuls[i]?.Tick(VengeSetAdvance);
                Vengefuls[i]?.Update();
                if (VengefulDifficulty is "hard" or "unfair")
                {
                    Vengefuls[i]?.GetSomeHealing(.005f);
                    if (Vengefuls[i]?.RealMe is Creature c)
                    {
                        c.stun = 0;
                    }
                }
            }
        }
    }

    public void TryVengeance(AbstractCreature self, int karma, bool enforced, int karmaCap, int points, int cycles, bool vengefulKill)
    {
        //Ebug($"Venging: {VengefulDifficulty}");
        int maxVengeful = Custom.rainWorld.options.quality switch
        {
            var a when a == Options.Quality.LOW => 60,
            var a when a == Options.Quality.MEDIUM => 120,
            var a when a == Options.Quality.HIGH => 240,
            _ => 120
        };
        if (Online)
        {
            maxVengeful = 40;
        }
        else
        {
            maxVengeful /= self.realizedCreature?.room?.game?.Players?.Count ?? 1;
        }
        if (VengefulHerd >= maxVengeful)
        {
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Max vengeful creatures reached! {maxVengeful}", LogLevel.WARN);
            }
            else
            {
                Ebug($"Max vengeful creatures reached! {maxVengeful}", LogLevel.WARN);
            }
            return;
        }

        if (vengefulKill)
        {
            VengefulHerd++;
            vengefulHunted++;
            if (vengefulHunted > UnityEngine.Random.Range(3, 100))
            {
                SurpriseMe = true;
                if (self.realizedCreature?.room is Room r && self.realizedCreature?.bodyChunks is not null)
                {
                    r.PlaySound(SoundID.MENU_Endgame_Meter_Fullfilled, 0, 1.7f, .5f);
                }
            }
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Killed a vengeful creature! Count: {VengefulHerd}");
            }
            else
            {
                Ebug($"Killed a vengeful creature! Count: {VengefulHerd}");
            }

            return;
        }
        if (VengefulDifficulty == "unfair")
        {
            if (VengefulHerd <= VengefulHerdBase) 
            {
                if (self.realizedCreature?.room is Room r && self.realizedCreature?.bodyChunks is not null)
                {
                    r.PlaySound(SoundID.MENU_Endgame_Notch_Meter_Start_Animation, 0, 1.1f, .6f);
                }
                VengefulHerd = VengeSetStart;
            }
            VengefulHerd++;
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Unfair Vengeful! Count: {VengefulHerd}");
            }
            else
            {
                Ebug($"Unfair Vengeful! Count: {VengefulHerd}");
            }
            return;
        }
        float probability = 0;
        switch (VengefulDifficulty)
        {
            case "hard":
                probability += vengefulHunted / 100f;
                probability += karmaCap / 100f;
                goto case "medium";
            case "medium":
                probability += Mathf.Lerp(0, .3f, points / 100f);
                goto case "easy";
            case "easy":
                probability += karma / 100f;
                if (enforced) probability += .01f;
                probability = Custom.LerpMap(cycles, 0, 100, 0, probability);
                break;
        }
        if (UnityEngine.Random.value < probability)
        {
            if (VengefulHerd <= VengefulHerdBase)             
            {
                if (self.realizedCreature?.room is Room r && self.realizedCreature?.bodyChunks is not null)
                {
                    r.PlaySound(SoundID.MENU_Endgame_Notch_Meter_Start_Animation, 0, 1.1f, .6f);
                }
                VengefulHerd = VengeSetStart;
            }

            VengefulHerd++;
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Vengeful activated! Count: {VengefulHerd}");
            }
            else
            {
                Ebug($"Vengeful activated! Count: {VengefulHerd}");
            }
        }
    }

    /// <summary>
    /// Checks whether creature is a vengeful entity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsVengefulEntity(EntityID id)
    {
        return Vengefuls.Any(a => a.Me.ID == id);
    }

    /// <summary>
    /// How much to divide the creature score by
    /// </summary>
    public int VengeSetDivision => VengefulDifficulty switch
    {
        "medium" => 4,
        "hard" => 2,
        _ => 1
    };

    /// <summary>
    /// How many ticks to advance the action script by
    /// </summary>
    public int VengeSetAdvance => VengefulDifficulty switch
    {
        "unfair" => 200,
        "hard" or "medium" => 40,
        "easy" => 10,
        _ => 1
    };

    /// <summary>
    /// How many lizards to add to hoard if value is at base
    /// </summary>
    public int VengeSetStart => VengefulDifficulty switch
    {
        "unfair" => 7,
        "hard" => 3,
        "medium" => 2,
        _ => 0
    };

    /// <summary>
    /// How many ticks before Vengeful gets to update
    /// </summary>
    public int VengeSetTack => VengefulDifficulty switch
    {
        "unfair" => 2,
        "hard" => 20,
        _ => 40
    };

    /// <summary>
    /// Respawn timer for vengeful creature. Uses (Advance/Tack)tps instead of RW 40tps
    /// </summary>
    public int VengeSetRespawn => VengefulDifficulty switch
    {
        "hard" => UnityEngine.Random.Range(320, 801),// 80 per second hard
        "medium" => UnityEngine.Random.Range(200, 801),  // 40 per second medium
        "easy" => UnityEngine.Random.Range(300, 601), // 10 per second easy
        _ => 400
    };

    /// <summary>
    /// How far apart to separate each spawn so they don't spawn all at once
    /// </summary>
    public int VengeSetNextSpawn => VengefulDifficulty switch
    {
        "hard" => 120,
        "medium" => 400,
        "easy" => 1200,
        _ => 0
    };
}

