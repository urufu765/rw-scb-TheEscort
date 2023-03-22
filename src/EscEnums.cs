using System;

namespace TheEscort{
    public class EscEnums{
        private static bool registered = false;
        public static void RegisterValues(){
            if (!EscEnums.registered){
                EscEnums.registered = true;
            }
        }

        public static void UnregisterValues(){
            if (EscEnums.registered){
                EscEnums.registered = false;
            }
        }
    }
}