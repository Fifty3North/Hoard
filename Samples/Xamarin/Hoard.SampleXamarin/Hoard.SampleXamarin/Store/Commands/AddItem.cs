using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Commands
{
    public class AddItem : DomainCommand<ItemStore>
    {
        public readonly Guid Id;
        public readonly string Title;
        public readonly string Description;

        public AddItem(Guid id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }
    }
}
