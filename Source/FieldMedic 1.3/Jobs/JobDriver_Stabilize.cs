using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

// This file has the JobDriver for stabilize. In the class we have access to the pawn, the patient and the
// medic bag. The code is a little messy, but the important part is where we determine how much deterioration
// should be applied to the medic bag according to the injury treated.
namespace FieldMedic
{
    public class JobDriver_Stabilize : JobDriver
    {
        private const float baseTendDuration = 64f;

        private Pawn Patient { get { return pawn.CurJob.targetA.Thing as Pawn; } }
        private Apparel_flyfire2002_MedicBag MedicBag { get { return pawn.CurJob.targetB.Thing as Apparel_flyfire2002_MedicBag; } }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Patient == null || MedicBag == null);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnNotDowned(TargetIndex.A);
            this.AddEndCondition(delegate
            {
                if (Patient.health.hediffSet.GetHediffsTendable().Any(h => h.CanBeStabilized())) return JobCondition.Ongoing;
                return JobCondition.Incompletable;
            });
            
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // Stabilize patient
            int duration = (int)(1f / this.pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * baseTendDuration);
            Toil waitToil = Toils_General.Wait(duration).WithProgressBarToilDelay(TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
            yield return waitToil;
            Toil stabilizeToil = new Toil();
            stabilizeToil.initAction = delegate
            {
                float xp = (!Patient.RaceProps.Animal) ? 125f : 50f * MedicBag.def.MedicineTendXpGainFactor;
                pawn.skills.Learn(SkillDefOf.Medicine, xp);
                foreach (Hediff curInjury in from x in Patient.health.hediffSet.GetHediffsTendable() orderby x.BleedRate descending select x)
                {
                    if (curInjury.CanBeStabilized())
                    {
                        // The deterioration on medic bag is a function of the bleedRate of the injury that's being tended.
                        // If the bleed rate <= 100%, the cost is 3%
                        float bleedRate = curInjury.BleedRate - 1.0f;
                        float cost = 0.03f;
                        // bleed rate >= 400%, cost 8%. 
                        if (bleedRate >= 3.0f)
                        {
                            cost = 0.08f;
                        } else if (bleedRate >= 0)
                        // in between, the cost is linear to the bleed rate.
                        {
                            cost = bleedRate / 3.0f * 0.05f + 0.03f;
                        }
                        Log.Message("[FieldMedic] Bleedrate: " + bleedRate + "; Cost: " + cost);
                        HediffComp_Stabilize comp = curInjury.TryGetComp<HediffComp_Stabilize>();
                        comp.Stabilize(pawn, MedicBag);
                        // The idea is to limit the use of the bag. You can use it below 50%, but heed the mood debuff.
                        MedicBag.HitPoints = MedicBag.HitPoints - (int)Math.Round(MedicBag.MaxHitPoints * cost, 0);
                        if (MedicBag.HitPoints <= 0)
                        {
                            MedicBag.Destroy();
                        }
                        break;
                    }
                }
            };
            stabilizeToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return stabilizeToil;
            yield return Toils_Jump.Jump(waitToil);
        }
    }
}
