using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class TitleUpdated : Event
    {
        public readonly string Title;

        public TitleUpdated(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
