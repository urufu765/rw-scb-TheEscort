using System;
using TheEscort;
using UnityEngine.PlayerLoop;
using SlugBase;
using static TheEscort.Eshelp;
using static RWCustom.Custom;
using static TheEscort.EscortTutorial;
using System.Collections.Generic;
using UnityEngine;

namespace TheEscort;

public class EscortRoomScript
{
    public static void Attach()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += Escort_Add_Room_Scripts;
    }

    /// <summary>
    /// Adds scripts to specified rooms and/or at certain conditions, allowing ingame cutscenes to play out
    /// </summary>
    private static void Escort_Add_Room_Scripts(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);
        //Ebug("SCRIPTADDER HERE LOL");
        if (room?.game?.session is null) return;
        if (room.game.session is StoryGameSession storyGameSession && Eshelp_IsMe(storyGameSession.saveState.saveStateNumber, false))
        {
            string name = room.abstractRoom.name;
            if (name is null) return;
            if (storyGameSession.saveState.cycleNumber < 2 && (name is "CC_SHAFT02" or "CC_CLOG" or "SU_B07" or "SI_D01" or "SI_D03" or "DM_LEG02" or "GW_TOWER15" or "LF_A10" or "LF_E03" or "CC_A10" or "HR_AP01") && !storyGameSession.saveState.deathPersistentSaveData.Etut(SuperWallFlip))
            {
                Ebug("Get the fucking tutorial bro");
                room.AddObject(new TellPlayerToDoASickFlip(room));
            }
            if (storyGameSession.saveState.cycleNumber == 0 && name is "CC_SUMP02" && storyGameSession.saveState.denPosition is "CC_SUMP02")
            {
                Ebug("Start Escort cutscene!");
                room.AddObject(new DefaultWatchesAPupFall(room));
            }
            if (name is "SB_L01")  // Void sea room in Depths
            {
                Ebug("Ending 1 zone!");
                room.AddObject(new EscortEndingA(room));
            }
        }
    }

    /// <summary>
    /// Tells player how to do a super wall flip, for each build in their respectful region too!
    /// </summary>
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
            if (room.game.session is StoryGameSession storyGameSession && !storyGameSession.saveState.deathPersistentSaveData.Etut(SuperWallFlip))
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
                            storyGameSession.saveState.deathPersistentSaveData.Etut(SuperWallFlip, true);
                            //Destroy();
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Intro cutscene (cut content) that was supposed to be Escort swimming up to see the pup fall from the sky, but the jump up two platforms sequence didn't work everytime and it looked bad so...
    /// </summary>
    private class DefaultEscortSwimsOutOfTheG : UpdatableAndDeletable
    {
        int cutsceneTimer;
        Phase phase;
        bool foodMeterInit;
        StartController startController;
        bool initDone, swimaroundDone, surfaceDone, moveupDone, stareatpupDone, endDone;

        public Player player
        {
            get
            {
                AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
                if (this.room.game.Players.Count > 0 && firstAlivePlayer?.realizedCreature is not null)
                {
                    return firstAlivePlayer.realizedCreature as Player;
                }
                return null;
            }
        }

        public class Phase : ExtEnum<DefaultEscortSwimsOutOfTheG.Phase>
        {
            public Phase(string value, bool register = false) : base(value, register)
            {
            }
            public static readonly Phase Init = new("DESOOTGInit", true);
            public static readonly Phase SwimAround = new("DESOOTGSwimAround", true);
            public static readonly Phase Surface = new("DESOOTGSurface", true);
            public static readonly Phase MoveUp = new("DESOOTGMoveUp", true);
            public static readonly Phase StareAtPup = new("DESOOTGStareAtPup", true);
            public static readonly Phase End = new("DESOOTGEnd", true);

        }

        public class StartController : Player.PlayerController
        {
            private readonly DefaultEscortSwimsOutOfTheG owner;
            public StartController(DefaultEscortSwimsOutOfTheG owner)
            {
                this.owner = owner;
            }

            public override Player.InputPackage GetInput()
            {
                return this.owner.GetInput();
            }
        }

        public DefaultEscortSwimsOutOfTheG(Room room)
        {
            this.room = room;
            this.phase = Phase.Init;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.phase == Phase.Init)
            {
                if (!initDone)
                {
                    Ebug("Cutscene init!", 2);
                    initDone = true;
                }
                if (this.player is not null)
                {
                    if (!foodMeterInit && this.room?.game?.cameras[0] is not null)
                    {
                        if (this.room.game.cameras[0].hud is null)
                        {
                            room.game.cameras[0].FireUpSinglePlayerHUD(player);
                        }
                        foodMeterInit = true;
                        this.room.game.cameras[0].hud.foodMeter.NewShowCount(this.player.FoodInStomach);
                        this.room.game.cameras[0].hud.foodMeter.visibleCounter = 0;
                        this.room.game.cameras[0].hud.foodMeter.fade = 0f;
                        this.room.game.cameras[0].hud.foodMeter.lastFade = 0f;
                        this.room.game.cameras[0].followAbstractCreature = this.player.abstractCreature;
                    }
                    this.startController = new StartController(this);
                    this.player.controller = this.startController;
                    this.phase = Phase.SwimAround;
                }
                return;
            }
            else
            {
                if (this.player is null)
                {
                    return;
                }
                cutsceneTimer++;
                if (phase == Phase.SwimAround)
                {
                    if (!swimaroundDone)
                    {
                        Ebug("Cutscene swimaround!", 2);
                        swimaroundDone = true;
                    }
                    if (cutsceneTimer > 560)
                    {
                        if (player.bodyMode == Player.BodyModeIndex.Swimming)
                        {
                            phase = Phase.Surface;
                            cutsceneTimer = 0;
                        }
                        else
                        {
                            phase = Phase.MoveUp;
                            cutsceneTimer = 0;
                        }
                        return;
                    }
                }
                if (phase == Phase.Surface)
                {
                    if (!surfaceDone)
                    {
                        Ebug("Cutscene forcesurface!", 2);
                        surfaceDone = true;
                    }
                    if (cutsceneTimer > 300)
                    {
                        player.mainBodyChunk.pos = new UnityEngine.Vector2(564.785f, 965.6f);
                        player.standing = true;
                        phase = Phase.MoveUp;
                        cutsceneTimer = 0;
                        return;
                    }
                }
                if (phase == Phase.MoveUp)
                {
                    if (!moveupDone)
                    {
                        Ebug("Cutscene Move UP!", 2);
                        moveupDone = true;
                    }
                    if (cutsceneTimer > 320)
                    {
                        phase = Phase.StareAtPup;
                        cutsceneTimer = 0;
                        return;
                    }
                }
                if (phase == Phase.StareAtPup)
                {
                    if (!stareatpupDone)
                    {
                        Ebug("Cutscene stare!", 2);
                        stareatpupDone = true;
                    }
                    // Spawn slugpup to drop down
                    if (cutsceneTimer > 200)
                    {
                        phase = Phase.End;
                        cutsceneTimer = 0;
                        return;
                    }
                }
                if (phase == Phase.End)
                {
                    if (!endDone)
                    {
                        Ebug("Cutscene end!", 2);
                        endDone = true;
                    }
                    if (this.player is not null)
                    {
                        player.controller = null;
                    }
                    Destroy();
                }
            }
        }

        public Player.InputPackage GetInput()
        {
            if (this.player is null)
            {
                return new Player.InputPackage(false, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
            }
            int x, y;
            x = y = 0;
            bool jmp, pckp, thrw;
            jmp = pckp = thrw = false;
            if (phase == Phase.SwimAround)
            {
                if (cutsceneTimer < 480)
                {
                    x = cutsceneTimer switch
                    {
                        >=129 and <=173 => 1,
                        >=193 and <=194 => 1,
                        >=196 and <=215 => -1,
                        >=232 and <=239 => 1,
                        >=243 and <=251 => -1,
                        >=253 and <=263 => 1,
                        >=264 and <=278 => -1,
                        >=288 and <=309 => 1,
                        >=342 and <=365 => 1,
                        >=394 and <=396 => -1,
                        >=461 and <=467 => -1,
                        _ => 0
                    };
                    y = cutsceneTimer switch
                    {
                        >=112 and <=133 => -1,
                        >=174 and <=324 => 1,
                        >=412 and <=456 => 1,
                        _ => 0
                    };
                }
                else
                {
                    x = player.mainBodyChunk.pos.x switch
                    {
                        >596.5f => -1,
                        <592 => 1,
                        _ => 0
                    };
                    y = 1;
                }
                jmp = cutsceneTimer switch
                {
                    >=525 and <=547 => true,
                    >=560 and <=571 => true,
                    _ => false
                };
            }
            if (phase == Phase.Surface)
            {
                if (player.animation == Player.AnimationIndex.DeepSwim)
                {
                    x = player.mainBodyChunk.pos.x switch
                    {
                        >503 => -1,
                        <505 => 1,
                        _ => 0
                    };
                    y = 1;
                }
                else
                {
                    x = player.mainBodyChunk.pos.x switch
                    {
                        >596.5f => -1,
                        <592 => 1,
                        _ => 0
                    };
                    y = 1;
                    jmp = cutsceneTimer switch
                    {
                        >=270 and <=280 => true,
                        >=290 and <=300 => true,
                        _ => false
                    };
                }
            }
            if (phase == Phase.MoveUp)
            {
                x = cutsceneTimer switch
                {
                    >=9 and <=27 => -1,
                    >=140 and <=164 => 1,
                    >=194 and <=201 => -1,
                    >=301 and <=308 => 1,
                    _ => 0
                };
                y = cutsceneTimer switch
                {
                    >=89 and <=113 => 1,
                    >=177 and <=181 => 1,
                    >=198 and <=203 => 1,
                    >=216 and <=237 => 1,
                    >=244 and <=276 => 1,
                    >=294 and <=301 => 1,
                    _ => 0
                };
                pckp = cutsceneTimer switch
                {
                    >=40 and <=46 => true,
                    >=56 and <=62 => true,
                    >=183 and <=189 => true,
                    _ => false
                };
                jmp = cutsceneTimer switch
                {
                    >=88 and <=102 => true,
                    >=222 and <=234 => true,
                    >=293 and <=303 => true,
                    _ => false
                };
                thrw = cutsceneTimer >= 103 && cutsceneTimer <= 107;
            }


            return new Player.InputPackage(false, Options.ControlSetup.Preset.None, x, y, jmp, thrw, pckp, false, false);
        }
    }

    /// <summary>
    /// Intro cutscene used in 0.3.0, where upon reaching the top of the two platforms, the player controls are locked and the slugpup drops from the sky. A new version where a lizard biting a slugpup will be made
    /// </summary>
    private class DefaultWatchesAPupFall : UpdatableAndDeletable
    {
        StartController startController;
        int cutsceneTimer;
        Phase phase;
        bool initDone, spawnPupDone, endDone, pupInit, foodMeterInit;

        public Player Playr
        {
            get
            {
                AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
                if (this.room.game.Players.Count > 0 && firstAlivePlayer?.realizedCreature is Player p && p.playerState.playerNumber == 0)
                {
                    return p;
                }
                else
                {
                    foreach (AbstractCreature ac in room.game.Players)
                    {
                        if (ac.realizedCreature is Player pl && pl.playerState.playerNumber == 0)
                        {
                            return pl;
                        }
                    }
                }
                return null;
            }
        }


        public class Phase : ExtEnum<Phase>
        {
            public Phase(string value, bool register = false) : base(value, register)
            {
            }
            public static readonly Phase Init = new("DWAPFInit", true);
            public static readonly Phase SpawnPup = new("DWAPFSpawnPup", true);
            public static readonly Phase End = new("DWAPFEnd", true);

        }


        public class StartController : Player.PlayerController
        {
            private readonly DefaultWatchesAPupFall owner;

            public StartController(DefaultWatchesAPupFall owner)
            {
                this.owner = owner;
            }

            public override Player.InputPackage GetInput()
            {
                return new Player.InputPackage(false, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
            }
        }


        public DefaultWatchesAPupFall(Room room)
        {
            this.room = room;
            this.phase = Phase.Init;
        }


        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.phase == Phase.Init)
            {
                if (!initDone)
                {
                    Ebug("Cutscene init!", 2);
                    initDone = true;
                }
                if (this.Playr is not null && Playr.room == room)
                {
                    if (Playr.mainBodyChunk.pos.y > 1068)
                    {
                        cutsceneTimer++;
                    }
                    if (cutsceneTimer > 80)
                    {
                        if (this.Playr.playerState.playerNumber != 0)
                        {
                            Ebug("No player 0 Escort found!", 1);
                            Destroy();
                        }
                        if (!foodMeterInit && this.room?.game?.cameras[0] is not null)
                        {
                            if (this.room.game.cameras[0].hud is null)
                            {
                                room.game.cameras[0].FireUpSinglePlayerHUD(Playr);
                            }
                            foodMeterInit = true;
                            this.room.game.cameras[0].hud.foodMeter.NewShowCount(this.Playr.FoodInStomach);
                            this.room.game.cameras[0].hud.foodMeter.visibleCounter = 0;
                            this.room.game.cameras[0].hud.foodMeter.fade = 0f;
                            this.room.game.cameras[0].hud.foodMeter.lastFade = 0f;
                            this.room.game.cameras[0].followAbstractCreature = this.Playr.abstractCreature;
                        }
                        this.startController = new StartController(this);
                        this.Playr.controller = this.startController;
                        this.phase = Phase.SpawnPup;
                        cutsceneTimer = 0;
                        return;
                    }
                }
            }
            if (this.phase == Phase.SpawnPup)
            {
                cutsceneTimer++;
                if (!spawnPupDone)
                {
                    Ebug("Cutscene spawnpup!", 2);
                    spawnPupDone = true;
                }
                if (this.Playr is not null && !pupInit && Plugin.eCon.TryGetValue(Playr, out Escort e))
                {
                    Plugin.SpawnThePup(ref e, room, room.LocalCoordinateOfNode(0), Playr.abstractCreature.ID);
                    e.SocksAliveAndHappy.SuperHardSetPosition(new UnityEngine.Vector2(560, 1970));
                    if (room.game.session is StoryGameSession sgs)
                    {
                        sgs.saveState.miscWorldSaveData.Esave().EscortPupEncountered = true;
                    }
                    Plugin.pupAvailable = true;
                    pupInit = true;
                }
                if (cutsceneTimer > 100)
                {
                    phase = Phase.End;
                    return;
                }
            }
            if (this.phase == Phase.End)
            {
                if (!endDone)
                {
                    Ebug("Cutscene end!", 2);
                    endDone = true;
                }
                if (this.Playr is not null)
                {
                    Playr.controller = null;
                }
                Destroy();
            }
        }

    }


    /// <summary>
    /// A script for performing a custom ending at the void sea.
    /// </summary>
    private class EscortEndingA : UpdatableAndDeletable
    {
        /// <summary>
        /// Controller of the selected player
        /// </summary>
        StartController startController;

        /// <summary>
        /// Cutscene timer (for each stage/phase)
        /// </summary>
        int cutsceneTimer;

        /// <summary>
        /// Phase of the cutscene
        /// </summary>
        Phase phase;

        /// <summary>
        /// Creatures detected other than Escort at the current scene
        /// </summary>
        List<AbstractCreature> creatures;

        /// <summary>
        /// Stages of the cutscene completion check
        /// </summary>
        bool initDone, movePlayerDone, creatureMoveDone, fadeDone, endDone, missionComplete;

        /// <summary>
        /// Void melting effect effect initial value
        /// </summary>
        float voidMeltInit;

        /// <summary>
        /// Screen fadeout module
        /// </summary>
        MoreSlugcats.FadeOut fadeOut;

        /// <summary>
        /// Finds the damn 1st player and keeps the reference handy
        /// </summary>
        public Player Playr
        {
            get
            {
                AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
                if (this.room.game.Players.Count > 0 && firstAlivePlayer?.realizedCreature is Player)
                {
                    return firstAlivePlayer.realizedCreature as Player;
                }
                return null;
            }
        }


        /// <summary>
        /// For checking which phase the cutscene is at
        /// </summary>
        public class Phase : ExtEnum<Phase>
        {
            public Phase(string value, bool register = false) : base(value, register)
            {
            }
            public static readonly Phase Init = new("EEn1Init", true);
            public static readonly Phase MovePlayer = new("EEn1MovePlayer", true);
            public static readonly Phase CreatureMove = new("EEn1CreatureMove", true);
            public static readonly Phase Fade = new("EEn1Fade", true);
            public static readonly Phase End = new("EEn1End", true);

        }


        /// <summary>
        /// The auto-controlled controller that will take over Player's controls while the cutscene plays
        /// </summary>
        public class StartController : Player.PlayerController
        {
            private readonly EscortEndingA owner;

            public StartController(EscortEndingA owner)
            {
                this.owner = owner;
            }

            public override Player.InputPackage GetInput()
            {
                return new Player.InputPackage(false, Options.ControlSetup.Preset.None, owner.GetXInput(), owner.GetYInput(), false, false, false, false, false);
            }
        }


        /// <summary>
        /// Initialization of cutscene object
        /// </summary>
        public EscortEndingA(Room room)
        {
            this.room = room;
            this.phase = Phase.Init;
            creatures = new List<AbstractCreature>();
            this.voidMeltInit = room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.VoidMelt);
        }


        /// <summary>
        /// Where all the magic happens!
        /// </summary>
        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.phase == Phase.Init)
            {
                if (!initDone)
                {
                    Ebug("Cutscene init!", 2);
                    initDone = true;
                }
                if (this.Playr is not null && Playr.room == room)
                {
                    if (Playr.mainBodyChunk.pos.x > 1000)
                    {
                        if (this.room?.game?.cameras[0] is not null)
                        {
                            this.room.game.cameras[0].followAbstractCreature = this.Playr.abstractCreature;
                        }
                        this.startController = new StartController(this);
                        this.Playr.controller = this.startController;
                        for (int i = 0; i < Playr.grasps.Length; i++)
                        {
                            if (Playr.grasps[i]?.grabbed is not null)
                            {
                                this.Playr.ReleaseGrasp(i);
                            }
                        }
                        this.phase = Phase.MovePlayer;
                        return;
                    }
                }
            }
            if (this.phase == Phase.MovePlayer)
            {
                cutsceneTimer++;
                if (!movePlayerDone)
                {
                    Ebug("Cutscene moving!", 2);
                    movePlayerDone = true;
                }
                if (Playr is not null && Playr.CanRetrieveSlugFromBack)
                {
                    Playr.slugOnBack.increment = true;
                }
                if (cutsceneTimer > 120)
                {
                    foreach(UpdatableAndDeletable uad in room.updateList)
                    {
                        if (uad is Creature c and not Player)
                        {
                            creatures.Add(c.abstractCreature);
                        }
                        else if (uad is Player p && p.Template.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                        {
                            creatures.Add(p.abstractCreature);
                        }
                    }
                    if (Playr is not null)
                    {
                        Playr.standing = true;
                        for (int i = 0; i < Playr.grasps.Length; i++)
                        {
                            if (Playr.grasps[i]?.grabbed is not null)
                            {
                                this.Playr.ReleaseGrasp(i);
                            }
                        }
                    }
                    this.phase = Phase.CreatureMove;
                }
            }
            if (this.phase == Phase.CreatureMove || this.phase == Phase.Fade)
            {
                cutsceneTimer++;
                foreach(AbstractCreature ac in creatures)
                {
                    // if (ac.abstractAI?.RealAI is not null)
                    // {
                    //     ac.abstractAI.RealAI.SetDestination(RWCustom.Custom.MakeWorldCoordinate(new(60, 62), 746));
                    // }
                    //ac.abstractAI?.SetDestination(RWCustom.Custom.MakeWorldCoordinate(new(60, 62), 746));
                    ac.abstractAI?.MigrateTo(RWCustom.Custom.MakeWorldCoordinate(new(60, 62), 746));
                }
                room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.VoidMelt).amount = Mathf.Lerp(voidMeltInit, 1, Mathf.InverseLerp(0, 400, cutsceneTimer));
                if (this.phase == Phase.CreatureMove)
                {
                    if (!creatureMoveDone)
                    {
                        Ebug("Cutscene moving creatures!", 2);
                        creatureMoveDone = true;
                    }
                    if (cutsceneTimer > 200)
                    {
                        phase = Phase.Fade;
                        return;
                    }
                }
                if (this.phase == Phase.Fade)
                {
                    if (!fadeDone)
                    {
                        Ebug("Cutscene fade!", 2);
                        fadeDone = true;
                    }
                    if (fadeOut is null)
                    {
                        fadeOut = new MoreSlugcats.FadeOut(room, Color.black, 200, false);
                        room.AddObject(fadeOut);
                    }
                    if (fadeOut is not null && fadeOut.IsDoneFading())
                    {
                        phase = Phase.End;
                        return;
                    }
                }
            }
            if (this.phase == Phase.End)
            {
                if (!endDone)
                {
                    Ebug("Cutscene end!", 2);
                    endDone = true;
                }
                if (!missionComplete)
                {
                    room.game.GoToRedsGameOver();
                    RainWorldGame.BeatGameMode(room.game, true);
                    // Later, hook into BeatGameMode for a more authentic experience
                    room.game.rainWorld.progression.miscProgressionData.Esave().beaten_Escort = true;
                    missionComplete = true;
                }
            }
        }

        /// <summary>
        /// Moves player to a certain position by returning the right direction press value
        /// </summary>
        public int GetXInput()
        {
            if (phase == Phase.MovePlayer)
            {
                if (Playr is not null && Playr.mainBodyChunk.pos.x < 1210)
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Simulates pressing the up direction on certain conditions
        /// </summary>
        public int GetYInput()
        {
            if (phase == Phase.CreatureMove || phase == Phase.Fade)
            {
                return 1;
            }
            return 0;
        }
    }

}
