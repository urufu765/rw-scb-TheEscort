using System;
using TheEscort;
using UnityEngine.PlayerLoop;
using SlugBase;
using static TheEscort.Eshelp;
using static RWCustom.Custom;

namespace TheEscort;

public class EscortRoomScript
{
    public static void Attach()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += Escort_Add_Room_Scripts;
    }

    private static void Escort_Add_Room_Scripts(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);
        //Ebug("SCRIPTADDER HERE LOL");
        if (room?.game?.session is null) return;
        if (room.game.session is StoryGameSession && Eshelp_IsMe(room.game.GetStorySession.saveState.saveStateNumber, false))
        {
            string name = room.abstractRoom.name;
            if (name is null) return;
            if (room.game.GetStorySession.saveState.cycleNumber < 2 && (name is "CC_SHAFT02" or "CC_CLOG" or "SU_B07" or "SI_D01" or "SI_D03" or "DM_LEG02" or "GW_TOWER15" or "LF_A10" or "LF_E03" or "CC_A10" or "HR_AP01") && !room.game.GetStorySession.saveState.deathPersistentSaveData.Esave().SuperWallFlipTutorial)
            {
                Ebug("Get the fucking tutorial bro");
                room.AddObject(new TellPlayerToDoASickFlip(room));
            }
        }
    }

    private class TellPlayerToDoASickFlip : UpdatableAndDeletable
    {
        private int waitForSpawn = 120;
        public TellPlayerToDoASickFlip(Room room)
        {
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (room?.game?.session is null) return;
            if (room.game.session is StoryGameSession && !room.game.GetStorySession.saveState.deathPersistentSaveData.Esave().SuperWallFlipTutorial)
            {
                waitForSpawn--;
                for (int i = 0; i < room.game.Players.Count && (ModManager.CoopAvailable || i == 0); i++)
                {
                    AbstractCreature abstractPlayer = room.game.Players[i];
                    if (abstractPlayer.realizedCreature is Player player && player.room == room)
                    {
                        //Ebug(player, "Player detected!");
                        if (room.abstractRoom.name switch {
                            "CC_SHAFT02" => player.mainBodyChunk.pos.y > 2340 && player.mainBodyChunk.pos.y < 2830,
                            "CC_CLOG" => true,
                            "SU_B07" => player.mainBodyChunk.pos.x > 932 && player.mainBodyChunk.pos.x < 1540,
                            "SI_D01" => player.mainBodyChunk.pos.x > 573 && player.mainBodyChunk.pos.x < 750 && player.mainBodyChunk.pos.y > 733 && player.mainBodyChunk.pos.y < 1062,
                            "SI_D03" => player.mainBodyChunk.pos.x > 3380 && player.mainBodyChunk.pos.x < 3700,
                            "DM_LEG02" => player.mainBodyChunk.pos.x > 113 && player.mainBodyChunk.pos.x < 339 && player.mainBodyChunk.pos.y > 996 && player.mainBodyChunk.pos.y < 1250,
                            "GW_TOWER15" => player.mainBodyChunk.pos.x > 2313 && player.mainBodyChunk.pos.x < 2800 && player.mainBodyChunk.pos.y < 666,
                            "LF_A10" => player.mainBodyChunk.pos.y > 96 && player.mainBodyChunk.pos.y < 167,
                            "LF_E03" => player.mainBodyChunk.pos.x > 3010 && player.mainBodyChunk.pos.x < 4640 && player.mainBodyChunk.pos.y > 120 && player.mainBodyChunk.pos.y < 188,
                            "CC_A10" => player.mainBodyChunk.pos.x > 275 && player.mainBodyChunk.pos.y > 389 && player.mainBodyChunk.pos.x < 311 && player.mainBodyChunk.pos.y < 572 && waitForSpawn <= 0,
                            "HR_AP01" => player.mainBodyChunk.pos.y > 725,
                            _ => false
                        })
                        {
                            Ebug(player, "SHOW TUTORIAL");
                            this.room.game.cameras[0].hud.textPrompt.AddMessage(rainWorld.inGameTranslator.Translate("flippounce_tutorial"), 20, 500, true, true);
                            room.game.GetStorySession.saveState.deathPersistentSaveData.Esave().SuperWallFlipTutorial = true;
                            //Destroy();
                            break;
                        }
                    }
                }
            }
        }
    }
}
