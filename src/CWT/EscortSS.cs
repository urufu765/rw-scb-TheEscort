using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        /// <summary>
        /// Can't believe it's Speedster
        /// </summary>
        public bool Speedster;
        /// <summary>
        /// Colour of the Speedster
        /// </summary>
        public Color SpeedsterColor;
        /// <summary>
        /// Speedster speeding duration
        /// </summary>
        public int SpeSpeedin;
        /// <summary>
        /// Whether Speedster is in speeding mode
        /// </summary>
        public bool SpeDashNCrash;
        /// <summary>
        /// A trail of Speedster afterimages
        /// </summary>
        public Queue<SpeedTrail> SpeTrail;
        /// <summary>
        /// Tick duration between each Speedster afterimage
        /// </summary>
        public int SpeTrailTick;
        /// <summary>
        /// Whether Speedster is in second gear or not (only used in old speedster)
        /// </summary>
        public bool SpeSecretSpeed;
        /// <summary>
        /// Used either to track (for TRACKRR) the Speedster's maximum duration or to track the value of old Speedster's second gear
        /// </summary>
        public int SpeExtraSpe;
        /// <summary>
        /// Speedster afterimage trail colour
        /// </summary>
        public Color SpeColor;
        /// <summary>
        /// Delay counter before Speedster is stunned from hitting a terrain too hard
        /// </summary>
        public int SpeBonk;
        /// <summary>
        /// Value of Speedster's charge buildup
        /// </summary>
        public float SpeBuildup;
        /// <summary>
        /// Speedster's current gear
        /// </summary>
        public int SpeGear;
        /// <summary>
        /// Speedster's current charge
        /// </summary>
        public int SpeCharge;
        /// <summary>
        /// Rate of speed charge buildup
        /// </summary>
        public float SpeGain;
        /// <summary>
        /// Whether to use old Speedster's abilities
        /// </summary>
        public bool SpeOldSpeed;
        /// <summary>
        /// Maximum gear Speedster can reach
        /// </summary>
        public int SpeMaxGear;
        /// <summary>
        /// Acceleration boost to Speedster when he's speeding
        /// </summary>
        public int SpeNitros;
        /// <summary>
        /// Locks the decrease of Speedster's speeding duration
        /// </summary>
        public bool SpeLockLevel;
        /// <summary>
        /// (Used for challenge) Whether Speedster is standing there, menacingly
        /// </summary>
        public int SpeStandingStill;
        /// <summary>
        /// (Used for challenge) When Speedster stood still for too long
        /// </summary>
        public bool SpeStoodStill;
        /// <summary>
        /// (Used for challenge) forgot what this was used for
        /// </summary>
        public int SpeTimer;

        /// <summary>
        /// Despite everything, this is not a self contained init function. Only to be used as a method in the main Escort class
        /// </summary>
        /// <param name="useOld">Set Speedster to old version</param>
        /// <param name="maxGear">Set gear limit</param>
        public void EscortSS(bool useOld = false, int maxGear = 4)
        {
            this.Speedster = false;
            this.SpeedsterColor = new Color(0.03f, 0.57f, 0.59f);
            this.SpeSpeedin = 0;
            this.SpeExtraSpe = 0;
            this.SpeDashNCrash = false;
            this.SpeSecretSpeed = false;
            this.SpeTrail ??= new Queue<SpeedTrail>();
            //this.SpeTrail2 ??= new Queue<SpeedsTrail>();
            this.SpeTrailTick = 0;
            this.SpeColor = new Color(0.76f, 0.78f, 0f);
            this.SpeBonk = 0;
            this.SpeBuildup = 0f;
            this.SpeGear = 0;
            this.SpeCharge = 0;
            this.SpeGain = -1f;
            this.SpeOldSpeed = useOld;
            this.SpeMaxGear = maxGear;
        }

        /// <summary>
        /// Creates and manages Speedster's afterimage trail
        /// </summary>
        /// <param name="rCam">Inherited romcam from predecessor method</param>
        /// <param name="s">Inherited sprite leaser from predecessor method</param>
        /// <param name="life">Sets max life duration of an afterimage</param>
        /// <param name="trailCount">Maximum number of afterimage trails</param>
        public void Escat_addTrail(RoomCamera rCam, RoomCamera.SpriteLeaser s, int life, int trailCount = 10)
        {
            if (this.SpeTrail.Count >= trailCount)
            {
                SpeedTrail trail = this.SpeTrail.Dequeue();
                if (!trail.killed)
                {
                    trail.Kill();
                }
            }
            if (this.SpeOldSpeed)
            {
                this.SpeTrail.Enqueue(new SpeedTrail(rCam, s, this.SpeSecretSpeed ? Color.white : this.hypeColor, this.SpeSecretSpeed ? this.hypeColor : Color.black, life));
            }
            else
            {
                this.SpeTrail.Enqueue(new SpeedTrail(rCam, s, Color.Lerp(this.hypeColor, Color.white, this.SpeGear * 0.33f), Color.Lerp(Color.black, this.hypeColor, this.SpeGear * 0.33f), life));
            }
        }


        /// <summary>
        /// Updates every living Speedster afterimage
        /// </summary>
        public void Escat_showTrail()
        {
            foreach (SpeedTrail trail in this.SpeTrail)
            {
                if (trail.killed)
                {
                    continue;
                }
                trail.Update();
            }
        }
    }
}