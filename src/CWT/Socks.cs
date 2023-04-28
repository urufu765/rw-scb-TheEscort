using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

namespace TheEscort
{
    public class Socks : Escort{

        public class Backpack{
            public Player owner;
            public TubeWorm backpack;
            public Player.AbstractOnBackStick abstractBackpack;
            public bool HasABackpack => backpack != null;

            public Backpack(Player owner){
                this.owner = owner;
            }

            public void GraphicsModuleUpdated(bool actuallyViewed, bool eu){
                if (backpack == null){
                    return;
                }
                if (backpack.slatedForDeletetion){
                    if (abstractBackpack != null){
                        abstractBackpack.Deactivate();
                    }
                    backpack = null;
                    return;
                }
                if (!owner.dead && backpack.dead){
                    backpack.Destroy();
                    return;
                }
                // Vector2 vector = owner.mainBodyChunk.pos;
                // Vector2 vector2 = owner.bodyChunks[1].pos;
                // if (owner.graphicsModule != null)
                // {
                //     vector = Vector2.Lerp((owner.graphicsModule as PlayerGraphics).drawPositions[0, 0], (owner.graphicsModule as PlayerGraphics).head.pos, 0.2f);
                //     vector2 = (owner.graphicsModule as PlayerGraphics).drawPositions[1, 0];
                // }
                ChangeOverlap(newOverlap: false);
                backpack.mainBodyChunk.MoveFromOutsideMyUpdate(eu, (owner.graphicsModule != null) ? (owner.graphicsModule as PlayerGraphics).head.pos : owner.mainBodyChunk.pos);
                backpack.mainBodyChunk.vel = owner.mainBodyChunk.vel;
                //
            }

            public void ChangeOverlap(bool newOverlap)
            {
                backpack.CollideWithObjects = newOverlap;
                backpack.canBeHitByWeapons = newOverlap;
                //backpack.onBack = (newOverlap ? null : owner);
                if (backpack.graphicsModule != null && owner.room != null)
                {
                    for (int i = 0; i < owner.room.game.cameras.Length; i++)
                    {
                        owner.room.game.cameras[i].MoveObjectToContainer(backpack.graphicsModule, owner.room.game.cameras[i].ReturnFContainer((!newOverlap) ? "Background" : "Midground"));
                    }
                }
            }

            public void EquipBackpack(GrappleBackpack gb){
                backpack = gb;
                if (abstractBackpack != null){
                    abstractBackpack.Deactivate();
                }
                abstractBackpack = new Player.AbstractOnBackStick(owner.abstractPhysicalObject, gb.abstractPhysicalObject);
                //owner.tubeWorm = gb;
                gb.mainBodyChunk.pos = owner.mainBodyChunk.pos;
                owner.SlugcatGrab(gb, 2);
            }
        }

        public bool coopMode;
        public Backpack backpack;
        //public Morebackpacks.SlugNPCAI sAI;
        public readonly int sID = 3118;

        private int makeBackpackWait;

        public World world { get; set; }

        public Socks(Player self) : base(self){
            this.Eskie = "Socks";
            this.customSprites = 0;
            this.coopMode = false;
            this.makeBackpackWait = 20;
        }

        public void Escat_kill_backpack(){
            if (this.backpack != null){
                if (this.backpack.abstractBackpack != null){
                    this.backpack.abstractBackpack.Deactivate();
                    this.backpack.abstractBackpack = null;
                }
                if (this.backpack.backpack != null){
                    this.backpack.backpack.Destroy();
                }
                this.backpack = null;

            }
        }

        public void Escat_generate_backpack(Player owner){
            if (this.backpack != null && this.backpack.abstractBackpack != null && this.backpack.backpack != null){
                this.backpack.abstractBackpack.Deactivate();
                this.backpack.abstractBackpack = null;
                this.backpack.backpack.Destroy();
                this.backpack = null;
            }
            Debug.Log("-> Essocks: GET PACK!");
            this.backpack = new Backpack(owner);
            AbstractCreature ac = new AbstractCreature(
                this.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TubeWorm), 
                null, owner.abstractCreature.pos, owner.room.game.GetNewID());
            owner.room.abstractRoom.AddEntity(ac);
            ac.creatureTemplate.name = GrappleBackpack.GrapplingPack.value;
            ac.realizedCreature = new GrappleBackpack(ac, this.world, owner);
            ac.InitiateAI();
            ac.RealizeInRoom();
            this.backpack.EquipBackpack((GrappleBackpack)ac.realizedCreature);
        }

        public bool Escat_clock_backpackReGen(int refresh=0){
            this.makeBackpackWait += refresh;
            if (this.makeBackpackWait > 0){
                this.makeBackpackWait--;
                return false;
            }
            else {
                return true;
            }
        }
    }
}