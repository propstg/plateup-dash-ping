using Kitchen;

namespace KitchenDashPing.settings {

    public class DashPreferences {

        private static Pref ShowMarkerPref = new Pref(DashSystem.MOD_ID, nameof(ShowMarkerPref));
        private static Pref HoldButtonPref = new Pref(DashSystem.MOD_ID, nameof(HoldButtonPref));
        private static Pref DashFlavor = new Pref(DashSystem.MOD_ID, nameof(DashFlavor));

        public static void registerPreferences() {
            Preferences.AddPreference(new BoolPreference(ShowMarkerPref, true));
            Preferences.AddPreference(new BoolPreference(HoldButtonPref, true));
            Preferences.AddPreference(new IntPreference(DashFlavor, (int) DashFlavorType.ORIGINAL));
            Preferences.Load();
        }

        public static bool isShowMarker() {
            return Preferences.Get<bool>(ShowMarkerPref);
        }

        public static void setShowMarker(bool value) {
            Preferences.Set(ShowMarkerPref, value);
        }

        public static bool isHoldButton() {
            return Preferences.Get<bool>(HoldButtonPref);
        }

        public static void setHoldButton(bool value) {
            Preferences.Set(HoldButtonPref, value);
        }

        public static DashFlavorType getDashFlavor() {
            return (DashFlavorType) Preferences.Get<int>(DashFlavor);
        }

        public static void setDashFlavor(DashFlavorType value) {
            Preferences.Set(DashFlavor, (int) value);
        }
    }
}
