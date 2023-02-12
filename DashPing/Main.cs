using Kitchen;
using Kitchen.Components;
using KitchenLib;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.VFX;
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

        [HarmonyPatch(typeof(PlayerView), "Update")]
        class PlayerView_Patch
        {

            private const float DASH_COOLDOWN = 1.9f;
            private const float COOLDOWN_REDUCE_PER_UPDATE = 0.03125f;
            private static Dictionary<int, DashStatus> statuses = new Dictionary<int, DashStatus>();

            private static void handleDecreasingCooldowns(int playerId) {
                if (statuses.TryGetValue(playerId, out DashStatus status)) {
                    if (status.DashCooldown > 0 && !status.CanDash) {
                        Debug.Log($"[{MOD_ID}] v{MOD_VERSION} Cooldown: {status.DashCooldown} to {status.DashCooldown - UnityEngine.Time.deltaTime}");
                        status.DashCooldown -= UnityEngine.Time.deltaTime;
                    } else if (!status.CanDash) {
                        Debug.Log($"[{MOD_ID}] v{MOD_VERSION} Cooldown Over for {playerId}!");
                        status.CanDash = true;
                    }
                }

        }

            public static void Postfix (
                ref PlayerView __instance,
                ref Rigidbody ___Rigidbody,
                ref VisualEffect ___Footsteps,
                ref SoundSource ___FootstepSound,
                ref bool ___FootstepsActive,
                int ___MovementSpeed,
                Animator ___Animator
            )
            {
                int playerId = __instance.GetInstanceID();
                handleDecreasingCooldowns(playerId);

                ___Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Data");

                PlayerView.ViewData viewData = (PlayerView.ViewData)fieldInfo.GetValue(__instance);
                ButtonState buttonState = viewData.Inputs.State.SecondaryAction2;


                if (!statuses.TryGetValue(playerId, out DashStatus status)) {
                     DashStatus newStatus = new DashStatus {
                        DashCooldown = 0f,
                        CanDash = true
                    };
                    statuses.Add(playerId, newStatus);
                }

                bool isStopMovingPressed = viewData.Inputs.State.StopMoving == ButtonState.Held || viewData.Inputs.State.StopMoving == ButtonState.Pressed;

                    if (buttonState == ButtonState.Pressed && !isStopMovingPressed && status.CanDash) {

                        Vector3 force = __instance.GetPosition().Forward(__instance.Speed);
                        force.y = 0f;
                        ___Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                        ___Rigidbody.AddForce(force, ForceMode.Force);
                        status.CanDash = false;
                        status.DashCooldown = DASH_COOLDOWN;

                        // honestly doesn't really do anything in the very short timespan until the next update
                        // but it seems more correct to do so anyway
                        /*___Footsteps.Play();
                        ___FootstepSound.Play();
                        ___FootstepsActive = true;
                        ___Animator.SetFloat(___MovementSpeed, 1);*/
                    }


		    }
        }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            DashPreferences.registerPreferences();
            initPauseMenu();
        }

        protected override void OnUpdate() {}

        private bool isPingDownForPlayer(PlayerView player) {
            FieldInfo fieldInfo = ReflectionUtils.GetField<PlayerView>("Data");
            PlayerView.ViewData viewData = (PlayerView.ViewData)fieldInfo.GetValue(player);
            ButtonState buttonState = viewData.Inputs.State.SecondaryAction2;

            // if the HoldButton option is used, a held dash button is allowed as well
            return buttonState == ButtonState.Pressed || (DashPreferences.isHoldButton() && buttonState == ButtonState.Held);
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(DashMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(DashMenu<PauseMenuAction>), new DashMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }

    class DashStatus {
        public bool CanDash = true;
        public float DashCooldown;
    }
}
