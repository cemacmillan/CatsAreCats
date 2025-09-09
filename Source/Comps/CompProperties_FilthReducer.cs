using Verse;

namespace SlippersNmSpc;

public class CompProperties_FilthReducer : CompProperties
{
    public int cleanInterval = 2500; // Default value

    public CompProperties_FilthReducer()
    {
        compClass = typeof(CompFilthReducer);
    }
}