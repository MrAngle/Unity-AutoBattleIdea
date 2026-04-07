using MageFactory.Character.Contract.Event;
using Zenject;

namespace MageFactory.InjectConfiguration {
    public sealed class MageFactoryUiSignalsInstaller : Installer<MageFactoryUiSignalsInstaller> {
        public override void InstallBindings() {
            Container.DeclareSignal<ItemRemovedDtoEvent>();
            Container.DeclareSignal<ItemPowerChangedDtoEvent>();
        }
    }
}