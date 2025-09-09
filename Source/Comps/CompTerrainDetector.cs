using RimWorld;
using Verse;
using System.Collections.Generic;
using System;

namespace SlippersNmSpc;

/// <summary>
/// Component that detects terrain changes and can signal other mods about situational context changes.
/// This is separate from movement speed modifications and focuses on terrain awareness.
/// </summary>
public class CompTerrainDetector : ThingComp, IDynamicComp
{
    public CompProperties_TerrainDetector Props => (CompProperties_TerrainDetector)props;

    private Pawn Wearer => (parent as Apparel)?.Wearer;
    private TerrainDef lastTerrain = null;
    private string lastTerrainType = null;

    // Terrain classification for external mod communication
    private static readonly Dictionary<string, string> TerrainClassifications = new Dictionary<string, string>
    {
        // Swamp-like conditions
        {"Marsh", "Swamp"},
        {"Mud", "Swamp"},
        
        // Rough terrain
        {"Sand", "Rough"},
        {"Gravel", "Rough"},
        {"RoughStone", "Rough"},
        
        // Smooth terrain
        {"WoodFloor", "Smooth"},
        {"Carpet", "Smooth"},
        {"PavedTile", "Smooth"},
        {"Bridge", "Smooth"},
    };

    public override void CompTick()
    {
        base.CompTick();

        if (Wearer != null && Wearer.IsHashIntervalTick(60))
        {
            CheckTerrainChange();
        }
    }

    private void CheckTerrainChange()
    {
        if (Wearer?.Map == null) return;

        TerrainDef currentTerrain = Wearer.Position.GetTerrain(Wearer.Map);
        
        // Only process if terrain actually changed
        if (currentTerrain == lastTerrain) return;
        
        lastTerrain = currentTerrain;
        string newTerrainType = GetTerrainClassification(currentTerrain);
        
        if (newTerrainType != lastTerrainType)
        {
            lastTerrainType = newTerrainType;
            OnTerrainTypeChanged(newTerrainType);
        }
    }

    private string GetTerrainClassification(TerrainDef terrain)
    {
        if (terrain == null) return "Unknown";
        
        return TerrainClassifications.TryGetValue(terrain.defName, out string classification) 
            ? classification 
            : "Normal";
    }

    private void OnTerrainTypeChanged(string newTerrainType)
    {
        SlippersUtility.DebugLog($"[TerrainDetector] {Wearer.LabelShort} entered {newTerrainType} terrain.");
        
        // Signal other mods about terrain change
        // This could be extended to communicate with Accidents mod or other systems
        SignalTerrainChange(newTerrainType);
    }

    private void SignalTerrainChange(string terrainType)
    {
        // This is where we could add communication with other mods
        // For example, using a message bus or direct mod communication
        
        // Example: Signal to Accidents mod that pawn is in swamp-like conditions
        if (terrainType == "Swamp")
        {
            // Could send a signal to Accidents mod here
            SlippersUtility.DebugLog($"[TerrainDetector] Signaling Accidents mod: Pawn in swamp-like terrain");
        }
        
        // Example: Signal other systems about rough terrain
        if (terrainType == "Rough")
        {
            SlippersUtility.DebugLog($"[TerrainDetector] Signaling other mods: Pawn in rough terrain");
        }
    }

    public void NotifyEquipped(Pawn pawn)
    {
        lastTerrain = null;
        lastTerrainType = null;
    }

    public void NotifyUnequipped(Pawn pawn)
    {
        lastTerrain = null;
        lastTerrainType = null;
    }

    public void onEquippedTick(Pawn pawn) { }
    public void onEquippedTickRare(Pawn pawn) { }

    // Public methods for other mods to query terrain state
    public string GetCurrentTerrainType()
    {
        return lastTerrainType ?? "Unknown";
    }

    public bool IsInSwampLikeTerrain()
    {
        return lastTerrainType == "Swamp";
    }

    public bool IsInRoughTerrain()
    {
        return lastTerrainType == "Rough";
    }

    public bool IsInSmoothTerrain()
    {
        return lastTerrainType == "Smooth";
    }
}
