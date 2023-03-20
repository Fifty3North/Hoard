using F3N.Hoard.State;
using F3N.Hoard;

namespace Maui.Hoard
{
    public class CounterStore : Store<CounterState>
    {
        public IEnumerable<Event> Handle(IncrementCounter command)
        {
            return new [] { new CounterIncremented() };
        }

        public IEnumerable<Event> Handle(DecrementCounter command)
        {
            return new[] { new CounterDecremented() };
        }

        public void On(CounterIncremented ev)
        {
            _state.Count++;
        }

        public void On(CounterDecremented ev)
        {
            _state.Count--;
        }

        public CounterStore(IStorage _storage) : base(_storage)
        {
        }
    }
}
