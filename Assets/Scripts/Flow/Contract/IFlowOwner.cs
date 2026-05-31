using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Contract {
    public interface IFlowOwner {
        Id<CharacterId> getFlowOwnerCharacterId();
        int getActiveFlowCountOnItem(Id<ItemId> itemId);
        bool canProcessFlowItem(IFlowItem item);
        ItemFlowProcessingSlot startProcessingFlowItem(IFlowItem item);
        void finishProcessingFlowItem(ItemFlowProcessingSlot processingSlot);
    }
}