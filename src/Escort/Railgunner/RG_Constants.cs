using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using SlugBase.Features;

namespace TheEscort.Railgunner;

public static class RG
{
    public static Color ColorRG { get; private set; }
    public static Color ColorElectric { get; private set; }

    public static void Constantise()
    {
        ColorRG = new Color(0.5f, 0.85f, 0.78f);
        ColorElectric = new Color(0.7f, 1f, 1f);
    }
}