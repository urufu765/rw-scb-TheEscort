using System;
using UnityEngine;



namespace TheEscort{
    public class Escort{
        public int DropKickCD;
        public int CentiCD;
        public float RollinCount;
        public int parryLean;
        public int iFrames;
        public bool Cometted;
        public int CometFrames;
        public BodyChunk RollinSFXChunk;
        public DynamicSoundLoop Rollin;
        public bool LizardDunk;
        public bool ParrySuccess;
        public bool ElectroParry;
        public int spriteQueue;
        
        // Build stuff
        public bool combatTech;
        
        public Escort(Player player){
            this.DropKickCD = 0;
            this.CentiCD = 0;
            this.iFrames = 0;
            this.parryLean = 0;
            this.Cometted = false;
            this.CometFrames = 0;
            this.RollinCount = 0f;
            this.LizardDunk = false;
            this.ParrySuccess = false;
            this.ElectroParry = false;
            this.RollinSFXChunk = player.bodyChunks[0];
            this.spriteQueue = 0;

            // Build stuff
            this.combatTech = true;
        }

        public void Escort_set_roller(SoundID sound){
            try{
                this.Rollin = new ChunkDynamicSoundLoop(RollinSFXChunk);
                this.Rollin.sound = sound;
            } catch (Exception e){
                Debug.Log("Something went horribly wrong when setting up the rolling sound!");
                Debug.Log(e.Message);
            }
        }
    }
}
