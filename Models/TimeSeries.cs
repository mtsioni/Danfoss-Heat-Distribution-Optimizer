
using System;
using System.Collections.Generic;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class TimeSeries<T>
    {
        private List<DateTime> _timestamps;
        private List<T> _values;
        private int _timeResolution;
        public int Length
        {
            get {return _values.Count;}
        }
        public T this[DateTime index]
        {
            get { return _values[_timestamps.IndexOf(index)];}
            set { _values[_timestamps.IndexOf(index)] = value;}
        }
        public TimeSeries()
        {
            _timestamps = new List<DateTime>();
            _values = new List<T>();
            _timeResolution = 1;
        } 
        public TimeSeries(List<DateTime> timeStamps, List<T> values)
        {
            _timestamps = timeStamps;
            _values = values;
            _timeResolution = 1;
        } 
        public List<DateTime> GetTimestamps()
        {
            return _timestamps; 
        }

        public int GetTimeResolution()
        {
            return _timeResolution;
        }
    }
}