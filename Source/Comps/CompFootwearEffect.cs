using System;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SlippersNmSpc;

public abstract class CompFootwearEffect : ThingComp
{
    public CompProperties_FootwearEffect Props => (CompProperties_FootwearEffect)props;

    public abstract void ApplyEffect();

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        ApplyEffect();
    }
}