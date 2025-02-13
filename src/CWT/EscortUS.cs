using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        /// <summary>
        /// Are thoust an Unstable?!
        /// </summary>
        public bool Unstable;
        /// <summary>
        /// Unstable body colour
        /// </summary>
        public Color UnstableColor;
        /// <summary>
        /// Fucker trips all the time lol
        /// </summary>
        public int UnsTripTime;
        /// <summary>
        /// Window for the thud soundfx to happen when you trip
        /// </summary>
        public int UnsTripping;
        /// <summary>
        /// Is in a blinking state? No, not the eye blink, the jump blink!
        /// </summary>
        public bool UnsBlinking;
        /// <summary>
        /// How many dashshhhshshsseess done
        /// </summary>
        public int UnsBlinkCount;
        /// <summary>
        /// Moment of opportunity to chain-blink
        /// </summary>
        public int UnsBlinkWindow;
        /// <summary>
        /// Fuck you. No more blinking for this amount of time
        /// </summary>
        public int UnsBlinkCD;
        /// <summary>
        /// 10 frame animation of the blink
        /// </summary>
        public int UnsBlinkFrame;
        /// <summary>
        /// Has player pressed different button than before?
        /// </summary>
        public bool UnsBlinkDifDir;
        /// <summary>
        /// Is player not pressing any directional buttons like a fokin monke?
        /// </summary>
        public bool UnsBlinkNoDir;
        /// <summary>
        /// How chewed up is this fucker
        /// </summary>
        public float UnsDamaged;
        /// <summary>
        /// Direction of blink
        /// </summary>
        public (int x, int y) UnsBlinkDir;
        /// <summary>
        /// When Unstable melees instead of toss or throw
        /// </summary>
        public Stack<Weapon> UnsMeleeWeapon;
        /// <summary>
        /// Regrab timer for melee attacks
        /// </summary>
        public int UnsMeleeGrab;
        /// <summary>
        /// Cooldown for preventing Unstable from trying to throw/toss/melee immediately
        /// </summary>
        public int UnsMeleeStun;
        /// <summary>
        /// The grasp hand that is used for melee toss
        /// </summary>
        public int UnsMeleeUsed;
        /// <summary>
        /// Prevents Unstable from sliding if more than 0
        /// </summary>
        public int UnsFuckYourSlide;
        /// <summary>
        /// The creature to home into
        /// </summary>
        public Creature UnsRockitCret;
        /// <summary>
        /// Let homing last for 1.5 seconds before canceling
        /// </summary>
        public int UnsRockitDur;

        // Constants for Unstable Rockit Kick values
        /// <summary>
        /// Unstable Rockit Kick Damage multiplier
        /// </summary>
        public const float UnsRKDx = 0.25f;
        /// <summary>
        /// Unstable Rockit Kick Stun multiplier
        /// </summary>
        public const int UnsRKSx = 15;

        /// <summary>
        /// Despite everything, this is not a self contained init function. Only to be used as a method in the main Escort class
        /// </summary>
        public void EscortUS()
        {
            Unstable = false;
            UnstableColor = new Color(0.176f, 0.42f, 0.176f);
            UnsTripTime = 0;
            UnsBlinking = false;
            UnsBlinkCount = 0;
            UnsBlinkWindow = 0;
            UnsBlinkCD = 0;
            UnsBlinkFrame = 0;
            UnsBlinkDifDir = false;
            UnsBlinkNoDir = false;
            UnsDamaged = 0.0f;
            UnsBlinkDir = (0, 0);
            UnsMeleeWeapon = new Stack<Weapon>(1);
            UnsMeleeGrab = -1;
            UnsMeleeStun = 0;
            UnsMeleeUsed = -1;
        }
    }
}