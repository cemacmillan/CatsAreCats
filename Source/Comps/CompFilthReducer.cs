using RimWorld;
using Verse;
using System.Linq;
using MindMattersInterface;

namespace SlippersNmSpc;

public class CompFilthReducer : ThingComp, IDynamicComp
{
    public CompProperties_FilthReducer Props => (CompProperties_FilthReducer)props;

    private Pawn wearer;
    private int cleaningInterval;
    
    public int CleaningInterval { get => cleaningInterval; set => cleaningInterval = value; }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);

        // Dynamically assign properties if not already set
        if (props is CompProperties_FilthReducer cleanProps)
        {
            CleaningInterval = cleanProps.cleanInterval;
            // SlippersUtility.DebugLog($"[Slippers] {this.GetType().Name} initialized for {parent.LabelCap} with CleaningInterval: {CleaningInterval}.");
        }
        /*
         Red Herring - it simply shouldn't do anything when finding a different comp properties
         else
        {
            Log.Warning($"[Slippers] {this.GetType().Name}: Missing or invalid properties during initialization for {parent.LabelCap}.");
        }*/
    }

    public override void CompTick()
    {
        // lippersUtility.DebugLog($"[CompFilthReducer] Functional CompTick method found!");
    }
    
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        if (respawningAfterLoad)
        {
            // SlippersUtility.DebugLog($"[CompFilthReducer] PostSpawnSetup called for {parent.LabelCap} after load.");
            Initialize(props); // Reinitialize properties if needed
        }
        else
        {
            // SlippersUtility.DebugLog($"[CompFilthReducer] PostSpawnSetup called for {parent.LabelCap}. Respawning: {respawningAfterLoad}");
        }
    }
    
    public override void PostExposeData()
    {
        base.PostExposeData();

        // Reload properties from registry
        props = DynamicCompPropertiesRegistry.GetPropertiesFor(this.GetType());
        if (props is CompProperties_FilthReducer cleanProps)
        {
            CleaningInterval = cleanProps.cleanInterval;
            // SlippersUtility.DebugLog($"[CompFilthReducer] PostExposeData: Reloaded for {parent.LabelCap} with CleaningInterval: {CleaningInterval}.");
        }
        else
        {
            MMToolkit.GripeOnce($"[CompFilthReducer] Missing CompProperties for {parent.LabelCap} during PostExposeData.");
        }
    }
    
    public void NotifyEquipped(Pawn pawn)
    {
        wearer = pawn;

        if (wearer != null && wearer.Spawned)
        {
            // SlippersUtility.DebugLog($"FilthReducer {parent.LabelCap} equipped by {pawn.LabelShort}.");
        }
    }

    public void NotifyUnequipped(Pawn pawn)
    {
      
        wearer = (Pawn) null;
        if (pawn != null && pawn.Spawned)
        {
            // SlippersUtility.DebugLog($"FilthReducer {parent.LabelCap} unequipped by {pawn.LabelShort}.");
           
        }
    }
    
 

    public void onEquippedTick(Pawn pawn)
    {
        // we do nothing here
    }
    
    public void onEquippedTickRare(Pawn pawn)
    {
        if(pawn == null) return;
        
        
        // Ensure `parent` is valid
        if (parent == null)
        {
            MMToolkit.GripeOnce($"[Slippers] CompFilthReducer parent is null. Cannot process onEquippedTickRare.");
            return;
        }
        
        if (Find.TickManager.TicksGame % CleaningInterval == 0)
        {
            try
            {
                // not useful log any longer
               // SlippersUtility.DebugLog($"FilthReducer OnEquippedTick called for {parent.LabelCap}, worn by {pawn.LabelShort}.");
                TryCleanFilth(pawn);
            }
            catch (System.Exception ex)
            {
                MMToolkit.GripeOnce($"[Slippers] Exception in OnEquippedTickRare for {parent.LabelCap}: {ex}");
            }
        }
      
        
    }

    private void TryCleanFilth(Pawn pawn)
    {
        if (pawn == null || pawn.Map == null)
        {
            // if the pawn isn't spawned, this is fine - disabling.
            // SlippersUtility.DebugLog($"TryCleanFilth aborted: invalid pawn or map.");
            return;
        }

        if (!ShouldCleanFilth(pawn))
        {
            //SlippersUtility.DebugLog($"TryCleanFilth aborted: conditions not met.");
            return;
        }

        IntVec3 cell = pawn.Position;
        Filth filth = pawn.Map.thingGrid.ThingsListAt(cell).OfType<Filth>().FirstOrDefault();

        if (filth != null)
        {
            filth.ThinFilth();
            //SlippersUtility.DebugLog($"[Slippers] TryCleanFilth(): {pawn.LabelShort}'s footwear reduced one layer of filth at {cell}.");
        }
        else
        {
            //SlippersUtility.DebugLog($"[Slippers] TryCleanFilth(): No filth to clean at {cell}.");
        }
    }

    private static bool ShouldCleanFilth(Pawn pawn)
    {

        if (!pawn.Spawned)
        {
            // catch the majority of log spam about pawns not yet in the game.
            return false;
        }
        
        if (!pawn.Awake())
        {
            // SlippersUtility.DebugLog($"[Slippers] ShouldCleanFilth: Pawn {pawn.LabelShort} is not awake.");
            return false;
        }

        if (pawn.Downed)
        {
            // SlippersUtility.DebugLog($"[Slippers] ShouldCleanFilth: Pawn {pawn.LabelShort} is downed.");
            return false;
        }

        if (pawn.InBed())
        {
            // SlippersUtility.DebugLog($"[Slippers] ShouldCleanFilth: Pawn {pawn.LabelShort} is in bed.");
            return false;
        }

        Room room = pawn.Position.GetRoom(pawn.Map);
        if (room == null)
        {
            // SlippersUtility.DebugLog($"[Slippers] ShouldCleanFilth: No valid room. Assuming outdoors.");
            return false;
        }

        bool isIndoors = !room.PsychologicallyOutdoors;
        // SlippersUtility.DebugLog($"[Slippers] ShouldCleanFilth: Indoors check for {pawn.LabelShort}: {isIndoors}.");
        return isIndoors;
    }
}