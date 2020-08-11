using F3N.Hoard.Storage;
using F3N.Hoard.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace F3N.Hoard.State
{
    public abstract class Store<TState> : IStore<TState>, IInternalStoreMethods
        where TState : class, IStatefulItem, new()
    {
        protected TState _state = default;

        protected static object _lock = new object();
        private string _storeKeyName;

        public bool IsInitialised { get; private set; }

        private IStorage Storage;

        public Store(IStorage _storage)
        {
            Storage = _storage;
        }
        
        public async Task Initialise()
        {
            if (!IsInitialised)
            {
                try
                {
                    if (!IsInitialised)
                    {
                        _storeKeyName = this.GetType().Name;

                        await LoadInitialState();

                        InitialiseStoreSubscription();

                        IsInitialised = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void InitialiseStoreSubscription()
        {
            // Subscribe to save to Storage
            this.Observe().Subscribe(async (state) =>
            {
                if (state != null)
                {
                    await SaveStateToStorage(state);
                }
            });
        }

        private async Task LoadInitialState()
        {
            // Load from Storage
            TState resumedState = await Storage.GetByKey<TState>(_storeKeyName);

            // Default if not found
            _state = resumedState ?? InitialiseState();
        }

        private async Task UnloadAndSetInitialState()
        {
            // Clear from storage
            await Storage.DeleteByKey<TState>(_storeKeyName);

            // Set default initialise state
            _state = InitialiseState();
        }

        protected TState InitialiseState()
        {
            return new TState();
        }

        public TState CurrentState
        {
            get
            {
                if (!IsInitialised) throw new MethodAccessException("Store must be initialised before use;");

                TState readOnly;

                lock (_lock)
                {
                    readOnly = _state;
                }

                return readOnly;
            }
        }

        protected Task SetState(TState newState)
        {
            if (newState == null) return Task.CompletedTask;

            lock (_lock)
            {
                _state = newState;
                return SaveStateToStorage(_state);
            }
        }

        private Task SaveStateToStorage(TState state)
        {
            return Storage.SaveByKey(_storeKeyName, state);
        }

        public event StageChangedEventHandler StateChangedEvent;

        public async Task Dispatch<T>(Command<T> command) where T : IStore<TState>
        {
            if (!IsInitialised) throw new MethodAccessException("Store must be initialised before use;");

            IEnumerable<Event> events = await Dispatcher<T>.Dispatch(this, command);

            foreach (Event e in events?.ToList())
            {
                Dispatcher<T>.Dispatch(this, e);
                StateChangedEvent?.Invoke(this, e);
            }
        }

        public TState Dispatch<T>(Event ev) where T : IStore<TState>
        {
            Dispatcher<T>.Dispatch(this, ev);
            StateChangedEvent?.Invoke(this, ev);
            return CurrentState;
        }
    }


    public abstract class StoreCollection<TState> : IStoreCollection<TState>
        where TState : class, IStatefulCollectionItem
    {
        private List<TState> _state = new List<TState>();

        private readonly object _lock = new object();

        private IStorage Storage { get { return LocalStorage.LocalMachineStorage; } }

        public async Task Initialise()
        {
            if (!IsInitialised)
            {
                try
                {
                    if (!IsInitialised)
                    {
                        await LoadInitialState();

                        InitialiseStoreSubscription();

                        await PostInitialise();

                        IsInitialised = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void InitialiseStoreSubscription()
        {
            // Subscribe to Save / Remove from Storage
            this.ObserveWithEvents().Subscribe(async (s) =>
            {
                if (s.@event is IDeletedEvent)
                {
                    await Storage.DeleteByKey<TState>(s.@event.Id.ToString());
                }
                else
                {
                    if (s.State != null)
                    {
                        await Storage.SaveByKey(s.State.Id.ToString(), s.State);
                    }
                }
            });
        }

        private async Task LoadInitialState()
        {
            // Load from Storage
            List<TState> resumedState = await Storage.GetAll<TState>();

            // Default if not found
            _state = resumedState ?? new List<TState>();
        }

        /// <summary>
        /// This will delete all items in state
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            // Clear from storage
            await Storage.DeleteAll<TState>();

            // Set default initial state
            _state = new List<TState>();
        }

        protected virtual async Task PostInitialise()
        {
            // Any post initialisation
            await Task.CompletedTask;
        }

        public IReadOnlyCollection<TState> CurrentState
        {
            get
            {
                if (!IsInitialised) throw new MethodAccessException("Store must be initialised before use;");

                IReadOnlyCollection<TState> readOnly;

                lock (_lock)
                {
                    readOnly = new ReadOnlyCollection<TState>(_state);
                }

                return readOnly;
            }
        }

        public bool IsInitialised { get; private set; }

        protected Task SetState(List<TState> newState)
        {
            if (newState == null) return null;

            lock (_lock)
            {
                foreach (TState item in newState)
                {
                    AddOrReplaceItem(item);
                }

                if (_state.Count > 0)
                {
                    return SaveStateToStorage();
                }

                return null;
            }
        }

        protected Task SaveStateToStorage()
        {
            Dictionary<string, TState> items = new Dictionary<string, TState>();

            foreach (TState state in _state)
            {
                if (state != null)
                {
                    if (state.Id != Guid.Empty)
                    {
                        string itemId = state.Id.ToString();
                        if (!items.ContainsKey(itemId))
                        {
                            items.Add(itemId, state);
                        }
                    }
                }
            }

            return Storage.SaveAllByKey(items);
        }

        protected void RemoveItem(TState item)
        {
            lock (_lock)
            {
                int index = _state.FindIndex(i => i.Id == item.Id);
                if (index != -1)
                {
                    _state.RemoveAt(index);
                }
            }
        }

        protected void RemoveItems(Predicate<TState> items)
        {
            lock (_lock)
            {
                _state.RemoveAll(items);
            }
        }

        protected void AddOrReplaceItem(TState item)
        {
            lock (_lock)
            {
                int index = _state.FindIndex(i => i.Id == item.Id);

                if (index == -1)
                {
                    _state.Add(item);
                }
                else
                {
                    _state[index] = item;
                }
            }
        }

        public TState GetItem(Guid itemId)
        {
            return CurrentState.FirstOrDefault(i => i.Id == itemId);
        }

        public event StageChangedEventHandler StateChangedEvent;

        public async Task Dispatch<T>(Command<T> command) where T : IStoreCollection<TState>
        {
            if (!IsInitialised) throw new MethodAccessException("Store must be initialised before use;");

            IEnumerable<Event> events = await Dispatcher<T>.Dispatch(this, command);

            foreach (Event e in events?.ToList())
            {
                Dispatcher<T>.Dispatch(this, e);
                StateChangedEvent?.Invoke(this, e);
            }
        }

        public IReadOnlyCollection<TState> Dispatch<T>(Event ev) where T : IStoreCollection<TState>
        {
            Dispatcher<T>.Dispatch(this, ev);
            StateChangedEvent?.Invoke(this, ev);
            return CurrentState;
        }
    }
}

