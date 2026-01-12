using System;
using System.Collections.Generic;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SlugBase;
using SlugBase.Features;
using static Menu.SlideShow;
using static Menu.MenuScene;
using static TheEscort.Eshelp;
using System.IO;
using UnityEngine;

namespace TheEscort;

public class EscortEndingStuff
{
    /// <summary>
    /// Slideshow enums
    /// </summary>
    public static class EsclideShow
    {
        /// <summary>
        /// Void with crit
        /// </summary>
        public static SlideShowID GuardianTributeEnd;
        /// <summary>
        /// Void without crit
        /// </summary>
        public static SlideShowID GuardianFailureEnd;
        /// <summary>
        /// OE
        /// </summary>
        public static SlideShowID GuardianEscapeEnd;
        /// <summary>
        /// After 100 cycles, experiencing memories, breaking
        /// </summary>
        public static SlideShowID GuardianFracturedEnd;

        /// <summary>
        /// Register da values!
        /// </summary>
        public static void RegisterValues()
        {
            GuardianTributeEnd = new SlideShowID("EscortGuardianTributeEnd", true);
        }

        /// <summary>
        /// Unregister the values...?
        /// </summary>
        public static void UnregisterValues()
        {
            GuardianTributeEnd?.Unregister();
            GuardianTributeEnd = null;
        }
    }

    /// <summary>
    /// Scene enums(primarily used for slideshow purposes)
    /// </summary>
    public static class Escene
    {
        /// <summary>
        /// Guardian Tribute scene 1
        /// </summary>
        public static SceneID Outro_Tribute_Depths;
        /// <summary>
        /// Guardian Tribute scene 2
        /// </summary>
        public static SceneID Outro_Tribute_Keeper;
        /// <summary>
        /// Guardian Tribute scene 3
        /// </summary>
        public static SceneID Outro_Tribute_Gold;
        /// <summary>
        /// Guardian Tribute scene 4
        /// </summary>
        public static SceneID Outro_Tribute_Purpose;

        /// <summary>
        /// Register da values!
        /// </summary>
        public static void RegisterValues()
        {
            Outro_Tribute_Depths = new SceneID("EscortOutro_GuardianTribute1", true);
            Outro_Tribute_Keeper = new SceneID("EscortOutro_GuardianTribute2", true);
            Outro_Tribute_Gold = new SceneID("EscortOutro_GuardianTribute3", true);
            Outro_Tribute_Purpose = new SceneID("EscortOutro_GuardianTribute4", true);
        }

        /// <summary>
        /// Unregister the values...?
        /// </summary>
        public static void UnregisterValues()
        {
            Outro_Tribute_Depths?.Unregister();
            Outro_Tribute_Depths = null;
            Outro_Tribute_Keeper?.Unregister();
            Outro_Tribute_Keeper = null;
            Outro_Tribute_Gold?.Unregister();
            Outro_Tribute_Gold = null;
            Outro_Tribute_Purpose?.Unregister();
            Outro_Tribute_Purpose = null;
        }
    }

    /// <summary>
    /// Sets up the slideshow sequence at the void end of the Escort campaign
    /// </summary>
    /// <param name="orig">Original method call</param>
    /// <param name="self">Instance</param>
    public static void Escort_Ending_Setup(On.RainWorldGame.orig_GoToRedsGameOver orig, RainWorldGame self)
    {
        if (self.StoryCharacter == Plugin.EscortMe && self?.GetStorySession?.saveState?.miscWorldSaveData?.Esave().GuardianEscortVoidEnding == true)
        {
            if (self.manager.upcomingProcess is not null)
            {
                orig(self);
                return;
            }

            self.manager.musicPlayer?.FadeOutAllSongs(20f);
            self.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
            // TODO: Do scoring based on creatures you escorted
            Ebug("Meow meow slideshow PREPARED");
            self.manager.statsAfterCredits = true;
            self.manager.nextSlideshow = EsclideShow.GuardianTributeEnd;
            self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
            Ebug("Let's fking go!!!");
        }
        orig(self);
    }

    /// <summary>
    /// Inserts the call to slideshow right after the slideShow list has been created.
    /// </summary>
    /// <param name="il">Who knows?</param>
    /// <exception cref="Exception">Matching or emitting exceptions</exception>
    public static void Escort_Slideshow_Abracadabrazam(ILContext il)
    {
        ILCursor wawa = new(il);

        try
        {
            // Find the part where the method creates the playList list and set the cursor right after it
            wawa.GotoNext(MoveType.After,
                matchThis => matchThis.MatchNewobj<List<Scene>>(),
                thenMatch => thenMatch.MatchStfld<SlideShow>(nameof(SlideShow.playList))
            );
            Ebug("Matched point of slideshow interest", LogLevel.MESSAGE, ignoreRepetition: true);
        }
        catch (Exception err)
        {
            Ebug(err, "Issue finding the playList init when using slideshow");
            throw new Exception("Failure in matching when trying to set up slideshow!", err);
        }

        try
        {
            wawa.Emit(OpCodes.Ldarg, 0);  // SlideShow Instance
            wawa.Emit(OpCodes.Ldarg, 1);  // ProcessManager
            wawa.Emit(OpCodes.Ldarg, 2);  // SlideShowID
        }
        catch (Exception err)
        {
            Ebug(err, "Failure to inject parameter IL code");
            throw new Exception("Failure in parameter injection when trying to set up slideshow!", err);
        }

        try
        {
            wawa.EmitDelegate(Escort_Slideshow_Bazinga);
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject method into the spot");
            throw new Exception("Failure in method injection when trying to set up slide show!", err);
        }
    }

