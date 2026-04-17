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
            _resultDataPath = $"Assets/ResultData/{unitName}.csv"; 
            
            string result = ConvertToCSV(data);

            File.WriteAllText(_resultDataPath, result);
        }

        
        public static string ConvertToCSV(List<ResultDataDTO> data)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("Time,HeatMWh,Electricity,ProductionCost,FuelConsumption,Co2Emissions,NetCost");

            foreach (var spec in data)
            {
                sb.AppendLine(
                    $"{spec.Time:dd.MM.yyyy HH:mm}," + // MM is Month, mm is minutes!
                    $"{spec.HeatMWh.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.Electricity.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.ProductionCost.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.FuelConsumption.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{spec.Co2Emissions.ToString(System.Globalization.CultureInfo.InvariantCulture)}," 
                    // $"{spec.NetCost.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
                );
            }

            return sb.ToString();
        }
    }
}