using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollGate.DTO.FeeRules;

namespace TollGateFeeLoader
{
    public interface IFeeService
    {
        List<TollYear> Load();
        void Save(String feeTable, TollYear theYear);
    }
}
