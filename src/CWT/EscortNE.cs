using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using RWCustom;
using static TheEscort.Eshelp;

namespace TheEscort;


public partial class Escort
{
    /// <summary>
    /// Color of New Escapist
    /// </summary>
    public Color NewEscapistColor;

    /// <summary>
    /// Color of shadow clone
    /// </summary>
    public Color NEsShadowColor;
    
    /// <summary>
    /// Last directional input made by the player (X, Y)
    /// </summary>
    public IntVector2 NEsLastInput;
    
    /// <summary>
    /// Cooldown that will be set next frame
    /// </summary>
    public int NEsSetCooldown;
    
    /// <summary>
    /// Cooldown of the New Escapist shadowclone ability
    /// </summary>
    public int NEsCooldown;
    
    /// <summary>
    /// List of creatures made vulnerable by the shadowclone ability
    /// </summary>
    public List<Creature> NEsVulnerable;
    
    /// <summary>
    /// Duration left before the vulnerable creature list is purged
    /// </summary>
    public int NEsClearVulnerable;
    
    /// <summary>
    /// Ability active duration
    /// </summary>
    public int NEsAbility;
    
    /// <summary>
    /// Abstractcreature address of the shadowclone
    /// </summary>
    public AbstractCreature NEsAbstractShadowPlayer;
    
    /// <summary>
    /// Address of the shadowclone (realised)
    /// </summary>
    public ShadowPlayer NEsShadowPlayer;
    
    /// <summary>
    /// Time duration reference of the shadowclone ability
    /// </summary>
    public const int NEsAbilityTime = 320;
    
    /// <summary>
    /// Early cooldown reset enabler
    /// </summary>
    public bool NEsResetCooldown;
    
    /// <summary>
    /// Whether shelter is closing or not to prevent shadowclone from being spawned
    /// </summary>
    public bool NEsShelterCloseTime;
    
    /// <summary>
    /// Trackrr information for the latest max cooldown
    /// </summary>
    public int NEsLastCooldown;
    
    /// <summary>
    /// (Unused) Shadowclone afterimage trail
    /// </summary>
    public Queue<BodyDouble> NEsShadow;
    
    /// <summary>
    /// (Unused) Time between each shadowclone afterimage trail
    /// </summary>
    public int NEsAddToTrailCD;
    
    /// <summary>
    /// Increases the time duration of the slugcat's ability to grab a thing to free themselves from the predator
    /// </summary>
    public int NEsDangerGraspExtend;

    /// <summary>
    /// Variable that determins if Socks has been spawned or not (may need revision)
    /// </summary>
    public bool NEsSocks;

    /// <summary>
    /// Despite everything, this is not a self contained init function. Only to be used as a method in the main Escort class
    /// </summary>
    public void EscortNE()
    {
        this.NewEscapist = false;
        this.NewEscapistColor = new Color(0.28f, 0.675f, 0.686f);
        this.NEsShadowColor = new Color(0.1f, 0.157f, 0.165f);
        this.NEsLastInput = new IntVector2(0, 0);
        this.NEsSetCooldown = 0;
        this.NEsCooldown = 0;
        this.NEsVulnerable = new();
        this.NEsClearVulnerable = 0;
        this.NEsAbility = 0;
        this.NEsResetCooldown = false;
        this.NEsShelterCloseTime = false;
        this.NEsShadow = new();
        this.NEsDangerGraspExtend = 0;
        this.NEsSocks = false;
    }


    /// <summary>
    /// (Unused) Adds and manages shadowclone afterimage trail
    /// </summary>
    /// <param name="roomCamera">Inherited romcam from predecessor method</param>
    /// <param name="s">Inherited sprite leaser from predecessor method</param>
    public void Escat_NE_AddTrail(RoomCamera roomCamera, RoomCamera.SpriteLeaser s)
    {
        if (NEsAddToTrailCD == 0 && NEsCooldown == 0 && NEsAbility == 0)
        {
            NEsShadow.Enqueue(new BodyDouble(roomCamera, s));
            if (Custom.rainWorld.options.quality == Options.Quality.MEDIUM)
            {
                NEsAddToTrailCD = 2;
            }
            else if (Custom.rainWorld.options.quality == Options.Quality.LOW)
            {
                NEsAddToTrailCD = 7;
            }
        }
        else
        {
            NEsAddToTrailCD--;
        }
    }

