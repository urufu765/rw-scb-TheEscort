using BepInEx;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort;

partial class Plugin : BaseUnityPlugin
{
    // Railgunner tweak values
    // public static readonly PlayerFeature<> railgun = Player("theescort/railgunner/");
    // public static readonly PlayerFeature<float> railgun = PlayerFloat("theescort/railgunner/");
    // public static readonly PlayerFeature<float[]> railgun = PlayerFloats("theescort/railgunner/");
    public static readonly PlayerFeature<float[]> railgunSpearVelFac;
    public static readonly PlayerFeature<float[]> railgunSpearDmgFac;
    public static readonly PlayerFeature<float[]> railgunSpearThrust;
    public static readonly PlayerFeature<float> railgunRockVelFac;
    public static readonly PlayerFeature<float> railgunLillyVelFac;
    public static readonly PlayerFeature<float> railgunBombVelFac;
    public static readonly PlayerFeature<float[]> railgunRockThrust;
    public static readonly PlayerFeature<float> railgunRecoil;
    public static readonly PlayerFeature<float[]> railgunRecoilMod;
    public static readonly PlayerFeature<int> railgunRecoilDelay;
    public static readonly PlayerFeature<float[]> railgunLaserPos;

    public static void Esclass_RG_Tick(Player self, ref Escort e)
    {
        if (e.RailWeaping > 0)
        {
            e.RailWeaping--;
        }

        if (e.RailGaussed > 0)
        {
            e.RailGaussed--;
        }

        if (e.RailIFrame > 0)
        {
            e.RailIFrame--;
        }
        else
        {
            e.RailBombJump = false;
        }

        if (e.RailgunCD > 0)
        {
            if (self.bodyMode != Player.BodyModeIndex.Stunned)
            {
                e.RailgunCD--;
            }
        }
        else
        {
            e.RailgunUse = 0;
        }

        if (e.RailRecoilLag > 0)  // Recoil lag
        {
            e.RailRecoilLag--;
        }

        if (e.RailLaserBlinkClock > 19)
        {
            e.RailLaserBlinkClock = 0;
            e.RailLaserBlink = !e.RailLaserBlink;
        }
        else
        {
            e.RailLaserBlinkClock++;
        }

        // 1 second delay per check
        if (e.RailTargetClock > 0)
        {
            e.RailTargetClock--;
        }
    }

    public static void Esclass_RG_Update(Player self, ref Escort e)
    {
        // VFX
        if (self != null && self.room != null)
        {
            // Railgunner cooldown timer
            if (e.RailgunCD > 0)
            {
                Color railgunColor = new(0.5f, 0.85f, 0.78f);
                float r = UnityEngine.Random.Range(-1, 1);
                // TODO: reduce amount of sparks
                for (int i = 0; i < (e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 3 : 1); i++)
                {
                    Vector2 v = RWCustom.Custom.RNV() * (r == 0 ? 0.1f : r);
                    self.room.AddObject(new Spark(self.mainBodyChunk.pos + 15f * v, v, Color.Lerp(railgunColor * 0.5f, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit * 0.7f, e.RailgunUse)), null, e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 8 : 6, e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 16 : 10));
                }
                /*
                self.room.AddObject(new Explosion.FlashingSmoke(self.bodyChunks[0].pos, self.mainBodyChunk.vel + new Vector2(0, 1), 1f, Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)), Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 6)));
                */
                /*
                self.room.AddObject(new Explosion.ExplosionSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV() * 1.5f * UnityEngine.Random.value, 1f){
                    colorA = Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)),
                    colorB = Color.Lerp(new Color(0f, 0f, 0f, 0f), railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD))
                });*/
                //Smoke.FireSmoke s = new Smoke.FireSmoke(self.room);
                //self.room.AddObject(s);
                //s.EmitSmoke(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.Lerp(Color.black, railgunColor, Mathf.InverseLerp(0, 400, e.RailgunCD)), (e.RailgunUse >= e.RailgunLimit - 3? 12 : 5));
            }

        }

        if (
            !railgunRecoil.TryGet(self, out float rRecoil) ||
            !railgunRecoilMod.TryGet(self, out float[] rRecoilMod)
        )
        {
            return;
        }
        // Do recoil
        if (e.RailRecoilLag == 0)
        {
            e.RailRecoilLag = -1;
            // 0.7f, 1.5f, 0.4f, 0.75f, 1.5f
            Esclass_RG_Recoil(self, e.RailLastThrowDir, rRecoil, rRecoilMod, e.RailFrail);
        }


