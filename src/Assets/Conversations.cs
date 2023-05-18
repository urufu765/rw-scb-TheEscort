using SlugBase.Features;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using static TheEscort.Plugin;


namespace TheEscort
{
    static class Escort_Conversation
    {
        public static readonly SSOracleBehavior.SubBehavior.SubBehavID MeatEscort = new("MeatEscort", true);

        public static readonly SSOracleBehavior.Action MeatEscort_init = new("MeatEscort_Init", true);
        public static readonly SSOracleBehavior.Action MeatEscort_marked = new("MeatEscort_WithMark", true);
        public static readonly SSOracleBehavior.Action MeatEscort_getout = new("MeatEscort_GetOut", true);
        public static readonly SSOracleBehavior.Action MeatEscort_angery = new("MeatEscort_Angery", true);

        public static readonly Conversation.ID Pebbles_Escort = new("Pebbles_Escort", true);
        public static readonly Conversation.ID Pebbles_Escort_2 = new("Pebbles_Escort_Two", true);
        public static readonly Conversation.ID Pebbles_Escort_Insult = new("Pebbles_Escort_Insult", true);
        public static readonly Conversation.ID Pebbles_Escort_Angery = new("Pebbles_Escort_Angery", true);

        public static readonly SSOracleBehavior.SubBehavior.SubBehavID MeetEscort = new("MeetEscort", true);

        public static readonly SSOracleBehavior.Action MeetEscort_norm = new("MeetEscort_Normal", true);
        public static readonly SSOracleBehavior.Action MeetEscort_unmarked = new("MeetEscort_Unmarked", true);
        public static readonly SSOracleBehavior.Action MeetEscort_marked = new("MeetEscort_Marked", true);
        public static readonly SSOracleBehavior.Action MeetEscort_markedpost = new("MeetEscort_MarkedPost", true);

        public static readonly Conversation.ID Moon_Escort = new("Moon_Escort", true);
        public static readonly Conversation.ID Moon_Escort_Post = new("Moon_Escort_Post", true);
        public static readonly Conversation.ID Moon_Escort_Reentry = new("Moon_Escort_Reentry", true);


        public static readonly GameFeature<bool> DoAConverse = GameBool("theescort/converse");

        public class FiveWeeStonzMeatsEskie : SSOracleBehavior.ConversationBehavior
        {
            public FiveWeeStonzMeatsEskie(SSOracleBehavior owner) : base(owner, MeatEscort, Pebbles_Escort)
            {
                //owner.getToWorking = 0f;
                // owner.SlugcatEnterRoomReaction();
                if (this.oracle.graphicsModule is not null)
                {
                    (this.oracle.graphicsModule as OracleGraphics).halo.ChangeAllRadi();
                    (this.oracle.graphicsModule as OracleGraphics).halo.connectionsFireChance = 1f;
                }
                owner.TurnOffSSMusic(false);
                owner.getToWorking = 1f;
            }

