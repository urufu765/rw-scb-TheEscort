using System;
using UnityEngine;



namespace TheEscort{
    public class Escort{
        public int DropKickCD;
        public int CentiCD;
        public float RollinCount;
        public int parrySlideLean;
        public int parryAirLean;
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
        public bool parryTech;
        public int parryExtras;
        
        public Escort(Player player){
            this.DropKickCD = 0;
            this.CentiCD = 0;
            this.iFrames = 0;
            this.parrySlideLean = 0;
            this.parryAirLean = 0;
            this.Cometted = false;
            this.CometFrames = 0;
            this.RollinCount = 0f;
            this.LizardDunk = false;
            this.ParrySuccess = false;
            this.ElectroParry = false;
            this.RollinSFXChunk = player.bodyChunks[0];
            this.spriteQueue = -1;

            // Build stuff
            this.combatTech = true;  // Bruiser
            this.parryTech = false;  // Deflector
            this.parryExtras = 0;    // Deflector
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
