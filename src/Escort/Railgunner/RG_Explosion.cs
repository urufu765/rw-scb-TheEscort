using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Railgunner;

public static class RG_Exploder
{
    /// <summary>
    /// Makes Railgunner go BOOM
    /// </summary>
    public static void InnerSplosion(Player self, float range, bool lethal = false)
    {
        try
        {
            Color c = RG.ColorRG;
            Vector2 v = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            Room room = self.room;
            room.FX_OverchargeExplosion(v, c);

            if (lethal)
            {
                room.AddObject(new Explosion(room, self, v, 2, range / 3f, 60f, 100f, 10f, 0.4f, self, 0.7f, 2f, 0f));
                room.AddObject(new Explosion(room, self, v, 8, range * 1.5f, 60f, 0.5f, 600f, 0.4f, self, 0.01f, 200f, 0f));
            }
            else
            {
                room.AddObject(new Explosion(room, self, v, 5, range, 60f, 1f, range, 0.4f, self, 0.01f, range / 4, 0.8f));
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Explosioning FAILED UH OH");
        }
    }



    /// <summary>
    /// Applies recoil on the player
    /// </summary>
    public static void Recoil(Player self, IntVector2 throwDir, float force = 20f, bool glassCannonBonus = false)
    {
        float xForce = force, yForce = force;


        // Up/down velocity adjustment (so recoil jumps are a thing (and you don't get stunned when recoiling downwards))
        if (self.bodyMode != Player.BodyModeIndex.ZeroG)
        {
            if (throwDir.y > 0)  // Reduce downwards recoil
            {
                yForce *= .7f;
            }
            else if (throwDir.y < 0)  // Increase upwards recoil
            {
                yForce *= 1;
            }
            if (throwDir.x != 0 && throwDir.y != 0)
            {
                xForce *= .9f;
                yForce *= .6f;
            }
        }

        // Reduce recoil if proned/standing with the power of friction
        if (self.bodyMode == Player.BodyModeIndex.Crawl)
        {
            xForce *= .6f;
            yForce *= .6f;
        }
        else if (self.bodyMode == Player.BodyModeIndex.Stand)
        {
            xForce *= .9f;
            yForce *= .9f;
        }

        if (glassCannonBonus)
        {
            xForce *= 1.15f;
            yForce *= 1.15f;
        }

        self.rollDirection = 0;
        for (int i = 0; i < 2; i++)
        {
            self.bodyChunks[i].vel.x += throwDir.x * -xForce;
            self.bodyChunks[i].vel.y += throwDir.y * -yForce;
        }

        // if (ModManager.MSC)  // TODO: Once Escort escapes MSC dependency, figure out an alternative
        // {
        //     self.immuneToFallDamage = 2;
        // }
        self.immuneToFallDamage = 2;

        //self.animation = Player.AnimationIndex.None;
        self.room?.SFX_RecoilBoom(self.mainBodyChunk.pos, self.Submersion > 0.5f);
    }


    /// <summary>
    /// Do the death explosion
    /// </summary>
    /// <param name="self"></param>
    /// <param name="room"></param>
    /// <param name="e"></param>
    public static void DeathExplosion(Player self, Room room, ref Escort e)
    {
        if (UnityEngine.Random.value > (e.RailFrail ? 0.75f : 0.25f))
        {
            int railGun = e.RailgunUse;
            e.RailgunUse = e.RailgunCD = 0;
            InnerSplosion(self, 600);
            e.Escat_RG_SetGlassMode(true);
            //self.stun += e.RailFrail ? 320 : 160;
            int stunDur = e.RailFrail ? 160 : 0;
            if (self.room?.game?.session is StoryGameSession sgs)
            {
                stunDur += 30 * (12 - sgs.saveState.deathPersistentSaveData.karma);
            }

            RG_Shocker.StunWave(self, 60 * railGun, Mathf.Lerp(0.01f, 0.6f, (float)railGun / e.RailgunLimit), 10 * railGun, 0.05f * railGun);
            self.Stun(stunDur);
            //self.SetMalnourished(true);
        }
        else
        {
            RG_Shocker.StunWave(self, 100 * e.RailgunUse, 0.15f * e.RailgunUse, 16 * e.RailgunUse);
            InnerSplosion(self, 1500, true);
            e.RailgunUse = e.RailgunCD = 0;
            self.Die();
        }
    }
}
