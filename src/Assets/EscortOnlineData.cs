using System;
using JetBrains.Annotations;
using RainMeadow;
using static TheEscort.Eshelp;

namespace TheEscort;

public class EscortOnlineData : OnlineEntity.EntityData
{
    [UsedImplicitly]
    public EscortOnlineData()
    {
    }
    public override EntityDataState MakeState(OnlineEntity entity, OnlineResource inResource)
    {
        return new State(entity);
    }
    public class State : EntityDataState
    {
        [OnlineField]
        public int buildId;

        #region Lazy Sync
        /// <summary>
        /// For audio purposes idk
        /// </summary>
        [OnlineField]
        public float RollinCount { get; set; }
        /// <summary>
        /// Sync global Deflector perma based on host's settings (figure this one out later)
        /// </summary>
        // [OnlineField]
        // public float DeflSharedPerma { get; set; }
        [OnlineField]
        public int GildPower { get; set; }
        #endregion

        #region One-time sync
        /// <summary>
        /// Sync railgunner limit based on the value set in the individual's remix settings
        /// </summary>
        [OnlineField]
        public int RailgunLimit { get; set; }
        [OnlineField]
        public int SpeMaxGear { get; set; }
        [OnlineField]
        public int SpeCharge { get; set; }
        [OnlineField]
        public int SpeSpeedin {get;set;} 
        [OnlineField]
        public bool SpeOldSpeed {get;set;}
        [OnlineField]
        public int GildPowerMax { get; set; }
        #endregion

        [UsedImplicitly]
        public State()
        {
        }

        public State(OnlineEntity oe)
        {
            // Ebug("State creation");
            if ((oe as OnlinePhysicalObject)?.apo?.realizedObject is not Player player) return;
            if (!Plugin.eCon.TryGetValue(player, out Escort e)) return;
            buildId = FindBuildIndex(e);
            // if (e.Deflector)
            // {
            //     DeflSharedPerma = e.DeflPerma;
            // }
            RollinCount = e.RollinCount;
            if (e.Speedster)
            {
                SpeMaxGear = e.SpeMaxGear;
                SpeCharge = e.SpeCharge;
                SpeSpeedin = e.SpeSpeedin;
                SpeOldSpeed = e.SpeOldSpeed;
            }
            if (e.Railgunner)
            {
                RailgunLimit = e.RailgunLimit;
            }
            if (e.Gilded)
            {
                GildPower = e.GildPower;
                GildPowerMax = e.GildPowerMax;
            }
            // Ebug("Save value " + buildId);
            // if (oe?.owner?.id?.DisplayName is string s)
            // {
            //     Ebug("From: " + s + "|" + e.GetHashCode());
            // }
        }

        public override void ReadTo(OnlineEntity.EntityData data, OnlineEntity onlineEntity)
        {
            // Ebug("Reading", LogLevel.MESSAGE);
            if ((onlineEntity as OnlinePhysicalObject)?.apo is not AbstractCreature ac)
            {
                return;
            }

            if (ac.realizedCreature is Player p && Plugin.eCon.TryGetValue(p, out Escort e))
            {
                SetBuildFromIndex(ref e, buildId);
                if (e.PleaseSyncMyUnimportantValues)
                {
                    e.RollinCount = RollinCount;
                    if (e.Gilded)
                    {
                        e.GildPower = GildPower;
                    }
                }
                if (e.PleaseSyncMyOneTimeValues)
                {
                    if (e.Railgunner)
                    {
                        e.RailgunLimit = RailgunLimit;
                    }
                    if (e.Speedster)
                    {
                        e.SpeMaxGear = SpeMaxGear;
                        e.SpeCharge = SpeCharge;
                        e.SpeSpeedin = SpeSpeedin;
                        e.SpeOldSpeed = SpeOldSpeed;
                    }
                    if (e.Gilded)
                    {
                        e.GildPowerMax = GildPowerMax;
                    }
                }
            }
            else
            {
                AbstractEscort ae = Plugin.aCon.GetOrCreateValue(ac);

                ae.buildId = this.buildId;
            }
            // Ebug("Set value " + buildId, LogLevel.MESSAGE);

            // if (onlineEntity?.owner?.id?.DisplayName is string s)
            // {
            //     Ebug("To: " + s);
            // }
        }

        public static void SetBuildFromIndex(ref Escort e, int i)
        {
            e.isDefault = e.Brawler = e.Deflector = e.Escapist = e.Railgunner = e.Speedster = e.Gilded = e.Barbarian = e.Unstable = false;
            switch (i)
            {
                case -1: e.Brawler = true; return;
                case -2: e.Deflector = true; return;
                case -3: e.Escapist = true; return;
                case -4: e.Railgunner = true; return;
                case -5: e.Speedster = true; return;
                case -6: e.Gilded = true; return;
                case -7: e.Barbarian = true; return;
                case -8: e.Unstable = true; return;
                // case -9: e.Brawler = true; return;
                default: e.isDefault = true; return;
            }
        }

        public static int FindBuildIndex(in Escort e)
        {
            if (e.Brawler) return -1;
            if (e.Deflector) return -2;
            if (e.Escapist) return -3;  // Yeah not letting new Escapist work on Meadow, too janky to fix.
            if (e.Railgunner) return -4;
            if (e.Speedster) return -5;
            if (e.Gilded) return -6;
            if (e.Barbarian) return -7;
            if (e.Unstable) return -8;
            // if (e.) return -9;
            return 0;
        }

        public override Type GetDataType()
        {
            return typeof(EscortOnlineData);
        }
    }
}
