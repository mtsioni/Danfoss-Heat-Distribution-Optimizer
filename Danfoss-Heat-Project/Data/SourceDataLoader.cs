using System;
using System.Globalization; // used for CultureInfo
using System.IO; // FileStream
using System.Linq; // ToList
using CsvHelper; // CsvReader
using CsvHelper.Configuration; // CsvConfig
using CsvHelper.TypeConversion; // Type conversion for DateTime
using Danfoss_Heat_Distribution_Optimizer.Models;
// this class has capabilities to read SourceData from a csv file and return, but not store that data
namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public static class SourceDataLoader
    {
        // this string shows a path to a csv file, will be replaced with a reference to paths.json in the future
        private static string _sourceDataPath = GetSourceDataPath();

        private static string GetSourceDataPath()
        {
            // Try current directory (running from root)
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "SourceData.csv");
            if (File.Exists(rootPath)) return rootPath;

            // Try going up from bin/Debug/...
            string debugPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", "SourceData.csv");
            if (File.Exists(debugPath)) return debugPath;

            return rootPath; // fallback
        }
        // this method takes input string as a parameter that tells it what data to load
        private static TimeSeries<double> LoadSourceData(string arg)
        {
            // check that argument is valid
            if (string.IsNullOrEmpty(arg) || string.IsNullOrWhiteSpace(arg) || ((arg != "HeatDemand") && (arg != "ElectricityPrice")))
            {
                throw new Exception("Invalid argument");
            }
            // check that path is valid
            if (!File.Exists(_sourceDataPath))
            {
                throw new Exception("File does not exist");
            }
            // this is the return variable
            TimeSeries<double> SourceData = new TimeSeries<double>();
            // this is a thing from CsvHelper library that give it some instructions
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) // culture info reffers to stuff like date format, invariant is meant to be the universal adaptive one
            {
                HasHeaderRecord = false, // this tells it that csv file starts with values without any overhead markers telling what is what
            };
            using (StreamReader _reader = new StreamReader(_sourceDataPath)) // StreamReader is part of System.IO, it is a filestream manager that opens and closes the file
            {
                using (CsvReader _csvReader = new CsvReader(_reader, config)) // CsvReader is part of CsvHelper
                {
                    // this is configuration for how DateTime looks in a csv file, part of the csvHelper
                    var optionsDate = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy HH:mm" }};
                    _csvReader.Context.TypeConverterOptionsCache.AddOptions<DateTime>(optionsDate); // add the configuration to the csvReader
                    if (arg == "HeatDemand")
                    {
                        _csvReader.Context.RegisterClassMap<SourceDataModelHeatDemandMap>(); // adds the map to the reader
                        var records = _csvReader.GetRecords<SourceDataModelHeatDemand>().ToList(); // reads the data, ToList is used because of how csvReader works
                        // CsvReader is using Yield to give data, what Yield effectively means is that whatever data it gives is never stored, that's why ToList is used
                        foreach (SourceDataModelHeatDemand line in records) // process the data into the return variable
                        {
                            SourceData[line.WinterTimeStamp] = line.WinterValue;
                            SourceData[line.SummerTimeStamp] = line.SummerValue;
                        }
                    }
                    if (arg == "ElectricityPrice") // Same as above but uses different map and model, I would want to extract this into a separate function, but idk how to pass data type as an argument
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