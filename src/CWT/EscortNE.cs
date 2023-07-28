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
    }

}

public class ShadowPlayer : Player
{
    private int killTime = Escort.NEsAbilityTime;
    private Smoke.FireSmoke smoke;

    public ShadowPlayer(AbstractCreature abstractCreature, World world, Player basePlayer) : base(abstractCreature, world)
    {
        this.controller = new NullController();
        this.standing = true;
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
        Ebug($"Shadowplayer created with ai? {this.AI is null}", 1, true);
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
        killTime--;
        standing = true;
        
        if (killTime <= 0)
        {
            smoke.Destroy();
            this.Destroy();
        }
    }




    public void GoAwayShadow()
    {
        killTime = 0;
    }
}
