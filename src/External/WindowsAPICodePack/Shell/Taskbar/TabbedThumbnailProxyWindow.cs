// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal sealed class TabbedThumbnailProxyWindow : Form, IDisposable
    {
        private IntPtr proxyingFor;

        internal TabbedThumbnailProxyWindow(TabbedThumbnail preview)
        {
            TabbedThumbnail = preview;

            if (preview.WindowHandle != IntPtr.Zero)
            {
                proxyingFor = preview.WindowHandle;
                Size = new System.Drawing.Size(1, 1);

                // Try to get the window text so we can use it on the tabbed thumbnail as well
                StringBuilder text = new StringBuilder(256);
                TabbedThumbnailNativeMethods.GetWindowText(proxyingFor, text, text.Capacity);
                Text = text.ToString();
                
                // If we get a valid title from the GetWindowText method,
                // and also if the user hasn't set any title on the preview object,
                // then update the preview's title with what we get from GetWindowTitle
                if(!string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(preview.Title))
                    preview.Title = Text;
            }
            else if (preview.WindowsControl != null)
            {
                proxyingFor = IntPtr.Zero;
                WindowsControl = preview.WindowsControl;
                Size = new System.Drawing.Size(1, 1);
                // Since we can't get the text/caption for a UIElement, not setting this.Text here.

            }
        }

        internal TabbedThumbnail TabbedThumbnail
        {
            get;
            private set;
        }

        internal IntPtr RealWindow
        {
            get { return proxyingFor; }
        }

        internal UIElement WindowsControl
        {
            get;
            private set;
        }

        internal IntPtr WindowToTellTaskbarAbout
        {
            get { return this.Handle; }
        }

        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            if(this.TabbedThumbnail != null)
                handled = TaskbarWindowManager.Instance.DispatchMessage(ref m, this.TabbedThumbnail.TaskbarWindow);

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
        ~TabbedThumbnailProxyWindow()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            base.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (TabbedThumbnail != null)
                    TabbedThumbnail.Dispose();

                TabbedThumbnail = null;

                // 
                WindowsControl = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}