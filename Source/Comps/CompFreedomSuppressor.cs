using System.Collections.Generic;
using RimWorld;
using Verse;
using MindMattersInterface;
using System.Linq;

namespace SlippersNmSpc;

// I don't like being told about typos in identifiers when they aren't typos
[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "IdentifierTypo")]
public class CompFreedomSuppressor : ThingComp, IDynamicComp
{
    public CompProperties_FreedomSuppressor Props => (CompProperties_FreedomSuppressor)props;

    private Pawn wearer;
    private int pinchInterval;
    private float constraintFactor;
    
    private DynAppCompModExtension cachedModExtension;
    private List<DynAppCompModExtension.NeedSatisfier> cachedNeedSatisfiers;
    private int updateCounter = 0; // To align with NeedInterval
    private const int UpdateFrequency = 150; // Adjust as needed
   
    public int PinchInterval
    {
        get => pinchInterval;
        set => pinchInterval = value;
    }

    public float ConstraintFactor
    {
        get => constraintFactor;
        set => constraintFactor = value;
    }
    
    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        
        cachedModExtension = parent.def.GetModExtension<DynAppCompModExtension>();
        cachedNeedSatisfiers = cachedModExtension?.needSatisfiers?.ToList();

        if (props is CompProperties_FreedomSuppressor constraintProps)
        {
            PinchInterval = constraintProps.pinchInterval;
            ConstraintFactor = constraintProps.constraintFactor;
        }
        else
        {
            MMToolkit.GripeOnce($"[CompFreedomSuppressor] Missing or invalid properties for {parent.LabelCap}.");
        }
    }

    public void NotifyEquipped(Pawn pawn)
    {
        wearer = pawn;
        cachedModExtension = parent.def.GetModExtension<DynAppCompModExtension>();
        cachedNeedSatisfiers = cachedModExtension?.needSatisfiers?.ToList();

        if (wearer != null && wearer.Spawned && cachedNeedSatisfiers != null)
        {
            foreach (DynAppCompModExtension.NeedSatisfier needSatisfier in cachedNeedSatisfiers)
            {
                DynamicNeeds.UpdateBaseline(wearer, needSatisfier.needDefName, needSatisfier.satisfactionBaseContrib, isAdding: true);
            }
        }
    }

    public void NotifyUnequipped(Pawn pawn)
    {
        if (pawn == null || wearer != pawn)
        {
            wearer = null;
            return;
        }

        if (cachedNeedSatisfiers != null)
        {
            foreach (DynAppCompModExtension.NeedSatisfier needSatisfier in cachedNeedSatisfiers)
            {
                DynamicNeeds.UpdateBaseline(pawn, needSatisfier.needDefName, needSatisfier.satisfactionBaseContrib, isAdding: false);
            }
        }

        wearer = null;
        cachedModExtension = null;
        cachedNeedSatisfiers = null;
    }

    public void onEquippedTick(Pawn pawn)
    {
        if (pawn == null || pawn.Dead || cachedNeedSatisfiers == null) return;

        updateCounter++;
        if (updateCounter < UpdateFrequency) return; // Skip until the update frequency matches
        updateCounter = 0;

        foreach (DynAppCompModExtension.NeedSatisfier needSatisfier in cachedNeedSatisfiers)
        {
            DynamicNeeds.SatisfyNeedOnTick(pawn, needSatisfier.needDefName, needSatisfier.satisfactionPerTick);
        }
    }

    public void onEquippedTickRare(Pawn pawn)
    {
        if (pawn == null || pawn.Dead) return;

        // Check and apply pinch effects
        TryApplyPinchEffect(pawn);
    }

    private void SatisfyConstraintNeed(Pawn pawn)
    {
        if (pawn == null || pawn.Dead) return;

        float qualityFactor = QualityFactor();
        float satisfactionIncrease = ConstraintFactor * qualityFactor;

        // Apply satisfaction for ConstraintNeed
        DynamicNeeds.SatisfyNeed(pawn, "ConstraintNeed", satisfactionIncrease);

        // Optionally apply satisfaction to FormalityNeed
        if (!pawn.Downed && !pawn.InMentalState)
        {
            DynamicNeeds.SatisfyNeed(pawn, "FormalityNeed", satisfactionIncrease * 0.5f);
        }
    }

    private void TryApplyPinchEffect(Pawn pawn)
    {
        if (pawn == null || pawn.health?.hediffSet == null) return;

        // Avoid pinch if pain level is already critical
        if (pawn.health.hediffSet.PainTotal > pawn.GetStatValue(StatDefOf.PainShockThreshold)) return;

        float qualityFactor = QualityFactor();
        float probability = SlippersMod.Settings.FaintingBanality/ qualityFactor; // Higher quality reduces chance

        if (Rand.Chance(probability))
        {
            ApplyPinchEffect(pawn, qualityFactor);
        }
    }

    private void ApplyPinchEffect(Pawn pawn, float qualityFactor)
    {
        if (pawn == null) return;

        Hediff pinchHediff;
        string eventType;
        HashSet<string> tags;

        if (Rand.Range(0f, 1f) < qualityFactor / 2)
        {
            pinchHediff = HediffMaker.MakeHediff(SlippersNmSpc.DefOfs.LittleFaint, pawn);
            eventType = "MinorFaint";
            tags = ["ConstrainingApparel", "MinorDiscomfort"];
        }
        else
        {
            pinchHediff = HediffMaker.MakeHediff(SlippersNmSpc.DefOfs.SevereFaint, pawn);
            eventType = "MajorFaint";
            tags = ["ConstrainingApparel", "SevereDiscomfort"];

            DynamicNeeds.SatisfyNeed(pawn, "ConstraintNeed", 0f, satisfyToMax: true);
        }

        if (pinchHediff != null)
        {
            pawn.health.AddHediff(pinchHediff);
            pinchHediff.Severity += ConstraintFactor * (1 / qualityFactor);

            DynamicNeeds.NotifyExperience(pawn, eventType, ExperienceValency.Negative, tags);
        }
    }

    private float QualityFactor()
    {
        CompQuality compQuality = parent.TryGetComp<CompQuality>();
        return compQuality?.Quality switch
        {
            QualityCategory.Awful => 0.5f,
            QualityCategory.Poor => 0.75f,
            QualityCategory.Normal => 1.0f,
            QualityCategory.Good => 1.25f,
            QualityCategory.Excellent => 1.5f,
            QualityCategory.Masterwork or QualityCategory.Legendary => 2.0f,
            _ => 1.0f
        };
    }
}