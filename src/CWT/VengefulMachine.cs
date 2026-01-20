using System;
using System.Collections.Generic;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;


public class VengefulLizard : UpdatableAndDeletable
{
    public AbstractCreature Me {get;set;}
    public AbstractCreature Target {get;set;}
    public Lizard Lizor => Me?.realizedCreature as Lizard;
    public Player Playr => Target?.realizedCreature as Player;
    public bool iAmStuck;
    public int iAmStuckCount;
    public int checkItOutClock;
    public int teleportClock;
    public int iAmWaitingToRespawn;
    public readonly bool iAmOnline;
    public int playerDead;
    public int respawns;
    public bool iWantToTeleport;
    public VengefulLizard(AbstractCreature target, Room room, bool online)
    {
        Target = target;
        this.room = room;
        Me = new AbstractCreature(room.game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, GetCornerableRoom(target), room.game.GetNewID());
        Me.saveCreature = false;
        Me.ignoreCycle = true;
        if (ModManager.MSC) Me.voidCreature = true;
        if (ModManager.Watcher)
        {
            Me.rippleCreature = true;
            Me.rippleBothSides = true;
        }
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceLike(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempLike(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceFear(-1);
        Me.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempFear(-1);
        room.abstractRoom.AddEntity(Me);
        iAmStuckCount = 1000;
        checkItOutClock = 0;
        iAmWaitingToRespawn = -1;
        iAmStuck = false;
        iAmOnline = online;
        respawns = 0;
        teleportClock = 0;
        playerDead = 0;
        iWantToTeleport = false;
    }

    public void Tick()
    {
        if (iAmStuck)
        {
            iAmStuckCount++;
        }
        else if (iAmStuckCount > 0)
        {
            iAmStuckCount--;
        }

        if (checkItOutClock > 0)
        {
            checkItOutClock--;
        }
        if (teleportClock > 0)
        {
            teleportClock--;
        }
    }

    public void UpdateLizor()
    {
        if (slatedForDeletetion) return;

        Tick();
        // if (Playr is null || Playr.dead)
        // {
        //     playerDead++;
        // }
        // else if (playerDead > 0)
        // {
        //     playerDead--;
        // }

        // if (playerDead > 20)
        // {
        //     Ebug("Player dead, show's over!");
        //     Die(Lizor?.room);
        //     //Destroy();
        //     return;
        // }

        // // Respawn lizard
        // if (respawns < 3 && (Lizor?.slatedForDeletetion == true || Lizor?.dead == true) && iAmWaitingToRespawn == -1)
        // {
        //     Die(Lizor?.room);
        //     iAmWaitingToRespawn = iAmOnline? 10 : 20;
        // }
        // if (iAmWaitingToRespawn > 0)
        // {
        //     iAmWaitingToRespawn--;
        //     return;
        // }
        // else if (iAmWaitingToRespawn == 0)
        // {
        //     if (Respawn())
        //     {
        //         respawns++;
        //         iAmWaitingToRespawn = -1;
        //     }
        //     else
        //     {
        //         iAmWaitingToRespawn = 3;
        //     }
        // }

        // Teleports lizard
        iAmStuck = Me.abstractAI?.path is null || Me.abstractAI?.path?.Count == 0;
        // iWantToTeleport = Me.pos.room != Target.pos.room && Me.abstractAI is not null && Me.abstractAI.RealAI is null;
        if (
            teleportClock == 0 &&
            (
                iAmStuckCount > (iAmOnline? 5 : 10) ||
                // iWantToTeleport ||
                (Me.abstractAI is not null && Me.abstractAI.strandedInRoom != -1) ||
                (Me.abstractAI?.RealAI is not null && Me.abstractAI.RealAI.stranded)
            ) &&
            Me.pos.room != Target.pos.room
        )
        {
            teleportClock = iAmOnline? 2 : 1;
            if (TeleportMeNearPlayer(Me, Playr))
            {
                iAmStuckCount = 0;
                teleportClock = 10;
            }
        }

        // Prevents lizard from seeking player in shelter/gate
        if (Target.Room.shelter || Target.Room.gate) return;

        // Does destination update every 8 seconds while not in same room, or every second if in same room. If online, update every 3 seconds instead of every second
        if (checkItOutClock == 0 || Me.pos.room == Target.pos.room)
        {
            if (iAmOnline && checkItOutClock > 7) return;
            checkItOutClock = 10;
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
        if (Playr?.room?.game?.world is not null)
        {
            if (Me?.realizedCreature is not null)
            {
                Me.destroyOnAbstraction = true;
                Me.Die();
            }
            Me = new AbstractCreature(Playr.room.game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, GetCornerableRoom(Target), Playr.room.game.GetNewID());
            iAmStuckCount = 1000;
            Me.saveCreature = false;
            Me.ignoreCycle = true;
            if (ModManager.MSC) Me.voidCreature = true;
            if (ModManager.Watcher)
            {
                Me.rippleCreature = true;
                Me.rippleBothSides = true;
            }
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceLike(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceTempLike(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceFear(-1);
            Me.state?.socialMemory?.GetOrInitiateRelationship(Target.ID).InfluenceTempFear(-1);
            Playr.room.abstractRoom.AddEntity(Me);
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
    public override void Destroy()
    {
        base.Destroy();
        Me?.realizedCreature?.Destroy();
        Me.Destroy();
    }

    public void Die(Room room = null)
    {
        if (room is not null && Lizor?.bodyChunks is not null)
        {
            room.PlaySound(SoundID.Puffball_Eplode, Lizor.firstChunk);
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
}