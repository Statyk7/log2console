using System;

namespace Log2Console.Log
{
  [Serializable]
  public enum LogLevel
  {
    None = -1,
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warn = 3,
    Error = 4,
    Fatal = 5,
  }
}
