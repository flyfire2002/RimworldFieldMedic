using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

namespace FieldMedic
{
    public class HediffCompProperties_Stabilize : HediffCompProperties
    {
        public HediffCompProperties_Stabilize()
        {
            compClass = typeof(HediffComp_Stabilize);
        }
    }
}