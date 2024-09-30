using Verse;
using RimWorld;

namespace DIL_CatsAreCats
{
    public class CompProperties_JumpAbility : CompProperties
    {
        public int jumpRange = 23; // Default jump range
        public int cooldownTicks = 60; // Cooldown in ticks (60 ticks = 1 second in RimWorld)

        public CompProperties_JumpAbility()
        {
            this.compClass = typeof(CompJumpAbility); // Links this property class to the CompJumpAbility class
        }
    }
}