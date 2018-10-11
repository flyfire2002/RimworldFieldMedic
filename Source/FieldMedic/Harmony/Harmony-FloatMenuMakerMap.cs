using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

namespace FieldMedic.Harmony
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    static class FloatMenuMakerMap_Modify_AddHumanlikeOrders
    {        
        [HarmonyPostfix]
        static void AddMenuItems(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Stabilize
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo curTarget in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
                {
                    Pawn patient = (Pawn)curTarget.Thing;
                    if (patient.Downed
                        && pawn.CanReach(patient, PathEndMode.InteractionCell, Danger.Deadly)
                        && patient.health.hediffSet.GetHediffsTendable().Any(h => h.CanBeStabilized()))
                    {
                        if (pawn.story.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                        {
                            opts.Add(new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + " (" + "IncapableOfCapacity".Translate(WorkTypeDefOf.Doctor.gerundLabel) + ")", null, MenuOptionPriority.Default));
                            return;
                        }

                        Apparel medicbagApparel = (Apparel)pawn.apparel.WornApparel.Find(t => t is Apparel_flyfire2002_MedicBag);
                        if (medicbagApparel == null || !(medicbagApparel is Apparel_flyfire2002_MedicBag))
                        {
                            opts.Add(new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + " (" + "FieldMedic_NoMedicBag".Translate() + ")", null, MenuOptionPriority.Default));
                            return;
                        }

                        string label = "FieldMedic_Stabilize".Translate(patient.LabelCap);
                        Action action = delegate
                        {
                            var medicbag = (Apparel_flyfire2002_MedicBag)medicbagApparel;
                            Job job = new Job(FieldMedic_JobDefOf.Stabilize, patient, medicbag);
                            job.count = 1;
                            pawn.jobs.TryTakeOrderedJob(job);
                        };
                        opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.Default, null, patient), pawn, patient, "ReservedBy"));
                    }
                }
            }
        }
    }
}