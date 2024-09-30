using UnityEngine;
using Verse;

namespace DIL_CatsAreCats
{
    public class CatsAreCatsModSettingsWindow : Window
    {
        private Vector2 scrollPosition;
        public CatsAreCatsModSettings modSettings;

        public CatsAreCatsModSettingsWindow(CatsAreCatsModSettings settings)
        {
            doCloseX = true;
            forcePause = false;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;

            modSettings = settings;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect contentRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 30f);

            Widgets.BeginScrollView(contentRect, ref scrollPosition, contentRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);

            // Checkbox for enabling logging
            listing.CheckboxLabeled("Enable Logging", ref modSettings.EnableLogging);

            // Checkbox for enabling Charge Rifle Eyes
            listing.CheckboxLabeled("Enable Charge Rifle Eyes", ref modSettings.ChargeRifleEyes);

            // Damage slider with custom labels
            listing.Label("Claw Damage:");
            Rect sliderRect = listing.GetRect(22f);
            modSettings.ClawDamage = Widgets.HorizontalSlider(sliderRect, modSettings.ClawDamage, 0f, 1f, true, 
                $"{modSettings.ClawDamage:0.00}", "Deadly", "Antigrain IED", 0.01f);

            listing.End();
            Widgets.EndScrollView();

            // Save button
            if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - 25f, inRect.width, 25f), "Save"))
            {
                modSettings.Write();
                Close();
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();

            windowRect = new Rect(0f, 0f, 400f, 300f);
            windowRect.center = new Vector2(UI.screenWidth / 2f, UI.screenHeight / 2f);
        }

        public override void Close(bool doCloseSound = true)
        {
            base.Close(doCloseSound);
            modSettings.Write();
        }
    }
}