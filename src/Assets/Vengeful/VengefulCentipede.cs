using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;

public class VengefulRedipede : VengefulCreatureManager
{
    public override Creature RealMe => Me?.realizedCreature as Centipede;
    public override CreatureTemplate.Type MyType => CreatureTemplate.Type.RedCentipede;
    public WorldCoordinate destination;
    public VengefulRedipede(AbstractCreature target, RainWorldGame game, bool online) : base(target, game, online)
    {
        destination = GetCornerableRoom(target, CreatureTemplate.Type.RedCentipede);
    }

    public override void GetMeToPlayerPos()
    {
        if (Me.abstractAI is not null && Player is not null && Target.pos != destination)
        {
            destination = Target.pos;
            Me.abstractAI.SetDestination(destination);
        }
    }

    /// <summary>
    /// Sets anger towards player and ignores everything else
    /// </summary>
    public override void BeVeryAngryAtPlayerAndNothingElse(AbstractCreature abstractMe, AbstractCreature abstractPlayer)
    {
        try
        {
            if (abstractMe.abstractAI?.RealAI is CentipedeAI centipedeAI && centipedeAI.tracker?.creatures is not null && centipedeAI.agressionTracker is not null)
            {
                centipedeAI.tracker.SeeCreature(abstractPlayer);
                foreach (Tracker.CreatureRepresentation tracked in centipedeAI.tracker.creatures)
                {
                    if (tracked?.representedCreature is null) continue;
                    if (tracked.representedCreature != abstractPlayer)
                    {
                        centipedeAI.tracker.ForgetCreature(tracked.representedCreature);
                    }
                    else 
                    {
                        if (Target.pos.room == Me.pos.room)
                        {
                            tracked.MakeVisible();
                        }
                        centipedeAI.preyTracker.AddPrey(tracked);
                    }
                }
                // if (abstractMe.pos.room == abstractPlayer.pos.room && abstractMe.realizedCreature is Centipede centipede && abstractPlayer.realizedCreature is Player p && centipede?.grasps?[0]?.grabbed is Creature c && c != p)
                // {
                //     centipede.ReleaseGrasp(0);
                // }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to anger!!");
        }
    }

    public override void GetSomeHealing(float healing)
    {
        if (RealMe is Centipede centipede && centipede.CentiState is not null && centipede.CentiState.health > .01f && centipede.CentiState.health < .9f)
        {
            centipede.CentiState.health += healing;
        }
    }
}