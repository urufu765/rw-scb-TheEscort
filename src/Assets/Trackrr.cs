using System;

namespace TheEscort;

public abstract class Trackrr<T>
{
    private T _tracked;
    private T _max;
    private T _limit;
    public readonly int playerNumber;
    public readonly int trackerNumber;
    public readonly string trackerName;
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

    public Trackrr(int playerNumber, int trackerNumber, string trackerName)
    {
        this.playerNumber = playerNumber;
        this.trackerNumber = trackerNumber;
        this.trackerName = trackerName;
    }
}

public static class ETrackrr
{
    public class TestTractoin : Trackrr<float>
    {
        public TestTractoin(int playerNumber, int trackerNumber) : base(playerNumber, trackerNumber, "test")
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

    public class TestTractoin2 : Trackrr<float>
    {
        public TestTractoin2(int playerNumber, int trackerNumber) : base(playerNumber, trackerNumber, "test")
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

    public class HypeTraction : Trackrr<float>
    {
        public Player player;
        public HypeTraction(Player player, int playerNumber, int trackerNumber, float limiter) : base( playerNumber, trackerNumber, "hype")
        {
            this.player = player;
            Max = 1f;
            Limit = limiter;
        }

        public override void UpdateTracker()
        {
            this.Value = player.aerobicLevel;
            force = Value > Limit;
        }
    }


}