using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Brawler;

public static class BL_Weaponry
{
    public static void Shanker(Player self, Spear spear, ref Escort e, ref float thrust)
    {
        if (
            !brawlerSpearVelFac.TryGet(self, out float[] bSpearVel) ||
            !brawlerSpearDmgFac.TryGet(self, out float[] bSpearDmg) ||
            //!brawlerSpearThrust.TryGet(self, out float bSpearThr) ||
            !brawlerSpearShankY.TryGet(self, out float[] bSpearY)
        )
        {
            return;
        }
        try
        {
            if (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection)
            {

            }
            else
            {
                spear.spearDamageBonus *= bSpearDmg[0];
                if (self.bodyMode == Player.BodyModeIndex.Crawl)
                {
                    spear.firstChunk.vel.x *= bSpearVel[0];
                }
                else if (self.bodyMode == Player.BodyModeIndex.Stand)
                {
                    spear.firstChunk.vel.x *= bSpearVel[1];
                }
                else
                {
                    spear.firstChunk.vel.x *= bSpearVel[2];
                }
            }
            if (self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump)
            {
                thrust *= 2;
            }
            else
            {
                thrust *= 0.4f;
            }
            if (e.BrawSuperShank)
            {
                //spear.throwDir = new RWCustom.IntVector2(0, -1);
                spear.firstChunk.pos = e.BrawShankDir;
                //spear.firstChunk.vel.y = -(Math.Abs(spear.firstChunk.vel.y)) * bSpearY[0];
                //spear.firstChunk.pos += new Vector2(0f, bSpearY[1]);
                spear.firstChunk.vel *= bSpearY[0];
                //spear.doNotTumbleAtLowSpeed = true;
                spear.firstChunk.vel.x *= 0.15f;
                spear.doNotTumbleAtLowSpeed = true;
                spear.spearDamageBonus = bSpearDmg[1];
                spear.spearDamageBonus *= Mathf.Max(0.15f, Mathf.InverseLerp(0, 20, 20 - self.slowMovementStun));
            }
            else if (e.BrawWeaponInAction is Melee.Shank or Melee.ExShank)
            {
                if (e.BrawWallSpear.Count > 0)
                {
                    e.BrawWallSpear.Pop().doNotTumbleAtLowSpeed = e.BrawWall;
                }
                e.BrawWall = spear.doNotTumbleAtLowSpeed;
                e.BrawRevertWall = 4;
                e.BrawWallSpear.Push(spear);
                spear.doNotTumbleAtLowSpeed = true;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error while applying Brawler-specific speartoss");
        }
    }

    public static void Puncher(Rock self, Player p)
    {
        if (!brawlerRockHeight.TryGet(p, out float roH))
        {
            return;
        }
        self.firstChunk.vel.y *= roH;
    }
}