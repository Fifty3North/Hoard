using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public class DuplicateProductIdEncountered : Event
    {
        public readonly string Title;

        public DuplicateProductIdEncountered(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
