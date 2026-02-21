using System;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.CombatContext.Controller {
    internal class CombatContextPresentationHandler : ICombatCharacterCreatedEventListener, ICombatContextEventListener,
        IDisposable {
        private ICombatContext combatContext; // TODO: change to "view model"
        private readonly ICombatContextEventRegistry combatContextEventRegistry;

        [Inject]
        public CombatContextPresentationHandler(ICombatContextEventRegistry combatContextEventRegistry) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);

            this.combatContextEventRegistry
                .subscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((ICombatContextEventListener)this);
        }

        public void onEvent(in CombatCharacterCreatedDtoEvent ev) {
            // throw new System.NotImplementedException();
            // TODO: update view model
        }

        public void onEvent(in CombatContextCreatedDtoEvent ev) {
            combatContext = ev.combatContext;
        }

        public void Dispose() {
            this.combatContextEventRegistry
                .unsubscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((ICombatContextEventListener)this);
        }
    }
}