    /// <summary>
    /// (Unused) Updates every living shadowclone afterimage
    /// </summary>
    public void Escat_NE_ShowTrail()
    {
        int killCount = 0;
        foreach (BodyDouble bodyDouble in NEsShadow)
        {
            if (bodyDouble.ReadyToKill)
            {
                killCount++;
                continue;
            }
            bodyDouble.Update();
        }
        for (int i = 0; i < killCount && NEsShadow.Count > 0; i++)
        {
            NEsShadow.Dequeue().Kill();
        }
    }
}


/// <summary>
/// The jank solution of my lifetime. Creates a shadow clone who's actually a player except stripped of control and AI
/// </summary>
public class ShadowPlayer : Player
{
    private int killTime = Escort.NEsAbilityTime;
    private Smoke.FireSmoke smoke;
    private Smoke.BlackHaze smokeScreen;
    public readonly Creature killTagPlayer;
    private Color smokeColor;

    public ShadowPlayer(AbstractCreature abstractCreature, World world, Player basePlayer) : base(abstractCreature, world)
    {
        this.controller = new ArenaGameSession.PlayerStopController();
        this.cameraSwitchDelay = -1;
        
        this.dropGrabTile = basePlayer.dropGrabTile;
        this.killTagPlayer = basePlayer;
        for (int i = 0; i < this.bodyChunks.Length; i++)
        {
            this.bodyChunks[i].vel.x = basePlayer.bodyChunks[i].vel.x;
            this.bodyChunks[i].vel.y = basePlayer.bodyChunks[i].vel.y;
            this.bodyChunks[i].pos.x = basePlayer.bodyChunks[i].pos.x;
            this.bodyChunks[i].pos.y = basePlayer.bodyChunks[i].pos.y;
        }
        this.animation = basePlayer.animation;
        this.bodyMode = basePlayer.bodyMode;
        this.slugcatStats.lungsFac = 0.01f;
        smokeColor = Color.grey;
        if (basePlayer.room is not null)
        {
            smoke = new(basePlayer.room);
            smokeScreen = new(basePlayer.room, bodyChunks[1].pos);
        }
        Ebug($"Shadowplayer created with ai? {this.AI is not null}", 1, true);
        Ebug($"I am a {this.slugcatStats.name.value}!", 1, true);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (Plugin.eCon.TryGetValue(this, out Escort escort))
        {
            smokeColor = escort.hypeColor;
        }
        if (this.room is not null)
        {
            smoke ??= new(room);
            smokeScreen ??= new(room, bodyChunks[1].pos);
        }
        if (smoke is not null)
        {
            smoke.Update(eu);
            if (room.ViewedByAnyCamera(firstChunk.pos, 300f))
            {
                smoke.EmitSmoke(bodyChunks[1].pos, new(0, 2), smokeColor, 40);
            }
            if (smoke.Dead)
            {
                smoke = null;
            }
        }
        if (smokeScreen is not null)
        {
            smokeScreen.MoveTo(bodyChunks[1].pos, eu);
            if (smokeScreen.slatedForDeletetion)
            {
                smokeScreen = null;
            }
        }
        if (room is not null && room.abstractRoom.gate)
        {
            GoAwayShadow();
        }
        killTime--;
        if (bodyMode != BodyModeIndex.ClimbingOnBeam)
        {
            standing = true;
        }
        
        if (killTime <= 0)
        {
            smoke.Destroy();
            this.Destroy();
        }

        if (dangerGrasp is not null && dangerGraspTime > 2)
        {
            this.Violence(dangerGrasp.grabber.firstChunk, Custom.DirVec(dangerGrasp.grabber.mainBodyChunk.pos, firstChunk.pos) * 0.1f, firstChunk, null, DamageType.Bite, 0.1f, 40);
        }
    }


