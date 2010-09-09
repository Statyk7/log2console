// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Event args for the TabbedThumbnailBitmapRequested event. The event allows applications to
    /// provide a bitmap for the tabbed thumbnail's preview and peek. The application should also
    /// set the Handled property if a custom bitmap is provided.
    /// </summary>
    public class TabbedThumbnailBitmapRequestedEventArgs : TabbedThumbnailEventArgs
    {
        /// <summary>
        /// Creates a Event Args for a TabbedThumbnailBitmapRequested event.
        /// </summary>
        /// <param name="windowHandle">Window handle for the control/window related to the event</param>
        /// <param name="preview">TabbedThumbnail related to this event</param>
        public TabbedThumbnailBitmapRequestedEventArgs(IntPtr windowHandle, TabbedThumbnail preview)
            : base(windowHandle, preview)
        {
        }

        /// <summary>
        /// Creates a Event Args for a TabbedThumbnailBitmapRequested event.
        /// </summary>
        /// <param name="windowsControl">WPF Control (UIElement) related to the event</param>
        /// <param name="preview">TabbedThumbnail related to this event</param>
        public TabbedThumbnailBitmapRequestedEventArgs(UIElement windowsControl, TabbedThumbnail preview)
            : base(windowsControl, preview)
        {
        }


        /// <summary>
        /// Gets or sets a value indicating whether the TabbedThumbnailBitmapRequested event was handled.
        /// Set this property if the SetImage method is called with a custom bitmap for the thumbnail/peek.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Override the thumbnail and peek bitmap. 
        /// By providing this bitmap manually, Thumbnail Window manager will provide the 
        /// Desktop Window Manager (DWM) this bitmap instead of rendering one automatically.
        /// Use this property to update the bitmap whenever the control is updated and the user
        /// needs to be shown a new thumbnail on the taskbar preview (or aero peek).
        /// </summary>
        /// <param name="bitmap">The bitmap to be displayed.</param>
        /// <remarks>
        /// If the bitmap doesn't have the right dimensions, the DWM may scale it or not 
        /// render certain areas as appropriate - it is the user's responsibility
        /// to render a bitmap with the proper dimensions.
        /// </remarks>
        public void SetImage(Bitmap bitmap)
        {
            base.TabbedThumbnail.SetImage(bitmap);
        }

        /// <summary>
        /// Override the thumbnail and peek bitmap. 
        /// By providing this bitmap manually, Thumbnail Window manager will provide the 
        /// Desktop Window Manager (DWM) this bitmap instead of rendering one automatically.
        /// Use this property to update the bitmap whenever the control is updated and the user
        /// needs to be shown a new thumbnail on the taskbar preview (or aero peek).
        /// </summary>
        /// <param name="bitmapSource">The bitmap to be displayed.</param>
        /// <remarks>
        /// If the bitmap doesn't have the right dimensions, the DWM may scale it or not 
        /// render certain areas as appropriate - it is the user's responsibility
        /// to render a bitmap with the proper dimensions.
        /// </remarks>
        public void SetImage(BitmapSource bitmapSource)
        {
            base.TabbedThumbnail.SetImage(bitmapSource);
        }


        /// <summary>
        /// Override the thumbnail and peek bitmap. 
        /// By providing this bitmap manually, Thumbnail Window manager will provide the 
        /// Desktop Window Manager (DWM) this bitmap instead of rendering one automatically.
        /// Use this property to update the bitmap whenever the control is updated and the user
        /// needs to be shown a new thumbnail on the taskbar preview (or aero peek).
        /// </summary>
        /// <param name="hBitmap">The bitmap to be displayed.</param>
        /// <remarks>
        /// If the bitmap doesn't have the right dimensions, the DWM may scale it or not 
        /// render certain areas as appropriate - it is the user's responsibility
        /// to render a bitmap with the proper dimensions.
        /// </remarks>
        public void SetImage(IntPtr hBitmap)
        {
            base.TabbedThumbnail.SetImage(hBitmap);
        }


    }
}
