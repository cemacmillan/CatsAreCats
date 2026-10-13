using RimWorld;
using Verse;

namespace DIL_CatsAreCats
{
    public class CompAbilityEffect_CatLeap : CompAbilityEffect
    {
        private new CompProperties_AbilityCatLeap Props => (CompProperties_AbilityCatLeap)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (CatsAreCatsMod.Settings.EnableLogging)
            {
                Log.Message($"CatsAreCats: {parent.pawn.Name?.ToStringSafe() ?? "Unnamed Cat"} has completed a leap.");
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return base.Valid(target, throwMessages);
        }
    }
}