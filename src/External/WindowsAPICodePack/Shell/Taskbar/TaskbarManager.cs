//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Interop;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents an instance of the Windows taskbar
    /// </summary>
    public class TaskbarManager
    {
        // Hide the default constructor
        private TaskbarManager()
        {

        }

        // Best practice recommends defining a private object to lock on
        private static Object syncLock = new Object();

        private static volatile TaskbarManager instance;
        /// <summary>
        /// Represents an instance of the Windows Taskbar
        /// </summary>
        public static TaskbarManager Instance
        {
            get
            {
                CoreHelpers.ThrowIfNotWin7();

                if (instance == null)
                {
                    lock (syncLock)
                    {
                        if (instance == null)
                            instance = new TaskbarManager();
                    }
                }

                return instance;
            }
        }

        // Internal implemenation of ITaskbarList4 interface
        private ITaskbarList4 taskbarList;
        internal ITaskbarList4 TaskbarList
        {
            get
            {
                if (taskbarList == null)
                {
                    // Create a new instance of ITaskbarList3
                    lock (syncLock)
                    {
                        if (taskbarList == null)
                        {
                            taskbarList = (ITaskbarList4)new CTaskbarList();
                            taskbarList.HrInit();
                        }
                    }
                }

                return taskbarList;
            }
        }

        /// <summary>
        /// Applies an overlay to a taskbar button of the main application window to indicate application status or a notification to the user.
        /// </summary>
        /// <param name="icon">The overlay icon</param>
        /// <param name="accessibilityText">String that provides an alt text version of the information conveyed by the overlay, for accessibility purposes</param>
        public void SetOverlayIcon(System.Drawing.Icon icon, string accessibilityText)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetOverlayIcon(OwnerHandle, icon != null ? icon.Handle : IntPtr.Zero, accessibilityText);
        }

        /// <summary>
        /// Applies an overlay to a taskbar button of the given window handle to indicate application status or a notification to the user.
        /// </summary>
        /// <param name="windowHandle">The handle of the window whose associated taskbar button receives the overlay. This handle must belong to a calling process associated with the button's application and must be a valid HWND or the call is ignored.</param>
        /// <param name="icon">The overlay icon</param>
        /// <param name="accessibilityText">String that provides an alt text version of the information conveyed by the overlay, for accessibility purposes</param>
        public void SetOverlayIcon(IntPtr windowHandle, System.Drawing.Icon icon, string accessibilityText)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetOverlayIcon(windowHandle, icon != null ? icon.Handle : IntPtr.Zero, accessibilityText);
        }

        /// <summary>
        /// Applies an overlay to a taskbar button of the given WPF window to indicate application status or a notification to the user.
        /// </summary>
        /// <param name="window">The window whose associated taskbar button receives the overlay. This window belong to a calling process associated with the button's application and must be already loaded.</param>
        /// <param name="icon">The overlay icon</param>
        /// <param name="accessibilityText">String that provides an alt text version of the information conveyed by the overlay, for accessibility purposes</param>
        public void SetOverlayIcon(System.Windows.Window window, System.Drawing.Icon icon, string accessibilityText)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetOverlayIcon(
                (new WindowInteropHelper(window)).Handle,
                icon != null ? icon.Handle : IntPtr.Zero,
                accessibilityText);
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button of the main application window 
        /// to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="currentValue">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
        /// <param name="maximumValue">An application-defined value that specifies the value currentValue will have when the operation is complete.</param>
        public void SetProgressValue(int currentValue, int maximumValue)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressValue(OwnerHandle, Convert.ToUInt32(currentValue), Convert.ToUInt32(maximumValue));
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button of the given window handle 
        /// to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="windowHandle">The handle of the window whose associated taskbar button is being used as a progress indicator.
        /// This window belong to a calling process associated with the button's application and must be already loaded.</param>
        /// <param name="currentValue">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
        /// <param name="maximumValue">An application-defined value that specifies the value currentValue will have when the operation is complete.</param>
        public void SetProgressValue(int currentValue, int maximumValue, IntPtr windowHandle)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressValue(windowHandle, Convert.ToUInt32(currentValue), Convert.ToUInt32(maximumValue));
        }

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button of the given WPF window 
        /// to show the specific percentage completed of the full operation.
        /// </summary>
        /// <param name="window">The window whose associated taskbar button is being used as a progress indicator. 
        /// This window belong to a calling process associated with the button's application and must be already loaded.</param>
        /// <param name="currentValue">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
        /// <param name="maximumValue">An application-defined value that specifies the value currentValue will have when the operation is complete.</param>
        public void SetProgressValue(int currentValue, int maximumValue, System.Windows.Window window)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressValue(
                (new WindowInteropHelper(window)).Handle,
                Convert.ToUInt32(currentValue),
                Convert.ToUInt32(maximumValue));
        }

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button of the main application window.
        /// </summary>
        /// <param name="state">Progress state of the progress button</param>
        public void SetProgressState(TaskbarProgressBarState state)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressState(OwnerHandle, (TBPFLAG)state);
        }

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button 
        /// of the given window handle 
        /// </summary>
        /// <param name="windowHandle">The handle of the window whose associated taskbar button is being used as a progress indicator.
        /// This window belong to a calling process associated with the button's application and must be already loaded.</param>
        /// <param name="state">Progress state of the progress button</param>
        public void SetProgressState(TaskbarProgressBarState state, IntPtr windowHandle)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressState(windowHandle, (TBPFLAG)state);
        }

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button 
        /// of the given WPF window
        /// </summary>
        /// <param name="window">The window whose associated taskbar button is being used as a progress indicator. 
        /// This window belong to a calling process associated with the button's application and must be already loaded.</param>
        /// <param name="state">Progress state of the progress button</param>
        public void SetProgressState(TaskbarProgressBarState state, System.Windows.Window window)
        {
            CoreHelpers.ThrowIfNotWin7();

            TaskbarList.SetProgressState(
                (new WindowInteropHelper(window)).Handle,
                (TBPFLAG)state);
        }

        private TabbedThumbnailManager tabbedThumbnail;
        /// <summary>
        /// Gets the Tabbed Thumbnail manager class for adding/updating
        /// tabbed thumbnail previews.
        /// </summary>
        public TabbedThumbnailManager TabbedThumbnail
        {
            get
            {
                CoreHelpers.ThrowIfNotWin7();

                if (tabbedThumbnail == null)
                    tabbedThumbnail = new TabbedThumbnailManager();

                return tabbedThumbnail;
            }
        }

        private ThumbnailToolbarManager thumbnailToolbarManager;
        /// <summary>
        /// Gets the Thumbnail toolbar manager class for adding/updating
        /// toolbar buttons.
        /// </summary>
        public ThumbnailToolbarManager ThumbnailToolbars
        {
            get
            {
                CoreHelpers.ThrowIfNotWin7();

                if (thumbnailToolbarManager == null)
                    thumbnailToolbarManager = new ThumbnailToolbarManager();

                return thumbnailToolbarManager;
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
                else
                {
                    SetCurrentProcessAppId(value);
                    ApplicationIdSetProcessWide = true;
                }
            }
        }

        private IntPtr ownerHandle;
        /// <summary>
        /// Sets the handle of the window whose taskbar button will be used
        /// to display progress.
        /// </summary>
        internal IntPtr OwnerHandle
        {
            get
            {
                if (ownerHandle == IntPtr.Zero)
                {
                    Process currentProcess = Process.GetCurrentProcess();

                    if (currentProcess != null && currentProcess.MainWindowHandle != IntPtr.Zero)
                        ownerHandle = currentProcess.MainWindowHandle;
                    else
                        throw new InvalidOperationException("A valid active Window is needed to update the Taskbar");
                }

                return ownerHandle;
            }
        }

        /// <summary>
        /// Sets the application user model id for an individual window
        /// </summary>
        /// <param name="appId">The app id to set</param>
        /// <param name="windowHandle">Window handle for the window that needs a specific application id</param>
        /// <remarks>AppId specifies a unique Application User Model ID (AppID) for the application or individual 
        /// top-level window whose taskbar button will hold the custom JumpList built through the methods <see cref="Microsoft.WindowsAPICodePack.Taskbar.JumpList"/> class.
        /// By setting an appId for a specific window, the window will not be grouped with it's parent window/application. Instead it will have it's own taskbar button.</remarks>
        public void SetApplicationIdForSpecificWindow(IntPtr windowHandle, string appId)
        {
            TaskbarNativeMethods.SetWindowAppId(windowHandle, appId);
        }

        /// <summary>
        /// Sets the application user model id for a given window
        /// </summary>
        /// <param name="appId">The app id to set</param>
        /// <param name="window">Window that needs a specific application id</param>
        /// <remarks>AppId specifies a unique Application User Model ID (AppID) for the application or individual 
        /// top-level window whose taskbar button will hold the custom JumpList built through the methods <see cref="Microsoft.WindowsAPICodePack.Taskbar.JumpList"/> class.
        /// By setting an appId for a specific window, the window will not be grouped with it's parent window/application. Instead it will have it's own taskbar button.</remarks>
        public void SetApplicationIdForSpecificWindow(System.Windows.Window window, string appId)
        {
            TaskbarNativeMethods.SetWindowAppId((new WindowInteropHelper(window)).Handle, appId);
        }

        /// <summary>
        /// Sets the current process' explicit application user model id.
        /// </summary>
        /// <param name="appId">The application id.</param>
        private void SetCurrentProcessAppId(string appId)
        {
            TaskbarNativeMethods.SetCurrentProcessExplicitAppUserModelID(appId);
        }

        /// <summary>
        /// Gets the current process' explicit application user model id.
        /// </summary>
        /// <returns>The app id or null if no app id has been defined.</returns>
        private string GetCurrentProcessAppId()
        {
            string appId = string.Empty;
            TaskbarNativeMethods.GetCurrentProcessExplicitAppUserModelID(out appId);
            return appId;
        }

        /// <summary>
        /// Indicates if the user has set the application id for the whole process (all windows)
        /// </summary>
        internal bool ApplicationIdSetProcessWide
        {
            get;
            private set;
        }


        /// <summary>
        /// Indicates whether this feature is supported on the current platform.
        /// </summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows 7 onwards ...
                return CoreHelpers.RunningOnWin7;
            }
        }

    }
}
