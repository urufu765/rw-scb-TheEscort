using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace TheEscort.Railgunner;

public static class RG_Weaponry
{
    public static void LillyThrow(On.MoreSlugcats.LillyPuck.orig_Thrown orig, LillyPuck self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        try
        {
            if (thrownBy == null)
            {
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }
            //self.doNotTumbleAtLowSpeed = false;
            self.canBeHitByWeapons = true;
            if (thrownBy is Player p)
            {
                if (Escort_IsNull(p.slugcatStats.name))
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (!eCon.TryGetValue(p, out Escort e) ||
                    !railgunLillyVelFac.TryGet(p, out float rLillyVel))
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (e.Railgunner)
                {
                    //float thruster = 5f;
                    if (e.RailDouble is DoubleUp.LillyPuck)
                    {
                        if (!e.RailFirstWeaped)
                        {
                            self.firstChunk.vel.x *= Math.Abs(self.throwDir.x);
                            self.firstChunk.vel.y *= Math.Abs(self.throwDir.y);
                            if (!(p.input[0].x == 0 && p.input[0].y != 0))
                            {
                                self.firstChunk.vel.y *= 0.25f;
                            }
                            e.RailFirstWeaper = self.firstChunk.vel;
                            e.RailFirstWeaped = true;
                        }
                        else
                        {
                            self.firstChunk.vel = e.RailFirstWeaper;
                            e.RailFirstWeaped = false;
                        }
                        self.canBeHitByWeapons = false;
                        //self.doNotTumbleAtLowSpeed = true;
                        frc *= rLillyVel;
                    }
                    else
                    {
                        frc *= 1.25f;
                    }
                }
            }
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        }
        catch (Exception err)
        {
            Ebug(self.thrownBy as Player, err, "Error in Lillythrow!");
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            return;
        }
    }

    public static void BombThrow(On.ScavengerBomb.orig_Thrown orig, ScavengerBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        try
        {
            if (thrownBy == null)
            {
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }
            if (thrownBy is Player p)
            {
                if (Escort_IsNull(p.slugcatStats.name))
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (!eCon.TryGetValue(p, out Escort e) ||
                    !railgunBombVelFac.TryGet(p, out float rBombVel))
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (e.Railgunner)
                {
                    //float thruster = 5f;
                    if (e.RailDouble is DoubleUp.Bomb)
                    {
                        if (!e.RailFirstWeaped)
                        {
                            e.RailFirstWeaper = self.firstChunk.vel;
                            //self.canBeHitByWeapons = false;
                            e.RailFirstWeaped = true;
                        }
                        else
                        {
                            self.firstChunk.vel = e.RailFirstWeaper;
                            e.RailFirstWeaped = false;
                        }
                        self.canBeHitByWeapons = false;
                        e.RailBombJump = p.animation == Player.AnimationIndex.Flip && p.input[0].x == 0 && p.input[0].y != 0;
                        if (!e.RailBombJump)
                        {
                            self.floorBounceFrames += 20;
                        }
                        e.RailIFrame = 20;
                        frc *= rBombVel;
                    }
                }

            }
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        }
        catch (Exception err)
        {
            Ebug(self.thrownBy as Player, err, "Error in Lillythrow!");
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            return;
        }
    }


    public static void Weapon_AntiDeflect(On.Weapon.orig_WeaponDeflect orig, Weapon self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        try
        {
            if (self.thrownBy is Player p)
            {
                if (Escort_IsNull(p.slugcatStats.name))
                {
                    orig(self, inbetweenPos, deflectDir, bounceSpeed);
                    return;
                }
                if (!eCon.TryGetValue(p, out Escort e))
                {
                    orig(self, inbetweenPos, deflectDir, bounceSpeed);
                    return;
                }
                if (e.Railgunner && (e.RailDouble is not DoubleUp.None || (e.RailGaussed > 0 && self.thrownBy == e.RailThrower)))
                {
                    Ebug(p, "NO DEFLECTING");
                    return;
                }

            }
            orig(self, inbetweenPos, deflectDir, bounceSpeed);
        }
        catch (Exception err)
        {
            Ebug(self.thrownBy as Player, err, "Weapon Anti-Deflect failed!");
            orig(self, inbetweenPos, deflectDir, bounceSpeed);
        }
    }


