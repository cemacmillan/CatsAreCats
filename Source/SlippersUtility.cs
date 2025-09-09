using Verse;
using System.Linq;

namespace SlippersNmSpc;

public static class SlippersUtility
{
    public static bool HasMaterialTag(ThingDef material, string tag)
    {
        if (material == null)
            return false;

        var extension = material.GetModExtension<MaterialTagDefExtension>();
        return extension != null && extension.materialTags != null && extension.materialTags.Contains(tag);
    }

    public static bool IsWaterproofMaterial(ThingDef material)
    {
        return HasMaterialTag(material, "WaterproofMaterial");
    }

    public static bool IsInsulatingMaterial(ThingDef material)
    {
        return HasMaterialTag(material, "InsulatingMaterial");
    }

    public static bool IsComfortMaterial(ThingDef material)
    {
        return HasMaterialTag(material, "ComfortMaterial");
    }

    public static bool IsBeautyMaterial(ThingDef material)
    {
        return HasMaterialTag(material, "BeautyMaterial");
    }

    public static void DebugLog(string message)
    {
        if (SlippersMod.Settings.EnableLogging)
        {
            Log.Message("[Slippers] " + message);
        }
    }
    
    public static void DebugWarn(string message)
    {
        if (SlippersMod.Settings.EnableLogging)
        {
            Log.Warning("[Slippers] " + message);
        }
    }
}