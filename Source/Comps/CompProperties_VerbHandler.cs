using System.Collections.Generic;
using System;
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
        private const int RegenerationTickInterval = 4500;
        private int lastRegenerationTick = 0;

        public CompProperties_VerbHandler Props => (CompProperties_VerbHandler)this.props;

    public override void CompTick()
    {
         base.CompTick();

         if (this.parent is Pawn pawn)
         {
             if (pawn.Dead)
             {
                // see below     
                    return;
             }
             // Regenerate health periodically
             if (Find.TickManager.TicksGame - lastRegenerationTick >= RegenerationTickInterval)
             {
                 CatKindHealthRegen.RegenerateHealth(pawn);
                 lastRegenerationTick = Find.TickManager.TicksGame;
             }

             // Check for death and handle revival
             /* Disabled pending analysis
             if (pawn.Dead)
             {
                 HandleRevival(pawn);
                 return;
             }
             */
             // Here we preserve normal cat behavior: drafting is advisory, not a command.
             if (pawn.Downed || pawn.stances.FullBodyBusy)
             {
                 return;
             }

             /* These would be the tests if we wanted to respect Drafted status
             // Skip if pawn is incapacitated or busy
             if (pawn.Downed || pawn.stances.FullBodyBusy || pawn.Drafted)
             {
                 return;
             }
             */

             // Skip if pawn is flying
             if (IsFlying(pawn))
             {
                 return;
             }

             // Handle combat if not casting
             if (!IsCurrentlyCasting(pawn))
             {
                 HandleCombat(pawn);
             }
         }
    }


       private bool IsFlying(Pawn pawn)
       {
           return pawn.holdingOwner?.Owner is PawnFlyer;
       }

        private bool IsCurrentlyCasting(Pawn pawn)
        {
            
            return pawn.stances.curStance is Stance_Warmup;
        }

        /* our hand crafted method
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
        */

        private void HandleCombat(Pawn pawn)
        {
            if (pawn.stances.FullBodyBusy)
            {
                return;
            }

            IAttackTarget target = FindValidTarget(pawn);
            if (target == null)
            {
                return;
            }

            bool useRanged = CatsAreCatsMod.settings.ChargeRifleEyes && Rand.Chance(Props.rangedProbability);

            if (useRanged)
            {
                Verb rangedVerb = GetRangedVerb(pawn);
                if (rangedVerb != null && rangedVerb.Available())
                {
                    rangedVerb.TryStartCastOn((LocalTargetInfo)target.Thing);
                }
            }
            else
            {
                Job meleeJob = JobMaker.MakeJob(JobDefOf.AttackMelee, (LocalTargetInfo)target.Thing);
                pawn.jobs.StartJob(meleeJob, JobCondition.InterruptForced);
            }
        }

        /* Our insane method:
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
        */

        private IAttackTarget FindValidTarget(Pawn pawn)
        {
            if (pawn == null || pawn.Map == null)
                return null;

            TargetScanFlags flags = TargetScanFlags.NeedThreat | TargetScanFlags.NeedLOSToAll | TargetScanFlags.NeedReachable;

            Predicate<Thing> validator = t =>
            {
                Pawn potentialTarget = t as Pawn;
                if (potentialTarget == null)
                    return false;

                if (potentialTarget.Dead || potentialTarget.Downed)
                    return false;

                if (!potentialTarget.HostileTo(pawn.Faction))
                    return false;

                if (potentialTarget.IsPsychologicallyInvisible())
                    return false;

                return true;
            };

            float maxDist = CatsAreCatsMod.settings.ChargeRifleEyes
                ? GetRangedVerb(pawn)?.verbProps.range ?? 27f
                : 10f; // Default melee range or adjust as needed

            IAttackTarget target = AttackTargetFinder.BestAttackTarget(
                searcher: pawn,
                flags: flags,
                validator: validator,
                minDist: 0f,
                maxDist: maxDist,
                canBashDoors: false,
                canTakeTargetsCloserThanEffectiveMinRange: true,
                onlyRanged: false
            );

            return target;
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