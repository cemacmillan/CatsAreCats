using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace DIL_CatsAreCats
{
    public class CompAbilityEffect_CatLeap : CompAbilityEffect
    {
        private new CompProperties_AbilityCatLeap Props => (CompProperties_AbilityCatLeap)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Log.Message($"CatsAreCats: {parent.pawn.Name?.ToStringSafe() ?? "Unnamed Cat"} has completed a leap.");
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return base.Valid(target, throwMessages);
        }

      
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
           
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            Command_Ability command_Action = new Command_Ability(parent, parent.pawn)
            {
                defaultLabel = "Cat Leap",
                defaultDesc = "Cats leap to a distant location with incredible precision and power.",
                icon = ContentFinder<Texture2D>.Get("UI/Commands/CatLeap"),
                
            };

           
            if (parent.GizmoDisabled(out string reason))
            {
                command_Action.Disable(reason);
            }

            yield return command_Action;
        }
    }
}