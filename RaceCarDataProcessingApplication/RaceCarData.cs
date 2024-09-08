using System;

namespace Data
{
    public class RaceCarData
    {
        public double Time { get; set; }
        public double Value { get; set; }
        public int Outing { get; set; }
        public int Channel { get; set; }

        public RaceCarData(double time, double value, int outing, int channel)
        {
            Time = time;
            Value = value;
            Outing = outing;
            Channel = channel;
        }
    }
}