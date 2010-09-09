//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Represents a thumbnail or an icon for a ShellObject.
    /// </summary>
    public class ShellThumbnail
    {
        #region Private members

        /// <summary>
        /// Native shellItem
        /// </summary>
        private IShellItem shellItemNative = null;

        /// <summary>
        /// Internal member to keep track of the current size
        /// </summary>
        private System.Windows.Size currentSize = new System.Windows.Size(256, 256);

        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor that takes in a parent ShellObject.
        /// </summary>
        /// <param name="shellObject"></param>
        internal ShellThumbnail(ShellObject shellObject)
        {
            if (shellObject == null || shellObject.NativeShellItem == null)
                throw new ArgumentNullException("shellObject");

            shellItemNative = shellObject.NativeShellItem;
        }

        /// <summary>
        /// No public default constructor. User should not be able to create a ShellThumbnail,
        /// only retrive it from an existing ShellFolder
        /// </summary>
        private ShellThumbnail() { }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the default size of the thumbnail or icon. The default is 32x32 pixels for icons and 
        /// 256x256 pixels for thumbnails.
        /// </summary>
        /// <remarks>If the size specified is larger than the maximum size of 1024x1024 for thumbnails and 256x256 for icons,
        /// an <see cref="System.ArgumentOutOfRangeException"/> is thrown.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "We are not currently handling globalization or localization")]
        public System.Windows.Size CurrentSize
        {
            get
            {
                return currentSize;
            }
            set
            {
                // Check for 0; negative number check not required as System.Windows.Size only allows positive numbers.
                if (value.Height == 0 || value.Width == 0)
                    throw new System.ArgumentOutOfRangeException("value", "CurrentSize (width or height) cannot be 0");

                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                {
                    if (value.Height > DefaultIconSize.Maximum.Height || value.Width > DefaultIconSize.Maximum.Width)
                        throw new System.ArgumentOutOfRangeException("value", string.Format("CurrentSize (width or height) cannot be greater than the maximum size", DefaultIconSize.Maximum));
                }
                else
                {
                    // Covers the default mode (Thumbnail Or Icon) as well as ThumbnailOnly
                    if (value.Height > DefaultThumbnailSize.Maximum.Height || value.Width > DefaultThumbnailSize.Maximum.Width)
                        throw new System.ArgumentOutOfRangeException("value", string.Format("CurrentSize (width or height) cannot be greater than the maximum size", DefaultThumbnailSize.Maximum));
                }

                currentSize = value;
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon image in <see cref="System.Drawing.Bitmap"/> format.
        /// Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return GetBitmap(CurrentSize);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon image in <see cref="System.Windows.Media.Imaging.BitmapSource"/> format. 
        /// Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public BitmapSource BitmapSource
        {
            get
            {
                return GetBitmapSource(CurrentSize);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon image in <see cref="System.Drawing.Icon"/> format. 
        /// Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return Icon.FromHandle(Bitmap.GetHicon());
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in small size and <see cref="System.Drawing.Bitmap"/> format.
        /// </summary>
        public Bitmap SmallBitmap
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmap(DefaultIconSize.Small);
                else
                    return GetBitmap(DefaultThumbnailSize.Small);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in small size and <see cref="System.Windows.Media.Imaging.BitmapSource"/> format.
        /// </summary>
        public BitmapSource SmallBitmapSource
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmapSource(DefaultIconSize.Small);
                else
                    return GetBitmapSource(DefaultThumbnailSize.Small);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in small size and <see cref="System.Drawing.Icon"/> format.
        /// </summary>
        public Icon SmallIcon
        {
            get
            {
                return Icon.FromHandle(SmallBitmap.GetHicon());
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in Medium size and <see cref="System.Drawing.Bitmap"/> format.
        /// </summary>
        public Bitmap MediumBitmap
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmap(DefaultIconSize.Medium);
                else
                    return GetBitmap(DefaultThumbnailSize.Medium);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in medium size and <see cref="System.Windows.Media.Imaging.BitmapSource"/> format.
        /// </summary>
        public BitmapSource MediumBitmapSource
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmapSource(DefaultIconSize.Medium);
                else
                    return GetBitmapSource(DefaultThumbnailSize.Medium);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in Medium size and <see cref="System.Drawing.Icon"/> format.
        /// </summary>
        public Icon MediumIcon
        {
            get
            {
                return Icon.FromHandle(MediumBitmap.GetHicon());
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in large size and <see cref="System.Drawing.Bitmap"/> format.
        /// </summary>
        public Bitmap LargeBitmap
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmap(DefaultIconSize.Large);
                else
                    return GetBitmap(DefaultThumbnailSize.Large);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in large size and <see cref="System.Windows.Media.Imaging.BitmapSource"/> format.
        /// </summary>
        public BitmapSource LargeBitmapSource
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmapSource(DefaultIconSize.Large);
                else
                    return GetBitmapSource(DefaultThumbnailSize.Large);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in Large size and <see cref="System.Drawing.Icon"/> format.
        /// </summary>
        public Icon LargeIcon
        {
            get
            {
                return Icon.FromHandle(LargeBitmap.GetHicon());
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in extra large size and <see cref="System.Drawing.Bitmap"/> format.
        /// </summary>
        public Bitmap ExtraLargeBitmap
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmap(DefaultIconSize.ExtraLarge);
                else
                    return GetBitmap(DefaultThumbnailSize.ExtraLarge);

            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in Extra Large size and <see cref="System.Windows.Media.Imaging.BitmapSource"/> format.
        /// </summary>
        public BitmapSource ExtraLargeBitmapSource
        {
            get
            {
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                    return GetBitmapSource(DefaultIconSize.ExtraLarge);
                else
                    return GetBitmapSource(DefaultThumbnailSize.ExtraLarge);
            }
        }

        /// <summary>
        /// Gets the thumbnail or icon in Extra Large size and <see cref="System.Drawing.Icon"/> format.
        /// </summary>
        public Icon ExtraLargeIcon
        {
            get
            {
                return Icon.FromHandle(ExtraLargeBitmap.GetHicon());
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if the current retrieval option is cache or extract, cache only, or from memory only.
        /// The default is cache or extract.
        /// </summary>
        public ShellThumbnailRetrievalOptions RetrievalOption
        {
            get;
            set;
        }

        private ShellThumbnailFormatOptions formatOption = ShellThumbnailFormatOptions.Default;
        /// <summary>
        /// Gets or sets a value that determines if the current format option is thumbnail or icon, thumbnail only, or icon only.
        /// The default is thumbnail or icon.
        /// </summary>
        public ShellThumbnailFormatOptions FormatOption
        {
            get
            {
                return formatOption;
            }
            set
            {
                formatOption = value;

                // Do a similar check as we did in CurrentSize property setter,
                // If our mode is IconOnly, then our max is defined by DefaultIconSize.Maximum. We should make sure 
                // our CurrentSize is within this max range
                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                {
                    if (CurrentSize.Height > DefaultIconSize.Maximum.Height || CurrentSize.Width > DefaultIconSize.Maximum.Width)
                        CurrentSize = DefaultIconSize.Maximum;
                }
            }

        }

        /// <summary>
        /// Gets or sets a value that determines if the user can manually stretch the returned image.
        /// The default value is false.
        /// </summary>
        /// <remarks>
        /// For example, if the caller passes in 80x80 a 96x96 thumbnail could be returned. 
        /// This could be used as a performance optimization if the caller will need to stretch 
        /// the image themselves anyway. Note that the Shell implementation performs a GDI stretch blit. 
        /// If the caller wants a higher quality image stretch, they should pass this flag and do it themselves.
        /// </remarks>
        public bool AllowBiggerSize
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        private ShellNativeMethods.SIIGBF CalculateFlags()
        {
            ShellNativeMethods.SIIGBF flags = 0x0000;

            if (AllowBiggerSize)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_BIGGERSIZEOK;

            if (RetrievalOption == ShellThumbnailRetrievalOptions.CacheOnly)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_INCACHEONLY;
            else if (RetrievalOption == ShellThumbnailRetrievalOptions.MemoryOnly)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_MEMORYONLY;

            if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_ICONONLY;
            else if (FormatOption == ShellThumbnailFormatOptions.ThumbnailOnly)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_THUMBNAILONLY;

            return flags;
        }

        private IntPtr GetHBitmap(System.Windows.Size size)
        {
            IntPtr hbitmap = IntPtr.Zero;

            // Create a size structure to pass to the native method
            CoreNativeMethods.SIZE nativeSIZE = new CoreNativeMethods.SIZE();
            nativeSIZE.cx = Convert.ToInt32(size.Width);
            nativeSIZE.cy = Convert.ToInt32(size.Height);

            // Use IShellItemImageFactory to get an icon
            // Options passed in: Resize to fit
            HRESULT hr = ((IShellItemImageFactory)shellItemNative).GetImage(nativeSIZE, CalculateFlags(), out hbitmap);

            if (hr == HRESULT.S_OK)
                return hbitmap;
            else if ((uint)hr == 0x8004B200 && FormatOption == ShellThumbnailFormatOptions.ThumbnailOnly)
            {
                // Thumbnail was requested, but this ShellItem doesn't have a thumbnail.
                throw new InvalidOperationException("The current ShellObject does not have a thumbnail. Try using ShellThumbnailFormatOptions.Default to get the icon for this item.", Marshal.GetExceptionForHR((int)hr));
            }
            else if ((uint)hr == 0x80040154) // REGDB_E_CLASSNOTREG
            {
                throw new NotSupportedException("The current ShellObject does not have a valid thumbnail handler or there was a problem in extracting the thumbnail for this specific shell object.", Marshal.GetExceptionForHR((int)hr));
            }
            else
                throw Marshal.GetExceptionForHR((int)hr);
        }

        private Bitmap GetBitmap(System.Windows.Size size)
        {
            IntPtr hBitmap = GetHBitmap(size);

            // return a System.Drawing.Bitmap from the hBitmap
            Bitmap returnValue = Bitmap.FromHbitmap(hBitmap);

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        private BitmapSource GetBitmapSource(System.Windows.Size size)
        {
            IntPtr hBitmap = GetHBitmap(size);

            // return a System.Media.Imaging.BitmapSource
            // Use interop to create a BitmapSource from hBitmap.
            BitmapSource returnValue = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        #endregion

    }
}