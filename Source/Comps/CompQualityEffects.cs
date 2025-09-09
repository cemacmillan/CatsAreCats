using RimWorld;
using Verse;

#nullable disable
namespace SlippersNmSpc
{
    public class CompQualityEffects : ThingComp
    {
        public CompProperties_QualityEffects Props => (CompProperties_QualityEffects)props;

        private QualityCategory Quality
        {
            get
            {
                CompQuality qualityComp = parent.GetComp<CompQuality>();
                return qualityComp?.Quality ?? QualityCategory.Normal; // Default to Normal if CompQuality is absent
            }
        }

        private Pawn Wearer => (parent as Apparel)?.Wearer;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            ApplyQualityEffect();
        }

        private void ApplyQualityEffect()
        {
            if (Wearer != null)
            {
                if (Quality == QualityCategory.Masterwork || Quality == QualityCategory.Legendary)
                {
                    // Apply special effects for high-quality items
                    SlippersUtility.DebugLog($"{Wearer.LabelShort} benefits from high-quality {parent.Label}.");
                }
                else
                {
                    SlippersUtility.DebugLog($"{Wearer.LabelShort} wears {parent.Label} of standard quality.");
                }
            }
        }
    }
}