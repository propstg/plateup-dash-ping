using Kitchen;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;
using Kitchen.Modules;

namespace KitchenDashPing {

    public class DashMenu<T> : KLMenu<T> {

        private static readonly List<bool> showMarkerValues = new List<bool> { true, false };
        private static readonly List<string> showMarkerLabels = new List<string> { "Show", "Hide" };

        public DashMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            Option<bool> option = new Option<bool>(showMarkerValues, DashPreferences.isShowMarker(), showMarkerLabels);

            AddLabel("Markers");
            AddInfo("Show the ping marker when dashing");
            AddSelect(option);
            AddInfo("This setting only works when you are the host, and affects everyone");
            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });

            option.OnChanged += delegate (object _, bool value) {
                DashPreferences.setShowMarker(value);
            };
        }
    }
}
