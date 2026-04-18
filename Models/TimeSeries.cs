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

        // Compatibility helper for list-based iteration
        public List<DateTime> Timestamps => new List<DateTime>(_values.Keys);

        public T this[DateTime index]
        {
            get 
            {
                if (_values.TryGetValue(index, out T? value)) return value;
                return default!;
            }
            set { _values[index] = value; }
        }

        public TimeSeries()
        {
        } 

        public TimeSeries(List<DateTime> timeStamps, List<T> values)
        {
            if (timeStamps != null && values != null)
            {
                for (int i = 0; i < Math.Min(timeStamps.Count, values.Count); i++)
                {
                    _values[timeStamps[i]] = values[i];
                }
            }
        } 
    }
}