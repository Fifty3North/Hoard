using F3N.Hoard;
using F3N.Hoard.State;
using F3N.Hoard.Storage;
using Hoard.Tests.TestStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.DependencyInjection;
using F3N.Hoard.Sqlite;

namespace Hoard.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Test store without any dependencies (in memory only)
            //services.AddScoped<IStorage, TestStore.TestStore>();

            /*
            * BlobCache.LocalMachine - Cached data. This data may get deleted without notification.
            * BlobCache.UserAccount - User settings. Some systems backup this data to the cloud.
            * BlobCache.Secure - For saving sensitive data - like credentials.
            * BlobCache.InMemory - A database, kept in memory. The data is stored for the lifetime of the app.
            * https://github.com/reactiveui/Akavache
            */

            // Akavache Sqlite secure store
            SqliteConfig.Initialise("HoardTest", () => BlobCache.Secure);
            services.AddSingleton<IStorage, BlobCacheStorage>();
        }
    }

    public class StoreTests
    {
        private WidgetStore widgetStore;
        public StoreTests(IStorage storage)
        {
            widgetStore = new WidgetStore(storage);
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItem()
        {
            await widgetStore.Initialise();

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
            Assert.Equal(command.InitialStockQuantity, updatedItem.StockQuantity);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemAndEvent()
        {
            await widgetStore.Initialise();

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
            Assert.Equal(command.InitialStockQuantity, updatedItem.StockQuantity);

            Assert.NotNull(ev);
            Assert.Equal(command.Id, ev.Id);
            Assert.Equal(command.InitialStockQuantity, ev.InitialQuantity);
            Assert.Equal(command.Title, ev.Title);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemUsingWhereClause()
        {
            await widgetStore.Initialise();

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
            Assert.Equal(product1Command.InitialStockQuantity, product.StockQuantity);

            subscription.Dispose();
        }

        [Fact]
        public async Task EventSubscriptionDeliversStateItemAndEventUsingWhereClause()
        {
            await widgetStore.Initialise();

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
            Assert.Equal(product1Command.InitialStockQuantity, product.StockQuantity);

            Assert.Single(events);
            Assert.True(events.First() is TestStore.Events.ProductRegistered);
            TestStore.Events.ProductRegistered ev = events.First() as TestStore.Events.ProductRegistered;
            Assert.Equal(product1Command.Id, ev.Id);
            Assert.Equal(product1Command.InitialStockQuantity, ev.InitialQuantity);
            Assert.Equal(product1Command.Title, ev.Title);

            subscription.Dispose();
        }

        // Command NonRegisteredCommand has no registered handler
        // Ensure it throws InvalidOperationException
        [Fact]
        public async Task NonRegisteredCommandThrows()
        {
            await widgetStore.Initialise();

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
            await widgetStore.Initialise();

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
            await widgetStore.Initialise();

            await widgetStore.Reset();

            var widgetState = widgetStore.CurrentState;

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

            widgetState = widgetStore.CurrentState;
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
            await widgetStore.Initialise();

            await widgetStore.Reset();

            var widgetState = widgetStore.CurrentState;

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

            widgetState = widgetStore.CurrentState;
            Assert.Equal(5, widgetState.Count);
            Assert.Equal(15, widgetState.First(w => w.Id == product1Command.Id).StockQuantity);
            Assert.Equal(16, widgetState.First(w => w.Id == product2Command.Id).StockQuantity);
            Assert.Equal(17, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);
            Assert.Equal(18, widgetState.First(w => w.Id == product4Command.Id).StockQuantity);
            Assert.Equal(19, widgetState.First(w => w.Id == product5Command.Id).StockQuantity);

            var stockAdjustmentCommand = new TestStore.Commands.CheckOutItem(product3Command.Id, 5);
            await widgetStore.Dispatch(stockAdjustmentCommand);

            widgetState = widgetStore.CurrentState;
            Assert.Equal(12, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);
        }

        // ensure item can be deleted
        [Fact]
        public async Task EnsureItemCanBeDeleted()
        {
            await widgetStore.Initialise();

            await widgetStore.Reset();

            var widgetState = widgetStore.CurrentState;

            Assert.Empty(widgetState);

            var product1Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 1", 15);
            var product2Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 2", 16);
            var product3Command = new TestStore.Commands.RegisterProduct(Guid.NewGuid(), "Widget 3", 17);

            await widgetStore.Dispatch(product1Command);
            await widgetStore.Dispatch(product2Command);
            await widgetStore.Dispatch(product3Command);

            widgetState = widgetStore.CurrentState;
            Assert.Equal(3, widgetState.Count());
            Assert.Equal(15, widgetState.First(w => w.Id == product1Command.Id).StockQuantity);
            Assert.Equal(16, widgetState.First(w => w.Id == product2Command.Id).StockQuantity);
            Assert.Equal(17, widgetState.First(w => w.Id == product3Command.Id).StockQuantity);

            var deleteCommand = new TestStore.Commands.RemoveProduct(product3Command.Id);
            await widgetStore.Dispatch(deleteCommand);

            widgetState = widgetStore.CurrentState;
            Assert.Equal(2, widgetState.Count);
            Assert.True(widgetState.All(w => w.Id != product3Command.Id));
        }
    }
}
