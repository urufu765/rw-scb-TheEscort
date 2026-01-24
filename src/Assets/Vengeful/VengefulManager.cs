using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;

public abstract class VengefulCreatureManager
{
    public AbstractCreature Me { get; set; }
    public AbstractCreature Target { get; set; }
    public virtual Creature RealMe => Me?.realizedCreature;
    public Player Player => Target?.realizedCreature as Player;
    public AbstractRoom StartingRoom { get; set; }
    public RainWorldGame Game { get; set; }
    public WorldCoordinate Coord { get; set; }
    public virtual CreatureTemplate.Type MyType => CreatureTemplate.Type.Slugcat;
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

    protected VengefulCreatureManager(AbstractCreature target, RainWorldGame game, bool online)
    {
        iAmReadyToBeAdded = false;
        Game = game;
        WorldCoordinate coord = GetCornerableRoom(target, MyType);
        Me = GiveMeAnAbstractThing(game, coord, target, MyType);
        StartingRoom = target.world.GetAbstractRoom(coord.room);
        Target = target;
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
            iAmStuckCount += by;
        }
        else if (iAmStuckCount > 0)
        {
            iAmStuckCount -= by;
        }

        if (checkItOutClock > 0)
        {
            checkItOutClock -= by;
        }
        if (teleportClock > 0)
        {
            teleportClock -= by;
        }

        if (iAmWaitingToRespawn > 0)
        {
            iAmWaitingToRespawn -= by;
        }

