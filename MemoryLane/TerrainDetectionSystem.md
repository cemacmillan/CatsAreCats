# Terrain Detection System for Slippers Mod

## Overview
This document describes the terrain detection system implemented in the Slippers mod, which provides a clean way to detect terrain changes and signal other mods about situational context changes.

## Key Components

### 1. CompTerrainDetector
- **Purpose**: Detects terrain changes and classifies them into meaningful categories
- **Location**: `Source/Comps/CompTerrainDetector.cs`
- **Features**:
  - Monitors terrain changes every 60 ticks
  - Classifies terrain into categories: Swamp, Rough, Smooth, Normal
  - Provides public methods for other mods to query terrain state
  - Signals terrain changes for inter-mod communication

### 2. Terrain Classifications
The system classifies terrain types as follows:

```csharp
// Swamp-like conditions (useful for Accidents mod)
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
```

### 3. Public API for Other Mods
The `CompTerrainDetector` provides these methods for other mods to query terrain state:

- `GetCurrentTerrainType()` - Returns current terrain classification
- `IsInSwampLikeTerrain()` - Boolean check for swamp conditions
- `IsInRoughTerrain()` - Boolean check for rough terrain
- `IsInSmoothTerrain()` - Boolean check for smooth terrain

### 4. Integration Points
- **Slippers**: Get terrain detector component
- **Moccasins**: Get terrain detector component  
- **Mukluks**: Get terrain detector component

## Usage Example for Accidents Mod

```csharp
// Get the terrain detector from a pawn's footwear
var terrainDetector = pawn.apparel?.WornApparel
    .FirstOrDefault(apparel => apparel.GetComp<CompTerrainDetector>() != null)
    ?.GetComp<CompTerrainDetector>();

if (terrainDetector != null)
{
    if (terrainDetector.IsInSwampLikeTerrain())
    {
        // Increase minibeast attack chance in swamp conditions
        // This is where Accidents mod would hook in
    }
}
```

## Benefits

1. **Separation of Concerns**: Terrain detection is separate from movement speed modifications
2. **Inter-Mod Communication**: Clean API for other mods to query terrain state
3. **Performance**: Only processes terrain changes, not every tick
4. **Extensibility**: Easy to add new terrain classifications
5. **Stability**: Avoids complex StatPart integration that caused InfoCard crashes

## Future Enhancements

1. **Message Bus Integration**: Could implement a proper message bus for mod communication
2. **Event System**: Could fire events when terrain changes occur
3. **Additional Classifications**: Could add more terrain categories as needed
4. **Caching**: Could implement more sophisticated caching for performance

## Files Created/Modified

- `Source/Comps/CompTerrainDetector.cs` - Main terrain detection component
- `Source/Comps/CompProperties_TerrainDetector.cs` - Properties for the component
- `Source/Comps/CompDynamicApparelComps.cs` - Updated to use terrain detector
- `Source/DynamicCompPropertiesRegistry.cs` - Registered new component

## Notes

This system was created as an alternative to the problematic StatPart approach that was causing InfoCard crashes. The terrain detection functionality is preserved and made available to other mods without interfering with RimWorld's core stat system.
