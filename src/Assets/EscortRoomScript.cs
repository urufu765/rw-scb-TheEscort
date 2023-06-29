using System;
using TheEscort;
using UnityEngine.PlayerLoop;
using static TheEscort.Eshelp;

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
        string name = room.abstractRoom.name;
        Player player = null;
        if (room?.game?.session?.Players is not null)
        {
            for (int i = 0; i < room.game.session.Players.Count; i++)
            {
                if (room.game.session.Players[i].realizedCreature != null && room.game.session.Players[i].realizedCreature is Player p){
                    player = p;
                    break;
                }
            }
        }
        else return;
        if (name is null) return;
        if (player is null) return;
        if (Eshelp_IsMe(player.slugcatStats.name)) return;
        if ((name == "CC_SHAFT02" || name == "CC_CLOG") && room.game.session is StoryGameSession storyGameSession && storyGameSession.saveState.cycleNumber <= 1)
        {
            room.AddObject(new TellPlayerToDoASickFlip(room));
        }
    }

    private class TellPlayerToDoASickFlip : UpdatableAndDeletable
    {
        private bool message;
        public TellPlayerToDoASickFlip(Room room)
        {
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (!message && this.room.PlayersInRoom.Count > 0){
                foreach(Player player in this.room.PlayersInRoom)
                {
                    if ((room.abstractRoom.name == "CC_SHAFT02" && player.mainBodyChunk.pos.y > 120 && player.mainBodyChunk.pos.y < 140) || room.abstractRoom.name == "CC_CLOG")
                    {
                        this.room.game.cameras[0].hud.textPrompt.AddMessage("Hold a diagonal direction for about 1 second then press jump while holding to do a super wall flip!", 20, 500, true, true);
                        message = true;
                        break;
                    }
                }
                if (message) Destroy();
            }
        }
    }
}
