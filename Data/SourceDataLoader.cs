using System;
using System.IO;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public static class SourceDataLoader
    {
        private static string _sourceDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", "SourceData.csv");
        private static StreamReader? _reader = null;
        private static TimeSeries<double> LoadData(int value1Index, int value2Index)
        {
            if (File.Exists(_sourceDataPath))
            {
                _reader = new StreamReader(File.OpenRead(_sourceDataPath));
                TimeSeries<double> sourceData = new TimeSeries<double>();
                while (!_reader.EndOfStream)
                {
                    string line = _reader.ReadLine() ?? ",,,,,,,,";
                    string[] lineValues = line.Split(',');
                    DateTime date;
                    double value;

                    date = DateTime.Parse(lineValues[0]);
                    value = double.Parse(lineValues[value1Index]);
                    sourceData.Timestamps.Add(date);
                    sourceData[date] = value;
                    
                    date = DateTime.Parse(lineValues[5]);
                    value = double.Parse(lineValues[value2Index]);
                    sourceData.Timestamps.Add(date);
                    sourceData[date] = value;
                }
                _reader.Close();
                return sourceData;
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }
        public static TimeSeries<double> LoadHeatDemand()
        {
            return LoadData(2, 7);
        }
        public static TimeSeries<double> LoadElectricityPrices()
        {
            return LoadData(3, 8);
        }
    }
}