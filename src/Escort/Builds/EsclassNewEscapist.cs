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
        // New Escapist tweak values
        // public static readonly PlayerFeature<> escapist = Player("theescort/newescapist/");
        // public static readonly PlayerFeature<float> escapist = PlayerFloat("theescort/newescapist/");
        // public static readonly PlayerFeature<float[]> escapist = PlayerFloats("theescort/newescapist/");


        public void Esclass_NE_Tick(Player self, ref Escort e)
        {
            if (e.NEsCooldown > 0)
            {
                e.NEsCooldown--;
            }

            if (e.NEsLastInput.x > 0)
            {
                e.NEsLastInput.x--;
            }
            else if (e.NEsLastInput.x < 0)
            {
                e.NEsLastInput.x++;
            }
            if (e.NEsLastInput.y > 0)
            {
                e.NEsLastInput.y--;
            }
        
            if (e.NEsAbilityTime > 0)
            {
                e.NEsAbilityTime--;
            }
            else if (e.NEsSetCooldown > 0)
            {
                e.NEsCooldown = e.NEsSetCooldown;
                e.NEsSetCooldown = 0;
            }
        }

        public void Esclass_NE_AbsoluteTick(Player self, ref Escort e)
        {
            throw new NotImplementedException();
        }

        private void Esclass_NE_Update(Player self, ref Escort e)
        {
            // Check if player has inputted the direction for ability activation
            if (e.NEsAbilityTime == 0 && e.NEsCooldown == 0)  // Doubletap direction checker
            {
                // Left
                if (self.input[0].x < 0 && self.input[1].x >= 0)
                {
                    if (e.NEsLastInput.x < 0)
                    {
                        e.NEsLastInput.x = 0;
                        e.NEsAbilityTime = 200;
                        Esclass_NE_CreateShadow(self, ref e);
                    }
                    else
                    {
                        e.NEsLastInput.x = -40;
                    }
                }

                // Right
                if (self.input[0].x > 0 && self.input[1].x <= 0)
                {
                    if (e.NEsLastInput.x > 0)
                    {
                        e.NEsLastInput.x = 0;
                        e.NEsAbilityTime = 200;
                        Esclass_NE_CreateShadow(self, ref e);
                    }
                    else
                    {
                        e.NEsLastInput.x = 40;
                    }
                }

                // Up (May not end up implementing)
                if (self.input[0].y > 0 && self.input[1].y <= 0)
                {
                    if (e.NEsLastInput.y > 0)
                    {
                        e.NEsLastInput.y = 0;
                        e.NEsAbilityTime = 200;
                    }
                    else
                    {
                        e.NEsLastInput.y = 40;
                    }
                }

            }
        }



        private static void Esclass_NE_CreateShadow(Player self, ref Escort e)
        {
            if (self.room is null) 
            {
                e.NEsAbilityTime = 0;
                e.NEsSetCooldown = 0;
                return;
            }

            // Realize shadow creature
            e.NEsAbstractShadowPlayer?.Destroy();
            e.NEsAbstractShadowPlayer = new AbstractCreature(self.abstractCreature.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat), null, self.coord, self.room.game.GetNewID())
            {
                state = new PlayerState(e.NEsAbstractShadowPlayer, self.playerState.playerNumber, ShadowEscort, false),
                saveCreature = false
            };
            //e.NEsAbstractShadowPlayer.RealizeInRoom();
            e.NEsAbstractShadowPlayer.realizedCreature = e.NEsShadowPlayer = new ShadowPlayer(e.NEsAbstractShadowPlayer, self.abstractCreature.world, self);
            self.room.AddObject(e.NEsShadowPlayer);

            // Dash
            float dashDistance = 0;
            bool hitWall = false;
            // Check for wall (max dash distance) so you don't go through it
            for (int i = 1; i < 20; i++)
            {
                for (int j = 0; j < self.bodyChunks.Length; j++)
                {
                    Room.Tile rt = self.room.GetTile(self.room.GetTilePosition(new(self.bodyChunks[j].pos.x + (20 * i * self.input[0].x), self.bodyChunks[j].pos.y)));
                    if (rt.Solid) // hit something
                    {
                        hitWall = true;
                        Ebug("Wall detected at: {dashDistance}", 2, true);
                    }
                }
                if (hitWall)
                {
                    break;
                }
                else
                {
                    dashDistance += 20;
                }
            }

            // Check for creature (if they're between your current position and your dash target position)
            bool creaturePass = false;
            bool emptyHanded = self.FreeHand() != -1;
            Vector2 a1, a2, b1, b2;
            a1 = self.bodyChunks[0].pos;
            a2 = self.bodyChunks[1].pos;
            b1 = new(self.bodyChunks[0].pos.x + self.input[0].x * dashDistance, self.bodyChunks[0].pos.y);
            b2 = new(self.bodyChunks[1].pos.x + self.input[0].x * dashDistance, self.bodyChunks[1].pos.y);
            foreach (UpdatableAndDeletable thing in self.room.updateList)
            {
                if (thing is Creature creature && !creature.dead)
                {
                    if (Custom.BetweenLines(creature.firstChunk.pos, a1, a2, b1, b2))
                    {
                        creaturePass = true;  // Reduce dash range to max 200 if you pass a creature or will pass creature
                        if (Custom.Dist(self.firstChunk.pos, creature.firstChunk.pos) < 200)
                        {
                            // Add creature to the vulnerable list if between current position and future position
                            e.NEsVulnerable.Add(creature.abstractCreature);
                            creature.Stun(120);
                            Ebug("There's a creature between!", 2, true);
                        }
                    }
                }
                else if (emptyHanded && thing is Weapon weapon)  // Pick up weapon if empty handed.
                {
                    if (Custom.BetweenLines(weapon.firstChunk.pos, a1, a2, b1, b2))
                    {
                        self.SlugcatGrab(weapon, self.FreeHand());
                        Ebug("Yoink a weapon while you're at it", 2, true);
                    }
                }
            }

            if (dashDistance > 200 && creaturePass)
            {
                dashDistance = 200;
            }
            self.bodyChunks[0].pos.x += self.input[0].x * dashDistance;
            self.bodyChunks[1].pos.x += self.input[0].x * dashDistance;
        }
    }
}
