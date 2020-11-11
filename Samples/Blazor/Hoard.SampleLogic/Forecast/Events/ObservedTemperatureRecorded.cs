using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Forecast.Events
{
    public record ObservedTemperatureRecorded : Event
    {
        public DateTime DateRecorded { get; }
        public int Temperature { get; }

        public ObservedTemperatureRecorded(Guid id, DateTime dateRecorded, int temperature) : base(id)
        {
            DateRecorded = dateRecorded;
            Temperature = temperature;
        }
    }
}
