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
    /// The folder location of the scenes
    /// </summary>
    private static string SceneFolder => Plugin.path + Path.DirectorySeparatorChar + "scenes";

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
        /// Guardian Tribute scenes folder location
        /// </summary>
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
