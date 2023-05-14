using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort
{
    /// <summary>
    /// Redoes JollyCoop ;)
    /// </summary>
    class ReJollyCoop
    {
        public static void Hooker()
        {
            On.PlayerGraphics.PopulateJollyColorArray += PopulateTheJollyMan;
        }




        public static void PopulateTheJollyMan(On.PlayerGraphics.orig_PopulateJollyColorArray orig, SlugcatStats.Name reference)
        {
            try
            {
                PlayerGraphics.jollyColors = new Color?[4][];
                JollyCoop.JollyCustom.Log("Initializing colors... reference " + reference);
                int size = 3;
                if (PlayerGraphics.ColoredBodyPartList(reference).Count > 0)
                {
                    size = PlayerGraphics.ColoredBodyPartList(reference).Count;
                }
                for (int i = 0; i < PlayerGraphics.jollyColors.Length; i++)
                {
                    PlayerGraphics.jollyColors[i] = new Color?[size];
                    if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.CUSTOM)
                    {
                        PlayerGraphics.LoadJollyColorsFromOptions(i);
                    }
                    else
                    {
                        if (!(Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.AUTO))
                        {
                            continue;
                        }
                        JollyCoop.JollyCustom.Log("Need to generate colors for player " + i);
                        if (i == 0)
                        {
                            List<string> list = PlayerGraphics.DefaultBodyPartColorHex(reference);
                            PlayerGraphics.jollyColors[0][0] = Color.white;
                            PlayerGraphics.jollyColors[0][1] = Color.black;
                            for (int a = 2; a < size; a++)
                            {
                                PlayerGraphics.jollyColors[0][a] = Color.green;
                            }

                            for (int b = 0; b < size; b++)
                            {

                            }
                            if (list.Count >= 1)
                            {
                                PlayerGraphics.jollyColors[0][0] = Custom.hexToColor(list[0]);
                            }
                            if (list.Count >= 2)
                            {
                                PlayerGraphics.jollyColors[0][1] = Custom.hexToColor(list[1]);
                            }
                            if (list.Count >= 3)
                            {
                                PlayerGraphics.jollyColors[0][2] = Custom.hexToColor(list[2]);
                            }
                        }
                        else
                        {
                            Color color = JollyCoop.JollyCustom.GenerateComplementaryColor(PlayerGraphics.JollyColor(i - 1, 0));
                            PlayerGraphics.jollyColors[i][0] = color;
                            HSLColor hSLColor = JollyCoop.JollyCustom.RGB2HSL(JollyCoop.JollyCustom.GenerateClippedInverseColor(color));
                            float num = hSLColor.lightness + 0.45f;
                            hSLColor.lightness *= num;
                            hSLColor.saturation *= num;
                            PlayerGraphics.jollyColors[i][1] = hSLColor.rgb;
                            HSLColor hSLColor2 = JollyCoop.JollyCustom.RGB2HSL(JollyCoop.JollyCustom.GenerateComplementaryColor(hSLColor.rgb));
                            hSLColor2.saturation = Mathf.Lerp(hSLColor2.saturation, 1f, 0.8f);
                            hSLColor2.lightness = Mathf.Lerp(hSLColor2.lightness, 1f, 0.8f);
                            PlayerGraphics.jollyColors[i][2] = hSLColor2.rgb;
                            JollyCoop.JollyCustom.Log("Generating auto color for player " + i);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Error when replacing jolly coop color array lol");
                orig(reference);
            }
        }
    }
}