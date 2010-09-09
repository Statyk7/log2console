//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents a jump list link object.
    /// </summary>
    public class JumpListLink : IJumpListTask, IJumpListItem, IDisposable
    {
        internal static PropertyKey PKEY_Title = SystemProperties.System.Title;

        /// <summary>
        /// Initializes a new instance of a JumpListLink with the specified path.
        /// </summary>
        /// <param name="pathValue">The path to the item. The path is required for the JumpList Link</param>
        /// <param name="titleValue">The title for the JumpListLink item. The title is required for the JumpList link.</param>
        public JumpListLink(string pathValue, string titleValue)
        {
            if (string.IsNullOrEmpty(pathValue))
                throw new ArgumentNullException("pathValue", "JumpListLink's path is required and cannot be null.");

            if (string.IsNullOrEmpty(titleValue))
                throw new ArgumentNullException("titleValue", "JumpListLink's title is required and cannot be null.");

            Path = pathValue;
            Title = titleValue;
        }

        private string title;
        /// <summary>
        /// Gets or sets the link's title
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "JumpListLink's title is required and cannot be null.");

                title = value;
            }
        }

        private string path;
        /// <summary>
        /// Gets or sets the link's path
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "JumpListLink's title is required and cannot be null.");

                path = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon reference (location and index) of the link's icon.
        /// </summary>
        public IconReference IconReference { get; set; }

        /// <summary>
        /// Gets or sets the object's arguments (passed to the command line).
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the object's working directory.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the show command of the lauched application.
        /// </summary>
        public WindowShowCommand ShowCommand { get; set; }

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

                nativeShellLink.SetPath(Path);

                if (!String.IsNullOrEmpty(IconReference.ModuleName))
                    nativeShellLink.SetIconLocation(IconReference.ModuleName, IconReference.ResourceId);

                if (!String.IsNullOrEmpty(Arguments))
                    nativeShellLink.SetArguments(Arguments);

                if (!String.IsNullOrEmpty(WorkingDirectory))
                    nativeShellLink.SetWorkingDirectory(WorkingDirectory);

                nativeShellLink.SetShowCmd((uint)ShowCommand);

                propVariant.SetString(Title);
                nativePropertyStore.SetValue(ref PKEY_Title, ref propVariant);
                propVariant.Clear();

                nativePropertyStore.Commit();

                return nativeShellLink;
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
            if (disposing)
            {
                title = null;
            }

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
        ~JumpListLink()
        {
            Dispose(false);
        }

        #endregion

    }
}
