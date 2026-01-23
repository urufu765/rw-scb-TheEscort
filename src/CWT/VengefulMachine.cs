using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;
public class VengefulLizardManager
{
    public List<VengefulLizardTracker> Vengefuls{get;set;}
    public int VengefulHerd{get;set;}
    public int VengefulUpdate{get;set;}
    public string VengefulDifficulty{get;set;}
    public int vengefulHunted;
    public bool Online{get;set;}
    public bool StoryTime{get;set;}
    public string LastRegion {get;set;}
    public int VengefulWait {get;set;}
    public int DeadCount {get;set;}

    public VengefulLizardManager(bool storyMode, bool online, string difficulty)
    {
        Vengefuls = [];
        VengefulDifficulty = difficulty;
        Online = online;
        StoryTime = storyMode;
        VengefulHerd = 0;
        VengefulUpdate = 0;
        vengefulHunted = 0;
        LastRegion = "";
        VengefulWait = 600;
        DeadCount = 0;
    }

    public void UpdateVengeance(AbstractCreature self)
    {
        if (StoryTime && self?.Room?.world?.game?.paused == false)
        {
            VengefulUpdate++;
            if (VengefulWait > 0) VengefulWait--;
            if (VengefulUpdate >= 20) VengefulUpdate = 0;

            if (self?.state?.dead == true)
            {
                DeadCount++;
            }
            else
            {
                DeadCount = 0;
            }

            if (DeadCount > 80)
            {
                for (int i = 0; i < Vengefuls.Count; i++)
                {
                    if (Vengefuls[i].iWantDie && Vengefuls[i]?.Lizor is null)
                    {
                        Vengefuls.RemoveAt(i);
                        continue;
                    }
                    if (Vengefuls[i]?.Lizor?.room is Room r)
                    {
                        Vengefuls[i].DieAnimation(r);
                    }
                    Vengefuls[i].Destroy();
                }
                if (VengefulUpdate == 0 && DeadCount < 120)
                {
                    Ebug($"Vengeful clear due to dead player! {DeadCount}");
                }
                VengefulWait = 400;
            }

            if (self?.Room?.world?.region?.name != LastRegion)
            {
                LastRegion = self.Room.world.region.name;
                foreach(VengefulLizardTracker vlt in Vengefuls)
                {
                    if (vlt?.Lizor?.room is not null)
                    {
                        vlt.DieAnimation(vlt.Lizor.room);    
                    }
                    vlt.Destroy();
                }
                Vengefuls.Clear();
                Ebug($"Vengeful clear due to region change!");
                VengefulWait = 1200;
            }

            if (VengefulWait > 0) return;

            if (VengefulUpdate == 0 && Vengefuls.Count < VengefulHerd && self?.realizedCreature?.room?.game?.world is not null)
            {
                Vengefuls.Add(new(self, self.realizedCreature.room.game, Online));
                Ebug($"Spawned {Vengefuls.Count}/{VengefulHerd} Vengeance Lizors!");
            }
            for (int i = 0; i < Vengefuls.Count; i++)
            {
                if (VengefulUpdate == i % 20)
                {
                    if (Vengefuls[i].iAmReadyToBeAdded)
                    {
                        Vengefuls[i].Spawn();
                    }
                    if (!(Vengefuls[i].iWantToRespawn || Vengefuls[i].iAmReadyToBeAdded) && Vengefuls[i]?.Lizor is not null && Vengefuls[i].Lizor.dead)
                    {
                        Vengefuls[i].KillAndRespawn(VengefulDifficulty != "unfair");
                        // Vengefuls[i].KillAndRespawn();
                        continue;
                    }
                    if (Vengefuls[i].iWantDie)
                    {
                        Vengefuls.RemoveAt(i);
                        VengefulHerd--;
                        continue;
                    }
                    Vengefuls[i]?.Tick(VengefulDifficulty switch
                    {
                        "unfair" => 200,
                        "hard" => 40,
                        "medium" => 20,
                        "easy" => 10,
                        _ => 1
                    });
                    Vengefuls[i]?.UpdateLizor();
                }
            }
        }
    }

    public void TryVengeance(AbstractCreature self, int karma, bool enforced, int karmaCap, int points, int cycles, bool vengefulKill)
    {
        //Ebug($"Venging: {VengefulDifficulty}");
        int maxLizards = Custom.rainWorld.options.quality switch
        {
            var a when a == Options.Quality.LOW => 60,
            var a when a == Options.Quality.MEDIUM => 120,
            var a when a == Options.Quality.HIGH => 240,
            _ => 120
        };
        if (Online)
        {
            maxLizards = 40;
        }
        else
        {
            maxLizards /= self.realizedCreature?.room?.game?.Players?.Count??1;
        }
        if (VengefulHerd >= maxLizards)
        {
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Max vengeful lizards reached! {maxLizards}", LogLevel.WARN);
            }
            else
            {
                Ebug($"Max vengeful lizards reached! {maxLizards}", LogLevel.WARN);
            }
            return;
        }

