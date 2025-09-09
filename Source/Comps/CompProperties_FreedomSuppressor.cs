using Verse;

namespace SlippersNmSpc;

public class CompProperties_FreedomSuppressor : CompProperties
{
    public float constraintFactor = 0.2f; // Default value
    public int pinchInterval = 60000;
    public float movementSpeedCap = 0.8125f;

    public CompProperties_FreedomSuppressor()
    {
        this.compClass = typeof(CompFreedomSuppressor);
    }
}