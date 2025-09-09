using HarmonyLib;
using RimWorld;
using Verse;
using SlippersNmSpc;
using UnityEngine;
using System.Collections.Generic;

namespace SlippersNmSpc;

[HarmonyPatch(typeof(GenClamor), "DoClamor", new[] { typeof(Thing), typeof(IntVec3), typeof(float), typeof(GenClamor.ClamorEffect) })]
public static class Patch_DoClamor
{
    private static readonly Dictionary<string, int> ProcessedClamors = new Dictionary<string, int>();
    private static int currentTick = 0;
    
    public static bool Prefix(Thing source, IntVec3 position, float radius, GenClamor.ClamorEffect clamorEffect)
    {
        // Only reduce clamor from pawns
        if (!(source is Pawn sourcePawn)) return true;
        
        // Update current tick for cleanup
        currentTick = Find.TickManager.TicksGame;
        
        // Create a unique key for this clamor event to prevent multiple processing
        string clamorKey = $"{sourcePawn.ThingID}_{position.x}_{position.z}_{radius}";
        
        // Skip if we've already processed this clamor recently (within 60 ticks)
        if (ProcessedClamors.TryGetValue(clamorKey, out int lastProcessedTick) && 
            currentTick - lastProcessedTick < 60)
        {
            return true; // Let the original method handle it
        }
        
        // Check if the source pawn has clamor-reducing footwear
        var clamorReducer = sourcePawn.apparel?.WornApparel
            .FirstOrDefault(apparel => apparel.GetComp<CompClamorReducer>() != null)
            ?.GetComp<CompClamorReducer>();
            
        if (clamorReducer == null) return true;
        
        // Roll for clamor reduction
        float reductionChance = clamorReducer.GetClamorReductionChance();
        if (Rand.Chance(reductionChance))
        {
            // Mark this clamor as processed
            ProcessedClamors[clamorKey] = currentTick;
            
            // Reduce the radius of the clamor
            float reducedRadius = radius * 0.3f; // Reduce to 30% of original radius
            
            SlippersUtility.DebugLog($"[Slippers] {sourcePawn.LabelShort}'s {clamorReducer.parent.LabelCap} reduced clamor radius from {radius} to {reducedRadius}.");
            
            // Create a modified clamor effect with reduced radius
            var modifiedClamorEffect = new GenClamor.ClamorEffect(delegate(Thing _, Pawn hearer)
            {
                // Only apply clamor if within the reduced radius
                float hearingLevel = Mathf.Clamp01(hearer.health.capacities.GetLevel(PawnCapacityDefOf.Hearing));
                if (hearingLevel > 0f && hearer.Position.InHorDistOf(position, reducedRadius * hearingLevel))
                {
                    clamorEffect(source, hearer);
                }
            });
            
            // Call the original method with modified parameters
            GenClamor.DoClamor(source, position, reducedRadius, modifiedClamorEffect);
            
            return false; // Skip the original method
        }
        
        return true; // Proceed normally if no reduction
    }
    
    // Clean up old entries periodically
    public static void CleanupOldEntries()
    {
        if (currentTick % 1000 == 0) // Every 1000 ticks
        {
            var keysToRemove = new List<string>();
            foreach (var kvp in ProcessedClamors)
            {
                if (currentTick - kvp.Value > 3000) // Remove entries older than 3000 ticks
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                ProcessedClamors.Remove(key);
            }
        }
    }
}
