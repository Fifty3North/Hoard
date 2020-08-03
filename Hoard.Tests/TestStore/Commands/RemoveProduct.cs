using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Commands
{
    public class RemoveProduct : Command<WidgetStore>
    {
        public readonly Guid Id;

        public RemoveProduct(Guid id)
        {
            Id = id;
        }
    }
}
