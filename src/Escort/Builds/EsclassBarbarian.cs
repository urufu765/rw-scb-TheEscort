using BepInEx;
using SlugBase.Features;
using System;
using RWCustom;
using MoreSlugcats;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        // Barbarian tweak values
        // public static readonly PlayerFeature<> barbarian = Player("theescort/barbarian/");
        // public static readonly PlayerFeature<float> barbarian = PlayerFloat("theescort/barbarian/");
        // public static readonly PlayerFeature<float[]> barbarian = PlayerFloats("theescort/barbarian/");
        public static readonly PlayerFeature<bool> barbarianDisallowOversizedLuggage = Player("theescort/barbarian/nooverlug");


        public void Esclass_BB_Tick(Player self, ref Escort e)
        {
            // Resets wiggle count of Barbarian is not holding onto a player
            if (!e.BarFkingCretin && e.BarWiggle > 0)
            {
                e.BarWiggle = 0;
            }

            // Checks whether the grasp is held creature
            e.BarFkingCretin = e.BarHasCretin = false;
            e.BarWhichCretin = -1;
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i].grabbed is Creature cretin && Esclass_BB_CretinNotOnBlacklist(cretin, self))  // holding legal cretin
                {
                    e.BarHasCretin = true;
                    e.BarWhichCretin = i;
                    if (cretin is Player)
                    {
                        e.BarFkingCretin = true;
                    }
                    e.BarCretin = cretin;
                }
            }
            if (!e.BarHasCretin && e.BarCretin is not null)  // not holding cretin
            {
                e.BarCretin = null;  // Blanks reference
            }

            // Increase shield delay count if creature held and grab held
            // TODO: Disable item swallowing
            if (e.BarHasCretin && self.input[0].pckp && e.BarShieldStunDelay == 0)
            {
                if (e.BarShieldDelay < 20)
                {
                    e.BarShieldDelay++;
                }
            }
            else
            {
                e.BarShieldDelay = 0;
            }

            // Shielding status check (if they go into a shortcut or exit reset shield)
            e.BarShieldState = 0;
            if (e.BarShieldDelay >= 20)
            {
                if (e.BarWhichCretin[0]) e.BarShieldState = -1;
                else if (e.BarWhichCretin[1]) e.BarShieldState = 1;
            }

            // Shield stun delay. Get stun value from creature (may just need to use cretin.stun.... if it doesn't get affected by pacifying hold)
            if (e.BarShieldStunDelay > 0)
            {
                e.BarShieldStunDelay--;
            }
        }

        private void Esclass_BB_Update(Player self, ref Escort e)
        {
            
        }


        /// <summary>
        /// Checks whether the creature is not on the Barbarian blacklist (which will allow Barbarian to use the creature as a weapon)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklist(Creature cretin, Player self = null)
        {
            // General case
            if (cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or JellyFish or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug)
            {
                return false;
            }

            // Edge case where small centipedes cannot be used (scenario where Barbarian can use fukin adult centipedes as shields and weapons lol)
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
            self is not null && barbarianDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized)
            {
                return false;
            }   
            return true;
        }

        /// <summary>
        /// Checks whether the creature is not on the Barbarian blacklist (which will allow Barbarian to use the creature as a weapon) (Single expression for shits and giggles)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklistSingleExpressionA(Creature cretin, Player self = null)
        {
            return !(cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or JellyFish or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug || (cretin is Centipede c && c.Small) || (cretin is Player && (cretin.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || pal.room?.game?.session is not ArenaGameSession && ModManager.CoopAvailable) && !RWCustom.Custom.rainWorld.options.friendlyFire) || (cretin is BigEel or Centipede or Deer or MirosBird or Vulture && self is not null && barbarianDisallowOversizedLuggage.TryGet(self, out bool noOversized) && noOversized));
        }

        /// <summary>
        /// Checks whether the creature is not on the Barbarian blacklist (which will allow Barbarian to use the creature as a weapon) (single expression for shits and giggles but it's also more readable)
        /// </summary>
        public static bool Esclass_BB_CretinNotOnBlacklistSingleExpressionB(Creature cretin, Player self = null)
        {
            return !(
                cretin is TubeWorm or JetFish or Fly or Cicada or Overseer or PoleMimic or TentaclePlant or Leech or DaddyLongLegs or TempleGuard or Hazer or JellyFish or SmallNeedleWorm or Spider or MoreSlugcats.Inspector or MoreSlugcats.BigJellyFish or MoreSlugcats.Yeek or MoreSlugcats.StowawayBug || 
                (
                    cretin is Centipede c && 
                    c.Small
                ) || 
                (
                    cretin is Player && 
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
                    barbarianDisallowOversizedLuggage.TryGet(self, out bool noOversized) && 
                    noOversized
            ));
        }

    }
}
