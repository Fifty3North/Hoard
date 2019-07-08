using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Forecast
{
    public class ForecastState : IStatefulCollectionItem
    {
        public Guid Id { get; }
        public DateTime DateRecorded { get; }
        public int Temperature { get; }

        public ForecastState(Guid id, DateTime dateRecorded, int temperature)
        {
            Id = id;
            DateRecorded = dateRecorded;
            Temperature = temperature;
        }
    }
}
