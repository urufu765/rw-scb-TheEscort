using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool Unstable;  // Are thoust an Unstable?!
        public Color UnstableColor;  // Unstable body color
        public int UnsTripTime;  // Fucker trips all the time lol
        public int UnsTripping;  // Window for the thud soundfx to happen when you trip
        public bool UnsBlinking;  // Is in a blinking state? No, not the eye blink, the jump blink!
        public int UnsBlinkCount;  // How many dashshhhshshsseess done
        public int UnsBlinkWindow;  // Moment of opportunity to chain-blink
        public int UnsBlinkCD;  // Fuck you. No more blinking for this amount of time.
        public int UnsBlinkFrame;  // 10 frame animation of the blink
        public bool UnsBlinkDifDir;  // Has player pressed different button than before?
        public bool UnsBlinkNoDir;  // Is player not pressing any directional buttons like a fokin monke?
        public float UnsDamaged;  // How chewed up is this fucker
        public (int x, int y) UnsBlinkDir;  // Direction of blink
        public Stack<Weapon> UnsMeleeWeapon;  // When Unstable melees instead of toss or throw
        public int UnsMeleeGrab;  // Regrab timer for melee attacks
        public int UnsMeleeStun;  // Cooldown for preventing Unstable from trying to throw/toss/melee immediately
        public int UnsMeleeUsed;  // The grasp hand that is used for melee toss

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