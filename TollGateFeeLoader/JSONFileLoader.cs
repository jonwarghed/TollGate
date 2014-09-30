using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollGate.DTO.FeeRules;
using Newtonsoft.Json;
using System.IO;

namespace TollGateFeeLoader
{
    public class JSONFileLoader : IFeeService
    {
        public JSONFileLoader()
        {
            Paths = new List<string>();
        }

        public List<String> Paths {get;set;}
        
        /// <summary>
        /// Loads all the TollYears given in Paths
        /// </summary>
        /// <returns>A list of toll years</returns>
        public List<TollYear> Load()
        {
            List<TollYear> years = new List<TollYear>();
            foreach(var path in Paths )
            {
                var json = File.ReadAllText(path);
                years.Add(JsonConvert.DeserializeObject<TollYear>(json));
            }
            return years;
           
        }

        /// <summary>
        /// Used to generate basic json at the moment
        /// </summary>
        /// <param name="path"></param>
        /// <param name="theYear"></param>
        public void Save(string path, TollGate.DTO.FeeRules.TollYear theYear)
        {
            var json = JsonConvert.SerializeObject(theYear);
            File.WriteAllText(path, json);
        }
    }
}
