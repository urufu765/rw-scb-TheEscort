using System;
using UnityEngine;

namespace TheEscort;

/// <summary>
/// A "middleman" to convert a bunch of data into something simple for the hud meters to understand
/// </summary>
public abstract class Trackrr<T>
{
    private T _tracked;
    private T _pretrack;
    private T _max;
    private T _limit;
    public readonly int playerNumber;
    public readonly int trackerNumber;
    public readonly string trackerName;
    public readonly string centerSprite;
    public Color trackerColor;
    public Color effectColor;
    public bool force;
    public bool overridden;
    public int spriteNumber;

    public virtual T Value
    {
        get {
            return _tracked;
        } 
        set
        {
            _tracked = value;
        }
    }

    public virtual T Max
    {
        get
        {
            return _max;
        }
        set
        {
            _max = value;
        }
    }

    public virtual T Limit
    {
        get
        {
            return _limit;
        }
        set
        {
            _limit = value;
        }
    }

    public virtual T PreValue
    {
        get
        {
            return _pretrack;
        }

        set
        {
            _pretrack = value;
        }
    }

    public abstract void DrawTracker(float timeStacker);

    public virtual void UpdateTracker()
    {
        PreValue = Value;
    }

    public Trackrr(int playerNumber, int trackerNumber, string trackerName, Color trackerColor = default, string centerSprite = "")
    {
        this.playerNumber = playerNumber;
        this.trackerNumber = trackerNumber;
        this.trackerName = trackerName;
        this.trackerColor = trackerColor;
        this.effectColor = trackerColor;
        this.centerSprite = centerSprite;
    }
}

public static class ETrackrr
{
    /// <summary>
    /// Testing tracker 1: Value reduction
    /// </summary>
    public class TestTractoin : Trackrr<float>
    {
        public TestTractoin(int playerNumber, int trackerNumber) : base(playerNumber, trackerNumber, "test", new Color(0.85f, 0.85f, 0.85f))
        {
            this.Max = 160;
            this.Limit = 80;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (this.Value > 0) this.Value--;
            else this.Value = Max;
        }
    }

    /// <summary>
    /// Testing tracker 2: Value addition
    /// </summary>
    public class TestTractoin2 : Trackrr<float>
    {
        public TestTractoin2(int playerNumber, int trackerNumber) : base(playerNumber, trackerNumber, "test", new Color(0.7f, 0.7f, 0.7f))
        {
            this.Max = 200;
            this.Limit = 120;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (this.Value < Max) this.Value++;
            else this.Value = 0;
        }
    }

    /// <summary>
    /// Hype (aerobiclevel tracker)
    /// </summary>
    public class HypeTraction : Trackrr<float>
    {
        private readonly Player player;
        private readonly Escort e;
        public HypeTraction(int playerNumber, int trackerNumber, float limiter, Player player, Escort e, string centerSprite) : base( playerNumber, trackerNumber, "hype", centerSprite: centerSprite)
        {
            this.player = player;
            this.e = e;
            Max = 1f;
            Limit = limiter;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = Mathf.Lerp(PreValue, player.aerobicLevel, timeStacker);
            force = Value > Limit;
            this.trackerColor = e.hypeColor;
            this.effectColor = Color.Lerp(Color.white, e.hypeColor, 0.5f);
            this.overridden = e.overrideSprite;
        }
    }

    /// <summary>
    /// Brawler's melee attack cooldown
    /// </summary>
    public class BrawlerMeleeTraction : Trackrr<float>
    {
        private readonly Player player;
        private readonly Escort e;
        public BrawlerMeleeTraction(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "brawler", new Color(0.8f, 0.4f, 0.6f))
        {
            this.player = player;
            this.e = escort;
            this.Max = 60;
        }

        public override void DrawTracker(float timeStacker)
        {
#if false
            this.Max = e.BrawLastWeapon switch {
                "shank" => 60,
                _ => 30
            };
#endif
            this.Value = Mathf.Lerp(PreValue, Max - player.slowMovementStun, timeStacker);
            this.Limit = player.slowMovementStun > 0? 0 : 1000;
            this.force = player.slowMovementStun > 0;
            e.overrideSprite = e.BrawLastWeapon != "";
            spriteNumber = e.BrawLastWeapon switch
            {
                "powerpunch" => 3,
                "punch" => 2,
                "shank" => 1,
                "supershank" => 0,
                _ => -1
            };
        }
    }

