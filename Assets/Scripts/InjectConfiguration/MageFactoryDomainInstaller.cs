using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Domain;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.Service;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Domain.Service;
using MageFactory.Flow.Api;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Domain.Service;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Domain.Service;
using MageFactory.Item.Domain.Service;
using MageFactory.Shared.Model;
using Zenject;

namespace MageFactory.InjectConfiguration {
    /// <summary>
    /// Domenowa kompozycja: Flow + CombatContext + Character + Inventory + Item + EventHuby + ActionExecutor.
    /// Bez UI/Unity-view bindingów (prefaby, FromComponentInHierarchy, MonoBehaviour itp.).
    ///
    /// Cel: możliwe uruchomienie w EditMode testach na czystym DiContainer.
    /// </summary>
    public sealed class MageFactoryDomainInstaller : Installer<MageFactoryDomainInstaller> {
        private const int FlowMaxStepsPerTickSafetyLimit = 5_000;
        private const int MaxInventoryGridWidth = 100;
        private const int MaxInventoryGridHeight = 50;

        public override void InstallBindings() {
            installSignals();
            installEventHubs();
            installItemsAndInventory();
            installFlow();
            installActionExecutor();
            installCombatAndCharacters();
        }

        private void installSignals() {
            // Domena używa SignalBus (ActionContextFactory -> ActionContext -> ItemPowerChanged event w UI),
            // ale w domenie nie musisz deklarować konkretnych sygnałów, jeśli nic nie subskrybuje.
            SignalBusInstaller.Install(Container);
        }

        private void installEventHubs() {
            // CombatContext events
            Container
                .Bind(typeof(ICombatContextEventPublisher), typeof(ICombatContextEventRegistry))
                .To<CombatContextEventHub>()
                .AsSingle();

            // Character events
            Container
                .Bind(typeof(ICharacterEventPublisher), typeof(ICharacterEventRegistry))
                .To<CharacterEventHub>()
                .AsSingle();

            // Inventory events
            Container
                .Bind(typeof(IInventoryEventPublisher), typeof(IInventoryEventRegistry))
                .To<InventoryEventHub>()
                .AsSingle();
        }

        private void installItemsAndInventory() {
            // Item factory (tworzenie entrypointów i normalnych itemów do inventory)
            Container.BindInterfacesTo<ItemFactoryService>().AsSingle();

            Container.Bind<InventoryGridConfiguration>()
                .FromInstance(new InventoryGridConfiguration(
                    new GridDimensions(MaxInventoryGridWidth, MaxInventoryGridHeight)));

            // Inventory factory per-character
            Container.Bind<ICharacterInventoryFactory>()
                .To<CharacterInventoryFactory>()
                .AsSingle();
        }

        private void installFlow() {
            // Factory do ActionContext (wymaga SignalBus)
            Container.Bind<ActionContextFactory>()
                .AsSingle();

            // Flow factory
            Container.Bind<IFlowFactory>()
                .To<FlowFactoryService>()
                .AsSingle();

            Container.Bind<FlowProcessorSettings>()
                .FromMethod(_ => new FlowProcessorSettings(
                    maxStepsPerSlice: FlowMaxStepsPerTickSafetyLimit));
        }

        private void installActionExecutor() {
            Container.Bind<IActionExecutor>()
                .To<ActionExecutorService>()
                .AsSingle();
        }

        private void installCombatAndCharacters() {
            // Character creation & capabilities
            Container.Bind<CharacterFactory>().AsSingle();
            Container.Bind<CharacterCombatEventProcessorFactory>().AsSingle();
            Container.Bind<CharacterCombatCapabilitiesFactory>().AsSingle();

            // Combat character factory (łączy CharacterAggregate + capabilities + flowFactory)
            Container.Bind<ICombatCharacterFactory>()
                .To<CombatCharacterFactory>()
                .AsSingle();

            // CombatContext factory
            Container.Bind<ICombatContextFactory>()
                .To<CombatContextFactoryService>()
                .AsSingle();
        }
    }
}