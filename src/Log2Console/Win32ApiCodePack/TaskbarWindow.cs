using System;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Microsoft.WindowsAPICodePack.Internal
{
  internal class TaskbarWindow : IDisposable
  {
    public TaskbarWindow(IntPtr userWindowHandle, ThumbnailToolbarButton[] buttons)
    {
      if (userWindowHandle == IntPtr.Zero)
        throw new ArgumentException("userWindowHandle");

      if (buttons == null || buttons.Length == 0)
        throw new ArgumentException("buttons");

      // Create our proxy window
      _thumbnailToolbarProxyWindow = new ThumbnailToolbarProxyWindow(userWindowHandle, buttons) { TaskbarWindow = this };

      // Set our current state
      ThumbnailButtons = buttons;
      WindowHandle = userWindowHandle;
    }

    ThumbnailToolbarProxyWindow _thumbnailToolbarProxyWindow;

    public readonly IntPtr WindowHandle;

    #region ThumbnailButtons Property

    ThumbnailToolbarButton[] _thumbnailButtons;
    public ThumbnailToolbarButton[] ThumbnailButtons
    {
      get { return _thumbnailButtons; }
      set
      {
        _thumbnailButtons = value;
        // Set the window handle on the buttons (for future updates)
        Array.ForEach(_thumbnailButtons, UpdateHandle);
      }
    }

    void UpdateHandle(ThumbnailToolbarButton button)
    {
      button.WindowHandle = _thumbnailToolbarProxyWindow.WindowHandle;
      button.AddedToTaskbar = false;
    }

    #endregion

    #region IDisposable Members

    ~TaskbarWindow()
    {
      Dispose(false);
    }

    /// <summary>
    /// Release the native objects.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
      if (!disposing) return;

      if (_thumbnailToolbarProxyWindow != null)
        _thumbnailToolbarProxyWindow.Dispose();
      _thumbnailToolbarProxyWindow = null;

      // Don't dispose the thumbnail buttons
      // as they might be used in another window.
      // Setting them to null will indicate we don't need use anymore.
      ThumbnailButtons = null;
    }

    #endregion
  }
}
