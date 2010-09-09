//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// The base class for all Shell objects in Shell Namespace.
    /// </summary>
    abstract public class ShellObject : IDisposable, IEquatable<ShellObject>
    {

        #region Public Static Methods

        /// <summary>
        /// Creates a ShellObject subclass given a parsing name.
        /// For file system items, this method will only accept absolute paths.
        /// </summary>
        /// <param name="parsingName">The parsing name of the object.</param>
        /// <returns>A newly constructed ShellObject object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public static ShellObject FromParsingName(string parsingName)
        {
            return ShellObjectFactory.Create(parsingName);
        }

        /// <summary>
        /// Indicates whether this feature is supported on the current platform.
        /// </summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows Vista onwards ...
                return CoreHelpers.RunningOnVista;
            }
        }

        #endregion

        #region Internal Fields

        /// <summary>
        /// Internal member to keep track of the native IShellItem2
        /// </summary>
        internal IShellItem2 nativeShellItem;

        #endregion

        #region Constructors
        
        internal ShellObject()
        {
        }

        internal ShellObject(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Protected Fields

        /// <summary>
        /// Parsing name for this Object e.g. c:\Windows\file.txt,
        /// or ::{Some Guid} 
        /// </summary>
        private string internalParsingName = null;

        /// <summary>
        /// A friendly name for this object that' suitable for display
        /// </summary>
        private string internalName = null;

        /// <summary>
        /// PID List (PIDL) for this object
        /// </summary>
        private IntPtr internalPIDL = IntPtr.Zero;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Return the native ShellFolder object as newer IShellItem2
        /// </summary>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">If the native object cannot be created.
        /// The ErrorCode member will contain the external error code.</exception>
        virtual internal IShellItem2 NativeShellItem2
        {
            get
            {
                if (nativeShellItem == null && ParsingName != null)
                {
                    Guid guid = new Guid(ShellIIDGuid.IShellItem2);
                    int retCode = ShellNativeMethods.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref guid, out nativeShellItem);

                    if (nativeShellItem == null || !CoreErrorHelper.Succeeded(retCode))
                    {
                        throw new ExternalException("Shell item could not be created.", Marshal.GetExceptionForHR(retCode));
                    }
                }
                return nativeShellItem;
            }
        }

        /// <summary>
        /// Return the native ShellFolder object
        /// </summary>
        virtual internal IShellItem NativeShellItem
        {
            get
            {
                return NativeShellItem2;
            }
        }

        /// <summary>
        /// Gets access to the native IPropertyStore (if one is already
        /// created for this item and still valid. This is usually done by the
        /// ShellPropertyWriter class. The reference will be set to null
        /// when the writer has been closed/commited).
        /// </summary>
        internal IPropertyStore NativePropertyStore
        {
            get;
            set;
        }

        #endregion

        #region Public Properties

        private ShellProperties properties = null;
        /// <summary>
        /// Gets an object that allows the manipulation of ShellProperties for this shell item.
        /// </summary>
        public ShellProperties Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new ShellProperties(this);
                }
                return properties;
            }
        }

        /// <summary>
        /// Gets the parsing name for this ShellItem.
        /// </summary>
        virtual public string ParsingName
        {
            get
            {
                if (internalParsingName == null && nativeShellItem != null)
                {
                    internalParsingName = ShellHelper.GetParsingName(nativeShellItem);
                }
                return internalParsingName;
            }
            protected set
            {
                this.internalParsingName = value;
            }
        }

        /// <summary>
        /// Gets the normal display for this ShellItem.
        /// </summary>
        virtual public string Name
        {
            get
            {
                if (internalName == null && NativeShellItem != null)
                {
                    IntPtr pszString = IntPtr.Zero;
                    HRESULT hr = NativeShellItem.GetDisplayName(ShellNativeMethods.SIGDN.SIGDN_NORMALDISPLAY, out pszString);
                    if (hr == HRESULT.S_OK && pszString != IntPtr.Zero)
                    {
                        internalName = Marshal.PtrToStringAuto(pszString);

                        // Free the string
                        Marshal.FreeCoTaskMem(pszString);

                    }
                }
                return internalName;
            }

            protected set
            {
                this.internalName = value;
            }
        }

        /// <summary>
        /// Gets the PID List (PIDL) for this ShellItem.
        /// </summary>
        virtual internal IntPtr PIDL
        {
            get
            {
                // Get teh PIDL for the ShellItem
                if (internalPIDL == IntPtr.Zero && NativeShellItem != null)
                    internalPIDL = ShellHelper.PidlFromShellItem(NativeShellItem);

                return internalPIDL;
            }
            set
            {
                this.internalPIDL = value;
            }
        }

        /// <summary>
        /// Overrides object.ToString()
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Returns the display name of the ShellFolder object. DisplayNameType represents one of the 
        /// values that indicates how the name should look. 
        /// See <see cref="Microsoft.WindowsAPICodePack.Shell.DisplayNameType"/>for a list of possible values.
        /// </summary>
        /// <param name="displayNameType">A disaply name type.</param>
        /// <returns>A string.</returns>
        virtual public string GetDisplayName(DisplayNameType displayNameType)
        {
            string returnValue = null;

            HRESULT hr = HRESULT.S_OK;

            if (NativeShellItem2 != null)
                hr = NativeShellItem2.GetDisplayName((ShellNativeMethods.SIGDN)displayNameType, out returnValue);

            if (hr != HRESULT.S_OK)
                throw new COMException("Can't get the display name", (int)hr);

            return returnValue;
        }

        /// <summary>
        /// Gets a value that determines if this ShellObject is a link or shortcut.
        /// </summary>
        public bool IsLink
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAO sfgao;
                    NativeShellItem.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_LINK, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAO.SFGAO_LINK) != 0;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    // NativeShellItem is null
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value that determines if this ShellObject is a file system object.
        /// </summary>
        public bool IsFileSystemObject
        {
            get
            {
                try
                {
                    ShellNativeMethods.SFGAO sfgao;
                    NativeShellItem.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM, out sfgao);
                    return (sfgao & ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    // NativeShellItem is null
                    return false;
                }
            }
        }

        private ShellThumbnail thumbnail;
        /// <summary>
        /// Gets the thumbnail of the ShellObject.
        /// </summary>
        public ShellThumbnail Thumbnail
        {
            get
            {
                if (thumbnail == null)
                    thumbnail = new ShellThumbnail(this);

                return thumbnail;
            }
        }

        private ShellObject parentShellObject = null;
        /// <summary>
        /// Gets the parent ShellObject.
        /// Returns null if the object has no parent, i.e. if this object is the Desktop folder.
        /// </summary>
        public ShellObject Parent
        {
            get
            {
                if (parentShellObject == null && NativeShellItem2 != null)
                {
                    IShellItem parentShellItem = null;

                    HRESULT hr = NativeShellItem2.GetParent(out parentShellItem);

                    if (hr == HRESULT.S_OK && parentShellItem != null)
                        parentShellObject = ShellObjectFactory.Create(parentShellItem);
                    else if (hr == HRESULT.NO_OBJECT)
                    {
                        // Should return null if the parent is desktop
                        return null;
                    }
                    else
                    {
                        throw Marshal.GetExceptionForHR((int)hr);
                    }
                }

                return parentShellObject;
            }
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release the native and managed objects
        /// </summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                internalName = null;
                internalParsingName = null;
                properties = null;
                thumbnail = null;
                parentShellObject = null;
            }

            if (internalPIDL != IntPtr.Zero)
            {
                ShellNativeMethods.ILFree(internalPIDL);
                internalPIDL = IntPtr.Zero;
            }

            if (nativeShellItem != null)
            {
                Marshal.ReleaseComObject(nativeShellItem);
                nativeShellItem = null;
            }

            if (NativePropertyStore != null)
            {
                Marshal.ReleaseComObject(NativePropertyStore);
                NativePropertyStore = null;
            }

        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implement the finalizer.
        /// </summary>
        ~ShellObject()
        {
            Dispose(false);
        }

        #endregion

        #region equality and hashing

        /// <summary>
        /// Returns the hash code of the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode( )
        {
            if( !hashValue.HasValue )
            {
                uint size = ShellNativeMethods.ILGetSize( PIDL );
                if( size != 0 )
                {
                    byte[ ] pidlData = new byte[ size ];
                    Marshal.Copy( PIDL, pidlData, 0, (int)size );
                    byte[ ] hashData = ShellObject.hashProvider.ComputeHash( pidlData );
                    hashValue = BitConverter.ToInt32( hashData, 0 );
                }
                else
                {
                    hashValue = 0;
                }
                    
            }
            return hashValue.Value;
        }
        private static MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider( );
        private int? hashValue;

        /// <summary>
        /// Determines if two ShellObjects are identical.
        /// </summary>
        /// <param name="other">The ShellObject to comare this one to.</param>
        /// <returns>True if the ShellObjects are equal, false otherwise.</returns>
        public bool Equals(ShellObject other)
        {
            bool areEqual = false;

            if ((object)other != null)
            {
                IShellItem ifirst = this.NativeShellItem;
                IShellItem isecond = other.NativeShellItem;
                if (((ifirst != null) && (isecond != null)))
                {
                    int result = 0;
                    HRESULT hr = ifirst.Compare(
                        isecond, SICHINTF.SICHINT_ALLFIELDS, out result);

                    if ((hr == HRESULT.S_OK) && (result == 0))
                        areEqual = true;
                }
            }

            return areEqual;
        }

        /// <summary>
        /// Returns whether this object is equal to another.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ShellObject);
        }

        /// <summary>
        /// Implements the == (equality) operator.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <returns>true if object a equals object b; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator ==(ShellObject a, ShellObject b)
        {
            if ((object)a == null)
            {
                return ((object)b == null);
            }
            else
            {
                return a.Equals(b);
            }
        }

        /// <summary>
        /// Implements the != (inequality) operator.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <returns>true if object a does not equal object b; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        public static bool operator !=(ShellObject a, ShellObject b)
        {
            if ((object)a == null)
            {
                return ((object)b != null);
            }
            else
            {
                return !a.Equals(b);
            }
        }


        #endregion
    }
}
