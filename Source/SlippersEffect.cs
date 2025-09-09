using System.Collections.Generic;
using Verse;

namespace DIL_CatsAreCats;

public class SlippersEffect
{
    private const int HediffsToRemovePerTick = 1; // Number of hediffs to remove per regeneration cycle
    private static List<Hediff> tmpHediffs = new List<Hediff>();
    //private CatsAreCatsModSettings _modSettings;
        
    /*CatKindHealthRegen(CatsAreCatsModSettings settings)
    {
        _modSettings = settings;    
    }*/
       
        
    public static void RegenerateHealth(Pawn pawn)
    {
        // Ensure pawn is not dead
        if (pawn.Dead)
        {
            return;
        }

        tmpHediffs.Clear();
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            // Only add non-permanent injuries to the list
            if (hediff is Hediff_Injury injury && !injury.IsPermanent())
            {
                tmpHediffs.Add(hediff);
            }
        }

        int hediffsRemoved = 0; // Count how many hediffs we remove this cycle

        
        for (int i = 0; i < HediffsToRemovePerTick && tmpHediffs.Count > 0; i++)
        {
            Hediff hediffToRemove = tmpHediffs.RandomElement();
            pawn.health.RemoveHediff(hediffToRemove);
            tmpHediffs.Remove(hediffToRemove);
            hediffsRemoved++;
        }

        tmpHediffs.Clear();

        if (CatsAreCatsMod.Settings.EnableLogging && hediffsRemoved > 0)
        {
            Log.Message($"[Cats Are Cats] {pawn.LabelCap} has recovered from {hediffsRemoved} injuries.");
        }
    }
}