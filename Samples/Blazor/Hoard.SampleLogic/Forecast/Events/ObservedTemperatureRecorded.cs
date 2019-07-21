using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Forecast.Events
{
    public class ObservedTemperatureRecorded : Event
    {
        public readonly DateTime DateRecorded;
        public readonly int Temperature;

        public ObservedTemperatureRecorded(Guid id, DateTime dateRecorded, int tempInCelcius)
        {
            Id = id;
            DateRecorded = dateRecorded;
            Temperature = tempInCelcius;
        }
    }
}
