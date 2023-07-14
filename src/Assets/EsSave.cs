using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheEscort
{
    /// <summary>
    /// A simple list that expands upon having an index greater than the size
    /// </summary>
    public record ExpandableList<T>
    {
        private readonly List<T> _list = new();

        public T this[int index]
        {
            get
            {
                while (index >= _list.Count)
                {
                    _list.Add(default);
                }
                return _list[index];
            }
            set
            {
                if (index >= _list.Count)
                {
                    while (index > _list.Count)
                    {
                        _list.Add(default);
                    }
                    _list.Add(value);
                }
                else
                {
                    _list[index] = value;
                }
            }
        }
    }

    /// <summary>
    /// Escort Savedata for run (doesn't save on death)
    /// </summary>
    public class EscortSaveDataMiscWorld
    {

    }

    /// <summary>
    /// Escort Savedata for run (saves even on death)
    /// </summary>
    public class EscortSaveDataDeathPersistent
    {

        public bool SuperWallFlipTutorial = false;
        public bool GildKillGuardianTutorial = false;
        public ExpandableList<int> SpeChargeStore = new();
        public Dictionary<int, float> Storage = new();
    }

    /// <summary>
    /// Escort Savedata for entire savefile
    /// </summary>
    public class EscortSaveDataMiscProgression
    {

    }
}