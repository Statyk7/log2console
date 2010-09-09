//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Windows.Forms;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal class ThumbnailToolbarProxyWindow : NativeWindow, IDisposable
    {
        private ThumbnailToolbarButton[] thumbnailButtons;
        private IntPtr internalWindowHandle;

        internal System.Windows.UIElement WindowsControl
        {
            get;
            set;
        }

        internal IntPtr WindowToTellTaskbarAbout
        {
            get
            {
                if (internalWindowHandle == IntPtr.Zero)
                    return this.Handle;
                else
                    return internalWindowHandle;
            }
        }

        internal TaskbarWindow TaskbarWindow
        {
            get;
            set;
        }

        internal ThumbnailToolbarProxyWindow(IntPtr windowHandle, ThumbnailToolbarButton[] buttons)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("Window handle cannot be empty", "windowHandle");
            if (buttons != null && buttons.Length == 0)
                throw new ArgumentException("Null or empty arrays are not allowed.", "buttons");

            //
            internalWindowHandle = windowHandle;
            thumbnailButtons = buttons;

            // Set the window handle on the buttons (for future updates)
            Array.ForEach(thumbnailButtons, new Action<ThumbnailToolbarButton>(UpdateHandle));

            // Assign the window handle (coming from the user) to this native window
            // so we can intercept the window messages sent from the taskbar to this window.
            this.AssignHandle(windowHandle);
        }

        internal ThumbnailToolbarProxyWindow(System.Windows.UIElement windowsControl, ThumbnailToolbarButton[] buttons)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("Control cannot be null", "windowsControl");
            if (buttons != null && buttons.Length == 0)
                throw new ArgumentException("Null or empty arrays are not allowed.", "buttons");

            //
            internalWindowHandle = IntPtr.Zero;
            WindowsControl = windowsControl;
            thumbnailButtons = buttons;

            // Set the window handle on the buttons (for future updates)
            Array.ForEach(thumbnailButtons, new Action<ThumbnailToolbarButton>(UpdateHandle));
        }


        private void UpdateHandle(ThumbnailToolbarButton button)
        {
            button.WindowHandle = internalWindowHandle;
            button.AddedToTaskbar = false;
        }

        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            handled = TaskbarWindowManager.Instance.DispatchMessage(ref m, this.TaskbarWindow);

            // If it's a WM_Destroy message, then also forward it to the base class (our native window)
            if ((m.Msg == (int)TabbedThumbnailNativeMethods.WM_DESTROY) ||
               (m.Msg == (int)TabbedThumbnailNativeMethods.WM_NCDESTROY) ||
               ((m.Msg == (int)TabbedThumbnailNativeMethods.WM_SYSCOMMAND) && (((int)m.WParam) == TabbedThumbnailNativeMethods.SC_CLOSE)))
            {
                base.WndProc(ref m);
            }
            else if (!handled)
                base.WndProc(ref m);
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        ~ThumbnailToolbarProxyWindow()
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

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources

                // Don't dispose the thumbnail buttons
                // as they might be used in another window.
                // Setting them to null will indicate we don't need use anymore.
                thumbnailButtons = null;
            }
        }

        #endregion

    }
}