    public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos hitAppendage, DamageType type, float damage, float stunBonus)
    {
        base.Violence(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
        if (type is null || (source is not null && source.owner is Player)) return;
        bool electricExplosion = false;
        bool targetted = false;
        Stack<Creature> creatureList = new();
        try
        {
            foreach (UpdatableAndDeletable thing in this.room.updateList)
            {
                // Determine if a targetted damage should be used or just a simple explosion
                // Prevents Escapists from killing other players by accident when spearhits are off
                if (thing is Player p and not ShadowPlayer && p != killTagPlayer && !(ModManager.CoopAvailable && !Custom.rainWorld.options.friendlyFire))
                {
                    targetted = true;
                }

                if (
                    thing is Creature creature and not ShadowPlayer &&
                    creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && 
                    creature != this &&
                    !(
                        creature is Player && 
                        ModManager.CoopAvailable && 
                        !Custom.rainWorld.options.friendlyFire
                    ) && 
                    !creature.dead && 
                    Custom.Dist(creature.firstChunk.pos, this.firstChunk.pos) < 180f
                )
                {
                    creatureList.Push(creature);
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Targetter didn't work!");
        }


        float frc, dmg, stn;
        frc = dmg = stn = 0;
        if (type == DamageType.Blunt)
        {
            frc = 10;
            dmg = 0.75f;
            stn = 480;
            Ebug("Shadowscort Blunt hit!", ignoreRepetition: true);
        }
        if (type == DamageType.Bite)
        {
            frc = 3.5f;
            dmg = 1.8f;
            stn = 160;
            Ebug("Shadowscort Bite hit!", ignoreRepetition: true);
        }
        if (type == DamageType.Stab)
        {
            frc = 6f;
            dmg = 3.5f;
            stn = 100;
            Ebug("Shadowscort Stab hit!", ignoreRepetition: true);
        }
        if (type == DamageType.Explosion)
        {
            frc = 2f;
            dmg = 0.75f;
            stn = 0f;
            for (int i = 0; i < 4; i++)
            {
                AbstractPhysicalObject apo = new(room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, coord, room.game.GetNewID());
                apo.RealizeInRoom();
                room?.AddObject(apo.realizedObject);
                apo.realizedObject.firstChunk.pos = i switch
                {
                    3 => new(firstChunk.pos.x - 20, firstChunk.pos.y),
                    2 => new(firstChunk.pos.x - 10, firstChunk.pos.y - 10),
                    1 => new(firstChunk.pos.x + 20, firstChunk.pos.y),
                    _ => new(firstChunk.pos.x + 10, firstChunk.pos.y - 10)
                };
            }
            Ebug("Shadowscort Explosion hit!", ignoreRepetition: true);
        }
        if (type == DamageType.Electric)
        {
            List<Creature> squirmList = new();
            foreach (UpdatableAndDeletable uad in room.updateList)
            {
                if (uad is Creature creature and not ShadowPlayer)
                {
                    if (Custom.Dist(creature.firstChunk.pos, firstChunk.pos) < 175)
                    {
                        squirmList.Add(creature);
                    }
                }
            }
            foreach (Creature c in squirmList)
            {
                c.Violence(firstChunk, directionAndMomentum * -1, c.firstChunk, null, Creature.DamageType.Electric, 0.1f, 320);
                if (this.Submersion <= 0.5f && c.Submersion > 0.5f)
                {
                    room?.AddObject(new UnderwaterShock(room, null, c.firstChunk.pos, 10, 800, 2, killTagPlayer, new Color(0.8f, 0.8f, 1f)));
                }
                room?.AddObject(new CreatureSpasmer(c, false, c.stun));
            }
            electricExplosion = true;
            frc = 4f;
            dmg = 0.1f;
            stn = 80;
        }
        if (targetted)
        {
            foreach (Creature cr in creatureList)
            {
                cr.Violence(this.mainBodyChunk, new Vector2(0, 0), cr.firstChunk, null, DamageType.Explosion, Mathf.Lerp(dmg, 0.1f, Mathf.InverseLerp(0, 175, Custom.Dist(cr.firstChunk.pos, this.firstChunk.pos))), 0);
            }
            dmg = 0.1f;
        }

        this.DoExplode(room, frc, dmg, stn, electricExplosion);
        GoAwayShadow();
    }


    public override bool SpearStick(Weapon source, float dmg, BodyChunk chunk, Appendage.Pos appPos, Vector2 direction)
    {
        Ebug("Spearstick!", ignoreRepetition: true);
        return true;
    }


    public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        base.Collide(otherObject, myChunk, otherChunk);
        if (otherObject is Weapon)
        {
            Ebug("Detected spear!", ignoreRepetition: true);
        }
    }


    public void GoAwayShadow(int setTime = 0)
    {
        killTime = setTime;
    }


    public override void Die()
    {
        base.dead = true;
        base.Die();
    }

    public override void Destroy()
    {
        // InsectCoordinator si = null;
        // foreach(UpdatableAndDeletable uad in room?.updateList)
        // {
        //     if (uad is InsectCoordinator ic)
        //     {
        //         si = ic;
        //         break;
        //     }
        // }

        // for (int i = 0; i < 10; i++)
        // {
        //     SporeCloud sc = new(bodyChunks[1].pos, new(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-0.5f, i * 0.1f)), Color.black, 1f, null, 0, si)
        //     {
        //         nonToxic = true
        //     };
        //     room?.AddObject(sc);
        // }
        // for (int j = 0; j < 8; j++)
        // {
        //     room?.AddObject(new Explosion.ExplosionSmoke(bodyChunks[1].pos, new(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-0.5f, 1.5f)), 6));
        // }
        if (room.ViewedByAnyCamera(firstChunk.pos, 300f))
        {
            smokeScreen.EmitSmoke(new(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.5f, 0.8f)), 3.8f);
            //smokeScreen.EmitBigSmoke(UnityEngine.Random.Range(1.7f, 4f));
        }

        base.dead = true;
        slatedForDeletetion = true;
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (firstContact && speed > 30f)
        {
            this.Violence(null, firstChunk.pos * 0.1f, firstChunk, null, DamageType.Explosion, 0.1f, 40);
            Ebug("Deploy explosive!", 2, true);
        }
    }
}


