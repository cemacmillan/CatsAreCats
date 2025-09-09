using RimWorld;
using Verse;

namespace SlippersNmSpc
{
    public class CompPainCauser : ThingComp, IDynamicComp
    {
        public CompProperties_PainCauser Props => (CompProperties_PainCauser)props;

        private Pawn wearer;
        private int stabInterval;
        private float painFactor;

        public int StabInterval
        {
            get => stabInterval;
            set => stabInterval = value;
        }

        public float PainFactor
        {
            get => painFactor;
            set => painFactor = value;
        }
        public override void Initialize(CompProperties props)
        {
            SlippersUtility.DebugLog($"[CompPainCauser] {this.GetType().Name} on {parent.LabelCap} enters Initialize.");
        
            
            base.Initialize(props);

            props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
            if (props is CompProperties_PainCauser painProps)
            {
                StabInterval = painProps.stabInterval;
                PainFactor = painProps.painFactor;
                SlippersUtility.DebugLog($"[CompPainCauser] {this.GetType().Name} initialized for for {parent.LabelCap} with StabInterval: {StabInterval} and PainFactor: {PainFactor}.");
            }
            else
            {
                Log.Warning($"[CompPainCauser] {this.GetType().Name}: Missing or invalid properties during initialization for {parent.LabelCap}.");
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
                SlippersUtility.DebugLog($"[CompPainCauser] PostSpawnSetup after load for {parent.LabelCap}.");
            }
            else
            {
                Initialize(props);
                SlippersUtility.DebugLog($"[CompPainCauser] ConditionB {parent.LabelCap}.");
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
            if (props is CompProperties_PainCauser painProps)
            {
                StabInterval = painProps.stabInterval;
                PainFactor = painProps.painFactor;
                SlippersUtility.DebugLog($"[CompPainCauser] PostExposeData: Reloaded for {parent.LabelCap} with StabInterval: {StabInterval}.");
            }
            else
            {
                Log.Error($"[CompPainCauser] Missing CompProperties for {parent.LabelCap} during PostExposeData. This is the serious case.");
            }
        }

        public void NotifyEquipped(Pawn pawn)
        {
            if(pawn == null)
                SlippersUtility.DebugLog("[CompPainCauser]NotifyEquipped - pawn is null. This serious.");
            wearer = pawn;

            if (wearer != null && wearer.Spawned)
            {
                // should trigger counter here ApplyComfortBonus();
                SlippersUtility.DebugLog($"[CompPainCauser] {parent.LabelCap} equipped by {pawn.LabelShort}.");
            }
        }

        public void NotifyUnequipped(Pawn pawn)
        {
            if (wearer == null || pawn == null)
            {
                SlippersUtility.DebugLog($"[PainCauser] Skipping NotifyUnequipped for {parent?.LabelCap ?? "unknown item"} as wearer or pawn is null.");
                return;
            }

            wearer = null; // Clear wearer reference
            SlippersUtility.DebugLog($"[PainCauserer] {parent.LabelCap} unequipped by {pawn.LabelShort}.");
        }
        
        public void onEquippedTick(Pawn pawn)
        {
            // Placeholder for normal ticks
        }

        public void onEquippedTickRare(Pawn pawn)
        {
            //if (pawn == null || parent == null) return;
            if (pawn == null) 
                SlippersUtility.DebugLog("Pawn null at OnEquippedTickRare.");
            if (parent == null)
                SlippersUtility.DebugLog("Parent null at OnEquippedTickRare.");

            SlippersUtility.DebugLog($"[CompPainCauser] Rare tick triggered for {pawn.LabelShort} wearing {parent.LabelCap}.");

            if (Find.TickManager.TicksGame % StabInterval == 0)
            {
                try
                {
                    SlippersUtility.DebugLog($"[CompPainCauser] Triggering pain effect for {pawn.LabelShort} from {parent.LabelCap}.");
                    ApplyPainEffect(pawn);
                }
                catch (System.Exception ex)
                {
                    Log.Error($"[CompPainCauser] Exception in OnEquippedTickRare for {parent.LabelCap}: {ex}");
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
            SlippersUtility.DebugLog($"[CompPainCauser] {pawn.LabelShort} suffers {painIncrease} additional pain from {parent.LabelCap}. Total pain severity: {hediff.Severity}");

            // Check for downing based on total pain
            float totalPain = pawn.health.hediffSet.PainTotal;
            if (totalPain >= pawn.GetStatValue(StatDefOf.PainShockThreshold))
            {
                SlippersUtility.DebugLog($"[CompPainCauser] {pawn.LabelShort} collapses from pain caused by {parent.LabelCap}!");
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