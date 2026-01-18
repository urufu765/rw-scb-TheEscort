using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;

namespace TheEscort.Deflector;

public static class DF_Player
{
    public static void UpdateAnimation(Player self, ref Escort e)
    {
        if (self.animation == Player.AnimationIndex.BellySlide)
        {
            e.DeflSlideKick = true;
            if (self.rollCounter < 8)
            {
                self.rollCounter += 9;
            }
            if (self.initSlideCounter < 3)
            {
                self.initSlideCounter += 3;
            }
            int da = 32;
            int db = 18;
            if (ins.config.cfgFunnyDeflSlide.Value)
            {
                da = 46;
                db = 22;
            }
            if (e.DeflSlideCom < (self.longBellySlide ? da : db) && self.rollCounter > 12 && self.rollCounter < 15)
            {
                self.rollCounter--;
                //self.exitBellySlideCounter--;
            }
            self.mainBodyChunk.vel.x *= Mathf.Lerp(1.1f, self.longBellySlide ? 1.33f : 1.3f, Mathf.InverseLerp(0, self.longBellySlide ? 20 : 10, e.DeflSlideCom));
        }
        else if (e.DeflSlideKick && self.animation == Player.AnimationIndex.RocketJump)
        {
            self.mainBodyChunk.vel.x *= 1.4f;
            self.mainBodyChunk.vel.y *= 0.9f;
            e.DeflSlideKick = false;
        }
        else
        {
            e.DeflSlideKick = false;
        }

        if (e.DeflAerialParry > 0)
        {
            float adapt = e.DeflAerialWasStanding == true? (Mathf.InverseLerp(0, Escort.DeflAerialWindow, e.DeflAerialParry) * 1.5f - .5f) : (Mathf.InverseLerp(0, Escort.DeflAerialWindow, e.DeflAerialParry) * 2f - 1f);
            Vector2 flipVec = Custom.PerpendicularVector(self.bodyChunks[1].pos, self.bodyChunks[0].pos) * (self.slideDirection * Mathf.Lerp(2.38f, 2.8f, self.Adrenaline)) * adapt;
            Vector2 gravVec = new(0f, self.gravity * (e.DeflAerialParry > 6? 1 : 0));
            self.bodyChunks[0].vel += gravVec - flipVec;
            self.bodyChunks[1].vel += flipVec + gravVec;
            self.standing = false;
        }
        else if (e.DeflAerialWasStanding is not null)
        {
            self.standing = e.DeflAerialWasStanding.Value;
            e.DeflAerialWasStanding = null;
        }

        if (e.DeflZeroGParry > 0)
        {
            float adapt = Mathf.InverseLerp(0, Escort.DeflZeroGWindow, e.DeflZeroGParry) * 2f - 1f;
            Vector2 boostVec = e.DeflZeroGDir * 2 * adapt;
            self.bodyChunks[0].vel += boostVec;
            self.bodyChunks[1].vel += boostVec;
        }

        if (e.DeflSwimParry > 0)
        {
            float adapt = Mathf.InverseLerp(0, Escort.DeflSwimWindow, e.DeflSwimParry);
            Vector2 boostVec = Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos) * 4f * adapt;
            self.bodyChunks[0].vel += boostVec;
            self.bodyChunks[1].vel += boostVec;
        }

        if (e.DeflCorridorParry > 0)
        {
            float adapt = Mathf.InverseLerp(0, Escort.DeflZeroGWindow, e.DeflZeroGParry) * 2f - 1f;
            Vector2 boostVec = (e.DeflCorridorDir + new Vector2(0, self.gravity)) * adapt;
            self.bodyChunks[0].vel += boostVec;
            self.bodyChunks[1].vel += boostVec;
        }
    }


    public static void UpdateBodyMode(Player self, ref Escort e)
    {
        bool inputCondition = e.CustomKeybindEnabled? (e.CustomInput[0] && !e.CustomInput[1]) : (self.input[0].jmp && !self.input[1].jmp);

        if (
            self.bodyMode == Player.BodyModeIndex.Default && 
            self.animation == Player.AnimationIndex.None && 
            !self.bodyChunks.Any(b => b.ContactPoint.x != 0 || b.ContactPoint.y != 0) &&
            inputCondition &&
            e.DeflAerialParry == -1
        )
        {
            self.bodyChunks[0].vel *= 0.1f;
            self.bodyChunks[1].vel *= 0.1f;
            e.DeflAerialParry = Escort.DeflAerialWindow;
            e.DeflAerialWasStanding = self.standing;
            Ebug(self, "AerialParry!");
        }

        if (self.bodyMode == Player.BodyModeIndex.ZeroG)
        {
            if ((self.animation != Player.AnimationIndex.ZeroGSwim || self.bodyChunks.Any(b => b.ContactPoint.x != 0 || b.ContactPoint.y != 0)) && e.DeflZeroGParry > 0)
            {
                e.DeflZeroGParry -= Escort.DeflZeroGWait;
            }

            if ((self.animation == Player.AnimationIndex.ZeroGSwim || self.animation == Player.AnimationIndex.ZeroGPoleGrab) && inputCondition && e.DeflZeroGParry == -1)
            {
                e.DeflZeroGParry = Escort.DeflZeroGWindow;
                if (self.input[0].x == 0 && self.input[0].y == 0)
                {
                    e.DeflZeroGDir = Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos);
                }
                else
                {
                    e.DeflZeroGDir = new Vector2(self.input[0].x, self.input[0].y).normalized;
                }
                Ebug(self, "ZeroGParry!");
            }

        }
        else if (self.bodyMode == Player.BodyModeIndex.Swimming)
        {
            if (e.DeflSwimParry > 0 && (self.animation != Player.AnimationIndex.DeepSwim || self.bodyChunks.Any(b => b.ContactPoint.x != 0 || b.ContactPoint.y != 0)))
            {
                e.DeflSwimParry = -self.waterJumpDelay;
            }
            if (self.animation == Player.AnimationIndex.DeepSwim && inputCondition && e.DeflSwimParry == -1)
            {
                e.DeflSwimParry = Escort.DeflSwimWindow;
                Ebug(self, "SwimParry!");
            }
        }
        else if (self.bodyMode == Player.BodyModeIndex.CorridorClimb)
        {
            if (e.DeflCorridorParry > 0 && self.IsTileSolid(0, (int)e.DeflCorridorDir.x, (int)e.DeflCorridorDir.y))
            {
                e.DeflCorridorParry -= Escort.DeflCorridorCD;
            }
            if (inputCondition && e.DeflCorridorParry == -1)
            {
                Vector2 direc = Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos);
                if (Mathf.Abs(direc.x) > Mathf.Abs(direc.y))
                {
                    e.DeflCorridorDir = new(1 * Mathf.Sign(direc.x), 0);
                }
                else
                {
                    e.DeflCorridorDir = new(0, 1 * Mathf.Sign(direc.y));
                }
                Ebug(self, "CorridorParry!");
            }
        }
    }


    public static bool StickySpear(Player self)
    {
        return !(
            self.animation == Player.AnimationIndex.BellySlide ||
            self.animation == Player.AnimationIndex.Roll ||
            self.animation == Player.AnimationIndex.Flip
        );
    }
}