        if (vengefulKill)
        {
            VengefulHerd++;
            vengefulHunted++;
            if (self.realizedCreature is Player p)
            {
                Ebug(p, $"Killed a vengeful lizard! Count: {VengefulHerd}");
            }
            else
            {
                Ebug($"Killed a vengeful lizard! Count: {VengefulHerd}");
            }

            return;
        }
        if (VengefulDifficulty == "unfair")
        {
            if (VengefulHerd == 0) 
            {
                if (self.realizedCreature?.room is Room r && self.realizedCreature?.bodyChunks is not null)
                {
                    r.PlaySound(SoundID.HUD_Game_Over_Prompt, 0, .6f, .4f);
                }
                VengefulHerd = 8;
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
            if (VengefulHerd == 0) 
            {
                if (self.realizedCreature?.room is Room r && self.realizedCreature?.bodyChunks is not null)
                {
                    r.PlaySound(SoundID.HUD_Game_Over_Prompt, 0, .6f, .4f);
                }
                VengefulHerd = 2;
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

    public bool IsVengeanceLizard(EntityID id)
    {
        return Vengefuls.Any(a => a.Me.ID == id);
    }

    public int VengefulDivision()
    {
        return VengefulDifficulty switch
        {
            "medium" => 4,
            "hard" => 2,
            _ => 1
        };
    }
}


public class VengefulLizardTracker
{
    public AbstractCreature Me {get;set;}
    public AbstractCreature Target {get;set;}
    public Lizard Lizor => Me?.realizedCreature as Lizard;
    public Player Playr => Target?.realizedCreature as Player;
    public AbstractRoom StartingRoom {get;set;}
    public RainWorldGame Game {get; set;}
    public bool iAmReadyToBeAdded;
    public bool iAmStuck;
    public int iAmStuckCount;
    public int checkItOutClock;
    public int teleportClock;
    public int iAmWaitingToRespawn;
    public bool iWantToRespawn;
    public readonly bool iAmOnline;
    public int respawns;
    public bool iWantToTeleport;
    public bool iWantDie;
    public VengefulLizardTracker(AbstractCreature target, RainWorldGame game, bool online)
    {
        iAmReadyToBeAdded = false;
        Game = game;
        WorldCoordinate coord = GetCornerableRoom(target);
        StartingRoom = target.world.GetAbstractRoom(coord.room);
        Target = target;
        Me = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, coord, game.GetNewID());
        Me.saveCreature = false;
        Me.ignoreCycle = true;
        if (ModManager.MSC) Me.voidCreature = true;
        // if (ModManager.Watcher)
        // {
        //     Me.rippleCreature = true;
        //     Me.rippleBothSides = true;
        // }
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceLike(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempLike(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceFear(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempFear(-1);
        iAmStuckCount = 0;
        checkItOutClock = 0;
        iAmWaitingToRespawn = 0;
        iAmStuck = false;
        iAmOnline = online;
        respawns = 0;
        teleportClock = 0;
        iWantToTeleport = false;
        iWantDie = false;
        iAmReadyToBeAdded = true;
        iWantToRespawn = false;
    }

    public void Tick(int by)
    {
        if (iAmStuck)
        {
            iAmStuckCount+=by;
        }
        else if (iAmStuckCount > 0)
        {
            iAmStuckCount-=by;
        }

        if (checkItOutClock > 0)
        {
            checkItOutClock-=by;
        }
        if (teleportClock > 0)
        {
            teleportClock-=by;
        }

        if (iAmWaitingToRespawn > 0)
        {
            iAmWaitingToRespawn-=by;
        }

        if (iAmStuckCount < 0) iAmStuckCount = 0;
        if (checkItOutClock < 0) checkItOutClock = 0;
        if (teleportClock < 0) teleportClock = 0;
        if (iAmWaitingToRespawn < 0) iAmWaitingToRespawn = 0;
    }

    public void UpdateLizor()
    {
        if (iWantDie) return;


        // // Respawn lizard
        if (iWantToRespawn && iAmWaitingToRespawn == 0)
        {
            if (Respawn())
            {
                respawns++;
                iAmWaitingToRespawn = -1;
                iWantToRespawn = false;
                iAmReadyToBeAdded = true;
            }
            else
            {
                iAmWaitingToRespawn = 80;
            }
        }
        if (Me?.abstractAI is null) return;

        // Teleports lizard
        iAmStuck = Me.abstractAI?.path is null || Me.abstractAI?.path?.Count == 0;
        // iWantToTeleport = Me.pos.room != Target.pos.room && Me.abstractAI is not null && Me.abstractAI.RealAI is null;
        if (
            teleportClock == 0 &&
            (
                iAmStuckCount > 120 ||
                // iWantToTeleport ||
                (Me.abstractAI is not null && Me.abstractAI.strandedInRoom != -1) ||
                (Me.abstractAI?.RealAI is not null && Me.abstractAI.RealAI.stranded)
            ) &&
            Me.pos.room != Target.pos.room
        )
        {
            teleportClock = 40;
            if (TeleportMeNearPlayer(Me, Playr))
            {
                iAmStuckCount = 0;
                teleportClock = 200;
            }
        }

        // Prevents lizard from seeking player in shelter/gate
        if (Target.Room.shelter || Target.Room.gate) return;

        // Does destination update every 8 seconds while not in same room, or every second if in same room.
        if (checkItOutClock == 0 || Me.pos.room == Target.pos.room)
        {
            checkItOutClock = 160;
        }
        else return;

        // Track player
        GetMeToPlayerPos(Me, Playr);

        // Be aggressive to player
        BeVeryAngryAtPlayerAndNothingElse(Me, Target);
    }

    /// <summary>
    /// Sets destination of lizard to player
    /// </summary>
    /// <param name="abstractLizard"></param>
    /// <param name="player"></param>
    public static void GetMeToPlayerPos(AbstractCreature abstractLizard, Player player)
    {
        if (player is not null && !player.dead && abstractLizard.abstractAI?.destination.room != player.abstractCreature.pos.room)
        {
            Ebug(player, $"Vengeful lizard destination changed! From: {abstractLizard.abstractAI.destination.ResolveRoomName()} to {player.abstractCreature.pos.ResolveRoomName()}");
            abstractLizard.abstractAI.SetDestination(player.abstractCreature.pos);
        }
    }

    /// <summary>
    /// Sets anger towards player and ignores everything else
    /// </summary>
    /// <param name="abstractLizard"></param>
    /// <param name="abstractPlayer"></param>
    public static void BeVeryAngryAtPlayerAndNothingElse(AbstractCreature abstractLizard, AbstractCreature abstractPlayer)
    {
        try
        {
            if (abstractLizard.abstractAI?.RealAI is LizardAI lizardAI && lizardAI.tracker?.creatures is not null && lizardAI.agressionTracker is not null)
            {
                lizardAI.friendTracker.tamingDifficlty = 99;
                lizardAI.fear = 0;
                lizardAI.usedToVultureMask = 3000;
                lizardAI.tracker.SeeCreature(abstractPlayer);
                foreach (Tracker.CreatureRepresentation tracked in lizardAI.tracker.creatures)
                {
                    if (tracked?.representedCreature is null) continue;
                    if (tracked.representedCreature != abstractPlayer)
                    {
                        lizardAI.tracker.ForgetCreature(tracked.representedCreature);
                    }
                    else
                    {
                        lizardAI.agressionTracker.SetAnger(tracked, 10, 10);
                    }
                }
                if (abstractLizard.pos.room == abstractPlayer.pos.room && abstractLizard.realizedCreature is Lizard lizor && abstractPlayer.realizedCreature is Player p && lizor?.grasps?[0]?.grabbed is Creature c && c != p)
                {
                    lizor.ReleaseGrasp(0);
                    // Make lizard drop everything and chase after player
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to anger!!");
        }
    }

    /// <summary>
    /// Teleports lizard to a nearby room if lost
    /// </summary>
    /// <param name="abstractLizard"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool TeleportMeNearPlayer(AbstractCreature abstractLizard, Player player)
    {
        if (abstractLizard.realizedCreature is not null) return false;
        try
        {
            if (abstractLizard is not null && player?.room is not null)
            {
                IntVector2 exit = player.room.exitAndDenIndex[UnityEngine.Random.Range(0, player.room.exitAndDenIndex.Length)];
                if (player.room.WhichRoomDoesThisExitLeadTo(exit) is AbstractRoom vroom && !vroom.shelter && !vroom.gate)
                {
                    abstractLizard.Move(vroom.RandomNodeInRoom());
                    Ebug(player, $"Vengeful lizard is teleporting to {vroom.name}!");
                    return true;
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to teleport vengeance lizor!");
        }
        return false;
    }

    /// <summary>
    /// Respawns the lizor
    /// </summary>
    /// <returns>Whether it was successful or not</returns>
    public bool Respawn()
    {
        if (Game?.world is not null)
        {
            WorldCoordinate coord = GetCornerableRoom(Target);
            StartingRoom = Target.world.GetAbstractRoom(coord.room);
            Me = new AbstractCreature(Game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, GetCornerableRoom(Target), Game.GetNewID());
            iAmStuckCount = 0;
            Me.saveCreature = false;
            Me.ignoreCycle = true;
            if (ModManager.MSC) Me.voidCreature = true;
            // if (ModManager.Watcher)
            // {
            //     Me.rippleCreature = true;
            //     Me.rippleBothSides = true;
            // }
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceLike(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceTempLike(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceFear(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceTempFear(-1);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets nearby room to spawn in
    /// </summary>
    /// <param name="abstractPlayer"></param>
    /// <returns></returns>
    public static WorldCoordinate GetCornerableRoom(AbstractCreature abstractPlayer)
    {
        List<int> l = [];

        for (int i = 0; i < abstractPlayer.world.NumberOfRooms; i++)
        {
            if (
                abstractPlayer.world.GetAbstractRoom(abstractPlayer.world.firstRoomIndex + i) is AbstractRoom ar && 
                !ar.shelter && 
                !ar.gate && 
                ar.name != abstractPlayer.Room.name &&
                ar.NodesRelevantToCreature(StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard)) > 0)
            {
                l.Add(i);
            }
        }

        if (l.Count > 0)
        {
            return abstractPlayer.world.GetAbstractRoom(abstractPlayer.world.firstRoomIndex + l[UnityEngine.Random.Range(0, l.Count)]).RandomNodeInRoom();
        }
        return abstractPlayer.Room.RandomNodeInRoom();
    }

    /// <summary>
    /// Cleans up mess
    /// </summary>
    public void Destroy()
    {
        iWantDie = true;
        if (Lizor is not null)
        {
            Lizor.AllGraspsLetGoOfThisObject(true);
            if (Lizor.grasps is not null)
            {
                for (int i = 0; i < Lizor.grasps.Length; i++)
                {
                    Lizor.grasps[i]?.grabbed?.AllGraspsLetGoOfThisObject(false);
                }
            }
        }
        Me?.realizedCreature?.Destroy();
    }

    public void DieAnimation(Room room = null)
    {
        if (room is not null && Lizor?.bodyChunks is not null)
        {
            room.PlaySound(SoundID.Puffball_Eplode, Lizor.firstChunk);
            room.AddObject(new ShockWave(Lizor.firstChunk.pos, Mathf.Lerp(25f, 50f, UnityEngine.Random.value), 0.07f, 20));
            for (int i = 0; i < Lizor.bodyChunks.Length; i++)
            {
                room.AddObject(new Explosion.ExplosionSmoke(Lizor.bodyChunks[i].pos, Custom.RNV() * (2f * UnityEngine.Random.value), 1f)
                {
                    colorA = new Color(0.796f, 0.549f, 0.27843f),
                    colorB = Color.black
                });
            }
        }
        // Me.destroyOnAbstraction = true;
        // Me.state?.Die();
        // Me.abstractAI?.Die();
    }

    public void Spawn()
    {
        if (iAmReadyToBeAdded)
        {
            StartingRoom.AddEntity(Me);
            Ebug("I have spawned!! Get ready!", ignoreRepetition: true);
            iAmReadyToBeAdded = false;
        }
    }

    public void KillAndRespawn(bool dontTakeMySpearsLmao = true)
    {
        if (Lizor?.room is not null)
        {
            DieAnimation(Lizor.room);
        }
        if (Lizor is not null)
        {
            Lizor.AllGraspsLetGoOfThisObject(true);
            if (dontTakeMySpearsLmao)
            {
                Me.LoseAllStuckObjects();
            }
            if (Lizor.grasps is not null)
            {
                for (int i = 0; i < Lizor.grasps.Length; i++)
                {
                    if (Lizor.grasps[i]?.grabbed is not null)
                    {
                        Lizor.ReleaseGrasp(i);
                    }
                }
            }
        }

        Lizor?.Destroy();
        if (respawns > 2)
        {
            Ebug("I ran out of respawns! Doh!", ignoreRepetition: true);
            iWantDie = true;
            return;
        }
        iAmWaitingToRespawn = 200 + (int)(600 * UnityEngine.Random.value);
        iWantToRespawn = true;
        Ebug($"I am getting ready to respawn! {iAmWaitingToRespawn}", ignoreRepetition: true);
    }
}