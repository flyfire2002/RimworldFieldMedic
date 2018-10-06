using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace FieldMedic.Harmony
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    static class FloatMenuMakerMap_Modify_AddHumanlikeOrders
    {
        static readonly string logPrefix = Assembly.GetExecutingAssembly().GetName().Name + " :: " + typeof(FloatMenuMakerMap_Modify_AddHumanlikeOrders).Name + " :: ";

        /* 
         * Opted for a postfix as the original Detour had the code inserted generally after other code had run and because we want the target's code
         * to always run unmodified.
         * There are two goals for this postfix, to add menu items for stabalizing a target and to add inventory pickup functions for pawns.
         * -Both when right clicking on something with a pawn selected.
         */

        // __instance isn't apt, target is static.
        // __result isn't apt, target return is void.
        [HarmonyPostfix]
        static void AddMenuItems(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Stabilize
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo curTarget in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true)) // !! This needs to be patched into A17
                {
                    Pawn patient = (Pawn)curTarget.Thing;
                    if (patient.Downed
                        //&& pawn.CanReserveAndReach(patient, PathEndMode.InteractionCell, Danger.Deadly)
                        && pawn.CanReach(patient, PathEndMode.InteractionCell, Danger.Deadly)
                        && patient.health.hediffSet.GetHediffsTendable().Any(h => h.CanBeStabilized()))
                    {
                        if (pawn.story.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                        {
                            opts.Add(new FloatMenuOption("CE_CannotStabilize".Translate() + ": " + "IncapableOfCapacity".Translate(WorkTypeDefOf.Doctor.gerundLabel), null, MenuOptionPriority.Default));
                        }
                        else
                        {
                            string label = "Stabilize "; // "CE_Stabilize".Translate(patient.LabelCap);
                            Action action = delegate
                            {
                                Apparel medicbag = (Apparel)pawn.apparel.WornApparel.OrderByDescending(t => t.GetStatValue(StatDefOf.MedicalPotency)).FirstOrDefault();
                                Log.Error("medic bag potency: " + medicbag.GetStatValue(StatDefOf.MedicalPotency).ToString("0.00"));
                                if (medicbag.GetStatValue(StatDefOf.MedicalPotency) != 0)
                                {
                                    Job job = new Job(FieldMedic_JobDefOf.Stabilize, patient, medicbag);
                                    job.count = 1;
                                    pawn.jobs.TryTakeOrderedJob(job);
                                }
                            };
                            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.Default, null, patient), pawn, patient, "ReservedBy"));
                        }
                    }
                }
            }
        }
    }
}