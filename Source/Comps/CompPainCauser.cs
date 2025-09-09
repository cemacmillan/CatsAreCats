using RimWorld;
using Verse;

namespace SlippersNmSpc
{
    public class CompPainCauser : ThingComp, IDynamicComp
    {
        public CompProperties_PainCauser Props => (CompProperties_PainCauser)props;

        private Pawn wearer;

        // ReSharper disable once MemberCanBePrivate.Global
        public int StabInterval { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public float PainFactor { get; set; }

        public override void Initialize(CompProperties props)
        {
           // MMToolkit.DebugLog($"[CompPainCauser] {this.GetType().Name} on {parent.LabelCap} enters Initialize.");
        
            
            base.Initialize(props);

            props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
            if (props is CompProperties_PainCauser painProps)
            {
                StabInterval = painProps.stabInterval;
                PainFactor = painProps.painFactor;
                // MMToolkit.DebugLog($"[CompPainCauser] {this.GetType().Name} initialized for for {parent.LabelCap} with StabInterval: {StabInterval} and PainFactor: {PainFactor}.");
            }
            else
            {
                MMToolkit.GripeOnce($"[CompPainCauser] {this.GetType().Name}: Missing or invalid properties during initialization for {parent.LabelCap}.");
            }
        }

        /*
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            /* base.PostSpawnSetup(respawningAfterLoad);

            props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
            if (props is CompProperties_PainCauser painProps)
            {
                StabInterval = painProps.stabInterval;
                SlippersUtility.DebugLog($"[CompPainCauser] PostSpawnSetup after load for {parent.LabelCap} with StabInterval: {StabInterval}.");
            }
        }*/
            
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            SlippersUtility.DebugLog($"[CompPainCauser] {this.GetType().Name} on {parent.LabelCap} enters PostSpawnSetup.");
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad)
            {
                Initialize(props); // Reinitialize properties if needed
                //MMToolkit.DebugLog($"[CompPainCauser] PostSpawnSetup after load for {parent.LabelCap}.");
            }
            else
            {
                Initialize(props);
                // MMToolkit.DebugLog($"[CompPainCauser] ConditionB {parent.LabelCap}.");
            }
        }
        
        // We just mean, apply pain, but since it's a Need
        private void SatisfyPainNeed(Pawn pawn)
        {
            if (pawn == null || pawn.Dead || pawn.Downed) return;

            float baseSatisfaction = 0.5f; // Base multiplier
            float qualityFactor = QualityFactor();
            float satisfactionIncrease = baseSatisfaction * PainFactor * qualityFactor;

            // Notify Mind Matters of ConstraintNeed satisfaction
            MindMattersInterface.MindMattersAPI.SatisfyDynamicNeed(pawn, "PainNeed", satisfactionIncrease);

            // Log debug information
            // MMToolkit.DebugLog($"[CompPainCauser] {pawn.LabelShort} satisfied PainNeed by {satisfactionIncrease} from {parent.LabelCap}.");
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
            if (props is CompProperties_PainCauser painProps)
            {
                StabInterval = painProps.stabInterval;
                PainFactor = painProps.painFactor;
                // MMToolkit.DebugLog($"[CompPainCauser] PostExposeData: Reloaded for {parent.LabelCap} with StabInterval: {StabInterval}.");
            }
            else
            {
                MMToolkit.GripeOnce($"[CompPainCauser] Missing CompProperties for {parent.LabelCap} during PostExposeData. This is the serious case.");
            }
        }

        public void NotifyEquipped(Pawn pawn)
        {
            if(pawn == null)
                MMToolkit.GripeOnce("[CompPainCauser]NotifyEquipped - pawn is null. This serious.");
            wearer = pawn;

            if (wearer != null && wearer.Spawned)
            {
                // should trigger counter here ApplyComfortBonus();
                // MMToolkit.DebugLog($"[CompPainCauser] {parent.LabelCap} equipped by {pawn.LabelShort}.");
            }
        }

        public void NotifyUnequipped(Pawn pawn)
        {
            if (wearer == null || pawn == null)
            {
                //MMToolkit.DebugLog($"[PainCauser] Skipping NotifyUnequipped for {parent?.LabelCap ?? "unknown item"} as wearer or pawn is null.");
                return;
            }

            wearer = null; // Clear wearer reference
            //MMToolkit.DebugLog($"[PainCauserer] {parent.LabelCap} unequipped by {pawn.LabelShort}.");
        }
        
        public void onEquippedTick(Pawn pawn)
        {
            // Placeholder for normal ticks
        }

        public void onEquippedTickRare(Pawn pawn)
        {
            if (pawn == null || parent == null) return;
           
            // MMToolkit.DebugLog($"[CompPainCauser] Rare tick triggered for {pawn.LabelShort} wearing {parent.LabelCap}.");

            if (Find.TickManager.TicksGame % StabInterval == 0)
            {
                try
                {
                    // MMToolkit.DebugLog($"[CompPainCauser] Triggering pain effect for {pawn.LabelShort} from {parent.LabelCap}.");
                    ApplyPainEffect(pawn);
                }
                catch (System.Exception ex)
                {
                    MMToolkit.GripeOnce($"[CompPainCauser] Exception in OnEquippedTickRare for {parent.LabelCap}: {ex}");
                }
              
            }
        }

        private void ApplyPainEffect(Pawn pawn)
        {
            if (pawn.Dead || pawn.Downed) return;

            // Calculate pain increase based on quality
            float qualityFactor = QualityFactor();
            float painIncrease = PainFactor * qualityFactor;

            // Retrieve or add the DebilitatingPain hediff
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("DebilitatingPain"));
            if (hediff == null)
            {
                hediff = HediffMaker.MakeHediff(HediffDef.Named("DebilitatingPain"), pawn);
                pawn.health.AddHediff(hediff);
            }

            // Increase the pain severity
            hediff.Severity += painIncrease;
            // MMToolkit.DebugLog($"[CompPainCauser] {pawn.LabelShort} suffers {painIncrease} additional pain from {parent.LabelCap}. Total pain severity: {hediff.Severity}");

            // Check for downing based on total pain
            float totalPain = pawn.health.hediffSet.PainTotal;
            // Checking against total pain downs universally - let's see if we can spare tougher pawns.
            if (totalPain  * 0.95 >= pawn.GetStatValue(StatDefOf.PainShockThreshold))
            {
                // Player sees pawn getting downed, so no further need for this log entry
                // MMToolkit.DebugLog($"[CompPainCauser] {pawn.LabelShort} collapses from pain caused by {parent.LabelCap}!");
                pawn.stances.stunner.StunFor(300, null, false, true);
            }
        }

        private float QualityFactor()
        {
            var compQuality = parent.TryGetComp<CompQuality>();
            return compQuality?.Quality switch
            {
                QualityCategory.Awful => 0.5f,
                QualityCategory.Poor => 0.75f,
                QualityCategory.Normal => 1.0f,
                QualityCategory.Good => 1.25f,
                QualityCategory.Excellent => 1.5f,
                QualityCategory.Masterwork or QualityCategory.Legendary => 2.0f,
                _ => 1.0f
            };
        }

        public override string CompInspectStringExtra()
        {
            return $"Causes pain every {Props.stabInterval / 250} seconds.";
        }
    }
}