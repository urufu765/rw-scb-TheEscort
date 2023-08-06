using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;
using System.IO;
using TheEscort;  // replace this with whatever namespace you have your Plugin in

namespace UrufuCutsceneTool;
static class CsInLogger
{
    /// <summary>
    /// Only supports one player (make sure to use this for just one player, otherwise it'll attempt to record all active players)
    /// </summary>
    public class CSIL
    {
        protected List<StrippedPlayerInputPackage> AllInputs {get; private set;} = new();
        private int num;
        private StrippedPlayerInputPackage lastAIn;
        private const bool recordEverything = false;

        public CSIL()
        {
            num = 0;
        }

        /// <summary>
        /// Capture player input. Optimal placement is in Player.Update(), though make sure to check if the playernumber of that player is 0 or whatever number you desire, otherwise it'll capture ALL active players
        /// </summary>
        /// <param name="player">Player to capture</param>
        public void Capture(in Player player)
        {
            num++;
            StrippedPlayerInputPackage spip = new(num, player.input[0], player.mainBodyChunk.pos);
            if (recordEverything || !IsSimilar(spip, lastAIn))
            {
                AllInputs.Add(spip);
                lastAIn = spip;
            }
        }

        /// <summary>
        /// Print everything stored in the logger. Optimal placement is wherever you'll reset the counter, be it a keypress or Savedata.SessionEnded (which you can trigger by purposefully dying)
        /// </summary>
        /// <param name="fileName">Filename to be outputted</param>
        /// <param name="csv">Export as CSV? (TXT if false)</param>
        /// <param name="separator">Separator between each inputs</param>
        public void Release(string fileName = "cutscene_input_log", bool csv = false, string separator = "|")
        {
            string filePath = AssetManager.ResolveFilePath(fileName + (csv? ".csv" : ".txt"));
            string printText = $"FRAME{separator}DPAD{separator} JMP{separator}THRW{separator}PCKP{separator} MAP{separator}CTGL{separator}positionX{separator}positionY\r\n";
            foreach(StrippedPlayerInputPackage spip in AllInputs)
            {
                printText += spip.ToString().Replace("<S>", separator) + "\r\n";
            }
            using (StreamWriter streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(printText);
            }
            UnityEngine.Debug.LogWarning("Inputs has been logged to an external file!");
            AllInputs.Clear();
            num = 0;
        }

        private bool IsSimilar(StrippedPlayerInputPackage a, StrippedPlayerInputPackage b)
        {
            if (a is null || b is null)
            {
                return false;
            }

            if (
                a.x == b.x &&
                a.y == b.y &&
                a.jmp == b.jmp &&
                a.thrw == b.thrw &&
                a.pckp == b.pckp &&
                a.mp == b.mp &&
                a.crouchToggle == b.crouchToggle
            )
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// A stripped way of keeping player inputs, meant to only keep the important bits
    /// </summary>
    public record StrippedPlayerInputPackage
    {
        public readonly int n, x, y;
        public readonly bool jmp, thrw, pckp, mp, crouchToggle;
        public readonly float posX, posY;
        private const string separator = "<S>";

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
            result += separator;
            result += x < 0? "←" : " ";
            result += y < 0? "↓" : " ";
            result += y > 0? "↑" : " ";
            result += x > 0? "→" : " ";
            result += separator;
            result += jmp? "■■■■":"    ";  // jmp
            result += separator;
            result += thrw? "■■■■":"    ";  // thrw
            result += separator;
            result += pckp? "■■■■":"    ";  // pckp
            result += separator;
            result += mp? "■■■■":"    ";  // mp
            result += separator;
            result += crouchToggle? "■■■■":"    ";  // ct
            result += separator;
            result += (posX < 0? "" : "+") + posX.ToString("0000.000");  // posX
            result += separator;
            result += (posY < 0? "" : "+") + posY.ToString("0000.000");  // posY
            return result;
        }
    }

    private static readonly ConditionalWeakTable<Plugin, CSIL> EILSTORE = new();
    public static CSIL GetCSIL(this Plugin instance) => EILSTORE.GetValue(instance, _ => new());
}

