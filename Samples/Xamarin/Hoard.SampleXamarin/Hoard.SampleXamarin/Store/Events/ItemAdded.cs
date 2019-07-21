using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class ItemAdded : Event
    {
        public readonly string Title;
        public readonly string Description;

        public ItemAdded(Guid id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }
    }
}
