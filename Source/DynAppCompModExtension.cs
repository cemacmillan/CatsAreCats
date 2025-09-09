using System.Collections.Generic;
using Verse;

namespace SlippersNmSpc;

public class DynAppCompModExtension : DefModExtension
{
    // Material properties
    public List<string> materialTags;

    // Need satisfaction logic
    public List<NeedSatisfier> needSatisfiers;

    // Excluded needs
    public List<string> excludedNeeds;

    // Additional dynamic effects
    public List<DynamicEffect> dynamicEffects;

    public class NeedSatisfier : IExposable
    {
        public string needDefName; // The defName of the Need this satisfies
        public float satisfactionPerTick; // Satisfaction contribution per tick
        public float satisfactionBaseContrib; // Base satisfaction contribution
        public List<string> requiredTags; // Tags required for this satisfaction to apply (e.g., "Constraining")

        public void ExposeData()
        {
            Scribe_Values.Look(ref needDefName, "needDefName");
            Scribe_Values.Look(ref satisfactionPerTick, "satisfactionPerTick");
            Scribe_Values.Look(ref satisfactionBaseContrib, "satisfactionBaseContrib");
            Scribe_Collections.Look(ref requiredTags, "requiredTags", LookMode.Value);
        }
    }

    public class DynamicEffect : IExposable
    {
        public string effectType; // e.g., "Hediff", "Thought", "Experience"
        public string defName; // DefName of the Hediff, Thought, or similar
        public float probability; // Chance to apply the effect
        public List<string> tags; // Relevant tags for the effect

        public void ExposeData()
        {
            Scribe_Values.Look(ref effectType, "effectType");
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref probability, "probability");
            Scribe_Collections.Look(ref tags, "tags", LookMode.Value);
        }
    }
}