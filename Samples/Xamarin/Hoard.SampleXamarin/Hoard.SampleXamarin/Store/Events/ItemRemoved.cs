using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class ItemRemoved : Event
    {
        public ItemRemoved(Guid id)
        {
            Id = id;
        }
    }
}
