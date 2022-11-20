using HarmonyLib;
using Kitchen;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace DashPing {

    [HarmonyPatch(typeof(MakePing), "Perform")]
    class MakePing_Patch {

        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 6000f;
        private const float DASH_DURATION = 0.35f;

        public static bool Prefix() {
            PlayerView[] players = PlayerInfoManager.FindObjectsOfType<PlayerView>();
            players.ToList().ForEach(setSpeedToDash);

            if (players.Length > 0) {
                players[0].StartCoroutine(returnSpeedToNormal(players));
            }

            return true;
        }

        private static IEnumerator returnSpeedToNormal(PlayerView[] players) {
            yield return new WaitForSeconds(DASH_DURATION);
            players.ToList().ForEach(setSpeedToNormal);
        }

        private static void setSpeedToDash(PlayerView player) => player.Speed = DASH_SPEED;

        private static void setSpeedToNormal(PlayerView player) => player.Speed = INITIAL_SPEED;
    }
}
