using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using F3N.Hoard;

namespace Hoard.SampleLogic.Forecast
{
    public class ForecastStore : StoreCollection<ForecastState>
    {
        public IEnumerable<Event> Handle(Commands.RecordObservedTemperature command)
        {
            return new[] { new Events.ObservedTemperatureRecorded(command.Id, command.DateRecorded, command.Temperature) };
        }

        public void On(Events.ObservedTemperatureRecorded ev)
        {
            AddOrReplaceItem(new ForecastState(ev.Id, ev.DateRecorded, ev.Temperature));
        }

        public ForecastStore(IStorage storage) : base(storage)
        {
        }
    }
}
