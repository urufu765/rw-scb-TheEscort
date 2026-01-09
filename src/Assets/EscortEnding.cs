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
    public static GameFeature<int[]> GuardianTributeDepths;
    public static GameFeature<int[]> GuardianTributeDepthsC;
    public static GameFeature<int[]> GuardianTributeKeeper;
    public static GameFeature<int[]> GuardianTributeKeeperC;
    public static GameFeature<int[]> GuardianTributeVoid;
    public static GameFeature<int[]> GuardianTributePurpose;
    public static GameFeature<int[]> GuardianTributePurposeC1;
    public static GameFeature<int[]> GuardianTributePurposeC2;
    public static GameFeature<int[]> GuardianTributePurposeC3;
    public static GameFeature<int[]> GuardianTributePurposeC4;
    public static GameFeature<int[]> GuardianTributeEmpty;
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
        public static void RegisterValues()
        {
            GuardianTributeEnd = new SlideShowID("EscortGuardianTributeEnd", true);
        }
        public static void UnregisterValues()
        {
            GuardianTributeEnd?.Unregister();
            GuardianTributeEnd = null;
        }
    }

    public static class Escene
    {
        public static SceneID Outro_Tribute_Depths;
        public static SceneID Outro_Tribute_Keeper;
        public static SceneID Outro_Tribute_Gold;
        public static SceneID Outro_Tribute_Purpose;
        public static void RegisterValues()
        {
            Outro_Tribute_Depths = new SceneID("EscortOutro_GuardianTribute1", true);
            Outro_Tribute_Keeper = new SceneID("EscortOutro_GuardianTribute2", true);
            Outro_Tribute_Gold = new SceneID("EscortOutro_GuardianTribute3", true);
            Outro_Tribute_Purpose = new SceneID("EscortOutro_GuardianTribute4", true);
        }
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

    public static void Escort_Slideshow_Abracadabrazam(ILContext il)
    {
        ILCursor wawa = new(il);

        try
        {
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
            wawa.Emit(OpCodes.Ldarg, 0);
            wawa.Emit(OpCodes.Ldarg, 1);
            wawa.Emit(OpCodes.Ldarg, 2);
        }
        catch (Exception err)
        {
            Ebug(err, "Failure to inject parameter IL code");
            throw new Exception("Failure in parameter injection when trying to set up slideshow!", err);
        }

        try
        {
            wawa.EmitDelegate(Escort_Slideshow_Bazinga);
            // wawa.Emit(OpCodes.Call, typeof(EscortEndingStuff).GetMethod(nameof(Escort_Slideshow_Bazinga), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject method into the spot");
            throw new Exception("Failure in method injection when trying to set up slide show!", err);
        }
    }

    public static void Escort_Slideshow_Bazinga(SlideShow self, ProcessManager manager, SlideShowID slideShowID)
    {
        Ebug("Boop");
        if (slideShowID == EsclideShow.GuardianTributeEnd)
        {
            Ebug("Slideshow let's go!");
            if (
                !SlugBaseCharacter.TryGet(Plugin.EscortMe, out SlugBaseCharacter sbc)
                )
            {
                Ebug("Couldn't find Escort!", LogLevel.WARN, true);
                return;
            }

            if (
                !GuardianTributeDepths.TryGet(sbc, out int[] cD) ||
                !GuardianTributeDepthsC.TryGet(sbc, out int[] cDc) ||
                !GuardianTributeKeeper.TryGet(sbc, out int[] cK) ||
                !GuardianTributeKeeperC.TryGet(sbc, out int[] cKc) ||
                !GuardianTributeVoid.TryGet(sbc, out int[] cV) ||
                !GuardianTributePurpose.TryGet(sbc, out int[] cP) ||
                !GuardianTributePurposeC1.TryGet(sbc, out int[] cPc1) ||
                !GuardianTributePurposeC2.TryGet(sbc, out int[] cPc2) ||
                !GuardianTributePurposeC3.TryGet(sbc, out int[] cPc3) ||
                !GuardianTributePurposeC4.TryGet(sbc, out int[] cPc4) ||
                !GuardianTributeEmpty.TryGet(sbc, out int[] c)
            )
            {
                Ebug("Couldn't find scenes!", LogLevel.WARN, true);
                return;
            }

            if (manager.musicPlayer is not null)
            {
                self.waitForMusic = "MC_RWTE_AA_Tribute_Outro";
                self.stall = true;
                manager.musicPlayer.MenuRequestsSong(self.waitForMusic, 1.5f, 10f);
                Ebug("Music init");
            }

            try
            {
                self.playList.Add(new Scene(SceneID.Empty, 0f, 0f, 0f));

                Scene depths = new(Escene.Outro_Tribute_Depths, self.ConvertTime(0, cD[0], cD[1]), self.ConvertTime(0, cD[2], cD[3]), self.ConvertTime(0, cD[4], cD[5]));
                depths.AddCrossFade(self.ConvertTime(0, cDc[1], cDc[2]), cDc[0]);
                self.playList.Add(depths);

                Scene keeper = new(Escene.Outro_Tribute_Keeper, self.ConvertTime(0, cK[0], cK[1]), self.ConvertTime(0, cK[2], cK[3]), self.ConvertTime(0, cK[4], cK[5]));
                keeper.AddCrossFade(self.ConvertTime(0, cKc[1], cKc[2]), cKc[0]);
                self.playList.Add(keeper);

                self.playList.Add(new Scene(Escene.Outro_Tribute_Gold, self.ConvertTime(0, cV[0], cV[1]), self.ConvertTime(0, cV[2], cV[3]), self.ConvertTime(0, cV[4], cV[5])));

                Scene purpose = new(Escene.Outro_Tribute_Purpose, self.ConvertTime(0, cP[0], cP[1]), self.ConvertTime(0, cP[2], cP[3]), self.ConvertTime(0, cP[4], cP[5]));
                purpose.AddCrossFade(self.ConvertTime(0, cPc1[1], cPc1[2]), cPc1[0]);
                purpose.AddCrossFade(self.ConvertTime(0, cPc2[1], cPc2[2]), cPc2[0]);
                purpose.AddCrossFade(self.ConvertTime(0, cPc3[1], cPc3[2]), cPc3[0]);
                purpose.AddCrossFade(self.ConvertTime(0, cPc4[1], cPc4[2]), cPc4[0]);
                self.playList.Add(purpose);

                self.playList.Add(new Scene(SceneID.Empty, self.ConvertTime(0, c[0], c[1]), self.ConvertTime(0, c[2], c[3]), self.ConvertTime(0, c[4], c[5])));
            }
            catch (Exception err)
            {
                Ebug(err, "Failed to add scenes to playlist!!");
                return;
            }

            // try
            // {
            //     for (int i = 1; i < self.playList.Count; i++)
            //     {
            //         self.playList[i].startAt -= 1.1f;
            //         self.playList[i].fadeInDoneAt -= 1.1f;
            //         self.playList[i].fadeOutStartAt -= 1.1f;
            //     }
            // }
            // catch(Exception err)
            // {
            //     Ebug(err, "Failure to adjust timing of playlist scenes!");
            //     return;
            // }

            self.processAfterSlideShow = ProcessManager.ProcessID.Credits;
            manager.statsAfterCredits = true;
            Ebug("Slideshow prepared!");
        }
    }

    internal static void Escort_Meow(On.Menu.SlideShow.orig_ctor orig, SlideShow self, ProcessManager manager, SlideShowID slideShowID)
    {
        Ebug("Meowcene!", LogLevel.WARN, true);
        orig(self, manager, slideShowID);
        Ebug("Meowcene End!", LogLevel.WARN, true);
    }

    public static void Escort_SlideScene_Init(ILContext il)
    {
        ILCursor damn = new(il);

        try
        {
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
            damn.Emit(OpCodes.Ldarg, 0);
            damn.Emit(OpCodes.Ldarg, 1);
            damn.EmitDelegate(Escort_SceneBuilder);
            // wawa.Emit(OpCodes.Call, typeof(EscortEndingStuff).GetMethod(nameof(Escort_Slideshow_Bazinga), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject method into the builder");
            throw new Exception("Failure in method injection when trying to build scenes!", err);
        }
    }

    public static void Escort_SceneBuilder(MenuScene self, Menu.Menu menu)
    {
        switch (self.sceneID)
        {
            case var scene when scene == Escene.Outro_Tribute_Depths:
                BuildOutro.Guardian.Tribute_Depths(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Keeper:
                BuildOutro.Guardian.Tribute_Keeper(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Gold:
                BuildOutro.Guardian.Tribute_Gold(self, menu); return;
            case var scene when scene == Escene.Outro_Tribute_Purpose:
                BuildOutro.Guardian.Tribute_Purpose(self, menu); return;
        }
    }

    public static class BuildOutro
    {
        private const string Prefix = "outro ";
        private static string SceneFolder => Plugin.path + Path.DirectorySeparatorChar + "scenes";
        public static class Guardian
        {
            private static string TributeP => Prefix + "guardian tribute ";
            private static string TributeF => SceneFolder + Path.DirectorySeparatorChar + "outro guardian tribute ";
            public static void Tribute_Depths(MenuScene self, Menu.Menu menu)
            {
                if (self.flatMode)
                {
                    self.useFlatCrossfades = true;
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "1 - depths", TributeP + "depths - flat - f0", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "1 - depths", TributeP + "depths - flat - f1", new Vector2(683f, 384f), false, true));
                    return;
                }
            }

            public static void Tribute_Keeper(MenuScene self, Menu.Menu menu)
            {
                if (self.flatMode)
                {
                    self.useFlatCrossfades = true;
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "2 - keeper", TributeP + "keeper - flat - f0", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "2 - keeper", TributeP + "keeper - flat - f1", new Vector2(683f, 384f), false, true));
                    return;
                }
            }

            public static void Tribute_Gold(MenuScene self, Menu.Menu menu)
            {
                if (self.flatMode)
                {
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "3 - gold", TributeP + "gold - flat - f0", new Vector2(683f, 384f), false, true));
                    return;
                }
            }

            public static void Tribute_Purpose(MenuScene self, Menu.Menu menu)
            {
                if (self.flatMode)
                {
                    self.useFlatCrossfades = true;
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "4 - purpose", TributeP + "purpose - flat - f0", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "4 - purpose", TributeP + "purpose - flat - f1", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "4 - purpose", TributeP + "purpose - flat - f2", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "4 - purpose", TributeP + "purpose - flat - f3", new Vector2(683f, 384f), false, true));
                    self.AddIllustration(new MenuIllustration(menu, self, TributeF + "4 - purpose", TributeP + "purpose - flat - f4", new Vector2(683f, 384f), false, true));
                    return;
                }
            }
        }
    }
}