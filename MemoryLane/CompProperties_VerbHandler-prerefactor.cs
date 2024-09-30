using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace DIL_CatsAreCats
{
    public class CompProperties_VerbHandler : CompProperties
    {
        public float rangedProbability = 0.5f; 
        public CompProperties_VerbHandler()
        {
            this.compClass = typeof(CompVerbHandler);
        }
    }

    public class CompVerbHandler : ThingComp
    {
        private const int RegenerationTickInterval = 5000;
        private int lastRegenerationTick = 0;

        public CompProperties_VerbHandler Props => (CompProperties_VerbHandler)this.props;

        public override void CompTick()
        {
            base.CompTick();

            if (this.parent is Pawn pawn)
            {
               // Various tests - we've implemented our own because we are weird
                if (IsFlying(pawn))
                {
                    return;
                }

                if (!IsCasting(pawn))
                {
                    HandleCombat(pawn);
                }

                if (Find.TickManager.TicksGame - lastRegenerationTick >= RegenerationTickInterval)
                {
                    CatKindHealthRegen.RegenerateHealth(pawn);
                    lastRegenerationTick = Find.TickManager.TicksGame;
                }
            }
        }

       private bool IsFlying(Pawn pawn)
       {
           return pawn.holdingOwner?.Owner is PawnFlyer;
       }

        private bool IsCasting(Pawn pawn)
        {
            
            return pawn.stances.curStance is Stance_Warmup;
        }

        private void HandleCombat(Pawn pawn)
        {
            if (pawn.stances.curStance is Stance_Busy)
            {
                return;
            }

            Pawn target = FindValidTarget(pawn);
            if (target == null)
            {
                return;
            }

            bool useRanged = CatsAreCatsMod.settings.ChargeRifleEyes == true ? Rand.Chance(Props.rangedProbability) : false;

            if (useRanged)
            {
                var rangedVerb = GetRangedVerb(pawn);
                if (rangedVerb != null && rangedVerb.Available())
                {
                    rangedVerb.TryStartCastOn(target);
                }
            }
            else
            {
                var meleeVerb = pawn.VerbTracker.PrimaryVerb;
                if (meleeVerb != null && meleeVerb.Available())
                {
                    Job meleeJob = JobMaker.MakeJob(JobDefOf.AttackMelee, target);
                    pawn.jobs.StartJob(meleeJob, JobCondition.InterruptForced);
                }
            }
        }

        private Pawn FindValidTarget(Pawn pawn)
        {
            Pawn bestTarget = null;
            float bestScore = float.MaxValue;

            foreach (Pawn potentialTarget in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (potentialTarget.HostileTo(pawn.Faction) && !potentialTarget.Downed)
                {
                    float score = (pawn.Position - potentialTarget.Position).LengthHorizontalSquared;
                    if (score < bestScore)
                    {
                        bestTarget = potentialTarget;
                        bestScore = score;
                    }
                }
            }

            return bestTarget;
        }

        private Verb GetRangedVerb(Pawn pawn)
        {
            List<Verb> verbs = pawn.VerbTracker.AllVerbs;

            foreach (Verb verb in verbs)
            {
                if (verb.verbProps.isPrimary && verb is Verb_Shoot)
                {
                    return verb;
                }
            }

            return null;
        }
    }
}