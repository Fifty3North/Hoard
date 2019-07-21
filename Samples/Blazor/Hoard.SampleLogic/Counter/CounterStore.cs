using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hoard.SampleLogic.Counter
{
    public class CounterStore : Store<CounterStore, CounterState>
    {
        public static Task<CounterStore> Instance
        {
            get
            {
                return GetInitialisedInstance();
            }
        }

        public static Task<CounterState> State
        {
            get
            {
                return GetStaticState();
            }
        }

        public IEnumerable<Event> Handle(Commands.IncrementCounter command)
        {
            return new [] { new Events.CounterIncremented() };
        }

        public IEnumerable<Event> Handle(Commands.DecrementCounter command)
        {
            return new[] { new Events.CounterDecremented() };
        }

        public void On(Events.CounterIncremented ev)
        {
            _state.Count++;
        }

        public void On(Events.CounterDecremented ev)
        {
            _state.Count--;
        }
    }
}
