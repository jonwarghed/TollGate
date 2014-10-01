using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using TollGateFeeLoader;
using System.Collections.Generic;
using Should;
using System.Threading.Tasks;

namespace TollGate.Specs
{
    
    
    public class EventdrivenTollGateSpec
    {
        [Fact(DisplayName = "Calculate a fee")]
        public void ACarThatPassesThroughTheGatesDuringDayShouldGetAFee()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
            var fee = tollGate.CalculateFee(new Car(), new System.Collections.Generic.List<PassedGateEvent>() { new PassedGateEvent(Guid.NewGuid(), DateTime.Now) });
            fee.Cost.ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact(DisplayName = "Async method should also calculate a fee")]
        public async Task ACarThatPassesThroughTheGatesDuringDayShouldGetAFeeAsync()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
            var fee = await tollGate.CalculateFeeAsync(new Car(), new System.Collections.Generic.List<PassedGateEvent>() { new PassedGateEvent(Guid.NewGuid(), DateTime.Now) });
            fee.Cost.ShouldBeGreaterThanOrEqualTo(0);
        }

        

        [Fact(DisplayName= "Calculate a positive fee on a date with fee")]
        public void PassingThroughTheGateWhenFeesAreActive()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
            var aCarWithFee = new Car();
            var aDateWithFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014,09,30,18,52,00));
            var fee = tollGate.CalculateFee(aCarWithFee, new System.Collections.Generic.List<PassedGateEvent>() { aDateWithFee });
            fee.Cost.ShouldBeGreaterThan(0);
        }

        [Fact(DisplayName = "Fee should be 16")]
        public void CheckFeeIsCorrectAmount()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
            var aCarWithFee = new Car();
            var aDateWithFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 18, 52, 00));
            var fee = tollGate.CalculateFee(aCarWithFee, new System.Collections.Generic.List<PassedGateEvent>() { aDateWithFee });
            fee.Cost.ShouldEqual(16);
        }

        [Fact(DisplayName = "Calculate same fee if passed by in rapid succession")]
        public void PassingThroughTheGateTwiceCloseBy()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
           
            var aCarWithFee = new Car();
            var aDayWithAFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 18, 52, 00));
            var anotherDateCloseBy = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 19, 07, 00));
            var singlePassList = new List<PassedGateEvent>() { aDayWithAFee };
            var doublePassList = new List<PassedGateEvent>() {aDayWithAFee,anotherDateCloseBy};
            var fee1 = tollGate.CalculateFee(aCarWithFee,singlePassList);
            var fee2 = tollGate.CalculateFee(aCarWithFee, doublePassList);
            fee1.Cost.ShouldBeGreaterThan(0);
            fee1.Cost.ShouldEqual(fee2.Cost);
        }

        [Fact(DisplayName = "Maximum fee should be 60")]
        public void MaximumFeeIsApplied()
        {
            //Arrange
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);
           
            var aCarWithFee = new Car();
            var night = new DateTime(2014, 09, 30, 00, 00, 00);
            var multiPassList = new List<PassedGateEvent>();            
            while(night.Month < 10)
            {
                multiPassList.Add(new PassedGateEvent(Guid.NewGuid(), night));
                night = night.AddMinutes(20);            
            }
            //Act
            var fee = tollGate.CalculateFee(aCarWithFee, multiPassList);

            fee.Cost.ShouldEqual(60);
        }

        [Fact(DisplayName = "Calculate added fee if passed by twice")]
        public void PassingThroughTheGateTwiceFarApart()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);

            var aCarWithFee = new Car();
            var aDayWithAFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 09, 52, 00));
            var anotherDateFurtherAway = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 19, 07, 00));
            var singlePassList = new List<PassedGateEvent>() { aDayWithAFee };
            var doublePassList = new List<PassedGateEvent>() { aDayWithAFee, anotherDateFurtherAway };
            var fee1 = tollGate.CalculateFee(aCarWithFee, singlePassList);
            var fee2 = tollGate.CalculateFee(aCarWithFee, doublePassList);
            fee1.Cost.ShouldBeGreaterThan(0);
            fee1.Cost.ShouldBeLessThan(fee2.Cost);
        }



        [Fact(DisplayName = "Calculate no fee on a date with is excempt")]
        public void PassingThroughTheGateAtExcemptDay()
        {
            var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);

            var aCarWithFee = new Car();
            var aDayWithoutAFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 12, 24, 18, 52, 00));
            var fee = tollGate.CalculateFee(aCarWithFee, new System.Collections.Generic.List<PassedGateEvent>() { aDayWithoutAFee });
            fee.Cost.ShouldEqual(0);
        }

        [Fact(DisplayName = "Tollfree vehicles should not get any fee")]
        public void TollfreeVehiclesGetNoCharge()
        {
             var loader = new JSONFileLoader();
            loader.Paths.Add("../../TestYear.json");
            var tollGate = new EventDrivenTollGate(loader);

            var ATollFreeVehicle = new Tractor();
            var aDayWithAFee = new PassedGateEvent(Guid.NewGuid(), new DateTime(2014, 09, 30, 15, 52, 00));
            var fee = tollGate.CalculateFee(ATollFreeVehicle, new System.Collections.Generic.List<PassedGateEvent>() { aDayWithAFee });
            fee.Cost.ShouldEqual(0);
        }
    }
}
