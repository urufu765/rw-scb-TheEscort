using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public enum SpeedVersion
    {
        Restless,
        Vroom,
        Speedway,
        Turbocharged
    }
    public partial class Escort
    {
        public bool Speedster;
        public Color SpeedsterColor;
        public int SpeSpeedin;
        public bool SpeDashNCrash;
        public Queue<SpeedTrail> SpeTrail;
        public int SpeTrailTick;
        public bool SpeSecretSpeed;
        public int SpeExtraSpe;
        public Color SpeColor;
        public int SpeBonk;
        public float SpeBuildup;
        public int SpeGear;
        public int SpeCharge;
        public float SpeGain;
        public bool SpeOldSpeed;
        public int SpeRollCounter;
        public int SpeMaxGear;
        public int SpeNitrosX;
        public int SpeNitrosY;
        /// <summary>
        /// Makes Speedster slide on walls
        /// </summary>
        public Vector2? SpeMomentumJump;
        /// <summary>
        /// Speedster residual momentum counter
        /// </summary>
        public int SpeResimo;
        public SpeedVersion SpeVersion;
        /// <summary>
        /// For keeping track of how long Speedster is boosting through corridor or sliding
        /// </summary>
        public int SpeBoosterTunneling;
        /// <summary>
        /// Max time for Speedster to be boostin
        /// </summary>
        public int SpeTimeLimit
        {
            get
            {
                if (karmaTen) return (int)(field * 1.34) * 40;
                return field * 40;
            }
            set;
        }
        public int SpeLastBonusTime;
        public bool SpeNitrosBoost;
        public const float SpeSpeedwayReady = 300;
        public const float SpeSpeedwayExtra = 200;
        public const float SpeSpeedwayExtraDifficulty = 10;
        public const float SpeNitrosMax = 40;

        public void EscortSS(int maxGear = 4, SpeedVersion speedVersion = SpeedVersion.Turbocharged)
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
            this.SpeOldSpeed = speedVersion == SpeedVersion.Restless;
            this.SpeMaxGear = maxGear;
            this.SpeNitrosX = 0;
            this.SpeNitrosY = 0;
            this.SpeResimo = 0;
            this.SpeMomentumJump = null;
            this.SpeVersion = speedVersion;
            this.SpeBoosterTunneling = 0;
            this.SpeTimeLimit = 15;
        }

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
            switch (this.SpeVersion)
            {
                case SpeedVersion.Restless:
                    this.SpeTrail.Enqueue(new SpeedTrail(rCam, s, this.SpeSecretSpeed ? Color.white : this.hypeColor, this.SpeSecretSpeed ? this.hypeColor : Color.black, life));
                    break;
                case SpeedVersion.Vroom:
                    this.SpeTrail.Enqueue(new SpeedTrail(rCam, s, Color.Lerp(this.hypeColor, Color.white, this.SpeGear * 0.33f), Color.Lerp(Color.black, this.hypeColor, this.SpeGear * 0.33f), life));
                    break;
                case SpeedVersion.Speedway:
                    Color colur;
                    if (this.SpeBoosterTunneling > 0)
                    {
                        colur = Color.Lerp(this.SpeColor, Color.white, .5f);
                    }
                    else
                    {
                        colur = Color.Lerp(this.hypeColor, Color.black, Mathf.InverseLerp(0, this.SpeTimeLimit, this.SpeSpeedin));
                    }
                    this.SpeTrail.Enqueue(new SpeedTrail(rCam, s, colur, colur, life));
                    break;
            }
        }

        public bool Escat_Nitros_X()
        {
            SpeNitrosBoost = true;
            if (SpeNitrosX > 0)
            {
                SpeNitrosX--;
                return true;
            }
            return false;
        }
        public bool Escat_Nitros_Y()
        {
            SpeNitrosBoost = true;
            if (SpeNitrosY > 0)
            {
                SpeNitrosY--;
                return true;
            }
            return false;
        }


        /*
        public void Escat_addSpeTrail(RoomCamera.SpriteLeaser s, RoomCamera roomCamera, int life, int trailCount = 10)
        {
            if (SpeTrail2.Count >= trailCount)
            {
                SpeedsTrail trail = SpeTrail2.Dequeue();
                if (!trail.killed)
                {
                    trail.Kill();
                }
            }
            SpeedsTrail st = null;
            if (SpeOldSpeed)
            {
                st = new SpeedsTrail(this.SpeSecretSpeed ? this.hypeColor : Color.black, life, this.SpeSecretSpeed ? Color.white : this.hypeColor);
            }
            else
            {
                st = new SpeedsTrail(Color.Lerp(Color.black, this.hypeColor, this.SpeGear * 0.33f), life, Color.Lerp(this.hypeColor, Color.white, this.SpeGear * 0.33f));
            }
            st?.InitiateSprites(s, roomCamera);
            st?.AddToContainer(s, roomCamera, null);
            SpeTrail2.Enqueue(st);
        }

        public void Escat_updateSpeTrail()
        {
            foreach(var slugcatTrail in SpeTrail2)
            {
                if (!slugcatTrail.killed)
                {
                    slugcatTrail.Update();
                }
            }
        }*/

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