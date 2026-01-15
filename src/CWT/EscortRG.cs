using System;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using static TheEscort.Eshelp;
using IL.MoreSlugcats;
using RWCustom;

namespace TheEscort;


/// <summary>
/// Double-handing weapon
/// </summary>
public enum DoubleUp
{
    None,
    Rock,
    Bomb,
    Spear,
    Firecracker,
    Flare,
    // MSC
    LillyPuck,
    ElectroSpear,
    Singularity
}


/// <summary>
/// Moveset
/// </summary>
public enum RailPower
{
    None,
    Dropkick,
    Slidestun,
    Comet
}


/// <summary>
/// Receiving shock from source
/// </summary>
public enum RailElectric
{
    None,
    Violence,
    ZapCoil,
    Centipede,
    OhNoCentipede,
    // MSC
    BigAssJellyCore,
    // WATCHER
    ARZapper,
}


public class TrackedLightning
{
    public Weapon Weapon { get; set; }
    public MoreSlugcats.LightningMachine Lightning { get; set; }
    public int Life { get; set; }

    public TrackedLightning(Weapon weapon, int life)
    {
        Weapon = weapon;
        Life = life;
    }

    public bool IsDead()
    {
        if (Life <= 0)
        {
            Lightning?.Destroy();
            Lightning = null;
            Weapon = null;
            return true;
        }
        return false;
    }

    public void Update(Player player, float intensity)
    {
        Life--;
        if (Weapon is null)
        {
            Life = 0;
            return;
        }


        if (Lightning is null)
        {
            if (Weapon.room is not null)
            {
                Lightning = new MoreSlugcats.LightningMachine(new(), Weapon.firstChunk.pos, player.firstChunk.pos, 0f, false, false, 0.3f, 0.7f, Mathf.Lerp(1.5f, 0.15f, R.value))
                {
                    volume = 0.4f,
                    impactType = 1,
                    lightningType = 0.5f
                };
                Weapon.room.AddObject(Lightning);
            }
            else
            {
                Life = 0;
            }
        }
        else
        {
            float strength = Custom.LerpMap(Custom.Dist(player.firstChunk.pos, Weapon.firstChunk.pos), 20, 1000, 0.8f, 0.1f);
            Lightning.startPoint = Weapon.firstChunk.pos;
            Lightning.endPoint = player.firstChunk.pos;
            Lightning.chance = strength / 1.5f;
            Lightning.intensity = Mathf.InverseLerp(0f, strength, intensity);
        }
    }
}


public partial class Escort
{
    /// <summary>
    /// Bodycolor of Railgunner when slugcat color is not custom
    /// </summary>
    public Color RailgunnerColor;

    /// <summary>
    /// Set above 0 if Railgunner uses dualweapons to allow effects to happen frames after the shot is done
    /// </summary>
    public int RailGaussed;

    /// <summary>
    /// Used to check if two thrown weapons both belong to Railgunner
    /// </summary>
    public Creature RailThrower;  // (TODO probably make this readonly if it doesn't break anything)

    // /// <summary>
    // /// Railgunner dualwields spears!
    // /// </summary>
    // public bool RailDoubleSpear;

    // /// <summary>
    // /// Railgunner dualwields rocks!
    // /// </summary>
    // public bool RailDoubleRock;

    // /// <summary>
    // /// Railgunner dualwields spears!
    // /// </summary>
    // public bool RailDoubleLilly;

    // /// <summary>
    // /// Railgunner dualwields spears!
    // /// </summary>
    // public bool RailDoubleBomb;

    // /// <summary>
    // /// Simple combined check to check if Railgunner is dualwielding anything
    // /// </summary>
    // public bool RailDoubled
    // {
    //     get
    //     {
    //         return RailDoubleBomb || RailDoubleSpear || RailDoubleLilly || RailDoubleRock;
    //     }
    // }

    /// <summary>
    /// An enum that tells whether Railgunner is holding two weapons
    /// </summary>
    public DoubleUp RailDouble;

    /// <summary>
    /// Indicates whether weapon is the first shot out of the two
    /// </summary>
    public bool RailFirstWeaped;

    /// <summary>
    /// Stores the first weapon's dir/vel to apply to the other so they go in the same direction
    /// </summary>
    public Vector2 RailFirstWeaper;

    /// <summary>
    /// A leniency variable to make sure weapons are fired at least 4 frames after the last time Railgunner had two weapons... dunno what this actually does in a practical sense
    /// </summary>
    public int RailWeaping;

    /// <summary>
    /// Cooldown of railgunner overcharge before it's reset to 0
    /// </summary>
    public int RailgunCD;

    /// <summary>
    /// Overcharge value
    /// </summary>
    public int RailgunUse;

    /// <summary>
    /// Limit to overcharge
    /// </summary>
    public int RailgunLimit;

    /// <summary>
    /// Indicates Railgunner threw a bomb a particular way and needs to IFrame it to survive or threw electric spear underwater and need to survive
    /// </summary>
    public int RailIFrame;

    /// <summary>
    /// Self explanatory, double bomb + backflip + downthrow
    /// </summary>
    public bool RailBombJump;

    /// <summary>
    /// Frames until recoil hits
    /// </summary>
    public int RailRecoilLag;

    /// <summary>
    /// Last throw direction when railgunned so recoil goes the correct direction even after player switches direction in the middle
    /// </summary>
    public IntVector2 RailLastThrowDir;

    /// <summary>
    /// A clock to tell the game when to check for targets
    /// </summary>
    public int RailTargetClock;

