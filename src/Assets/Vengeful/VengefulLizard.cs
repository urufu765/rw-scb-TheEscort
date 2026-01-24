using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using SlugBase.DataTypes;
using Smoke;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort.VengefulLizards;


public class VengefulLizardManager : VengefulCreatureManager
{
    public override Creature RealMe => Me?.realizedCreature as Lizard;
    public VengefulLizardManager(AbstractCreature target, RainWorldGame game, bool online) : base(target, game, online)
    {
    }

    /// <summary>
    /// Sets anger towards player and ignores everything else
    /// </summary>
    public override void BeVeryAngryAtPlayerAndNothingElse(AbstractCreature abstractMe, AbstractCreature abstractPlayer)
    {
        try
        {
            if (abstractMe.abstractAI?.RealAI is LizardAI lizardAI && lizardAI.tracker?.creatures is not null && lizardAI.agressionTracker is not null)
            {
                lizardAI.friendTracker.tamingDifficlty = 99;
                lizardAI.fear = 0;
                lizardAI.usedToVultureMask = 3000;
                bool targetSighted = false;
                lizardAI.preyTracker.ForgetAllPrey();
                foreach (Tracker.CreatureRepresentation tracked in lizardAI.tracker.creatures)
                {
                    if (tracked?.representedCreature is null) continue;
                    if (tracked.representedCreature != abstractPlayer)
                    {
                        if (tracked.representedCreature.voidCreature)
                        {
                            lizardAI.agressionTracker.ForgetCreature(tracked.representedCreature);
                        }
                        else
                        {
                            lizardAI.agressionTracker.SetAnger(tracked, 0, 0);
                        }
                        tracked.dynamicRelationship.currentRelationship.type = CreatureTemplate.Relationship.Type.Ignores;
                    }
                    else
                    {
                        targetSighted = true;
                        lizardAI.agressionTracker.SetAnger(tracked, 1, 1);
                        if (abstractMe.pos.room == abstractPlayer.pos.room)
                        {
                            tracked.MakeVisible();
                        }
                        lizardAI.preyTracker.AddPrey(tracked);
                    }
                }
                if (!targetSighted)
                {
                    lizardAI.tracker.SeeCreature(abstractPlayer);
                }
                if (abstractMe.pos.room == abstractPlayer.pos.room && abstractMe.realizedCreature is Lizard lizor && abstractPlayer.realizedCreature is Player p && lizor?.grasps?[0]?.grabbed is Creature c && c != p)
                {
                    lizor.ReleaseGrasp(0);
                    // Make lizard drop everything and chase after player
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to anger!!");
        }
    }

    public override void GetSomeHealing(float healing)
    {
        if (RealMe is Lizard lizor && lizor.LizardState is not null && lizor.LizardState.health > .01f && lizor.LizardState.health < .9f)
        {
            lizor.LizardState.health += healing;
        }
    }
}

public class VengefulCyan : VengefulLizardManager
{
    public override CreatureTemplate.Type MyType => CreatureTemplate.Type.CyanLizard;
    public VengefulCyan(AbstractCreature target, RainWorldGame game, bool online) : base(target, game, online)
    {
    }
}


public class VengefulRed : VengefulLizardManager
{
    public override CreatureTemplate.Type MyType => CreatureTemplate.Type.RedLizard;
    public VengefulRed(AbstractCreature target, RainWorldGame game, bool online) : base(target, game, online)
    {
    }

    // /// <summary>
    // /// Sets anger towards player and ignores everything else
    // /// </summary>
    // public override void BeVeryAngryAtPlayerAndNothingElse(AbstractCreature abstractMe, AbstractCreature abstractPlayer)
    // {
    //     base.BeVeryAngryAtPlayerAndNothingElse(abstractMe, abstractPlayer);
    //     try
    //     {
    //         if (abstractMe.pos.room == abstractPlayer.pos.room && abstractMe.abstractAI?.RealAI is LizardAI lizardAI && lizardAI.redSpitAI?.spitting == true)
    //         {
    //             lizardAI.redSpitAI.spitting = false;
    //         }
    //     }
    //     catch (Exception err)
    //     {
    //         Ebug(err, "Failed to Red anger!!");
    //     }
    // }

    public override void GetSomeHealing(float healing)
    {
        if (RealMe is Lizard lizor && lizor.LizardState is not null && lizor.LizardState.health > .005f && lizor.LizardState.health < .9f)
        {
            lizor.LizardState.health += healing;
        }
    }
}

public class VengefulTrain : VengefulLizardManager
{
    public override CreatureTemplate.Type MyType => MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;
    public VengefulTrain(AbstractCreature target, RainWorldGame game, bool online) : base(target, game, online)
    {
    }

    public override void GetSomeHealing(float healing)
    {
        if (RealMe is Lizard lizor && lizor.LizardState is not null && lizor.LizardState.health > 0.001f && lizor.LizardState.health < .9f)
        {
            lizor.LizardState.health += healing;
        }
    }
}
