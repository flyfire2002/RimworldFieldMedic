using Verse;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

namespace FieldMedic
{
    static class Harmony_HediffWithComps_BleedRate_Patch
    {
        public static void Postfix(HediffWithComps __instance, ref float __result)
        {
            if (__result > 0)
            {
                // Check for stabilized comp
                HediffComp_Stabilize comp = __instance.TryGetComp<HediffComp_Stabilize>();
                if (comp != null)
                {
                    __result = __result * (comp.BleedModifier);
                    // Force update total bleeding rate. Why would the cache take multi-ingame-hours to refresh?
                    __instance.pawn.health.hediffSet.DirtyCache();
                }
            }
        }
    }
}
