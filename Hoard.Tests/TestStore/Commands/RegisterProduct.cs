using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Commands
{
    public class RegisterProduct : DomainCommand<WidgetStore>
    {
        public readonly Guid Id;
        public readonly string Title;
        public readonly int IntialStockQuantity;

        public RegisterProduct(Guid id, string title, int initialStockQuantity)
        {
            Id = id;
            Title = title;
            IntialStockQuantity = initialStockQuantity;
        }
    }
}
