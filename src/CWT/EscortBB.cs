using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        /// <summary>
        /// Is this Barbarian? YES OR NO ANSWER THE GODDAMN QUESTION YOU PLEB
        /// </summary>
        public bool Barbarian;

        /// <summary>
        /// THe color of barbarian
        /// </summary>
        public Color BarbarianColor;

        /// <summary>
        /// True or false whether creature is being held
        /// </summary>
        public bool BarHasCretin;

        /// <summary>
        /// Which hand is the cretin in?
        /// </summary>
        public int BarWhichCretin;

        /// <summary>
        /// True or false whether that held creature is a PLAYER (a simple check). Might make this a property if need be but for now, for performance purposes, this will have to do
        /// </summary>
        public bool BarFkingCretin;

        /// <summary>
        /// Reference to the Cretin!
        /// </summary>
        public Creature BarCretin;

        /// <summary>
        /// How many times a different player (presumably in arena mode) tries to wiggle out of Barbarian's grasp
        /// </summary>
        public int BarWiggle;

        /// <summary>
        /// Delay before shield is active (hold Grab for 10-20 frames maybe)
        /// </summary>
        public int BarShieldDelay;

        /// <summary>
        /// Delay before delay kicks in due to stun application (needs to hook into Creature.Stun())
        /// </summary>
        public int BarShieldStunDelay;

        /// <summary>
        /// -1 if shielding left side, 1 if shielding right side, 0 if not shielding
        /// </summary>
        public int BarShieldState;

        public void EscortBB(Player player)
        {
            BarbarianColor = new(1f, 0, 0);  // Fukin red for now
            BarHasCretin = false;
            BarWhichCretin = -1;
            BarFkingCretin = false;
            BarCretin = null;
            BarWiggle = 0;
            BarShieldDelay = 0;
            BarShieldStunDelay = 0;
            BarShieldState = 0;
        }
    }
}