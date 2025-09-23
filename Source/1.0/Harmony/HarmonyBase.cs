using System.Reflection;
using Harmony;
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
            var harmonyInstance = HarmonyInstance.Create("FieldMedic");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            PatchHediffWithComps(harmonyInstance);
            Log.Message("FieldMedic inited");
        }

        private static void PatchHediffWithComps(HarmonyInstance harmonyInstance)
        {
            var postfixBleedRate = typeof(Harmony_HediffWithComps_BleedRate_Patch).GetMethod("Postfix");
            var baseType = typeof(HediffWithComps);
            var types = baseType.AllSubclassesNonAbstract().Add(baseType);
            foreach (Type cur in types)
            {
                harmonyInstance.Patch(cur.GetProperty("BleedRate").GetGetMethod(), null, new HarmonyMethod(postfixBleedRate));
            }
        }
    }
}