using Verse;

namespace DIL_CatsAreCats
{
    public class CatsAreCatsModSettings : ModSettings
    {
        public bool EnableLogging = false; // Enable/disable logging
        public bool ChargeRifleEyes = true; // Enable/disable Charge Rifle Eyes
        public float ClawDamage = 0.5f; // Damage slider, default at 0.5 (middle)

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnableLogging, "EnableLogging", false); // Save/Load EnableLogging
            Scribe_Values.Look(ref ChargeRifleEyes, "ChargeRifleEyes", false); // Save/Load ChargeRifleEyes
            Scribe_Values.Look(ref ClawDamage, "ClawDamage", 0.5f); // Save/Load ClawDamage
        }
    }
}