using Kitchen;

namespace KitchenDashPing {

    public class DashPreferences {

        private static Pref ShowMarkerPref = new Pref(DashSystem.MOD_ID, nameof(ShowMarkerPref));

        public static void registerPreferences() {
            Preferences.AddPreference<bool>(new BoolPreference(ShowMarkerPref, true));
            Preferences.Load();
        }

        public static bool isShowMarker() {
            return Preferences.Get<bool>(ShowMarkerPref);
        }

        public static void setShowMarker(bool value) {
            Preferences.Set<bool>(ShowMarkerPref, value);
        }
    }
}
