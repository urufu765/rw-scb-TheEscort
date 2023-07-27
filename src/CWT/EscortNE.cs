using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace TheEscort;


public partial class Escort
{
}

public static class NewEscapistShadow
{
    public class Shadowscort
    {
        public Shadowscort()
        {
            
        }
    }

    private static readonly ConditionalWeakTable<Player, Shadowscort> neCWT = new();

    public static Shadowscort NE(this Player p) => neCWT.GetValue(p, _ => new());
}
