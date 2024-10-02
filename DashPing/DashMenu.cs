using KitchenDashPing.settings;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenDashPing {

    public class DashMenu<T> : KLMenu<T> {

        private static readonly List<bool> showMarkerValues = new List<bool> { true, false };
        private static readonly List<string> showMarkerLabels = new List<string> { "Show", "Hide" };
        private static readonly List<bool> holdButtonValues = new List<bool> { false, true };
        private static readonly List<string> holdButtonLabels = new List<string> { "Press Only - press once to dash once", "Press or Hold - hold dash to dash repeatedly" };
        private static readonly List<int> flavorValues = new List<int> { (int) DashFlavorType.ORIGINAL, (int) DashFlavorType.OVERCOOKED };
        private static readonly List<string> flavorLabels = new List<string> { "Original", "Overcooked" };
        private static readonly List<string> flavorExtendedInfo = new List<string> {
            "The original dash used in the mod. Works by momentarily increasing the player's speed.",
            "Crylion's addition to make the mod feel more like Overcooked. Works by pushing the player forward."
        };

        public DashMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            Option<int> flavorOption = new Option<int>(flavorValues, (int) DashPreferences.getDashFlavor(), flavorLabels);
            Option<bool> showMarkerOption = new Option<bool>(showMarkerValues, DashPreferences.isShowMarker(), showMarkerLabels);
            Option<bool> holdButtonOption = new Option<bool>(holdButtonValues, DashPreferences.isHoldButton(), holdButtonLabels);

            AddLabel("Dash Flavor");
            AddSelect(flavorOption);
            var flavorInfo = AddInfo(flavorExtendedInfo[(int)DashPreferences.getDashFlavor()]);
            AddInfo("In online multiplayer, 'Original' may be safer, as there's a very rare chance of 'Overcooked' allowing the player to phase through walls.");

            AddLabel("Ping Marker");
            AddSelect(showMarkerOption);
            AddInfo("This setting only works when you are the host, and affects everyone");

            AddLabel("Controls");
            AddSelect(holdButtonOption);
            AddInfo("In online multiplayer, 'Press Only' can make dashing unreliable. Choose 'Press or Hold' when you are not the host");

            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });

            showMarkerOption.OnChanged += delegate (object _, bool value) {
                DashPreferences.setShowMarker(value);
            };

            holdButtonOption.OnChanged += delegate (object _, bool value) {
                DashPreferences.setHoldButton(value);
            };

            flavorOption.OnChanged += delegate (object _, int value) {
                DashPreferences.setDashFlavor((DashFlavorType)value);
                flavorInfo.SetLabel(flavorExtendedInfo[value]);
            };
        }
    }
}
