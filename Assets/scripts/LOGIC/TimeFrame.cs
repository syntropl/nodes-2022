using System.Collections;
using System.Collections.Generic;
using System;

public class TimeFrame
{
    public DateTime start;
    public DateTime end;


    private long ticks;
    public long Ticks
    {
        get
        {
            return end.Ticks - start.Ticks;
        }
    }

    private int daySpan;
    public int DaySpan
    {
        get
        {
            DateTime d = new DateTime(end.Ticks - start.Ticks);
            long ticksInADay = 864000000000;
            return (int)(d.Ticks / ticksInADay);
        }
    }

    public void Set(DateTime starting, DateTime ending)
    {
        start = starting;
        end = ending;
    }

    public TimeFrame(DateTime starting, DateTime ending)
    {
        Set(starting, ending);
    }


    private string dateString;
    public string DateString
    {
        get
        {
            if (DaySpan > 1)
            { 
                return $"{start.Year}-{start.Month}-{start.Day} - {end.Year}-{end.Month}-{end.Day}";
            }
            else
            {
                return $"{start.Year}-{start.Month}-{start.Day}";
            }
        }
    }


}
