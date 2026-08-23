using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;

namespace TheEscort.Speedster;

public static class SS_Gfx
{
    public static void DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
    {
        try
        {
            if (e.SpeTrailTick == 0 && e.SpeDashNCrash && self?.player?.room is not null && self.player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut)
            {
                e.Escat_addTrail(rCam, s, (int)Mathf.Lerp(20, 40, self.player.Adrenaline), (int)Mathf.Lerp(10, 20, self.player.Adrenaline));
                e.SpeTrailTick = 2;
            }
        }
        catch (Exception err)
        {
            Ebug(self.player, err, "Speedster Draw Sprite failed!");
        }
    }
}