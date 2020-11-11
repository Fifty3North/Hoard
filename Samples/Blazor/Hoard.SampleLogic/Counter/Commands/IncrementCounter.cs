using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Counter.Commands
{
    public record IncrementCounter : Command<CounterStore>
    {
    }
}
