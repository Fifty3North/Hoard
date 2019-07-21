using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Forecast.Commands
{
    public class RecordObservedTemperature : DomainCommand<ForecastStore>
    {
        public readonly Guid Id;
        public readonly DateTime DateRecorded;
        public readonly int Temperature;

        public RecordObservedTemperature(Guid id, DateTime dateRecorded, int tempInCelcius)
        {
            Id = id;
            DateRecorded = dateRecorded;
            Temperature = tempInCelcius;
        }
    }
}
