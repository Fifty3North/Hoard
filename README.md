# F3N.Hoard

![Nuget](https://img.shields.io/nuget/v/F3N.Hoard.svg)
![Azure DevOps builds](https://img.shields.io/azure-devops/build/andy0505/3270ed0d-e050-46bb-be0a-077a5b7e8f5a/1.svg)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/andy0505/3270ed0d-e050-46bb-be0a-077a5b7e8f5a/1.svg)

Cross platform storage for native apps used in-house at Fifty3North.

Based loosely on Redux and generic dispatcher used in Orleankka, views can subscribe to the store and be notified when it updates.

Works anywhere but tested in Blazor and Xamarin.

Uses SQLite and Akavache API to provide persistence

## TODOs

1. ~~Create tests~~
2. Make storage module pluggable
3. Create none Akavache storage example
4. ~~Provide Xamarin example~~
5. ~~Documentation~~
6. ~~Publish NuGet package~~

## Works well with:
[F3N.YaMVVM](https://github.com/Fifty3North/YaMVVM) 
> Yet another Model, View, ViewModel framework for Xamarin Forms.

## Pre-requisites

Visual Studio 2019 and .Net Core Preview 6 (for Blazor samples)

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
public class RegisterProduct : Command<WidgetStore>
{
    public readonly Guid Id;
    public readonly string Title;
    public readonly int IntialStockQuantity;

    public RegisterProduct(Guid id, string title, int initialStockQuantity)
    {
        Id = id;
        Title = title;
        IntialStockQuantity = initialStockQuantity;
    }
}
```

### Event

An event is a fact that has happened within your system. It contains all the information required to change state in your store.

Events always have an Id of type Guid.

```csharp
public class ProductRegistered : Event
{
    public readonly string Title;
    public readonly int InitialQuantity;

    public ProductRegistered(Guid id, string title, int initialQuantity)
    {
        Id = id;
        Title = title;
        InitialQuantity = initialQuantity;
    }
}
```

### Store

There are two types of **store**: one for storing a single object, and one for storing a collection of objects.

**Single object**

```csharp
public class CounterStore : Store<CounterStore, CounterState> { ... }
```

**Collection**

```csharp
public class WidgetStore : StoreCollection<WidgetStore,WidgetState> { ... }
```

#### Command Hanlders

Command handlers examine command and determine which events to raise. This is also where any communication with any external APIs occurs. Based on the result from the API different events can be raised.

```csharp
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

await forecastStore.Dispatch(new Hoard.SampleLogic.Forecast.Commands.RecordObservedTemperature(Guid.NewGuid(), recordedDate, temperatureRecorded));
```


### SQLite

You must initialise Akavache using the following on App start:

```csharp
LocalStorage.Initialise("Your.App.Name");
```

The data is stored in `%LocalAppData%\Hoard.SampleWeb\BlobCache` (`c:\users\<username>\Appdata\Local\Hoard.SampleWeb\BlobCache`)
