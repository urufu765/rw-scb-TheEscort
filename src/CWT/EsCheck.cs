using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace TheEscort
{
    internal static class EscortFunctionChecks
    {
        public class EFD
        {
            private readonly Dictionary<string, int> accessedNames;
            private int accessTimes;

            public EFD()
            {
                this.accessedNames = new Dictionary<string, int>();
            }


            public void SetAccess(bool check, string name)
            {
                if (!this.accessedNames.ContainsKey(name))
                {
                    this.accessedNames.Add(name, 0);
                }
                if (check) this.accessedNames[name]++;
                else accessTimes++;
            }

            public override string ToString()
            {
                string finalString = "";
                foreach (KeyValuePair<string, int> v in this.accessedNames)
                {
                    if (finalString != "")
                    {
                        finalString += System.Environment.NewLine;
                    }
                    finalString += "  Access-" + accessTimes;
                    finalString += " /Checked-" + v.Value;
                    if (v.Key != "")
                    {
                        finalString += " <" + v.Key + ">";
                    }
                }
                return finalString;
            }

            /*
            public string NoString(){
                string finalString = "";
                finalString += "A-" + accessedCheck[0];
                finalString += " /C-" + accessedCheck[1];
                if (accessedName != ""){
                    finalString += " <" + accessedName + ">";
                }
                return finalString;
            }*/
        }

        public class EFC
        {
            private readonly Dictionary<string, EFD> EscheckFunctionDictionary;
            private readonly Queue<string> EscheckFunctionLastChecks;
            private string lastAccess = "";
            private string lastCheck = "";
            private static readonly bool NOLOG = true;
            private bool frequentLogMode = false;
            public bool Crispmunch { get; private set; }

            public EFC()
            {
                this.EscheckFunctionDictionary = new Dictionary<string, EFD>(128);
                this.EscheckFunctionLastChecks = new Queue<string>(32);
                this.Crispmunch = true;
            }

            public void TurnOnLog()
            {
                UnityEngine.Debug.LogWarning("Turned on frequent checker log mode. All methods hooked by Escort will be logged!");
                this.frequentLogMode = true;
            }

            public void TurnOffLog()
            {
                UnityEngine.Debug.LogWarning("Turned off frequent checker log mode. Only methods hooked by Escort that don't update frequently will be logged!");
                this.frequentLogMode = false;
            }

            public void Christmas(bool hasArrived = false)
            {
                this.Crispmunch = !hasArrived;
            }

            public void Set(bool isChecked = false, [CallerMemberName] string callerName = "")
            {
                if (NOLOG)
                {
                    return;
                }
                try
                {
                    if (!EscheckFunctionDictionary.ContainsKey(callerName))
                    {
                        EscheckFunctionDictionary.Add(callerName, new EFD());
                    }
                    EscheckFunctionDictionary[callerName].SetAccess(isChecked, "");
                    if (lastAccess != callerName)
                    {
                        if (this.EscheckFunctionLastChecks.Count == 31)
                        {
                            this.EscheckFunctionLastChecks.Dequeue();
                        }
                        this.EscheckFunctionLastChecks.Enqueue("Name: " + callerName + " | Access Success: " + (isChecked ? "eyup." : "Not A Check"));
                        lastAccess = callerName;
                    }
                }
                catch (ArgumentException)
                {
                    UnityEngine.Debug.LogError("Something went wrong while setting value!");
                }
                catch (KeyNotFoundException)
                {
                    UnityEngine.Debug.LogError("Somehow misplaced the key!");
                }
            }

            public void Set(string functionname, bool isChecked = true, [CallerMemberName] string callerName = "")
            {
                if (NOLOG)
                {
                    return;
                }
                try
                {
                    if (!EscheckFunctionDictionary.ContainsKey(callerName))
                    {
                        EscheckFunctionDictionary.Add(callerName, new EFD());
                    }
                    EscheckFunctionDictionary[callerName].SetAccess(isChecked, functionname);
                    if (lastAccess != callerName)
                    {
                        lastCheck = "";
                    }
                    if (lastCheck != functionname)
                    {
                        if (this.EscheckFunctionLastChecks.Count == 31)
                        {
                            this.EscheckFunctionLastChecks.Dequeue();
                        }
                        this.EscheckFunctionLastChecks.Enqueue("Name: " + callerName + " | SubName: " + functionname + " | Access Success: " + (isChecked ? "eyup." : "NO!!!"));
                        lastAccess = callerName;
                        lastCheck = functionname;
                    }
                }
                catch (ArgumentException)
                {
                    UnityEngine.Debug.LogError("Something went wrong while setting value!");
                }
                catch (KeyNotFoundException)
                {
                    UnityEngine.Debug.LogError("Somehow misplaced the key!");
                }
            }

            public void SetF(bool isChecked = false, [CallerMemberName] string callerName = "")
            {
                if (NOLOG)
                {
                    return;
                }
                if (!frequentLogMode)
                {
                    return;
                }
                try
                {
                    if (!EscheckFunctionDictionary.ContainsKey(callerName))
                    {
                        EscheckFunctionDictionary.Add(callerName, new EFD());
                    }
                    EscheckFunctionDictionary[callerName].SetAccess(isChecked, "");
                    if (lastAccess != callerName)
                    {
                        if (this.EscheckFunctionLastChecks.Count == 31)
                        {
                            this.EscheckFunctionLastChecks.Dequeue();
                        }
                        this.EscheckFunctionLastChecks.Enqueue("Name: " + callerName + " | Access Success: " + (isChecked ? "eyup." : "Not A Check"));
                        lastAccess = callerName;
                    }
                }
                catch (ArgumentException)
                {
                    UnityEngine.Debug.LogError("Something went wrong while setting value!");
                }
                catch (KeyNotFoundException)
                {
                    UnityEngine.Debug.LogError("Somehow misplaced the key!");
                }
            }

            public void SetF(string functionname, bool isChecked = true, [CallerMemberName] string callerName = "")
            {
                if (NOLOG)
                {
                    return;
                }
                if (!frequentLogMode)
                {
                    return;
                }
                try
                {
                    if (!EscheckFunctionDictionary.ContainsKey(callerName))
                    {
                        EscheckFunctionDictionary.Add(callerName, new EFD());
                    }
                    EscheckFunctionDictionary[callerName].SetAccess(isChecked, functionname);
                    if (lastAccess != callerName)
                    {
                        lastCheck = "";
                    }
                    if (lastCheck != functionname)
                    {
                        if (this.EscheckFunctionLastChecks.Count == 31)
                        {
                            this.EscheckFunctionLastChecks.Dequeue();
                        }
                        this.EscheckFunctionLastChecks.Enqueue("Name: " + callerName + " | SubName: " + functionname + " | Access Success: " + (isChecked ? "eyup." : "NO!!!"));
                        lastAccess = callerName;
                        lastCheck = functionname;
                    }
                }
                catch (ArgumentException)
                {
                    UnityEngine.Debug.LogError("Something went wrong while setting value!");
                }
                catch (KeyNotFoundException)
                {
                    UnityEngine.Debug.LogError("Somehow misplaced the key!");
                }
            }

            public void LetItRip(String message = "Final", bool fatal = false)
            {
                string lastStr = "";
                if (fatal)
                {
                    UnityEngine.Debug.LogError("======================");
                    UnityEngine.Debug.LogError("Escort mod accesses/checks (helps Deathpits with some debugging) but ERROR");
                    UnityEngine.Debug.LogError("----------------------");
                    UnityEngine.Debug.LogError("-" + message + " Access/Checks-");
                    foreach (KeyValuePair<string, EFD> eCheckz in this.EscheckFunctionDictionary)
                    {
                        if (eCheckz.Key != lastStr)
                        {
                            UnityEngine.Debug.LogError(eCheckz.Key + ":");
                        }
                        UnityEngine.Debug.LogError(eCheckz.Value.ToString());
                    }
                    UnityEngine.Debug.LogError("----------------------");
                    UnityEngine.Debug.LogError("-" + message + " Latest Accesses-");
                    foreach (string msg in this.EscheckFunctionLastChecks)
                    {
                        UnityEngine.Debug.LogError(msg);
                    }
                    UnityEngine.Debug.LogError("======================");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("======================");
                    UnityEngine.Debug.LogWarning("Escort mod accesses/checks (helps Deathpits with some debugging)");
                    UnityEngine.Debug.LogWarning("----------------------");
                    UnityEngine.Debug.LogWarning("-" + message + " Access/Checks-");
                    foreach (KeyValuePair<string, EFD> eCheckz in this.EscheckFunctionDictionary)
                    {
                        if (eCheckz.Key != lastStr)
                        {
                            UnityEngine.Debug.LogWarning(eCheckz.Key + ":");
                        }
                        UnityEngine.Debug.LogWarning(eCheckz.Value.ToString());
                    }
                    UnityEngine.Debug.LogWarning("----------------------");
                    UnityEngine.Debug.LogWarning("-" + message + " Latest Accesses-");
                    foreach (string msg in this.EscheckFunctionLastChecks)
                    {
                        UnityEngine.Debug.LogWarning(msg);
                    }
                    UnityEngine.Debug.LogWarning("======================");
                }
            }
        }

        private static readonly ConditionalWeakTable<Plugin, EFC> CWT = new();
        public static EFC L(this Plugin instance) => CWT.GetValue(instance, _ => new());
    }
}