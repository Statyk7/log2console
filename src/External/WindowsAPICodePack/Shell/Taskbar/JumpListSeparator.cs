//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents a separator in the user task list. The JumpListSeparator control
    /// can only be used in a user task list.
    /// </summary>
    public class JumpListSeparator : IJumpListTask, IDisposable
    {
        internal static PropertyKey PKEY_AppUserModel_IsDestListSeparator = SystemProperties.System.AppUserModel.IsDestListSeparator;
        
        /// <summary>
        /// Initializes a new instance of a JumpListSeparator.
        /// </summary>
        public JumpListSeparator()
        {
        }

        private IPropertyStore nativePropertyStore;
        private IShellLinkW nativeShellLink;
        /// <summary>
        /// Gets an IShellLinkW representation of this object
        /// </summary>
        internal IShellLinkW NativeShellLink
        {
            get
            {
                if (nativeShellLink != null)
                {
                    Marshal.ReleaseComObject(nativeShellLink);
                    nativeShellLink = null;
                }

                nativeShellLink = (IShellLinkW)new CShellLink();

                if (nativePropertyStore != null)
                {
                    Marshal.ReleaseComObject(nativePropertyStore);
                    nativePropertyStore = null;
                }

                nativePropertyStore = (IPropertyStore)nativeShellLink;

                PropVariant propVariant = new PropVariant();

                propVariant.SetBool(true);
                nativePropertyStore.SetValue(ref PKEY_AppUserModel_IsDestListSeparator, ref propVariant);
                propVariant.Clear();

                nativePropertyStore.Commit();

                return nativeShellLink;;
            }
        }


        #region IJumpListTask Members

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release the native and managed objects
        /// </summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        public void Dispose(bool disposing)
        {
            if (nativePropertyStore != null)
            {
                Marshal.ReleaseComObject(nativePropertyStore);
                nativePropertyStore = null;
            }

            if (nativeShellLink != null)
            {
                Marshal.ReleaseComObject(nativeShellLink);
                nativeShellLink = null;
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
        ~JumpListSeparator()
        {
            Dispose(false);
        }

        #endregion

    }
}
