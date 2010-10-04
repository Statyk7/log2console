using System;
using Microsoft.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
  public sealed class ThumbnailToolbarManager
  {
    internal ThumbnailToolbarManager() { }

    public void AddButtons(IntPtr windowHandle, params ThumbnailToolbarButton[] buttons)
    {
      if (windowHandle == IntPtr.Zero)
        throw new ArgumentException("Window handle cannot be empty", "windowHandle");
      
      if (buttons == null || buttons.Length == 0)
        throw new ArgumentException("Null or empty arrays are not allowed.", "buttons");

      if (buttons.Length > 7)
        throw new ArgumentException("Maximum number of buttons allowed is 7.", "buttons");

      // Add the buttons to our window manager, which will also create a proxy window
      TaskbarWindowManager.Instance.AddThumbnailButtons(windowHandle, buttons);
    }
  }
}
