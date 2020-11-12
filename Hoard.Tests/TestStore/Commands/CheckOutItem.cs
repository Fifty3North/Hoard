using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Commands
{
    public record CheckOutItem(Guid Id, int Quantity) : Command<WidgetStore>;
}
