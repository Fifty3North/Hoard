using F3N.Hoard.State;
using Hoard.SampleXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoard.SampleXamarin.Store
{
    public class ItemStore : StoreCollection<ItemStore,Item>
    {
        public static Task<ItemStore> Instance
        {
            get
            {
                return GetInitialisedInstance();
            }
        }

        public static Task<IReadOnlyCollection<Item>> State
        {
            get
            {
                return GetStaticState();
            }
        }

        public IEnumerable<Event> Handle(Commands.AddItem command)
        {
            if(command.Id == Guid.Empty)
            {
                return new[] { new Events.ItemAddFailed("Id is blank") };
            }
            else if (string.IsNullOrEmpty(command.Title))
            {
                return new[] { new Events.ItemAddFailed("Title is a mandatory field") };
            }

            return new[] { new Events.ItemAdded(command.Id, command.Title, command.Description) };
        }

        public IEnumerable<Event> Handle(Commands.RemoveItem command)
        {
            var lookup = CurrentState.FirstOrDefault(i => i.Id == command.Id);

            if (lookup == null)
            {
                return new[] { new Events.ItemNotFoundOccured(command.Id) };
            }

            return new[] { new Events.ItemRemoved(command.Id) };
        }

        public IEnumerable<Event> Handle(Commands.UpdateTitle command)
        {
            return new[] { new Events.TitleUpdated(command.Id, command.Title) };
        }

        public IEnumerable<Event> Handle(Commands.UpdateDescription command)
        {
            return new[] { new Events.DescriptionUpdated(command.Id, command.Description) };
        }

        public void On(Events.ItemAdded ev)
        {
            AddOrReplaceItem(new Item() { Id = ev.Id, Text = ev.Title, Description = ev.Description });
        }

        public void On(Events.ItemRemoved ev) => RemoveItems(i => i.Id == ev.Id);

        public void On(Events.TitleUpdated ev)
        {
            var lookup = CurrentState.FirstOrDefault(widget => widget.Id == ev.Id);
            
            if(lookup != null)
            {
                lookup.Text = ev.Title;
                AddOrReplaceItem(lookup);
            }
        }

        public void On(Events.DescriptionUpdated ev)
        {
            var lookup = CurrentState.FirstOrDefault(widget => widget.Id == ev.Id);

            if (lookup != null)
            {
                lookup.Description = ev.Description;
                AddOrReplaceItem(lookup);
            }
        }

        public void On(Events.ItemAddFailed ev) { }
        public void On(Events.ItemNotFoundOccured ev) { }
    }
}
