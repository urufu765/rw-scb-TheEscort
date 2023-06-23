using System;

namespace TheEscort;

public abstract class Trackrr<T>
{
    private T _tracked;
    private T _max;
    private T _limit;
    public readonly int playerNumber;
    public readonly int trackerNumber;

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

    public Trackrr(int playerNumber, int trackerNumber)
    {
        this.playerNumber = playerNumber;
        this.trackerNumber = trackerNumber;
    }
}

public static class ETrackrr
{
    public class HypeTraction : Trackrr<float>
    {
        public Player player;
        public HypeTraction(Player player, int playerNumber, int trackerNumber, float limiter) : base( playerNumber, trackerNumber )
        {
            this.player = player;
            Max = 1f;
            Limit = limiter;
        }

        public override void UpdateTracker()
        {
            this.Value = player.aerobicLevel;
        }
    }
}