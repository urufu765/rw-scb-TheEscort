using BepInEx;
using RWCustom;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Railgunner tweak values
        // public static readonly PlayerFeature<> railgun = Player("theescort/railgunner/");
        // public static readonly PlayerFeature<float> railgun = PlayerFloat("theescort/railgunner/");
        // public static readonly PlayerFeature<float[]> railgun = PlayerFloats("theescort/railgunner/");
        public static readonly PlayerFeature<float[]> railgunSpearVelFac = PlayerFloats("theescort/railgunner/spear_vel_fac");
        public static readonly PlayerFeature<float[]> railgunSpearDmgFac = PlayerFloats("theescort/railgunner/spear_dmg_fac");
        public static readonly PlayerFeature<float[]> railgunSpearThrust = PlayerFloats("theescort/railgunner/spear_thrust");
        public static readonly PlayerFeature<float> railgunRockVelFac = PlayerFloat("theescort/railgunner/rock_vel_fac");
        public static readonly PlayerFeature<float> railgunLillyVelFac = PlayerFloat("theescort/railgunner/lilly_vel_fac");
        public static readonly PlayerFeature<float> railgunBombVelFac = PlayerFloat("theescort/railgunner/bomb_vel_fac");
        public static readonly PlayerFeature<float[]> railgunRockThrust = PlayerFloats("theescort/railgunner/rock_thrust");
        public static readonly PlayerFeature<float> railgunRecoil = PlayerFloat("theescort/railgunner/recoil_fac");
        public static readonly PlayerFeature<float[]> railgunRecoilMod = PlayerFloats("theescort/railgunner/recoil_mod");
        public static readonly PlayerFeature<int> railgunRecoilDelay = PlayerInt("theescort/railgunner/recoil_delay");

        public void Esclass_RG_Tick(Player self, ref Escort e)
        {
            if (e.RailWeaping > 0)
            {
                e.RailWeaping--;
            }

            if (e.RailGaussed > 0)
            {
                e.RailGaussed--;
            }
            else
            {
                e.RailIReady = false;
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

            // 1 second clock
            if (e.RailTargetClock > 0)
            {
                e.RailTargetClock--;
            }
            else
            {
                e.RailTargetClock = 39;
            }
        }

        private void Esclass_RG_Update(Player self, ref Escort e)
        {
            // VFX
            if (self != null && self.room != null)
            {
                // Railgunner cooldown timer
                if (e.RailgunCD > 0)
                {
                    Color railgunColor = new(0.5f, 0.85f, 0.78f);
                    float r = UnityEngine.Random.Range(-1, 1);
                    for (int i = 0; i < (e.RailgunUse >= e.RailgunLimit - 3 ? 3 : 1); i++)
                    {
                        Vector2 v = RWCustom.Custom.RNV() * (r == 0 ? 0.1f : r);
                        self.room.AddObject(new Spark(self.mainBodyChunk.pos + 15f * v, v, Color.Lerp(railgunColor * 0.5f, railgunColor, Mathf.InverseLerp(0, e.RailgunLimit - 3, e.RailgunUse)), null, (e.RailgunUse >= e.RailgunLimit - 3 ? 8 : 6), (e.RailgunUse >= e.RailgunLimit - 3 ? 16 : 10)));
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
                Esclass_RG_Recoil(self, e.RailLastThrowDir, rRecoil, rRecoilMod);
            }


            // Creature check every 1 second
            if (e.RailTargetClock == 0)
            {
                e.RailTargetAcquired = Esclass_RG_Spotter(self);
            }

        }


        /// <summary>
        /// For now just gives Railgunner passive movement speed boost upon charge buildup
        /// </summary>
        private void Esclass_RG_UpdateBodyMode(Player self, ref Escort e)
        {
            self.dynamicRunSpeed[0] += e.RailgunUse * 0.3f;
            self.dynamicRunSpeed[1] += e.RailgunUse * 0.3f;
        }


        private void Esclass_RG_ThrownSpear(Player self, Spear spear, in bool onPole, ref Escort e, ref float thrust)
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
                if (e.RailDoubleSpear)
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
                            thrust *= (rSpearThr[0] + (self.Malnourished ? 1f : 0f));
                        }
                        else if (self.bodyMode == Player.BodyModeIndex.Stand)
                        {
                            thrust *= (rSpearThr[1] + (self.Malnourished ? 3.25f : 0f));
                        }
                        else
                        {
                            thrust *= (rSpearThr[2] + (self.Malnourished ? 5f : 0f));
                        }
                    }
                    spear.spearDamageBonus *= Mathf.Lerp(rSpearDmg[0], rSpearDmg[1], Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse));
                    spear.alwaysStickInWalls = true;
                    if (!onPole)
                    {
                        self.standing = false;
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


        private void Esclass_RG_LillyThrow(On.MoreSlugcats.LillyPuck.orig_Thrown orig, MoreSlugcats.LillyPuck self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
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
                    if (p.slugcatStats.name.value != "EscortMe")
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
                        if (e.RailDoubleLilly)
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

        private void Esclass_RG_BombThrow(On.ScavengerBomb.orig_Thrown orig, ScavengerBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
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
                    if (p.slugcatStats.name.value != "EscortMe")
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
                        if (e.RailDoubleBomb)
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
                            e.RailIReady = true;
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

        private void Esclass_RG_AntiDeflect(On.Weapon.orig_WeaponDeflect orig, Weapon self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
        {
            try
            {
                if (self.thrownBy is Player p)
                {
                    if (p.slugcatStats.name.value != "EscortMe")
                    {
                        orig(self, inbetweenPos, deflectDir, bounceSpeed);
                        return;
                    }
                    if (!eCon.TryGetValue(p, out Escort e))
                    {
                        orig(self, inbetweenPos, deflectDir, bounceSpeed);
                        return;
                    }
                    if (e.Railgunner && (e.RailDoubled || (e.RailGaussed > 0 && self.thrownBy == e.RailThrower)))
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


        private void Esclass_RG_RockThrow(Rock self, Player p, ref float frc, ref Escort e)
        {
            if (
                !railgunRockVelFac.TryGet(p, out float rRockVel)
                // || !railgunRockThrust.TryGet(p, out float[] rRockThr)
                )
            {
                return;
            }
            //float thruster = 5f;
            if (e.RailDoubleRock)
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


        private bool Esclass_RG_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
        {
            if (!railgunRecoilDelay.TryGet(self, out int rRecoilDelay))
            {
                return false;
            }

            if (!(e.RailDoubleSpear || e.RailDoubleRock || e.RailDoubleBomb || e.RailDoubleLilly))
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
            //Weapon w = self.grasps[grasp].grabbed as Weapon;
            orig(self, grasp, eu);
            e.RailLastThrowDir = w.throwDir;  // Save last throw direction
            e.RailRecoilLag = rRecoilDelay;  // Get ready to recoil
            self.grasps[1 - grasp].grabbed.firstChunk.pos = p;
            //self.grasps[1].grabbed.firstChunk.vel = v;
            orig(self, 1 - grasp, eu);

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
                self.room.PlaySound(e.RailgunUse >= e.RailgunLimit - 3 ? SoundID.Cyan_Lizard_Powerful_Jump : SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk, false, (e.RailgunUse >= e.RailgunLimit - 3 ? 0.8f : 0.93f), Mathf.Lerp(1.15f, 2f, Mathf.InverseLerp(0, e.RailgunLimit, e.RailgunUse)));
                if (Esclass_RG_Death(self, self.room, ref e))
                {
                    if (Esconfig_SFX(self))
                    {
                        self.room.PlaySound(Escort_SFX_Railgunner_Death, e.SFXChunk);
                    }
                    return true;
                }

                // (v * UnityEngine.Random.value * -0.5f + RWCustom.Custom.RNV() * Math.Abs(v.x * w.throwDir.x + v.y * w.throwDir.y)) * -1f
                // self.room.ScreenMovement(self.mainBodyChunk.pos, self.mainBodyChunk.vel * 0.02f, Mathf.Max(Mathf.Max(self.mainBodyChunk.vel.x, self.mainBodyChunk.vel.y) * 0.05f, 0f));
            }
            if (self.Malnourished)
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
            int addition = 0;
            if (e.RailDoubleRock)
            {
                addition = 1;
            }
            else if (e.RailDoubleLilly)
            {
                addition = 2;
            }
            else if (e.RailDoubleSpear)
            {
                addition = 3;
            }
            else if (e.RailDoubleBomb)
            {
                addition = 4;
            }
            if (e.RailgunCD == 0)
            {
                e.RailgunCD = 400;
            }
            else
            {
                e.RailgunCD += (self.Malnourished ? 60 : 80) * addition;
            }
            if (e.RailgunCD > 800)
            {
                e.RailgunCD = 800;
            }
            e.RailgunUse += addition;
            return true;
        }

        private static void Esclass_RG_GrabUpdate(Player self, ref Escort e)
        {
            if (e.RailWeaping == 0)
            {
                e.RailDoubleSpear = false;
                e.RailDoubleRock = false;
                e.RailDoubleLilly = false;
                e.RailDoubleBomb = false;
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
                e.RailDoubleSpear = true;
                e.RailWeaping = 4;
            }
            else if (self.grasps[0].grabbed is Rock && self.grasps[1].grabbed is Rock)
            {
                e.RailDoubleRock = true;
                e.RailWeaping = 4;
            }
            else if (self.grasps[0].grabbed is MoreSlugcats.LillyPuck && self.grasps[1].grabbed is MoreSlugcats.LillyPuck)
            {
                e.RailDoubleLilly = true;
                e.RailWeaping = 4;
            }
            else if (self.grasps[0].grabbed is ScavengerBomb && self.grasps[1].grabbed is ScavengerBomb)
            {
                e.RailDoubleBomb = true;
                e.RailWeaping = 4;
            }
        }


        public bool Esclass_RG_Death(Player self, Room room, ref Escort e)
        {
            bool secondChance = false;
            if (e.RailgunUse >= e.RailgunLimit)
            {
                if (UnityEngine.Random.value > (self.Malnourished ? 0.75f : 0.25f))
                {
                    secondChance = true;
                }
                Color c = new(0.5f, 0.85f, 0.78f);
                Vector2 v = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
                room.AddObject(new SootMark(room, v, 120f, bigSprite: true));
                if (!secondChance)
                {
                    room.AddObject(new Explosion(room, self, v, 10, 50f, 60f, 10f, 10f, 0.4f, self, 0.7f, 2f, 0f));
                    room.AddObject(new Explosion(room, self, v, 8, 500f, 60f, 0.5f, 600f, 0.4f, self, 0.01f, 200f, 0f));
                }
                else
                {
                    room.AddObject(new Explosion(room, self, v, 8, 500f, 60f, 0.02f, 360f, 0.4f, self, 0.01f, 120f, 0f));
                }
                room.AddObject(new Explosion.ExplosionLight(v, 210f, 0.7f, 7, c));
                room.AddObject(new ShockWave(v, 500f, 0.05f, 6));
                for (int i = 0; i < 20; i++)
                {
                    Vector2 v2 = RWCustom.Custom.RNV();
                    room.AddObject(new Spark(v + v2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), v2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + RWCustom.Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(Color.white, c, UnityEngine.Random.value), null, 11, 33));
                    room.AddObject(new Explosion.FlashingSmoke(v + v2 * 40f * UnityEngine.Random.value, v2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, Color.white, c, UnityEngine.Random.Range(3, 11)));
                }
                room.ScreenMovement(v, default, 1.5f);
                if (!secondChance)
                {
                    room.PlaySound(SoundID.Bomb_Explode, e.SFXChunk, false, 0.93f, 0.28f);
                    self.Die();
                }
                else
                {
                    room.PlaySound(SoundID.Bomb_Explode, e.SFXChunk, false, 0.86f, 0.4f);
                    //self.stun += self.Malnourished ? 320 : 160;
                    int stunDur = self.Malnourished ? 320 : 160;
                    if (self.room?.game?.session is StoryGameSession sgs)
                    {
                        stunDur *= 10 - sgs.saveState.deathPersistentSaveData.karmaCap;
                    }

                    self.Stun(stunDur);
                    self.SetMalnourished(true);
                    e.RailgunUse = e.RailgunLimit - 3;
                }
                return true;
            }
            return false;
        }


        private bool Esclass_RG_SpearGet(PhysicalObject obj)
        {
            if (obj != null && obj is Spear s && s.mode == Weapon.Mode.StuckInWall)
            {
                return true;
            }
            return false;
        }

        private void Esclass_RG_Spasm(On.Player.orig_Stun orig, Player self, int st)
        {
            orig(self, st);
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
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
        public static void Esclass_RG_Recoil(Player self, IntVector2 throwDir, float force = 20f, float[] recoilMod = default)
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
            if (self.Malnourished)
            {
                force *= recoilMod[4];
            }

            for (int i = 0; i < 2; i++)
            {
                self.bodyChunks[i].vel.x += throwDir.x * -force;
                self.bodyChunks[i].vel.y += throwDir.y * -force;
            }
        }

        /// <summary>
        /// Finds a valid bodychunk when Railgunner is dualwielding to point the crosshair at it (and also point at it)
        /// </summary>
        public static BodyChunk Esclass_RG_Spotter(Player self)
        {
            float minDist = 2000;
            BodyChunk target = null;

            try
            {
                foreach (UpdatableAndDeletable thing in self.room.updateList)
                {
                    // TODO: Find a way to know the room's side edge coordinates, and also allow vertical targetting later down the line
                    if (
                        thing is Creature creature && creature != self && !(
                            creature is Player &&
                            ModManager.CoopAvailable &&
                            !Custom.rainWorld.options.friendlyFire
                        ) &&
                        !creature.dead &&
                        Custom.Dist(self.mainBodyChunk.pos, creature.firstChunk.pos) < minDist &&
                        Esclass_NE_BodyChecker(
                            creature, 
                            self.bodyChunks[0].pos, 
                            self.bodyChunks[1].pos, 
                            new(self.bodyChunks[0].pos.x + (self.rollDirection * minDist), self.bodyChunks[0].pos.y), 
                            new(self.bodyChunks[1].pos.x + (self.rollDirection * minDist), self.bodyChunks[1].pos.y), 
                            out int bodyChunk) &&
                        Esclass_RG_UninterruptedSight(self.room, self.mainBodyChunk.pos, creature.bodyChunks[bodyChunk].pos)
                    )
                    {
                        minDist = Custom.Dist(self.mainBodyChunk.pos, creature.bodyChunks[bodyChunk].pos);
                        target = creature.bodyChunks[bodyChunk];
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "FECK something happened with spotting a creature");
            }

            return target;
        }

        /// <summary>
        /// Finds a valid creature when Railgunner is dualwielding to point the crosshair at it (and also point at it)
        /// </summary>
        public static Creature Esclass_RG_Spo_t_er(Player self)
        {
            float minDist = 2000;
            Creature target = null;

            try
            {
                foreach (UpdatableAndDeletable thing in self.room.updateList)
                {
                    // TODO: Find a way to know the room's side edge coordinates, and also allow vertical targetting later down the line
                    if (
                        thing is Creature creature && creature != self && !(
                            creature is Player &&
                            ModManager.CoopAvailable &&
                            !Custom.rainWorld.options.friendlyFire
                        ) &&
                        !creature.dead &&
                        Custom.Dist(self.mainBodyChunk.pos, creature.firstChunk.pos) < minDist &&
                        Esclass_NE_BodyChecker(
                            creature, 
                            self.bodyChunks[0].pos, 
                            self.bodyChunks[1].pos, 
                            new(self.bodyChunks[0].pos.x + (self.rollDirection * minDist), self.bodyChunks[0].pos.y), 
                            new(self.bodyChunks[1].pos.x + (self.rollDirection * minDist), self.bodyChunks[1].pos.y), 
                            out int bodyChunk) &&
                        Esclass_RG_UninterruptedSight(self.room, self.mainBodyChunk.pos, creature.bodyChunks[bodyChunk].pos)
                    )
                    {
                        minDist = Custom.Dist(self.mainBodyChunk.pos, creature.bodyChunks[bodyChunk].pos);
                        target = creature;
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "FECK something happened with spotting a creature");
            }

            return target;
        }

        /// <summary>
        /// Checks if there's any solid terrain between Railgunner and the target
        /// </summary>
        public static bool Esclass_RG_UninterruptedSight(Room room, Vector2 railsPos, Vector2 creturPos, bool vertical = false)
        {
            int tileNum = 0;  // Number of tiles between the two points
            bool lefty = (railsPos.x - creturPos.x) > 0;  // May need to reevaluate once my brain isn't eepy
            bool downy = (railsPos.y - creturPos.y) > 0;
            if (vertical)
            {
                tileNum = (int)(Mathf.Abs(railsPos.y - creturPos.y) / 20);
            }
            else
            {
                tileNum = (int)(Mathf.Abs(railsPos.x - creturPos.x) / 20);
            }

            for (int i = 0; i <= tileNum; i++)
            {
                Vector2 position = (lefty || downy)? creturPos : railsPos;
                if (vertical)
                {
                    position.y += i * 20;
                }
                else
                {
                    position.x += i * 20;
                }

                // If there's an obstacle 
                if (room.GetTile(room.GetTilePosition(position)).Solid)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Makes Railgunner point to whoever dares to enter her line of sight
        /// </summary>
        public static void Esclass_RG_DrawHands(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
        {
            // Points the spears at the target when target acquired
            if (e.RailTargetAcquired is not null && e.RailDoubled)
            {
                for (int i = 0; i < self.hands.Length; i++)
                {
                    self.hands[i].reachingForObject = true;
                    self.hands[i].absoluteHuntPos = e.RailTargetAcquired.pos;
                }
            }
        }
    }
}
