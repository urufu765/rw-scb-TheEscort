using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Railgunner;

public static class RG_Player
{
    /// <summary>
    /// For now just gives Railgunner passive movement speed boost upon charge buildup
    /// </summary>
    public static void UpdateBodyMode(Player self, ref Escort e)
    {
        if (self.bodyMode != Player.BodyModeIndex.ZeroG && self.animation != Player.AnimationIndex.DeepSwim)
        {
            self.dynamicRunSpeed[0] += e.RailgunUse * 0.3f;
            self.dynamicRunSpeed[1] += e.RailgunUse * 0.3f;
        }
    }


    public static void ThrownSpear(Player self, Spear spear, in bool onPole, ref Escort e, ref float thrust)
    {
        try
        {
            thrust = 2f;  // Inverted the negatives so recoil isn't achieved here
            spear.spearDamageBonus = Mathf.Max(spear.spearDamageBonus, 1.1f);
            if (e.RailDouble is DoubleUp.Spear or DoubleUp.ElectroSpear)
            {
                if (!e.RailFirstWeaped)
                {
                    spear.firstChunk.vel *= new Vector2(Math.Abs(e.RailLastThrowDir.x), Math.Abs(e.RailLastThrowDir.y));
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
                        thrust *= 8 + (e.RailFrail ? 1f : 0f);
                    }
                    else if (self.bodyMode == Player.BodyModeIndex.Stand)
                    {
                        thrust *= 11 + (e.RailFrail ? 3.25f : 0f);
                    }
                    else
                    {
                        thrust *= 14 + (e.RailFrail ? 5f : 0f);
                    }
                }
                spear.spearDamageBonus = .7f + (.05f * e.RailgunUse);
                spear.throwModeFrames = 400;
                //spear.alwaysStickInWalls = true;
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
                        //self.room?.AddObject(new ZapCoil.ZapFlash(es.sparkPoint, 10f));
                        self.room?.PlaySound(DLCSharedEnums.SharedSoundID.Volt_Shock, es.firstChunk, false, 0.6f, es.zapPitch == 0 ? (1.5f + UnityEngine.Random.value * 1.5f) : es.zapPitch);
                    }
                    self.room?.PlaySound(SoundID.Death_Lightning_Spark_Object, position, 0.95f, 0.9f);
                    self.room?.AddObject(new Explosion.ExplosionLight(position, 40f, 0.7f, 7, RG.ColorElectric));
                    es.Spark();
                    if (es.Submersion > 0.5)  // Part 2 of Zap
                    {
                        self.room?.AddObject(new UnderwaterShock(self.room, null, es.firstChunk.pos, 10, 400f, 0.5f, self, RG.ColorElectric));
                        e.RailIFrame = 10;
                    }
                    //self.room?.AddObject(new ZapCoil.ZapFlash(position, 25f));
                }
            }
            else
            {
                thrust *= .75f;
                spear.firstChunk.vel *= .95f;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error while applying Railgunner-specific spearthrow");
        }
    }






    /// <summary>
    /// Method that allows Railgunner to throw two objects at once.
    /// </summary>
    public static bool ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
    {
        if (e.RailDouble is DoubleUp.None)
        {
            if (e.RailgunCD > 0 && self.grasps?[grasp]?.grabbed is Weapon weaponry)
            {
                e.Escat_RG_IncreaseCD(80);
                e.RailZap.Add(new(weaponry.firstChunk, 20));
                e.RailGaussed = 30;
            }
            return false;
        }
        // TODO fix an exception that occurs somewhere here
        self.standing = false;
        Vector2 p = new();
        Vector2 v = new();
        Weapon w = null;  // So that the thrown direction can be achieved
        Weapon w2 = null;
        if (self.grasps[grasp] != null && self.grasps[grasp].grabbed is Weapon weapon)
        {
            p = self.grasps[grasp].grabbed.firstChunk.pos;
            v = self.grasps[grasp].grabbed.firstChunk.vel;
            w = weapon;
        }
        if (self.grasps[1 - grasp]?.grabbed is Weapon weapon2)
        {
            w2 = weapon2;
        }

        bool misfire = false;
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
            RG_Shocker.StunWave(self, 20 * e.RailgunUse, 0.01f * e.RailgunUse, 8 * e.RailgunUse, 0.05f * e.RailgunUse);
            RG_Fx.InnerSplosion(self, 400);
            e.RailgunUse = e.RailgunCD = 0;
            self.Stun(120);
            misfire = true;
        }
        else // normal
        {
            e.RailLastThrowDir = new(self.ThrowDirection, 0);
            if (
                self.bodyMode == Player.BodyModeIndex.ZeroG ||
                self.animation == Player.AnimationIndex.Flip ||
                self.animation == Player.AnimationIndex.DeepSwim
                )
            {
                e.RailLastThrowDir = new(self.input[0].y == 0 ? self.ThrowDirection : self.input[0].x, self.input[0].y);
            }
            if (Escort_CorridorThrowDir(self, out IntVector2 throwDir))
            {
                e.RailLastThrowDir = throwDir;
            }
            // else
            // {
            //     if (e.RailLastThrowDir.y != 0)
            //     {
            //         self.grasps[0].grabbed.firstChunk.pos.x -= 1.5f;
            //         self.grasps[1].grabbed.firstChunk.pos.x += 1.5f;
            //     }
            //     if (e.RailLastThrowDir.x != 0)
            //     {
            //         self.grasps[1 - grasp].grabbed.firstChunk.pos.y += self.animation == Player.AnimationIndex.DownOnFours? 3 : -3;
            //     }
            // }
            orig(self, grasp, eu);  // Throw the first hand
            //e.RailLastThrowDir = w.throwDir;  // Save last throw direction (may not be used)
            //self.grasps[1 - grasp].grabbed.firstChunk.pos = p;
            orig(self, 1 - grasp, eu);  // Also throw the second hand

            if (w is FirecrackerPlant fp1 && w2 is FirecrackerPlant fp2)
            {
                fp1.fuseCounter = fp2.fuseCounter = 3;  // Makes firecrackers immediately pop on fire
            }
            if (w is FlareBomb fl1 && w2 is FlareBomb fl2)
            {
                // Makes flares activate immediately on use
                fl1.StartBurn();
                fl2.StartBurn();
            }
            if (ModManager.MSC && w is SingularityBomb sb1 && w2 is SingularityBomb sb2)
            {
                // Sets singularity bomb states to activation mode, also skipping over the movement stop command
                sb1.activateSingularity = sb2.activateSingularity = true;
                sb1.counter = sb2.counter = 1;  // Skips over the setup
                sb1.gravity = sb2.gravity = 0;
            }
        }
        e.RailRecoilLag = 3;  // Get ready to recoil

        if (self.room != null)
        {
            Color c = RG.ColorRG;
            Smoke.FireSmoke s = new(self.room);
            self.room.AddObject(s);
            for (int i = 0; i < 6; i++)
            {
                self.room.AddObject(new Spark(self.bodyChunks[1].pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(2f, 7f, UnityEngine.Random.value) * 6, c, null, 10, 170));
                s.EmitSmoke(self.bodyChunks[1].pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, self.mainBodyChunk.vel + v * UnityEngine.Random.value * -10f, c, 12);
            }
            self.room.AddObject(new Explosion.ExplosionLight(p, 90f, 0.7f, 4, c));
            self.room.PlaySound(e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? SoundID.Cyan_Lizard_Powerful_Jump : SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, e.RailgunUse >= (int)(e.RailgunLimit * 0.7f) ? 0.8f : 0.93f, Mathf.Lerp(1.15f, 2f, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)));

            // Now Railgunner is VERY LOUD
            self.room.InGameNoise(new Noise.InGameNoise(self.mainBodyChunk.pos, 12000, self, 1f));

            // Railgunner now has a 50% (+5% per additional charge) chance to explode on overuse
            if (e.RailgunUse >= e.RailgunLimit && UnityEngine.Random.value < 0.5f + (0.05f * (e.RailgunUse - e.RailgunLimit)))
            {
                RG_Fx.DeathExplosion(self, self.room, ref e);
                if (ins.Esconfig_SFX(self))
                {
                    self.room.PlaySound(Escort_SFX_Railgunner_Death, e.SFXChunk);
                }
                return true;
            }
        }
        if (e.RailFrail)
        {
            int stunValue = 10 * e.RailgunUse;
            if (self.room?.game?.session is StoryGameSession sgs)
            {
                stunValue *= 10 - sgs.saveState.deathPersistentSaveData.karmaCap;
            }
            self.Stun(stunValue);
        }
        e.RailZap.Add(new(w.firstChunk, 40));
        e.RailZap.Add(new(w2.firstChunk, 40));
        if (w is Spear s1 && w2 is Spear s2)
        {
            e.Escat_RG_ResetSpearValues();
            (float? a, float? b) gravoty = (null, null);
            if (self.bodyMode != Player.BodyModeIndex.ZeroG && self.animation != Player.AnimationIndex.DeepSwim)
            {
                gravoty = (s1.g, s2.g);
                s1.gravity = s2.gravity = .42f;
            }
            e.RailLastSpear[0] = (
                s1,
                s1.doNotTumbleAtLowSpeed,
                s1.alwaysStickInWalls,
                gravoty.a
            );
            e.RailLastSpear[1] = (
                s2,
                s2.doNotTumbleAtLowSpeed,
                s2.alwaysStickInWalls,
                gravoty.b
            );
            s1.doNotTumbleAtLowSpeed = s2.doNotTumbleAtLowSpeed = true;
            s1.alwaysStickInWalls = s2.alwaysStickInWalls = true;
            e.RailLastReset = 400;
            e.RailLetNextPass = false;
        }
        self.noGrabCounter += 30;
        e.RailGaussed = 80;
        e.Escat_RG_Overcharge(halveAddition: misfire);
        e.Escat_RG_IncreaseCD(400);
        return true;
    }


    public static void GrabUpdate(Player self, ref Escort e)
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
        else if (self.grasps[0].grabbed is ScavengerBomb && self.grasps[1].grabbed is ScavengerBomb)
        {
            e.RailDouble = DoubleUp.Bomb;
            e.RailWeaping = 4;
        }
        else if (self.grasps[0].grabbed is FirecrackerPlant && self.grasps[1].grabbed is FirecrackerPlant)
        {
            e.RailDouble = DoubleUp.Firecracker;
            e.RailWeaping = 4;
        }
        else if (self.grasps[0].grabbed is FlareBomb && self.grasps[1].grabbed is FlareBomb)
        {
            e.RailDouble = DoubleUp.Flare;
            e.RailWeaping = 4;
        }
        else if (ModManager.MSC && self.grasps[0].grabbed is LillyPuck && self.grasps[1].grabbed is LillyPuck)
        {
            e.RailDouble = DoubleUp.LillyPuck;
            e.RailWeaping = 4;
        }
        else if (ModManager.MSC && self.grasps[0].grabbed is SingularityBomb && self.grasps[1].grabbed is SingularityBomb)
        {
            e.RailDouble = DoubleUp.Singularity;
            e.RailWeaping = 4;
        }
    }


    public static bool SpearGet(PhysicalObject obj)
    {
        if (obj is Spear s && s.mode == Weapon.Mode.StuckInWall)
        {
            return true;
        }
        return false;
    }
}