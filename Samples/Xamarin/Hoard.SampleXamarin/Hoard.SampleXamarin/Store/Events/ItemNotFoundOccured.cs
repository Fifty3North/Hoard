using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class ItemNotFoundOccured : Event
    {
        public ItemNotFoundOccured(Guid id)
        {
            Id = id;
        }
    }
}