    /// <summary>
    /// Prepares the Escort slideshow!
    /// </summary>
    /// <param name="self">Instance</param>
    /// <param name="manager">Manager</param>
    /// <param name="slideShowID">Slideshow ID</param>
    public static void Escort_Slideshow_Bazinga(SlideShow self, ProcessManager manager, SlideShowID slideShowID)
    {
        if (slideShowID == EsclideShow.GuardianTributeEnd)  // Void ending
        {
            Ebug("Slideshow let's go!");

            if (manager.musicPlayer is not null)
            {
                self.waitForMusic = "MC_RWTE_AA_Tribute_Outro";
                self.stall = true;
                manager.musicPlayer.MenuRequestsSong(self.waitForMusic, 1.5f, 10f);
                Ebug("Music init");
            }

            try
            {
                self.playList.Add(new Scene(SceneID.Empty, 0f, 0f, 0f));  // Blank

                Scene depths = new(Escene.Outro_Tribute_Depths, self.ConvertTime(0, 0, 30), self.ConvertTime(0, 1, 80), self.ConvertTime(0, 6, 82));
                depths.AddCrossFade(self.ConvertTime(0, 4, 22), 58);  // The "duration" is in hundredths of a second (or how RW likes to call it: PPS, Platypuses Per Sandals). You're welcome.
                self.playList.Add(depths);

                Scene keeper = new(Escene.Outro_Tribute_Keeper, self.ConvertTime(0, 8, 31), self.ConvertTime(0, 9, 97), self.ConvertTime(0, 14, 79));
                keeper.AddCrossFade(self.ConvertTime(0, 12, 26), 72);
                self.playList.Add(keeper);

                self.playList.Add(new Scene(Escene.Outro_Tribute_Gold, self.ConvertTime(0, 16, 33), self.ConvertTime(0, 19, 77), self.ConvertTime(0, 22, 84)));

                Scene purpose = new(Escene.Outro_Tribute_Purpose, self.ConvertTime(0, 24, 34), self.ConvertTime(0, 27, 99), self.ConvertTime(0, 35, 30));
                purpose.AddCrossFade(self.ConvertTime(0, 27, 99), 249);  // A -> B
                purpose.AddCrossFade(self.ConvertTime(0, 30, 49), 219);  // B -> C
                purpose.AddCrossFade(self.ConvertTime(0, 32, 68), 164);  // C -> D
                purpose.AddCrossFade(self.ConvertTime(0, 34, 33), 96);   // D -> E
                self.playList.Add(purpose);

                self.playList.Add(new Scene(SceneID.Empty, self.ConvertTime(0, 35, 30), self.ConvertTime(0, 35, 30), self.ConvertTime(0, 37, 65)));  // Blank... and also buffer to time the second part of the song with the credit scroll
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to add scenes to playlist!!");
                return;
            }

            self.processAfterSlideShow = ProcessManager.ProcessID.Credits;
            manager.statsAfterCredits = true;  // Just in case someone flipped this to false... I mean why?
            Ebug("Slideshow prepared!");
        }
    }

    /// <summary>
    /// Creates the scenes, but not with SlugBase since SlugBase doesn't support scene transitions
    /// </summary>
    /// <param name="il">il for i don't know</param>
    /// <exception cref="Exception">Matching or emitting failure</exception>
    public static void Escort_SlideScene_Init(ILContext il)
    {
        ILCursor damn = new(il);

        try
        {
            // Find the BuildScene() call and place the cursor right after it. If BuildScene() fails, then this should fail too! Totally 100% safe.
            damn.GotoNext(MoveType.After,
                matchThis => matchThis.MatchCall<MenuScene>(nameof(MenuScene.BuildScene))
            );
            Ebug("Matched point of scene interest", LogLevel.MESSAGE, ignoreRepetition: true);
        }
        catch (Exception err)
        {
            Ebug(err, "Issue finding the buildscene when making new scenes");
            throw new Exception("Failure in matching when trying to set up scenes!", err);
        }

        try
        {
            damn.Emit(OpCodes.Ldarg, 0);  // MenuScene instance
            damn.Emit(OpCodes.Ldarg, 1);  // Menu.Menu
            damn.EmitDelegate(Escort_SceneBuilder);  // Call to Escort_SceneBuilder
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject method into the builder");
            throw new Exception("Failure in method injection when trying to build scenes!", err);
        }
    }

    /// <summary>
    /// Points to the right method to build the desired scene.
    /// </summary>
    /// <param name="self">MenuScene instance</param>
    /// <param name="menu">Menu menu menu menu</param>
    public static void Escort_SceneBuilder(MenuScene self, Menu.Menu menu)
    {
        switch (self.sceneID)
        {
            case var scene when scene == Escene.Outro_Tribute_Depths:
                BuildScenery.Guardian.Tribute_Depths(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Keeper:
                BuildScenery.Guardian.Tribute_Keeper(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Gold:
                BuildScenery.Guardian.Tribute_Gold(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Purpose:
                BuildScenery.Guardian.Tribute_Purpose(self, menu); return;
        }
    }

}