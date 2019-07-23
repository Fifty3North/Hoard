using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F3N.Hoard.State
{
    public interface IStatefulItem { }

    public interface IStatefulCollectionItem
    {
        Guid Id { get; }
    }

    public interface DomainCommand { }

    public interface DomainCommand<TStore> : DomainCommand where TStore : IStore
    {
    }

    public interface IDeletedEvent { }

    public abstract class Event
    {
        public Guid Id { get; protected set; }
    }
    
    public interface IStore : IInternalStoreMethods
    {
        //event Action StateChanged;
        event StageChangedEventHandler StateChangedEvent;
    }

    public delegate void StageChangedEventHandler(object sender, Event e);

    public interface IInternalStoreMethods
    {
        bool IsInitialised { get; }

        Task Initialise();
    }

    /// <summary>
    ///     Represents a store that encapsulates a state tree and is used to dispatch actions to update the
    ///     state tree.
    /// </summary>
    /// <typeparam name="TState">
    ///     The state tree type.
    /// </typeparam>
    public interface IStore<TState> : IStore where TState : class, IStatefulItem
    {
        /// <summary>
        ///     Dispatches an action to the store.
        /// </summary>
        /// <param name="action">
        ///     The action to dispatch.
        /// </param>
        /// <returns>
        ///     Varies depending on store enhancers. With no enhancers Dispatch returns the action that 
        ///     was passed to it.
        /// </returns>
        Task Dispatch<T>(DomainCommand<T> action) where T : IStore<TState>;

        TState Dispatch<T>(Event ev) where T : IStore<TState>;

        /// <summary>
        ///     Gets the current state tree.
        /// </summary>
        /// <returns>
        ///     The current state tree.
        /// </returns>
        TState CurrentState { get; }
    }

    /// <summary>
    ///     Represents a store that encapsulates a state tree and is used to dispatch actions to update the
    ///     state tree.
    /// </summary>
    /// <typeparam name="TState">
    ///     The state tree type.
    /// </typeparam>
    public interface IStoreCollection<TState> : IStore where TState : class, IStatefulCollectionItem
    {
        /// <summary>
        ///     Dispatches an action to the store.
        /// </summary>
        /// <param name="action">
        ///     The action to dispatch.
        /// </param>
        /// <returns>
        ///     Varies depending on store enhancers. With no enhancers Dispatch returns the action that 
        ///     was passed to it.
        /// </returns>
        Task Dispatch<T>(DomainCommand<T> action) where T : IStoreCollection<TState>;

        IReadOnlyCollection<TState> Dispatch<T>(Event ev) where T : IStoreCollection<TState>;

        /// <summary>
        ///     Gets the current state tree.
        /// </summary>
        /// <returns>
        ///     The current state tree.
        /// </returns>
        IReadOnlyCollection<TState> CurrentState { get; }
    }
}
