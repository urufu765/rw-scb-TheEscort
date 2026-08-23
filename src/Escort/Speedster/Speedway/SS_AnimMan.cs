using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster;

public static class Speedway_AnimStuff
{
    public static void UpdateAnimation(Player self, ref Escort e)
    {
        float speed_Mod = (e.SpeDashNCrash ? 1.5f : 0) + ((e.SpeDashNCrash ? 1.5f : 1) * .1f * e.SpeGear);
        float boost_Mod = e.SpeBoosterTunneling;

        switch (self.animation)
        {
            case var a when a == Player.AnimationIndex.Roll:  // ADJUSTMENT NEEDED
                if (self.input[0].x > 0)
                {
                    self.bodyChunks[self.bodyChunks[0].vel.x > self.bodyChunks[1].vel.x ? 0 : 1].vel.x += speed_Mod;
                }
                else
                {
                    self.bodyChunks[self.bodyChunks[0].vel.x < self.bodyChunks[1].vel.x ? 0 : 1].vel.x -= speed_Mod;
                }
                break;
            case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                self.bodyChunks[0].vel.x += Mathf.Sign(self.bodyChunks[0].vel.x) * ((1.7f * speed_Mod) + boost_Mod);
                self.bodyChunks[1].vel.x += Mathf.Sign(self.bodyChunks[1].vel.x) * ((1.55f * speed_Mod) + boost_Mod);
                self.bodyChunks[0].vel.y *= .9f;
                self.bodyChunks[1].vel.y *= .83f;
                break;
            case var a when a == Player.AnimationIndex.HangFromBeam:  // ADJUSTMENT NEEDED
                self.bodyChunks[0].vel.x += self.input[0].x * .5f * speed_Mod;
                self.bodyChunks[1].vel.x += self.input[0].x * .45f * speed_Mod;
                break;
            case var a when a == Player.AnimationIndex.StandOnBeam:  // ADJUSTMENT NEEDED
                self.bodyChunks[0].vel.x += self.input[0].x * 1.05f * speed_Mod;
                self.bodyChunks[1].vel.x += self.input[0].x * 1.1f * speed_Mod;
                break;
            case var a when a == Player.AnimationIndex.ClimbOnBeam:  // ADJUSTMENT NEEDED
                if (self.input[0].y > 0)
                {
                    self.bodyChunks[0].vel.y += self.input[0].y * (.7f + .2f * Mathf.InverseLerp(0, 5, self.gravity)) * speed_Mod;
                    self.bodyChunks[1].vel.y += self.input[0].y * (.6f + .2f * Mathf.InverseLerp(0, 5, self.gravity)) * speed_Mod;
                }
                else
                {
                    self.bodyChunks[0].vel.y += self.input[0].y * (.6f + .2f * Mathf.InverseLerp(5, 0, self.gravity)) * speed_Mod;
                    self.bodyChunks[1].vel.y += self.input[0].y * (.7f + .2f * Mathf.InverseLerp(5, 0, self.gravity)) * speed_Mod;
                }
                break;
            case var a when a == Player.AnimationIndex.RocketJump && self.allowRoll == 0:  // ADJUSTMENT NEEDED
                // Create new variable that slowly decreases boost so player doesn't go yeet
                self.bodyChunks[0].vel.x += Mathf.Sign(self.bodyChunks[0].vel.x) * 1.7f * speed_Mod;
                self.bodyChunks[1].vel.x += Mathf.Sign(self.bodyChunks[1].vel.x) * 1.8f * speed_Mod;
                break;
            case var a when a == Player.AnimationIndex.DownOnFours:  // ADJUSTMENT NEEDED
                self.bodyChunks[0].vel.x += self.input[0].x * 1.5f * speed_Mod;
                self.bodyChunks[1].vel.x += self.input[0].x * 1.45f * speed_Mod;
                break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
                // case var a when a == Player.AnimationIndex.BellySlide:  // ADJUSTMENT NEEDED
                //     break;
        }
    }
}