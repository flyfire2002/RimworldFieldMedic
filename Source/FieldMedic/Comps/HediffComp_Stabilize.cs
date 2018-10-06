﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

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
        private const float bleedIncreasePerSec = 0.01f;    // After stabilizing, bleed modifier is increased by this much
        private const float internalBleedOffset = 0.2f;

        private static readonly Texture2D StabilizedIcon = ContentFinder<Texture2D>.Get("UI/Stabilized_Icon"); // TODO: Actually draw something

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
            float bleedReduction = 1.5f * medic.GetStatValue(StatDefOf.MedicalTendQuality);
            // Especially high treatment quality extends time at 0% bleed by setting bleedModifier to a negative number,
            // which is then clampped [0, 1] in BleedModifier getter.
            bleedModifier = 1 - bleedReduction;
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
                bleedModifier = bleedModifier + bleedIncreasePerSec;
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
                if (bleedModifier < 1 && !parent.IsPermanent() && !parent.IsTended()) return new TextureAndColor(StabilizedIcon, Color.white);
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
