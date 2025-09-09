using System;
using System.Collections.Generic;
using Verse;

namespace SlippersNmSpc;

public static class DynamicCompPropertiesRegistry
{
    private static readonly Dictionary<Type, CompProperties> CompPropertiesMap = new();

    static DynamicCompPropertiesRegistry()
    {
        // Register default properties for dynamic comps
        RegisterDefaultProperties<CompFilthReducer>(new CompProperties_FilthReducer
        {
            cleanInterval = 2500
        });

        // Register default properties for dynamic comps
        RegisterDefaultProperties<CompPainCauser>(new CompProperties_PainCauser
        {
            stabInterval = 25000,
            painFactor = 0.2f
        });
        
        // Register default properties for dynamic comps
        RegisterDefaultProperties<CompComfortEnhancer>(new CompProperties_ComfortEnhancer
        {
            comfortFactor = 0.1f,
        });
        
        // Register default properties for dynamic comps
        RegisterDefaultProperties<CompFreedomSuppressor>(new CompProperties_FreedomSuppressor
        {
            constraintFactor = 0.2f,
            movementSpeedCap = 0.8f,
            pinchInterval = 60000,
        });
        
        
        RegisterDefaultProperties<CompHasSpecialInsulation>(new CompProperties_SpecialInsulation
        {
            insulationFactor = 1.8f
        });
        
        RegisterDefaultProperties<CompQualityEffects>(new CompProperties_QualityEffects
        {
            gleamInterval = 2500
        });
        
        RegisterDefaultProperties<CompClamorReducer>(new CompProperties_ClamorReducer
        {
            clamorReductionFactor = 0.3f
        });
        
        RegisterDefaultProperties<CompTerrainDetector>(new CompProperties_TerrainDetector());

        // Add other dynamic comps as needed
    }

    public static void RegisterDefaultProperties<TComp>(CompProperties properties) where TComp : ThingComp
    {
        var compType = typeof(TComp);
        if (!CompPropertiesMap.ContainsKey(compType))
        {
            CompPropertiesMap.Add(compType, properties);
        }
    }

    public static CompProperties GetPropertiesFor(Type compType)
    {
        return CompPropertiesMap.TryGetValue(compType, out var props) ? props : null;
    }
}