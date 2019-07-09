# F3N.Hoard
Cross platform storage for native apps used in-house at Fifty3North.

Based loosely on Redux and generic dispatcher used in Orleankka, views can subscribe to the store and be notified when it updates.

Works anywhere but tested in Blazor and Xamarin.

Uses SQLLite and Akavache API to provide persistence

## TODOs

1. ~~Create tests~~
2. Make storage module pluggable
3. Create none Akavache storage example
4. Provide Xamarin example
5. ~~Documentation~~
6. Publish NuGet package

## How to use

You will need a **store**, a **state** object to put in your store, a **command** and an **event**.

Your store will have **command handlers** and **event handlers**.

**Command handlers** will perform the logic and any communication with external APIs. Depending on the outcome, one or more **events** will be raised.

**Event handlers** will respond and update **state** in the **store**.

### Command

Commands contain a unit of user intent. They contain all the information required to validate the intent and change state.

Commands belong to a store and are typed as such using inheritance from base DomainCommand.

They should be immutable by design but this is not enforced by Hoard.

```
public class RegisterProduct : DomainCommand<WidgetStore>
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

An event is a fact that has happened within your system. It contains all the information required to chang state in your store.

Events always have an Id of type Guid.

```
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

Single object

`public class CounterStore : Store<CounterStore, CounterState> { ... }`

Collection

`public class WidgetStore : StoreCollection<WidgetStore,WidgetState> { ... }`

#### Command Hanlders

Command handlers examine command and determine which events to raise. This is also where any communication with any external APIs occurs. Based on the result from the API different events can be raised.

```
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

State can be modified in a single object store by accessing the `_state` object.

In a store collection, state is modified using: `AddOrReplaceItem(product);` and `RemoveItem(product);`

```
public void On(Events.ProductRegistered ev)
{
    var product = new WidgetState(ev.Id, ev.Title, ev.InitialQuantity);

    AddOrReplaceItem(product);
}
```

### UI

The UI can either query state directly:

`WidgetState state = await WidgetStore.State`

Or subsribe to the store to receive updates:

```
ForecastStore forecastStore = await ForecastStore.Instance;

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

#### Issuing commands from the UI

Take user input and dispatch to the store

```
ForecastStore forecastStore = await ForecastStore.Instance;

await forecastStore.Dispatch(new Hoard.SampleLogic.Forecast.Commands.RecordObservedTemperature(Guid.NewGuid(), recordedDate, temperatureRecorded));
```
