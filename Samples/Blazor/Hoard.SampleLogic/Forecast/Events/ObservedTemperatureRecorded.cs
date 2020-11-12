using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Forecast.Events
{
    public record ObservedTemperatureRecorded(Guid Id, DateTime DateRecorded, int Temperature) : Event(Id);
}
