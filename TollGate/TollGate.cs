using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollGate.DTO.DTO.Vehicles;
using TollGate.DTO.FeeRules;
using TollGateFeeLoader;

namespace TollGate
{
    public class TollGate : ITollGate
    {
        public int getTollFee(IVehicle vehicle, List<DateTime> dates)
        {
            var intervalStart = dates[0];
            int totalFee = 0;
            //The rules for the tollfee are hard to read
            foreach (var date in dates) 
            {
              int nextFee = getTollFee(date, vehicle);
              
              int tempFee = getTollFee(intervalStart, vehicle);

              //Should the difference be between 2 individual passing of a gate?
              TimeSpan difference = date - intervalStart;
            
              if (difference.TotalMinutes <= 60) {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
              } else {
                totalFee += nextFee;
              }
            }
            //Which interval has a maximum fee of 60?
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        public int getTollFee(DateTime date, IVehicle vehicle)
        {
            //These rules are coded as values, instead of using a backing value store
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
            //Why are we using an enum to gather information on what kind of car it is?
            return Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.MOTORBIKE) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.TRACTOR) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.EMERGENCY) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.DIPLOMAT) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.FOREIGN) == vehicleType ||
                  Enum.GetName(typeof(TollFreeVehicles), TollFreeVehicles.MILITARY) == vehicleType;
        }

        private bool isTollFreeDate(DateTime date)
        {
            //This is using hardcoded values
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

    public class EventDrivenTollGate : TollGate, IFeeCalculator, ILoaderEnabled
    {
        protected List<TollYear> tolls;
        public EventDrivenTollGate(IFeeService feeLoader)
        {
            tolls = feeLoader.Load();
        }

        public virtual Fee CalculateFee(IVehicle vehicle, List<PassedGateEvent> passedGate)
        {
            var state = CostCalculator.Zero();
            if (vehicle is ITollFreeVehicle)
                return new Fee() { Cost = 0 };
            foreach(var passing in passedGate)
            {
                state = Handle(state,passing);
            }
            return new Fee()
            {
                Cost = state.currentFee
            };
        }

        public virtual async Task<Fee> CalculateFeeAsync(IVehicle vehicle, List<PassedGateEvent> passedGate)
        {
            return CalculateFee(vehicle, passedGate);
        }

        /// <summary>
        /// Applies an gate event to the current state and returns a new state
        /// </summary>
        /// <param name="currentState">Current state for a vehicles fees</param>
        /// <param name="gateEvent">A passed gate event</param>
        /// <returns></returns>
        protected virtual CostCalculator Handle(CostCalculator currentState, PassedGateEvent gateEvent)
        {
            if (IsOnFreeDay(gateEvent))
            {
                return currentState;
            }
            if(TollHasAlreadyBeenApplied(currentState,gateEvent))
            {
                return currentState;
            }

            var fee = GetTollFee(gateEvent);
            if(fee != null)
            {
                currentState.currentFee = Math.Min(currentState.currentFee + fee.Toll, 60);
                currentState.lastEventThatGeneratedCost = gateEvent.Timestamp;
                return currentState;
            }
            //No positive or negative rule was found an applied.
            return currentState;
        }

        protected virtual bool TollHasAlreadyBeenApplied(CostCalculator currentState, PassedGateEvent passedGate)
        {
            if (!currentState.lastEventThatGeneratedCost.HasValue)
                return false;
            var difference = passedGate.Timestamp - currentState.lastEventThatGeneratedCost.Value;
            if (difference.TotalMinutes < 60)
                return true;
            return false;
        }

        protected virtual bool IsOnFreeDay(PassedGateEvent passedGate)
        {
            return tolls.Any(year => year.ExcemptDates.Any(excemption => excemption.Date.Date.Equals(passedGate.Timestamp.Date)));
        }

        protected virtual FeeTime GetTollFee(PassedGateEvent passedGate)
        {
            //Todo: Verify nullpointer exceptions here
            var feeTimes = tolls.SelectMany(year => year.WeeklyTolls.FeeTimes);
            return feeTimes.FirstOrDefault(
                    feeTime => feeTime.StartTime < passedGate.Timestamp.TimeOfDay &&
                    feeTime.EndTime > passedGate.Timestamp.TimeOfDay &&
                    feeTime.WeekDay == passedGate.Timestamp.DayOfWeek);
        }

        protected class CostCalculator
        {
            public int currentFee {get;set;}
            public DateTime? lastEventThatGeneratedCost { get; set; }
            
            private CostCalculator()
            {

            }

            public static CostCalculator Zero()
            {
                return new CostCalculator()
                {
                    currentFee = 0,
                    lastEventThatGeneratedCost = null
                };
            }
        }


    }
    
   
    
}