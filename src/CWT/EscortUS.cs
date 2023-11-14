using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool Unstable;  // Are thoust an Unstable?!
        public int UnsTripTime;  // Fucker trips all the time lol
        public bool UnsBlinking;  // Is in a blinking state? No, not the eye blink, the jump blink!
        public int UnsBlinkCount;  // How many dashshhhshshsseess done
        public int UnsBlinkWindow;  // Moment of opportunity to chain-blink
        public int UnsBlinkCD;  // Fuck you. No more blinking for this amount of time.
        public int UnsBlinkFrame;  // 10 frame animation of the blink
        public bool UnsBlinkDifDir;  // Has player pressed different button than before?
        public bool UnsBlinkNoDir;  // Is player not pressing any directional buttons like a fokin monke?
        public float UnsDamaged;  // How chewed up is this fucker
        public (int x, int y) UnsBlinkDir;  // Direction of blink

        public void EscortUS()
        {
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
        }
    }
}