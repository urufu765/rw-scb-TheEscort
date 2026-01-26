using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Railgunner;

public static class RG_Fx
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
            room.AddObject(new SootMark(room, v, 120f, bigSprite: true));
            room.AddObject(new Explosion.ExplosionLight(v, 210f, 0.7f, 7, c));
            room.AddObject(new ShockWave(v, 500f, 0.05f, 6));
            for (int i = 0; i < 20; i++)
            {
                Vector2 v2 = Custom.RNV();
                room.AddObject(new Spark(v + v2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), v2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(Color.white, c, UnityEngine.Random.value), null, 11, 33));
                room.AddObject(new Explosion.FlashingSmoke(v + v2 * 40f * UnityEngine.Random.value, v2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, Color.white, c, UnityEngine.Random.Range(3, 11)));
            }
            room.ScreenMovement(v, default, 1.5f);


            if (lethal)
            {
                room.AddObject(new Explosion(room, self, v, 10, range / 10, 60f, 10f, 10f, 0.4f, self, 0.7f, 2f, 0f));
                room.AddObject(new Explosion(room, self, v, 8, range, 60f, 0.5f, 600f, 0.4f, self, 0.01f, 200f, 0f));
                room.PlaySound(SoundID.Bomb_Explode, self.mainBodyChunk, false, 0.93f, 0.28f);
                self.Die();
            }
            else
            {
                room.PlaySound(SoundID.Bomb_Explode, self.mainBodyChunk, false, 0.86f, 0.4f);
                room.AddObject(new Explosion(room, self, v, 8, range, 60f, 0.02f, 360f, 0.4f, self, 0.01f, 120f, 0f));
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Explosioning FAILED UH OH");
        }
    }


    public static void Spasm(On.Player.orig_Stun orig, Player self, int st)
    {
        orig(self, st);
        try
        {
            if (Eshelp_IsNull(self.slugcatStats.name))
            {
                return;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Stun!");
            return;
        }
        if (
            !eCon.TryGetValue(self, out Escort e)
            )
        {
            return;
        }
        if (!e.Railgunner) return;

        self?.room?.AddObject(new CreatureSpasmer(self, true, st));
        self.exhausted = true;
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


        // Malnutrition bonus
        if (glassCannonBonus)
        {
            xForce *= 1.5f;
            yForce *= 1.5f;
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
        self.room?.PlaySound(Escort_SFX_Pole_Bounce, self.mainBodyChunk.pos, 0.3f, 0.2f);
        if (ModManager.Watcher)
        {
            self.room?.PlaySound(Watcher.WatcherEnums.WatcherSoundID.Water_Machinery_Hit, self.mainBodyChunk.pos, 0.5f, Mathf.Lerp(0.5f, 0.8f, UnityEngine.Random.value));
        }
        if (self.Submersion > 0.5f)
        {
            self.room?.PlaySound(SoundID.Leviathan_Bite, self.mainBodyChunk.pos, .74f, Mathf.Lerp(.5f, .8f, UnityEngine.Random.value));
        }
        else
        {
            self.room?.PlaySound(SoundID.Gate_Pillows_In_Place, self.mainBodyChunk.pos, 1.06f, Mathf.Lerp(.6f, .7f, UnityEngine.Random.value));
        }
        self.room?.PlaySound(SoundID.Coral_Circuit_Break, self.mainBodyChunk.pos, .55f, Mathf.Lerp(.6f, .75f, UnityEngine.Random.value));
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
            e.RailgunUse = (int)(e.RailgunLimit * 0.7);
            InnerSplosion(self, 600);
            e.Escat_RG_SetGlassMode(true);
            //self.stun += e.RailFrail ? 320 : 160;
            int stunDur = e.RailFrail ? 320 : 160;
            if (self.room?.game?.session is StoryGameSession sgs)
            {
                stunDur *= 10 - sgs.saveState.deathPersistentSaveData.karmaCap;
            }

            RG_Shocker.StunWave(self, 60 * railGun, Mathf.Lerp(0.01f, 0.6f, (float)railGun / e.RailgunLimit), 10 * railGun, 0.05f * railGun);
            self.Stun(stunDur);
            //self.SetMalnourished(true);
        }
        else
        {
            RG_Shocker.StunWave(self, 100 * e.RailgunUse, 0.15f * e.RailgunUse, 16 * e.RailgunUse);
            InnerSplosion(self, 1000, true);
        }
    }

}