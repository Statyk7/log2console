//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Helper class to capture a control or window as System.Drawing.Bitmap
    /// </summary>
    public static class TabbedThumbnailScreenCapture
    {
        /// <summary>
        /// Captures a screenshot of the specified window at the specified
        /// bitmap size. <para/>NOTE: This method will not accurately capture controls
        /// that are hidden or obstructed (partially or completely) by another control (e.g. hidden tabs,
        /// or MDI child windows that are obstructed by other child windows/forms).
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="bitmapSize">The requested bitmap size.</param>
        /// <returns>A screen capture of the window.</returns>
        public static Bitmap GrabWindowBitmap(IntPtr hwnd, System.Drawing.Size bitmapSize)
        {
            if (bitmapSize.Height <= 0 || bitmapSize.Width <= 0)
                return null;

            IntPtr windowDC = IntPtr.Zero;
            IntPtr targetDC = IntPtr.Zero;
            Graphics targetGr = null;

            try
            {
                System.Drawing.Size realWindowSize;
                TabbedThumbnailNativeMethods.GetClientSize(hwnd, out realWindowSize);

                if (realWindowSize == System.Drawing.Size.Empty)
                    realWindowSize = new System.Drawing.Size(200, 200);

                windowDC = TabbedThumbnailNativeMethods.GetWindowDC(hwnd);

                Bitmap targetBitmap = null;

                if (bitmapSize == System.Drawing.Size.Empty)
                    targetBitmap = new Bitmap(realWindowSize.Width, realWindowSize.Height);
                else
                    targetBitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

                targetGr = Graphics.FromImage(targetBitmap);

                targetDC = targetGr.GetHdc();
                uint operation = 0x00CC0020 /*SRCCOPY*/;

                System.Drawing.Size ncArea = WindowUtilities.GetNonClientArea(hwnd);

                bool success = TabbedThumbnailNativeMethods.StretchBlt(targetDC, 0, 0, targetBitmap.Width, targetBitmap.Height,
                    windowDC, ncArea.Width, ncArea.Height, realWindowSize.Width, realWindowSize.Height, operation);
                return targetBitmap;
            }
            finally
            {
                if (windowDC != IntPtr.Zero)
                {
                    TabbedThumbnailNativeMethods.ReleaseDC(hwnd, windowDC);
                }
                if (targetGr != null && targetDC != IntPtr.Zero)
                {
                    targetGr.ReleaseHdc(targetDC);
                }
            }
        }

        /// <summary>
        /// Grabs a snapshot of a WPF UIElement and returns the image as Bitmap.
        /// </summary>
        /// <param name="element">Represents the element to take the snapshot from.</param>
        /// <param name="dpiX">Represents the X DPI value used to capture this snapshot.</param>
        /// <param name="dpiY">Represents the Y DPI value used to capture this snapshot.</param>
        /// <param name="width">The requested bitmap width.</param>
        /// <param name="height">The requested bitmap height.</param>
        /// <returns>Returns the bitmap (PNG format).</returns>
        public static Bitmap GrabWindowBitmap(UIElement element, int dpiX, int dpiY, int width, int height)
        {
            // Special case for HwndHost controls
            if (element is HwndHost)
            {
                IntPtr handle = ((HwndHost)element).Handle;
                return GrabWindowBitmap(handle, new System.Drawing.Size(width, height));
            }

            Rect bounds = VisualTreeHelper.GetDescendantBounds(element);

            // create the renderer.
            if (bounds.Height == 0 || bounds.Width == 0)
                return null;    // 0 sized element. Probably hidden

            RenderTargetBitmap rendertarget = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
             (int)(bounds.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            rendertarget.Render(dv);

            BitmapEncoder bmpe;

            bmpe = new PngBitmapEncoder();
            bmpe.Frames.Add(BitmapFrame.Create(rendertarget));

            // Create a MemoryStream with the image.
            // Returning this as a MemoryStream makes it easier to save the image to a file or simply display it anywhere.
            MemoryStream fl = new MemoryStream();
            bmpe.Save(fl);

            Bitmap bmp = new Bitmap(fl);

            fl.Close();

            return (Bitmap)bmp.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }

        /// <summary>
        /// Resizes the given bitmap while maintaining the aspect ratio.
        /// </summary>
        /// <param name="originalHBitmap">Original/source bitmap</param>
        /// <param name="newWidth">Maximum width for the new image</param>
        /// <param name="maxHeight">Maximum height for the new image</param>
        /// <param name="resizeIfWider">If true and requested image is wider than the source, the new image is resized accordingly.</param>
        /// <returns></returns>
        internal static Bitmap ResizeImageWithAspect(IntPtr originalHBitmap, int newWidth, int maxHeight, bool resizeIfWider)
        {
            Bitmap originalBitmap = Bitmap.FromHbitmap(originalHBitmap);

            try
            {
                if (resizeIfWider)
                {
                    if (originalBitmap.Width <= newWidth)
                        newWidth = originalBitmap.Width;
                }

                int newHeight = originalBitmap.Height * newWidth / originalBitmap.Width;

                if (newHeight > maxHeight) // Height resize if necessary
                {
                    newWidth = originalBitmap.Width * maxHeight / originalBitmap.Height;
                    newHeight = maxHeight;
                }

                // Create the new image with the sizes we've calculated
                return (Bitmap)originalBitmap.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
            }
            finally
            {
                originalBitmap.Dispose();
                originalBitmap = null;
            }
        }
    }
}
