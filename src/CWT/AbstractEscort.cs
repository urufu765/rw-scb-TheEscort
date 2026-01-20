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

        public List<VengefulLizard> Vengefuls{get;set;}
        public int VengefulLizards{get;set;}
        public int VengefulUpdate{get;set;}
        public string VengefulDifficulty{get;set;}
        public int vengefulHunted;
        public bool Online{get;set;}
        public bool StoryTime{get;set;}

        public AbstractEscort()
        {
            buildId = 0;
            // ReadyForRespawn = -1;
            // HoldMyEscort = null;
            // HoldMyAbstract = null;
            Vengefuls = [];
            VengefulDifficulty = "disable";
            Online = false;
            StoryTime = false;
            VengefulLizards = 0;
            VengefulUpdate = 0;
            vengefulHunted = 0;
        }

        public void UpdateVengeance(AbstractCreature self)
        {
            if (StoryTime && self?.Room?.world?.game?.paused == false)
            {
                VengefulUpdate++;
                if (VengefulUpdate >= (Online? 40 : 20)) VengefulUpdate = 0;

                if (VengefulUpdate == 0 && Vengefuls.Count < VengefulLizards && self?.realizedCreature?.room?.game?.world is not null && self.world.rainCycle.timer > 600)
                {
                    Vengefuls.Add(new(self, self.realizedCreature.room, Online));
                    if (self?.realizedCreature is Player p)
                    {
                        Ebug(p, $"Spawned {Vengefuls.Count}/{VengefulLizards} Vengeance Lizors!");
                    }
                }
                for (int i = 0; i < Vengefuls.Count; i++)
                {
                    if (VengefulUpdate == i % (Online? 40 : 20))
                    {
                        if (Vengefuls[i]?.Lizor is null || Vengefuls[i]?.Lizor?.dead is true)
                        {
                            Vengefuls.RemoveAt(i);
                            continue;
                        }
                        Vengefuls[i].UpdateLizor();
                    }
                }
            }
        }

        public void TryVengeance(AbstractCreature self, int karma, bool enforced, int karmaCap, int points, int cycles, bool vengefulKill)
        {
            //Ebug($"Venging: {VengefulDifficulty}");
            int maxLizards = Custom.rainWorld.options.quality switch
            {
                var a when a == Options.Quality.LOW => 60,
                var a when a == Options.Quality.MEDIUM => 120,
                var a when a == Options.Quality.HIGH => 240,
                _ => 120
            };
            if (Online)
            {
                maxLizards = 40;
            }
            else
            {
                maxLizards /= self.realizedCreature?.room?.game?.Players?.Count??1;
            }
            if (VengefulLizards >= maxLizards)
            {
                if (self.realizedCreature is Player p)
                {
                    Ebug(p, $"Max vengeful lizards reached! {maxLizards}", LogLevel.WARN);
                }
                else
                {
                    Ebug($"Max vengeful lizards reached! {maxLizards}", LogLevel.WARN);
                }
                return;
            }

            if (vengefulKill)
            {
                VengefulLizards++;
                vengefulHunted++;
                if (self.realizedCreature is Player p)
                {
                    Ebug(p, $"Killed a vengeful lizard! Count: {VengefulLizards}");
                }
                else
                {
                    Ebug($"Killed a vengeful lizard! Count: {VengefulLizards}");
                }

                return;
            }
            if (VengefulDifficulty == "unfair")
            {
                if (VengefulLizards == 0) VengefulLizards = 8;
                VengefulLizards++;
                if (self.realizedCreature is Player p)
                {
                    Ebug(p, $"Unfair Vengeful! Count: {VengefulLizards}");
                }
                else
                {
                    Ebug($"Unfair Vengeful! Count: {VengefulLizards}");
                }
                return;
            }
            float probability = 0;
            switch (VengefulDifficulty)
            {
                case "hard":
                    probability += vengefulHunted / 100f;
                    probability += karmaCap / 100f;
                    goto case "medium";
                case "medium":
                    probability += Mathf.Lerp(0, .3f, points / 100f);
                    goto case "easy";
                case "easy":
                    probability += karma / 100f;
                    if (enforced) probability += .01f;
                    probability = Custom.LerpMap(cycles, 0, 100, 0, probability);
                    break;
            }
            if (UnityEngine.Random.value < probability)
            {
                if (VengefulLizards == 0) VengefulLizards = 2;
                VengefulLizards++;
                if (self.realizedCreature is Player p)
                {
                    Ebug(p, $"Vengeful activated! Count: {VengefulLizards}");
                }
                else
                {
                    Ebug($"Vengeful activated! Count: {VengefulLizards}");
                }
            }
        }

        public bool IsVengeanceLizard(EntityID id)
        {
            return Vengefuls.Any(a => a.Me.ID == id);
        }

        public int VengefulDivision()
        {
            return VengefulDifficulty switch
            {
                "medium" => 4,
                "hard" => 2,
                _ => 1
            };
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