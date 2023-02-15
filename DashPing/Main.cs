using Kitchen;
using Kitchen.Components;
using KitchenLib;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Controllers;
using KitchenLib.Event;
using HarmonyLib;

namespace KitchenDashPing {

    public class DashSystem : BaseMod {

        public const string MOD_ID = "blargle.DashPing";
        public const string MOD_NAME = "Dash Ping";
        public const string MOD_VERSION = "0.1.9";
        public const string MOD_AUTHOR = "blargle";

        public DashSystem() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, "1.1.3", Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            DashPreferences.registerPreferences();
            initPauseMenu();
        }

        protected override void OnUpdate() {}

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(DashMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(DashMenu<PauseMenuAction>), new DashMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }

    }

    [HarmonyPatch(typeof(PlayerView), "Update")]
    class PlayerView_Patch
    {

        // shortest possible time between
        private const float DASH_COOLDOWN = 0.45f;
        // amount of time the dash force should be distributed over
        private const float DASH_DURATION = 0.15f;
        // total amount of force the dash should apply
        // calculated to achieve the same distance as previous implementation
        private const float DASH_TOTAL_FORCE = 2160f;
        private static Dictionary<int, DashStatus> statuses = new Dictionary<int, DashStatus>();

        private static void handleDecreasingCooldowns(int playerId, float deltaTime) {
            if (statuses.TryGetValue(playerId, out DashStatus status)) {
                if (status.DashCooldown > 0 && !status.CanDash) {
                    if (status.DashCooldown - deltaTime < 0) {
                        status.DashCooldown = 0;
                    } else {
                        status.DashCooldown -= deltaTime;
                    }
                } else if (!status.CanDash) {
                    status.CanDash = true;
                }
            }

        }

        public static void Postfix (
            ref PlayerView __instance,
            ref Rigidbody ___Rigidbody
        )
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            int playerId = __instance.GetInstanceID();

            FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Data");
            PlayerView.ViewData viewData = (PlayerView.ViewData)fieldInfo.GetValue(__instance);
            ButtonState buttonState = viewData.Inputs.State.SecondaryAction2;

            // create or get status dictionary for this player instance
            if (!statuses.TryGetValue(playerId, out DashStatus status)) {
                    DashStatus newStatus = new DashStatus {
                    DashCooldown = 0f,
                    CanDash = true
                };
                statuses.Add(playerId, newStatus);
            }

            // apply fractions of the total dash force over the amount of time defined in DASH_DURATION to avoid collision issues
            if (status.DashCooldown > (DASH_COOLDOWN - DASH_DURATION)){
                Vector3 force = __instance.GetPosition().Forward(DASH_TOTAL_FORCE * (deltaTime / DASH_DURATION));
                force.y = 0f;
                ___Rigidbody.AddForce(force, ForceMode.Force);
            }

            bool isStopMovingPressed = viewData.Inputs.State.StopMoving == ButtonState.Held || viewData.Inputs.State.StopMoving == ButtonState.Pressed;
            if ((buttonState == ButtonState.Pressed || (DashPreferences.isHoldButton())
                && buttonState == ButtonState.Held)
                && !isStopMovingPressed
                && status.CanDash) {
                // start the cooldown, also signaling that the dash interpolation can begin
                status.CanDash = false;
                status.DashCooldown = DASH_COOLDOWN;
            }

            if (status.CanDash == false) handleDecreasingCooldowns(playerId, deltaTime);

        }
    }

    class DashStatus {
        public bool CanDash = true;
        public float DashCooldown;
    }
}
