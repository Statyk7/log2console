//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal class TabbedThumbnailNativeMethods
    {
        internal const int DWM_SIT_DISPLAYFRAME = 0x00000001;

        internal const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
        internal const int DWMWA_HAS_ICONIC_BITMAP = 10;

        internal const uint WM_ACTIVATEAPP = 0x001C;
        internal const uint WM_CREATE = 0x1;
        internal const uint WM_DESTROY = 0x2;
        internal const uint WM_NCDESTROY = 0x0082;
        internal const uint WM_ACTIVATE = 0x0006;
        internal const uint WM_CLOSE = 0x0010;
        internal const uint WM_SYSCOMMAND = 0x112;
        internal const uint WM_DWMSENDICONICTHUMBNAIL = 0x0323;
        internal const uint WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;

        internal const uint WA_ACTIVE = 1;
        internal const uint WA_CLICKACTIVE = 2;

        internal const int SC_CLOSE = 0xF060;
        internal const int SC_MAXIMIZE = 0xF030;
        internal const int SC_MINIMIZE = 0xF020;

        internal const uint MSGFLT_ADD = 1;
        internal const uint MSGFLT_REMOVE = 2;



        private TabbedThumbnailNativeMethods()
        {
            // Hide the default constructor as this is a static class and we don't want to instantiate it.
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicThumbnail(
            IntPtr hwnd, IntPtr hbitmap, uint flags);

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(
            IntPtr hwnd, StringBuilder str, int maxCount);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicLivePreviewBitmap(
            IntPtr hwnd,
            IntPtr hbitmap,
            ref CoreNativeMethods.POINT ptClient,
            uint flags);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicLivePreviewBitmap(
            IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, uint flags);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = true)]
        internal static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            //DWMWA_* values.
            uint dwAttributeToSet,
            IntPtr pvAttributeValue,
            uint cbAttribute);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref CoreNativeMethods.RECT rect);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
           int nBottomRect);

        [DllImport("user32.dll")]
        internal static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hwnd, ref CoreNativeMethods.RECT rect);

        internal static bool GetClientSize(IntPtr hwnd, out System.Drawing.Size size)
        {
            CoreNativeMethods.RECT rect = new CoreNativeMethods.RECT();
            if (!GetClientRect(hwnd, ref rect))
            {
                size = new System.Drawing.Size(-1, -1);
                return false;
            }
            size = new System.Drawing.Size(rect.right, rect.bottom);
            return true;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(
            IntPtr hwnd,
            ref CoreNativeMethods.POINT point);


        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StretchBlt(
            IntPtr hDestDC, int destX, int destY, int destWidth, int destHeight,
            IntPtr hSrcDC, int srcX, int srcY, int srcWidth, int srcHeight,
            uint operation);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr ChangeWindowMessageFilter(uint message, uint dwFlag);

        /// <summary>
        /// Sets the specified iconic thumbnail for the specified window.
        /// This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="hBitmap">The thumbnail bitmap.</param>
        internal static void SetIconicThumbnail(IntPtr hwnd, IntPtr hBitmap)
        {
            int rc = DwmSetIconicThumbnail(
                hwnd,
                hBitmap,
                DWM_SIT_DISPLAYFRAME);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }

        /// <summary>
        /// Sets the specified peek (live preview) bitmap for the specified
        /// window.  This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="bitmap">The thumbnail bitmap.</param>
        /// <param name="displayFrame">Whether to display a standard window
        /// frame around the bitmap.</param>
        internal static void SetPeekBitmap(IntPtr hwnd, IntPtr bitmap, bool displayFrame)
        {
            int rc = DwmSetIconicLivePreviewBitmap(
                hwnd,
                bitmap,
                IntPtr.Zero,
                displayFrame ? DWM_SIT_DISPLAYFRAME : (uint)0);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }

        /// <summary>
        /// Sets the specified peek (live preview) bitmap for the specified
        /// window.  This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="bitmap">The thumbnail bitmap.</param>
        /// <param name="offset">The client area offset at which to display
        /// the specified bitmap.  The rest of the parent window will be
        /// displayed as "remembered" by the DWM.</param>
        /// <param name="displayFrame">Whether to display a standard window
        /// frame around the bitmap.</param>
        internal static void SetPeekBitmap(IntPtr hwnd, IntPtr bitmap, Point offset, bool displayFrame)
        {
            var nativePoint = new CoreNativeMethods.POINT(offset.X, offset.Y);
            int rc = DwmSetIconicLivePreviewBitmap(
                hwnd,
                bitmap,
                ref nativePoint,
                displayFrame ? DWM_SIT_DISPLAYFRAME : (uint)0);

            if (rc != 0)
            {
                Exception e = Marshal.GetExceptionForHR(rc);

                if (e is ArgumentException)
                {
                    // Ignore argument exception as it's not really recommended to be throwing
                    // exception when rendering the peek bitmap. If it's some other kind of exception,
                    // then throw it.
                }
                else
                    throw e;
            }
        }

        /// <summary>
        /// Call this method to either enable custom previews on the taskbar (second argument as true)
        /// or to disable (second argument as false). If called with True, the method will call DwmSetWindowAttribute
        /// for the specific window handle and let DWM know that we will be providing a custom bitmap for the thumbnail
        /// as well as Aero peek.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="enable"></param>
        internal static void EnableCustomWindowPreview(IntPtr hwnd, bool enable)
        {
            IntPtr t = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(t, enable ? 1 : 0);

            try
            {
                int rc;
                rc = DwmSetWindowAttribute(
                    hwnd, DWMWA_HAS_ICONIC_BITMAP, t, 4);
                if (rc != 0)
                    throw Marshal.GetExceptionForHR(rc);

                rc = DwmSetWindowAttribute(
                    hwnd, DWMWA_FORCE_ICONIC_REPRESENTATION, t, 4);
                if (rc != 0)
                    throw Marshal.GetExceptionForHR(rc);
            }
            finally
            {
                Marshal.FreeHGlobal(t);
            }
        }

    }
}
