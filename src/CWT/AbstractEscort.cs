using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RWCustom;
using TheEscort.VengefulLizards;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort
{
    public class AbstractEscort
    {
        public int buildId;
        // public int ReadyForRespawn{get;set;}

        // public Escort HoldMyEscort{get;set;}

        // public AbstractCreature HoldMyAbstract{get;set;}

        // public World HoldMyWorld{get;set;}


        public AbstractEscort()
        {
            buildId = 0;
            // ReadyForRespawn = -1;
            // HoldMyEscort = null;
            // HoldMyAbstract = null;
        }


        // public void TryRespawnReady(in AbstractCreature p, in World w, in Escort e)
        // {
        //     if (HoldMyEscort is null)
        //     {
        //         ReadyForRespawn = 40;
        //         HoldMyAbstract = p;
        //         HoldMyWorld = w;
        //         HoldMyEscort = e;
        //     }
        // }

        // public void TryRespawnEscort()
        // {
        //     if (HoldMyAbstract is not null && HoldMyEscort is not null && HoldMyWorld is not null)
        //     {
        //         if (HoldMyAbstract.realizedCreature is null || HoldMyAbstract.realizedCreature.slatedForDeletetion)
        //         {
        //             HoldMyAbstract.realizedCreature = new Player(HoldMyAbstract, HoldMyWorld);
                    
        //         }
        //     }
        // }
    }
    // public class Type : ExtEnum<Type>
    // {
    //     public static readonly Type Default = new("EscortMe");
    //     public static readonly Type Brawler = new("EscortBriish");
    //     public static readonly Type Deflector = new("EscortGamer");
    //     public static readonly Type Escapist = new("EscortHax");
    //     public static readonly Type Railgunner = new("EscortRizzgayer");
    //     public static readonly Type Speedster = new("EscortCheese");
    //     public static readonly Type Gilded = new("EscortDrip");
    //     public static readonly Type Blaster = new("EscortForce");
    //     public static readonly Type Barbarian = new("EscortProWrestler");

    //     public Type(string value, bool register = false) : base (value, register)
    //     {
    //     }
    // }
}