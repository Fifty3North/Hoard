# F3N.Hoard

[![Join the chat at https://gitter.im/Fifty3North/Hoard](https://badges.gitter.im/Fifty3North/Hoard.svg)](https://gitter.im/Fifty3North/Hoard?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
![.NET Core](https://github.com/Fifty3North/Hoard/workflows/.NET%20Core/badge.svg)

## F3N.Hoard
![Nuget](https://img.shields.io/nuget/v/F3N.Hoard.svg)

## F3N.Hoard.Sqlite
![Nuget](https://img.shields.io/nuget/v/F3N.Hoard.Sqlite.svg)

## F3N.Hoard.BlazorLocalStorage
![Nuget](https://img.shields.io/nuget/v/F3N.Hoard.BlazorServerStorage.svg)

## F3N.Hoard.BlazorWasmStorage
![Nuget](https://img.shields.io/nuget/v/F3N.Hoard.BlazorWasmStorage.svg)


True MVU pattern for UI frameworks including Blazor and Maui with cross-platform persistent storage.

Based loosely on Redux and generic dispatcher used in Orleankka, views can subscribe to the store and be notified when it updates.

## How to use

You will need a **store**, a **state** object to put in your store, a **command** and an **event**.

Your store will have **command handlers** and **event handlers**.

**Command handlers** will perform the logic and any communication with external APIs. Depending on the outcome, one or more **events** will be raised.

**Event handlers** will respond and update **state** in the **store**.

### Command

Commands contain a unit of user intent. They contain all the information required to validate the intent and change state.

Commands belong to a store and are typed as such using inheritance from base Command.

They should be immutable by design but this is not enforced by Hoard.

```csharp
public record IncrementCounter : F3N.Hoard.State.Command<CounterStore>;
```

### Event

An event is a fact that has happened within your system. It contains all the information required to change state in your store.

Events always have an Id of type Guid.

```csharp
public record CounterIncremented : Event;
```

### Store

```csharp
public class CounterStore : Store<CounterState>
{
    public CounterStore(IStorage _storage) : base(_storage) { }
    public IEnumerable<Event> Handle(IncrementCounter command) => new[] { new CounterIncremented() };
    public IEnumerable<Event> Handle(DecrementCounter command) => new[] { new CounterDecremented() };
    public void On(CounterIncremented ev) => _state.Count++;
    public void On(CounterDecremented ev) => _state.Count--;
}
```
#### Command Hanlders

Command handlers examine command and determine which events to raise. This is also where any communication with any external APIs occurs. Based on the result from the API different events can be raised.

```csharp
public IEnumerable<Event> Handle(Commands.RegisterProduct command)
{

    // Do any API communication here

    if (CurrentState.Any(widget => widget.Id == command.Id))
    {
        return new[] { new Events.DuplicateProductIdEncountered(command.Id, command.Title) };
    }
    else
    {
        return new[] { new Events.ProductRegistered(command.Id, command.Title, command.InitialStockQuantity) };
    }
}
```

#### Event Handlers

Event handlers take the event and mutate state. 

State can be modified in a single object store by accessing the `_state` object to update properties or can use `SetState` to replace the entire state.

In a store collection, state is modified using: `AddOrReplaceItem(product);` and `RemoveItem(product);` or to replace the whole collection either `SetState` or `Reset` to clear state.

```csharp
public void On(Events.ProductRegistered ev)
{
    var product = new WidgetState(ev.Id, ev.Title, ev.InitialQuantity);

    AddOrReplaceItem(product);
}
```

### State

Your state needs to be serializable to store it in Akavache. If you have a constructor you need to make sure the parameters are named the same as the public properties or fields.

Single object state items implement `IStatefulItem` and store collection state items implement `IStatefulCollectionItem`

```csharp
public class WidgetState : IStatefulCollectionItem
{
    public Guid Id { get; }

    public string Title { get; }

    public int StockQuantity { get; set; }

    public WidgetState(Guid id, string title, int stockQuantity = 0)
    {
        Id = id;
        Title = title;
        StockQuantity = stockQuantity;
    }
}
```

### UI

The UI can either query state directly:
```csharp
WidgetStore widgetStore = new WidgetStore();
await widgetStore.Initialise();

WidgetState state = widgetStore.CurrentState
```

Or subsribe to the store to receive updates:

```csharp
ForecastStore forecastStore = new ForecastStore();
await forecastStore.Initialise();

IDisposable subscription = forecastStore.Observe().Subscribe(state =>
{
    int lookupIndex = forecasts.FirstIndexOf(s => s?.Id == state.Id);

    WeatherForecast preparedViewModel = ToViewModel(state);

    if (lookupIndex == -1)
    {
        forecasts.Insert(preparedViewModel, (a, b) => a.Date > b.Date);
    }
    else
    {
        forecasts[lookupIndex] = preparedViewModel;
    }
});
```

Or both: load initial state from Store and then keep up to date by subscribing.

There are multiple Observe methods that will deliver events also and can filter based on specific event Ids:

#### ObserveWithEvents
```csharp
var subscription = widgetStore.ObserveWithEvents().Subscribe((payload) =>
{
    updatedItem = payload.State;
    if (payload.@event is TestStore.Events.ProductRegistered registeredEvent)
    {
        ev = registeredEvent;
    }
});
```

#### ObserveWhere

```csharp
var subscription = widgetStore.ObserveWhere(ev => ev.Id == product1Command.Id).Subscribe(state =>
{
    products.Add(state);
});
```

#### ObserveWhereWithEvents

```csharp
var subscription = widgetStore.ObserveWhereWithEvents(ev => ev.Id == product1Command.Id).Subscribe((payload) =>
{
    products.Add(payload.State);
    events.Add(payload.@event);
});
```

#### Issuing commands from the UI

Take user input and dispatch to the store

```csharp
ForecastStore forecastStore = new ForecastStore();
await forecastStore.Initialise();

await forecastStore.Dispatch(new RecordObservedTemperature(Guid.NewGuid(), recordedDate, temperatureRecorded));
```

### SQLite

You must initialise Akavache using the following on App start:

```csharp
SqliteConfig.Initialise("Your.App.Name", () => BlobCache.Secure);
```

The data is stored in `%LocalAppData%\Hoard.SampleWeb\BlobCache` (`c:\users\<username>\Appdata\Local\Hoard.SampleWeb\BlobCache`)

