using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace FieldMedic
{
    public class FloatMenuOptionProvider_Stabilize : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {

            if (context.FirstSelectedPawn.CanReach(clickedPawn, PathEndMode.InteractionCell, Danger.Deadly)
                && clickedPawn.health.hediffSet.GetHediffsTendable().Any(h => h.CanBeStabilized()))
            {
                if (context.FirstSelectedPawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                {
                    return new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + ": " + "IncapableOfCapacity".Translate(WorkTypeDefOf.Doctor.gerundLabel), null, MenuOptionPriority.Default);
                }

                Apparel medicbagApparel = (Apparel)context.FirstSelectedPawn.apparel.WornApparel.Find(t => t is Apparel_flyfire2002_MedicBag);
                if (medicbagApparel == null || !(medicbagApparel is Apparel_flyfire2002_MedicBag))
                {
                    return new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + ": " + "FieldMedic_NoMedicBag".Translate(), null, MenuOptionPriority.Default);
                }

                string label = "FieldMedic_Stabilize".Translate(clickedPawn.LabelCap);
                Action action = delegate
                {
                    var medicbag = (Apparel_flyfire2002_MedicBag)medicbagApparel;
                    Job job = new Job(FieldMedic_JobDefOf.Stabilize, clickedPawn, medicbag);
                    job.count = 1;
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job);
                };
                return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.Default, null, clickedPawn), context.FirstSelectedPawn, clickedPawn);
            }
            return null;
        }
    }
}
