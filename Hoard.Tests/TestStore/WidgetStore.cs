using F3N.Hoard.State;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hoard.Tests.TestStore
{
    public class WidgetStore : StoreCollection<WidgetStore,WidgetState>
    {
        public static Task<WidgetStore> Instance
        {
            get
            {
                return GetInitialisedInstance();
            }
        }

        public static Task<IReadOnlyCollection<WidgetState>> State
        {
            get
            {
                return GetStaticState();
            }
        }

        public IEnumerable<Event> Handle(Commands.RegisterProduct command)
        {
            if (CurrentState.Any(widget => widget.Id == command.Id))
            {
                return new[] { new Events.DuplicateProductIdEncountered(command.Id, command.Title) };
            }
            else
            {
                return new[] { new Events.ProductRegistered(command.Id, command.Title, command.IntialStockQuantity) };
            }
        }

        public IEnumerable<Event> Handle(Commands.CheckOutItem command)
        {
            var lookup = CurrentState.FirstOrDefault(widget => widget.Id == command.Id);

            if (lookup == null)
            {
                return new[] { new Events.InvalidProductIdEncoutered(command.Id) };
            }
            else
            {
                if (lookup.StockQuantity < command.Quantity)
                {
                    return new[] { new Events.InsufficientStockLevelEncountered(command.Id) };
                }
                else
                {
                    return new[] { new Events.ItemCheckedOut(command.Id, command.Quantity) };
                }
            }
        }

        public IEnumerable<Event> Handle(Commands.RemoveProduct command)
        {
            return new[] { new Events.ProductRemoved(command.Id) };
        }

        public void On(Events.ItemCheckedOut ev)
        {
            var lookup = CurrentState.FirstOrDefault(widget => widget.Id == ev.Id);

            if (lookup != null)
            {
                lookup.StockQuantity -= ev.QuantityCheckedOut;
                AddOrReplaceItem(lookup);
            }
        }

        public void On(Events.InsufficientStockLevelEncountered ev) { }

        public void On(Events.ProductRegistered ev)
        {
            var product = new WidgetState(ev.Id, ev.Title, ev.InitialQuantity);

            AddOrReplaceItem(product);
        }

        public void On(Events.InvalidProductIdEncoutered ev) { }

        public void On(Events.ProductRemoved ev)
        {
            var lookup = CurrentState.FirstOrDefault(widget => widget.Id == ev.Id);

            if (lookup != null)
            {
                RemoveItem(lookup);
            }
        }
    }
}
