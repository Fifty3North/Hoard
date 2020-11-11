using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore.Events
{
    public record ProductRegistered : Event
    {
        public readonly string Title;
        public readonly int InitialQuantity;

        public ProductRegistered(Guid id, string title, int initialQuantity) : base(id)
        {
            Title = title;
            InitialQuantity = initialQuantity;
        }
    }
}
