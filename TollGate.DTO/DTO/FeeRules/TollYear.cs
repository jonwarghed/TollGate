using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollGate.DTO.FeeRules
{
    public class TollYear
    {
        public TollWeek WeeklyTolls { get; set; }
        public List<TollFreeDate> ExcemptDates { get; set; }
    }
}
