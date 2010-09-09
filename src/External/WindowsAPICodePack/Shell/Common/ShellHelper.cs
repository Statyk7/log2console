//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// A helper class for Shell Objects
    /// </summary>
    internal sealed class ShellHelper
    {
        private ShellHelper()
        {
            // Private constructor so no one can construct this using the default 
            // provided by the compiler.
        }

        internal static string GetParsingName(IShellItem shellItem)
        {
            if (shellItem == null)
                return null;

            string path = null;

            IntPtr pszPath = IntPtr.Zero;
            HRESULT hr = shellItem.GetDisplayName(ShellNativeMethods.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out pszPath);

            if (false ==
                    (hr == HRESULT.S_OK ||
                     hr == HRESULT.E_INVALIDARG))
                throw new COMException("GetParsingName", (int)hr);

            if (pszPath != IntPtr.Zero)
            {
                path = Marshal.PtrToStringAuto(pszPath);
                Marshal.FreeCoTaskMem( pszPath );
                pszPath = IntPtr.Zero;
            }

            return path;

        }

        internal static string GetAbsolutePath(string path)
        {
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                return path;
            else
                return Path.GetFullPath((path));

        }

        internal static PropertyKey ItemTypePropertyKey = new PropertyKey(new Guid("28636AA6-953D-11D2-B5D6-00C04FD918D0"), 11);

        internal static string GetItemType(IShellItem2 shellItem)
        {
            if (shellItem != null)
            {
                string itemType = null;

                HRESULT hr = shellItem.GetString(ref ItemTypePropertyKey, out itemType);

                if (hr == HRESULT.S_OK)
                    return itemType;
            }

            return null;
        }
  
        internal static IntPtr PidlFromParsingName(string name)
        {
            IntPtr pidl;

            ShellNativeMethods.SFGAO sfgao;
            int retCode = ShellNativeMethods.SHParseDisplayName(name, IntPtr.Zero, out pidl, (ShellNativeMethods.SFGAO)0,
                out sfgao);

            return (CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero);
        }

        internal static IntPtr PidlFromShellItem(IShellItem nativeShellItem)
        {
            IntPtr shellItem = Marshal.GetComInterfaceForObject(nativeShellItem, typeof(IShellItem));
            IntPtr pidl;

            int retCode = ShellNativeMethods.SHGetIDListFromObject(shellItem, out pidl);

            return (CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero);
        }

    }
}
