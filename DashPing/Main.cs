using Kitchen;
using KitchenLib;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KitchenDashPing {

    public class DashSystem : BaseMod {

        public const string MOD_ID = "blargle.DashPing";
        public const string MOD_NAME = "Dash Ping";
        public const string MOD_VERSION = "0.1.5";

        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 6000f;
        private const float DASH_DURATION = 0.35f;
        private const float DASH_REDUCE_PER_UPDATE = 0.03125f;

        private bool isDashing = false;
        private float dashCooldown;

        public DashSystem() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise() {
            base.Initialise();
            Debug.Log($"{MOD_ID} v{MOD_VERSION}: initialized");
        }

        protected override void OnUpdate() {
            if (dashCooldown > 0) {
                dashCooldown -= DASH_REDUCE_PER_UPDATE;

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
