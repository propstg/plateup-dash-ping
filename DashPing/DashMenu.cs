using Kitchen;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;
using Kitchen.Modules;

namespace KitchenDashPing {

    public class DashMenu<T> : KLMenu<T> {

        private static readonly List<bool> showMarkerValues = new List<bool> { true, false };
        private static readonly List<string> showMarkerLabels = new List<string> { "Show", "Hide" };

        private static readonly List<bool> holdButtonValues = new List<bool> { false, true };
        private static readonly List<string> holdButtonLabels = new List<string> { "Press Only", "Press or Hold" };

        public DashMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            Option<bool> showMarkerOption = new Option<bool>(showMarkerValues, DashPreferences.isShowMarker(), showMarkerLabels);
            Option<bool> holdButtonOption = new Option<bool>(holdButtonValues, DashPreferences.isHoldButton(), holdButtonLabels);

            AddLabel("Markers");
            AddInfo("Show the ping marker when dashing");
            AddSelect(showMarkerOption);
            AddInfo("This setting only works when you are the host, and affects everyone");
            New<SpacerElement>();

            AddLabel("Controls");
            AddInfo("Hold dash button to dash repeatedly");
            AddSelect(holdButtonOption);
            AddInfo("In online multiplayer, 'Press Only' can make dashing unreliable. Choose 'Press or Hold' when you are not the host");
            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });

            showMarkerOption.OnChanged += delegate (object _, bool value) {
                DashPreferences.setShowMarker(value);
            };

            holdButtonOption.OnChanged += delegate (object _, bool value) {
                DashPreferences.setHoldButton(value);
            };
        }
    }
}
