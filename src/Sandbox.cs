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
    partial class Plugin : BaseUnityPlugin
    {
        private void Esclass_Test_Update(Player self){    
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            // Parry
            e.UpdateParryCD(self);
            if (self.Consious && (self.canJump > 0 || self.wantToJump > 0) && self.input[0].pckp && (self.input[0].y < 0 || self.bodyMode == Player.BodyModeIndex.Crawl)){
                e.ThundahParry(self);
            }

            // Sparking when close to death VFX
            if (self.room != null && e.parryNum > e.parryMax - 5){
                self.room.AddObject(new Spark(self.mainBodyChunk.pos, RWCustom.Custom.RNV(), Color.white, null, 4, 8));
            }
        }
    }
}