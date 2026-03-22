
using System;
using System.Collections.Generic;


public class TimeSeries<T>
{
    private List<DateTime> _timestamps;
    private List<T> _values;
    private int _timeResolution; 

    public TimeSeries()
    {
        _timestamps = new List<DateTime>();
        _values = new List<T>();
        _timeResolution = 1;
    } 

    public TimeSeries(List<DateTime> date , List<T> value)
    {
        _timestamps = date;
        _values = value;
        _timeResolution = 1;
    } 

    public T? GetAt(DateTime date)
    {
        int index = _timestamps.IndexOf(date);
        
        return _values[index]; 
        
    }

    public int Length()
    {
        return _timestamps.Count;
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