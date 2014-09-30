using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollGate
{
    public class Car : IVehicle
    {
        public string getType()
        {
            return "Car";
        }
    }
}
