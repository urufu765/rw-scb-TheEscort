using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IL.Menu;

namespace TheEscort
{
    /// <summary>
    /// A simple list that expands upon having an index greater than the size
    /// </summary>
    public class ExpandableList<T> : List<T>
    {
        private readonly List<T> _list = new();

        public new T this[int index]
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
    public record EscortSaveDataMiscWorld
    {
        public Dictionary<int, int> SpeChargeStore {get; set;} = new();
        public Dictionary<int, float> DeflPermaDamage {get; set;} = new();
        public bool RespawnPupReady {get; set;} = false;
        public bool AltRespawnReady {get; set;} = false;
        public bool HackPupSpawn {get; set;} = false;
        public bool EscortPupEncountered {get; set;} = false;
        public float EscortPupLike {get; set;} = -1;
        public float EscortPupTempLike {get; set;} = -1;
        public int EscortPupCampaignID {get; set;} = 0;
        public bool SocksIsAlive {get; set;} = false;
    }


    /// <summary>
    /// Escort Savedata for tutorials (death persistent)
    /// </summary>
    public class EscortTutorial : DeathPersistentSaveData.Tutorial
    {
        public static readonly EscortTutorial SuperWallFlip = new("EscorTutorialSuperWallFlip", true);
        public static readonly EscortTutorial GildKillGuardian = new("EscorTutorialGildedKillAGuardian", true);
        public static readonly EscortTutorial EscortPupRespawned = new("EscorTutorialPupRespawn", true);
        public static readonly EscortTutorial EscortPupRespawnedNotify = new("EscorTutorialPupRespawnNote", true);
        public static readonly EscortTutorial EscortAltPupRespawned = new("EscorTutorialAltPupRespawn", true);
        public static readonly EscortTutorial EscortAltPupRespawnedNotify = new("EscorTutorialAltPupRespawnNote", true);

        public EscortTutorial(string value, bool register = false) : base(value, register)
        {
        }
    }


    /// <summary>
    /// Escort Savedata for entire savefile
    /// </summary>
    public record EscortSaveDataMiscProgression
    {
    }
}