using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollGate.DTO.DTO.Vehicles;

namespace TollGate
{
    public class Car : IVehicle
    {

        public string getType()
        {
            return "CAR";
        }
    }

    public class Tractor: ITollFreeVehicle, IVehicle
    {

        public string getType()
        {
            throw new NotImplementedException();
        }
    }

    public class Emergency : ITollFreeVehicle, IVehicle
    {
        public string getType()
        {
            throw new NotImplementedException();
        }
    }

    public class Diplomat : ITollFreeVehicle, IVehicle
    {
        public string getType()
        {
            throw new NotImplementedException();
        }
    }

    public class Military : ITollFreeVehicle, IVehicle
    {
        public string getType()
        {
            throw new NotImplementedException();
        }
    }

    public class Foreign: ITollFreeVehicle, IVehicle
    {
        public string getType()
        {
            throw new NotImplementedException();
        }
    }
}
