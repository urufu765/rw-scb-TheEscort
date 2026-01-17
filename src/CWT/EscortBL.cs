using System;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using static TheEscort.Eshelp;
using MoreSlugcats;
using RWCustom;

namespace TheEscort;

public enum Melee
{
    None,
    /// <summary>
    /// Rock.
    /// </summary>
    Punch,
    /// <summary>
    /// Grenade.
    /// </summary>
    ExPunch,
    /// <summary>
    /// Spear/electric spear
    /// </summary>
    Shank,
    /// <summary>
    /// Explosive spear
    /// </summary>
    ExShank,
    /// <summary>
    /// FOR VISUAL PURPOSE ONLY
    /// </summary>
    SuperShank
}

public partial class Escort
{
    /// <summary>
    /// Brawler body color when color mode is not custom
    /// </summary>
    public Color BrawlerColor;

    /// <summary>
    /// Stores the original "doNotTumbleAtLowSpeed" state of a spear before it's shoved into a wall
    /// </summary>
    public bool BrawWall;

    // /// <summary>
    // /// Supershanking state
    // /// </summary>
    // public bool BrawShankMode;

    // /// <summary>
    // /// Regular shanking state
    // /// </summary>
    // public bool BrawShank;

    // /// <summary>
    // /// Punching state
    // /// </summary>
    // public bool BrawPunch;

    /// <summary>
    /// Stores the original "doNotTumbleAtLowSpeed" state of a spear before it's used as a shiv
    /// </summary>
    public bool BrawShankSpearTumbler;

    /// <summary>
    /// Direction of the supershank (towards shank creature)
    /// </summary>
    public Vector2 BrawShankDir;

    /// <summary>
    /// Stores the reference to the weapon that's used for melee attacks so it can be affected beyond the first frame
    /// </summary>
    public Stack<Weapon> BrawMeleeWeapon;

    /// <summary>
    /// Melee weapon use grasp index (-1 when not using melee)
    /// </summary>
    public int BrawThrowUsed;

    /// <summary>
    /// Delay before Brawler retrieves the melee weapon (higher = longer distance throw)
    /// </summary>
    public int BrawThrowGrab;

    /// <summary>
    /// Delay before the spear is stuck in wall and no longer tampered with by Brawler
    /// </summary>
    public int BrawRevertWall;

    /// <summary>
    /// Stores the reference to the spear that will be thrown into a wall
    /// </summary>
    public Stack<Spear> BrawWallSpear;

    /// <summary>
    /// Melee weapon Brawler is holding (Used primarily for HUD tracking)
    /// </summary>
    public Melee BrawLastWeapon;

    /// <summary>
    /// Melee explosive weapon that is currently in motion(being used for melee attack) and should be parried
    /// </summary>
    public Melee BrawWeaponInAction;

    /// <summary>
    /// stores the value of ExplosiveSpear.explodeAt
    /// </summary>
    public int BrawExpspearAt;

    /// <summary>
    /// Whether Brawler is performing a super shank
    /// </summary>
    public bool BrawSuperShank;

    /// <summary>
    /// stores the value of slowMovementStun (Used primarily for HUD tracking)
    /// </summary>
    public int BrawSetCooldown;

    /// <summary>
    /// IFrames for self-induced explosions
    /// </summary>
    public int BrawExpIFrames;

    /// <summary>
    /// Alternative check for when the InAction expires and the explosion happens a few frames later
    /// </summary>
    public int BrawExpIFrameReady;

    // /// <summary>
    // /// Explosive punch state
    // /// </summary>
    // public bool BrawExPunch;

    public void EscortBL()
    {
        this.Brawler = false;
        this.BrawlerColor = new Color(0.447f, 0.235f, 0.53f);
        this.BrawWall = false;
        this.BrawRevertWall = -1;
        this.BrawWallSpear = new Stack<Spear>(1);
        this.BrawShankDir = new Vector2();
        this.BrawMeleeWeapon = new Stack<Weapon>(1);
        this.BrawShankSpearTumbler = false;
        this.BrawThrowGrab = -1;
        this.BrawThrowUsed = -1;
        this.BrawLastWeapon = Melee.None;
        this.BrawSetCooldown = 20;
        this.BrawSuperShank = false;
        this.BrawWeaponInAction = Melee.None;
    }
}