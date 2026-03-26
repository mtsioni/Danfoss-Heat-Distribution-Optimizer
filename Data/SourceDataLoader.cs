using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public static class SourceDataLoader
    {
        private static string _sourceDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", "SourceData.csv");
        private static TimeSeries<double> LoadSourceData(string arg)
        {
            if (string.IsNullOrEmpty(arg) || string.IsNullOrWhiteSpace(arg) || ((arg != "HeatDemand") && (arg != "ElectricityPrice")))
            {
                throw new Exception("Invalid argument");
            }
            if (!File.Exists(_sourceDataPath))
            {
                throw new Exception("File does not exist");
            }
            TimeSeries<double> SourceData = new TimeSeries<double>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (StreamReader _reader = new StreamReader(_sourceDataPath))
            {
                using (CsvReader _csvReader = new CsvReader(_reader, config))
                {
                    var optionsDate = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy HH:mm" }};
                    _csvReader.Context.TypeConverterOptionsCache.AddOptions<DateTime>(optionsDate);
                    if (arg == "HeatDemand")
                    {
                        _csvReader.Context.RegisterClassMap<SourceDataModelHeatDemandMap>();
                        var records = _csvReader.GetRecords<SourceDataModelHeatDemand>().ToList();
                        foreach (SourceDataModelHeatDemand line in records)
                        {
                            SourceData[line.WinterTimeStamp] = line.WinterValue;
                            SourceData[line.SummerTimeStamp] = line.SummerValue;
                            Console.WriteLine(SourceData[line.WinterTimeStamp]);
                        }
                    }
                    if (arg == "ElectricityPrice")
                    {
                        _csvReader.Context.RegisterClassMap<SourceDataModelElectricityPriceMap>();
                        var records = _csvReader.GetRecords<SourceDataModelElectricityPrice>().ToList();
                        foreach (SourceDataModelElectricityPrice line in records)
                        {
                            SourceData[line.WinterTimeStamp] = line.WinterValue;
                            SourceData[line.SummerTimeStamp] = line.SummerValue;
                        }
                    }
                }
                return SourceData;
            }
        }
        public static TimeSeries<double> LoadHeatDemand()
        {
            return LoadSourceData("HeatDemand");
        }
        public static TimeSeries<double> LoadElectricityPrices()
        {
            return LoadSourceData("ElectricityPrice");
        }
    }
}