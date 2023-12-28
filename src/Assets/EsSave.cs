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
        /*ACHIEVEMENTS (POTENTIAL SPOILERS!!)*/
        // Implemented below
        public bool beaten_Escort = false;  // Beat Escort campaign (set to false on release)


        // Not Implemented below
        public bool achieveEscort_Bare_Fists = false;  // Beat Escort campaign without ever picking up a weapon
        public bool achieveEscort_All_Known = false;  // Beat Escort true ending OR complete wanderer passage in Escort campaign
        public bool achieveEscort_Peter_Pan = false;  // Bring at least 5 creatures to the void pool and achieve the ascension ending
        public bool achieveEscort_Bloodthrister = false;  // Survive 20 cycles of just meat diet
        public bool achieveEscort_Forager = false;  // Survive 20 cycles of just fruit diet
        public bool achieveEscort_Fuckin = false;  // Fall into a deathpit after a slidestun 50 times
        public bool achieveEscort_Bad_Parent = false;  // Let Socks die 10 times in a single campaign
        public bool achieveEscort_Wawa_Mothafuka = false;  // Taunt for more than 8 seconds while engaged in combat
        public bool achieveEscort_Perfectionist = false;  // Beat Escort campaign without ever missing a spear throw (wallspears count as a miss)
        public bool achieveEscort_The_Banba_Fan = false;  // Do the Banba!
        public bool achieveEscort_Dolofin = false;  // Find the Dolofin mural
        public bool achieveEscort_Favouritism = false;  // Feed the child to your favorite lizard 5 times in a single run
        public bool achieveEscort_Voids_Favour = false;  // Dropkick a Saint

        public bool achieveEscommunity_Spearfishin = false;  // Kill a leviathan in Escort campaign
        public bool achieveEscommunity_Space_Elevator = false;  // Elevator off a creature so high Escort falls to their death
        public bool achieveEscommunity_Skills_Skills_Skills = false;  // Don't ever grab onto a pole, maneuver using pole tech instead

        // Future achievements for much later dates
        // Brawler and the Great Tea Party
        public bool beaten_BrawlerEscort = false;  // Beat Escort campaign
        public bool achieveBrawler_Pipebomb = false;  // Die in the intro
        public bool achieveBrawler_Not_A_Parrot = false;  // Don't do everything the narrator says and get the Tea Ending
        public bool achieveBrawler_No_Table_Manners = false;  // Be a disappointment 30 times
        public bool achieveBrawler_Londoner = false;  // Supershank 50 times
        public bool achieveBrawler_The_Spanish = false;  // Get the defiance Ending
        public bool achieveBrawler_Outta_Time = false;  // Get Out of Time Ending
        public bool achieveBrawler_Tea_Time = false;  // Get Tea Ending

        // Deflector: Super Rainworld Saturation(consult book of stuff)
        public bool beaten_DeflectorEscort = false;  // Beat Deflector campaign
        public bool achieveDeflector_Youre_Win = false;  // Complete a run of Deflector campaign by going through all the main stages
        public bool achieveDeflector_Your_Special = false;  // Complete a run of Deflector campaign by completing at least one bonus stage
        public bool achieveDeflector_The_Ignition = false;  // Beat stage 1
        
        // Escapist:
        public bool beaten_EscapistEscort = false;  // Beat Escapist campaign
        public bool achieveEscapist_Peek_Into_Reality = false;  // Break out of the delusion, if only for a mere moment
        public bool achieveEscapist_Mission_Complete = false;  // Achieve the main ending
        public bool achieveEscapist_Undetectable = false;  // Beat Escapist campaign without being discovered once
        public bool achieveEscapist_Guns_Blazin = false;  // Beat Escapist campaign by being detected at least once in every act (or for the majority of the campaign)

        // Railgunner:
        public bool beaten_RailgunnerEscort = false;  // Beat Railgunner campaign
        public bool achieveRailgunner_Bittersweet = false;  // Achieve the main ending

        // Speedster:
        public bool beaten_SpeedsterEscort = false;  // Beat Speedster campaign
        public bool achieveSpeedster_The_Ultimate_Prize = false;  // Achieve the main ending

        // Gilded:
        public bool beaten_GildedEscort = false;  // Beat Gilded campaign
        public bool achieveGilded_Where_We_Are = false;  // Achieve the main ending

        // Barbarian:
        public bool beaten_BarbarianEscort = false;  // Beat Barbarian campaign
        public bool achieveBarbarian_The_End = false;  // Achieve the main ending
    }
}