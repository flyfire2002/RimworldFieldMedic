using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

// This file has two logical parts. The first part is the FieldMedic_Util class that defines
// CanBeStabilized() function, determining whether a Hediff (injury) is stabilizable.
// The second part is HediffComp_Stabilize, the modifier for an injury. This class is triggered
// after an stabilize job actually begins. It contains internal math of how much bleeding to surpress,
// how fast the bleeding resumes, and what icon to display for the stabilized effect.

namespace FieldMedic
{
    static class FieldMedic_Util
    {
        public static bool CanBeStabilized(this Hediff diff)
        {
            HediffWithComps hediff = diff as HediffWithComps;
            if (hediff == null)
            {
                return false;
            }
            if (hediff.BleedRate == 0f || hediff.IsTended() || hediff.IsPermanent())
            {
                return false;
            }
            HediffComp_Stabilize comp = hediff.TryGetComp<HediffComp_Stabilize>();
            return comp != null && !comp.Stabilized;
        }
    }

    [StaticConstructorOnStartup]
    public class HediffComp_Stabilize : HediffComp
    {
        private const float bleedIncreasePerSec = 0.0075f;    // After stabilizing, bleed modifier is increased by this much
        private const float internalBleedOffset = 0.3f;

        // Use ReductionLeft / Reduction to decide which icon to display.
        private float bleedReduction;
        private float bleedReductionLeft;

        private static readonly Texture2D Stabilized100Icon = ContentFinder<Texture2D>.Get("UI/Stabilized_icon_100");
        private static readonly Texture2D Stabilized75Icon = ContentFinder<Texture2D>.Get("UI/Stabilized_icon_75");
        private static readonly Texture2D Stabilized50Icon = ContentFinder<Texture2D>.Get("UI/Stabilized_icon_50");
        private static readonly Texture2D Stabilized25Icon = ContentFinder<Texture2D>.Get("UI/Stabilized_icon_25");

        private bool stabilized = false;
        private float bleedModifier = 1;

        public HediffCompProperties_Stabilize Props { get { return props as HediffCompProperties_Stabilize; } }
        public bool Stabilized { get { return stabilized; } }
        public float BleedModifier
        {
            get
            {
                float mod = bleedModifier;
                if (parent.Part.depth == BodyPartDepth.Inside) mod += internalBleedOffset;
                return Mathf.Clamp01(mod);
            }
        }

        public void Stabilize(Pawn medic, Apparel_flyfire2002_MedicBag medicbag)
        {
            if (stabilized)
            {
                Log.Error("FieldMedic tried to stabilize an injury that is already stabilized");
                return;
            }
            if (medicbag == null)
            {
                Log.Error("FieldMedic tried to stabilize without a medicbag");
                return;
            }
            bleedReduction = 1.0f - 0.8f * medic.GetStatValue(StatDefOf.MedicalTendQuality);
            // Especially high treatment quality extends time at 0% bleed by setting bleedModifier to a negative number,
            // which is then clampped [0, 1] in BleedModifier getter. However, the overflow is diminished by half.
            if (bleedReduction < 0)
            {
                bleedReduction /= 2;
            }
            bleedModifier = bleedReduction;
            // Now we get the "real" bleed reduction amount, and set the numerator for the icon fullness.
            bleedReduction = 1.0f - bleedModifier;
            bleedReductionLeft = bleedReduction;
            stabilized = true;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref stabilized, "stabilized", false);
            Scribe_Values.Look(ref bleedModifier, "bleedModifier", 1);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            // Increase bleed modifier once per second
            if (stabilized && bleedModifier < 1 && parent.ageTicks % 60 == 0)
            {
                bleedModifier += bleedIncreasePerSec;
                bleedReductionLeft -= bleedIncreasePerSec;
                if (bleedModifier >= 1)
                {
                    bleedModifier = 1;
                    stabilized = false;
                }
            }
        }

        public override TextureAndColor CompStateIcon
        {
            get
            {
                if (bleedModifier < 1 && !parent.IsPermanent() && !parent.IsTended())
                {
                    if (bleedReductionLeft / bleedReduction > 0.75f) {
                        return new TextureAndColor(Stabilized100Icon, Color.white);
                    }
                    if (bleedReductionLeft / bleedReduction > 0.5f)
                    {
                        return new TextureAndColor(Stabilized75Icon, Color.white);
                    }
                    if (bleedReductionLeft / bleedReduction > 0.25f)
                    {
                        return new TextureAndColor(Stabilized50Icon, Color.white);
                    }
                    return new TextureAndColor(Stabilized25Icon, Color.white);

                }
                return TextureAndColor.None;
            }
        }

        public override string CompDebugString()
        {
            if (parent.BleedRate < 0) return "Not bleeding";
            if (!stabilized) return "Not stabilized";
            return String.Concat("Stabilized", parent.Part.depth == BodyPartDepth.Inside ? " internal bleeding" : "", "\nbleed rate modifier: ", bleedModifier.ToString());
        }
    }
}
