using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        /// <summary>
        /// Is this Conquerer? YES OR NO ANSWER THE GODDAMN QUESTION YOU PLEB
        /// </summary>
        public bool Conqueror;

        /// <summary>
        /// THe color of barbarian
        /// </summary>
        public Color ConquererColor;

        /// <summary>
        /// True or false whether creature is being held
        /// </summary>
        public bool ConHasCretin;

        /// <summary>
        /// Which hand is the cretin in?
        /// </summary>
        public int ConWhichCretin;

        /// <summary>
        /// True or false whether that held creature is a PLAYER (a simple check). Might make this a property if need be but for now, for performance purposes, this will have to do
        /// </summary>
        public bool ConFkingCretin;

        /// <summary>
        /// Reference to the Cretin!
        /// </summary>
        public Creature ConCretin;

        /// <summary>
        /// How long before the cretin can escape
        /// </summary>
        public int ConCretinFredom;

        /// <summary>
        /// How many times a different player (presumably in arena mode) tries to wiggle out of Conquerer's grasp
        /// </summary>
        public int ConWiggle;

        /// <summary>
        /// Delay before shield is active (hold Grab for 10-20 frames maybe)
        /// </summary>
        public int ConShieldDelay;

        /// <summary>
        /// Delay before delay kicks in due to stun application (needs to hook into Creature.Stun())
        /// </summary>
        public int ConShieldStunDelay;

        /// <summary>
        /// -1 if shielding left side, 1 if shielding right side, 0 if not shielding
        /// </summary>
        public int ConShieldState;

        /// <summary>
        /// Maximum number of ticks Conqueror can hold the cretin
        /// </summary>
        public int ConMaximumHold;
        
        /// <summary>
        /// Cooldown for invincibility
        /// </summary>
        public int ConInviParryCD;

        public void EscortCQ(Player player, int maxHold = 1200)
        {
            ConquererColor = new(1f, 0, 0);  // Fukin red for now
            ConHasCretin = false;
            ConWhichCretin = -1;
            ConFkingCretin = false;
            ConCretin = null;
            ConWiggle = 0;
            ConShieldDelay = 0;
            ConShieldStunDelay = 0;
            ConShieldState = 0;
            ConMaximumHold = maxHold;
        }
    }
}