using BepInEx;
using Kitchen;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace DashPing {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.1.0")]
    public class Mod : BaseUnityPlugin {

        private const string MOD_ID = "dashping";
        private const string MOD_NAME = "Dash Ping";
        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 6000f;
        private const float DASH_DURATION = 0.35f;

        public void Update() {
            if (isPingDown()) {
                PlayerView[] players = PlayerInfoManager.FindObjectsOfType<PlayerView>();
                players.ToList().ForEach(setSpeedToDash);
                StartCoroutine(returnSpeedToNormal(players));
            }
        }

        private bool isPingDown() => isKeyDown(KeyCode.Joystick1Button1) || isKeyDown(KeyCode.L);

        private bool isKeyDown(KeyCode key) => UnityInput.Current.GetKeyDown(key);

        private IEnumerator returnSpeedToNormal(PlayerView[] players) {
            yield return new WaitForSeconds(DASH_DURATION);
            players.ToList().ForEach(setSpeedToNormal);
        }

        private void setSpeedToDash(PlayerView player) => player.Speed = DASH_SPEED;

        private void setSpeedToNormal(PlayerView player) => player.Speed = INITIAL_SPEED;
    }
}
