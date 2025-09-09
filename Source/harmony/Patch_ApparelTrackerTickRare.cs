using System;
using HarmonyLib;
using RimWorld;
using Verse;
using static SlippersNmSpc.MMToolkit;

namespace SlippersNmSpc;

[HarmonyPatch(typeof(Pawn_ApparelTracker), "ApparelTrackerTickRare")]
public static class Patch_ApparelTrackerTickRare
{
    public static void Postfix(Pawn_ApparelTracker __instance)
    {
        // Validate the pawn owning the apparel tracker
        Pawn pawn = __instance.pawn;
        if (pawn == null || pawn.Dead)
        {
            GripeOnce($"[Slippers] Pawn_ApparelTrackerTickRare: Pawn is null or dead for ApparelTracker of {__instance.GetType().Name}. Skipping.");
            return;
        }

        foreach (Apparel apparel in __instance.WornApparel)
        {
            if (apparel.AllComps == null)
            {
                GripeOnce($"[ApparelTrackerTickRare] Skipping null comps for {apparel?.LabelCap ?? "unknown apparel"}.");
                continue;
            }

            foreach (ThingComp comp in apparel.AllComps)
            {
                if (comp is IDynamicComp dynamicComp)
                {
                    try
                    {
                        // not useful log any longer
                        // SlippersUtility.DebugLog($"FilthReducer OnEquippedTick called for {parent.LabelCap}, worn by {pawn.LabelShort}.");
                        dynamicComp.onEquippedTickRare(pawn);
                    }
                    catch (System.Exception ex)
                    {
                        GripeOnce($"[ApparelTrackerTickRare] Exception in OnEquippedTickRare for {pawn.LabelShort}: {ex}. Please notify the mod developer.");
                    }
                   
                }
            }
        }
    }
}