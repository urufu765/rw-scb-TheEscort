using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool GildFloatState;
        public int GildFloatFloat;
        public int GildMoonJump;
        public readonly int GildMoonJumpMax = 20;
        public bool GildCrush;
        public int GildCrushTime;
        public bool GildCrushReady;
        public int GildPower;
        public int GildLevitateLimit;
        public int GildLaser;  // Laser
        public int GildBlast;  // Cone
        
        public void EscortGD()
        {
            Gilded = false;
            GildFloatState = false;
            GildFloatFloat = 0;
            GildMoonJump = 0;
            GildCrush = false;  // Stomping state
            GildCrushTime = 0;  // Chargeup
            GildCrushReady = false;  // Stops Gilded from stomping multiple times in the air
            GildPower = 2000;
            GildLevitateLimit = 200;
            GildLaser = 0;
            GildBlast = 0;
        }

        public void Escat_float_state(Player self, bool status = true){
            if (status){
                GildFloatState = true;
                self.wantToJump = 0;
                self.room?.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, SFXChunk, false, 1f, 0.6f);
                for (int i = 0; i < 7; i++){
                    self.room?.AddObject(new WaterDrip(self.bodyChunks[1].pos, RWCustom.Custom.DegToVec(Random.value * 360) * Mathf.Lerp(4, 20, Random.value), false));
                }
            }
            else{
                self.room?.PlaySound(SoundID.HUD_Pause_Game, SFXChunk, loop: false, 1f, 0.5f);
                GildFloatState = false;
            }
        }
    }
}