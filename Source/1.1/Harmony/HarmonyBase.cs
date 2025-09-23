using System.Reflection;
using HarmonyLib;
using Verse;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

namespace FieldMedic
{
    [StaticConstructorOnStartup]
    public static class HarmonyBase
    {
        static HarmonyBase()
        {
            var harmonyInstance = new Harmony("FieldMedic");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            PatchHediffWithComps(harmonyInstance);
            Log.Message("[FieldMedic] inited");
        }

        private static void PatchHediffWithComps(Harmony harmonyInstance)
        {
            var postfixBleedRate = typeof(Harmony_HediffWithComps_BleedRate_Patch).GetMethod("Postfix");
            var baseType = typeof(HediffWithComps);
            var types = baseType.AllSubclassesNonAbstract();
            foreach (Type cur in types)
            {
                var getMethod = cur.GetProperty("BleedRate").GetGetMethod();
                if (getMethod.IsVirtual && (getMethod.DeclaringType.Equals(cur)))
                {
                    Log.Message("[FieldMedic] patched get_BleedRate for " + cur);
                    harmonyInstance.Patch(getMethod, null, new HarmonyMethod(postfixBleedRate));
                }
            }
        }
    }
}