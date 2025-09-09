using Verse;

namespace SlippersNmSpc;

public class CompProperties_ClamorReducer : CompProperties
{
    public float clamorReductionFactor = 0.3f; // Base 30% chance to reduce clamor

    public CompProperties_ClamorReducer()
    {
        compClass = typeof(CompClamorReducer);
    }
}
