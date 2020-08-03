using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Commands
{
    public class UpdateTitle : Command<ItemStore>
    {
        public readonly Guid Id;
        public readonly string Title;

        public UpdateTitle(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
