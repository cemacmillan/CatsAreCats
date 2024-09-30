using RimWorld;
using Verse;

namespace DIL_CatsAreCats
{
    public class Verb_CastAbilityCatLeap : Verb_CastAbilityJump
    {       
        public override ThingDef JumpFlyerDef => ThingDefOf.PawnFlyer;
    }
}