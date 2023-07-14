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
            if (room.game.GetStorySession.saveState.cycleNumber < 2 && (name is "CC_SHAFT02" or "CC_CLOG" or "SU_B07" or "SI_D01" or "SI_D06" or "DM_LEG02" or "GW_TOWER15" or "LF_A10" or "LF_E03" or "CC_A10" or "HR_AP01") && !room.game.GetStorySession.saveState.deathPersistentSaveData.Esave().SuperWallFlipTutorial)
            {
                Ebug("Get the fucking tutorial bro");
                room.AddObject(new TellPlayerToDoASickFlip(room));
            }
        }
    }

    private class TellPlayerToDoASickFlip : UpdatableAndDeletable
    {
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
                for (int i = 0; i < room.game.Players.Count && (ModManager.CoopAvailable || i == 0); i++)
                {
                    AbstractCreature abstractPlayer = room.game.Players[i];
                    if (abstractPlayer.realizedCreature is Player player && player.room == room)
                    {
                        //Ebug(player, "Player detected!");
                        if (room.abstractRoom.name switch {
                            "CC_SHAFT02" => player.mainBodyChunk.pos.y > 2340 && player.mainBodyChunk.pos.y < 2830,
                            "CC_CLOG" => true,
                            "SU_B07" => false,
                            "SI_D06" => false,
                            "DM_LEG02" => false,
                            "GW_TOWER15" => false,
                            "LF_A10" => false,
                            "LF_E03" => false,
                            "CC_A10" => player.mainBodyChunk.pos.x > 195 && player.mainBodyChunk.pos.y > 334 && player.mainBodyChunk.pos.x < 311 && player.mainBodyChunk.pos.y < 572,
                            "HR_AP01" => player.mainBodyChunk.pos.y > 625,
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