            public override void Update()
            {
                base.Update();
                if (base.player is null)
                {
                    return;
                }
                if (base.action != MeatEscort_getout && base.action != SSOracleBehavior.Action.ThrowOut_KillOnSight)
                {
                    this.owner.LockShortcuts();
                    base.player.enteringShortCut = null;
                }
                else
                {
                    this.owner.UnlockShortcuts();
                }
                if (base.action == SSOracleBehavior.Action.General_GiveMark)
                {
                    return;
                }
                if (base.action == MeatEscort_init)
                {
                    if (base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {
                        base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                        if (this.inActionCounter == 10)
                        {
                            base.dialogBox.Interrupt(base.Translate("... what now?"), 20);
                        }
                        if (this.inActionCounter == 50 && (this.owner?.conversation is null || this.owner.conversation.id != Pebbles_Escort))
                        {
                            this.owner.InitateConversation(Pebbles_Escort, this);
                        }
                        if (this.inActionCounter > 50 && (this.owner?.conversation is null || (this.owner.conversation is not null && this.owner.conversation.id == Pebbles_Escort && this.owner.conversation.slatedForDeletion)))
                        {
                            this.owner.conversation = null;
                            this.owner.NewAction(MeatEscort_marked);
                        }
                    }
                    else
                    {
                        if (this.inActionCounter == 30)
                        {
                            base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_1, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                        if (this.inActionCounter == 140)
                        {
                            base.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_3, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                        if (this.inActionCounter > 320)
                        {
                            base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                            this.owner.NewAction(MeatEscort_angery);
                        }
                    }
                    return;
                }
                if (base.action == MeatEscort_marked)
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
                    if (this.inActionCounter == 120 && (this.owner?.conversation is null || this.owner.conversation.id != Pebbles_Escort_Insult))
                    {
                        this.owner.InitateConversation(Pebbles_Escort_Insult, this);
                    }
                    if (this.inActionCounter > 120 && (this.owner?.conversation is null || (this.owner.conversation is not null && this.owner.conversation.id == Pebbles_Escort_Insult && this.owner.conversation.slatedForDeletion)))
                    {
                        this.owner.conversation = null;
                        this.owner.NewAction(MeatEscort_angery);
                    }
                    return;
                }
                if (base.action == MeatEscort_angery)
                {
                    if (base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {
                        base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;

                        if (this.inActionCounter == 15 && (this.owner?.conversation is null || this.owner.conversation.id != Pebbles_Escort_2))
                        {
                            this.owner.InitateConversation(Pebbles_Escort_2, this);
                        }
                        if (this.inActionCounter > 15 && (this.owner?.conversation is null || (this.owner.conversation is not null && this.owner.conversation.id == Pebbles_Escort_2 && this.owner.conversation.slatedForDeletion)))
                        {
                            this.owner.conversation = null;
                            this.owner.NewAction(MeatEscort_getout);
                            return;
                        }
                    }
                    else
                    {
                        if (this.inActionCounter > 200)
                        {
                            this.owner.NewAction(MeatEscort_getout);
                            return;
                        }
                        if (this.inActionCounter == 140)
                        {
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_5, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                        if (this.inActionCounter == 20)
                        {
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_2, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                    }
                    return;
                }
                if (base.action == MeatEscort_getout)
                {
                    //this.owner.getToWorking = Mathf.InverseLerp(0, 200, this.inActionCounter);
                    if (base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {
                        if (this.inActionCounter == 15 && (this.owner?.conversation is null || this.owner.conversation.id != Pebbles_Escort_Angery))
                        {
                            this.owner.InitateConversation(Pebbles_Escort_Angery, this);
                        }

                        if (this.inActionCounter == 300)
                        {
                            base.dialogBox.Interrupt(base.Translate("OUT!"), 60);
                        }
                    }
                    else
                    {
                        if (this.inActionCounter == 80)
                        {
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_5, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                        if (this.inActionCounter == 500)
                        {
                            this.owner.voice = base.oracle.room?.PlaySound(SoundID.SS_AI_Talk_3, base.oracle.firstChunk);
                            this.owner.voice.requireActiveUpkeep = true;
                        }
                    }
                    if (this.inActionCounter > 550)
                    {
                        this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                    }
                    if (this.inActionCounter > 100)
                    {
                        if (base.player.room == base.oracle.room)
                        {
                            if (!base.oracle.room.aimap.getAItile(base.player.mainBodyChunk.pos).narrowSpace)
                            {
                                base.player.mainBodyChunk.vel += RWCustom.Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 2f * (1f - base.oracle.room.gravity) * Mathf.InverseLerp(20f, 150f, inActionCounter);
                            }
                            return;
                        }
                        base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad = 1;
                        this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                    }
                    return;
                }
                this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                return;
            }
        }

        public class MoonLookerMeetsEskie : SSOracleBehavior.ConversationBehavior
        {
            float lowGravity;
            bool gravOn;
            int timeUntilNextPanic;
            MoreSlugcats.OraclePanicDisplay panic;
            int panicTimer;
            float lastGetToWork;

            public MoonLookerMeetsEskie(SSOracleBehavior owner) : base(owner, MeetEscort, Moon_Escort)
            {
                CueNextPanicSession();
                lowGravity = -1f;
                if (!base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                {
                    this.owner.getToWorking = 0f;
                    gravOn = false;
                    this.owner.SlugcatEnterRoomReaction();
                    this.owner.voice = base.oracle.room.PlaySound(SoundID.SL_AI_Talk_5, base.oracle.firstChunk);
                    this.owner.voice.requireActiveUpkeep = true;
                    this.owner.LockShortcuts();
                    return;
                }
                if (this.owner.conversation is not null)
                {
                    this.owner.conversation.Destroy();
                    this.owner.conversation = null;
                    return;
                }
                owner.TurnOffSSMusic(true);
                owner.getToWorking = 1f;
                this.gravOn = true;

                string theline = UnityEngine.Random.value switch
                {
                    < 0.001f => "Fucking.",
                    < 0.33f => "Hello little beast. It is nice to see you again.",
                    < 0.5f => "Thank you for visiting me. I would not mind some company, but you cannot stay here. I do not want you to be caught in the collapse of my structure.",
                    _ => "It is nice to see you again, but I'm afraid I have nothing for you."
                };
                base.dialogBox.NewMessage(base.Translate(theline), 0);
                return;
            }

            public void CueNextPanicSession()
            {
                this.timeUntilNextPanic = UnityEngine.Random.Range(800, 2400);
                Ebug("Moon panics at the disco: " + timeUntilNextPanic);
            }

            public override bool Gravity => this.gravOn;

            public override float LowGravity => this.lowGravity;

            public override void Update()
            {
                base.Update();
                if (base.player is null)
                {
                    return;
                }
                if (this.panic is null || this.panic.slatedForDeletetion)
                {
                    if (this.panic is not null)
                    {
                        this.owner.getToWorking = this.lastGetToWork;
                    }
                    this.panic = null;
                    this.lastGetToWork = this.owner.getToWorking;
                }
                else
                {
                    this.owner.getToWorking = 1f;
                    if (this.lowGravity < 0f)
                    {
                        this.lowGravity = 0f;
                    }
                    if (this.panic.gravOn)
                    {
                        this.lowGravity = Mathf.Lerp(this.lowGravity, 0.5f, 0.01f);
                    }
                    this.gravOn = this.panic.gravOn;
                    this.owner.SetNewDestination(base.oracle.firstChunk.pos);
                }
                if (base.action == SSOracleBehavior.Action.General_GiveMark)
                {
                    return;
                }
                if (base.action == MeetEscort_norm)
                {
                    if (!base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {
                        this.owner.NewAction(MeetEscort_unmarked);
                    }
                    if (this.panic is null)
                    {
                        this.lowGravity = -1f;
                        this.panicTimer++;
                        if (this.panicTimer > this.timeUntilNextPanic)
                        {
                            this.panicTimer = 0;
                            this.CueNextPanicSession();
                            this.panic = new MoreSlugcats.OraclePanicDisplay(base.oracle);
                            base.oracle.room.AddObject(this.panic);
                        }
                    }
                    return;
                }
                if (base.action == MeetEscort_unmarked)
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    this.gravOn = true;
                    if (this.inActionCounter == 120)
                    {
                        this.owner.voice = base.oracle.room.PlaySound(SoundID.SL_AI_Talk_1, base.oracle.firstChunk);
                    }
                    if (this.inActionCounter == 320)
                    {
                        this.owner.voice = base.oracle.room.PlaySound(SoundID.SL_AI_Talk_2, base.oracle.firstChunk);
                    }
                    if (this.inActionCounter > 480)
                    {
                        this.owner.afterGiveMarkAction = MeetEscort_marked;
                        this.owner.NewAction(SSOracleBehavior.Action.General_GiveMark);
                    }
                    return;
                }
                if (base.action == MeetEscort_marked)
                {
                    this.owner.LockShortcuts();
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    this.gravOn = true;
                    if (this.inActionCounter == 80 && (this.owner?.conversation is null || this.owner.conversation.id != Moon_Escort))
                    {
                        this.owner.InitateConversation(Moon_Escort, this);
                    }
                    if (this.inActionCounter > 80 && (this.owner?.conversation is null || (this.owner.conversation is not null && this.owner.conversation.id == Moon_Escort && this.owner.conversation.slatedForDeletion)))
                    {
                        this.owner.conversation = null;
                        this.owner.NewAction(MeetEscort_markedpost);
                        if (this.panic is null)
                        {
                            this.panic = new MoreSlugcats.OraclePanicDisplay(base.oracle);
                            base.oracle.room.AddObject(this.panic);
                        }
                    }
                    return;
                }
                if (base.action == MeetEscort_markedpost)
                {
                    this.owner.LockShortcuts();
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    this.gravOn = true;
                    if (this.inActionCounter == 400 && (this.owner?.conversation is null || this.owner.conversation.id != Moon_Escort_Post))
                    {
                        this.owner.InitateConversation(Moon_Escort_Post, this);
                    }
                    if (this.inActionCounter > 400 && (this.owner?.conversation is null || (this.owner.conversation is not null && this.owner.conversation.id == Moon_Escort_Post && this.owner.conversation.slatedForDeletion)))
                    {
                        base.movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
                        this.owner.UnlockShortcuts();
                        this.owner.conversation = null;
                        this.owner.getToWorking = 1f;
                        this.owner.NewAction(MeetEscort_norm);
                    }
                }
                return;
            }
        }

        public static void Attach()
        {
            On.SSOracleBehavior.SeePlayer += Escort_Sight_Seeing;
            On.SSOracleBehavior.NewAction += Escort_Next_Sight;
            On.SSOracleBehavior.PebblesConversation.AddEvents += Escort_Meets_Pebbles;
            On.SSOracleBehavior.PebblesConversation.AddEvents += Escort_Meets_Moon;
            On.SSOracleBehavior.Update += Moon_Updates_Escort;
            On.HUD.DialogBox.Interrupt += Escort_Not_Interrupt_Pearl;
            //On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += Escort_Meets_Moon;
        }

        private static void Escort_Not_Interrupt_Pearl(On.HUD.DialogBox.orig_Interrupt orig, HUD.DialogBox self, string text, int extraLinger)
        {
            if (text is "Yes, help yourself. They are not edible.")
            {
                return;
            }
            else
            {
                orig(self, text, extraLinger);
            }
        }

        private static void Eshelp_Converse(Conversation talkingTo, ref List<Conversation.DialogueEvent> conversation, int delay = 0, int sustain = 0, params string[] wa)
        {
            foreach (string wawa in wa)
            {
                conversation.Add(new Conversation.TextEvent(talkingTo, delay, wawa, sustain));
            }
        }
        private static void Eshelp_Converse(Conversation talkingTo, ref List<Conversation.DialogueEvent> conversation, params string[] wa)
        {
            foreach (string wawa in wa)
            {
                conversation.Add(new Conversation.TextEvent(talkingTo, 0, wawa, 0));
            }
        }

        private static void Moon_Updates_Escort(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
        {
            if (self.oracle.room.game.StoryCharacter == EscortMe && self.action == SSOracleBehavior.Action.General_GiveMark)
            {
                self.inActionCounter++;
                self.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                if ((self.inActionCounter > 30 && self.inActionCounter < 300) || self.oracle.ID == MoreSlugcats.MoreSlugcatsEnums.OracleID.DM)
                {
                    if (self.inActionCounter < 300)
                    {
                        if (ModManager.CoopAvailable)
                        {
                            self.StunCoopPlayers(20);
                        }
                        else
                        {
                            self.player.Stun(20);
                        }
                    }
                    Vector2 yeetus = Vector2.ClampMagnitude(self.oracle.room.MiddleOfTile(24, 14) - self.player.mainBodyChunk.pos, 40f) / 40f * 2.8f * Mathf.InverseLerp(30f, 160f, self.inActionCounter);
                    if (ModManager.CoopAvailable)
                    {
                        foreach (Player p in self.PlayersInRoom)
                        {
                            p.mainBodyChunk.vel += yeetus;
                        }
                    }
                    else
                    {
                        self.player.mainBodyChunk.vel += yeetus;
                    }
                }
                if (self.inActionCounter == 30)
                {
                    self.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Telekenisis, 0, 1, 1);
                }
                if (self.inActionCounter == 300)
                {
                    if (ModManager.CoopAvailable)
                    {
                        foreach (Player p in self.PlayersInRoom)
                        {
                            p.aerobicLevel = 1.1f;
                            p.exhausted = true;
                            p.SetMalnourished(true);
                            for (int i = 0; i < 20; i++)
                            {
                                self.oracle.room.AddObject(new Spark(p.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                            }
                        }
                        self.StunCoopPlayers(60);
                    }
                    else
                    {
                        self.player.aerobicLevel = 1.1f;
                        self.player.exhausted = true;
                        self.player.SetMalnourished(true);
                        self.player.Stun(60);
                        for (int i = 0; i < 20; i++)
                        {
                            self.oracle.room.AddObject(new Spark(self.player.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                        }
                    }
                    self.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, 0f, 1f, 0.87f);
                    (self.oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.theMark = true;
                }
                if (self.inActionCounter > 300 && self.player.graphicsModule != null)
                {
                    (self.player.graphicsModule as PlayerGraphics).markAlpha = Mathf.Max((self.player.graphicsModule as PlayerGraphics).markAlpha, Mathf.InverseLerp(500f, 300f, self.inActionCounter));
                }
                if (self.inActionCounter >= 500)
                {
                    self.NewAction(MeetEscort_marked);
                }
            }
            else
            {
                orig(self, eu);
            }
        }


        private static void Escort_Sight_Seeing(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self)
        {
            try
            {
                if (DoAConverse.TryGet(self.oracle.room.game, out bool val) && val && self.oracle.room.game.StoryCharacter == EscortMe)
                {
                    if (self.timeSinceSeenPlayer < 0)
                    {
                        self.timeSinceSeenPlayer = 0;
                    }
                    self.SlugcatEnterRoomReaction();

                    if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0 || (self.oracle.ID == MoreSlugcats.MoreSlugcatsEnums.OracleID.DM))
                    {
                        if (self.oracle.ID == MoreSlugcats.MoreSlugcatsEnums.OracleID.DM)
                        {
                            self.NewAction(MeetEscort_norm);
                            return;
                        }
                        else
                        {
                            self.NewAction(MeatEscort_init);
                            return;
                        }
                    }
                }
                else
                {
                    orig(self);
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Couldn't sight see!");
                orig(self);
            }
        }

        private static void Escort_Next_Sight(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action nextAction)
        {
            if (nextAction == self.action)
            {
                return;
            }
            if (nextAction == MeatEscort_init)
            {
                if (self.currSubBehavior.ID == MeatEscort)
                {
                    return;
                }
                self.inActionCounter = 0;
                self.action = nextAction;
                SSOracleBehavior.SubBehavior subBehavior = null;
                for (int i = 0; i < self.allSubBehaviors.Count; i++)
                {
                    if (self.allSubBehaviors[i].ID == MeatEscort)
                    {
                        subBehavior = self.allSubBehaviors[i];
                        break;
                    }
                }
                subBehavior ??= new FiveWeeStonzMeatsEskie(self);
                self.allSubBehaviors.Add(subBehavior);
                subBehavior.Activate(self.action, nextAction);
                self.currSubBehavior.Deactivate();
                self.currSubBehavior = subBehavior;
            }
            else if (nextAction == MeatEscort_marked || nextAction == MeatEscort_angery || nextAction == MeatEscort_getout)
            {
                self.inActionCounter = 0;
                self.action = nextAction;
            }
            else if (nextAction == MeetEscort_norm)
            {
                self.inActionCounter = 0;
                self.action = nextAction;
                SSOracleBehavior.SubBehavior subBehavior = null;
                for (int i = 0; i < self.allSubBehaviors.Count; i++)
                {
                    if (self.allSubBehaviors[i].ID == MeetEscort)
                    {
                        subBehavior = self.allSubBehaviors[i];
                        break;
                    }
                }
                subBehavior ??= new MoonLookerMeetsEskie(self);
                self.allSubBehaviors.Add(subBehavior);
                subBehavior.Activate(self.action, nextAction);
                self.currSubBehavior.Deactivate();
                self.currSubBehavior = subBehavior;
            }
            else if (nextAction == MeetEscort_marked || nextAction == MeetEscort_unmarked)
            {
                self.inActionCounter = 0;
                self.action = nextAction;
            }
            else
            {
                orig(self, nextAction);
            }
        }

        private static void Escort_Meets_Pebbles(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            orig(self);
            if (DoAConverse.TryGet(self.owner.oracle.room.game, out bool val) && val)
            {
                Ebug("ID: " + self.id);
                Ebug("Oracle: " + self.owner.oracle);
                Ebug("Current Save: " + self.currentSaveFile);
                Ebug("Behaviour: " + self.owner);
                Ebug("Interface Owner: " + self.interfaceOwner);
                if (self.id == Pebbles_Escort)
                {
                    if (self.owner.playerEnteredWithMark)
                    {
                        Eshelp_Converse(
                            self, ref self.events,
                            sustain: 30,
                            wa: "Has Suns decided to insult me with yet another messenger?"
                        );
                    }
                }
                if (self.id == Pebbles_Escort_Insult)
                {
                    //self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 40));
                    if (self.owner.playerEnteredWithMark)
                    {
                        Eshelp_Converse(
                            self, ref self.events,
                            0, 20,
                            "...",
                            "... a rat."
                        );
                    }
                }
                if (self.id == Pebbles_Escort_2)
                {
                    if (self.owner.playerEnteredWithMark)
                    {
                        Eshelp_Converse(
                            self, ref self.events,
                            "Just my luck.",
                            "...",
                            "I don't have time to deal with this right now."
                        );
                    }
                }
                if (self.id == Pebbles_Escort_Angery)
                {
                    if (self.owner.playerEnteredWithMark)
                    {
                        Eshelp_Converse(self, ref self.events,
                        0, 10,
                        "...",
                        "Get out. ",
                        "Get out of my chamber."
                        );
                    }
                }
                // base.dialogBox.NewMessage(base.Translate("First, Suns sends me an insult of a messenge, and now..."), 10);
                // base.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
                // base.dialogBox.NewMessage(base.Translate("..."), 30);
                // base.dialogBox.NewMessage(base.Translate("... a rat."), 30);
                // base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                // base.dialogBox.NewMessage(base.Translate("Just my luck."), 10);
            }
        }

        private static void Escort_Meets_Moon(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            orig(self);
            if (DoAConverse.TryGet(self.owner.oracle.room.game, out bool val) && val)
            {
                if (self.id == Moon_Escort)
                {
                    Eshelp_Converse(
                        self, ref self.events,
                        "I'm so sorry little creature! I never meant to hurt you!",
                        "My structure is on the verge of collapsing. This is all I can offer...",
                        "...",
                        "Isn't it funny? The last creature to enter my chamber had extreme adaptations as well.",
                        "Though yours seem vastly different... What are you?",
                        "I could look up the records of your body's blueprints, but it would be dangerous for you to stay here for the process.",
                        "My n...",
                        "My neighb... b b b"
                    );
                }
                if (self.id == Moon_Escort_Post)
                {
                    Eshelp_Converse(
                        self, ref self.events,
                        delay: 30,
                        wa: "..."
                    );
                    if (self.owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad > 0)
                    {  // Has met bitch boy
                        // Takes place after Spearmaster returned to Moon after meeting pebbles
                        Eshelp_Converse(
                            self, ref self.events,
                            "My neighbor, Five Pebbles, has not stopped his output. If the messenger made it safely to the communications array, the local group would know.",
                            "...",
                            "Were you sent by the local group?",
                            "Who sent you?",
                            "...",
                            "I should not keep you here any longer.",
                            "I cannot offer you hospitality either, poor thing. Run along now, before my structure collapses."
                        );
                    }
                    else
                    {  // Hasn't met bitch boy
                        // Takes place After Spearmaster meets Moon for the first time
                        Eshelp_Converse(
                            self, ref self.events,
                            "My neighbor, Five Pebbles, has not stopped his output. I do not know if the messenger made it to him, or if he has chosen to ignore the message.",
                            "...",
                            "There is not much I can do, and I urge you to move on, before my structure fails."
                        );
                    }
                }
                if (self.id == Moon_Escort_Reentry)
                {
                    Eshelp_Converse(
                        self, ref self.events,
                        "I would not mind some company, however I urge you to move along. I do not want you to be caught in the collapse of my structure."
                    );
                }
            }
        }
    }
}