using System.IO;
using Menu;
using UnityEngine;

namespace TheEscort;

/// <summary>
/// A tree of scenes. Really for organizational purposes
/// </summary>
public static class BuildScenery
{
    /// <summary>
    /// The self.sceneFolder location of the scenes
    /// </summary>
    private static string SceneFolder => Plugin.path + Path.DirectorySeparatorChar + "scenes";
    /// <summary>
    /// For adding a blank image for crossfading purposes
    /// </summary>
    private const string BLANK = "blank";

    /// <summary>
    /// For matching the foreground object with the flats. Differenciates between the flat and the foreground objects easier
    /// </summary>
    private const string BLUEFILTER = "bluefilter";

    /// <summary>
    /// Scenes for Guardian Escort
    /// </summary>
    public static class Guardian
    {
        /// <summary>
        /// Guardian Tribute outro PREFIX
        /// </summary>
        private static string TributeP => "outro guardian tribute ";
        /// <summary>
        /// Guardian Tribute scenes self.sceneFolder location
        /// </summary>
        private static string TributeF => SceneFolder + Path.DirectorySeparatorChar + "outro guardian tribute ";
        public static void Tribute_Depths(MenuScene self, Menu.Menu menu)
        {
            self.sceneFolder = TributeF + "1 - depths";
            if (self.flatMode)
            {
                self.useFlatCrossfades = true;
                self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "depths - flat - f0", new Vector2(683f, 384f), false, true));
                self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "depths - flat - f1", new Vector2(683f, 384f), false, true));
                return;
            }
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 8 bg", new Vector2(-118f, -64f), 6.0f, MenuDepthIllustration.MenuShader.Basic));

            // self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - flat - f1", new Vector2(), 5.9f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLUEFILTER, new Vector2(), 5.8f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 7 pillars", new Vector2(268f, 282f), 4.0f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 6 toarch", new Vector2(0f, 184f), 3.0f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 5 path", new Vector2(-117f, -64f), 2.0f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 4 scorted", new Vector2(853f, 216f), 1.9f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 3 scort 1", new Vector2(384f, 132f), 1.5f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 3 scort 2", new Vector2(429f, 133f), 1.5f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.Standard });

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 2 obstruction", new Vector2(84, -66), 0.5f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 1.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "depths - 1 guardian 2", new Vector2(-115f, -64f), 1.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
        }

        public static void Tribute_Keeper(MenuScene self, Menu.Menu menu)
        {
            self.sceneFolder = TributeF + "2 - keeper";
            if (self.flatMode)
            {
                self.useFlatCrossfades = true;
                self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "keeper - flat - f0", new Vector2(683f, 384f), false, true));
                self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "keeper - flat - f1", new Vector2(683f, 384f), false, true));
                return;
            }
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 7 bg", new Vector2(-118f, -64f), 5.0f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - flat - f1", new Vector2(), 4.9f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLUEFILTER, new Vector2(), 4.8f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 6 piller", new Vector2(1168f, -1f), 3.5f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 5 korma 1", new Vector2(611f, 297f), 4.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 5 korma 2", new Vector2(441f, 137f), 4.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 4 scorted 1", new Vector2(645f, 162f), 2.6f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 4 scorted 2", new Vector2(645f, 162f), 2.6f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 3 scort 1", new Vector2(583f, 73f), 2.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 3 scort 2", new Vector2(583f, 73f), 2.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 2 seaweed", new Vector2(888f, -66f), 0.5f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 1 guardian 1", new Vector2(-117f, -66f), 1.5f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "keeper - 1 guardian 2", new Vector2(-117f, -66f), 1.5f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
        }

        public static void Tribute_Gold(MenuScene self, Menu.Menu menu)
        {
            self.sceneFolder = TributeF + "3 - gold";
            if (self.flatMode)
            {
                self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "gold - flat - f0", new Vector2(683f, 384f), false, true));
                return;
            }
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 7 bg", new Vector2(-118f, -64f), 2.0f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - flat - f0", new Vector2(), 2.9f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLUEFILTER, new Vector2(), 2.8f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 6 jalapeno", new Vector2(-117f, -68f), 0.9f, MenuDepthIllustration.MenuShader.Basic));
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 5 rocks", new Vector2(-117f, -68f), 0.6f, MenuDepthIllustration.MenuShader.Basic));
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 4 scavman", new Vector2(581f, 201f), 0.7f, MenuDepthIllustration.MenuShader.Basic));
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 3 woks", new Vector2(-114f, 456f), 0.4f, MenuDepthIllustration.MenuShader.Basic));
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 2 plunge", new Vector2(459f, 214f), 0.8f, MenuDepthIllustration.MenuShader.Basic));
            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "gold - 1 scort", new Vector2(929f, 0f), 0.5f, MenuDepthIllustration.MenuShader.Basic));
        }

        public static void Tribute_Purpose(MenuScene self, Menu.Menu menu)
        {
            self.sceneFolder = TributeF + "4 - purpose";
            if (self.flatMode)
            {
                self.useFlatCrossfades = true;
                for (int i = 0; i < 5; i++)
                {
                    self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "purpose - flat - f" + i, new Vector2(-118f, -64f), false, true));
                }
                return;
            }

            // self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - flat - f2", Vector2.zero, 6.9f, MenuDepthIllustration.MenuShader.Basic));
            // self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLUEFILTER, Vector2.zero, 6.8f, MenuDepthIllustration.MenuShader.Basic));

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 1", new Vector2(338f, 22f), 6.0f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman " + i,
                    i switch
                    {
                        5 => new Vector2(224f, -66f),
                        4 => new Vector2(251f, -62f),
                        3 => new Vector2(286f, -29f),
                        _ => new Vector2(338f, 22f)
                    }, 6.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 9 look 1", new Vector2(531f, -66f), 5.0f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 9 look " + i, new Vector2(531f, -66f), 5.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 3.5f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 3.5f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            for (int i = 3; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 8 yosh " + i,
                    i switch
                    {
                        5 => new Vector2(-117f, 52f),
                        4 => new Vector2(-34f, 52f),
                        _ => new Vector2(-24f, 52f)
                    }, 3.5f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 3.2f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 3.2f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            for (int i = 3; i <= 5; i++)
            {
                self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 7 yarrow " + i,
                    i switch
                    {
                        5 => new Vector2(51f, 87f),
                        _ => new Vector2(66f, 100f)
                    }, 3.2f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.3f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.3f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            for (int i = 3; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 6 post " + i,
                    i switch
                    {
                        _ => new Vector2(890f, -60f)
                    }, 2.3f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            for (int i = 3; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 5 pink " + i,
                    i switch
                    {
                        5 => new Vector2(943f, -16f),
                        _ => new Vector2(959f, -1f)
                    }, 2.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 3.2f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 4 sost " + i,
                    i switch
                    {
                        5 => new Vector2(148f, -32f),
                        4 => new Vector2(232f, -27f),
                        _ => new Vector2(245f, -27f)
                    }, 3.2f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.9f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 3 socks " + i,
                    i switch
                    {
                        5 => new Vector2(274f, -2f),
                        4 => new Vector2(291f, 6f),
                        _ => new Vector2(288f, 9f)
                    }, 2.9f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK, Vector2.zero, 2.0f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 2 blost " + i,
                    i switch
                    {
                        5 => new Vector2(1056f, 106f),
                        _ => new Vector2(1142f, 118f)
                    }, 2.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, SceneFolder, BLANK,Vector2.zero, 1.7f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 1 blue " + i,
                    i switch
                    {
                        5 => new Vector2(1187f, 147f),
                        _ => new Vector2(1206f, 155f)
                    }, 1.7f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }
        }
        public static void Tribute_PurposeX(MenuScene self, Menu.Menu menu)
        {
            self.sceneFolder = TributeF + "4 - purpose";
            if (self.flatMode)
            {
                self.useFlatCrossfades = true;
                for (int i = 0; i < 5; i++)
                {
                    self.AddIllustration(new MenuIllustration(menu, self, self.sceneFolder, TributeP + "purpose - flat - f" + i, new Vector2(683f, 384f), false, true));
                }
                return;
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 1 blue 2", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 1 blue 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 1 blue 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 1 blue 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 2 blost 2", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 2 blost 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 2 blost 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 2 blost 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 3 socks 2", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 3 socks 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 3 socks 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 3 socks 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 4 sost 2", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 4 sost 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 4 sost 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 4 sost 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 5 pink 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 5 pink 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 5 pink 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 6 post 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 6 post 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 6 post 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 7 yarrow 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 7 yarrow 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 7 yarrow 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, BLANK, "blank", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 8 yosh 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 8 yosh 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 8 yosh 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 9 look 1", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            for (int i = 2; i <= 5; i++)
            {
                self.AddCrossfade(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 9 look " + i, new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            }

            self.AddIllustration(new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 1", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic));
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 2", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 3", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 4", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
            self.AddCrossfade(   new MenuDepthIllustration(menu, self, self.sceneFolder, TributeP + "purpose - 10 carman 5", new Vector2(683f, 384f), 0.0f, MenuDepthIllustration.MenuShader.Basic){ crossfadeMethod = MenuIllustration.CrossfadeType.MaintainBackground });
        }
    }
}
