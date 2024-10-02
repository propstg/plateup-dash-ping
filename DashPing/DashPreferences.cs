using System.Collections.Generic;
using PreferenceSystem;
using PreferenceSystem.Preferences;

namespace KitchenDashPing {

    public class DashPreferences {

        public static PreferenceManager manager;
        public static PreferenceSystemManager preferenceSystemManager;

        private static PreferenceBool ShowMarkerPref = new PreferenceBool("showMarker", true);
        private static PreferenceBool HoldButtonPref = new PreferenceBool("holdToDash", true);

        private static readonly List<bool> showMarkerValues = new List<bool> { true, false };
        private static readonly List<string> showMarkerLabels = new List<string> { "Show", "Hide" };

        private static readonly List<bool> holdButtonValues = new List<bool> { false, true };
        private static readonly List<string> holdButtonLabels = new List<string> { "Press Only", "Press or Hold" };

        public static void registerPreferences() {
            manager = new PreferenceManager(DashSystem.MOD_ID);

            manager.RegisterPreference(ShowMarkerPref);
            manager.RegisterPreference(HoldButtonPref);
            manager.Load();
        }

        public static bool isShowMarker() {
            return manager.GetPreference<PreferenceBool>(ShowMarkerPref.Key).Value;
        }

        public static void setShowMarker(bool value) {
            manager.GetPreference<PreferenceBool>(ShowMarkerPref.Key).Set(value);
            manager.Save();
        }

        public static bool isHoldButton() {
            return manager.GetPreference<PreferenceBool>(HoldButtonPref.Key).Value;
        }

        public static void setHoldButton(bool value) {
            manager.GetPreference<PreferenceBool>(HoldButtonPref.Key).Set(value);
            manager.Save();
        }

        /**
        * Builds and registers a preference menu using PreferenceSystem to add to the pause menu
        */
        public static void registerMenu() {
            preferenceSystemManager = new PreferenceSystemManager(DashSystem.MOD_ID, DashSystem.MOD_NAME);

            preferenceSystemManager.AddLabel("Markers");
            preferenceSystemManager.AddInfo("Show the ping marker when dashing");
            preferenceSystemManager.AddOption<bool>(ShowMarkerPref.Key, isShowMarker(), showMarkerValues.ToArray(), showMarkerLabels.ToArray(), delegate (bool value) {
                setShowMarker(value);
            });
            preferenceSystemManager.AddInfo("This setting only works when you are the host, and affects everyone");

            preferenceSystemManager.AddLabel("Controls");
            preferenceSystemManager.AddInfo("Hold dash button to dash repeatedly");
            preferenceSystemManager.AddOption<bool>(HoldButtonPref.Key, isHoldButton(), holdButtonValues.ToArray(), holdButtonLabels.ToArray(), delegate (bool value) {
                setHoldButton(value);
            });
            preferenceSystemManager.AddInfo("In online multiplayer, 'Press Only' can make dashing unreliable. Choose 'Press or Hold' when you are not the host");

            preferenceSystemManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }
    }
}
