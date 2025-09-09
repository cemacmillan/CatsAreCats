using UnityEngine;
using Verse;

namespace DIL_CatsAreCats
{
    public class SlippersModSettingsWindow : Window
    {
        private Vector2 scrollPosition;
        private CatsAreCatsModSettings _modSettings;

        public SlippersModSettingsWindow(CatsAreCatsModSettings settings)
        {
            doCloseX = true;
            forcePause = false;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;

            _modSettings = settings;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect contentRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 30f);

            Widgets.BeginScrollView(contentRect, ref scrollPosition, contentRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);

            // Checkbox for enabling logging
            listing.CheckboxLabeled("Enable Logging", ref _modSettings.EnableLogging, "DIL_CatsAreCatsModSettings.EnableLogging");

            // Checkbox for enabling Charge Rifle Eyes
            listing.CheckboxLabeled("Enable Charge Rifle Eyes", ref _modSettings.ChargeRifleEyes);

            // Damage slider with custom labels
            listing.Label("Claw Damage:");
            Rect sliderRect = listing.GetRect(22f);
            _modSettings.ClawDamage = Widgets.HorizontalSlider(sliderRect, _modSettings.ClawDamage, 0f, 1f, true, 
                $"{_modSettings.ClawDamage:0.00}", "Deadly", "Antigrain IED", 0.01f);

            listing.End();
            Widgets.EndScrollView();

            // Save button
            if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - 25f, inRect.width, 25f), "Save"))
            {
                _modSettings.Write();
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
            _modSettings.Write();
        }
    }
}