using System.Collections.Generic;
using System.Linq;
using Kitchen;
using KitchenDashPing.settings;
using KitchenMods;

namespace KitchenDashPing {

    public class DashSystemOriginal : AbstractDashSystem, IModSystem {

        private const float INITIAL_SPEED = 3000f;
        private const float DASH_SPEED = 12000f;
        private const float DASH_OVERALL_COOLDOWN = 0.9f;
        private const float DASH_REDUCE_PER_UPDATE = 0.03125f;
        private const float DASH_DURATION = DASH_REDUCE_PER_UPDATE * 10;

        private Dictionary<int, DashStatus> statuses = new Dictionary<int, DashStatus>();

        protected override bool supports(DashFlavorType type) => type == DashFlavorType.ORIGINAL;

        protected override void preDashUpdate() => handleDecreasingCooldowns();

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

        protected override void handleDashPressedForPlayer(PlayerView player) {
            int playerId = player.GetInstanceID();

            if (player.Speed > INITIAL_SPEED) {
                return;
            }

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
        private void returnSpeedToNormal(int playerId) {
            PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                .Where(playerView => playerId == playerView.GetInstanceID()).ToList()
                .ForEach(setSpeedToNormal);
        }

        private void setSpeedToDash(PlayerView player) => player.Speed = DASH_SPEED;

        private void setSpeedToNormal(PlayerView player) => player.Speed = INITIAL_SPEED;
    }
}
