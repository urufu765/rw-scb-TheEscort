using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool Unstable;
        public int UnsTripTime;
        public bool UnsBlinking;
        public int UnsBlinkCount;
        public int UnsBlinkWindow;
        public int UnsBlinkCD;
        public int UnsBlinkFrame;
        public bool UnsBlinkDifDir;
        public float UnsDamaged;
        public (int x, int y) UnsBlinkDir;

        public void EscortUS()
        {
            UnsTripTime = 0;
            UnsBlinking = false;
            UnsBlinkCount = 0;
            UnsBlinkWindow = 0;
            UnsBlinkCD = 0;
            UnsBlinkFrame = 0;
            UnsBlinkDifDir = false;
            UnsDamaged = 0.0f;
            UnsBlinkDir = (0, 0);
        }
    }
}