using Verse;

namespace SlippersNmSpc;

public interface IDynamicComp
{
    void Initialize(CompProperties props);
    //void CompTick();
    //void CompTickRare(); // Add this if needed for rare ticks
    void onEquippedTick(Pawn pawn);
    void onEquippedTickRare(Pawn pawn);
    void PostExposeData();
    void PostSpawnSetup(bool respawningAfterLoad);
    string CompInspectStringExtra();

    void NotifyEquipped(Pawn pawn);   // Notify when equipped
    void NotifyUnequipped(Pawn pawn); // Notify when unequipped
}