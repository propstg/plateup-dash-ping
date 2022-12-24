using Kitchen;
using KitchenLib;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Controllers;

namespace KitchenDashPing {

    public class DashSystem : BaseMod {

        public const string MOD_ID = "blargle.DashPing";
        public const string MOD_NAME = "Dash Ping";
        public const string MOD_VERSION = "0.1.6";

        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 6000f;
        private const float DASH_OVERALL_COOLDOWN = 0.75f;
        private const float DASH_DURATION = 0.35f;
        private const float DASH_REDUCE_PER_UPDATE = 0.03125f;

        private Dictionary<int, DashStatus> statuses = new Dictionary<int, DashStatus>();

        public DashSystem() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise() {
            base.Initialise();
            Debug.Log($"{MOD_ID} v{MOD_VERSION}: initialized");
        }

        protected override void OnUpdate() {
            handleDecreasingCooldowns();

            PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                .Where(isPingDownForPlayer).ToList()
                .ForEach(handleDashPressedForPlayer);
        }

        private void handleDecreasingCooldowns() {
            foreach (KeyValuePair<int, DashStatus> entry in statuses) {
                DashStatus status = entry.Value;

                if (status.DashCooldown > 0) {
                    status.DashCooldown -= DASH_REDUCE_PER_UPDATE;
                }

                if (status.IsDashing && status.DashCooldown <= DASH_OVERALL_COOLDOWN - DASH_DURATION) {
                    status.IsDashing = false;
                    returnSpeedToNormal(entry.Key);
                }
            }
        }

        private void handleDashPressedForPlayer(PlayerView player) {
            int playerId = player.GetInstanceID();

            if (statuses.TryGetValue(playerId, out DashStatus status)) {
                if (status.DashCooldown <= 0) {
                    status.IsDashing = true;
                    status.DashCooldown = DASH_OVERALL_COOLDOWN;
                    setSpeedToDash(player);
                }
            } else {
                DashStatus newStatus = new DashStatus {
                    IsDashing = true,
                    DashCooldown = DASH_OVERALL_COOLDOWN
                };
                statuses.Add(playerId, newStatus);
                setSpeedToDash(player);
            }
        }

        private bool isPingDownForPlayer(PlayerView player) {
            FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Data");
            PlayerView.ViewData viewData = (PlayerView.ViewData)fieldInfo.GetValue(player);
            ButtonState buttonState = viewData.Inputs.State.SecondaryAction2;

            return buttonState == ButtonState.Pressed ||
                buttonState == ButtonState.Held ||
                buttonState == ButtonState.Released ||
                buttonState == ButtonState.Consumed;
        }

        private void returnSpeedToNormal(int playerId) {
            List<PlayerView> lists = PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                .Where(playerView => playerId == playerView.GetInstanceID()).ToList();
            lists.ForEach(setSpeedToNormal);
        }

        private void setSpeedToDash(PlayerView player) => player.Speed = DASH_SPEED;

        private void setSpeedToNormal(PlayerView player) => player.Speed = INITIAL_SPEED;
    }

    class DashStatus {
        public bool IsDashing;
        public float DashCooldown;
    }
}
