using Verse;

namespace SlippersNmSpc;

public class CompProperties_QualityEffects : CompProperties
{
    public int gleamInterval = 2500;
    public bool isMasterpiece = false;
    public CompProperties_QualityEffects()
    {
        compClass = typeof(CompQualityEffects);
    }
}