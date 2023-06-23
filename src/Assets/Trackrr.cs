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

    public abstract void UpdateTracker();

    public Trackrr(int playerNumber, int trackerNumber, string trackerName, Color trackerColor = default)
    {
        this.playerNumber = playerNumber;
        this.trackerNumber = trackerNumber;
        this.trackerName = trackerName;
        this.trackerColor = trackerColor;
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

        public override void UpdateTracker()
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

        public override void UpdateTracker()
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
        public Player player;
        public Escort e;
        public HypeTraction(int playerNumber, int trackerNumber, float limiter, Player player, Escort e) : base( playerNumber, trackerNumber, "hype")
        {
            this.player = player;
            this.e = e;
            Max = 1f;
            Limit = limiter;
        }

        public override void UpdateTracker()
        {
            this.Value = player.aerobicLevel;
            force = Value > Limit;
            this.trackerColor = e.hypeColor;
        }
    }

    /// <summary>
    /// Brawler's melee attack cooldown
    /// </summary>
    public class BrawlerMeleeTraction : Trackrr<float>
    {
        public Player player;
        public Escort e;
        public BrawlerMeleeTraction(int playerNumber, int trackerNumber, Player player, Escort escort) : base(playerNumber, trackerNumber, "brawler", new Color(0.8f, 0.4f, 0.6f))
        {
            this.player = player;
            this.e = escort;
        }

        public override void UpdateTracker()
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
        public Escort e;
        public DeflectorEmpowerTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "deflector", new Color(0.69f, 0.55f, 0.9f))
        {
            this.e = escort;
            this.Max = 320;
        }

        public override void UpdateTracker()
        {
            this.Value = e.DeflAmpTimer;
            this.Limit = e.DeflPowah == 3? 0 : 160;
            this.force = e.DeflAmpTimer > 0;
        }
    }

    public class EscapistUngraspTraction : Trackrr<float>
    {
        public Escort e;
        private readonly Color escapistColor;
        private int transitioning;
        private int prevMax;
        public EscapistUngraspTraction(int playerNumber, int trackerNumber, Escort escort) : base(playerNumber, trackerNumber, "escapist")
        {
            this.e = escort;
            this.escapistColor = new Color(0.42f, 0.75f, 0.1f);
        }

        public override void UpdateTracker()
        {
            if (e.EscUnGraspCD == 0)
            {
                this.Limit = 0;
                this.trackerColor = escapistColor;
                if (e.EscUnGraspLimit != 0) prevMax = e.EscUnGraspLimit;
                this.Max = prevMax;
                this.Value = Mathf.Lerp(0, e.EscUnGraspLimit - e.EscUnGraspTime, Mathf.InverseLerp(0, 60, transitioning));

                if (e.EscUnGraspLimit == 0 && transitioning > 0) {
                    this.transitioning--;
                }
                else if (e.EscUnGraspLimit > 0 && transitioning < 60) {
                    this.transitioning++;
                }
            }
            else
            {
                this.Limit = 500;
                this.Max = 480;
                this.Value = this.Max - e.EscUnGraspCD;
                this.trackerColor = Color.Lerp(escapistColor, Color.Lerp(escapistColor, Color.black, 0.4f), Mathf.InverseLerp(0, 60, e.EscUnGraspCD));
            }
        }
    }
}