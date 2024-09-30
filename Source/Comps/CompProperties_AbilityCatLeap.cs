using RimWorld;
using Verse;

namespace DIL_CatsAreCats
{
    public class CompProperties_AbilityCatLeap : CompProperties_AbilityEffect
    {
        public int jumpRange = 23; 

        public CompProperties_AbilityCatLeap()
        {
            compClass = typeof(CompAbilityEffect_CatLeap);
        }
    }
}