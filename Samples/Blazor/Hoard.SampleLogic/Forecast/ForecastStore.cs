using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hoard.SampleLogic.Forecast
{
    public class ForecastStore : StoreCollection<ForecastStore, ForecastState>
    {
        public static Task<ForecastStore> Instance
        {
            get
            {
                return GetInitialisedInstance();
            }
        }

        public static Task<IReadOnlyCollection<ForecastState>> State
        {
            get
            {
                return GetStaticState();
            }
        }

        public IEnumerable<Event> Handle(Commands.RecordObservedTemperature command)
        {
            return new[] { new Events.ObservedTemperatureRecorded(command.Id, command.DateRecorded, command.Temperature) };
        }

        public void On(Events.ObservedTemperatureRecorded ev)
        {
            AddOrReplaceItem(new ForecastState(ev.Id, ev.DateRecorded, ev.Temperature));
        }
    }
}
