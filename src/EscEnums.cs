using System;

namespace TheEscort{
    public static class EscEnums{
        public static float rollin;
        private static bool registered = false;
        public static void RegisterValues(){
            if (!EscEnums.registered){
                EscEnums.rollin = 0f;
                EscEnums.registered = true;
            }
        }

        public static void UnregisterValues(){
            if (EscEnums.registered){
                EscEnums.rollin = 0f;
                EscEnums.registered = false;
            }
        }
    }
}