// Decompiled with JetBrains decompiler
// Type: Log2Console.Settings.LayoutSettings
// Assembly: Log2Console, Version=9.9.9.9, Culture=neutral, PublicKeyToken=null
// MVID: 44D35A25-C349-4FB9-B272-3AC90AA136EE
// Assembly location: C:\Users\rwahl\Desktop\Log2Console.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Log2Console.Settings
{
  [Serializable]
  public sealed class LayoutSettings
  {
    public Rectangle WindowPosition { get; set; }

    public FormWindowState WindowState { get; set; }

    public bool ShowLogDetailView { get; set; }

    public Size LogDetailViewSize { get; set; }

    public bool ShowLoggerTree { get; set; }

    public Size LoggerTreeSize { get; set; }

    public int[] LogListViewColumnsWidths { get; set; }

    public void Set(Rectangle position, FormWindowState state, Control detailView, Control loggerTree)
    {
      this.WindowPosition = position;
      this.WindowState = state;
      this.ShowLogDetailView = detailView.Visible;
      this.LogDetailViewSize = detailView.Size;
      this.ShowLoggerTree = loggerTree.Visible;
      this.LoggerTreeSize = loggerTree.Size;
    }
  }
}
