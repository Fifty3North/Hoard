using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using F3N.Hoard.BlazorLocalStorage;
using F3N.Hoard.Shared;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;

namespace Hoard.SampleLogic.Counter
{
    public class CounterStore : Store<CounterState>
    {
        public IEnumerable<Event> Handle(Commands.IncrementCounter command)
        {
            return new [] { new Events.CounterIncremented() };
        }

        public IEnumerable<Event> Handle(Commands.DecrementCounter command)
        {
            return new[] { new Events.CounterDecremented() };
        }

        public async void On(Events.CounterIncremented ev)
        {
            _state.Count++;
        }

        public async void On(Events.CounterDecremented ev)
        {
            _state.Count--;
        }

        public CounterStore(IStorage _storage) : base(_storage)
        {
        }
    }
}
