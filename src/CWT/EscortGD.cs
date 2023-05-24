using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool float_state;
        public void EscortGD()
        {
            Gilded = false;
            float_state = false;
        }

        public void Escat_float_state(Player self, bool status = true){
            if (status){
                float_state = true;
                self.wantToJump = 0;
                self.room?.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, SFXChunk, false, 1f, 0.6f);
                for (int i = 0; i < 7; i++){
                    self.room?.AddObject(new WaterDrip(self.bodyChunks[1].pos, RWCustom.Custom.DegToVec(Random.value * 360) * Mathf.Lerp(4, 20, Random.value), false));
                }
            }
            else{
                self.room?.PlaySound(SoundID.HUD_Pause_Game, SFXChunk, loop: false, 1f, 0.5f);
                float_state = false;
            }
        }
    }
}