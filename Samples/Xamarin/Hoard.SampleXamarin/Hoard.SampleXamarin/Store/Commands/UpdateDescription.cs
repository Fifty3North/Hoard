using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Commands
{
    public class UpdateDescription : DomainCommand<ItemStore>
    {
        public readonly Guid Id;
        public readonly string Description;

        public UpdateDescription(Guid id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
