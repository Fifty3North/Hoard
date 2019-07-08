using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public class InvalidProductIdEncoutered : Event
    {
        public InvalidProductIdEncoutered(Guid id)
        {
            Id = id;
        }
    }
}
