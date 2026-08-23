using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster;

public static class Speedway_Player
{
    public static void Tick(Player self, ref Escort e)
    {
        if (!e.SpeNitrosBoost)
        {
            if (e.SpeNitrosX < Escort.SpeNitrosMax) e.SpeNitrosX += 2;
            if (e.SpeNitrosY < Escort.SpeNitrosMax) e.SpeNitrosY += 2;
        }


        if (e.SpeSpeedin > 0)
        {
            e.SpeDashNCrash = true;
            e.SpeSpeedin--;
            // if (self.input[0].x != 0 && e.SpeNitrosX > 0)
            // {
            //     e.SpeNitrosX--;
            // }
            // else if (self.input[0].x == 0 && e.SpeNitrosX < 40)
            // {
            //     e.SpeNitrosX += 2;
            // }
            // if (self.input[0].y != 0 && e.SpeNitrosY > 0)
            // {
            //     e.SpeNitrosY--;
            // }
            // else if (self.input[0].y == 0 && e.SpeNitrosY < 40)
            // {
            //     e.SpeNitrosY += 2;
            // }
        }
        else if (e.SpeDashNCrash)
        {
            e.SpeBuildup = 0;
            e.SpeDashNCrash = false;
            if (!e.karmaTen)
            {
                e.SpeGear = 0;
            }
        }
    }


    public static void Update(Player self, ref Escort e)
    {
        // Build up charge
        e.SpeGain = e.SpeDashNCrash ? 0 : .5f;
        if (Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)) > 2f)
        {
            // Build up charge from simply going fast
            if (self.input[0].AnyDirectionalInput)
            {
                e.SpeGain += 3f * Mathf.InverseLerp(3f, 17f, Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)));
            }

            // Build up charge from doing tricks
            e.SpeGain += self.animation switch
            {
                var value when value == Player.AnimationIndex.Flip => 1,
                var value when value == Player.AnimationIndex.BellySlide => 1.3f,
                var value when value == Player.AnimationIndex.Roll => 1.1f,
                var value when value == Player.AnimationIndex.StandOnBeam => 0.7f,
                var value when value == Player.AnimationIndex.SurfaceSwim => 1.2f,
                var value when value == Player.AnimationIndex.RocketJump => 2,
                var value when value == Player.AnimationIndex.ZeroGSwim => .2f,
                _ => 0
            };

            // Double the gain amount when in hyped
            if (e.SpeGain > 0 && self.aerobicLevel > ins.hypeRequirement)
            {
                e.SpeGain *= 2;
            }
        }

        if (e.SpeCharge < 1 && !e.SpeDashNCrash)
        {
            e.SpeBuildup += e.SpeGain;
            if (e.SpeBuildup > Escort.SpeSpeedwayReady)
            {
                e.SpeCharge = 1;
                e.SpeBuildup = 0;
                //TODO: FX
            }
        }
        else if (e.SpeDashNCrash)
        {
            e.SpeBuildup += Mathf.Max(0, e.SpeGain);
            if (e.SpeBuildup > Escort.SpeSpeedwayExtra + (Escort.SpeSpeedwayExtraDifficulty * e.SpeGear))
            {
                e.SpeGear++;
                e.SpeBuildup = 0;
                e.SpeLastBonusTime = Math.Min(e.SpeTimeLimit / 2, e.SpeTimeLimit - e.SpeSpeedin);
                e.SpeSpeedin = Math.Min(e.SpeTimeLimit, e.SpeSpeedin + (e.SpeTimeLimit / 2));
                //TODO: FX
            }
        }

        if (self.input[0].spec && e.SpeCharge == 1)
        {
            e.SpeCharge = 0;
            e.SpeBuildup = 0;
            e.SpeDashNCrash = true;
            if (e.SpeGear == 0) e.SpeGear++;
            e.SpeSpeedin = e.SpeTimeLimit;
        }

        e.SpeBuildup = Mathf.Clamp(e.SpeBuildup, 0, e.SpeDashNCrash ? (Escort.SpeSpeedwayExtra + (Escort.SpeSpeedwayExtraDifficulty * e.SpeGear)) : Escort.SpeSpeedwayReady);
    }
}