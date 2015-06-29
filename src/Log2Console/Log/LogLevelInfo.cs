using System;
using System.Drawing;

namespace Log2Console.Log
{
    [Serializable]
    public class LogLevelInfo
    {
        public LogLevel Level;
        public string Name;
        public Color Color;
        public int Value;
        public int RangeMin;
        public int RangeMax;


        public LogLevelInfo(LogLevel level, Color color)
        {
            Level = level;
            Name = level.ToString();
            Color = color;
            RangeMax = RangeMin = 0;
        }

        public LogLevelInfo(LogLevel level, string name, Color color, int value, int rangeMin, int rangeMax)
        {
            Level = level;
            Name = name;
            Color = color;
            Value = value;
            RangeMin = rangeMin;
            RangeMax = rangeMax;
        }

        public override bool Equals(object obj)
        {
            if (obj is LogLevelInfo)
                return ((obj as LogLevelInfo) == this);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(LogLevelInfo first, LogLevelInfo second)
        {
            if (((object)first == null) || ((object)second == null))
                return (((object)first == null) && ((object)second == null));
            return (first.Value == second.Value);
        }

        public static bool operator !=(LogLevelInfo first, LogLevelInfo second)
        {
            if (((object)first == null) || ((object)second == null))
                return !(((object)first == null) && ((object)second == null));
            return first.Value != second.Value;
        }
    }

}
