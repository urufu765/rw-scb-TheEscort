using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;
using System.IO;
using TheEscort;

namespace EscortCutsceneTool;
static class EsInLogger
{
    public class EIL
    {
        protected List<StrippedPlayerInputPackage> AllInputs {get; private set;} = new();
        private int num;

        public EIL()
        {
            num = 0;
        }

        public void Capture(in Player player)
        {
            num++;
            AllInputs.Add(new(num, in player.input[0], in player.mainBodyChunk.pos));
        }

        public void Release()
        {
            string filePath = Custom.RootFolderDirectory() + Path.DirectorySeparatorChar.ToString() + "escort_cuts_input_log.txt";
            string printText = "NUMBR|   X|   Y| JMP|THRW|PCKP| MAP|CTGL|positionX|positionY\r\n";
            foreach(StrippedPlayerInputPackage spip in AllInputs)
            {
                printText += spip.ToString() + "\r\n";
            }
            using (StreamWriter streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(printText);
            }
            UnityEngine.Debug.LogWarning("Inputs has been logged to an external file!");
            AllInputs.Clear();
            num = 0;
        }
    }

    public record StrippedPlayerInputPackage
    {
        public readonly int n, x, y;
        public readonly bool jmp, thrw, pckp, mp, crouchToggle;
        public readonly float posX, posY;

        public StrippedPlayerInputPackage(int n, in Player.InputPackage pIn, in Vector2 pos)
        {
            this.n = n;
            this.x = pIn.x;
            this.y = pIn.y;
            this.jmp = pIn.jmp;
            this.thrw = pIn.thrw;
            this.pckp = pIn.pckp;
            this.mp = pIn.mp;
            this.crouchToggle = pIn.crouchToggle;
            this.posX = pos.x;
            this.posY = pos.y;
        }

        public override string ToString()
        {
            string result = n.ToString("00000");
            result += "|";
            result += x < 0? "" : "+";  // X
            result += x.ToString("0.00");
            result += "|";
            result += y < 0? "" : "+";  // Y
            result += y.ToString("0.00");
            result += "|";
            result += jmp? "XXXX":"    ";  // jmp
            result += "|";
            result += thrw? "XXXX":"    ";  // thrw
            result += "|";
            result += pckp? "XXXX":"    ";  // pckp
            result += "|";
            result += mp? "XXXX":"    ";  // mp
            result += "|";
            result += crouchToggle? "XXXX":"    ";  // ct
            result += "|";
            result += posX.ToString("0000.0000");  // posX
            result += "|";
            result += posY.ToString("0000.0000");  // posY
            return result;
        }
    }

    private static readonly ConditionalWeakTable<Plugin, EIL> EILSTORE = new();
    public static EIL GetEIL(this Plugin instance) => EILSTORE.GetValue(instance, _ => new());
}

