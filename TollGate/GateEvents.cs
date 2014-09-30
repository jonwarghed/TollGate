using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollGate
{
    public class GateEvent
    {
        public GateEvent(Guid id)
        {
            ID = id;
        }
        public Guid ID;
    }

    public class PassedGateEvent : GateEvent
    {
        public PassedGateEvent(Guid id, DateTime timestamp) : base(id)
        {
            Timestamp = timestamp;
        }

        public DateTime Timestamp { get; set; }
    }
}