    public static void RockThrow(Rock self, Player p, ref float frc, ref Escort e)
    {
        if (
            !railgunRockVelFac.TryGet(p, out float rRockVel)
            // || !railgunRockThrust.TryGet(p, out float[] rRockThr)
            )
        {
            return;
        }
        //float thruster = 5f;
        if (e.RailDouble is DoubleUp.Rock)
        {
            if (!e.RailFirstWeaped)
            {
                if (e.RailTargetAcquired is not null)
                {
                    self.firstChunk.vel += Custom.DirVec(self.firstChunk.pos, e.RailTargetAcquired.pos) * 2;
                }
                e.RailFirstWeaper = self.firstChunk.vel;
                //self.canBeHitByWeapons = false;
                e.RailFirstWeaped = true;
            }
            else
            {
                self.firstChunk.vel = e.RailFirstWeaper;
                e.RailFirstWeaped = false;
                /*
                if (p.bodyMode == Player.BodyModeIndex.Crawl){
                    thruster *= rRockThr[0];
                } else if (p.bodyMode == Player.BodyModeIndex.Stand) {
                    thruster *= rRockThr[1];
                } else {
                    thruster *= rRockThr[2];
                }
                */
                //e.RailDoubleRock = false;
            }
            self.canBeHitByWeapons = false;
            frc *= rRockVel;
        }
        else
        {
            //thruster *= rRockThr[3];
            frc *= 1.5f;
        }
        /*
        BodyChunk firstChunker = p.firstChunk;
        if (!(p.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || p.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut)){
            if ((self.room != null && self.room.gravity == 0f) || (Mathf.Abs(self.firstChunk.vel.x) < 1f && Mathf.Abs(self.firstChunk.vel.y) < 1f)){
                //p.firstChunk.vel += RWCustom.Custom.IntVector2ToVector2(throwDir) * rRockThr[0];
                p.firstChunk.vel += self.firstChunk.vel.normalized * Math.Abs(thruster);
            } else {
                if (Esconfig_Spears(p)){
                    p.rollDirection = (int)Mathf.Sign(self.firstChunk.vel.x);
                }
                if (p.animation != Player.AnimationIndex.BellySlide){
                    p.firstChunk.vel.x = firstChunker.vel.x + Mathf.Sign(self.firstChunk.vel.x * firstChunker.vel.x) * thruster;
                }
            }
        }
        */
    }

