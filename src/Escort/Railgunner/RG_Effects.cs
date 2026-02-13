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
    /// SFX, VFX for Railgunner exploding from overcharge
    /// </summary>
    /// <param name="room"></param>
    /// <param name="pos"></param>
    /// <param name="color"></param>
    public static void FX_OverchargeExplosion(this Room room, Vector2 pos, Color color)
    {
        room.AddObject(new SootMark(room, pos, 120f, bigSprite: true));
        room.AddObject(new Explosion.ExplosionLight(pos, 210f, 0.7f, 7, color));
        room.AddObject(new ShockWave(pos, 500f, 0.05f, 6));
        for (int i = 0; i < 20; i++)
        {
            Vector2 v2 = Custom.RNV();
            room.AddObject(new Spark(pos + v2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), v2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(Color.white, color, UnityEngine.Random.value), null, 11, 33));
            room.AddObject(new Explosion.FlashingSmoke(pos + v2 * 40f * UnityEngine.Random.value, v2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, Color.white, color, UnityEngine.Random.Range(3, 11)));
        }
        room.ScreenMovement(pos, default, 1.5f);
        room.PlaySound(SoundID.Bomb_Explode, pos, 0.87f, 0.32f);
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
    /// SFX for the echo caused by the recoil
    /// </summary>
    /// <param name="room"></param>
    /// <param name="pos"></param>
    /// <param name="submerged"></param>
    public static void SFX_RecoilBoom(this Room room, Vector2 pos, bool submerged)
    {
        room.PlaySound(Escort_SFX_Pole_Bounce, pos, 0.3f, 0.2f);
        if (submerged)
        {
            if (ModManager.Watcher)
            {
                room.PlaySound(Watcher.WatcherEnums.WatcherSoundID.Water_Machinery_Hit, pos, 0.5f, Mathf.Lerp(0.5f, 0.8f, UnityEngine.Random.value));
            }
            room.PlaySound(SoundID.Leviathan_Bite, pos, .74f, Mathf.Lerp(.5f, .8f, UnityEngine.Random.value));
        }
        else
        {
            room.PlaySound(SoundID.Gate_Pillows_In_Place, pos, 1.06f, Mathf.Lerp(.6f, .7f, UnityEngine.Random.value));
        }
        room.PlaySound(SoundID.Coral_Circuit_Break, pos, .55f, Mathf.Lerp(.6f, .75f, UnityEngine.Random.value));
    }
}