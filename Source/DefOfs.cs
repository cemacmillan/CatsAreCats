using RimWorld;
using Verse;

namespace SlippersNmSpc;

[DefOf]
public static class DefOfs
{
    public static HediffDef DebilitatingPain;
    public static HediffDef LittleFaint;
    public static HediffDef SevereFaint;
    public static NeedDef Comfort;
        
    static DefOfs()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DefOfs));
    }
}