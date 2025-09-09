using Verse;

namespace SlippersNmSpc;

public class CompProperties_PainCauser : CompProperties
{
    public float painFactor = 0.2f; // Base pain
    public int stabInterval = 65000; // Frequency

    public CompProperties_PainCauser()
    {
        this.compClass = typeof(CompPainCauser);
    }
}