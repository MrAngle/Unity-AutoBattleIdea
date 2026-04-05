using System;
using System.Collections.Generic;
using MageFactory.Flow.Contract;

namespace MageFactory.Character.Domain.FlowCapability {
    internal class FlowQueries : IFlowQueries {
        public bool tryGetRightAdjacentItems(IFlowItem sourceFlowItem, out IEnumerable<IFlowItem> adjacentFlowItem) {
            throw new NotImplementedException();
        }
    }
}