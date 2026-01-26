using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Escapist.Strappable;


/// <summary>
/// Strappable + arm = Strammable! I'm a genius!
/// </summary>
public abstract class Strammable<T> : IStrammable where T : AbstractPhysicalObject 
{
    public Player owner;
    public T thing;
    public int hand;
    public bool useAvail;
    public bool NullReady { get; set; }

    //public Player.AbstractOnBackStick abstractSticker;

    protected Strammable(Player owner, T thing, int hand)
    {
        this.owner = owner;
        this.thing = thing;
        this.hand = hand;
        this.useAvail = true;
    }

    public virtual void Update(bool eu)
    {
    }

    public abstract bool Activate(bool first);

    public abstract bool EmergencyActivate(bool first, Creature creature);

    public virtual void GraphicsModuleUpdated(bool actuallyViewed, bool eu)
    {
        // TODO: Find out how the hand disappearing thing works
        if (owner.graphicsModule is PlayerGraphics pg && pg.hands?[hand] is SlugcatHand sch)
        {
            Vector2 wristPos = Vector2.Lerp(sch.pos, pg.head.pos, .1f);
            DoVisualShtick(wristPos);
        }
    }
    public abstract void DoVisualShtick(Vector2 wristPos);

    public virtual void Destroy()
    {
        thing.Destroy();
        NullReady = true;
    }
}


/// <summary>
/// Interface so I can use the same methods across all of them Strammable
/// </summary>
public interface IStrammable
{
    public bool NullReady{get;set;}
    public abstract void Update(bool eu);
    
    /// <summary>
    /// Activates the strammable skill that is used with the special button
    /// </summary>
    /// <param name="first">First to activate (so the other ones don't activate duplicate things)</param>
    /// <returns>if the thing activated successfully</returns>
    public abstract bool Activate(bool first);
    public abstract bool EmergencyActivate(bool first, Creature creature);
    public abstract void GraphicsModuleUpdated(bool actuallyViewed, bool eu);
    public abstract void Destroy();
}
