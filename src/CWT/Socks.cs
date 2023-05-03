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

            public void EquipBackpack(TubeWorm gb){
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

        private int swapBackpack;
        private int backpackID;
        private int makeBackpackWait;
        private readonly float[] tunnelSpd = new float[]{0.55f, 1f, 1.24f};
        private readonly float[] climbSpd = new float[]{0.6f, 1.07f, 1.33f};
        private readonly float[] walkSpd = new float[]{0.65f, 1.14f, 1.42f};

        public World world { get; set; }

        public Socks(Player self) : base(self){
            this.Eskie = "Socks";
            this.customSprites = 0;
            this.coopMode = false;
            this.makeBackpackWait = 20;
            this.swapBackpack = 40;
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

        public int Escat_socks_skillz(bool malnourished, float tiredness){
            return (malnourished, tiredness) switch{
                (false, < 0.33f) => 2,
                (false, < 0.66f) or (true, < 0.5f) => 1,
                _ => 0
            };
        }

        public float Escat_socks_runspd(bool malnourished, float tiredness, float drugs){
            return Mathf.Lerp(this.walkSpd[0], malnourished? this.walkSpd[1] : this.walkSpd[2], (1 + drugs)-tiredness) + (0.24f * drugs);
        }
        public float Escat_socks_corrspd(bool malnourished, float tiredness, float drugs){
            return Mathf.Lerp(this.tunnelSpd[0], malnourished? this.tunnelSpd[1] : this.tunnelSpd[2], (1 + drugs)-tiredness) + (0.08f * drugs);
        }
        public float Escat_socks_climbspd(bool malnourished, float tiredness, float drugs){
            return Mathf.Lerp(this.climbSpd[0], malnourished? this.climbSpd[1] : this.climbSpd[2], (1 + drugs)-tiredness) + (0.16f * drugs);
        }

        public void Escat_generate_backpack(Player owner){
            if (this.backpack != null){
                Escat_kill_backpack();
            }
            Debug.Log("-> Essocks: GET PACK!");
            this.backpack = new Backpack(owner);
            AbstractCreature ac = new AbstractCreature(
                this.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TubeWorm), 
                null, owner.abstractCreature.pos, owner.room.game.GetNewID());
            //ac.saveCreature = false;
            owner.room.abstractRoom.AddEntity(ac);
            //ac.creatureTemplate = new CreatureTemplate(ac.creatureTemplate);
            //ac.creatureTemplate.name = GrappleBackpack.GrapplingPack.value;
            ac.realizedCreature = this.backpackID switch{
                1 => new LauncherBackpack(ac, this.world, owner),
                _ => new GrappleBackpack(ac, this.world, owner)
            };
            ac.InitiateAI();
            ac.RealizeInRoom();
            this.backpack.EquipBackpack((TubeWorm)ac.realizedCreature);
        }

        public void Escat_swap_backpack(Player owner){
            if (!(this.backpack != null && this.backpack.backpack != null)){
                return;
            }
            if (this.swapBackpack > 0){
                if (owner.input[0].pckp && owner.input[0].jmp){
                    this.swapBackpack--;
                }
                else {
                    this.swapBackpack = 40;
                }
                return;
            }
            if (this.backpack.backpack is GrappleBackpack) this.backpackID = 1;
            else if (this.backpack.backpack is LauncherBackpack) this.backpackID = 0;
            Escat_kill_backpack();
            Escat_clock_backpackReGen(5);
            this.swapBackpack = 40;
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