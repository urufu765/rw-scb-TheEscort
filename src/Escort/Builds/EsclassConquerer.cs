using BepInEx;
using SlugBase.Features;
using System;
using RWCustom;
using MoreSlugcats;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Drawing.Drawing2D;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Conquerer tweak values
        // public static readonly PlayerFeature<> conquerer = Player("theescort/conquerer/");
        // public static readonly PlayerFeature<float> conquerer = PlayerFloat("theescort/conquerer/");
        // public static readonly PlayerFeature<float[]> conquerer = PlayerFloats("theescort/conquerer/");
        public static readonly PlayerFeature<bool> conquererDisallowOversizedLuggage = PlayerBool("theescort/conquerer/nooverlug");


        public void Esclass_CQ_Tick(Player self, ref Escort e)
        {
            // Resets wiggle count of Conquerer is not holding onto a player
            if (!e.ConFkingCretin && e.ConWiggle > 0)
            {
                e.ConWiggle = 0;
            }


            // Shielding status check (if they go into a shortcut or exit reset shield)
            e.ConShieldState = 0;
            if (e.ConShieldDelay >= 20 && e.ConCretinFredom < e.ConMaximumHold)

            {
                if (e.ConWhichCretin == 0) e.ConShieldState = -1;
                else if (e.ConWhichCretin == 1) e.ConShieldState = 1;
            }

            // Shield stun delay. Get stun value from creature (may just need to use cretin.stun.... if it doesn't get affected by pacifying hold)
            if (e.ConShieldStunDelay > 0)
            {
                e.ConShieldStunDelay--;
            }


            // Cretin freedom cooldown
            if (e.ConCretinFredom > 0 && e.ConCretin is null)
            {
                e.ConCretinFredom -= 3;
            }


            // Invincibility cooldown
            if (e.ConInviParryCD > 0 && !self.Stunned)
            {
                e.ConInviParryCD--;
            }
        }


        private void Esclass_CQ_Update(Player self, ref Escort e)
        {
            // Checks whether the grasp is held creature
            e.ConFkingCretin = e.ConHasCretin = false;
            try
            {

                for (int i = 0; i < self.grasps.Length; i++)
                {
                    if (self.grasps[i]?.grabbed is Creature cretin && cretin != self && Esclass_CQ_CretinNotOnBlacklist(cretin, self) && !cretin.dead)  // holding legal cretin
                    {
                        e.ConHasCretin = true;
                        e.ConWhichCretin = i;
                        if (cretin is Player)
                        {
                            e.ConFkingCretin = true;
                        }
                        self.grasps[e.ConWhichCretin].pacifying = true;
                        e.ConCretinFredom++;
                    }
                }
            }
            catch (Exception ex)
            {
                Ebug(ex, "Error in Update: grasp check");
            }

            try
            {
                // Letting go of creature
                if ((!e.ConHasCretin || e.ConCretinFredom >= e.ConMaximumHold) && e.ConWhichCretin >= 0 && e.ConWhichCretin < self.grasps.Length && e.ConCretin is not null)  // not holding cretin
                {
                    Ebug("Conqueror lost grip!");
                    if (self.grasps[e.ConWhichCretin]?.grabbed is Creature c && c == e.ConCretin)
                    {
                        self.ReleaseGrasp(e.ConWhichCretin);
                    }
                    //self.grasps[e.ConWhichCretin].pacifying = false;
                    e.ConWhichCretin = -1;
                    e.ConCretin = null;  // Blanks reference
                }
            }
            catch (Exception ex)
            {
                Ebug(ex, "Error in Conquerer Update: release grasp");
            }

            try
            {
                // Increase shield delay count if creature held and grab held
                // TODO: Disable item swallowing
                if (e.ConHasCretin && self.input[0].pckp && e.ConShieldStunDelay == 0)
                {
                    if (e.ConShieldDelay < 20)
                    {
                        e.ConShieldDelay++;
                    }
                }
                else
                {
                    e.ConShieldDelay = 0;
                }
            }
            catch (Exception ex)
            {
                Ebug(ex, "Error in Conquerer Update: Shield delay");
            }

        }


        /// <summary>
        /// Prevents Conqueror from regurgitating objects
        /// </summary>
        private static void Esclass_CQ_DontRegurgutato(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchLdsfld<ModManager>(nameof(ModManager.MMF))
                );
            }
            catch (Exception e)
            {
                Ebug(e, "IL Match 1 failed");
                throw new Exception("Modmanager.mmf not found", e);
            }
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchCallOrCallvirt<BodyChunk>("get_submersion")
                );
            }
            catch (Exception e)
            {
                Ebug(e, "IL Match 2 failed");
                throw new Exception("Submersion not found", e);
            }
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchLdcR4(0.5f)
                );
            }
            catch (Exception e)
            {
                Ebug(e, "IL Match 3 failed");
                throw new Exception("comparison value not found", e);
            }
            try
            {
                c.GotoNext(MoveType.After,
                    i => i.MatchLdloc(1)
                );
            }
            catch (Exception e)
            {
                Ebug(e, "IL Match 4 failed");
                throw new Exception("evaluation value not found", e);
            }
            Ebug("DontRegurgutato identified point of interest", 0, true);
            try
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(
                    (bool original, Player self) =>
                    {
                        if (Eshelp_IsMe(self.slugcatStats.name, false) && eCon.TryGetValue(self, out Escort e) && e.Conqueror)
                        {
                            return false;
                        }
                        return original;
                    }
                );
            }
            catch (Exception e)
            {
                Ebug(e, "IL Emit failed");
                throw new Exception("IL Emit failed", e);
            }
        }


        private static void Esclass_CQ_GrabUpdate(Player self, ref Escort e)
        {
            // Conqueror shielding ability
            try
            {
                if (e.ConShieldState == self.ThrowDirection && e.ConShieldDelay >= 20 && self.bodyMode != Player.BodyModeIndex.ZeroG && e.ConCretin is not null && !e.ConCretin.dead)
                {
                    Ebug("Conqueror shielded!");
                    PlayerGraphics playg = self.graphicsModule as PlayerGraphics;
                    if (playg is not null)
                    {
                        playg.hands[e.ConWhichCretin].reachingForObject = true;
                        playg.hands[e.ConWhichCretin].absoluteHuntPos = new Vector2(self.mainBodyChunk.pos.x + self.ThrowDirection * 100f, self.mainBodyChunk.pos.y + 75f);
                    }
                }
            }
            catch (Exception ex)
            {
                Ebug(ex, "Error in Conquerer GrabUpdate: Shielding");
            }
        }


        private bool Esclass_CQ_Grabbability(Player self, Creature c, ref Escort e, out Player.ObjectGrabability grabability)
        {
            grabability = Player.ObjectGrabability.BigOneHand;
            if (!Esclass_CQ_CretinNotOnBlacklist(c, self) || c == self)
            {
                return false;
            }
            if (e.ConCretinFredom <= 0 && e.ConCretin is null && !c.dead)
            {
                Ebug("Grabbed a creature!");
                c.Violence(null, null, c.mainBodyChunk, null, Creature.DamageType.Electric, 0f, 40f);
                e.ConCretin = c;
                return true;
            }
            return false;
        }

        private bool Esclass_CQ_HeavyCarry(Player self, Creature c)
        {
            if (Esclass_CQ_CretinNotOnBlacklist(c, self) && self != c)
            {
                return true;
            }
            return false;
        }

        private bool Esclass_CQ_CanConqPickThisUp(Player self, Creature c, ref Escort e)
        {
            if (Esclass_CQ_CretinNotOnBlacklist(c, self) && self != c && !c.dead && e.ConCretinFredom <= 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks whether the creature is not on the Conquerer blacklist (which will allow Conquerer to use the creature as a weapon)
        /// </summary>
        public static bool Esclass_CQ_CretinNotOnBlacklist(Creature cretin, Player self = null)
        {
            // General case
            if (cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug)
            {
                return false;
            }

            // Edge case where small centipedes cannot be used (scenario where Conquerer can use fukin adult centipedes as shields and weapons lol)
            if (cretin is Centipede c && c.Small)
            {
                return false;
            }

            // Edge case where slugpups.
            if (cretin is Player && cretin.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !RWCustom.Custom.rainWorld.options.friendlyFire)
            {
                return false;
            }

            // Edge case where player but it's not arena mode
            if (cretin is Player pal && pal.room?.game?.session is not ArenaGameSession && ModManager.CoopAvailable && !RWCustom.Custom.rainWorld.options.friendlyFire)
            {
                return false;
            }

            // Disallow comically giant creatures if they aren't allowed
            if (cretin is BigEel or Centipede or Deer or MirosBird or Vulture && 
            self is not null && conquererDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized)
            {
                return false;
            }   
            return true;
        }

        /// <summary>
        /// Checks whether the creature is not on the Conquerer blacklist (which will allow Conquerer to use the creature as a weapon) (Single expression for shits and giggles)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklistSingleExpressionA(Creature cretin, Player self = null)
        {
            return !(cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug || (cretin is Centipede c && c.Small) || (cretin is Player pal && (cretin.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || pal.room?.game?.session is not ArenaGameSession && ModManager.CoopAvailable) && !RWCustom.Custom.rainWorld.options.friendlyFire) || (cretin is BigEel or Centipede or Deer or MirosBird or Vulture && self is not null && conquererDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized));
        }

        /// <summary>
        /// Checks whether the creature is not on the Conquerer blacklist (which will allow Conquerer to use the creature as a weapon) (single expression for shits and giggles but it's also more readable)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklistSingleExpressionB(Creature cretin, Player self = null)
        {
            return !(
                cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug || 
                (
                    cretin is Centipede c && 
                    c.Small
                ) || 
                (
                    cretin is Player pal && 
                    (
                        cretin.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || 
                        pal.room?.game?.session is not ArenaGameSession && 
                        ModManager.CoopAvailable
                    ) && 
                    !RWCustom.Custom.rainWorld.options.friendlyFire
                ) || 
                (
                    cretin is BigEel or Centipede or Deer or MirosBird or Vulture && 
                    self is not null && 
                    conquererDisallowOversizedLuggage.TryGet(self, out bool noOversized) && 
                    noOversized
            ));
        }

    }
}
