using System;
using UnityEngine;

namespace TheEscort;

public abstract class Trackrr<T>
{
    private T _tracked;
    private T _max;
    private T _limit;
    public readonly int playerNumber;
    public readonly int trackerNumber;
    public readonly string trackerName;
    public Color trackerColor;
    public Color effectColor;
    public bool force;

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

    public abstract void DrawTracker(float timeStacker);

    public virtual void UpdateTracker()
    {
    }

    public Trackrr(int playerNumber, int trackerNumber, string trackerName, Color trackerColor = default)
    {
        this.playerNumber = playerNumber;
        this.trackerNumber = trackerNumber;
        this.trackerName = trackerName;
        this.trackerColor = trackerColor;
        this.effectColor = trackerColor;
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
        public HypeTraction(int playerNumber, int trackerNumber, float limiter, Player player, Escort e) : base( playerNumber, trackerNumber, "hype")
        {
            this.player = player;
            this.e = e;
            Max = 1f;
            Limit = limiter;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = player.aerobicLevel;
            force = Value > Limit;
            this.trackerColor = e.hypeColor;
            this.effectColor = Color.Lerp(Color.white, e.hypeColor, 0.7f);
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
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Max = e.BrawLastWeapon switch {
                "shank" => 60,
                _ => 40
            };
            this.Value = Max - player.slowMovementStun;
            this.Limit = player.slowMovementStun > 0? 0 : 1000;
            this.force = player.slowMovementStun > 0;
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
            this.Value = e.DeflAmpTimer;
            this.Limit = e.DeflPowah == 3? 0 : 160;
            this.force = e.DeflAmpTimer > 0;
        }
    }

    public class EscapistUngraspTraction : Trackrr<float>
    {
        private readonly Escort e;
        private readonly Color escapistColor;
        private float transitioning;
        private int prevMax;
        public EscapistUngraspTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "escapist")
        {
            this.e = escort;
            this.escapistColor = new Color(0.42f, 0.75f, 0.1f);
        }

        public override void DrawTracker(float timeStacker)
        {
            if (e.EscUnGraspCD == 0)
            {
                this.Limit = 0;
                this.trackerColor = escapistColor;
                if (e.EscUnGraspLimit != 0) prevMax = e.EscUnGraspLimit;
                this.Max = prevMax;
                this.Value = Mathf.Lerp(0, e.EscUnGraspTime, Mathf.InverseLerp(0, 20, transitioning));

                if (e.EscUnGraspLimit == 0 && transitioning > 0) {
                    this.transitioning -= timeStacker;
                }
                else if (e.EscUnGraspLimit > 0 && transitioning < 20) {
                    this.transitioning += timeStacker;
                }
            }
            else
            {
                this.Limit = 500;
                this.Max = 480;
                this.Value = e.EscUnGraspCD;
                this.trackerColor = Color.Lerp(Color.Lerp(escapistColor, Color.black, 0.6f), escapistColor, Mathf.InverseLerp(0, 40, e.EscUnGraspCD));
            }
        }
    }

    public class RailgunnerCDTraction : Trackrr<float>
    {
        private readonly Player player;
        private readonly Escort e;

        public RailgunnerCDTraction(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "RailgunnerCD", new Color(0.5f, 0.85f, 0.78f))
        {
            this.player = player;
            this.e = escort;
            this.Max = 800;
        }

        public override void DrawTracker(float timeStacker)
        {
            this.Value = e.RailgunCD;
            this.Limit = player.Malnourished? 0 : 1000;
        }
    }

    public class RailgunnerUsageTraction : Trackrr<float>
    {
        private float preValue;
        private float setValue;
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
                preValue = setValue;
            }
            setValue = Mathf.Min(e.RailgunUse, e.RailgunLimit);

            // Smoothly transition the value with an ease out
            this.Value = preValue < setValue? Mathf.Lerp(preValue, setValue, Mathf.Log(transitioning, 10)) :  Mathf.Lerp(setValue, preValue, Mathf.Log(transitioning, 10));

            // Advance the transition, or reset transition ticker
            if (this.Value == setValue) {
                preValue = setValue;
                transitioning = 0;
            }
            else {
                transitioning += timeStacker;
            }
        }
    }

    public class SpeedsterTraction : Trackrr<float>
    {
        private readonly Escort e;
        private readonly int gear;
        private float oldValue;
        private float transitioning;
        public SpeedsterTraction(int playerNumber, int trackerNumber, Escort escort, int gear) : base(playerNumber, trackerNumber, "speedster", new Color(0.76f, 0.78f, 0f))
        {
            this.e = escort;
            this.gear = gear;
        }

        public override void DrawTracker(float timeStacker)
        {
            if (e.SpeDashNCrash)
            {
                Max = e.SpeExtraSpe;
                Limit = 0;
                if (gear <= e.SpeGear + 1){
                    Value = e.SpeSpeedin;
                }
                else 
                {
                    transitioning += timeStacker;
                    Value = Mathf.Lerp(oldValue, 0, Mathf.InverseLerp(0, 20, transitioning));
                }
            }
            else 
            {
                this.Max = 240;
                if (e.SpeCharge == gear - 1)
                {
                    Value = e.SpeBuildup;
                    Limit = 300;
                }
                else if (e.SpeCharge >= gear)
                {
                    Value = Max;
                    Limit = 0;
                }
                else {
                    Value = 0;
                }
                transitioning = 0;
                oldValue = Value;
            }
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
                Value = escort.SpeSecretSpeed? escort.SpeSpeedin * 2 : escort.SpeExtraSpe;
            }
            else 
            {
                this.effectColor = escort.SpeSecretSpeed? Color.Lerp(trackerColor, Color.white, 0.35f) : Color.Lerp(trackerColor, Color.black, 0.35f);
                Limit = escort.SpeDashNCrash? 0 : Max;
                Value = escort.SpeSpeedin;
            }
        }
    }
}