using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Commands
{
    public class CheckOutItem : Command<WidgetStore>
    {
        public readonly Guid Id;
        public readonly int Quantity; 

        public CheckOutItem(Guid id, int quantity)
        {
            Id = id;
            Quantity = quantity;
        }
    }
}
