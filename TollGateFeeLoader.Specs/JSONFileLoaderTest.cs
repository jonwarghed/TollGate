using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TollGate.DTO.FeeRules;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Should;

namespace TollGateFeeLoader.Specs
{
   
    public class JSONFileLoaderTest
    {
        [Fact(DisplayName = "Serialization works")]
        public void CanSaveFile()
        {
            //Arrange
            var Year = new TollYear()
            {
                ExcemptDates = new System.Collections.Generic.List<TollFreeDate>
                {
                    new TollFreeDate()
                    {
                        Date = new DateTime(2014,12,24)
                    }
                },
                WeeklyTolls = new TollWeek()
                {
                    FeeTimes = new List<FeeTime>
                    {
                        new FeeTime
                        {
                            StartTime = new TimeSpan(08,00,00),
                            EndTime = new TimeSpan(18,00,00),
                            Toll = 16,
                            WeekDay = DayOfWeek.Monday
                        }
                    }
                }
            };


            //Act
            var loader = new JSONFileLoader();
            loader.Save("TestYear.json", Year);           
            //Assert
            //Thrown Exceptions fail the test        
            
        }

        [Fact(DisplayName="Deserialization works")]
        public void CanLoadFile()
        {
            //TODO: Add dependency that the first test is always run before this one
            //Arrange
            var loader = new JSONFileLoader();
            loader.Paths.Add("TestYear.json");
            //Act           
            var years = loader.Load();
            //Assert
            years.ShouldNotBeNull();
            years.Any().ShouldBeTrue();
        }
    }
}
