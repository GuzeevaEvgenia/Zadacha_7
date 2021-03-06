﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class Candle
    {
        public long TimeStamp { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public DateTime Date { get; set; }

        public Candle(DateTime date, long timeStamp, decimal open, decimal high, decimal low, decimal close) 
        {
            Date = date;
            TimeStamp = (int)timeStamp;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
    }
}
