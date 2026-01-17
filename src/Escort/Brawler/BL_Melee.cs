using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using IL;

namespace TheEscort.Brawler;

public static class BL_Melee
{
    public static bool CreatureLegality(On.Player.orig_IsCreatureLegalToHoldWithoutStun orig, Player self, Creature grabCheck)
    {
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                return orig(self, grabCheck);
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                return orig(self, grabCheck);
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Is Illegal.");
        }
        if (grabCheck is Overseer || grabCheck is PoleMimic || grabCheck is TempleGuard || grabCheck is Deer || grabCheck is DaddyLongLegs || grabCheck is Leech || grabCheck is TentaclePlant || grabCheck is MoreSlugcats.Inspector || grabCheck is MoreSlugcats.BigJellyFish)
        {
            return orig(self, grabCheck);
        }
        return grabCheck.TotalMass <= self.TotalMass * ins.ratioed * 2;
    }


    public static bool MeleeThrow(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
    {
        if (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection)
        {
            return false;
        }
        if (GetMeleeWeaponIndexable(self) is (Weapon w, int wi))
        {
            if (w is Spear)
            {
                e.BrawSuperShank = false;
                if (GetShankableCreatureIndexable(self) is (Creature c, int ci))
                {
                    if (self.slowMovementStun > 30)
                    {
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.4f);
                        return true;
                    }
                    // Throw creature first
                    orig(self, ci, eu);
                    e.BrawShankDir = c.firstChunk.pos;
                    Ebug(self, $"Hey {c}, would you like a cup of tea? Well it's a mugging!");
                    e.BrawSuperShank = true;
                }
                else
                {
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.2f);
                        return true;
                    }
                    Ebug(self, "SHANK!");
                    e.BrawShankDir = w.firstChunk.pos;
                }
                e.BrawShankSpearTumbler = w.doNotTumbleAtLowSpeed;
                e.BrawWeaponInAction = Melee.Shank;
                e.BrawThrowGrab = 5;
                if (w is ExplosiveSpear es)
                {
                    e.BrawExpspearAt = es.explodeAt;
                    es.explodeAt = 12;
                    es.igniteCounter = 1;
                    e.BrawExpIFrameReady = 8;
                    e.BrawWeaponInAction = Melee.ExShank;
                }
            }
            else if (w is Rock)
            {
                self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                if (self.slowMovementStun > 0)
                {
                    Ebug(self, "Too tired to punch!");
                    self.Blink(15);
                    Eshelp_Player_Shaker(self, 1f);
                    return true;
                }
                Ebug(self, "PUNCH!");
                e.BrawWeaponInAction = Melee.Punch;
                e.BrawThrowGrab = 4;
            }
            else if (w is ScavengerBomb)
            {
                self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                if (self.slowMovementStun > 0)
                {
                    Ebug(self, "Too tired to punch!");
                    self.Blink(15);
                    Eshelp_Player_Shaker(self, 1.3f);
                    return true;
                }
                e.BrawWeaponInAction = Melee.ExPunch;
                e.BrawThrowGrab = 4;
                e.BrawExpIFrameReady = 7;
            }
            if (e.BrawMeleeWeapon.Count > 0)
            {
                if (w is Spear)
                {
                    e.BrawMeleeWeapon.Pop().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
                }
                else
                {
                    e.BrawMeleeWeapon.Pop();
                }
            }
            e.BrawMeleeWeapon.Push(w);
            e.BrawThrowUsed = wi;
            orig(self, wi, eu);
            return true;

        }
        return false;
    }

    /// <summary>
    /// Updates the melee status
    /// </summary>
    public static void UpdateMeleeStatus(Player self, ref Escort e)
    {
        if (self?.grasps is null) return;
        e.BrawLastWeapon = GetMeleeWeapon(self) switch
        {
            ScavengerBomb => Melee.ExPunch,
            Rock => Melee.Punch,
            ExplosiveSpear => Melee.ExShank,
            Spear => Melee.Shank,
            _ => Melee.None
        };

        if (e.BrawLastWeapon is Melee.Shank or Melee.ExShank && HoldingShankableCreature(self))
        {
            e.BrawLastWeapon = Melee.SuperShank;
        }
    }

    /// <summary>
    /// Finds and returns a meleeable weapon
    /// </summary>
    public static Weapon GetMeleeWeapon(Player self)
    {
        return self.grasps.Where(g => g?.grabbed is not null).Select(g => g.grabbed).OfType<Weapon>().FirstOrDefault(w => w is Rock or Spear or ScavengerBomb);
    }

    /// <summary>
    /// Finds and returns a tuple of a weapon and the grasp
    /// </summary>
    public static (Weapon, int)? GetMeleeWeaponIndexable(Player self)
    {
        return (from g in self.grasps
                where g?.grabbed is Weapon
                let i = self.grasps.IndexOf(g)
                let w = g.grabbed as Weapon
                where w is Rock or Spear or ScavengerBomb
                select (w, i)).FirstOrDefault();
    }

    /// <summary>
    /// Finds and returns a shankable creature
    /// </summary>
    public static Creature GetShankableCreature(Player self)
    {
        return self.grasps
            .Where(g => g?.grabbed is not null)
            .Select(g => g.grabbed)
            .OfType<Creature>()
            .FirstOrDefault(
                c => !c.dead &&
                !(
                    c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly ||
                    c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.TubeWorm ||
                    c.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC
                ) &&
                !(c is Player && !Escort_CanHurtPlayer())
            );
    }
    /// <summary>
    /// Finds and returns a shankable creature and index
    /// TODO: Just replace with a plain ol' for loop with a break
    /// </summary>
    public static (Creature, int)? GetShankableCreatureIndexable(Player self)
    {
        return (from g in self.grasps
                where g?.grabbed is Creature
                let i = self.grasps.IndexOf(g)
                let c = g.grabbed as Creature
                select (c, i))
            .FirstOrDefault(
                tup => !tup.c.dead &&
                !(
                    tup.c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly ||
                    tup.c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.TubeWorm ||
                    tup.c.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC
                ) &&
                !(tup.c is Player && !Escort_CanHurtPlayer())
            );
    }

    public static bool HoldingShankableCreature(Player self)
    {
        return self.grasps.Any(
            g => g?.grabbed is Creature c &&
            !c.dead &&
            !(
                c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly ||
                c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.TubeWorm ||
                c.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC
            ) &&
            !(c is Player && !Escort_CanHurtPlayer())
        );
    }
}