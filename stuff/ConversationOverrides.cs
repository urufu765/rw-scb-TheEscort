using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace thelost
{
    static class ConversationOverrides
    {
        public static readonly GameFeature<bool> CustomConversations = GameBool("CustomConversations");
        public static void Hooks()
        {
            On.GhostConversation.AddEvents += GhostOVerride;
            On.SSOracleBehavior.PebblesConversation.AddEvents += pebblesOverride;
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonOverride;
        }

        private static void MoonOverride(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            orig(self);
            if(CustomConversations.TryGet(self.myBehavior.oracle.room.game,out bool value) && value)
            {
                if(self.id == Conversation.ID.MoonFirstPostMarkConversation)
                {
                    self.events = new List<Conversation.DialogueEvent>();
                    switch (Mathf.Clamp(self.State.neuronsLeft, 0, 5)) //this gets the number of neurons left for moon.
                    {
                        case 0:
                            break;
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 0, "...", 0));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 0, "... 2 NEURONS LEFT!", 0));
                            return;
                        case 3:
                            self.events.Add(new Conversation.TextEvent(self, 0, "3 NEURON CONVO!", 0));
                            return;
                        case 4:
                            self.events.Add(new Conversation.TextEvent(self, 0, "4 NEURON CONVO!!!", 0));

                            return;
                        case 5:
                            self.events.Add(new Conversation.TextEvent(self, 0, "Hewwo Wittle Cweatuwe!", 0));

                            return;

                    }
                }
            }
        }

        private static void pebblesOverride(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            orig(self);
            if(CustomConversations.TryGet(self.owner.oracle.room.game,out bool custom) && custom)
            {
                if (self.id == Conversation.ID.Pebbles_White) //Pebbles_White is the default convo as far as i can see.
                {
                    self.events = new List<Conversation.DialogueEvent>();
                    self.events.Add(new Conversation.TextEvent(self, 0, "Hewwo wittle cweatuwe!", 0));


                    //heres some things you might want!

                    //Make five pebbles wait for the player to be still:
                    self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 20));

                    //convos for already existing mark
                    if (self.owner.playerEnteredWithMark)
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "You already have a mark!", 0));
                    }
                    else
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "This conversation requires me giving you the mark!", 0));
                    }

                    //convo if you've gone thru mem arrays:
                    if (self.owner.oracle.room.game.IsStorySession && self.owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked)
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "Pwease don't go in my memowy awways", 0));
                    }

                    //convo for slugpups in room!
                    if(ModManager.MSC && self.owner.CheckSlugpupsInRoom()){
                        self.events.Add(new Conversation.TextEvent(self, 0, "Scuppies in room!", 0));
                    }

                    //convo for creauters in room
                    if (ModManager.MMF && self.owner.CheckStrayCreatureInRoom() != CreatureTemplate.Type.StandardGroundCreature)
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "Creature in room!", 0));
                    }
                    }
            }
        }

        private static void GhostOVerride(On.GhostConversation.orig_AddEvents orig, GhostConversation self)
        {
            orig(self);
            if (CustomConversations.TryGet(self.ghost.room.game,out bool value) && value) //check if this game has custom conversations
            {
                if(self.id == Conversation.ID.Ghost_CC) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "This is a test!", 0)); //add in your own text events!
                    self.events.Add(new Conversation.TextEvent(self, 0, "This should be the second text!", 0));
                }
            }
        }
    }
}
