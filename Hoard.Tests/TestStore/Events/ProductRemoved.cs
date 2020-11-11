using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public record ProductRemoved : Event, IDeletedEvent
    {
        public ProductRemoved(Guid id) : base(id) { }
    }
}
