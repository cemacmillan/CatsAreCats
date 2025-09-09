using RimWorld;
using Verse;

namespace SlippersNmSpc;

public class CompComfortEnhancer : ThingComp, IDynamicComp
{
    public CompProperties_ComfortEnhancer Props => (CompProperties_ComfortEnhancer)props;

    private Pawn wearer;
    private float comfortFactor;

    public float ComfortFactor
    {
        get => comfortFactor;
        set => comfortFactor = value;
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);

        props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
        if (props is CompProperties_ComfortEnhancer comfortProps)
        {
            ComfortFactor = comfortProps.comfortFactor;
            //SlippersUtility.DebugLog($"[ComfortEnhancer] {this.GetType().Name} initialized for {parent.LabelCap} with ComfortFactor: {ComfortFactor}.");
        }
        else
        {
            MMToolkit.GripeOnce($"[CompComfortEnhancer::Initialize] {this.GetType().Name}: Missing or invalid properties during initialization for {parent.LabelCap}.");
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        if (respawningAfterLoad)
        {
            Initialize(props); // Reinitialize properties if needed
            //MMToolkit.DebugLog($"[CompComfortEnhancer] PostSpawnSetup after load for {parent.LabelCap}.");
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();

        // Reload properties from registry
        props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
        if (props is CompProperties_ComfortEnhancer comfortProps)
        {
            ComfortFactor = comfortProps.comfortFactor;
            MMToolkit.DebugLog($"[CompComfortEnhancer] PostExposeData: Reloaded for {parent.LabelCap} with ComfortFactor: {ComfortFactor}.");
        }
        else
        {
            MMToolkit.GripeOnce($"[CompComfortEnhancer] PostExposeData: Missing CompProperties for {parent.LabelCap} during PostExposeData.");
        }
    }

    public float GetSleepDisturbanceReductionChance()
    {
        var compQuality = parent.TryGetComp<CompQuality>();
        return compQuality?.Quality switch
        {
            QualityCategory.Awful => 0.1f,
            QualityCategory.Poor => 0.25f,
            QualityCategory.Normal => 0.5f,
            QualityCategory.Good => 0.7f,
            QualityCategory.Excellent => 0.9f,
            QualityCategory.Masterwork or QualityCategory.Legendary => 1.0f,
            _ => 0.5f
        };
    }

    public void NotifyEquipped(Pawn pawn)
    {
        wearer = pawn;

        if (wearer != null && wearer.Spawned)
        {
            ApplyComfortBonus();
            // SlippersUtility.DebugLog($"[CompComfortEnhancer] {parent.LabelCap} equipped by {pawn.LabelShort}.");
        }
    }

    public void NotifyUnequipped(Pawn pawn)
    {
        if (wearer == null || pawn == null)
        {
            // SlippersUtility.DebugLog($"[CompComfortEnhancer] Skipping NotifyUnequipped for {parent?.LabelCap ?? "unknown item"} as wearer or pawn is null.");
            return;
        }

        wearer = null; // Clear wearer reference
        // SlippersUtility.DebugLog($"[CompComfortEnhancer] {parent.LabelCap} unequipped by {pawn.LabelShort}.");
    }

    public void onEquippedTick(Pawn pawn)
    {
        // Placeholder for normal ticks
    }

    public void onEquippedTickRare(Pawn pawn)
    {
        if (pawn == null || parent == null) return;

        // Apply small comfort bonus during rare ticks
        SlippersUtility.DebugLog($"[CompComfortEnhancer] Rare tick invoked for {parent.LabelCap} worn by {pawn.LabelShort}.");
        
        try
        {
            MindMattersInterface.MindMattersAPI.SatisfyDynamicNeed(pawn, "Comfort", 0.05f, false);
        }
        catch (System.Exception ex)
        {
            MMToolkit.GripeOnce($"[CompComfortEnhancer] Failed to apply rare tick comfort bonus: {ex.Message}");
        }
    }

    private void ApplyComfortBonus()
    {
        if (parent is Apparel apparel && wearer != null)
        {
            float baseComfort = apparel.GetStatValue(StatDefOf.Comfort);
            float newComfort = baseComfort + ComfortFactor;

            // Dynamically adjust comfort
            SlippersUtility.DebugLog($"[CompComfortEnhancer] Adjusting comfort for {wearer.LabelShort}. Base: {baseComfort}, New: {newComfort}");
            
            // Apply comfort bonus through MindMatters interface if available
            try
            {
                MindMattersInterface.MindMattersAPI.SatisfyDynamicNeed(wearer, "Comfort", ComfortFactor, false);
            }
            catch (System.Exception ex)
            {
                MMToolkit.GripeOnce($"[CompComfortEnhancer] Failed to apply comfort bonus through MindMatters: {ex.Message}");
            }
        }
    }
}