using HarmonyLib;
using Verse;
using UnityEngine;

namespace DIL_CatsAreCats;

public class CatsAreCatsMod : Mod
{
    // Using a property with a private setter to encapsulate the settings object
    public static CatsAreCatsModSettings Settings { get; private set; }

    public CatsAreCatsMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<CatsAreCatsModSettings>();
        var harmony = new Harmony("cem.catsarecats");
        harmony.PatchAll();
        Log.Message("<color=#00FF7F>[Cats Are Cats But Their Eyes Are Charge-Rifles And Their Claws Are Power Claws And They Regenerate]</color> 1.6");
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        new CatsAreCatsModSettingsWindow(Settings).DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Cats Are Cats Mod Settings";
    }
}