    /// <summary>
    /// Bodychunk that Railgunner will point to
    /// </summary>
    public BodyChunk RailTargetAcquired;

    /// <summary>
    /// Railgunner weakened state. HAHA GET REKT LOL RAILS WHY YOU SO WEAK
    /// </summary>
    public bool RailFrail;

    /// <summary>
    /// Index which Railgunner's laser pointer is stored
    /// </summary>
    public int RailLaserSightIndex;

    /// <summary>
    /// Makes laser pointer blink
    /// </summary>
    public int RailLaserBlinkClock;

    /// <summary>
    /// Beep boop beep boop
    /// </summary>
    public bool RailLaserBlink;

    /// <summary>
    /// The colour to flash when blinking
    /// </summary>
    public Color RailLaserColor;

    /// <summary>
    /// Dims the laser if unused
    /// </summary>
    public int RailLaserDimmer;

    /// <summary>
    /// How long to go from max brightness to minimum
    /// </summary>
    public const int RailLaserDimmerDuration = 120;

    /// <summary>
    /// How long to hold the max brightness before beginning the dimmer
    /// </summary>
    public const int RailLaserDimmerHoldDur = -200;

    /// <summary>
    /// Tracks the address of fired weapons and binds lightning to them so that it brings lightning from the paws!
    /// </summary>
    public List<TrackedLightning> RailZap;

    public const int RAILGUNNER_CD_MAX = 1200;

    public void EscortRG(Player self)
    {
        this.Railgunner = false;
        this.RailgunnerColor = new Color(0.525f, 0.8f, 0.8f);
        this.RailGaussed = 0;
        this.RailThrower = self;
        this.RailDouble = DoubleUp.None;
        // this.RailDoubleSpear = false;
        // this.RailDoubleRock = false;
        // this.RailDoubleLilly = false;
        // this.RailDoubleBomb = false;
        this.RailFirstWeaped = false;
        this.RailFirstWeaper = new Vector2();
        this.RailWeaping = 0;
        this.RailgunCD = 0;
        this.RailgunUse = 0;
        this.RailgunLimit = Plugin.ins.config.cfgRailgunnerLimiter.Value;
        this.RailIFrame = 0;
        this.RailBombJump = false;
        this.RailRecoilLag = -1;
        this.RailLastThrowDir = new(0, 0);
        this.RailTargetClock = 39;
        this.RailLaserBlinkClock = 0;
        this.RailLaserBlink = false;
        this.RailLaserColor = new Color(1f, 0.7f, 0.0f);
        this.RailLaserSightIndex = -1;
        this.RailZap = [];
        this.RailLaserDimmer = RailLaserDimmerDuration;
    }

    /// <summary>
    /// Increases Railgunner overcharge
    /// </summary>
    /// <param name="fromWeapon">Whether it's from weapon or from ability</param>
    public void Escat_RG_Overcharge(bool fromWeapon = true, bool halveAddition = false)
    {
        int addition = 0;

        if (fromWeapon)
        {
            addition += RailDouble switch
            {
                DoubleUp.Rock or DoubleUp.Firecracker or DoubleUp.Flare => 1,
                DoubleUp.Bomb => 3,
                DoubleUp.Singularity => 5,
                _ => 2
            };
        }
        if (halveAddition)
        {
            addition /= 2;
        }
        RailgunUse += Math.Max(1, addition);
    }

    public void Escat_RG_Overcharge(RailElectric type)
    {
        RailgunUse += type switch
        {
            RailElectric.Violence or RailElectric.Centipede => 5,
            RailElectric.OhNoCentipede or RailElectric.BigAssJellyCore => 8,
            RailElectric.ZapCoil or RailElectric.ARZapper => 10,
            _ => 0
        };
    }

    /// <summary>
    /// Returns how much extra stun Railgunner does
    /// </summary>
    /// <param name="type">Type of dual wield</param>
    /// <returns>How much stun</returns>
    public float Escat_RG_SheDoesHowMuch(DoubleUp type)
    {
        float stun = RailgunUse * 5f;
        stun *= type switch
        {
            DoubleUp.Rock or DoubleUp.Flare => 0.75f,
            DoubleUp.Bomb => 0.67f,
            DoubleUp.Firecracker => 0.1f,
            DoubleUp.LillyPuck => 2f,
            DoubleUp.Spear => 1.5f,
            DoubleUp.Singularity => 5f,
            _ => 0.5f
        };
        if (RailFrail) stun *= 1.5f;
        return stun;
    }

    /// <summary>
    /// Returns how much extra stun Railgunner does
    /// </summary>
    /// <param name="type">Type of martial arts</param>
    /// <returns>How much stun</returns>
    public float Escat_RG_SheDoesHowMuch(RailPower type)
    {
        float stun = RailgunUse * 8f;
        stun *= type switch
        {
            RailPower.Slidestun => 2f,
            RailPower.Dropkick => 0.67f,
            RailPower.Comet => 0.1f,
            _ => 0f
        };
        if (RailFrail) stun *= 1.5f;
        return stun;
    }

    /// <summary>
    /// Imitates malnourished state entering stuff TODO
    /// </summary>
    /// <param name="fragility"></param>
    public void Escat_RG_SetGlassMode(bool fragility)
    {
        RailFrail = fragility;
    }

    public void Escat_RG_IncreaseCD(int increase)
    {
        if (RailgunCD == 0)
        {
            RailgunCD = RailFrail ? 600 : 400;
        }
        RailgunCD += RailFrail? increase / 2 : increase;
        if (RailgunCD > RAILGUNNER_CD_MAX) RailgunCD = RAILGUNNER_CD_MAX;
    }
}