using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollGate.DTO.DTO.Vehicles;

namespace TollGate
{
    public class Motorcycle: ITollFreeVehicle, IVehicle
    {
        public new string getType()
        {
            return "MOTORCYCLE";
        }
    }
}
