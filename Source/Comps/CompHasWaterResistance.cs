using System;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SlippersNmSpc;

public class CompHasWaterResistance : CompFootwearEffect
{
    public override void ApplyEffect()
    {
        // Implement water resistance effect
        SlippersUtility.DebugLog($"{parent.Label} gains water resistance.");
        // For example, adjust stats or apply a Hediff
    }
}

public class CompProperties_WaterResistance : CompProperties_FootwearEffect
{
    public float resistanceValue = 0.8f;

    public CompProperties_WaterResistance()
    {
        compClass = typeof(CompHasWaterResistance);
    }
}