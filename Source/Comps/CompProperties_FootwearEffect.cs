using System;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace SlippersNmSpc;

public class CompProperties_FootwearEffect : CompProperties
{
    public CompProperties_FootwearEffect()
    {
        compClass = typeof(CompFootwearEffect);
    }
}