        // Creature check every 1 second
        // if (e.RailTargetClock == 0)
        // {
        //     e.RailTargetAcquired = Esclass_RG_Spotter(self);
        // }


        // Auto-escape out of danger grasp if overcharged
        if (self.dangerGraspTime == 29 && UnityEngine.Random.value <= (e.RailgunUse / e.RailgunLimit))
        {
            self.dangerGrasp.grabber.LoseAllGrasps();
            self.cantBeGrabbedCounter = 40;
            if (!e.RailFrail)
            {
                e.Escat_RG_SetGlassMode(true);
                Esclass_RG_InnerSplosion(self);
            }
            else
            {
                Esclass_RG_InnerSplosion(self, UnityEngine.Random.value < 0.75f);
            }
        }
    }


    /// <summary>
    /// For now just gives Railgunner passive movement speed boost upon charge buildup
    /// </summary>
    public static void Esclass_RG_UpdateBodyMode(Player self, ref Escort e)
    {
        self.dynamicRunSpeed[0] += e.RailgunUse * 0.3f;
        self.dynamicRunSpeed[1] += e.RailgunUse * 0.3f;
    }


    public static void Esclass_RG_ThrownSpear(Player self, Spear spear, in bool onPole, ref Escort e, ref float thrust)
    {
        if (
            !railgunSpearVelFac.TryGet(self, out float[] rSpearVel) ||
            !railgunSpearDmgFac.TryGet(self, out float[] rSpearDmg) ||
            !railgunSpearThrust.TryGet(self, out float[] rSpearThr)
        )
        {
            return;
        }
        try
        {
            thrust = 2f;  // Inverted the negatives so recoil isn't achieved here
            spear.spearDamageBonus = Mathf.Max(spear.spearDamageBonus, 1.1f);
            if (e.RailDouble is DoubleUp.Spear or DoubleUp.ElectroSpear)
            {
                if (!e.RailFirstWeaped)
                {
                    spear.firstChunk.vel *= rSpearVel[0];
                    e.RailFirstWeaper = spear.firstChunk.vel;
                    e.RailFirstWeaped = true;
                }
                else
                {
                    spear.firstChunk.vel = e.RailFirstWeaper;
                    e.RailFirstWeaped = false;
                    //e.BarbDoubleSpear = false;
                    if (self.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        thrust *= rSpearThr[0] + (e.RailFrail ? 1f : 0f);
                    }
                    else if (self.bodyMode == Player.BodyModeIndex.Stand)
                    {
                        thrust *= rSpearThr[1] + (e.RailFrail ? 3.25f : 0f);
                    }
                    else
                    {
                        thrust *= rSpearThr[2] + (e.RailFrail ? 5f : 0f);
                    }
                }
                spear.spearDamageBonus *= Mathf.Lerp(rSpearDmg[0], rSpearDmg[1], Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse));
                spear.alwaysStickInWalls = true;
                if (!onPole)
                {
                    self.standing = false;
                }
                if (ModManager.MSC && spear is ElectricSpear es)
                {
                    // A weaker version of a charge thing
                    Vector2 position = (self.graphicsModule as PlayerGraphics)?.hands?[0]?.pos ?? e.SFXChunk.pos;
                    if (spear.abstractSpear.electricCharge < 3)
                    {
                        spear.abstractSpear.electricCharge++;
                        self.room?.AddObject(new ZapCoil.ZapFlash(es.sparkPoint, 10f));
                        self.room?.PlaySound(SoundID.Zapper_Zap, es.firstChunk, false, 0.7f, es.zapPitch == 0 ? (1.5f + UnityEngine.Random.value * 1.5f) : es.zapPitch);
                    }
                    self.room?.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, position, 0.7f, 0.9f);
                    self.room?.AddObject(new Explosion.ExplosionLight(position, 25f, 0.3f, 2, new Color(0.7f, 1f, 1f)));
                    es.Spark();
                    if (es.Submersion > 0.5)  // Part 2 of Zap
                    {
                        self.room?.AddObject(new UnderwaterShock(self.room, null, es.firstChunk.pos, 10, 400f, 0.5f, self, new Color(0.8f, 0.8f, 1f)));
                        e.RailIFrame = 10;
                    }
                    self.room?.AddObject(new ZapCoil.ZapFlash(position, 25f));
                }
            }
            else
            {
                thrust *= rSpearThr[3];
                spear.firstChunk.vel *= rSpearVel[1];
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error while applying Railgunner-specific spearthrow");
        }
    }


    public static void Esclass_RG_LillyThrow(On.MoreSlugcats.LillyPuck.orig_Thrown orig, LillyPuck self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
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

    public static void Esclass_RG_BombThrow(On.ScavengerBomb.orig_Thrown orig, ScavengerBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
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


    public static void Esclass_RG_AntiDeflect(On.Weapon.orig_WeaponDeflect orig, Weapon self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
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


    public static void Esclass_RG_RockThrow(Rock self, Player p, ref float frc, ref Escort e)
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
    /// Method that allows Railgunner to throw two objects at once.
    /// </summary>
    public static bool Esclass_RG_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
    {
        if (!railgunRecoilDelay.TryGet(self, out int rRecoilDelay))
        {
            return false;
        }

        if (e.RailDouble is DoubleUp.None)
        {
            return false;
        }
        // TODO fix an exception that occurs somewhere here
        self.standing = false;
        Vector2 p = new();
        Vector2 v = new();
        Weapon w = null;  // So that the thrown direction can be achieved
        if (self.grasps[grasp] != null && self.grasps[grasp].grabbed is Weapon weapon)
        {
            p = self.grasps[grasp].grabbed.firstChunk.pos;
            v = self.grasps[grasp].grabbed.firstChunk.vel;
            w = weapon;
        }

        // Misfire!
        if (UnityEngine.Random.value < (e.RailFrail ? 0.02f : 0.005f) * e.RailgunUse)
        {
            self.TossObject(grasp, eu);
            self.TossObject(1 - grasp, eu);
            Esclass_GD_ReplicateThrowBodyPhysics(self, grasp);
            Esclass_GD_ReplicateThrowBodyPhysics(self, 1 - grasp);
            self.ReleaseGrasp(grasp);
            self.ReleaseGrasp(1 - grasp);
            self.dontGrabStuff = 15;
            Esclass_RG_InnerSplosion(self);
            self.Stun(120);
        }
        else // normal
        {
            //Weapon w = self.grasps[grasp].grabbed as Weapon;
            orig(self, grasp, eu);
            e.RailLastThrowDir = w.throwDir;  // Save last throw direction (may not be used)
            self.grasps[1 - grasp].grabbed.firstChunk.pos = p;
            //self.grasps[1].grabbed.firstChunk.vel = v;
            orig(self, 1 - grasp, eu);
        }
        e.RailRecoilLag = rRecoilDelay;  // Get ready to recoil

        if (self.room != null)
        {
            Color c = new(0.5f, 0.85f, 0.78f);
            Smoke.FireSmoke s = new(self.room);
            self.room.AddObject(s);
            for (int i = 0; i < 6; i++)
            {
                self.room.AddObject(new Spark(self.bodyChunks[1].pos + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(2f, 7f, UnityEngine.Random.value) * 6, c, null, 10, 170));
                s.EmitSmoke(self.bodyChunks[1].pos + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, self.mainBodyChunk.vel + v * UnityEngine.Random.value * -10f, c, 12);
            }
            self.room.AddObject(new Explosion.ExplosionLight(p, 90f, 0.7f, 4, c));
            self.room.PlaySound(e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? SoundID.Cyan_Lizard_Powerful_Jump : SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 0.8f : 0.93f, Mathf.Lerp(1.15f, 2f, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)));
            if (Esclass_RG_Death(self, self.room, ref e))
            {
                if (ins.Esconfig_SFX(self))
                {
                    self.room.PlaySound(Escort_SFX_Railgunner_Death, e.SFXChunk);
                }
                return true;
            }

            // (v * UnityEngine.Random.value * -0.5f + RWCustom.Custom.RNV() * Math.Abs(v.x * w.throwDir.x + v.y * w.throwDir.y)) * -1f
            // self.room.ScreenMovement(self.mainBodyChunk.pos, self.mainBodyChunk.vel * 0.02f, Mathf.Max(Mathf.Max(self.mainBodyChunk.vel.x, self.mainBodyChunk.vel.y) * 0.05f, 0f));
        }
        if (e.RailFrail)
        {
            e.RailgunUse++;
            int stunValue = 10 * e.RailgunUse;
            if (self.room?.game?.session is StoryGameSession sgs)
            {
                stunValue *= 10 - sgs.saveState.deathPersistentSaveData.karmaCap;
            }
            self.Stun(stunValue);
        }
        e.RailGaussed = 60;
        e.Escat_RG_Overcharge();
        return true;
    }



    public static void Esclass_RG_GrabUpdate(Player self, ref Escort e)
    {
        if (e.RailWeaping == 0)
        {
            e.RailDouble = DoubleUp.None;
        }
        for (int b = 0; b < 2; b++)
        {
            if (self.grasps[b] == null)
            {
                return;
            }
        }
        if (self.grasps[0].grabbed is Spear && self.grasps[1].grabbed is Spear)
        {
            e.RailDouble = DoubleUp.Spear;
            if (ModManager.MSC && (self.grasps[0].grabbed is ElectricSpear || self.grasps[1].grabbed is ElectricSpear))
            {
                e.RailDouble = DoubleUp.ElectroSpear;
            }
            e.RailWeaping = 4;
        }
        else if (self.grasps[0].grabbed is Rock && self.grasps[1].grabbed is Rock)
        {
            e.RailDouble = DoubleUp.Rock;
            e.RailWeaping = 4;
        }
        else if (ModManager.MSC && self.grasps[0].grabbed is LillyPuck && self.grasps[1].grabbed is LillyPuck)
        {
            e.RailDouble = DoubleUp.LillyPuck;
            e.RailWeaping = 4;
        }
        else if (self.grasps[0].grabbed is ScavengerBomb && self.grasps[1].grabbed is ScavengerBomb)
        {
            e.RailDouble = DoubleUp.Bomb;
            e.RailWeaping = 4;
        }
    }


    /// <summary>
    /// Makes Railgunner go BOOM
    /// </summary>
    public static void Esclass_RG_InnerSplosion(Player self, bool lethal = false)
    {
        try
        {
            Color c = new(0.5f, 0.85f, 0.78f);
            Vector2 v = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            Room room = self.room;
            room.AddObject(new SootMark(room, v, 120f, bigSprite: true));
            room.AddObject(new Explosion.ExplosionLight(v, 210f, 0.7f, 7, c));
            room.AddObject(new ShockWave(v, 500f, 0.05f, 6));
            for (int i = 0; i < 20; i++)
            {
                Vector2 v2 = RWCustom.Custom.RNV();
                room.AddObject(new Spark(v + v2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), v2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + RWCustom.Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(Color.white, c, UnityEngine.Random.value), null, 11, 33));
                room.AddObject(new Explosion.FlashingSmoke(v + v2 * 40f * UnityEngine.Random.value, v2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, Color.white, c, UnityEngine.Random.Range(3, 11)));
            }
            room.ScreenMovement(v, default, 1.5f);


            if (lethal)
            {
                room.AddObject(new Explosion(room, self, v, 10, 50f, 60f, 10f, 10f, 0.4f, self, 0.7f, 2f, 0f));
                room.AddObject(new Explosion(room, self, v, 8, 500f, 60f, 0.5f, 600f, 0.4f, self, 0.01f, 200f, 0f));
                room.PlaySound(SoundID.Bomb_Explode, self.mainBodyChunk, false, 0.93f, 0.28f);
                self.Die();
            }
            else
            {
                room.PlaySound(SoundID.Bomb_Explode, self.mainBodyChunk, false, 0.86f, 0.4f);
                room.AddObject(new Explosion(room, self, v, 8, 500f, 60f, 0.02f, 360f, 0.4f, self, 0.01f, 120f, 0f));
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Explosioning FAILED UH OH");
        }
    }


    public static bool Esclass_RG_Death(Player self, Room room, ref Escort e)
    {
        if (e.RailgunUse >= e.RailgunLimit)
        {
            if (UnityEngine.Random.value > (e.RailFrail ? 0.75f : 0.25f))
            {
                Esclass_RG_InnerSplosion(self);
                //self.stun += e.RailFrail ? 320 : 160;
                int stunDur = e.RailFrail ? 320 : 160;
                if (self.room?.game?.session is StoryGameSession sgs)
                {
                    stunDur *= 10 - sgs.saveState.deathPersistentSaveData.karmaCap;
                }

                self.Stun(stunDur);
                //self.SetMalnourished(true);
                e.Escat_RG_SetGlassMode(true);
                e.RailgunUse = (int)(e.RailgunLimit * 0.7f);
            }
            else
            {
                Esclass_RG_InnerSplosion(self, true);
            }
            return true;
        }
        return false;
    }


    public static bool Esclass_RG_SpearGet(PhysicalObject obj)
    {
        if (obj != null && obj is Spear s && s.mode == Weapon.Mode.StuckInWall)
        {
            return true;
        }
        return false;
    }

    public static void Esclass_RG_Spasm(On.Player.orig_Stun orig, Player self, int st)
    {
        orig(self, st);
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                return;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Stun!");
            return;
        }
        if (
            !eCon.TryGetValue(self, out Escort e)
            )
        {
            return;
        }
        if (!e.Railgunner) return;

        self?.room?.AddObject(new CreatureSpasmer(self, true, st));
        self.exhausted = true;
    }

    /// <summary>
    /// Applies recoil on the player
    /// </summary>
    public static void Esclass_RG_Recoil(Player self, IntVector2 throwDir, float force = 20f, float[] recoilMod = default, bool glassCannonBonus = false)
    {
        // Up/down velocity adjustment (so recoil jumps are a thing (and you don't get stunned when recoiling downwards))
        if (self.bodyMode != Player.BodyModeIndex.ZeroG)
        {
            if (throwDir.y > 0)  // Reduce downwards recoil
            {
                force *= recoilMod[0];
            }
            else if (throwDir.y < 0)  // Increase upwards recoil
            {
                force *= recoilMod[1];
            }
        }

        // Reduce recoil if proned/standing with the power of friction
        if (self.bodyMode == Player.BodyModeIndex.Crawl)
        {
            force *= recoilMod[2];
        }
        else if (self.bodyMode == Player.BodyModeIndex.Stand)
        {
            force *= recoilMod[3];
        }

        // Malnutrition bonus
        if (glassCannonBonus)
        {
            force *= recoilMod[4];
        }

        for (int i = 0; i < 2; i++)
        {
            self.bodyChunks[i].vel.x += throwDir.x * -force;
            self.bodyChunks[i].vel.y += throwDir.y * -force;
        }
    }

    public static BodyChunk Esclass_RG_Spotter(PlayerGraphics self, Vector2 origin, Vector2 direction, Vector2 corner, Vector2 camP)
    {
        BodyChunk target = null;
        // if (!railgunLaserPos.TryGet(self.player, out float[] lPos))
        // {
        //     Ebug("Failed to get slugbase feature for Railgunner laser", LogLevel.WARN);
        // }
        try
        {
            float minX = Mathf.Min(origin.x, corner.x) - 15;
            float maxX = Mathf.Max(origin.x, corner.x) + 15;
            float minY = Mathf.Min(origin.y, corner.y) - 15;
            float maxY = Mathf.Max(origin.y, corner.y) + 15;
            float minDist = int.MaxValue;
            // Ebug("Origin: [" + origin.x + ", " + origin.y + "] | Corner: [" + corner.x + ", " + corner.y + "]");
            // Ebug("Positions xy: " + minX + "-" + maxX + ", " + minY + "-" + maxY);
            if (self?.player?.room?.physicalObjects is null) return null;
            // List<BodyChunk> potentials = (
            //     from po in self.player.room.physicalObjects
            //     where po.Any(a => a is Creature)
            //     from co in po
            //     where co is Creature
            //     where co != self.player
            //     from bc in co.bodyChunks
            //     where bc.pos.y < maxY && bc.pos.y > minY && bc.pos.x < maxX && bc.pos.x > minX
            //     select bc
            // ).ToList();
            List<BodyChunk> potentials = self.player.room.physicalObjects
                .SelectMany(a => a)
                .OfType<Creature>()
                .Where(c => c != self.player)
                .SelectMany(c => c.bodyChunks)
                .Where(bc => bc.pos.y < maxY && bc.pos.y > minY && bc.pos.x < maxX && bc.pos.x > minX)
                .ToList();

            if (direction.x == 0 && direction.y != 0)  // Vertical
            {
                foreach (BodyChunk bc in potentials)
                {
                    if (Mathf.Abs(origin.y - bc.pos.y) < minDist)
                    {
                        minDist = Mathf.Abs(origin.y - bc.pos.y);
                        target = bc;
                    }
                }
            }
            else if (direction.x != 0 && direction.y == 0)  // Horizontal
            {
                foreach (BodyChunk bc in potentials)
                {
                    if (Mathf.Abs(origin.x - bc.pos.x) < minDist)
                    {
                        minDist = Mathf.Abs(origin.x - bc.pos.x);
                        target = bc;
                    }
                }
            }
            else  // Diagonal
            {
                foreach (BodyChunk bc in potentials)
                {
                    float why = Mathf.Abs(Mathf.Lerp(origin.y, corner.y, Mathf.InverseLerp(origin.x, corner.x, bc.pos.x)));
                    float compare = Mathf.Abs(bc.pos.y);
                    // Check if in diagonal line with padding
                    if (compare < why + 10 && compare > why - 10)
                    {
                        float distance = Custom.Dist(origin, bc.pos);
                        if (distance < minDist)  // Then check distance from player
                        {
                            minDist = distance;
                            target = bc;
                        }
                    }
                }
            }
            if (target is not null)
            {
                Ebug("Target is: " + target.owner.GetType().ToString());
            }

        }
        catch (Exception err)
        {
            Ebug(err, "FECK something happened with spotting a creature");
        }
        return target;
    }


    /// <summary>
    /// Makes Railgunner point to whoever dares to enter her line of sight
    /// </summary>
    public static void Esclass_RG_DrawThings(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
    {
        try
        {
            if (e.RailLaserSightIndex > s.sprites.Length)
            {
                Ebug("Laser sprite index is higher than expected!", LogLevel.ERR);
                return;
            }
            // Higher charge = brighter laser
            // Laser blinks if creature is in line of fire
            // 
            if (s.sprites[e.RailLaserSightIndex] is CustomFSprite cs)
            {
                if (e.RailDouble is not DoubleUp.None && self?.player?.dead == false)
                {
                    cs.isVisible = true;
                    float intensityThickness = Mathf.Lerp(0.6f, 0.8f, (float)e.RailgunUse / e.RailgunLimit);
                    float intensityAlpha = Mathf.Lerp(0.6f, 0.8f, (float)e.RailgunUse / e.RailgunLimit);
                    bool targetSpotted = false;
                    Vector2 headPos = Vector2.Lerp(self.head.lastPos, self.head.pos, t);
                    Vector2 bodyPos = Vector2.Lerp(self.player.bodyChunks[1].lastPos, self.player.bodyChunks[1].pos, t);
                    Vector2 neckPos = Vector2.Lerp(headPos, bodyPos, 0.14f);
                    // TODO: Give railgunner 8 dir shots

                    // 8 direction throw
                    // Vector2 throwDir = self.player.animation switch
                    // {
                    //     var a when a == Player.AnimationIndex.Flip => new(self.player.input[0].y == 0 ? self.player.ThrowDirection : self.player.input[0].x, self.player.input[0].y),
                    //     _ => new(self.player.ThrowDirection, 0)
                    // };

                    // How the game does throw direction calculation
                    Vector2 throwDir = new(self.player.ThrowDirection, 0);
                    if (self.player.animation == Player.AnimationIndex.Flip && self.player.input[0].x == 0)
                    {
                        if (self.player.input[0].y < 0)
                        {
                            throwDir.x = 0;
                            throwDir.y = -1;
                        }
                        else if (ModManager.MMF && MMF.cfgUpwardsSpearThrow.Value && self.player.input[0].y > 0)
                        {
                            throwDir.x = 0;
                            throwDir.y = 1;
                        }
                    }
                    if (self.player.bodyMode == Player.BodyModeIndex.ZeroG && ModManager.MMF && MMF.cfgUpwardsSpearThrow.Value)
                    {
                        throwDir = self.player.input[0].y == 0? new(self.player.ThrowDirection, 0) : new(0, self.player.input[0].y);
                    }


                    Vector2 corner = Custom.RectCollision(neckPos, neckPos + throwDir * 100000f, rCam.room.RoomRect.Grow(200f)).GetCorner(FloatRect.CornerLabel.D);
                    IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(rCam.room, neckPos, corner);
                    if (intVector is not null)
                    {
                        corner = Custom.RectCollision(corner, neckPos, rCam.room.TileRect(intVector.Value)).GetCorner(FloatRect.CornerLabel.D);
                    }

                    if (e.RailTargetClock == 0)
                    {
                        e.RailTargetAcquired = Esclass_RG_Spotter(self, neckPos, throwDir, corner, camP);
                        Ebug("Target checking! Found creature? " + (e.RailTargetAcquired is not null), ignoreRepetition: true);
                        e.RailTargetClock = 4;
                    }

                    if (e.RailTargetAcquired is not null)
                    {
                        float newX = corner.x;
                        float newY = corner.y;
                        if (throwDir.x < 0)
                        {
                            newX = Mathf.Max(corner.x, e.RailTargetAcquired.pos.x);
                        }
                        else if (throwDir.x > 0)
                        {
                            newX = Mathf.Min(corner.x, e.RailTargetAcquired.pos.x);
                        }
                        if (throwDir.y < 0)
                        {
                            newY = Mathf.Max(corner.y, e.RailTargetAcquired.pos.y);
                        }
                        else if (throwDir.y > 0)
                        {
                            newY = Mathf.Min(corner.y, e.RailTargetAcquired.pos.y);
                        }
                        corner = new(newX, newY);

                        if (e.RailTargetAcquired.owner is Creature cr && !cr.dead)
                        {
                            targetSpotted = true;
                            intensityAlpha = Mathf.Max(0.8f, intensityAlpha);
                            intensityThickness += 0.2f;
                        }
                    }

                    Color c = e.RailLaserBlink && targetSpotted ? Color.Lerp(Color.red, e.RailLaserColor, Mathf.InverseLerp(0, 10, e.RailLaserBlinkClock)) : e.RailgunnerColor;
                    cs.verticeColors = [.. cs.verticeColors.Select(a => Custom.RGB2RGBA(c, intensityAlpha))];

                    // This does some space magic, based on Vulture laser thing
                    cs.MoveVertice(0, neckPos + throwDir * 4f + Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(1, neckPos + throwDir * 4f - Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(2, corner + Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(3, corner - Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                }
                else
                {
                    cs.isVisible = false;
                }
            }
            else
            {
                Ebug("Unexpected failure to identify laser sprite!", LogLevel.WARN);
                return;
            }
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null reference when drawing Railgunner sprites");
        }
        catch (IndexOutOfRangeException ioore)
        {
            Ebug(ioore, "Index out of bounds when drawing Railgunner sprites");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when drawing Railgunner sprites");
        }
    }

    public static void Esclass_RG_UpdateGraphics(PlayerGraphics self, ref Escort e)
    {
        // Points the spears at the target when target acquired
        if (e.RailTargetAcquired?.owner is Creature c && !c.dead && e.RailDouble is not DoubleUp.None && self.hands is not null)
        {
            for (int i = 0; i < self.hands.Length; i++)
            {
                if (self.hands[i].reachingForObject) continue; // Don't override other things

                self.hands[i].reachingForObject = true;
                self.hands[i].absoluteHuntPos = e.RailTargetAcquired.pos;
            }
        }
    }


    public static Vector2 Esclass_RG_PointWeaponAt(On.Player.orig_GetHeldItemDirection orig, Player self, int hand)
    {
        if (self is null) return orig(self, hand);
        if (!eCon.TryGetValue(self, out Escort e)) return orig(self, hand);

        if (e.Railgunner && e.RailDouble is not DoubleUp.None && e.RailTargetAcquired?.owner is Creature c && !c.dead)
        {
            if (self.grasps[hand].grabbed is Spear s)
            {
                return Custom.DirVec(s.firstChunk.pos, e.RailTargetAcquired.pos);
            }

            else if (ModManager.MSC && self.grasps[hand].grabbed is LillyPuck l)
            {
                return Custom.DirVec(l.firstChunk.pos, e.RailTargetAcquired.pos);
            }
        }
        return orig(self, hand);
    }


    public static void Esclass_RG_InitiateSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, ref Escort e)
    {
        try
        {
            if (self is null || s?.sprites is null || rCam is null) return;
            Escort.Escat_setIndex_sprite_cue(ref e.RailLaserSightIndex, s.sprites.Length);
            Ebug("Set cue for Railgunner laser sprite");
            Array.Resize(ref s.sprites, s.sprites.Length + 1);
            s.sprites[e.RailLaserSightIndex] = new CustomFSprite("Futile_White")
            {
                shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"]
            };
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null reference when initiating Railgunner laser!");
        }
        catch (IndexOutOfRangeException ioore)
        {
            Ebug(ioore, "Index out of bounds when initiating Railgunner laser!");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when initiating Railgunner laser");
        }
    }

}
