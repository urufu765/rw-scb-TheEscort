using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using RWCustom;
using static TheEscort.Eshelp;

namespace TheEscort;


public partial class Escort
{
    public IntVector2 NEsLastInput;
    public int NEsSetCooldown;
    public int NEsCooldown;
    public List<Creature> NEsVulnerable;
    public int NEsClearVulnerable;
    public int NEsAbility;
    public AbstractCreature NEsAbstractShadowPlayer;
    public ShadowPlayer NEsShadowPlayer;
    public const int NEsAbilityTime = 400;
    public bool NEsResetCooldown;
    public bool NEsShelterCloseTime;


    public void EscortNE()
    {
        this.NewEscapist = false;
        this.NEsLastInput = new IntVector2(0, 0);
        this.NEsSetCooldown = 0;
        this.NEsCooldown = 0;
        this.NEsVulnerable = new();
        this.NEsClearVulnerable = 0;
        this.NEsAbility = 0;
        this.NEsResetCooldown = false;
        this.NEsShelterCloseTime = false;
    }

}

public class ShadowPlayer : Player
{
    private int killTime = Escort.NEsAbilityTime;
    private Smoke.FireSmoke smoke;
    private readonly Creature killTagPlayer;

    public ShadowPlayer(AbstractCreature abstractCreature, World world, Player basePlayer) : base(abstractCreature, world)
    {
        this.controller = new ArenaGameSession.PlayerStopController();
        this.cameraSwitchDelay = -1;
        
        this.standing = true;
        this.killTagPlayer = basePlayer;
        for (int i = 0; i < this.bodyChunks.Length; i++)
        {
            this.bodyChunks[i].vel.x = basePlayer.bodyChunks[i].vel.x;
            this.bodyChunks[i].vel.y = basePlayer.bodyChunks[i].vel.y;
            this.bodyChunks[i].pos.x = basePlayer.bodyChunks[i].pos.x;
            this.bodyChunks[i].pos.y = basePlayer.bodyChunks[i].pos.y;
        }
        if (basePlayer.room is not null)
        {
            smoke = new(basePlayer.room);
        }
        Ebug($"Shadowplayer created with ai? {this.AI is not null}", 1, true);
        Ebug($"I am a {this.slugcatStats.name.value}!", 1, true);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (smoke is null && this.room is not null)
        {
            smoke = new(room);
        }
        if (smoke is not null)
        {
            smoke.Update(eu);
            if (room.ViewedByAnyCamera(firstChunk.pos, 300f))
            {
                smoke.EmitSmoke(bodyChunks[1].pos, new(0, 2), Color.grey, 40);
            }
            if (smoke.Dead)
            {
                smoke = null;
            }
        }
        if (room is not null && room.abstractRoom.gate)
        {
            GoAwayShadow();
        }
        killTime--;
        standing = true;
        
        if (killTime <= 0)
        {
            smoke.Destroy();
            this.Destroy();
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
                if (thing is Player and not ShadowPlayer)
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

        room?.AddObject(new Explosion(room: room, sourceObject: this, pos: bodyChunks[1].pos, lifeTime: 6, rad: 175f, force: frc, damage: dmg, stun: stn, deafen: 0.25f, killTagHolder: killTagPlayer, killTagHolderDmgFactor: 0.7f, minStun: stn * 0.8f, backgroundNoise: 1));
        room?.AddObject(new ShockWave(bodyChunks[1].pos, 200f, 0.075f, 5));
        if (electricExplosion)
        {
            room?.AddObject(new Explosion.ExplosionLight(firstChunk.pos, 40, 1, 2, new Color(0.0f, 0.8f, 0.5f)));
            room?.AddObject(new Explosion.ExplosionLight(firstChunk.pos, 200, 1, 4, new Color(0.7f, 1f, 1f)));
            room?.PlaySound(SoundID.Fire_Spear_Pop, firstChunk.pos);
            room?.PlaySound(SoundID.Firecracker_Bang, firstChunk.pos);
            room?.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, firstChunk.pos);
            room?.InGameNoise(new Noise.InGameNoise(firstChunk.pos, 800, this, 1f));
        }
        else
        {
            room?.AddObject(new Explosion.ExplosionLight(bodyChunks[1].pos, 175, 1f, 7, new Color(0.0f, 0.8f, 0.5f)));
            room?.AddObject(new Explosion.ExplosionLight(bodyChunks[1].pos, 120, 1f, 3, Color.white));
            room?.AddObject(new SootMark(room, bodyChunks[1].pos, 80f, true));
            room?.ScreenMovement(bodyChunks[1].pos, default, 1.3f);
            room?.PlaySound(SoundID.Bomb_Explode, bodyChunks[1].pos);
            room?.InGameNoise(new Noise.InGameNoise(bodyChunks[1].pos, 9000, this, 1f));
        }
        this.Destroy();
    }


    public override void Grabbed(Grasp grasp)
    {
        base.Grabbed(grasp);
        if (dangerGrasp is not null)
        {
            this.Violence(dangerGrasp.grabber.firstChunk, Custom.DirVec(dangerGrasp.grabber.mainBodyChunk.pos, firstChunk.pos) * 0.1f, firstChunk, null, DamageType.Bite, 0.1f, 40);
        }
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


    public void GoAwayShadow()
    {
        killTime = 0;
    }


    public override void Die()
    {
        base.dead = true;
        base.Die();
    }

    public override void Destroy()
    {
        slatedForDeletetion = true;
    }
}
