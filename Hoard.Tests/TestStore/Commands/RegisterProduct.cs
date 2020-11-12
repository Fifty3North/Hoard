using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Commands
{
    public record RegisterProduct(Guid Id, string Title, int InitialStockQuantity) : Command<WidgetStore>;
}
