using F3N.Hoard.Extensions;
using F3N.Hoard.State;
using F3N.Hoard.Storage;
using Hoard.Tests.TestStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Hoard.Tests
{
    public class StoreTests
    {
        public StoreTests()
        {
            LocalStorage.Initialise("StoreTests");
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItem()
        {
            await LocalStorage.FlushAll();

            var widgetStore = await WidgetStore.Instance;
            WidgetState updatedItem = null;

            var subscription = widgetStore.Observe().Subscribe(state =>
            {
                updatedItem = state;
            });

            var command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 10);
            await widgetStore.Dispatch(command);

            Assert.NotNull(updatedItem);
            Assert.Equal(command.Id, updatedItem.Id);
            Assert.Equal(command.Title, updatedItem.Title);
            Assert.Equal(command.IntialStockQuantity, updatedItem.StockQuantity);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemAndEvent()
        {
            var widgetStore = await WidgetStore.Instance;
            WidgetState updatedItem = null;
            TestStore.Events.ProductRegistered ev = null;

            var subscription = widgetStore.ObserveWithEvents().Subscribe((payload) =>
            {
                updatedItem = payload.State;
                if (payload.@event is TestStore.Events.ProductRegistered registeredEvent)
                {
                    ev = registeredEvent;
                }
            });

            var command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 15);
            await widgetStore.Dispatch(command);

            Assert.NotNull(updatedItem);
            Assert.Equal(command.Id, updatedItem.Id);
            Assert.Equal(command.Title, updatedItem.Title);
            Assert.Equal(command.IntialStockQuantity, updatedItem.StockQuantity);

            Assert.NotNull(ev);
            Assert.Equal(command.Id, ev.Id);
            Assert.Equal(command.IntialStockQuantity, ev.InitialQuantity);
            Assert.Equal(command.Title, ev.Title);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemUsingWhereClause()
        {
            var widgetStore = await WidgetStore.Instance;

            List<WidgetState> products = new List<WidgetState>();

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);
            var product4Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 4", 18);
            var product5Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 5", 19);

            var subscription = widgetStore.ObserveWhere(ev => ev.Id == product1Command.Id).Subscribe(state =>
            {
                products.Add(state);
            });

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);
            await widgetStore.Dispatch(product4Command);
            await widgetStore.Dispatch(product5Command);

            Assert.Single(products);
            var product = products.First();
            Assert.Equal(product1Command.Id, product.Id);
            Assert.Equal(product1Command.Title, product.Title);
            Assert.Equal(product1Command.IntialStockQuantity, product.StockQuantity);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemAndEventUsingWhereClause()
        {
            var widgetStore = await WidgetStore.Instance;

            List<WidgetState> products = new List<WidgetState>();
            List<Event> events = new List<Event>();

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);
            var product4Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 4", 18);
            var product5Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 5", 19);

            var subscription = widgetStore.ObserveWhereWithEvents(ev => ev.Id == product1Command.Id).Subscribe((payload) =>
            {
                products.Add(payload.State);
                events.Add(payload.@event);
            });

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);
            await widgetStore.Dispatch(product4Command);
            await widgetStore.Dispatch(product5Command);

            Assert.Single(products);
            var product = products.First();
            Assert.Equal(product1Command.Id, product.Id);
            Assert.Equal(product1Command.Title, product.Title);
            Assert.Equal(product1Command.IntialStockQuantity, product.StockQuantity);

            Assert.Single(events);
            Assert.True(events.First() is TestStore.Events.ProductRegistered);
            TestStore.Events.ProductRegistered ev = events.First() as TestStore.Events.ProductRegistered;
            Assert.Equal(product1Command.Id, ev.Id);
            Assert.Equal(product1Command.IntialStockQuantity, ev.InitialQuantity);
            Assert.Equal(product1Command.Title, ev.Title);

            subscription.Dispose();
        }

        // Command NonRegisteredCommand has no registered handler
        // Ensure it throws InvalidOperationException
        [Fact]
        public async Task NonRegisteredCommandThrows()
        {
            var widgetStore = await WidgetStore.Instance;
            WidgetState updatedItem = null;

            var subscription = widgetStore.Observe().Subscribe(state =>
            {
                updatedItem = state;
            });

            var product1Command = new TestStore.Commands.NonRegisteredCommand();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await widgetStore.Dispatch(product1Command));
            subscription.Dispose();
        }

        // Event DuplicateProductIdEncountered has no registered handler
        // Ensure it throws InvalidOperationException
        [Fact]
        public async Task NonRegisteredEventThrows()
        {
            var widgetStore = await WidgetStore.Instance;
            WidgetState updatedItem = null;

            var subscription = widgetStore.Observe().Subscribe(state =>
            {
                updatedItem = state;
            });

            var duplicateId = Guid.NewGuid();
            var product1Command = new TestStore.Commands.RegisterProduct(duplicateId, "Widget 1", 10);
            var product2Command = new TestStore.Commands.RegisterProduct(duplicateId, "Widget 2", 8);
            await widgetStore.Dispatch(product1Command);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await widgetStore.Dispatch(product2Command));
            subscription.Dispose();
        }

        [Fact]
        public async Task EnsureStateIsUpdated()
        {
            var widgetStore = await WidgetStore.Instance;
            await widgetStore.Reset();

            var widgetState = await WidgetStore.State;

            Assert.Empty(widgetState);

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);
            var product4Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 4", 18);
            var product5Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 5", 19);

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);
            await widgetStore.Dispatch(product4Command);
            await widgetStore.Dispatch(product5Command);

            widgetState = await WidgetStore.State;
            Assert.Equal(5, widgetState.Count);
            Assert.Equal(15, widgetState.First(w => w.Id == product1Command.Id).StockQuantity);
            Assert.Equal(16, widgetState.First(w => w.Id == product2Command.Id).StockQuantity);
            Assert.Equal(17, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);
            Assert.Equal(18, widgetState.First(w => w.Id == product4Command.Id).StockQuantity);
            Assert.Equal(19, widgetState.First(w => w.Id == product5Command.Id).StockQuantity);
        }

        // ensure an item in collection can be modified
        [Fact]
        public async Task EnsureItemInCollectionCanBeUpdated()
        {
            var widgetStore = await WidgetStore.Instance;
            await widgetStore.Reset();

            var widgetState = await WidgetStore.State;

            Assert.Empty(widgetState);

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);
            var product4Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 4", 18);
            var product5Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 5", 19);

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);
            await widgetStore.Dispatch(product4Command);
            await widgetStore.Dispatch(product5Command);

            widgetState = await WidgetStore.State;
            Assert.Equal(5, widgetState.Count);
            Assert.Equal(15, widgetState.First(w => w.Id == product1Command.Id).StockQuantity);
            Assert.Equal(16, widgetState.First(w => w.Id == product2Command.Id).StockQuantity);
            Assert.Equal(17, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);
            Assert.Equal(18, widgetState.First(w => w.Id == product4Command.Id).StockQuantity);
            Assert.Equal(19, widgetState.First(w => w.Id == product5Command.Id).StockQuantity);

            var stockAdjustmentCommand = new TestStore.Commands.CheckOutItem(product3Command.Id, 5);
            await widgetStore.Dispatch(stockAdjustmentCommand);

            widgetState = await WidgetStore.State;
            Assert.Equal(12, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);
        }

        // ensure item can be deleted
        [Fact]
        public async Task EnsureItemCanBeDeleted()
        {
            var widgetStore = await WidgetStore.Instance;
            await widgetStore.Reset();

            var widgetState = await WidgetStore.State;

            Assert.Empty(widgetState);

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);

            widgetState = await WidgetStore.State;
            Assert.Equal(3, widgetState.Count);
            Assert.Equal(15, widgetState.First(w => w.Id == product1Command.Id).StockQuantity);
            Assert.Equal(16, widgetState.First(w => w.Id == product2Command.Id).StockQuantity);
            Assert.Equal(17, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);

            var deleteCommand = new TestStore.Commands.RemoveProduct(product3Command.Id);
            await widgetStore.Dispatch(deleteCommand);

            widgetState = await WidgetStore.State;
            Assert.Equal(2, widgetState.Count);
            Assert.True(widgetState.All(w => w.Id != product3Command.Id));
        }
    }
}
