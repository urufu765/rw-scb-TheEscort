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
        public class SpeChargeStore
        {
            private readonly List<int> _SpeChargeStore = new();

            public int this[int index]
            {
                get
                {
                    if (index >= _SpeChargeStore.Count)
                    {
                        _SpeChargeStore.Add(0);
                    }
                    return _SpeChargeStore[index];
                }
                set
                {
                    if (index >= _SpeChargeStore.Count)
                    {
                        _SpeChargeStore.Add(value);
                    }
                    else
                    {
                        _SpeChargeStore[index] = value;
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