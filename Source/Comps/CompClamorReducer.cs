using RimWorld;
using Verse;
using System;

namespace SlippersNmSpc;

public class CompClamorReducer : ThingComp, IDynamicComp
{
    public CompProperties_ClamorReducer Props => (CompProperties_ClamorReducer)props;

    private Pawn wearer;
    private float clamorReductionFactor;

    public float ClamorReductionFactor
    {
        get => clamorReductionFactor;
        set => clamorReductionFactor = value;
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);

        props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
        if (props is CompProperties_ClamorReducer clamorProps)
        {
            ClamorReductionFactor = clamorProps.clamorReductionFactor;
            SlippersUtility.DebugLog($"[ClamorReducer] {this.GetType().Name} initialized for {parent.LabelCap} with ClamorReductionFactor: {ClamorReductionFactor}.");
        }
        else
        {
            MMToolkit.GripeOnce($"[CompClamorReducer::Initialize] {this.GetType().Name}: Missing or invalid properties during initialization for {parent.LabelCap}.");
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        if (respawningAfterLoad)
        {
            Initialize(props); // Reinitialize properties if needed
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();

        // Reload properties from registry
        props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
        if (props is CompProperties_ClamorReducer clamorProps)
        {
            ClamorReductionFactor = clamorProps.clamorReductionFactor;
            MMToolkit.DebugLog($"[ClamorReducer] PostExposeData: Reloaded for {parent.LabelCap} with ClamorReductionFactor: {ClamorReductionFactor}.");
        }
        else
        {
            MMToolkit.GripeOnce($"[CompClamorReducer] PostExposeData: Missing CompProperties for {parent.LabelCap} during PostExposeData.");
        }
    }

    public void NotifyEquipped(Pawn pawn)
    {
        wearer = pawn;

        if (wearer != null && wearer.Spawned)
        {
            SlippersUtility.DebugLog($"[CompClamorReducer] {parent.LabelCap} equipped by {pawn.LabelShort}.");
        }
    }

    public void NotifyUnequipped(Pawn pawn)
    {
        if (wearer == null || pawn == null)
        {
            return;
        }

        wearer = null; // Clear wearer reference
        SlippersUtility.DebugLog($"[CompClamorReducer] {parent.LabelCap} unequipped by {pawn.LabelShort}.");
    }

    public void onEquippedTick(Pawn pawn)
    {
        // Not used for this comp
    }

    public void onEquippedTickRare(Pawn pawn)
    {
        // Not used for this comp
    }

    public float GetClamorReductionChance()
    {
        var compQuality = parent.TryGetComp<CompQuality>();
        float baseChance = ClamorReductionFactor;
        
        // Quality affects clamor reduction effectiveness
        float qualityMultiplier = compQuality?.Quality switch
        {
            QualityCategory.Awful => 0.5f,
            QualityCategory.Poor => 0.7f,
            QualityCategory.Normal => 1.0f,
            QualityCategory.Good => 1.3f,
            QualityCategory.Excellent => 1.6f,
            QualityCategory.Masterwork => 2.0f,
            QualityCategory.Legendary => 2.5f,
            _ => 1.0f
        };

        return Math.Min(baseChance * qualityMultiplier, 0.95f); // Cap at 95% reduction
    }
}
