using System.Reflection;
using System.Linq;
using Controllers;
using Kitchen;
using KitchenLib.Utils;
using KitchenDashPing.settings;

namespace KitchenDashPing {

    public abstract class AbstractDashSystem : GenericSystemBase {

        /// <summary>
        /// the currently selected DashPreferences.getDashFlavor() value is passed to this method. if your system supports it, return true
        /// </summary>
        protected abstract bool supports(DashFlavorType type);

        /// <summary>
        /// your implmentation's handler. called for each player that is currently pressing ping
        /// </summary>
        protected abstract void handleDashPressedForPlayer(PlayerView player);

        /// <summary>
        /// override if something needs to happen on update, even if ping isn't pressed
        /// </summary>
        protected virtual void preDashUpdate() {
        }

        protected override void OnUpdate() {
            if (supports(DashPreferences.getDashFlavor())) {
                preDashUpdate();
                PlayerInfoManager.FindObjectsOfType<PlayerView>().ToList()
                    .Where(isPingDownForPlayer).ToList()
                    .ForEach(handleDashPressedForPlayer);
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

        public AbstractDashSystem() { }
    }
}
