using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Escapist;


/// <summary>
/// Strappable + arm = Strammable! I'm a genius!
/// </summary>
public abstract class Strammable<T> : IStrammable where T : AbstractPhysicalObject 
{
    public Player owner;
    public T thing;
    public int hand;
    public bool useAvail;
    
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

    public abstract void Activate();

    public abstract void EmergencyActivate();

    public virtual void GraphicsModuleUpdated(bool actaullyViewed, bool eu)
    {
    }
}

public interface IStrammable
{
    public abstract void Update(bool eu);
    public abstract void Activate();
    public abstract void EmergencyActivate();
    public abstract void GraphicsModuleUpdated(bool actaullyViewed, bool eu);
}
