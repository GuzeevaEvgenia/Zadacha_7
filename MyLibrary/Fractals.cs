using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class Fractals
    {
        public enum Type
        {
            Top, Bottom, None
        }

        public static (bool, Type) Calculate(List<Candle> candlesList, int currentIndex)
        {
            Candle current = candlesList[currentIndex];
            Candle prev1 = candlesList[currentIndex - 1],
                prev2 = candlesList[currentIndex - 2],
                next1 = candlesList[currentIndex + 1],
                next2 = candlesList[currentIndex + 2];
            if (current.High >= prev1.High && current.High >= prev2.High && current.High >= next1.High && current.High >= next2.High)
            {
                return (true, Type.Top);
            }
            else if (current.Low <= prev1.Low && current.Low <= prev2.Low && current.Low <= next1.Low && current.Low <= next2.Low)
            {
                return (true, Type.Bottom);
            }
            return (false, Type.None);
        }
    }
}
