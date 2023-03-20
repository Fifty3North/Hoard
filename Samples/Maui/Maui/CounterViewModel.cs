using F3N.Hoard;
using Maui.Hoard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Maui.ViewModels
{
    public class CounterViewModel : IDisposable, INotifyPropertyChanged
    {
        private CounterStore _counterStore;
        private IDisposable _subscription;

        public int Count { get; set; }
        public ICommand IncrementCounter { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CounterViewModel(CounterStore counterStore)
        {
            _counterStore = counterStore;
            IncrementCounter = new Command(async () => await _counterStore.Dispatch(new IncrementCounter()));
        }

        public CounterStore Store => _counterStore;

        public async Task Initialise()
        {
            await _counterStore.Initialise();

            _subscription = _counterStore.Observe().Subscribe((s) =>
            {
                Count = s.Count;
            });

            Count = _counterStore.CurrentState.Count;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
