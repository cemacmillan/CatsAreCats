namespace SlippersNmSpc;

public class CompHasSpecialInsulation : CompFootwearEffect
{
    public override void ApplyEffect()
    {
        // Implement insulation effect
        SlippersUtility.DebugLog($"{parent.Label} gains special insulation.");
        // Adjust stats accordingly
    }
}

public class CompProperties_SpecialInsulation : CompProperties_FootwearEffect
{
    public float insulationFactor = 1.8f;

    public CompProperties_SpecialInsulation()
    {
        compClass = typeof(CompHasSpecialInsulation);
    }
}