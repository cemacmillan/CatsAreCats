using Verse;

namespace DIL_CatsAreCats;

public class CatsAreCatsModSettings : ModSettings
{
    // Private backing fields for properties
    public bool EnableLogging = false;
    public bool ChargeRifleEyes = true;
    public float ClawDamage = 0.5f;


    public override void ExposeData()
    {
        base.ExposeData();
        // Using the backing fields for Scribe_Values.Look
        Scribe_Values.Look(ref EnableLogging, "EnableLogging", false);
        Scribe_Values.Look(ref ChargeRifleEyes, "ChargeRifleEyes", true);
        Scribe_Values.Look(ref ClawDamage, "ClawDamage", 0.5f);
    }
}