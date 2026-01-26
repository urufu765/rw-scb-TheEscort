using System;
using System.Collections.Generic;
using RWCustom;
using TheEscort.Escapist;
using TheEscort.Escapist.Strappable;
using TheEscort.Patches;
using UnityEngine;
using static TheEscort.Eshelp;


namespace TheEscort;


public enum StrammableType
{
    None,

    Cracker,  // Firecracker
    Flare,  // Flarebomb
    Puff,  // Puffball
    Rock,  // Movement tech
    WaterNut,  // Deflect Weapons
    Bees,  // SporePlant
    Jelly,  // Jellyfish stun
    Flower,  // Karma 10 ability
    // Watcher
    Graffiti  // Graffiti boom
}


public partial class Escort
{
    /// <summary>
    /// The bodycolor of Escapist when colors are not custom
    /// </summary>
    public Color EscapistColor;

    /// <summary>
    /// Extends self.dangerGraspTime so the player has much more time to grab a weapon and free themselves while they're being carried away
    /// </summary>
    public int EscDangerExtend;

    /// <summary>
    /// Special grasp store that stores the creature that is grabbing onto Escapist, so when they press the emergency eject ability button, they pop out of whatever they're being grabbed by... even includes your friends and famil- no not the last one.
    /// </summary>
    public Creature.Grasp EscDangerGrasp;

    /// <summary>
    /// How long the player has to hold the special eject ability button for before they can pop out of any grasp
    /// </summary>
    public int EscUnGraspTime;

    /// <summary>
    /// The max time the player has to hold the special eject ability button for. Is used to slowly reset the EscUnGraspTime timer until it reaches this max.
    /// </summary>
    public int EscUnGraspLimit;

    /// <summary>
    /// Cooldown for this boring ability.
    /// </summary>
    public int EscUnGraspCD;

    /// <summary>
    /// Strap the weapons to the arm!
    /// </summary>
    public IStrammable[] EscStrammables;
    
    /// <summary>
    /// Strammable thing
    /// </summary>
    public StrammableType EscStramType;



    public void EscortEC()
    {
        this.Escapist = false;
        this.EscapistColor = new Color(0.1f, 0.767f, 0.306f);
        this.EscDangerExtend = 0;
        this.EscDangerGrasp = null;
        this.EscUnGraspTime = 0;
        this.EscUnGraspLimit = 0;
        this.EscUnGraspCD = 0;
        this.EscStrammables = new IStrammable[2];
        this.EscStramType = StrammableType.None;
    }
}