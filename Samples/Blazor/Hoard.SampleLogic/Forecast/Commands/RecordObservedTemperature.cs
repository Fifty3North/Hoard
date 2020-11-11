using F3N.Hoard.State;
using System;

namespace Hoard.SampleLogic.Forecast.Commands
{
    public record RecordObservedTemperature(Guid Id, DateTime DateRecorded, int Temperature) : Command<ForecastStore>;
}
