using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool Barbarian;
        public Color BarbarianColor;

        public void EscortBB()
        {
            BarbarianColor = new(1f, 0, 0);
        }
    }
}