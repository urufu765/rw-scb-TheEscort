using System;
using UnityEngine;
using SlugBase.SaveData;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using System.Linq;
using Menu.Remix.MixedUI;

namespace TheEscort;

public enum LogLevel
{
    ERR = 0,
    WARN = 1,
    MESSAGE = 2,
    INFO = 3,
    DEBUG = 4,
}

public static class Eshelp
{
    private static string prevLog = "";
    private static int logRepetition;
    /*
    LogInfo Priority:
    -1: No logs
        0: Exceptions
        1: Important things
        2: Less important things
        3: Method pings
        4: Ebug errors (done by design)
    */
    public static int logImportance = 4;
    private static readonly string[] prevLogs = new string[RainWorld.PlayerObjectBodyColors.Length]; //THIS IS THE MYRIAD PLAYER COUNT -WW
    private static readonly int[] logRepetitions = new int[RainWorld.PlayerObjectBodyColors.Length];

    public static ManualLogSource LR;
    // public static ManualLogSource LR => Plugin.Log;


    public static Dictionary<string, int> themCreatureScores;

    public static void Ebug(string message, LogLevel logPrio = LogLevel.DEBUG, bool ignoreRepetition = false)
    {
        if ((int)logPrio <= logImportance)
        {
            if (message != prevLog || ignoreRepetition)
            {
                if (logRepetition > 0)
                {
                    LR.AutoLog("-> Escort: Previous message repeated " + logRepetition + " times: " + prevLog, logPrio);
                }
                prevLog = message;
                logRepetition = 0;
                LR.AutoLog("-> Escort: " + message, logPrio);
            }
            else
            {
                logRepetition++;
            }
        }
    }
    public static void Ebug(object message, LogLevel logPrio = LogLevel.DEBUG)
    {
        if ((int)logPrio <= logImportance)
        {
            LR.AutoLog("-> Escort: " + message.ToString(), logPrio);
        }
    }
    public static void Ebug(string[] messages, LogLevel logPrio = LogLevel.DEBUG, bool separated = true)
    {
        if ((int)logPrio <= logImportance)
        {
            if (separated)
            {
                string message = "";
                foreach (string msg in messages)
                {
                    message += ", " + msg;
                }
                LR.AutoLog("-> Escort: " + message.Substring(2), logPrio);
            }
            else
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    if (i == 0)
                    {
                        LR.AutoLog("-> Escort: " + messages[i], logPrio);
                    }
                    else
                    {
                        LR.AutoLog("->         " + messages[i], logPrio);
                    }
                }
            }
        }
    }
    public static void Ebug(object[] messages, LogLevel logPrio = LogLevel.DEBUG, bool separated = true)
    {
        if ((int)logPrio <= logImportance)
        {
            if (separated)
            {
                string message = "";
                foreach (object msg in messages)
                {
                    message += ", " + msg.ToString();
                }
                LR.AutoLog("-> Escort: " + message.Substring(2), logPrio);
            }
            else
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    if (i == 0)
                    {
                        LR.AutoLog("-> Escort: " + messages[i].ToString(), logPrio);
                    }
                    else
                    {
                        LR.AutoLog("->         " + messages[i].ToString(), logPrio);
                    }
                }
            }
        }
    }
    public static void Ebug(Exception exception, string message = "caught error!", LogLevel logPrio = LogLevel.ERR, bool asregular = false, [CallerMemberName] string callerName = "")
    {
        if ((int)logPrio <= logImportance)
        {
            string toSend = $"-> ERcORt[{callerName}]: {message}";
            if (asregular)
            {
                Debug.LogError(toSend + " => " + exception.Message);
                if (exception.StackTrace != null)
                {
                    Debug.LogError("->       : " + exception.StackTrace);
                }
                Debug.LogException(exception);
            }
            else
            {
                LR.LogError(toSend);
                if (exception.StackTrace != null)
                {
                    LR.LogError("->       : " + exception.StackTrace);
                }
                LR.LogError(exception);
            }

            // if (Plugin.escPatch_guardian)
            // {
            //     Eshelp_Throw_Exception_At_Vigaro(new Exception(toSend, exception));
            // }
        }
    }
    public static void Ebug(Player self, string message, LogLevel logPrio = LogLevel.DEBUG, bool ignoreRepetition = false)
    {
        if (self == null)
        {
            Ebug(message, logPrio, ignoreRepetition);
        }
        try
        {
            if ((int)logPrio <= logImportance)
            {
                if (message != prevLogs[self.playerState.playerNumber] || ignoreRepetition)
                {
                    if (logRepetitions[self.playerState.playerNumber] > 0)
                    {
                        LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: Previous message repeated " + logRepetitions[self.playerState.playerNumber] + " times: " + prevLogs[self.playerState.playerNumber], logPrio);
                    }
                    prevLogs[self.playerState.playerNumber] = message;
                    logRepetitions[self.playerState.playerNumber] = 0;
                    LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + message, logPrio);
                }
                else
                {
                    logRepetitions[self.playerState.playerNumber]++;
                }
            }
        }
        catch (Exception err)
        {
            Ebug(message, logPrio);
            Ebug(err, asregular: true);
        }
    }

    public static void Ebug(Player self, object message, LogLevel logPrio = LogLevel.DEBUG)
    {
        if (self == null)
        {
            Ebug(message, logPrio);
        }
        try
        {
            if ((int)logPrio <= logImportance)
            {
                LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + message.ToString(), logPrio);
            }
        }
        catch (Exception err)
        {
            Ebug(message, logPrio);
            Ebug(err, asregular: true);
        }
    }
    public static void Ebug(Player self, string[] messages, LogLevel logPrio = LogLevel.DEBUG, bool separated = true)
    {
        if (self == null)
        {
            Ebug(messages, logPrio, separated);
        }
        try
        {
            if ((int)logPrio <= logImportance)
            {
                if (separated)
                {
                    String message = "";
                    foreach (String msg in messages)
                    {
                        message += ", " + msg;
                    }
                    LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2), logPrio);
                }
                else
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (i == 0)
                        {
                            LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i], logPrio);
                        }
                        else
                        {
                            LR.AutoLog("->        [" + self.playerState.playerNumber + "]: " + messages[i], logPrio);
                        }
                    }
                }
            }
        }
        catch (Exception err)
        {
            Ebug(messages, logPrio, separated);
            Ebug(err, asregular: true);
        }

    }
    public static void Ebug(Player self, object[] messages, LogLevel logPrio = LogLevel.DEBUG, bool separated = true)
    {
        if (self == null)
        {
            Ebug(messages, logPrio, separated);
        }
        try
        {
            if ((int)logPrio <= logImportance)
            {
                if (separated)
                {
                    string message = "";
                    foreach (object msg in messages)
                    {
                        message += ", " + msg.ToString();
                    }
                    LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2), logPrio);
                }
                else
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (i == 0)
                        {
                            LR.AutoLog("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i].ToString(), logPrio);
                        }
                        else
                        {
                            LR.AutoLog("->         [" + self.playerState.playerNumber + "]: " + messages[i].ToString(), logPrio);
                        }
                    }
                }
            }
        }
        catch (Exception err)
        {
            Ebug(messages, logPrio, separated);
            Ebug(err, asregular: true);
        }
    }

    public static void Ebug(Player self, Exception exception, string message = "caught error!", LogLevel logPrio = LogLevel.ERR, bool asregular = false, [CallerMemberName] string callerName = "")
    {
        if (self == null)
        {
            Ebug(exception, message, logPrio, asregular, callerName);
        }
        try
        {
            string toSend = $"-> ERcORt[{callerName}|{self.playerState.playerNumber}]: {message}";
            if ((int)logPrio <= logImportance)
            {
                if (asregular)
                {
                    Debug.LogError(toSend + $" => {exception.Message}");
                    if (exception.StackTrace != null)
                    {
                        Debug.LogError("->       : " + exception.StackTrace);
                    }
                    Debug.LogException(exception);
                }
                else
                {
                    LR.LogError(toSend);
                    if (exception.StackTrace != null)
                    {
                        LR.LogError($"->       [{callerName}|{self.playerState.playerNumber}]: {exception.StackTrace}");
                    }
                    LR.LogError(exception);
                }
            }
            // if (Plugin.escPatch_guardian)
            // {
            //     Eshelp_Throw_Exception_At_Vigaro(new Exception(toSend, exception));
            // }
        }
        catch (Exception err)
        {
            Ebug(exception, message, logPrio, asregular);
            Ebug(err, asregular: true);
        }
    }


    public static void AutoLog(this ManualLogSource source, object message, LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.ERR:
                source.LogError(message);
                break;
            case LogLevel.WARN:
                source.LogWarning(message);
                break;
            case LogLevel.MESSAGE:
                source.LogMessage(message);
                break;
            case LogLevel.INFO:
                source.LogInfo(message);
                break;
            default:
                source.LogDebug(message);
                break;
        }
    }


    // public static void Eshelp_Throw_Exception_At_Vigaro(Exception exception)
    // {
    //     UploadException(exception);
    // }



    public static Color Eshelp_cycle_dat_RGB(ref float t, float cycleDuration = 959f, float saturation = 1f, float lightness = 0.5f, float increment = 1f)
    {
        if (t >= cycleDuration)
        {
            t = 0;
        }
        else
        {
            t += increment;
        }
        return new HSLColor(Mathf.InverseLerp(0f, cycleDuration, t), saturation, lightness).rgb;
    }

    /// <summary>
    /// Checks if the slugcat is an Escort or not. By default, false is Escort, true is a null or not escort
    /// </summary>
    /// <param name="theSlugcat">Slugcat's name</param>
    /// <param name="nullCheck">For nullchecks (inverts result)</param>
    /// <returns>Whether the slugcat is not an Escort</returns>
    public static bool Eshelp_IsNull(SlugcatStats.Name theSlugcat, bool nullCheck = true)
    {
        try
        {
            if (theSlugcat is null)
            {
                return nullCheck;
            }
            // Specific check
            if (
                theSlugcat.value == "EscortMe" ||
                theSlugcat.value == "EscortBriish" ||
                theSlugcat.value == "EscortGamer" ||
                theSlugcat.value == "EscortHax" ||
                theSlugcat.value == "EscortRizzgayer" ||
                theSlugcat.value == "EscortCheese" ||
                theSlugcat.value == "EscortDrip" ||
                theSlugcat.value == "EscortBodyarmor"
                )
            {
                return !nullCheck; // it IS an Escort!
            }

            if (theSlugcat.value == "EscortSocks")
            {
                return nullCheck;
            }

            // In case I get lazy
            if (theSlugcat.value.Length > 6 && theSlugcat.value.Substring(0, 6) == "Escort")
            {
                Ebug("Stop being lazy you ass");
                return !nullCheck;
            }
        }
        catch (NullReferenceException nerr)
        {
            Ebug(nerr, "Null value slipped through the gaps somehow. What the actual fuck.");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when checking whether the following scug is an Escort or not.");
        }
        return nullCheck;
    }

    /// <summary>
    /// Checks if the slugcat is an Escort or not. By default, false is Escort, true is not escort or null.
    /// </summary>
    /// <param name="theTimeline">Timeline</param>
    /// <param name="nullCheck">True for nullcheck, false for Escort check</param>
    /// <returns>Whether the slugcat is not an Escort</returns>
    public static bool Eshelp_IsNull(SlugcatStats.Timeline theTimeline, bool nullCheck = true)
    {
        try
        {
            if (theTimeline is null) return nullCheck;

            if (theTimeline.value == Plugin.EscortMeTime.value) return !nullCheck;
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null timeline value slipped through the gaps somehow...");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when checking whether the following timeline is Escort's or not.");
        }
        return nullCheck;
    }


    public static void Eshelp_Player_Shaker(Player self, float intensity, bool head = true, bool body = false, bool different = false)
    {
        Vector2 vec = Vector3.Slerp(-RWCustom.Custom.RNV().normalized, RWCustom.Custom.RNV(), UnityEngine.Random.value);
        if (head)
        {
            Vector2 vecHead = vec * Mathf.Min(3f, UnityEngine.Random.value * 3f / Mathf.Lerp(self.bodyChunks[0].mass, 1f, 0.5f)) * intensity;
            self.bodyChunks[0].pos += vecHead;
            self.bodyChunks[0].vel += vecHead * 0.5f;
        }
        if (different)
        {
            vec = Vector3.Slerp(-RWCustom.Custom.RNV().normalized, RWCustom.Custom.RNV(), UnityEngine.Random.value);
        }
        if (body)
        {
            Vector2 vecBody = vec * Mathf.Min(3f, UnityEngine.Random.value * 3f / Mathf.Lerp(self.bodyChunks[1].mass, 1f, 0.5f)) * intensity;
            self.bodyChunks[1].pos += vecBody;
            self.bodyChunks[1].vel += vecBody * 0.5f;
        }
    }

    public static bool ParryCondition(Player player, in Escort escort, out EsType type)
    {
        type = EsType.None;
        // Deflector extra parry check
        if (escort.Deflector && (player.animation == Player.AnimationIndex.BellySlide || player.animation == Player.AnimationIndex.Flip || player.animation == Player.AnimationIndex.Roll))
        {
            type = EsType.Deflector;
            return true;
        }

        // New Escapist hidden parry tech check
        if (escort.NewEscapist && escort.NEsAbility > 0 && (player.animation == Player.AnimationIndex.Flip))
        {
            type = EsType.ShadowEscapist;
            return true;
        }

        // Regular parry check
        else if (player.animation == Player.AnimationIndex.BellySlide && escort.parryAirLean > 0)
        {
            type = EsType.Generic;
            return true;
        }

        // Not in parry condition
        else
        {
            return escort.parrySlideLean > 0;
        }
    }

    public static void Etut(this DeathPersistentSaveData data, EscortTutorial tutorial, bool value)
    {
        data.SetTutorialValue(tutorial, value);
    }

    public static bool Etut(this DeathPersistentSaveData data, EscortTutorial tutorial)
    {
        return data.tutorialMessages.Contains(tutorial);
    }

    public static EscortSaveDataMiscWorld Esave(this MiscWorldSaveData data)
    {
        if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out EscortSaveDataMiscWorld save))
        {
            data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
        }
        return save;
    }

    public static EscortSaveDataMiscProgression Esave(this PlayerProgression.MiscProgressionData data)
    {
        if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out EscortSaveDataMiscProgression save))
        {
            data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
        }
        return save;
    }


    public static string Swapper(string text, string with = "")
    {
        text = text.Replace("<LINE>", System.Environment.NewLine);
        text = text.Replace("<REPLACE>", with);
        text = text.Replace("<DEFAULT>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Guardian"));
        text = text.Replace("<BRAWLER>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Brawler"));
        text = text.Replace("<DEFLECTOR>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Deflector"));
        text = text.Replace("<ESCAPIST>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Escapist"));
        text = text.Replace("<EVADER>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Evader"));
        text = text.Replace("<RAILGUNNER>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Railgunner"));
        text = text.Replace("<SPEEDSTER>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Speedster"));
        text = text.Replace("<GILDED>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Gilded"));
        text = text.Replace("<CONQUEROR>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Conqueror"));
        text = text.Replace("<UNSTABLE>", RWCustom.Custom.rainWorld.inGameTranslator.Translate("Unstable"));

        return text;
    }


    /// <summary>
    /// Checks if the thing's position is inside a cone area with Position as the origin(anglegirth acts as angle +- girth)
    /// </summary>
    public static bool ConeDetection(this Creature origin, Vector2 thing, float range, float angle, float angleGirth)
    {
        if (!RWCustom.Custom.DistLess(thing, origin.firstChunk.pos, range))
        {
            return false;
        }

        float a = RWCustom.Custom.VecToDeg(RWCustom.Custom.DirVec(origin.firstChunk.pos, thing));
        if (a > angle - angleGirth && a < angle + angleGirth)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Translates player input to angle
    /// </summary>
    public static float InputToDeg(Player.InputPackage input)
    {
        Vector2 dirInput = new(input.x, input.y);
        if (dirInput == Vector2.zero)
        {
            throw new ZeroValException("Input cannot be zero!");
        }

        if (input.analogueDir.magnitude > 0.2f)
        {
            dirInput = input.analogueDir;
        }
        return RWCustom.Custom.VecToDeg(dirInput);
    }

    /// <summary>
    /// Finds the index of the List of ListItems, based on the predicate. Returns 0 if not found.
    /// </summary>
    /// <param name="items">The list</param>
    /// <param name="match">Predicate</param>
    /// <returns></returns>
    public static int GetIndex(this List<ListItem> items, Predicate<ListItem> match)
    {
        try
        {
            return Math.Max(items.FindIndex(match), 0);
        }
        catch (Exception err)
        {
            Ebug(err, "Oh no! Couldn't find ListItem!");
        }
        return 0;
    }

}

[Serializable]
public class ZeroValException : Exception
{
    public ZeroValException() : base()
    {
    }

    public ZeroValException(string message) : base(message)
    {
    }

    public ZeroValException(string message, Exception inner) : base(message, inner)
    {
    }
}
