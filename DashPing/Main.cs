using Kitchen;
using KitchenMods;
using System.Linq;
using UnityEngine;

namespace KitchenDashPing {

    public class Main : Mod {
        public const string MOD_NAME = "dashping";
        public const string MOD_VERSION = "0.2.0";

        public Main() : base(MOD_NAME) {
            Debug.LogWarning($"{MOD_NAME}: Loaded");
        }
    }

    public class DashSystem : GenericSystemBase, IModSystem {

        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 6000f;
        private const float DASH_DURATION = 0.35f;
        private bool isDashing = false;
        private float dashCooldown;

        protected override void OnUpdate() {
            if (dashCooldown > 0) {
                dashCooldown -= Time.DeltaTime;

                if (isDashing && dashCooldown <= 0) {
                    isDashing = false;
                    returnSpeedToNormal();
                }
            }

            if (isPingDown()) {
                if (isDashNoLongerOnCoolDown()) {
                    isDashing = true;
                    dashCooldown = DASH_DURATION;

                    PlayerView[] players = PlayerInfoManager.FindObjectsOfType<PlayerView>();
                    players.ToList().ForEach(setSpeedToDash);
                }
            }
        }

        private bool isPingDown() => isKeyDown(KeyCode.Joystick1Button1) || isKeyDown(KeyCode.L);

        private bool isKeyDown(KeyCode key) => Input.GetKeyDown(key);

        private bool isDashNoLongerOnCoolDown() => dashCooldown <= 0;

        private void returnSpeedToNormal() {
            PlayerView[] players = PlayerInfoManager.FindObjectsOfType<PlayerView>();
            players.ToList().ForEach(setSpeedToNormal);
        }

        private void setSpeedToDash(PlayerView player) => player.Speed = DASH_SPEED;

        private void setSpeedToNormal(PlayerView player) => player.Speed = INITIAL_SPEED;
    }
}
