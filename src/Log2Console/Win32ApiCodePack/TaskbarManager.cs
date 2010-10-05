using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
  public sealed class TaskbarManager
  {
    static readonly object Lock = new object();
    static volatile TaskbarManager _instance;

    TaskbarManager() { }

    /// <summary>
    /// Indicates whether this feature is supported on the current platform.
    /// </summary>
    public static bool IsPlatformSupported
    {
      get { return CoreHelpers.RunningOnWin7; }
    }

    ThumbnailToolbarManager _thumbnailToolbarManager;
    /// <summary>
    /// Gets the Thumbnail toolbar manager class for adding/updating
    /// toolbar buttons.
    /// </summary>
    public ThumbnailToolbarManager ThumbnailToolbars
    {
      get
      {
        CoreHelpers.ThrowIfNotWin7();

        return _thumbnailToolbarManager ?? (_thumbnailToolbarManager = new ThumbnailToolbarManager());
      }
    }

    /// <summary>
    /// Represents an instance of the Windows Taskbar
    /// </summary>
    public static TaskbarManager Instance
    {
      get
      {
        CoreHelpers.ThrowIfNotWin7();

        if (_instance == null)
          lock (Lock)
          {
            if (_instance == null)
              _instance = new TaskbarManager();
          }

        return _instance;
      }
    }

    /// <summary>
    /// Gets or sets the application user model id. Use this to explicitly
    /// set the application id when generating custom jump lists
    /// </summary>
    public string ApplicationId
    {
      get
      {
        CoreHelpers.ThrowIfNotWin7();

        return GetCurrentProcessAppId();
      }
      set
      {
        CoreHelpers.ThrowIfNotWin7();

        if (string.IsNullOrEmpty(value))
          throw new ArgumentNullException("value", "Application Id cannot be an empty or null string.");

        SetCurrentProcessAppId(value);
      }
    }

    static void SetCurrentProcessAppId(string value)
    {
      TaskbarNativeMethods.SetCurrentProcessExplicitAppUserModelID(value);
    }

    static string GetCurrentProcessAppId()
    {
      string appId;
      TaskbarNativeMethods.GetCurrentProcessExplicitAppUserModelID(out appId);
      return appId;
    }

    IntPtr _ownerHandle;
    /// <summary>
    /// Sets the handle of the window whose taskbar button will be used
    /// to display progress.
    /// </summary>
    IntPtr OwnerHandle
    {
      get
      {
        if (_ownerHandle == IntPtr.Zero)
        {
          var currentProcess = Process.GetCurrentProcess();

          if (currentProcess.MainWindowHandle != IntPtr.Zero)
            _ownerHandle = currentProcess.MainWindowHandle;
        }

        return _ownerHandle;
      }
    }

    // Internal implemenation of ITaskbarList4 interface
    ITaskbarList4 _taskbarList;
    internal ITaskbarList4 TaskbarList
    {
      get
      {
        if (_taskbarList == null)
          lock (Lock)
          {
            if (_taskbarList == null)
            {
              _taskbarList = (ITaskbarList4)new CTaskbarList();
              _taskbarList.HrInit();
            }
          }

        return _taskbarList;
      }
    }


    public void SetProgressState(TaskbarProgressBarState state)
    {
      CoreHelpers.ThrowIfNotWin7();

      if (OwnerHandle != IntPtr.Zero)
        TaskbarList.SetProgressState(OwnerHandle, (TBPFLAG)state);
    }

    public void SetOverlayIcon(Icon icon, string accessibilityText)
    {
      CoreHelpers.ThrowIfNotWin7();

      if (OwnerHandle != IntPtr.Zero)
        TaskbarList.SetOverlayIcon(OwnerHandle, icon != null ? icon.Handle : IntPtr.Zero, accessibilityText);
    }
  }
}
