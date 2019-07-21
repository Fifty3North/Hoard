using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Commands
{
    public class RemoveItem : DomainCommand<ItemStore>
    {
        public readonly Guid Id;

        public RemoveItem(Guid id)
        {
            Id = id;
        }
    }
}
