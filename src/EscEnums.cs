using System;

namespace TheEscort{
    public static class EscEnums{
        public static float rollin;
        private static bool registered = false;

        public static int EscortDropKickCooldown;
        public static float hypeMin;

        public static void RegisterValues(){
            if (!EscEnums.registered){
                EscEnums.rollin = 0f;
                EscEnums.hypeMin = 0f;
                EscEnums.EscortDropKickCooldown = 0;
                EscEnums.registered = true;
            }
        }

        public static void UnregisterValues(){
            if (EscEnums.registered){
                EscEnums.rollin = -1f;
                EscEnums.hypeMin = -1f;
                EscEnums.EscortDropKickCooldown = -1;
                EscEnums.registered = false;
            }
        }
    }
}