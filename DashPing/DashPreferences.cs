using Kitchen;

namespace KitchenDashPing {

    public class DashPreferences {

        private static Pref ShowMarkerPref = new Pref(DashSystem.MOD_ID, nameof(ShowMarkerPref));
        private static Pref HoldButtonPref = new Pref(DashSystem.MOD_ID, nameof(HoldButtonPref));

        public static void registerPreferences() {
            Preferences.AddPreference<bool>(new BoolPreference(ShowMarkerPref, true));
            Preferences.AddPreference<bool>(new BoolPreference(HoldButtonPref, true));
            Preferences.Load();
        }

        public static bool isShowMarker() {
            return Preferences.Get<bool>(ShowMarkerPref);
        }

        public static void setShowMarker(bool value) {
            Preferences.Set<bool>(ShowMarkerPref, value);
        }

        public static bool isHoldButton() {
            return Preferences.Get<bool>(HoldButtonPref);
        }

        public static void setHoldButton(bool value) {
            Preferences.Set<bool>(HoldButtonPref, value);
        }
    }
}
