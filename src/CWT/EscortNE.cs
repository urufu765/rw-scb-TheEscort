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
    public List<AbstractCreature> NEsVulnerable;
    public int NEsClearVulnerable;
    public int NEsAbilityTime;
    public AbstractCreature NEsAbstractShadowPlayer;
    public ShadowPlayer NEsShadowPlayer;


    public void EscortNE()
    {
        this.NewEscapist = false;
        this.NEsLastInput = new IntVector2(0, 0);
        this.NEsSetCooldown = 0;
        this.NEsCooldown = 0;
    }

}

public class ShadowPlayer : Player
{
    private int killTime = 400;
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
        Ebug($"Shadowplayer created with ai? {this.AI is null}", 1, true);
        Ebug($"I am a {this.slugcatStats.name.value}!", 1, true);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        killTime--;
        
        if (killTime <= 0)
        {
            this.Die();
            this.Destroy();
        }
    }
}
