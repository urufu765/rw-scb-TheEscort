using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;

namespace TheEscort
{
    
    public class GrappleBackpack : TubeWorm
    {
        public static readonly CreatureTemplate.Type GrapplingPack = new CreatureTemplate.Type("BackpackWorm", register:true);

        private Player attachment {get; set;}

        public GrappleBackpack(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++){
                this.bodyChunks[i].mass *= 0.01f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            this.attachment = null;
            Debug.Log("Backpack Initiated!");
        }

        public GrappleBackpack(AbstractCreature abstractCreature, World world, Player owner) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++){
                this.bodyChunks[i].mass *= 0.01f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            this.attachment = owner;
            Debug.Log("Backpack Initiated!");
        }


        public void setOwner(Player owner){
            this.attachment = owner;
        }


        public override void InitiateGraphicsModule()
        {
            Debug.Log("Backpack Graphics initiated");
            if (this.graphicsModule == null){
                this.graphicsModule = new GrappleBackpackGraphics(this);
            }
        }

        public override void Update(bool eu)
        {
            this.sleeping = false;
            // Destroy worm upon resting in shelter
            if (this.grabbedBy.Count == 0){
                Debug.Log("Sending backpack to Lost & Find");
                this.Destroy();
                return;
            }

            if (this.useBool){
                for (int i = 0; i < this.tongues.Length; i++){
                    if (this.tongues[i].Attached){
                        for (int j = 0; j < this.grabbedBy.Count; j++){
                            if (grabbedBy[j].grabber is Player p){
                                if (p.animation == Player.AnimationIndex.Flip){
                                    p.mainBodyChunk.vel *= 2f;
                                }
                                p.bodyChunks[0].vel.y += 15f;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            base.Update(eu);
        }

        public override bool CanBeGrabbed(Creature grabber)
        {
            if (this.grabbedBy.Count > 0){
                return false;
            }
            return base.CanBeGrabbed(grabber);
        }

        public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos onAppendagePos, DamageType type, float damage, float stunBonus)
        {
            base.Violence(source, directionAndMomentum, hitChunk, onAppendagePos, type, 0f, stunBonus);
        }

        public new bool JumpButton(Player plr){
            if (plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.ZeroG){
                this.useBool = true;
                return false;
            }
            return base.JumpButton(plr);
        }

    }

    public class GrappleBackpackGraphics : TubeWormGraphics
    {
        public Color ringColor = Color.gray;
        public Color tongueColor = Color.gray;
        public GrappleBackpackGraphics(PhysicalObject ow) : base(ow)
        {
            this.color = Color.white;
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette);
            //sLeaser.sprites[2].color = tongueColor;
            for (int i = 3; i < 5; i++){
                sLeaser.sprites[i].color = ringColor;
            }
            for (int j = 0; j < (sLeaser.sprites[2] as TriangleMesh).verticeColors.Length; j++)
            {
                (sLeaser.sprites[2] as TriangleMesh).verticeColors[j] = tongueColor;
            }
        }

        public void setColor(string name, Color color){
            if (name == "Backpack"){
                this.color = color;
            }
            else if (name == "Accents"){
                this.ringColor = color;
            }
            else if (name == "Tongue"){
                this.tongueColor = color;
            }
        }
    }
}
