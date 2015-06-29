// Decompiled with JetBrains decompiler
// Type: Log2Console.Settings.SourceFileLocation
// Assembly: Log2Console, Version=9.9.9.9, Culture=neutral, PublicKeyToken=null
// MVID: 44D35A25-C349-4FB9-B272-3AC90AA136EE
// Assembly location: C:\Users\rwahl\Desktop\Log2Console.exe

using System;
using System.ComponentModel;

namespace Log2Console.Settings
{
  [Serializable]
  public class SourceFileLocation
  {
    [DisplayName("Log File Source Code Path")]
    [Description("The Base Path of the Source Code in the Log File")]
    [Category("Source Location Mapping")]
    public string LogSource { get; set; }

    [Description("The Base Path of the Source Code on the Local Computer")]
    [Category("Source Location Mapping")]
    [DisplayName("Local Source Code Path")]
    public string LocalSource { get; set; }
  }
}
