using Verse;

namespace SlippersNmSpc;

public class CompProperties_ComfortEnhancer : CompProperties
{
    public float comfortFactor = 0.1f; // Default bonus value

    public CompProperties_ComfortEnhancer()
    {
        compClass = typeof(CompComfortEnhancer);
    }
}
