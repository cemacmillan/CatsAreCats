using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace DIL_CatsAreCats
{
    public class CompJumpAbility : ThingComp
    {
        private int lastUsedTick = -9999; // Stores the last time the ability was used (for cooldown)

        public CompProperties_JumpAbility Props => (CompProperties_JumpAbility)this.props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (this.parent.Faction == Faction.OfPlayer)
            {
                yield return new Command_Action
                {
                    action = () => StartJump(),
                    defaultLabel = "Cat Leap",
                    defaultDesc = $"Leap up to {Props.jumpRange} cells. Cooldown: {Props.cooldownTicks / 60} seconds.",
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/CatLeap", true),
                    Disabled = !CanJump(),
                    disabledReason = !CanJump() ? "Cooldown not finished" : null
                };
            }
        }

        private bool CanJump()
        {
            return Find.TickManager.TicksGame >= lastUsedTick + Props.cooldownTicks;
        }

        public void StartJump()
        {
            Pawn catPawn = this.parent as Pawn;

            if (catPawn == null)
            {
                Log.Error("CatsAreCats: catPawn is null in StartJump.");
                return;
            }

            if (CatsAreCatsMod.settings.EnableLogging)
            {
                Log.Message($"CatsAreCats: {catPawn.Name?.ToStringSafe() ?? "Unnamed Cat"} is attempting a jump.");
            }

            if (catPawn.abilities != null)
            {
                Ability jumpAbility = catPawn.abilities.GetAbility(DefDatabase<AbilityDef>.GetNamed("CatLeap"));

                if (jumpAbility != null)
                {
                    // Create a new job for the jump
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("CastJump"), catPawn.Position);
                    job.ability = jumpAbility; // Assign the ability to the job

                    // Assign the job to the pawn
                    catPawn.jobs.TryTakeOrderedJob(job);

                    // Start cooldown and mark the ability as cast
                    jumpAbility.StartCooldown(Props.cooldownTicks);
                    jumpAbility.Notify_StartedCasting();
                    lastUsedTick = Find.TickManager.TicksGame;
                }
            }
        }
    }
}