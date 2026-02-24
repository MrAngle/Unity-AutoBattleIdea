using MageFactory.Shared.Id;

namespace MageFactory.Flow.Contract {
    public interface IFlowOwner {
        Id<CharacterId> getFlowOwnerCharacterId();
    }
}