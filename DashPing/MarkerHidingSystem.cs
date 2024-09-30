using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenDashPing {

    [UpdateBefore(typeof(PlayerPingView.UpdateView))]
    public class MarkerHiderSystem : GenericSystemBase, IModSystem {

        private EntityQuery Query;

        protected override void Initialise() {
            base.Initialise();
            Query = GetEntityQuery((ComponentType)typeof(CPlayerPing));
        }

        protected override void OnUpdate() {
            if (!DashPreferences.isShowMarker()) {
                EntityManager.DestroyEntity(Query);
            }
        }
    }
}
