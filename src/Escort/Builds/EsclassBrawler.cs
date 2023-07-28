using BepInEx;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Brawler tweak values
        // public static readonly PlayerFeature<> brawler = Player("theescort/brawler/");
        // public static readonly PlayerFeature<float> brawler = PlayerFloat("theescort/brawler/");
        // public static readonly PlayerFeature<float[]> brawler = PlayerFloats("theescort/brawler/");
        public static readonly PlayerFeature<float> brawlerSlideLaunchFac = PlayerFloat("theescort/brawler/slide_launch_fac");
        public static readonly PlayerFeature<float> brawlerDKHypeDmg = PlayerFloat("theescort/brawler/dk_h_dmg");
        public static readonly PlayerFeature<float[]> brawlerSpearVelFac = PlayerFloats("theescort/brawler/spear_vel_fac");
        public static readonly PlayerFeature<float[]> brawlerSpearDmgFac = PlayerFloats("theescort/brawler/spear_dmg_fac");
        public static readonly PlayerFeature<float> brawlerSpearThrust = PlayerFloat("theescort/brawler/spear_thrust");
        public static readonly PlayerFeature<float[]> brawlerSpearShankY = PlayerFloats("theescort/brawler/spear_shank");
        public static readonly PlayerFeature<float> brawlerRockHeight = PlayerFloat("theescort/brawler/rock_height");


        private void Esclass_BL_Tick(Player self, ref Escort e)
        {
            if (e.BrawRevertWall > 0)
            {
                e.BrawRevertWall--;
            }
            if (e.BrawThrowGrab > 0)
            {
                e.BrawThrowGrab--;
            }
        }



        private void Esclass_BL_Update(Player self, ref Escort e)
        {
            // Melee weapon use
            if (e.BrawMeleeWeapon.Count > 0 && e.BrawThrowGrab == 0 && self.grasps[e.BrawThrowUsed] == null)
            {
                if (e.BrawMeleeWeapon.Peek() == null)
                {
                    e.BrawMeleeWeapon.Clear();
                    return;
                }
                Ebug("Weapon mode was: " + e.BrawMeleeWeapon.Peek().mode);
                if (self.room != null && e.BrawMeleeWeapon.Peek().mode == Weapon.Mode.StuckInCreature)
                {
                    self.room.PlaySound(Escort_SFX_Brawler_Shank, e.SFXChunk);
                    self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, e.SFXChunk);
                    if (self.slowMovementStun > 0) self.Blink(30);
                    else 
                    {
                        self.slowMovementStun += 30;
                    }
                }
                else if (e.BrawMeleeWeapon.Peek() is Rock)
                {
                    self.slowMovementStun += 30;
                    e.BrawPunch = false;
                }
                if (e.BrawMeleeWeapon.Peek() is Spear)
                {
                    e.BrawMeleeWeapon.Peek().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
                    self.slowMovementStun += 20;
                }
                if (self.room != null && e.BrawMeleeWeapon.Peek().mode != Weapon.Mode.StuckInWall){
                    e.BrawMeleeWeapon.Peek().ChangeMode(Weapon.Mode.Free);
                    self.SlugcatGrab(e.BrawMeleeWeapon.Pop(), e.BrawThrowUsed);
                }
                else {
                    e.BrawMeleeWeapon.Pop();
                }
                e.BrawThrowGrab = -1;
                e.BrawThrowUsed = -1;
            }

            if (e.BrawMeleeWeapon.Count == 0)
            {
                string leftHand = Esclass_BL_Weapon(self, 0);
                string rightHand = Esclass_BL_Weapon(self, 1);
                e.BrawLastWeapon = (leftHand, rightHand) switch
                {
                    ("supershank", "supershank") => "supershank",
                    ("", not "") => rightHand,
                    (not "", "") => leftHand,
                    (_, _) => ""
                };
            }

            // Brawler wall spear
            if (e.BrawWallSpear.Count > 0 && e.BrawRevertWall == 0)
            {
                e.BrawWallSpear.Pop().doNotTumbleAtLowSpeed = e.BrawWall;
                e.BrawRevertWall = -1;
            }

            // VFX
            if (self.room != null && e.BrawThrowGrab > 0 && e.BrawMeleeWeapon.Count > 0)
            {
                for (int i = -4; i < 5; i++)
                {
                    self.room.AddObject(new Spark(self.mainBodyChunk.pos + new Vector2(self.mainBodyChunk.Rotation.x * 5f, i * 0.5f), new Vector2(self.mainBodyChunk.Rotation.x * (10f - e.BrawThrowGrab) * (3f - (0.4f * Mathf.Abs(i))), e.BrawThrowGrab * 0.5f), new Color(0.8f, 0.4f, 0.6f), null, 4, 6));
                }
            }
        }

        /// <summary>
        /// Stops
        /// </summary>
        private bool Esclass_BL_HeavyCarry(Player self, PhysicalObject obj)
        {
            if (obj.TotalMass <= self.TotalMass * ratioed * 2 && obj is Creature)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (self.grasps[i] != null && self.grasps[i].grabbed != obj && self.grasps[i].grabbed is Spear && self.grasps[1 - i] != null && self.grasps[1 - i].grabbed == obj)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private bool Esclass_BL_Grabability(Player self, PhysicalObject obj, ref Escort e)
        {
            if (obj is Creature c && !c.dead)
            {
                if (obj is JetFish || obj is Fly || obj is TubeWorm || obj is Cicada || obj is MoreSlugcats.Yeek || (obj is Player && obj == self))
                {
                    return false;
                }
                if (c.Stunned && c is Lizard && Esconfig_Dunkin() && !e.LizardDunk)
                {
                    if (e.LizGoForWalk == 0)
                    {
                        e.LizGoForWalk = 320;
                    }
                    e.LizardDunk = true;
                }
                return true;
            }
            return false;
        }

        private bool Esclass_BL_Legality(On.Player.orig_IsCreatureLegalToHoldWithoutStun orig, Player self, Creature grabCheck)
        {
            try
            {
                if (Eshelp_IsMe(self.slugcatStats.name))
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
            return grabCheck.TotalMass <= self.TotalMass * ratioed * 2;
        }


        private void Esclass_BL_ThrownSpear(Player self, Spear spear, ref Escort e, ref float thrust)
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
                if (self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump){
                    thrust *= 2;
                }
                else {
                    thrust *= 0.4f;
                }
                if (e.BrawShankMode)
                {
                    //spear.throwDir = new RWCustom.IntVector2(0, -1);
                    spear.firstChunk.pos = e.BrawShankDir;
                    //spear.firstChunk.vel.y = -(Math.Abs(spear.firstChunk.vel.y)) * bSpearY[0];
                    //spear.firstChunk.pos += new Vector2(0f, bSpearY[1]);
                    spear.firstChunk.vel *= bSpearY[0];
                    //spear.doNotTumbleAtLowSpeed = true;
                    if (e.BrawLastWeapon == "supershank")
                    {
                        spear.firstChunk.vel.x *= 0.15f;
                        spear.doNotTumbleAtLowSpeed = true;
                    }
                    e.BrawShankMode = false;
                    spear.spearDamageBonus = bSpearDmg[1];
                    spear.spearDamageBonus *= Mathf.Max(0.15f, Mathf.InverseLerp(0, 20, 20 - self.slowMovementStun));
                }
                else
                {
                    if (e.BrawShank){
                        e.BrawShank = false;
                    }
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

        /// <summary>
        /// Checks if Brawler has a weapon or not
        /// </summary>
        /// <param name="self"></param>
        /// <param name="grasp"></param>
        /// <returns></returns>
        private static string Esclass_BL_Weapon(Player self, int grasp)
        {
            if (
                self.grasps[grasp]?.grabbed is Spear and not ExplosiveSpear &&
                self.grasps[1 - grasp]?.grabbed is Creature
            ) return "supershank";
            else if (
                self.grasps[grasp]?.grabbed is Spear and not ExplosiveSpear
            ) return "shank";
            else if (
                self.grasps[grasp]?.grabbed is Rock
            ) return "punch";
            else if (false &&
                self.grasps[grasp]?.grabbed is ScavengerBomb
            ) return "powerpunch";
            return "";
        }


        private bool Esclass_BL_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
        {
            if (self.Malnourished || (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection))
            {
                return false;
            }
            for (int j = 0; j < 2; j++)
            {
                // Shank
                if (
                    self.grasps[j] != null &&
                    self.grasps[j].grabbed != null &&
                    self.grasps[j].grabbed is Spear s &&
                    self.grasps[j].grabbed is not ExplosiveSpear &&
                    self.grasps[1 - j] != null &&
                    self.grasps[1 - j].grabbed != null &&
                    self.grasps[1 - j].grabbed is Creature cs
                )
                {
                    if (cs.dead || cs.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || cs.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && cs is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
                    {
                        break;
                    }
                    if (self.slowMovementStun > 30){
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.4f);
                        return true;
                    }
                    Creature c = cs;
                    //c.firstChunk.vel.y += 1f;
                    orig(self, 1 - j, eu);
                    c.mainBodyChunk.vel *= 0.5f;
                    //s.alwaysStickInWalls = false;
                    //if (c.mainBodyChunk != null){
                    //    s.meleeHitChunk = c.mainBodyChunk;
                    //}

                    //s.firstChunk.pos = self.mainBodyChunk.pos + new Vector2(0f, 80f);
                    //s.firstChunk.vel = new Vector2(c.mainBodyChunk.pos.x - s.firstChunk.pos.x, c.mainBodyChunk.pos.y - s.firstChunk.pos.y - 5f);
                    //s.firstChunk.pos = c.mainBodyChunk.pos;
                    //Vector2 v = (c.firstChunk.pos - s.firstChunk.pos).normalized * 3f;
                    e.BrawShankDir = c.mainBodyChunk.pos;
                    Ebug(self, "Hey " + cs.GetType() + ", Like a cuppa tea? Well it's a mugging now.");
                    e.BrawShankMode = true;
                    if (e.BrawMeleeWeapon.Count > 0)
                    {
                        e.BrawMeleeWeapon.Pop().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
                    }
                    e.BrawShankSpearTumbler = s.doNotTumbleAtLowSpeed;
                    e.BrawMeleeWeapon.Push(s);
                    e.BrawThrowGrab = 5;
                    e.BrawThrowUsed = j;
                    e.BrawLastWeapon = "shank";
                    s.doNotTumbleAtLowSpeed = true;
                    orig(self, j, eu);
                    //self.SlugcatGrab(s, j);
                    return true;
                }
                // Alternate Shank
                else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Spear sa && self.grasps[j].grabbed is not ExplosiveSpear)
                {
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
                    {
                        continue;
                    }
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to shank!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1.2f);
                        return true;
                    }
                    Ebug(self, "SHANK!");
                    e.BrawShank = true;
                    e.BrawShankDir = sa.firstChunk.pos;
                    if (e.BrawMeleeWeapon.Count > 0)
                    {
                        e.BrawMeleeWeapon.Pop().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
                    }
                    e.BrawShankSpearTumbler = sa.doNotTumbleAtLowSpeed;
                    e.BrawMeleeWeapon.Push(sa);
                    e.BrawThrowGrab = 5;
                    e.BrawThrowUsed = j;
                    e.BrawLastWeapon = "knife";
                    sa.doNotTumbleAtLowSpeed = true;
                    orig(self, j, eu);
                    return true;
                }
                // Punch
                else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Rock r)
                {
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
                    {
                        continue;
                    }
                    if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Creature)
                    {
                        break;
                    }
                    self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
                    if (self.slowMovementStun > 0)
                    {
                        Ebug(self, "Too tired to punch!");
                        self.Blink(15);
                        Eshelp_Player_Shaker(self, 1f);
                        return true;
                    }
                    Ebug(self, "PUNCH!");
                    e.BrawPunch = true;
                    e.BrawMeleeWeapon.Push(r);
                    e.BrawThrowGrab = 4;
                    e.BrawThrowUsed = j;
                    e.BrawLastWeapon = "brass";
                    orig(self, j, eu);
                    return true;
                }
            }
            return false;
        }


        private void Esclass_BL_RockThrow(Rock self, Player p)
        {
            if (!brawlerRockHeight.TryGet(p, out float roH))
            {
                return;
            }
            self.firstChunk.vel.y *= roH;
        }
    }
}
