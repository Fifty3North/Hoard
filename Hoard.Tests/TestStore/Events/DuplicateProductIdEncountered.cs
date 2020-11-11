using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public record DuplicateProductIdEncountered : Event
    {
        public readonly string Title;

        public DuplicateProductIdEncountered(Guid id, string title) : base(id)
        {
            Title = title;
        }
    }
}
