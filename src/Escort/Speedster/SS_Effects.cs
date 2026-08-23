using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster;

public static class SS_Fx
{
    public static void Gfx_Sparkle_Boom(Room room, Player self, Color color, float y_offset = -5f, int amount = 10)
    {
        if (room is not null)
        {
            for (int i = 0; i < amount; i++)
            {
                room.AddObject(new Spark(
                    self.bodyChunks[1].pos + new Vector2(-10 * Mathf.Sign(self.bodyChunks[1].vel.x), y_offset),
                    new(-2f * self.bodyChunks[0].vel.x, Mathf.Lerp(0f, 10f, UnityEngine.Random.value)),
                    color, null, 20, 40
                ));
            }
        }
    }

    public static void Sfx_Sparkle_Click(Room room, BodyChunk soundChunk, int gear = 1)
    {
        if (room is not null)
        {
            room.PlaySound(SoundID.Weapon_Skid, soundChunk, false, .74f, .5f + .15f * gear);
            if (ModManager.MSC)
            {
                room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, soundChunk, false, .32f, 6f + .5f * gear);
            }
        }
    }

    public static void Sfx_Click_Bang(Room room, BodyChunk soundChunk, int gear = 1)
    {
        room?.PlaySound(SoundID.Firecracker_Bang, soundChunk, false, .5f, 1.5f + .2f * gear);
    }

    public static void Sfx_Whamo(Room room, BodyChunk soundChunk)
    {
        if (room is not null)
        {
            room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, soundChunk, false, 2.3f, 1.2f);
            room.PlaySound(Escort_SFX_Impact, soundChunk);
        }
    }
}