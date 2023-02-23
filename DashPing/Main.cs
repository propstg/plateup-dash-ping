using Kitchen;
using KitchenLib;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Controllers;
using KitchenLib.Event;

namespace KitchenDashPing {

    public class DashSystem : BaseMod {

        public const string MOD_ID = "blargle.DashPing";
        public const string MOD_NAME = "Dash Ping";
        public const string MOD_VERSION = "0.2.0";
        public const string MOD_AUTHOR = "blargle";

        // shortest possible time between
        private const float DASH_COOLDOWN = 0.45f;
        // amount of time the dash force should be distributed over
        private const float DASH_DURATION = 0.15f;
        // total amount of force the dash should apply
        // calculated to achieve the same distance as previous implementation
        private const float DASH_TOTAL_FORCE = 2160f;

        private Dictionary<int, DashStatus> statuses = new Dictionary<int, DashStatus>();
        private float deltaTimeThisUpdate;

        public DashSystem() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, ">=1.1.4", Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            DashPreferences.registerPreferences();
            initPauseMenu();
        }

        protected override void OnUpdate() {
            deltaTimeThisUpdate = UnityEngine.Time.deltaTime;

            PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                .Where(isPingDownForPlayer).ToList()
                .ForEach(handleDashPressedForPlayer);

            // Also handle the decreasing cooldowns per player view to have access to the rigidbody
            PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                .ForEach(handleDecreasingCooldowns);
        }

        private void handleDecreasingCooldowns(PlayerView player) {
            int playerId = player.GetInstanceID();

            if (statuses.TryGetValue(playerId, out DashStatus status)) {

                if (status.DashCooldown > 0) {
                    if (status.DashCooldown - deltaTimeThisUpdate < 0) {
                        status.DashCooldown = 0;
                    } else {
                        status.DashCooldown -= deltaTimeThisUpdate;
                    }
                }

                if (status.DashCooldown > DASH_COOLDOWN - DASH_DURATION) {
                     dashForward(player);
                }
            }
        }

        private void handleDashPressedForPlayer(PlayerView player) {
            int playerId = player.GetInstanceID();

            if (statuses.TryGetValue(playerId, out DashStatus status)) {
                if (status.DashCooldown <= 0) {
                    status.DashCooldown = DASH_COOLDOWN;
                }
            } else {
                DashStatus newStatus = new DashStatus {
                    DashCooldown = DASH_COOLDOWN
                };
                statuses.Add(playerId, newStatus);
            }
        }

        private bool isPingDownForPlayer(PlayerView player) {
            FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Data");
            PlayerView.ViewData viewData = (PlayerView.ViewData)fieldInfo.GetValue(player);
            ButtonState pingButton = viewData.Inputs.State.SecondaryAction2;
            ButtonState stopMovingButton = viewData.Inputs.State.StopMoving;

            return isPingButtonPressedOrHeld(pingButton) && isStopMovingNotPressedOrHeld(stopMovingButton);
        }

        private bool isPingButtonPressedOrHeld(ButtonState pingButton) => isButtonPressed(pingButton) || isPingButtonHeldIfAllowed(pingButton);

        private bool isPingButtonHeldIfAllowed(ButtonState pingButton) => DashPreferences.isHoldButton() && isButtonHeld(pingButton);

        private bool isStopMovingNotPressedOrHeld(ButtonState stopMovingButton) => !isButtonPressed(stopMovingButton) && !isButtonHeld(stopMovingButton);

        private bool isButtonPressed(ButtonState button) => button == ButtonState.Pressed;

        private bool isButtonHeld(ButtonState button) => button == ButtonState.Held;

        private void dashForward(PlayerView player) {
            FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Rigidbody");
            Rigidbody rigidBody = (Rigidbody)fieldInfo.GetValue(player);

            Vector3 force = player.GetPosition().Forward(DASH_TOTAL_FORCE * (deltaTimeThisUpdate / DASH_DURATION));
            force.y = 0f;
            rigidBody.AddForce(force, ForceMode.Force);
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(DashMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(DashMenu<PauseMenuAction>), new DashMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }
}
