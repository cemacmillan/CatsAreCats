using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SlippersNmSpc;

[HarmonyPatch(typeof(Pawn), "CheckForDisturbedSleep")]
public static class Patch_CheckForDisturbedSleep
{
    public static bool Prefix(Pawn __instance, Pawn source)
    {
        // Skip if the pawn is awake or ineligible
        if (__instance.Awake() || __instance.Deathresting || __instance.needs?.mood == null)
            return true;

        // Check for footwear that reduces sleep disturbance
        var footwear = __instance.apparel?.WornApparel
            .FirstOrDefault(apparel => apparel.GetComp<CompComfortEnhancer>() != null);

        if (footwear != null)
        {
            var comfortEnhancer = footwear.GetComp<CompComfortEnhancer>();
            if (comfortEnhancer != null)
            {
                // Roll for disturbance reduction
                float reductionChance = comfortEnhancer.GetSleepDisturbanceReductionChance();
                if (Rand.Chance(reductionChance))
                {
                    SlippersUtility.DebugLog($"[Slippers] {__instance.LabelShort}'s {footwear.LabelCap} prevented a sleep disturbance.");
                    return false; // Skip the rest of the method
                }
            }
        }

        return true; // Proceed normally if no enhancement is applied
    }
}