        if (iAmStuckCount < 0) iAmStuckCount = 0;
        if (checkItOutClock < 0) checkItOutClock = 0;
        if (teleportClock < 0) teleportClock = 0;
        if (iAmWaitingToRespawn < 0) iAmWaitingToRespawn = 0;
    }

    public void Update()
    {
        if (iWantDie) return;


        // Respawn creature
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

        // Teleports creature
        iAmStuck = Me.abstractAI?.path is null || Me.abstractAI?.path?.Count == 0;
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
            if (TeleportMeNearPlayer())
            {
                iAmStuckCount = 0;
                teleportClock = 200;
            }
        }

        // Prevents lizard from seeking player in shelter/gate
        if (Target.Room.shelter || Target.Room.gate) return;

        // Be aggressive to player
        BeVeryAngryAtPlayerAndNothingElse(Me, Target);

        // Does destination update infrequently while not in same room, or frequently if in same room.
        if (checkItOutClock == 0 || Me.pos.room == Target.pos.room)
        {
            checkItOutClock = 160;
        }
        else return;

        // Track player
        GetMeToPlayerPos();
    }

    /// <summary>
    /// Sets destination of creature to player
    /// </summary>
    public virtual void GetMeToPlayerPos()
    {
        if (Player is not null && !Player.dead && Me.abstractAI?.destination.room != Player.abstractCreature.pos.room)
        {
            //Ebug(Player, $"Vengeful creature destination changed! From: {Me.abstractAI.destination.ResolveRoomName()} to {Player.abstractCreature.pos.ResolveRoomName()}");
            Me.abstractAI.SetDestination(Player.abstractCreature.pos);
        }
    }

    /// <summary>
    /// Sets anger towards player and ignores everything else
    /// </summary>
    /// <param name="abstractMe"></param>
    /// <param name="abstractPlayer"></param>
    public abstract void BeVeryAngryAtPlayerAndNothingElse(AbstractCreature abstractMe, AbstractCreature abstractPlayer);

    /// <summary>
    /// Teleports creature to a nearby room if lost
    /// </summary>
    /// <returns>Whether the teleportation was successful</returns>
    public bool TeleportMeNearPlayer()
    {
        if (Me.realizedCreature is not null) return false;
        try
        {
            if (Me is not null && Player?.room is not null)
            {
                IntVector2 exit = Player.room.exitAndDenIndex[UnityEngine.Random.Range(0, Player.room.exitAndDenIndex.Length)];
                if (Player.room.WhichRoomDoesThisExitLeadTo(exit) is AbstractRoom vroom && !vroom.shelter && !vroom.gate)
                {
                    Me.Move(vroom.RandomNodeInRoom());
                    Ebug(Player, $"Vengeful lizard is teleporting to {vroom.name}!");
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
            WorldCoordinate coord = GetCornerableRoom(Target, MyType);
            StartingRoom = Target.world.GetAbstractRoom(coord.room);
            Me = GiveMeAnAbstractThing(Game, coord, Target, MyType);
            iAmStuckCount = 0;
            return true;
        }
        return false;
    }

    public static AbstractCreature GiveMeAnAbstractThing(RainWorldGame game, WorldCoordinate coord, AbstractCreature target, CreatureTemplate.Type type)
    {
        AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(type), null, coord, game.GetNewID());
        creature.saveCreature = false;
        creature.ignoreCycle = true;
        if (ModManager.MSC) creature.voidCreature = true;
        creature.HypothermiaImmune = true;
        creature.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceLike(-1);
        creature.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempLike(-1);
        creature.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceFear(-1);
        creature.state?.socialMemory?.GetOrInitiateRelationship(target.ID).InfluenceTempFear(-1);
        return creature;
    }



    /// <summary>
    /// Gets nearby room to spawn in
    /// </summary>
    /// <param name="abstractPlayer"></param>
    /// <returns></returns>
    public static WorldCoordinate GetCornerableRoom(AbstractCreature abstractPlayer, CreatureTemplate.Type type)
    {
        List<int> l = [];

        for (int i = 0; i < abstractPlayer.world.NumberOfRooms; i++)
        {
            if (
                abstractPlayer.world.GetAbstractRoom(abstractPlayer.world.firstRoomIndex + i) is AbstractRoom ar &&
                !ar.shelter &&
                !ar.gate &&
                ar.name != abstractPlayer.Room.name &&
                ar.NodesRelevantToCreature(StaticWorld.GetCreatureTemplate(type)) > 0)
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
        if (RealMe is not null)
        {
            RealMe.AllGraspsLetGoOfThisObject(true);
            if (RealMe.grasps is not null)
            {
                for (int i = 0; i < RealMe.grasps.Length; i++)
                {
                    RealMe.grasps[i]?.grabbed?.AllGraspsLetGoOfThisObject(false);
                }
            }
        }
        Me?.realizedCreature?.Destroy();
    }

    /// <summary>
    /// Does a dying animation
    /// </summary>
    /// <param name="room"></param>
    public virtual void DieAnimation(Room room = null)
    {
        if (room is not null && RealMe?.bodyChunks is not null)
        {
            room.PlaySound(SoundID.Puffball_Eplode, RealMe.firstChunk);
            room.AddObject(new ShockWave(RealMe.firstChunk.pos, Mathf.Lerp(25f, 50f, UnityEngine.Random.value), 0.07f, 20));
            for (int i = 0; i < RealMe.bodyChunks.Length; i++)
            {
                room.AddObject(new Explosion.ExplosionSmoke(RealMe.bodyChunks[i].pos, Custom.RNV() * (2f * UnityEngine.Random.value), 1f)
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

    /// <summary>
    /// Adds spawned vengeful into room
    /// </summary>
    public void Spawn()
    {
        if (iAmReadyToBeAdded)
        {
            StartingRoom.AddEntity(Me);
            Ebug("I have spawned!! Get ready!", ignoreRepetition: true);
            iAmReadyToBeAdded = false;
        }
    }

    /// <summary>
    /// Do a killing animation, then attempt to respawn
    /// </summary>
    /// <param name="respawnTime"></param>
    /// <param name="dontTakeMySpearsLmao"></param>
    public void KillAndRespawn(int respawnTime, bool dontTakeMySpearsLmao = true)
    {
        if (RealMe?.room is not null)
        {
            DieAnimation(RealMe.room);
        }
        if (RealMe is not null)
        {
            RealMe.AllGraspsLetGoOfThisObject(true);
            if (dontTakeMySpearsLmao)
            {
                Me.LoseAllStuckObjects();
            }
            if (RealMe.grasps is not null)
            {
                for (int i = 0; i < RealMe.grasps.Length; i++)
                {
                    if (RealMe.grasps[i]?.grabbed is not null)
                    {
                        RealMe.ReleaseGrasp(i);
                    }
                }
            }
        }

        RealMe?.Destroy();
        if (respawns > 2)
        {
            Ebug("I ran out of respawns! Doh!", ignoreRepetition: true);
            iWantDie = true;
            return;
        }
        iAmWaitingToRespawn = respawnTime;
        iWantToRespawn = true;
        Ebug($"I am getting ready to respawn! {iAmWaitingToRespawn}", ignoreRepetition: true);
    }

    public virtual void GetSomeHealing(float healing)
    {
    }
}