    public class DeflectorEmpowerTraction : Trackrr<float>
    {
        private readonly Escort e;
        public DeflectorEmpowerTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "deflector", new Color(0.69f, 0.55f, 0.9f))
        {
            this.e = escort;
            this.Max = 320;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = Mathf.Lerp(PreValue, e.DeflAmpTimer, timeStacker);
            this.Limit = e.DeflPowah == 3? 0 : 160;
            spriteNumber = e.DeflPowah;
        }
    }

    public class EscapistUngraspTraction : Trackrr<float>
    {
        private readonly Escort e;
        private float transitioner;
        private float transitioning;
        private int prevMax;
        public EscapistUngraspTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "escapist")
        {
            this.e = escort;
            Color escapistColor = new Color(0.42f, 0.75f, 0.1f);
            this.trackerColor = Color.Lerp(Color.Lerp(escapistColor, Color.black, 0.6f), escapistColor, Mathf.InverseLerp(0, 40, e.EscUnGraspCD));
            this.effectColor = escapistColor;

        }

        public override void DrawTracker(float timeStacker)
        {
            if (e.EscUnGraspCD <= 0)
            {
                this.Limit = 0;
                if (e.EscUnGraspLimit != 0) 
                {
                    prevMax = e.EscUnGraspLimit;
                    e.overrideSprite = true;
                }
                else
                {
                    e.overrideSprite = false;
                }
                this.Max = prevMax;
                this.Value = Mathf.Lerp(PreValue, Mathf.Lerp(0, e.EscUnGraspTime, Mathf.InverseLerp(0, 20, transitioning)), timeStacker);
                if (e.EscUnGraspLimit == 0 && transitioning > 0) {
                    this.transitioner = -1;
                }
                else if (e.EscUnGraspLimit > 0 && transitioning < 20) {
                    this.transitioner = 1;
                }
                else
                {
                    this.transitioner = 0;
                }
            }
            else
            {
                this.Limit = 500;
                this.Max = 480;
                this.Value = Mathf.Lerp(PreValue, e.EscUnGraspCD, timeStacker);
                e.overrideSprite = e.EscUnGraspCD > 0;
            }
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            transitioning += transitioner;
        }
    }

    public class RailgunnerCDTraction : Trackrr<float>
    {
        private readonly Player player;
        private readonly Escort e;

        public RailgunnerCDTraction(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "RailgunnerCD", new Color(0.35f, 0.7f, 0.63f))
        {
            this.player = player;
            effectColor = new Color(0.85f, 0.3f, 0.0f);
            this.e = escort;
            this.Max = 800;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = Mathf.Lerp(PreValue, e.RailgunCD, timeStacker);
            this.Limit = player.Malnourished? 0 : 1000;
        }
    }

    public class RailgunnerUsageTraction : Trackrr<float>
    {
        private float preValue;
        private float setValue;
        private bool yesTrans;
        private float transitioning;
        private readonly Escort e;
        public RailgunnerUsageTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "railgunnerUse")
        {
            trackerColor = new Color(0.5f, 0.85f, 0.78f);
            effectColor = new Color(1f, 0.45f, 0.0f);
            this.e = escort;
            this.Max = e.RailgunLimit;
            this.Limit = e.RailgunLimit - 3;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (setValue != Mathf.Min(e.RailgunUse, e.RailgunLimit) && (preValue < setValue || e.RailgunCD == 0))
            {
                transitioning = 0;
                preValue = e.RailgunCD == 0? 0 : setValue;
            }
            setValue = Mathf.Min(e.RailgunUse, e.RailgunLimit);

            // Advance the transition, or reset transition ticker
            if (this.Value == setValue) {
                preValue = setValue;
                transitioning = 0;
                yesTrans = false;
            }
            else {
                yesTrans = true;
            }

            e.overrideSprite = e.RailDoubleBomb || e.RailDoubleLilly || e.RailDoubleRock || e.RailDoubleSpear;
            spriteNumber = -1;
            if(e.RailDoubleBomb) spriteNumber = 0;
            if(e.RailDoubleLilly) spriteNumber = 1;
            if(e.RailDoubleRock) spriteNumber = 2;
            if(e.RailDoubleSpear) spriteNumber = 3;

            // Smoothly transition the value with an ease out
            this.Value = Mathf.Lerp(PreValue, preValue < setValue? Mathf.Lerp(preValue, setValue, Mathf.Log(transitioning, 10)) : Mathf.Lerp(setValue, preValue, Mathf.Log(transitioning, 10)), timeStacker);
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            if (yesTrans) transitioning++;
        }
    }

    public class SpeedsterTraction : Trackrr<float>
    {
        private readonly Escort e;
        private readonly int gear;
        private float oldValue;
        private float transitioning;
        private bool yesTrans;
        public SpeedsterTraction(int playerNumber, int trackerNumber, Escort escort, int gear) : base(playerNumber, trackerNumber, "speedster")
        {
            this.e = escort;
            this.gear = gear;
            trackerColor = new Color(0.76f, 0.78f, 0f);
            effectColor = new Color(0.52f, 0.48f, 0f);
        }

        public override void DrawTracker(float timeStacker)
        {
            if (e.SpeDashNCrash)
            {
                Max = e.SpeExtraSpe;
                Limit = 0;
                if (gear <= e.SpeGear + 1){
                    Value = Mathf.Lerp(PreValue, e.SpeSpeedin, timeStacker);
                    oldValue = Value;
                    yesTrans = false;
                }
                else 
                {
                    yesTrans = true;
                    Value = Mathf.Lerp(PreValue, Mathf.Lerp(oldValue, 0, Mathf.InverseLerp(0, 20, transitioning)), timeStacker);
                }
                spriteNumber = e.SpeGear + 1;
            }
            else 
            {
                this.Max = 240;
                if (e.SpeCharge == gear - 1)
                {
                    Value = Mathf.Lerp(PreValue, e.SpeBuildup, timeStacker);
                    Limit = 300;
                }
                else if (e.SpeCharge >= gear)
                {
                    Value = Mathf.Lerp(PreValue, Max, timeStacker);
                    Limit = 0;
                }
                else {
                    Value = Mathf.Lerp(PreValue, 0, timeStacker);
                }
                yesTrans = false;
                transitioning = 0;
                oldValue = Value;
                spriteNumber = 0;
            }
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            if (yesTrans) transitioning++;
        }
    }

    public class SpeedsterOldTraction : Trackrr<float>
    {
        private readonly Escort escort;
        private readonly bool extra;
        public SpeedsterOldTraction(int playerNumber, int trackerNumber, Escort escort, bool extra = false) : base(playerNumber, trackerNumber, "speedsterOld")
        {
            this.escort = escort;
            this.extra = extra;
            this.trackerColor = extra? new Color(0.86f, 0.65f, 0f) : new Color(0.76f, 0.78f, 0f);
            this.Max = extra? 480 : 240;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (extra) 
            {
                this.effectColor = Color.Lerp(trackerColor, Color.white, 0.35f);
                Limit = escort.SpeSecretSpeed? 0 : Max;
                Value = Mathf.Lerp(PreValue, escort.SpeSecretSpeed? escort.SpeSpeedin * 2 : escort.SpeExtraSpe, timeStacker);
                spriteNumber = escort.SpeSecretSpeed? 2 : 0;
            }
            else 
            {
                this.effectColor = escort.SpeSecretSpeed? Color.Lerp(trackerColor, Color.white, 0.35f) : Color.Lerp(trackerColor, Color.black, 0.35f);
                Limit = escort.SpeDashNCrash? 0 : Max;
                Value = Mathf.Lerp(PreValue, escort.SpeSpeedin, timeStacker);
                spriteNumber = escort.SpeDashNCrash? 1 : 0;
            }
        }
    }

    /// <summary>
    /// Value: Alpha, Limit: Parried timer 
    /// </summary>
    public class DamageProtectionTraction : Trackrr<float>
    {
        private readonly Player player;
        private readonly Escort escort;
        private int transition;
        private int transitioner;
        private float preLimit;
        private int transitionLim;

        public DamageProtectionTraction(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "parry", centerSprite: "escort_hud_parry")
        {
            this.player = player;
            this.escort = escort;
            this.trackerColor = player.ShortCutColor();
            if (this.trackerColor.r < 0.44f && this.trackerColor.g < 0.44f && this.trackerColor.b < 0.44f)
            {
                this.trackerColor = Color.Lerp(Color.white, this.trackerColor, 0.7f);
            }
            this.effectColor = Color.white;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (transition < 5 && ParryCondition())
            {
                transitioner = 1;
            }
            else if (transition > 0 && transitionLim == 0 && !ParryCondition())
            {
                transitioner = -1;
            }
            else
            {
                transitioner = 0;
            }
            Value = Mathf.Lerp(PreValue, transition, timeStacker);
            Max = escort.iFrames;
            Limit = Mathf.Lerp(preLimit, transitionLim / 10, timeStacker);
            if (Max > 0) transitionLim = 10;
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            this.preLimit = this.Limit;
            if (this.transitionLim > 0) this.transitionLim--;
            this.transition += this.transitioner;
        }

        /// <summary>
        /// Check Escort's parry condition. For tracker use (minimal logggin)
        /// </summary>
        /// <param name="self">Player instance</param>
        /// <param name="escort">Escort instance</param>
        /// <returns>True if in parry condition</returns>
        private bool ParryCondition()
        {
            if (escort.Deflector && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll))
            {
                return true;
            }
            else if (player.animation == Player.AnimationIndex.BellySlide && escort.parryAirLean > 0)
            {
                return true;
            }
            return escort.parrySlideLean > 0;
        }
    }

    public class GildedPoweredTraction : Trackrr<float>
    {
        private readonly Escort escort;
        private readonly Color gildColor;
        private readonly Color overflowColor;
        private int crafting;
        private int changing;
        private int animation;
        public GildedPoweredTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "gilded")
        {
            Max = escort.GildPowerMax;
            this.escort = escort;
            gildColor = new Color(0.85f, 0.58f, 0.3f);
            overflowColor = new Color(1f, 0.4f, 0.2f);
            effectColor = gildColor;
            trackerColor = gildColor;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (escort.GildRequiredPower == 0)
            {
                Limit = escort.GildPowerMax;
                if (escort.GildPower > escort.GildPowerMax - 800)
                {
                    trackerColor = overflowColor;
                    effectColor = overflowColor;
                }
                else
                {
                    trackerColor = gildColor;
                    effectColor = gildColor;
                }
                if (crafting == 0) overridden = escort.overrideSprite = false;
            }
            else
            {
                Limit = escort.GildStartPower - escort.GildRequiredPower;
                if (escort.GildReservePower >= escort.GildRequiredPower && !escort.GildFloatState) crafting = 30;
                overridden = escort.overrideSprite = true;
            }
            Value = Mathf.Lerp(PreValue, escort.GildPower, timeStacker);
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            if (crafting > 0) 
            {
                crafting--;
                spriteNumber = 3;
            }
            else
            {
                spriteNumber = animation + (escort.GildFloatState? 4 : 0);
            }
            if (changing > 0)
            {
                changing--;
            }
            else
            {
                changing = 5;
                animation++;
                if (animation == 3) animation = 0;
            }
        }
    }

    public class DeflectorPermaDamage : Trackrr<float>
    {
        private readonly Escort e;
        private int tickSlowly;
        private int tickEvenSlowly;
        private int tickExtremelySlowly;
        private int sizeIncrease;
        private bool firstInit = true;
        private readonly Color deflectorColor = new Color(0.69f, 0.55f, 0.9f);
        public DeflectorPermaDamage(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "deflectorPerma")
        {
            this.e = escort;
            this.Limit = 0;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = Mathf.Lerp(PreValue, Mathf.InverseLerp(0, 50, sizeIncrease + tickExtremelySlowly), timeStacker);
            this.trackerColor = Color.Lerp(deflectorColor, Color.white, Mathf.InverseLerp(0, 40, tickExtremelySlowly));
            this.effectColor = e.hypeColor;
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            if (this.Limit < e.DeflPerma)
            {
                if (!firstInit) tickSlowly++;
                if (sizeIncrease < 30 && tickSlowly >= 4) 
                {
                    sizeIncrease++;
                    tickSlowly = 0;
                }
                tickEvenSlowly++;
                if (tickEvenSlowly >= (firstInit? 4 : 10))
                {
                    this.Limit += 0.001f;
                    tickEvenSlowly = 0;
                    tickExtremelySlowly = firstInit? 8 : 20;
                }
            }
            else
            {
                firstInit = false;
                if (sizeIncrease > 0)
                {
                    sizeIncrease--;
                }
            }
            if (tickExtremelySlowly > 0)
            {
                tickExtremelySlowly--;
            }
            Max = e.DeflPowah switch {
                3 => 69,
                2 => 7,
                1 => 3,
                _ => 0.5f
            };
        }
    }

    public class SwimTracker : Trackrr<float>
    {
        readonly Player player;
        readonly Escort escort;
        int animation, shallow, deep;
        float aniTick;


        public SwimTracker(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "swimming")
        {
            this.player = player;
            this.escort = escort;
            Max = escort.isDefault? 1 : 0;
        }

        public override void DrawTracker(float timeStacker)
        {
            trackerColor = escort.hypeColor;
            effectColor = escort.viscoColor;
            Limit = Mathf.InverseLerp(0, 40, shallow);
            Value = Mathf.InverseLerp(0, 30, deep);
            spriteNumber = animation;
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            if (player.animation == Player.AnimationIndex.SurfaceSwim)
            {
                if (shallow < 40) shallow += 4;
            }
            else if (player.animation == Player.AnimationIndex.DeepSwim)
            {
                if (deep < 30) deep++;
                if (shallow < 40) shallow += 80;
            }
            else
            {
                if (deep > 0) deep--;
                if (shallow > 0) shallow--;
            }

            if (aniTick < 2)
            {
                aniTick += 1 - Mathf.Lerp(0f, 0.9f, escort.viscoDance);
            }
            else
            {
                aniTick = 0;
                animation++;
                if (animation >= 12)
                {
                    animation = 0;
                }
            }
        }
    }

}