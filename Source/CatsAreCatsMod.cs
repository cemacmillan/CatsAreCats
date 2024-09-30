using HarmonyLib;
using Verse;
using UnityEngine;

namespace DIL_CatsAreCats
{
    public class CatsAreCatsMod : Mod
    {
        public static CatsAreCatsModSettings settings;

        public CatsAreCatsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<CatsAreCatsModSettings>();
            var harmony = new Harmony("cem.catsarecats");
            harmony.PatchAll();
            Log.Message("<color=#00FF7F>[Cats Are Cats But Their Eyes Are Charge-Rifles And Their Claws Are Power Claws And They Regenerate]</color> v1.5.4 dajjaj-bing");
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            new CatsAreCatsModSettingsWindow(settings).DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Cats Are Cats Mod Settings";
        }
    }
}