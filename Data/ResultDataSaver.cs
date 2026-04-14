using System.Collections.Generic;
using System.IO;
using System.Text;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public class ResultDataSaver
    {
        private static string? _resultDataPath {get;set;}

        public static void SaveToCSV (string unitName, List<ResultDataDTO> data)
        {
            _resultDataPath = "Assets/ResultData/" + unitName; 
            
            string result = ConvertToCSV(data);

            File.WriteAllText(_resultDataPath, result);
        }

        public static string ConvertToCSV (List<ResultDataDTO> data)
        {
            string csv = "Time,HeatMWh,Electricity,ProductionCost,PrimaryEnergy,Co2Emissions\n";

            foreach(var spec in data)
            {
                csv +=
                    $"{spec.Time:dd.mm.yyyy HH:mm}," +
                    $"{spec.HeatMWh.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.Electricity.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.ProductionCost.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.PrimaryEnergy.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.Co2Emissions.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }

            return csv;
        }
    }
}
