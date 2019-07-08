using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public class ItemCheckedOut : Event
    {
        public readonly int QuantityCheckedOut;

        public ItemCheckedOut(Guid id, int quantityCheckedOut)
        {
            Id = id;
            QuantityCheckedOut = quantityCheckedOut;
        }
    }
}
