using System;
using System.Collections.Generic;

namespace TollGate
{
    public interface ITollGate
    {
        /// <summary>
        /// Calculates the fee associated with passing through a gate
        /// </summary>
        /// <param name="date">The times a vehicle has passed a gate</param>
        /// <param name="vehicle">The vehicle in question</param>
        /// <returns>The toll fee for the passing a gate</returns>
        int getTollFee(DateTime date, IVehicle vehicle);

        /// <summary>
        /// Calculates the fee for multiple passings through the toll gates
        /// </summary>
        /// <param name="date">The times a vehicle has passed a gate</param>
        /// <param name="vehicle">The vehicle in question</param>
        /// <returns>The value of all toll fees in the speicifed span</returns>
        int getTollFee(IVehicle vehicle, System.Collections.Generic.List<DateTime> dates);
    }

    public interface IFeeCalculator
    {
        Fee CalculateFee(IVehicle vehicle, List<PassedGateEvent> passedGate);
    }

    public interface ILoaderEnabled
    {

    }
}
