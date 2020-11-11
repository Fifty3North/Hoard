using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public record ItemCheckedOut : Event
    {
        public readonly int QuantityCheckedOut;

        public ItemCheckedOut(Guid id, int quantityCheckedOut) : base(id) => QuantityCheckedOut = quantityCheckedOut;
    }
}
