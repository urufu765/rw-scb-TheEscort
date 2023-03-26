using System;


namespace TheEscort{
    public class Escort{
        public int EscortDropKickCooldown;
        public int EscortStunSlideCooldown;
        public float EscortRollinCounter;
        public BodyChunk EscortSoundBodyChunk;
        public DynamicSoundLoop EscortRoller;
        public Escort(Player player){
            this.EscortDropKickCooldown = 0;
            this.EscortStunSlideCooldown = 0;
            this.EscortRollinCounter = 0f;
            this.EscortSoundBodyChunk = player.bodyChunks[0];
        }

        public void Escort_set_roller(SoundID sound){
            this.EscortRoller = new ChunkDynamicSoundLoop(EscortSoundBodyChunk);
            this.EscortRoller.sound = sound;
        }
    }
}
