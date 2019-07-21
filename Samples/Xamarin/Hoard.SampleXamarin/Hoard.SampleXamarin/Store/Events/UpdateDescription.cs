using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class DescriptionUpdated : Event
    {
        public readonly string Description;

        public DescriptionUpdated(Guid id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
