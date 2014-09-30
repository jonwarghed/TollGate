using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollGate
{
    public class TollGate
    {
        public int getTollFee(IVehicle vehicle, List<DateTime> dates)
        {
            var intervalStart = dates[0];
            int totalFee = 0;
            foreach (var date in dates) 
            {
              int nextFee = getTollFee(date, vehicle);
              int tempFee = getTollFee(intervalStart, vehicle);

              TimeSpan difference = date - intervalStart;
            
              if (difference.TotalMinutes <= 60) {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
              } else {
                totalFee += nextFee;
              }
            }
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        public int getTollFee(DateTime date, IVehicle vehicle)
        {
            if (isTollFreeDate(date) || isTollFreeVehicle(vehicle)) return 0;
            var hour = date.Hour;
            var minute = date.Minute;

            if (hour == 6 && minute >= 0 && minute <= 29) return 8;
            else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
            else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
            else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
            else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
            else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
            else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
            else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
            else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
            else return 0;
        }

        private bool isTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            String vehicleType = vehicle.getType();
            return Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.MOTORBIKE) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.TRACTOR) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.EMERGENCY) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.DIPLOMAT) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.FOREIGN) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.MILITARY) == vehicleType;
        }

        private bool isTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday) return true;

            if (year == 2013)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    enum TollFreeVehicles
    {
        MOTORBIKE,
        TRACTOR,
        EMERGENCY,
        DIPLOMAT,
        FOREIGN,
        MILITARY
    };
    
}