using System;
using System.Collections.Generic;
using RWCustom;
using TheEscort.Patches;
using UnityEngine;
using static TheEscort.Eshelp;


namespace TheEscort;

public partial class Escort
{
    /// <summary>
    /// Deflector color when scug color is not custom
    /// </summary>
    public Color DeflectorColor;

    /// <summary>
    /// Deflector empowered timer
    /// </summary>
    public int DeflAmpTimer;

    /// <summary>
    /// Deflector easy parry (backflip onto creature)
    /// </summary>
    public bool DeflTrampoline;

    /// <summary>
    /// Deflector parry sfx cooldown (so it doesn't make the sound like 10 times per parry)
    /// </summary>
    public int DeflSFXcd;

    /// <summary>
    /// Super extended belly slide accumulator
    /// </summary>
    public int DeflSlideCom;

    /// <summary>
    /// Gives a micro boost when slide-pouncing
    /// </summary>
    public bool DeflSlideKick;

    /// <summary>
    /// Level of empowerment
    /// </summary>
    public int DeflPowah;

    /// <summary>
    /// Deflector permanent damage multiplier (per player)
    /// </summary>
    private float _deflperma;
    /// <summary>
    /// Deflector permanent damage multiplier (synced to host only)
    /// </summary>
    private float _deflpermaOnline;
    public bool DeflPermaHostWantsToShare;

    public int DeflParryCD;

    /// <summary>
    /// Gets either the per player perma damage multiplier, or the shared pool
    /// </summary>
    public float DeflPerma
    {
        get
        {
            if (Plugin.ins.config.cfgDeflecterSharedPool.Value && !escortArena)
            {
                if (DeflPermaHostWantsToShare) return _deflpermaOnline;
                return Plugin.DeflSharedPerma;
            }
            return _deflperma;
        }
        set
        {
            if (Plugin.ins.config.cfgDeflecterSharedPool.Value && !escortArena)
            {
                if (DeflPermaHostWantsToShare) _deflpermaOnline = value;
                Plugin.DeflSharedPerma = value;
            }
            else
            {
                _deflperma = value;
            }
        }
    }


    /// <summary>
    /// Empowered damage based on level
    /// </summary>
    public float DeflDamageMult
    {
        get
        {
            return DeflPowah switch
            {
                3 => 1000000f,
                2 => 7f,
                1 => 3f,
                _ => 0.5f
            };
        }
    }

    public const int DeflAerialWindow = 20;
    public const int DeflAerialWait = 10;
    public const int DeflZeroGWindow = 20;
    public const int DeflZeroGWait = 120;
    public const int DeflSwimWindow = 20;
    public const int DeflSwimCD = 80;
    public const int DeflCorridorWindow = 10;
    public const int DeflCorridorCD = 30;

    public int DeflBonusWindow
    {
        get
        {
            if (karmaTen) return 40;
            return 0;
        }
    }

    /// <summary>
    /// Deflector midair parry
    /// </summary>
    public int DeflAerialParry;

    /// <summary>
    /// Stores whether Deflector was standing or not
    /// </summary>
    public bool? DeflAerialWasStanding;

    /// <summary>
    /// Deflector zeroG parry
    /// </summary>
    public int DeflZeroGParry;

    /// <summary>
    /// Deflector zerG direction
    /// </summary>
    public Vector2 DeflZeroGDir;

    /// <summary>
    /// Deflector swim parry
    /// </summary>
    public int DeflSwimParry;

    /// <summary>
    /// Deflector corridor parry
    /// </summary>
    public int DeflCorridorParry;

    /// <summary>
    /// Deflector bonus parry
    /// </summary>
    public int DeflBonusParry;

    /// <summary>
    /// Direction the player is going in corridor
    /// </summary>
    public Vector2 DeflCorridorDir;


    public void EscortDF()
    {
        this.Deflector = false;
        this.DeflectorColor = new Color(0.23f, 0.24f, 0.573f);
        this.DeflAmpTimer = 0;
        this.DeflTrampoline = false;
        this.DeflSFXcd = 0;
        this.DeflSlideCom = 0;
        this.DeflSlideKick = false;
        this.DeflPowah = 0;
        this.DeflPerma = 0f;
        this.DeflParryCD = 0;
        this.DeflAerialParry = -1;
        this.DeflSwimParry = -1;
        this.DeflZeroGParry = -1;
        this.DeflCorridorParry = -1;
        this.DeflZeroGDir = Vector2.zero;
        this.DeflCorridorDir = Vector2.zero;
        this.DeflBonusParry = 0;
    }
}