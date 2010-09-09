// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Windows;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal class TaskbarWindow : IDisposable
    {
        internal TabbedThumbnailProxyWindow TabbedThumbnailProxyWindow
        {
            get;
            set;
        }

        internal ThumbnailToolbarProxyWindow ThumbnailToolbarProxyWindow
        {
            get;
            set;
        }

        internal bool EnableTabbedThumbnails
        {
            get;
            set;
        }

        internal bool EnableThumbnailToolbars
        {
            get;
            set;
        }

        internal IntPtr UserWindowHandle
        {
            get;
            set;
        }

        internal UIElement WindowsControl
        {
            get;
            set;
        }

        private TabbedThumbnail tabbedThumbnailPreview = null;
        internal TabbedThumbnail TabbedThumbnail
        {
            get { return tabbedThumbnailPreview; }
            set
            {
                if (tabbedThumbnailPreview == null)
                {
                    TabbedThumbnailProxyWindow = new TabbedThumbnailProxyWindow(value);
                    tabbedThumbnailPreview = value;
                }
                else
                    throw new InvalidOperationException("Value is already set. It cannot be set more than once.");
            }
        }

        private ThumbnailToolbarButton[] thumbnailButtons;
        internal ThumbnailToolbarButton[] ThumbnailButtons
        {
            get { return thumbnailButtons; }
            set
            {
                thumbnailButtons = value;

                // Set the window handle on the buttons (for future updates)
                Array.ForEach(thumbnailButtons, new Action<ThumbnailToolbarButton>(UpdateHandle));
            }
        }

        private void UpdateHandle(ThumbnailToolbarButton button)
        {
            button.WindowHandle = WindowToTellTaskbarAbout;
            button.AddedToTaskbar = false;
        }

        internal IntPtr WindowToTellTaskbarAbout
        {
            get
            {
                if (EnableThumbnailToolbars && !EnableTabbedThumbnails && ThumbnailToolbarProxyWindow != null)
                    return ThumbnailToolbarProxyWindow.WindowToTellTaskbarAbout;
                else if (!EnableThumbnailToolbars && EnableTabbedThumbnails && TabbedThumbnailProxyWindow != null)
                    return TabbedThumbnailProxyWindow.WindowToTellTaskbarAbout;
                else if (EnableTabbedThumbnails && EnableThumbnailToolbars && TabbedThumbnailProxyWindow != null)
                    return TabbedThumbnailProxyWindow.WindowToTellTaskbarAbout;
                else
                    throw new InvalidOperationException();
            }
        }

        internal string Title
        {
            set
            {
                if (TabbedThumbnailProxyWindow != null)
                    TabbedThumbnailProxyWindow.Text = value;
                else
                    throw new InvalidOperationException();
            }
        }

        internal TaskbarWindow(IntPtr userWindowHandle, params ThumbnailToolbarButton[] buttons)
        {
            if (userWindowHandle == IntPtr.Zero)
                throw new ArgumentException("userWindowHandle");

            if (buttons == null || buttons.Length == 0)
                throw new ArgumentException("buttons");

            // Create our proxy window
            ThumbnailToolbarProxyWindow = new ThumbnailToolbarProxyWindow(userWindowHandle, buttons);
            ThumbnailToolbarProxyWindow.TaskbarWindow = this;

            // Set our current state
            EnableThumbnailToolbars = true;
            EnableTabbedThumbnails = false;

            //
            this.ThumbnailButtons = buttons;
            UserWindowHandle = userWindowHandle;
            WindowsControl = null;
        }

        internal TaskbarWindow(System.Windows.UIElement windowsControl, params ThumbnailToolbarButton[] buttons)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("windowsControl");

            if (buttons == null || buttons.Length == 0)
                throw new ArgumentException("buttons");

            // Create our proxy window
            ThumbnailToolbarProxyWindow = new ThumbnailToolbarProxyWindow(windowsControl, buttons);
            ThumbnailToolbarProxyWindow.TaskbarWindow = this;

            // Set our current state
            EnableThumbnailToolbars = true;
            EnableTabbedThumbnails = false;
            
            this.ThumbnailButtons = buttons;
            UserWindowHandle = IntPtr.Zero;
            WindowsControl = windowsControl;
        }

        internal TaskbarWindow(TabbedThumbnail preview)
        {
            if (preview == null)
                throw new ArgumentException("preview");

            // Create our proxy window
            TabbedThumbnailProxyWindow = new TabbedThumbnailProxyWindow(preview);

            // set our current state
            EnableThumbnailToolbars = false;
            EnableTabbedThumbnails = true;

            //
            UserWindowHandle = preview.WindowHandle;
            WindowsControl = preview.WindowsControl; 
            TabbedThumbnail = preview;
        }


        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
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

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (tabbedThumbnailPreview != null)
                    tabbedThumbnailPreview.Dispose();
                tabbedThumbnailPreview = null;

                if (ThumbnailToolbarProxyWindow != null)
                    ThumbnailToolbarProxyWindow.Dispose();
                ThumbnailToolbarProxyWindow = null;

                if (TabbedThumbnailProxyWindow != null)
                    TabbedThumbnailProxyWindow.Dispose();
                TabbedThumbnailProxyWindow = null;

                // Don't dispose the thumbnail buttons
                // as they might be used in another window.
                // Setting them to null will indicate we don't need use anymore.
                thumbnailButtons = null;
            }
        }

        #endregion
    }
}
