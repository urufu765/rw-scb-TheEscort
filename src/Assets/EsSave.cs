using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheEscort
{
    /// <summary>
    /// Escort Savedata for run
    /// </summary>
    public class EscortSaveDataDeathPersistent
    {

        public bool SuperWallFlipTutorial = false;
        public bool GildKillGuardianTutorial = false;
        public SpeedsterChargeStorage SpeChargeStore = new();
        public class SpeedsterChargeStorage
        {
            private readonly List<int> _speChargeStore = new();

            public int this[int index]
            {
                get
                {
                    while (index >= _speChargeStore.Count)
                    {
                        _speChargeStore.Add(0);
                    }
                    return _speChargeStore[index];
                }
                set
                {
                    if (index >= _speChargeStore.Count)
                    {
                        while (index > _speChargeStore.Count)
                        {
                            _speChargeStore.Add(0);
                        }
                        _speChargeStore.Add(value);
                    }
                    else
                    {
                        _speChargeStore[index] = value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Escort Savedata for campaign
    /// </summary>
    public class EscortSaveDataMiscWorld
    {

    }

    /// <summary>
    /// Escort Savedata for entire savefile
    /// </summary>
    public class EscortSaveDataMiscProgression
    {

    }
}