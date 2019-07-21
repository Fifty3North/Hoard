using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.SampleXamarin.Store.Events
{
    public class ItemAddFailed : Event
    {
        public readonly string Reason;

        public ItemAddFailed(string reason)
        {
            Reason = reason;
        }
    }
}
