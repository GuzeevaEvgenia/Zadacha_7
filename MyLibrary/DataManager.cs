using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using Newtonsoft.Json;

namespace MyLibrary
{
    public class DataManager
    {
        public static Queue<Candle> GetCandlesFromXLSX(string fileName)
        {
            Queue<Candle> candles = new Queue<Candle>();
            List<Candle> candlesTemp = new List<Candle>();
            var workbook = new XLWorkbook(fileName);
            var worksheet = workbook.Worksheet(1);
            for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                string d = worksheet.Cell(row, 3).GetValue<string>(); //Date
                DateTime date = DateTime.ParseExact(d, "yyyyMMdd", CultureInfo.InvariantCulture);
                Candle candle = new Candle(
                    date,
                    worksheet.Cell(row, 4).GetValue<long>(),        //TimeStamp
                    worksheet.Cell(row, 5).GetValue<decimal>(),     //Open
                    worksheet.Cell(row, 6).GetValue<decimal>(),     //High
                    worksheet.Cell(row, 7).GetValue<decimal>(),     //Low
                    worksheet.Cell(row, 8).GetValue<decimal>()      //Close
                );
                candles.Enqueue(candle);
            }
            return candles;
        }

        public static Queue<Candle> GetCandlesFromJSON(string filepath)
        {
            Queue<Candle> candles = new Queue<Candle>();
            var text = File.ReadAllText(filepath);
            dynamic jsonDe = JsonConvert.DeserializeObject(text);

            List<decimal> open = jsonDe.o.ToObject<List<decimal>>();
            List<decimal> high = jsonDe.h.ToObject<List<decimal>>();
            List<decimal> low = jsonDe.l.ToObject<List<decimal>>();
            List<decimal> close = jsonDe.c.ToObject<List<decimal>>();
            List<long> timestamp = jsonDe.t.ToObject<List<long>>();

            for (int i = 0; i < open.Count; i++)
            {
                Candle candle = new Candle(
                    new DateTime(),
                    timestamp[i],        //TimeStamp
                    open[i],             //Open
                    high[i],             //High
                    low[i],              //Low
                    close[i]             //Close
                );
                candles.Enqueue(candle);
            }

            return candles;
        }
    }
}
