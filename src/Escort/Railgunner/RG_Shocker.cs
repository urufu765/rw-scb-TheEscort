using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheEscort.Railgunner;

/// <summary>
/// Electricity related Railgunning shenanigans
/// </summary>
public static class RG_Shocker
{
    /// <summary>
    /// Checks whether creature is immune to shock or not
    /// </summary>
    /// <param name="areThey"></param>
    /// <returns></returns>
    public static bool IsCreatureImmuneToShock(Creature areThey)
    {
        if (areThey is Centipede or BigEel || (ModManager.MSC && areThey is BigJellyFish or Inspector))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the stun amount based on stun resistance
    /// </summary>
    /// <param name="creature">Creature checking</param>
    /// <param name="baseAmount">Original amount</param>
    /// <returns></returns>
    public static float GetCreatureStunAmount(Creature creature, float baseAmount)
    {
        if (creature is not Player)
        {
            return baseAmount * Mathf.Lerp(creature.Template.baseStunResistance, 1, 0.5f);
        }
        return baseAmount;
    }


    /// <summary>
    /// For stunning a creature electrically
    /// </summary>
    /// <param name="by">player</param>
    /// <param name="at">Target</param>
    /// <param name="sorce">Source</param>
    /// <param name="amount">Stun duration</param>
    /// <param name="room">In room</param>
    public static void ApplyShockingStuff(Player by, Creature at, BodyChunk sorce, SharedPhysics.CollisionResult? result, float amount, Room room, bool intensify = false)
    {
        float actualAmount = GetCreatureStunAmount(at, amount);

        // do a violent stun
        if (!IsCreatureImmuneToShock(at))
        {
            at.Violence(sorce, Custom.DirVec(sorce.pos, at.firstChunk.pos) * 3f, at.firstChunk, result?.onAppendagePos??null, Creature.DamageType.Electric, 0.1f, actualAmount);
            room?.AddObject(new CreatureSpasmer(at, false, at.stun));
        }
        if (at.Submersion > 0.5f)
        {
            room?.AddObject(new UnderwaterShock(room, null, at.firstChunk.pos, 5, 250f, 1f, by, RG.ColorRG));
        }
        Color electro = RG.ColorElectric;
        room?.PlaySound(DLCSharedEnums.SharedSoundID.Volt_Shock, sorce.pos, 0.8f, Mathf.Lerp(0.67f, 1.1f, UnityEngine.Random.value));
        room?.InGameNoise(new Noise.InGameNoise(sorce.pos, amount * 4, sorce.owner ?? at, 0.8f));
        room?.AddObject(new ShockWave(sorce.pos, Mathf.Lerp(25f, 50f, UnityEngine.Random.value), 0.07f, 5));
        if (intensify)
        {
            room?.AddObject(new ZapCoil.ZapFlash(sorce.pos, 10f));
            for (int i = 0; i < 8; i++)
            {
                Vector2 randomDir = Custom.DegToVec(360f * UnityEngine.Random.value);
                room?.AddObject(new MouseSpark(
                    sorce.pos + randomDir * 9f,
                    sorce.pos + randomDir * (28 * UnityEngine.Random.value),
                    16f, new Color(0.56f, 0.75f, 0.8f)));
            }
        }
        else
        {
            room?.AddObject(new Explosion.ExplosionLight(sorce.pos, 40, 0.8f, 6, electro));
            for (int i = 0; i < 5; i++)
            {
                Vector2 randDir = Custom.RNV();
                room?.AddObject(new Spark(sorce.pos + randDir * UnityEngine.Random.value * 10,
                    randDir * Mathf.Lerp(6, 18, UnityEngine.Random.value), electro, null, 4, 18));
            }
        }
    }


    /// <summary>
    /// For stunning a creature electrically using a martial move. Gives a bit of immunity as well
    /// </summary>
    /// <param name="by">You</param>
    /// <param name="at">Target</param>
    /// <param name="type">Type of move</param>
    /// <param name="room">The room</param>
    /// <param name="e">Escort</param>
    public static void ApplyShockingStuff(Player by, Creature at, RailPower type, Room room, ref Escort e)
    {
        if (e.RailgunCD == 0) return;
        e.RailIFrame += 5;
        ApplyShockingStuff(by, at, by.mainBodyChunk, null, e.Escat_RG_SheDoesHowMuch(type), room);
    }

    /// <summary>
    /// For stunning a creature electrically using a doubled weapon. Gives a bit of immunity as well
    /// </summary>
    /// <param name="by">You</param>
    /// <param name="at">Target</param>
    /// <param name="type">Type of weapon</param>
    /// <param name="room">The room</param>
    /// <param name="e">Escort</param>
    public static void ApplyShockingStuff(Player by, Creature at, Weapon with, SharedPhysics.CollisionResult? result, DoubleUp type, Room room, ref Escort e)
    {
        if (e.RailgunCD == 0) return;
        e.RailIFrame += 2;
        ApplyShockingStuff(by, at, with.firstChunk, result, e.Escat_RG_SheDoesHowMuch(type), room);
    }

    /// <summary>
    /// Does an aoe shock. TODO: Swap with an Update & Deleteable version on public release
    /// </summary>
    /// <param name="self">Causer</param>
    /// <param name="range">Shockrange</param>
    public static void StunWave(Player self, float range, float damage, float stun, float? underwaterDamage = null)
    {
        if (self?.room is null || range < 10) return;

        // Find a list of electric spears and charge or explode them
        if (ModManager.MSC)
        {
            foreach (ElectricSpear eSpear in self.room.physicalObjects
                .SelectMany(a => a)
                .OfType<ElectricSpear>()
                .Where(s => (
                    s.abstractPhysicalObject.rippleLayer == self.abstractPhysicalObject.rippleLayer || 
                    s.abstractPhysicalObject.rippleBothSides || 
                    self.abstractPhysicalObject.rippleBothSides
                    ) && Custom.DistLess(self.firstChunk.pos, s.firstChunk.pos, range)))
            {
                switch (eSpear.abstractSpear.electricCharge, UnityEngine.Random.value)
                {
                    case (3, _):
                    case (2, <0.67f):
                    case (1, <0.25f):
                        eSpear.ExplosiveShortCircuit();
                        break;
                    default:
                        eSpear.Recharge();
                        break;
                }
            }
        }
        // If submerged do underwater shock too
        if (self.Submersion > 0.5f)
        {
            self.room.AddObject(new UnderwaterShock(self.room, null, self.mainBodyChunk.pos, 7, range, underwaterDamage??(damage * 2), self, RG.ColorRG));
        }
        self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, self.mainBodyChunk);
        self.room.InGameNoise(new Noise.InGameNoise(self.mainBodyChunk.pos, range * 2, self, 0.8f));
        // self.room.AddObject(new ZapCoil.ZapFlash(self.mainBodyChunk.pos, range / 2));
        self.room.AddObject(new ShockWave(self.mainBodyChunk.pos, range, 0.01f, 40, false));
        // Find a list of creatures and do violence
        foreach (Creature cret in self.room.physicalObjects
            .SelectMany(a => a)
            .OfType<Creature>()
            .Where(c => c != self && !IsCreatureImmuneToShock(c) && (
                c.abstractPhysicalObject.rippleLayer == self.abstractPhysicalObject.rippleLayer || 
                c.abstractPhysicalObject.rippleBothSides || 
                self.abstractPhysicalObject.rippleBothSides
                ) && Custom.DistLess(self.firstChunk.pos, c.firstChunk.pos, range)))
        {
            float val = Custom.Dist(self.firstChunk.pos, cret.firstChunk.pos) / range;
            if (val == 0)
            {
                val = 0.05f;
            }
            float reduction = Mathf.Lerp(1, 0.01f, Mathf.Log(val) / 3 + 1);
            cret.Violence(self.firstChunk, null, cret.firstChunk, null, Creature.DamageType.Electric, damage * reduction, stun * reduction);
            self.room.AddObject(new CreatureSpasmer(cret, false, cret.stun));
            self.room.AddObject(new Explosion.FlashingSmoke(cret.firstChunk.pos, Custom.RNV() * Mathf.Lerp(4, 20, Mathf.Pow(UnityEngine.Random.value, 2)), 1.5f, RG.ColorElectric, RG.ColorRG, UnityEngine.Random.Range(3, 11)));
            for (int i = 0; i < 6; i++)
            {
                Vector2 random = Custom.RNV();
                self.room.AddObject(new Spark(cret.firstChunk.pos + 20 * random, random * Mathf.Lerp(4, 20, UnityEngine.Random.value), RG.ColorElectric, null, 4, 18));
            }
        }
    }

}