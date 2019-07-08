using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleLogic.Counter
{
    public class CounterState : IStatefulItem
    { 
        public int Count { get; set; }
    }
}