public static class NewEscapistHelpers
{
    public static void DoExplode(this ShadowPlayer source, Room room, float force, float damage, float stun, bool electric = false)
    {
        room?.AddObject(new Explosion(room: room, sourceObject: source, pos: source.bodyChunks[1].pos, lifeTime: 6, rad: 175f, force: force, damage: damage, stun: stun, deafen: 0.25f, killTagHolder: source.killTagPlayer, killTagHolderDmgFactor: 0.7f, minStun: stun * 0.8f, backgroundNoise: 1));
        room?.AddObject(new ShockWave(source.bodyChunks[1].pos, 200f, 0.075f, 5));
        if (electric)
        {
            room?.AddObject(new Explosion.ExplosionLight(source.firstChunk.pos, 40, 1, 2, new Color(0.0f, 0.8f, 0.5f)));
            room?.AddObject(new Explosion.ExplosionLight(source.firstChunk.pos, 200, 1, 4, new Color(0.7f, 1f, 1f)));
            room?.PlaySound(SoundID.Fire_Spear_Pop, source.firstChunk.pos);
            room?.PlaySound(SoundID.Firecracker_Bang, source.firstChunk.pos);
            room?.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, source.firstChunk.pos);
            room?.InGameNoise(new Noise.InGameNoise(source.firstChunk.pos, 800, source, 1f));
        }
        else
        {
            room?.AddObject(new Explosion.ExplosionLight(source.bodyChunks[1].pos, 175, 1f, 7, new Color(0.0f, 0.8f, 0.5f)));
            room?.AddObject(new Explosion.ExplosionLight(source.bodyChunks[1].pos, 120, 1f, 3, Color.white));
            room?.AddObject(new SootMark(room, source.bodyChunks[1].pos, 80f, true));
            room?.ScreenMovement(source.bodyChunks[1].pos, default, 1.3f);
            room?.PlaySound(SoundID.Bomb_Explode, source.bodyChunks[1].pos);
            room?.InGameNoise(new Noise.InGameNoise(source.bodyChunks[1].pos, 9000, source, 1f));
        }
    }
}
