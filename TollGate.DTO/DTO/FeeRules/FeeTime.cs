using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollGate.DTO.FeeRules
{
    public class FeeTime
    {
        public TimeSpan StartTime { get; set;}
        public TimeSpan EndTime { get; set; }
        public int Toll { get; set; }
        public DayOfWeek WeekDay { get; set; }
    }
}
