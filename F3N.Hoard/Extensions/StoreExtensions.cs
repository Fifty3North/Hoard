using F3N.Hoard.State;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace F3N.Hoard
{
    public static class StoreExtensions
    {
        public static IObservable<T> Observe<T>(this IStore<T> store) where T : class, IStatefulItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h)
                .Select(ev => store.CurrentState);
        }

        public static IObservable<(T State, Event @event)> ObserveWithEvents<T>(this IStore<T> store) where T : class, IStatefulItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h)
                .Select(ev => (store.CurrentState, ev));
        }

        public static IObservable<T> ObserveWhere<T>(this IStore<T> store, Func<Event, bool> predicate) where T : class, IStatefulItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h).Where(predicate)
                .Select(ev => store.CurrentState);
        }

        public static IObservable<(T State, Event @event)> ObserveWhereWithEvents<T>(this IStore<T> store, Func<Event, bool> predicate) where T : class, IStatefulItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h).Where(predicate)
                .Select(ev => (store.CurrentState, ev));
        }

        public static IObservable<T> Observe<T>(this IStoreCollection<T> store) where T : class, IStatefulCollectionItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h)
                .Select(ev => store.CurrentState.FirstOrDefault(s => s.Id == ev.Id));
        }

        public static IObservable<(T State, Event @event)> ObserveWithEvents<T>(this IStoreCollection<T> store) where T : class, IStatefulCollectionItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h)
                .Select(ev => (store.CurrentState.FirstOrDefault(s => s.Id == ev.Id), ev));
        }

        public static IObservable<T> ObserveWhere<T>(this IStoreCollection<T> store, Func<Event, bool> predicate) where T : class, IStatefulCollectionItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h).Where(predicate)
                .Select(ev => store.CurrentState.FirstOrDefault(s => s.Id == ev.Id));
        }

        public static IObservable<(T State, Event @event)> ObserveWhereWithEvents<T>(this IStoreCollection<T> store, Func<Event, bool> predicate) where T : class, IStatefulCollectionItem
        {
            return Observable
                .FromEvent<StageChangedEventHandler, Event>(
                    handler => {
                        StageChangedEventHandler fsHandler = (sender, e) =>
                        {
                            handler(e);
                        };

                        return fsHandler;
                    },
                    h => store.StateChangedEvent += h,
                    h => store.StateChangedEvent -= h).Where(predicate)
                .Select(ev => (store.CurrentState.FirstOrDefault(s => s.Id == ev.Id), ev));
        }
    }
}
