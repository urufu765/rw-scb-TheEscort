using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public int UnsTripTime;
        public int UnsBlinkCount;
        public int UnsBlinkWindow;
        public float UnsDamaged;

        public void EscortUS()
        {
            UnsTripTime = 0;
            UnsBlinkCount = 0;
            UnsBlinkWindow = 0;
            UnsDamaged = 0.0f;
        }
    }
}