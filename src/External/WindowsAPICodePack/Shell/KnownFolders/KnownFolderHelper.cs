// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Creates the helper class for known folders.
    /// </summary>
    public static class KnownFolderHelper
    {
        static KnownFolderHelper()
        {
            // Private constructor to prevent the compiler from generating the default one.
        }

        /// <summary>
        /// Returns the native known folder (IKnownFolderNative) given a PID list
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        internal static IKnownFolderNative FromPIDL(IntPtr pidl)
        {
            IKnownFolderManager knownFolderManager = (IKnownFolderManager)new KnownFolderManagerClass();
            IKnownFolderNative knownFolder;
            HRESULT hr = knownFolderManager.FindFolderFromIDList(pidl, out knownFolder);

            if (hr != HRESULT.S_OK)
                return null;
            else
                return knownFolder;
        }

        /// <summary>
        /// Returns a known folder given a globally unique identifier.
        /// </summary>
        /// <param name="knownFolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given Known Folder ID is invalid.</exception>
        public static IKnownFolder FromKnownFolderId(Guid knownFolderId)
        {
            IKnownFolderNative knownFolderNative;
            IKnownFolderManager knownFolderManager = (IKnownFolderManager)new KnownFolderManagerClass();

            HRESULT hr = knownFolderManager.GetFolder(knownFolderId, out knownFolderNative);

            if (hr == HRESULT.S_OK)
            {
                IKnownFolder kf = GetKnownFolder(knownFolderNative);

                if (kf != null)
                    return kf;
                else
                    throw new ArgumentException("Given Known Folder ID is invalid.", "knownFolderId");
            }
            else
                throw Marshal.GetExceptionForHR((int)hr);
        }

        /// <summary>
        /// Returns a known folder given a globally unique identifier.
        /// </summary>
        /// <param name="knownFolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name. Returns null if Known Folder is not found or could not be created.</returns>
        internal static IKnownFolder FromKnownFolderIdInternal(Guid knownFolderId)
        {
            IKnownFolderNative knownFolderNative;
            IKnownFolderManager knownFolderManager = (IKnownFolderManager)new KnownFolderManagerClass();

            HRESULT hr = knownFolderManager.GetFolder(knownFolderId, out knownFolderNative);

            if (hr == HRESULT.S_OK)
            {
                return GetKnownFolder(knownFolderNative);
            }
            else
                return null;
        }

        /// <summary>
        /// Given a native KnownFolder (IKnownFolderNative), create the right type of
        /// IKnownFolder object (FileSystemKnownFolder or NonFileSystemKnownFolder)
        /// </summary>
        /// <param name="knownFolderNative">Native Known Folder</param>
        /// <returns></returns>
        private static IKnownFolder GetKnownFolder(IKnownFolderNative knownFolderNative)
        {
            Debug.Assert(knownFolderNative != null, "Native IKnownFolder should not be null.");

            // Get the native IShellItem2 from the native IKnownFolder
            IShellItem2 shellItem;
            Guid guid = new Guid(ShellIIDGuid.IShellItem2);
            HRESULT hr = knownFolderNative.GetShellItem(0, ref guid, out shellItem);

            if (!CoreErrorHelper.Succeeded((int)hr))
                return null;

            bool isFileSystem = false;

            // If we have a valid IShellItem, try to get the FileSystem attribute.
            if (shellItem != null)
            {
                ShellNativeMethods.SFGAO sfgao;
                shellItem.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM, out sfgao);

                // Is this item a FileSystem item?
                isFileSystem = (sfgao & ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0;
            }

            // If it's FileSystem, create a FileSystemKnownFolder, else NonFileSystemKnownFolder
            if (isFileSystem)
            {
                FileSystemKnownFolder kf = new FileSystemKnownFolder(knownFolderNative);
                return kf;
            }
            else
            {
                NonFileSystemKnownFolder kf = new NonFileSystemKnownFolder(knownFolderNative);
                return kf;
            }

        }

        /// <summary>
        /// Returns the known folder given its canonical name.
        /// </summary>
        /// <param name="canonicalName">A non-localized canonical name for the known folder, such as MyComputer.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given canonical name is invalid or if the KnownFolder could not be created.</exception>
        public static IKnownFolder FromCanonicalName(string canonicalName)
        {
            IKnownFolderNative knownFolderNative;
            IKnownFolderManager knownFolderManager = (IKnownFolderManager)new KnownFolderManagerClass();

            knownFolderManager.GetFolderByName(canonicalName, out knownFolderNative);
            IKnownFolder kf = KnownFolderHelper.GetKnownFolder(knownFolderNative);

            if (kf != null)
                return kf;
            else
                throw new ArgumentException("Canonical name is invalid.", "canonicalName");
        }

        /// <summary>
        /// Returns a known folder given its shell path, such as <c>C:\users\public\documents</c> or 
        /// <c>::{645FF040-5081-101B-9F08-00AA002F954E}</c> for the Recycle Bin.
        /// </summary>
        /// <param name="path">The path for the requested known folder; either a physical path or a virtual path.</param>
        /// <returns>A known folder representing the specified name.</returns>
        public static IKnownFolder FromPath(string path)
        {
            return KnownFolderHelper.FromParsingName(path);
        }

        /// <summary>
        /// Returns a known folder given its shell namespace parsing name, such as 
        /// <c>::{645FF040-5081-101B-9F08-00AA002F954E}</c> for the Recycle Bin.
        /// </summary>
        /// <param name="parsingName">The parsing name (or path) for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given parsing name is invalid.</exception>
        public static IKnownFolder FromParsingName(string parsingName)
        {
            IntPtr pidl = IntPtr.Zero;
            IntPtr pidl2 = IntPtr.Zero;

            try
            {
                pidl = ShellHelper.PidlFromParsingName(parsingName);

                if (pidl == IntPtr.Zero)
                {
                    throw new ArgumentException("Parsing name is invalid.", "parsingName");
                }


                // It's probably a special folder, try to get it                
                IKnownFolderNative knownFolderNative = KnownFolderHelper.FromPIDL(pidl);

                if (knownFolderNative != null)
                {
                    IKnownFolder kf = KnownFolderHelper.GetKnownFolder(knownFolderNative);

                    if (kf != null)
                        return kf;
                    else
                        throw new ArgumentException("Parsing name is invalid.", "parsingName");
                }
                else
                {
                    // No physical storage was found for this known folder
                    // We'll try again with a different name

                    // try one more time with a trailing \0
                    pidl2 = ShellHelper.PidlFromParsingName(parsingName.PadRight(1, '\0'));

                    if (pidl2 == IntPtr.Zero)
                    {
                        throw new ArgumentException("Parsing name is invalid.", "parsingName");
                    }

                    IKnownFolder kf = KnownFolderHelper.GetKnownFolder(KnownFolderHelper.FromPIDL(pidl));

                    if (kf != null)
                        return kf;
                    else
                        throw new ArgumentException("Parsing name is invalid.", "parsingName");
                }
            }
            finally
            {
                ShellNativeMethods.ILFree(pidl);
                ShellNativeMethods.ILFree(pidl2);
            }

        }
    }
}