    /// <summary>
    /// Increases velocity of firecrackers and makes them unhittable when railgunned, also gives Railgunner a 7 frame immunity window in case the firecracker pops right in her face
    /// </summary>
    public static void CrackerThrow(On.FirecrackerPlant.orig_Thrown orig, FirecrackerPlant self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && eCon.TryGetValue(p, out Escort e) && e.Railgunner && e.RailDouble is DoubleUp.Firecracker)
        {
            if (!e.RailFirstWeaped)
            {
                e.RailFirstWeaper = self.firstChunk.vel;
                e.RailFirstWeaped = true;
            }
            else
            {
                self.firstChunk.vel = e.RailFirstWeaper;
            }
            self.canBeHitByWeapons = false;
            e.RailIFrame = 7;
            frc *= 8f;
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    /// <summary>
    /// Increases velocity of flares and makes them unhittable when railgunned
    /// </summary>
    public static void FlareThrow(On.FlareBomb.orig_Thrown orig, FlareBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && eCon.TryGetValue(p, out Escort e) && e.Railgunner && e.RailDouble is DoubleUp.Flare)
        {
            if (!e.RailFirstWeaped)
            {
                e.RailFirstWeaper = self.firstChunk.vel;
                e.RailFirstWeaped = true;
            }
            else
            {
                self.firstChunk.vel = e.RailFirstWeaper;
            }
            self.canBeHitByWeapons = false;
            frc *= 2.5f;
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    /// <summary>
    /// Increases velocity of singularity and makes them unhittable when railgunned
    /// </summary>
    public static void SingularThrow(On.MoreSlugcats.SingularityBomb.orig_Thrown orig, MoreSlugcats.SingularityBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && eCon.TryGetValue(p, out Escort e) && e.Railgunner && e.RailDouble is DoubleUp.Singularity)
        {
            if (!e.RailFirstWeaped)
            {
                e.RailFirstWeaper = self.firstChunk.vel;
                e.RailFirstWeaped = true;
            }
            else
            {
                self.firstChunk.vel = e.RailFirstWeaper;
            }
            self.canBeHitByWeapons = false;
            frc *= 3f;
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    public static void SpearThrow(On.Spear.orig_Thrown orig, Spear self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && eCon.TryGetValue(p, out Escort e) && e.Railgunner && e.RailDouble is DoubleUp.Spear or DoubleUp.ElectroSpear)
        {
            self.canBeHitByWeapons = false;
            frc *= 1.5f;
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    public static void WeaponThrow(On.Weapon.orig_Thrown orig, Weapon self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && eCon.TryGetValue(p, out Escort e) && e.Railgunner && e.RailDouble is not DoubleUp.None)
        {
            throwDir = e.RailLastThrowDir;
            thrownPos = p.firstChunk.pos + throwDir.ToVector2() * 10f + new Vector2(0f, 4f);
            firstFrameTraceFromPos = p.mainBodyChunk.pos - throwDir.ToVector2() * 10f;

            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);

            if (throwDir.x != 0 && throwDir.y != 0)
            {
                self.firstChunk.vel = new(throwDir.x * 40f * frc, throwDir.y * 40f * frc);
            }
            return;
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }


    /// <summary>
    /// Generic weapon hit a creature
    /// </summary>
    /// <param name="self"></param>
    /// <param name="result"></param>
    /// <param name="weaponType"></param>
    public static void GenericHit(Weapon self, SharedPhysics.CollisionResult result, DoubleUp weaponType = DoubleUp.None)
    {
        if (self.thrownBy is Player p && Escort_IsNull(p.slugcatStats?.name, false) && result.chunk?.owner is Creature c && eCon.TryGetValue(p, out Escort e) && e.Railgunner)
        {
            RG_Shocker.ApplyShockingStuff(p, c, self, weaponType, self.room, ref e);
        }
    }

    /// <summary>
    /// Makes spear just penetrate through anything
    /// </summary>
    public static void SpearHitSomething(ref ILCursor c)
    {
        try
        {
            c.GotoNext(MoveType.After,
                pleaseFind => pleaseFind.MatchCallOrCallvirt<Creature>(nameof(Creature.SpearStick))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Railgunner Spear Penetration IL match failed");
            throw new Exception("RailPen IL Match failed", ex);
        }
        Ebug("SpearPen Identified point of interest", LogLevel.MESSAGE, true);

        try
        {
            c.Emit(OpCodes.Ldarg, 0);
            c.Emit(OpCodes.Ldarg, 1);
            c.EmitDelegate(RailsPenetratesAllArmor);
        }
        catch (Exception ex)
        {
            Ebug(ex, "Railgunner Spear Penetration injection failed");
            throw new Exception("RailPen IL Emit failed", ex);
        }
    }

    public static bool RailsPenetratesAllArmor(bool original, Spear self, SharedPhysics.CollisionResult result)
    {
        if (!original && result.chunk is not null && self?.thrownBy is Player p && eCon.TryGetValue(p, out Escort e) && e.Railgunner && (e.RailLastSpears?.a == self || e.RailLastSpears?.b == self))
        {
            Ebug("RAILGUNNER PENETRATION: " + result.obj.ToString());
            return true;
        }
        return original;
    }
}