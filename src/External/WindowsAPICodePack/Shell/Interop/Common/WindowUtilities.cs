//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Microsoft.WindowsAPICodePack.Taskbar;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{

    internal static class WindowUtilities
    {
        internal static System.Drawing.Point GetParentOffsetOfChild(IntPtr hwnd, IntPtr hwndParent)
        {
            var childScreenCoord = new CoreNativeMethods.POINT(0, 0);

            TabbedThumbnailNativeMethods.ClientToScreen(
                hwnd, ref childScreenCoord);

            var parentScreenCoord = new CoreNativeMethods.POINT(0, 0);

            TabbedThumbnailNativeMethods.ClientToScreen(
                hwndParent, ref parentScreenCoord);

            System.Drawing.Point offset = new System.Drawing.Point(
                childScreenCoord.X - parentScreenCoord.X,
                childScreenCoord.Y - parentScreenCoord.Y);

            return offset;
        }

        internal static System.Drawing.Size GetNonClientArea(IntPtr hwnd)
        {
            var c = new CoreNativeMethods.POINT(0, 0);

            TabbedThumbnailNativeMethods.ClientToScreen(hwnd, ref c);

            var r = new CoreNativeMethods.RECT();

            TabbedThumbnailNativeMethods.GetWindowRect(hwnd, ref r);

            return new System.Drawing.Size(c.X - r.left, c.Y - r.top);
        }
    }

}
