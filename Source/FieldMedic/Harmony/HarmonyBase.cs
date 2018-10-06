using System.Reflection;
using Harmony;
using Verse;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;

namespace FieldMedic.Harmony
{
    public static class HarmonyBase
    {
        private static HarmonyInstance harmony = null;
        static internal HarmonyInstance instance
        {
            get
            {
                if (harmony == null)
                    harmony = harmony = HarmonyInstance.Create("FieldMedic.Harmony");
                return harmony;
            }
        }

        public static void InitPatches()
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}