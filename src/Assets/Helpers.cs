using System;
using UnityEngine;


namespace TheEscort
{
    public static class Eshelp
    {
        private static string prevLog = "";
        private static int logRepetition;

        /*
        Log Priority:
        -1: No logs
         0: Exceptions
         1: Important things
         2: Less important things
         3: Method pings
         4: Ebug errors (done by design)
        */
        private static readonly int logImportance = 4;
        private static readonly string[] prevLogs = new string[4];
        private static readonly int[] logRepetitions = new int[4];

        public static void Ebug(string message, int logPrio = 3, bool ignoreRepetition = false)
        {
            if (logPrio <= logImportance)
            {
                if (message != prevLog || ignoreRepetition)
                {
                    if (logRepetition > 0)
                    {
                        Debug.Log("-> Escort: Previous message repeated " + logRepetition + " times: " + prevLog);
                    }
                    prevLog = message;
                    logRepetition = 0;
                    Debug.Log("-> Escort: " + message);
                }
                else
                {
                    logRepetition++;
                }
            }
        }
        public static void Ebug(object message, int logPrio = 3)
        {
            if (logPrio <= logImportance)
            {
                Debug.Log("-> Escort: " + message.ToString());
            }
        }
        public static void Ebug(string[] messages, int logPrio = 3, bool separated = true)
        {
            if (logPrio <= logImportance)
            {
                if (separated)
                {
                    String message = "";
                    foreach (String msg in messages)
                    {
                        message += ", " + msg;
                    }
                    Debug.Log("-> Escort: " + message.Substring(2));
                }
                else
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (i == 0)
                        {
                            Debug.Log("-> Escort: " + messages[i]);
                        }
                        else
                        {
                            Debug.Log("->         " + messages[i]);
                        }
                    }
                }
            }
        }
        public static void Ebug(object[] messages, int logPrio = 3, bool separated = true)
        {
            if (logPrio <= logImportance)
            {
                if (separated)
                {
                    string message = "";
                    foreach (object msg in messages)
                    {
                        message += ", " + msg.ToString();
                    }
                    Debug.Log("-> Escort: " + message.Substring(2));
                }
                else
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (i == 0)
                        {
                            Debug.Log("-> Escort: " + messages[i].ToString());
                        }
                        else
                        {
                            Debug.Log("->         " + messages[i].ToString());
                        }
                    }
                }
            }
        }
        public static void Ebug(Exception exception, string message = "caught error!", int logPrio = 0, bool asregular = false)
        {
            if (logPrio <= logImportance)
            {
                if (asregular)
                {
                    Debug.LogWarning("-> ERcORt: " + message + " => " + exception.Message);
                    if (exception.StackTrace != null)
                    {
                        Debug.LogWarning("->       : " + exception.StackTrace);
                    }
                }
                else
                {
                    Debug.LogError("-> ERcORt: " + message);
                    if (exception.StackTrace != null)
                    {
                        Debug.LogError("->       : " + exception.StackTrace);
                    }
                    Debug.LogException(exception);
                }
            }
        }
        public static void Ebug(Player self, string message, int logPrio = 3, bool ignoreRepetition = false)
        {
            if (self == null)
            {
                Ebug(message, logPrio, ignoreRepetition);
            }
            try
            {
                if (logPrio <= logImportance)
                {
                    if (message != prevLogs[self.playerState.playerNumber] || ignoreRepetition)
                    {
                        if (logRepetition > 0)
                        {
                            Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: Previous message repeated " + logRepetitions[self.playerState.playerNumber] + " times: " + prevLogs[self.playerState.playerNumber]);
                        }
                        prevLogs[self.playerState.playerNumber] = message;
                        logRepetitions[self.playerState.playerNumber] = 0;
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message);
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
                Ebug(err, logPrio: 4, asregular: true);
            }
        }

        public static void Ebug(Player self, object message, int logPrio = 3)
        {
            if (self == null)
            {
                Ebug(message, logPrio);
            }
            try
            {
                if (logPrio <= logImportance)
                {
                    Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.ToString());
                }
            }
            catch (Exception err)
            {
                Ebug(message, logPrio);
                Ebug(err, logPrio: 4, asregular: true);
            }
        }
        public static void Ebug(Player self, string[] messages, int logPrio = 3, bool separated = true)
        {
            if (self == null)
            {
                Ebug(messages, logPrio, separated);
            }
            try
            {
                if (logPrio <= logImportance)
                {
                    if (separated)
                    {
                        String message = "";
                        foreach (String msg in messages)
                        {
                            message += ", " + msg;
                        }
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2));
                    }
                    else
                    {
                        for (int i = 0; i < messages.Length; i++)
                        {
                            if (i == 0)
                            {
                                Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i]);
                            }
                            else
                            {
                                Debug.Log("->        [" + self.playerState.playerNumber + "]: " + messages[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(messages, logPrio, separated);
                Ebug(err, logPrio: 4, asregular: true);
            }

        }
        public static void Ebug(Player self, object[] messages, int logPrio = 3, bool separated = true)
        {
            if (self == null)
            {
                Ebug(messages, logPrio, separated);
            }
            try
            {
                if (logPrio <= logImportance)
                {
                    if (separated)
                    {
                        string message = "";
                        foreach (object msg in messages)
                        {
                            message += ", " + msg.ToString();
                        }
                        Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + message.Substring(2));
                    }
                    else
                    {
                        for (int i = 0; i < messages.Length; i++)
                        {
                            if (i == 0)
                            {
                                Debug.Log("-> Escort[" + self.playerState.playerNumber + "]: " + messages[i].ToString());
                            }
                            else
                            {
                                Debug.Log("->         [" + self.playerState.playerNumber + "]: " + messages[i].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(messages, logPrio, separated);
                Ebug(err, logPrio: 4, asregular: true);
            }
        }
        public static void Ebug(Player self, Exception exception, string message = "caught error!", int logPrio = 0, bool asregular = false)
        {
            if (self == null)
            {
                Ebug(exception, message, logPrio, asregular);
            }
            try
            {
                if (logPrio <= logImportance)
                {
                    if (asregular)
                    {
                        Debug.LogWarning("-> ERcORt[" + self.playerState.playerNumber + "]: " + message + " => " + exception.Message);
                        if (exception.StackTrace != null)
                        {
                            Debug.LogWarning("->       : " + exception.StackTrace);
                        }
                    }
                    else
                    {
                        Debug.LogError("-> ERcORt[" + self.playerState.playerNumber + "]: " + message);
                        if (exception.StackTrace != null)
                        {
                            Debug.LogError("->       [" + self.playerState.playerNumber + "]: " + exception.StackTrace);
                        }
                        Debug.LogException(exception);
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(exception, message, logPrio, asregular);
                Ebug(err, logPrio: 4, asregular: true);
            }
        }



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
    }
}