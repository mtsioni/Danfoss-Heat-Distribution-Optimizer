
using System;
using System.Collections.Generic;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class TimeSeries<T>
    {
        private Dictionary<DateTime, T> _values = new();
        public Dictionary<DateTime, T> Values
        {
            get {return _values;}
            set {_values = value;}
        }
        public T this[DateTime index]
        {
            get { return _values[index];}
            set { _values[index] = value;}
        }
        /*
        private List<DateTime> _timestamps;
        public List<DateTime> Timestamps
        {
            get { return _timestamps; }
            set { _timestamps = value; }
        }
        private List<T> _values;
        public List<T> Values
        {
            get { return _values; }
            set { _values = value; }
        }
        private int _timeResolution;
        public int TimeResolution
        {
            get { return _timeResolution; }
            set { _timeResolution = value; }
        }
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
        */
    }
}