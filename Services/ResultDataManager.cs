using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using System;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using CsvHelper.Configuration.Attributes;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class ResultDataManager
    {
        public static DateTime OptimizationPeriodStart {get;set;}
        public static DateTime OptimizationPeriodEnd {get;set;}
        public static int TimeResolution {get;set;}
        private static List<IOptimizedUnit>? _resultData {get;set;}
        private static ResultDataSaver? _dataSaver {get;set;}

        private static void SaveResultData()
        {
            
        }

        public static TimeSeries<IOptimizedUnit> GetResultData()
        {
            TimeSeries<IOptimizedUnit> ResultData = new TimeSeries<IOptimizedUnit>();

            return ResultData;
        }
    }
}