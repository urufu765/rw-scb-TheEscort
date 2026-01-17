using BepInEx;
using IL.MoreSlugcats;
using SlugBase.Features;
using System;
using TheEscort.Brawler;
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
        public static readonly PlayerFeature<float> brawlerSlideLaunchFac;
        public static readonly PlayerFeature<float> brawlerDKHypeDmg;
        public static readonly PlayerFeature<float[]> brawlerSpearVelFac;
        public static readonly PlayerFeature<float[]> brawlerSpearDmgFac;
        public static readonly PlayerFeature<float> brawlerSpearThrust;
        public static readonly PlayerFeature<float[]> brawlerSpearShankY;
        public static readonly PlayerFeature<float> brawlerRockHeight;


        public static void Esclass_BL_Tick(Player self, ref Escort e)
        {
            if (e.BrawRevertWall > 0)
            {
                e.BrawRevertWall--;
            }
            if (e.BrawThrowGrab > 0)
            {
                e.BrawThrowGrab--;
            }

            if (e.BrawExpIFrameReady > 0)
            {
                e.BrawExpIFrameReady--;
            }

            if (e.BrawExpIFrames > 0)
            {
                e.BrawExpIFrames--;
            }
        }



        public static void Esclass_BL_Update(Player self, ref Escort e)
        {
            // Melee weapon use
            try
            {
                if (e.BrawMeleeWeapon.Count > 0 && e.BrawThrowGrab == 0 && self.grasps[e.BrawThrowUsed] == null)
                {
                    if (e.BrawMeleeWeapon.Peek() == null)
                    {
                        Ebug("NULL IN STACK!");
                        e.BrawMeleeWeapon.Clear();
                        return;
                    }
                    bool retrieveExplosive = false;
                    Ebug("Weapon mode was: " + e.BrawMeleeWeapon.Peek().mode);

                    // Post weapon usage cooldowns
                    try
                    {
                        if (self.room != null && e.BrawMeleeWeapon.Peek().mode == Weapon.Mode.StuckInCreature)
                        {
                            self.room.PlaySound(Escort_SFX_Brawler_Shank, e.SFXChunk);
                            if (e.BrawMeleeWeapon.Peek() is ExplosiveSpear es)
                            {
                                es.igniteCounter = e.BrawExpspearAt;
                            }
                            else
                            {
                                self.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, e.SFXChunk);
                                if (self.slowMovementStun > 0) self.Blink(30);
                                else
                                {
                                    self.slowMovementStun += 20;
                                }
                            }
                        }
                        else if (e.BrawMeleeWeapon.Peek() is Rock)
                        {
                            self.slowMovementStun += 25;
                            e.BrawLastWeapon = Melee.None;
                        }
                        else if (e.BrawMeleeWeapon.Peek() is ExplosiveSpear es && !(es.mode == Weapon.Mode.StuckInWall || es.exploded || es.slatedForDeletetion))
                        {
                            self.slowMovementStun += 10;
                            es.igniteCounter = 0;
                            es.explodeAt = e.BrawExpspearAt;
                            retrieveExplosive = true;
                        }
                        else if (e.BrawMeleeWeapon.Peek() is ScavengerBomb sb && !sb.slatedForDeletetion)
                        {
                            self.slowMovementStun += 40;
                            sb.ignited = false;
                            sb.burn = 0f;
                            retrieveExplosive = true;
                        }
                    }
                    catch (Exception exceptPostWeapEff)
                    {
                        Ebug(exceptPostWeapEff, "Post Weapon Usage Cooldown fail!");
                        throw exceptPostWeapEff;
                    }

                    // Spear post effects
                    try
                    {
                        if (e.BrawMeleeWeapon.Peek() is Spear)
                        {
                            e.BrawMeleeWeapon.Peek().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
                            self.slowMovementStun += 20;
                        }
                    }
                    catch (Exception exceptPostSpearEff)
                    {
                        Ebug(exceptPostSpearEff, "Post Spear Usage Thing Fail!");
                        throw exceptPostSpearEff;
                    }

                    // Item retrieval effect
                    try
                    {
                        // bool wallCondition = true;
                        // if (ModManager.MSC && MoreSlugcats.MMF.cfgDislodgeSpears.Value && self?.room is not null)
                        // {
                        //     wallCondition = false;
                        //     if (self.input[0].x != 0)
                        //     {
                        //         wallCondition = self.IsTileSolid(0, self.input[0].x, 0);
                        //     }
                        //     else if (self.input[0].y != 0)
                        //     {
                        //         if (self.animation == Player.AnimationIndex.Flip)
                        //         {
                        //             wallCondition = true;
                        //         }
                        //         else if (self.bodyMode == Player.BodyModeIndex.ZeroG)
                        //         {
                        //             wallCondition = self.IsTileSolid(0, 0, self.input[0].y) || self.IsTileSolid(1, 0, self.input[0].y);
                        //         }
                        //     }
                        // }
                        if (
                            self.room != null && 
                            e.BrawMeleeWeapon.Peek().mode != Weapon.Mode.StuckInWall && 
                            !(e.BrawWeaponInAction is Melee.ExPunch or Melee.ExShank && !retrieveExplosive))
                        {
                            e.BrawMeleeWeapon.Peek().ChangeMode(Weapon.Mode.Free);
                            self.SlugcatGrab(e.BrawMeleeWeapon.Pop(), e.BrawThrowUsed);
                        }
                        else
                        {
                            e.BrawMeleeWeapon.Pop();
                        }
                    }
                    catch (Exception exceptGetBackItem)
                    {
                        Ebug(exceptGetBackItem, "Item retrieval fail!");
                        throw exceptGetBackItem;
                    }

                    if (!retrieveExplosive)
                    {
                        if (e.BrawWeaponInAction is Melee.ExPunch)
                        {
                            self.slowMovementStun += 60;
                        }
                        else if (e.BrawWeaponInAction is Melee.ExShank)
                        {
                            self.slowMovementStun += 30;
                        }
                    }
                    if (e.isChunko)
                    {
                        self.slowMovementStun += (int)(10 * self.TotalMass / e.originalMass) - 10;
                    }
                    e.BrawWeaponInAction = Melee.None;
                    e.BrawSuperShank = false;
                    e.BrawLastWeapon = Melee.None;
                    e.BrawSetCooldown = self.slowMovementStun;
                    e.BrawThrowGrab = -1;
                    e.BrawThrowUsed = -1;
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Something went wrong when applying effect for melee weapon!");
            }

            if (e.BrawMeleeWeapon.Count == 0)
            {
                BL_Melee.UpdateMeleeStatus(self, ref e);
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






        // /// <summary>
        // /// Checks if Brawler has a weapon or not
        // /// </summary>
        // /// <param name="self"></param>
        // /// <param name="grasp"></param>
        // /// <returns></returns>
        // public static string Esclass_BL_Weapon(Player self, int grasp)
        // {
        //     if (
        //         self.grasps[grasp]?.grabbed is Spear and not ExplosiveSpear &&
        //         self.grasps[1 - grasp]?.grabbed is Creature
        //     ) return "supershank";
        //     else if (
        //         self.grasps[grasp]?.grabbed is Spear and not ExplosiveSpear
        //     ) return "shank";
        //     else if (
        //         self.grasps[grasp]?.grabbed is Rock
        //     ) return "punch";
        //     else if (
        //         self.grasps[grasp]?.grabbed is ScavengerBomb
        //     ) return "powerpunch";
        //     return "";
        // }
        // public static bool Esclass_BL_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu, ref Escort e)
        // {
        //     if (self.animation == Player.AnimationIndex.BellySlide && self.slideDirection == self.ThrowDirection)
        //     {
        //         return false;
        //     }
        //     for (int j = 0; j < 2; j++)
        //     {
        //         // Shank
        //         if (
        //             self.grasps[j] != null &&
        //             self.grasps[j].grabbed != null &&
        //             self.grasps[j].grabbed is Spear s &&
        //             self.grasps[j].grabbed is not ExplosiveSpear &&
        //             self.grasps[1 - j] != null &&
        //             self.grasps[1 - j].grabbed != null &&
        //             self.grasps[1 - j].grabbed is Creature cs
        //         )
        //         {
        //             if (cs.dead || cs.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || cs.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && cs is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
        //             {
        //                 break;
        //             }
        //             if (self.slowMovementStun > 30)
        //             {
        //                 Ebug(self, "Too tired to shank!");
        //                 self.Blink(15);
        //                 Eshelp_Player_Shaker(self, 1.4f);
        //                 return true;
        //             }
        //             Creature c = cs;
        //             //c.firstChunk.vel.y += 1f;
        //             orig(self, 1 - j, eu);
        //             c.mainBodyChunk.vel *= 0.5f;
        //             //s.alwaysStickInWalls = false;
        //             //if (c.mainBodyChunk != null){
        //             //    s.meleeHitChunk = c.mainBodyChunk;
        //             //}

        //             //s.firstChunk.pos = self.mainBodyChunk.pos + new Vector2(0f, 80f);
        //             //s.firstChunk.vel = new Vector2(c.mainBodyChunk.pos.x - s.firstChunk.pos.x, c.mainBodyChunk.pos.y - s.firstChunk.pos.y - 5f);
        //             //s.firstChunk.pos = c.mainBodyChunk.pos;
        //             //Vector2 v = (c.firstChunk.pos - s.firstChunk.pos).normalized * 3f;
        //             e.BrawShankDir = c.mainBodyChunk.pos;
        //             Ebug(self, "Hey " + cs.GetType() + ", Like a cuppa tea? Well it's a mugging now.");
        //             e.BrawShankMode = true;
        //             if (e.BrawMeleeWeapon.Count > 0)
        //             {
        //                 e.BrawMeleeWeapon.Pop().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
        //             }
        //             e.BrawShankSpearTumbler = s.doNotTumbleAtLowSpeed;
        //             e.BrawMeleeWeapon.Push(s);
        //             e.BrawThrowGrab = 5;
        //             e.BrawThrowUsed = j;
        //             e.BrawLastWeapon = "supershank";
        //             s.doNotTumbleAtLowSpeed = true;
        //             orig(self, j, eu);
        //             //self.SlugcatGrab(s, j);
        //             return true;
        //         }
        //         // Alternate Shank
        //         else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Spear sa && self.grasps[j].grabbed is not ExplosiveSpear)
        //         {
        //             if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
        //             {
        //                 continue;
        //             }
        //             self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
        //             if (self.slowMovementStun > 0)
        //             {
        //                 Ebug(self, "Too tired to shank!");
        //                 self.Blink(15);
        //                 Eshelp_Player_Shaker(self, 1.2f);
        //                 return true;
        //             }
        //             Ebug(self, "SHANK!");
        //             e.BrawShank = true;
        //             e.BrawShankDir = sa.firstChunk.pos;
        //             if (e.BrawMeleeWeapon.Count > 0)
        //             {
        //                 e.BrawMeleeWeapon.Pop().doNotTumbleAtLowSpeed = e.BrawShankSpearTumbler;
        //             }
        //             e.BrawShankSpearTumbler = sa.doNotTumbleAtLowSpeed;
        //             e.BrawMeleeWeapon.Push(sa);
        //             e.BrawThrowGrab = 5;
        //             e.BrawThrowUsed = j;
        //             e.BrawLastWeapon = "shank";
        //             sa.doNotTumbleAtLowSpeed = true;
        //             orig(self, j, eu);
        //             return true;
        //         }
        //         // Punch
        //         else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is Rock r)
        //         {
        //             if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
        //             {
        //                 continue;
        //             }
        //             if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Creature)
        //             {
        //                 break;
        //             }
        //             self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
        //             if (self.slowMovementStun > 0)
        //             {
        //                 Ebug(self, "Too tired to punch!");
        //                 self.Blink(15);
        //                 Eshelp_Player_Shaker(self, 1f);
        //                 return true;
        //             }
        //             Ebug(self, "PUNCH!");
        //             e.BrawPunch = true;
        //             e.BrawMeleeWeapon.Push(r);
        //             e.BrawThrowGrab = 4;
        //             e.BrawThrowUsed = j;
        //             e.BrawLastWeapon = "punch";
        //             orig(self, j, eu);
        //             return true;
        //         }
        //         // Explosive punch
        //         else if (self.grasps[j] != null && self.grasps[j].grabbed != null && self.grasps[j].grabbed is ScavengerBomb sb)
        //         {
        //             if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Weapon)
        //             {
        //                 continue;
        //             }
        //             if (self.grasps[1 - j] != null && self.grasps[1 - j].grabbed != null && self.grasps[1 - j].grabbed is Creature)
        //             {
        //                 break;
        //             }
        //             self.aerobicLevel = Mathf.Max(0, self.aerobicLevel - 0.07f);
        //             if (self.slowMovementStun > 0)
        //             {
        //                 Ebug(self, "Too tired to explopunch!");
        //                 self.Blink(15);
        //                 Eshelp_Player_Shaker(self, 1f);
        //                 return true;
        //             }
        //             try
        //             {
        //                 Ebug(self, "EXPLOPUNCH!");
        //                 e.BrawExPunch = true;
        //                 e.BrawMeleeWeapon.Push(sb);
        //                 e.BrawThrowGrab = 4;
        //                 e.BrawThrowUsed = j;
        //                 e.BrawLastWeapon = "powerpunch";
        //                 orig(self, j, eu);
        //             }
        //             catch (Exception exploerr)
        //             {
        //                 Ebug(exploerr, "Failure to explopunch!");
        //             }
        //             return true;
        //         }
        //     }
        //     return false;
        // }
    }
}
