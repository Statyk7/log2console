//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    internal enum SICHINTF : uint
    {
        SICHINT_DISPLAY = 0x00000000,
        SICHINT_CANONICAL = 0x10000000,
        SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
        SICHINT_ALLFIELDS = 0x80000000
    }

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 108

    #region COM Interfaces

    [ComImport(),
    Guid(ShellIIDGuid.IModalWindow),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IModalWindow
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        int Show([In] IntPtr parent);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellItem),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        // Not supported: IBindCtx.
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler(
            [In] IntPtr pbc,
            [In] ref Guid bhid,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayName(
            [In] ShellNativeMethods.SIGDN sigdnName,
            out IntPtr ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] ShellNativeMethods.SFGAO sfgaoMask, out ShellNativeMethods.SFGAO psfgaoAttribs);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Compare(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            [In] SICHINTF hint,
            out int piOrder);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellItem2),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem2 : IShellItem
    {
        // Not supported: IBindCtx.
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler(
            [In] IntPtr pbc,
            [In] ref Guid bhid,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayName(
            [In] ShellNativeMethods.SIGDN sigdnName,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] ShellNativeMethods.SFGAO sfgaoMask, out ShellNativeMethods.SFGAO psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            [In] uint hint,
            out int piOrder);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        int GetPropertyStore(
            [In] ShellNativeMethods.GETPROPERTYSTOREFLAGS Flags,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreWithCreateObject([In] ShellNativeMethods.GETPROPERTYSTOREFLAGS Flags, [In, MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreForKeys([In] ref PropertyKey rgKeys, [In] uint cKeys, [In] ShellNativeMethods.GETPROPERTYSTOREFLAGS Flags, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.IUnknown)] out IPropertyStore ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyDescriptionList([In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetProperty([In] ref PropertyKey key, out PropVariant ppropvar);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCLSID([In] ref PropertyKey key, out Guid pclsid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTime([In] ref PropertyKey key, out System.Runtime.InteropServices.ComTypes.FILETIME pft);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetInt32([In] ref PropertyKey key, out int pi);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetString([In] ref PropertyKey key, [MarshalAs(UnmanagedType.LPWStr)] out string ppsz);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt32([In] ref PropertyKey key, out uint pui);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt64([In] ref PropertyKey key, out ulong pull);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetBool([In] ref PropertyKey key, out int pf);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellItemArray),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItemArray
    {
        // Not supported: IBindCtx.
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler(
            [In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc,
            [In] ref Guid rbhid,
            [In] ref Guid riid,
            out IntPtr ppvOut);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPropertyStore(
            [In] int Flags,
            [In] ref Guid riid,
            out IntPtr ppv);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPropertyDescriptionList(
            [In] ref PropertyKey keyType,
            [In] ref Guid riid,
            out IntPtr ppv);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetAttributes(
            [In] ShellNativeMethods.SIATTRIBFLAGS dwAttribFlags,
            [In] ShellNativeMethods.SFGAO sfgaoMask,
            out ShellNativeMethods.SFGAO psfgaoAttribs);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetCount(out uint pdwNumItems);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetItemAt(
            [In] uint dwIndex,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead).
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellLibrary),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLibrary
    {
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT LoadLibraryFromItem(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem library,
            [In] ShellNativeMethods.STGM grfMode);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LoadLibraryFromKnownFolder(
            [In] ref Guid knownfidLibrary,
            [In] ShellNativeMethods.STGM grfMode);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem location);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem location);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolders(
            [In] ShellNativeMethods.LIBRARYFOLDERFILTER lff,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ResolveFolder(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem folderToResolve,
            [In] uint timeout,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultSaveFolder(
            [In] ShellNativeMethods.DEFAULTSAVEFOLDERTYPE dsft,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultSaveFolder(
            [In] ShellNativeMethods.DEFAULTSAVEFOLDERTYPE dsft,
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem si);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(
            out ShellNativeMethods.LIBRARYOPTIONFLAGS lofOptions);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions(
            [In] ShellNativeMethods.LIBRARYOPTIONFLAGS lofMask,
            [In] ShellNativeMethods.LIBRARYOPTIONFLAGS lofOptions);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderType(out Guid ftid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolderType([In] ref Guid ftid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetIcon([MarshalAs(UnmanagedType.LPWStr)] out string icon);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetIcon([In, MarshalAs(UnmanagedType.LPWStr)] string icon);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Save(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem folderToSaveIn,
            [In, MarshalAs(UnmanagedType.LPWStr)] string libraryName,
            [In] ShellNativeMethods.LIBRARYSAVEFLAGS lsf,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SaveInKnownFolder(
            [In] ref Guid kfidToSaveIn,
            [In, MarshalAs(UnmanagedType.LPWStr)] string libraryName,
            [In] ShellNativeMethods.LIBRARYSAVEFLAGS lsf,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);
    };

    [ComImportAttribute()]
    [GuidAttribute("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IShellItemImageFactory
    {
        [PreserveSig]
        HRESULT GetImage(
        [In, MarshalAs(UnmanagedType.Struct)] CoreNativeMethods.SIZE size,
        [In] ShellNativeMethods.SIIGBF flags,
        [Out] out IntPtr phbm);
    }

    [ComImport,
    Guid(ShellIIDGuid.IThumbnailCache),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IThumbnailCache
    {
        void GetThumbnail([In] IShellItem pShellItem,
        [In] uint cxyRequestedThumbSize,
        [In] Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods.WTS_FLAGS flags,
        [Out] out ISharedBitmap ppvThumb,
        [Out] out Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods.WTS_CACHEFLAGS pOutFlags,
        [Out] Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods.WTS_THUMBNAILID pThumbnailID);

        void GetThumbnailByID([In] Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods.WTS_THUMBNAILID thumbnailID,
        [In] uint cxyRequestedThumbSize,
        [Out] out ISharedBitmap ppvThumb,
        [Out] out Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods.WTS_CACHEFLAGS pOutFlags);
    }

    [ComImport,
    Guid(ShellIIDGuid.ISharedBitmap),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ISharedBitmap
    {
        void GetSharedBitmap([Out] out IntPtr phbm);
        void GetSize([Out] out CoreNativeMethods.SIZE pSize);
        void GetFormat([Out] out ShellNativeMethods.WTS_ALPHATYPE pat);
        void InitializeBitmap([In] IntPtr hbm, [In] ShellNativeMethods.WTS_ALPHATYPE wtsAT);
        void Detach([Out] out IntPtr phbm);
    }
    [ComImport,
    Guid(ShellIIDGuid.IShellFolder),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    ComConversionLoss]
    internal interface IShellFolder
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ParseDisplayName(IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In, Out] ref uint pchEaten, [Out] IntPtr ppidl, [In, Out] ref uint pdwAttributes);
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT EnumObjects([In] IntPtr hwnd, [In] ShellNativeMethods.SHCONT grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToObject([In] IntPtr pidl, /*[In, MarshalAs(UnmanagedType.Interface)] IBindCtx*/ IntPtr pbc, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToStorage([In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CompareIDs([In] IntPtr lParam, [In] ref IntPtr pidl1, [In] ref IntPtr pidl2);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref uint rgfInOut);


        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayNameOf([In] ref IntPtr pidl, [In] uint uFlags, out IntPtr pName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetNameOf([In] IntPtr hwnd, [In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszName, [In] uint uFlags, [Out] IntPtr ppidlOut);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellFolder2),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    ComConversionLoss]
    internal interface IShellFolder2 : IShellFolder
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ParseDisplayName([In] IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In, Out] ref uint pchEaten, [Out] IntPtr ppidl, [In, Out] ref uint pdwAttributes);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumObjects([In] IntPtr hwnd, [In] ShellNativeMethods.SHCONT grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToObject([In] IntPtr pidl, /*[In, MarshalAs(UnmanagedType.Interface)] IBindCtx*/ IntPtr pbc, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToStorage([In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CompareIDs([In] IntPtr lParam, [In] ref IntPtr pidl1, [In] ref IntPtr pidl2);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref uint rgfInOut);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayNameOf([In] ref IntPtr pidl, [In] uint uFlags, out IntPtr pName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetNameOf([In] IntPtr hwnd, [In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszName, [In] uint uFlags, [Out] IntPtr ppidlOut);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultSearchGUID(out Guid pguid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumSearches([Out] out IntPtr ppenum);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultColumn([In] uint dwRes, out uint pSort, out uint pDisplay);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultColumnState([In] uint iColumn, out uint pcsFlags);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDetailsEx([In] ref IntPtr pidl, [In] ref PropertyKey pscid, [MarshalAs(UnmanagedType.Struct)] out object pv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDetailsOf([In] ref IntPtr pidl, [In] uint iColumn, out IntPtr psd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void MapColumnToSCID([In] uint iColumn, out PropertyKey pscid);
    }

    [ComImport,
    Guid(ShellIIDGuid.IEnumIDList),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumIDList
    {
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Next(uint celt, out IntPtr rgelt, out uint pceltFetched);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Skip([In] uint celt);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Reset();

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellLinkW),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW
    {
        void GetPath(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxPath,
            //ref _WIN32_FIND_DATAW pfd,
            IntPtr pfd,
            uint fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxName);
        void SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
            int cchMaxPath
            );
        void SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
            int cchMaxPath);
        void SetArguments(
            [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotKey(out short wHotKey);
        void SetHotKey(short wHotKey);
        void GetShowCmd(out uint iShowCmd);
        void SetShowCmd(uint iShowCmd);
        void GetIconLocation(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
            int cchIconPath,
            out int iIcon);
        void SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);
        void SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);
        void Resolve(IntPtr hwnd, uint fFlags);
        void SetPath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport,
    Guid(ShellIIDGuid.CShellLink),
    ClassInterface(ClassInterfaceType.None)]
    internal class CShellLink { }

    // Summary:
    //     Provides the managed definition of the IPersistStream interface, with functionality
    //     from IPersist.
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000109-0000-0000-C000-000000000046")]
    internal interface IPersistStream
    {
        // Summary:
        //     Retrieves the class identifier (CLSID) of an object.
        //
        // Parameters:
        //   pClassID:
        //     When this method returns, contains a reference to the CLSID. This parameter
        //     is passed uninitialized.
        [PreserveSig]
        void GetClassID(out Guid pClassID);
        //
        // Summary:
        //     Checks an object for changes since it was last saved to its current file.
        //
        // Returns:
        //     S_OK if the file has changed since it was last saved; S_FALSE if the file
        //     has not changed since it was last saved.
        [PreserveSig]
        HRESULT IsDirty();

        [PreserveSig]
        HRESULT Load([In, MarshalAs(UnmanagedType.Interface)] IStream stm);

        [PreserveSig]
        HRESULT Save([In, MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

        [PreserveSig]
        HRESULT GetSizeMax(out ulong cbSize);
    }

    [ComImport(),
    Guid(ShellIIDGuid.ICondition),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICondition : IPersistStream
    {
        // Summary:
        //     Retrieves the class identifier (CLSID) of an object.
        //
        // Parameters:
        //   pClassID:
        //     When this method returns, contains a reference to the CLSID. This parameter
        //     is passed uninitialized.
        [PreserveSig]
        void GetClassID(out Guid pClassID);
        //
        // Summary:
        //     Checks an object for changes since it was last saved to its current file.
        //
        // Returns:
        //     S_OK if the file has changed since it was last saved; S_FALSE if the file
        //     has not changed since it was last saved.
        [PreserveSig]
        HRESULT IsDirty();

        [PreserveSig]
        HRESULT Load([In, MarshalAs(UnmanagedType.Interface)] IStream stm);

        [PreserveSig]
        HRESULT Save([In, MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

        [PreserveSig]
        HRESULT GetSizeMax(out ulong cbSize);

        // For any node, return what kind of node it is.
        [PreserveSig]
        HRESULT GetConditionType([Out()] out SearchConditionType pNodeType);

        // riid must be IID_IEnumUnknown, IID_IEnumVARIANT or IID_IObjectArray, or in the case of a negation node IID_ICondition.
        // If this is a leaf node, E_FAIL will be returned.
        // If this is a negation node, then if riid is IID_ICondition, *ppv will be set to a single ICondition, otherwise an enumeration of one.
        // If this is a conjunction or a disjunction, *ppv will be set to an enumeration of the subconditions.
        [PreserveSig]
        HRESULT GetSubConditions([In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        // If this is not a leaf node, E_FAIL will be returned.
        // Retrieve the property name, operation and value from the leaf node.
        // Any one of ppszPropertyName, pcop and ppropvar may be NULL.
        [PreserveSig]
        HRESULT GetComparisonInfo([Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName,
            [Out()] out SearchConditionOperation pcop,
            [Out()] out PropVariant ppropvar);

        // If this is not a leaf node, E_FAIL will be returned.
        // *ppszValueTypeName will be set to the semantic type of the value, or to NULL if this is not meaningful.
        [PreserveSig]
        HRESULT GetValueType([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszValueTypeName);

        // If this is not a leaf node, E_FAIL will be returned.
        // If the value of the leaf node is VT_EMPTY, *ppszNormalization will be set to an empty string.
        // If the value is a string (VT_LPWSTR, VT_BSTR or VT_LPSTR), then *ppszNormalization will be set to a
        // character-normalized form of the value.
        // Otherwise, *ppszNormalization will be set to some (character-normalized) string representation of the value.
        [PreserveSig]
        HRESULT GetValueNormalization([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszNormalization);

        // Return information about what parts of the input produced the property, the operation and the value.
        // Any one of ppPropertyTerm, ppOperationTerm and ppValueTerm may be NULL.
        // For a leaf node returned by the parser, the position information of each IRichChunk identifies the tokens that
        // contributed the property/operation/value, the string value is the corresponding part of the input string, and
        // the PROPVARIANT is VT_EMPTY.
        [PreserveSig]
        HRESULT GetInputTerms([Out] out IRichChunk ppPropertyTerm, [Out] out IRichChunk ppOperationTerm, [Out] out IRichChunk ppValueTerm);

        // Make a deep copy of this ICondition.
        [PreserveSig]
        HRESULT Clone([Out()] out ICondition ppc);
    };

    [ComImport,
    Guid(ShellIIDGuid.IRichChunk),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRichChunk
    {
        // The position *pFirstPos is zero-based.
        // Any one of pFirstPos, pLength, ppsz and pValue may be NULL.
        [PreserveSig]
        HRESULT GetData(/*[out, annotation("__out_opt")] ULONG* pFirstPos, [out, annotation("__out_opt")] ULONG* pLength, [out, annotation("__deref_opt_out_opt")] LPWSTR* ppsz, [out, annotation("__out_opt")] PROPVARIANT* pValue*/);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(ShellIIDGuid.IEnumUnknown)]
    internal interface IEnumUnknown
    {
        [PreserveSig]
        HRESULT Next(UInt32 requestedNumber, ref IntPtr buffer, ref UInt32 fetchedNumber);
        [PreserveSig]
        HRESULT Skip(UInt32 number);
        [PreserveSig]
        HRESULT Reset();
        [PreserveSig]
        HRESULT Clone(out IEnumUnknown result);
    }


    [ComImport,
    Guid(ShellIIDGuid.IConditionFactory),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IConditionFactory
    {
        [PreserveSig]
        HRESULT MakeNot([In] ICondition pcSub, [In] bool fSimplify, [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT MakeAndOr([In] SearchConditionType ct, [In] IEnumUnknown peuSubs, [In] bool fSimplify, [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT MakeLeaf(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName,
            [In] SearchConditionOperation cop,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszValueType,
            [In] ref PropVariant ppropvar,
            IRichChunk richChunk1,
            IRichChunk richChunk2,
            IRichChunk richChunk3,
            [In] bool fExpand,
            [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT Resolve(/*[In] ICondition pc, [In] STRUCTURED_QUERY_RESOLVE_OPTION sqro, [In] ref SYSTEMTIME pstReferenceTime, [Out] out ICondition ppcResolved*/);

    };

    [ComImport,
    Guid(ShellIIDGuid.IConditionFactory),
    CoClass(typeof(ConditionFactoryCoClass))]
    internal interface INativeConditionFactory : IConditionFactory
    {
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ShellCLSIDGuid.ConditionFactory)]
    internal class ConditionFactoryCoClass
    {
    }



    [ComImport,
    Guid(ShellIIDGuid.ISearchFolderItemFactory),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISearchFolderItemFactory
    {
        [PreserveSig]
        HRESULT SetDisplayName([In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName);

        [PreserveSig]
        HRESULT SetFolderTypeID([In] Guid ftid);

        [PreserveSig]
        HRESULT SetFolderLogicalViewMode([In] FolderLogicalViewMode flvm);

        [PreserveSig]
        HRESULT SetIconSize([In] int iIconSize);

        [PreserveSig]
        HRESULT SetVisibleColumns([In] uint cVisibleColumns, [In, MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgKey);

        [PreserveSig]
        HRESULT SetSortColumns([In] uint cSortColumns, [In, MarshalAs(UnmanagedType.LPArray)] SortColumn[] rgSortColumns);

        [PreserveSig]
        HRESULT SetGroupColumn([In] ref PropertyKey keyGroup);

        [PreserveSig]
        HRESULT SetStacks([In] uint cStackKeys, [In, MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgStackKeys);

        [PreserveSig]
        HRESULT SetScope([In, MarshalAs(UnmanagedType.Interface)] IShellItemArray ppv);

        [PreserveSig]
        HRESULT SetCondition([In] ICondition pCondition);

        [PreserveSig]
        int GetShellItem(ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [PreserveSig]
        HRESULT GetIDList([Out] IntPtr ppidl);
    };

    [ComImport,
    Guid(ShellIIDGuid.ISearchFolderItemFactory),
    CoClass(typeof(SearchFolderItemFactoryCoClass))]
    internal interface INativeSearchFolderItemFactory : ISearchFolderItemFactory
    {
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ShellCLSIDGuid.SearchFolderItemFactory)]
    internal class SearchFolderItemFactoryCoClass
    {
    }

    [ComImport,
    Guid(ShellIIDGuid.IQuerySolution),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IQuerySolution : IConditionFactory
    {
        [PreserveSig]
        HRESULT MakeNot([In] ICondition pcSub, [In] bool fSimplify, [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT MakeAndOr([In] SearchConditionType ct, [In] IEnumUnknown peuSubs, [In] bool fSimplify, [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT MakeLeaf(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName,
            [In] SearchConditionOperation cop,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszValueType,
            [In] ref PropVariant ppropvar,
            IRichChunk richChunk1,
            IRichChunk richChunk2,
            IRichChunk richChunk3,
            [In] bool fExpand,
            [Out] out ICondition ppcResult);

        [PreserveSig]
        HRESULT Resolve([In] ICondition pc, [In] int sqro, [In] ref SYSTEMTIME pstReferenceTime, [Out] out ICondition ppcResolved);

        // Retrieve the condition tree and the "main type" of the solution.
        // ppQueryNode and ppMainType may be NULL.
        [PreserveSig]
        HRESULT GetQuery([Out, MarshalAs(UnmanagedType.Interface)] out ICondition ppQueryNode, [Out, MarshalAs(UnmanagedType.Interface)] out IEntity ppMainType);

        // Identify parts of the input string not accounted for.
        // Each parse error is represented by an IRichChunk where the position information
        // reflect token counts, the string is NULL and the value is a VT_I4
        // where lVal is from the ParseErrorType enumeration. The valid
        // values for riid are IID_IEnumUnknown and IID_IEnumVARIANT.
        [PreserveSig]
        HRESULT GetErrors([In] ref Guid riid, [Out] out /* void** */ IntPtr ppParseErrors);

        // Report the query string, how it was tokenized and what LCID and word breaker were used (for recognizing keywords).
        // ppszInputString, ppTokens, pLocale and ppWordBreaker may be NULL.
        [PreserveSig]
        HRESULT GetLexicalData([MarshalAs(UnmanagedType.LPWStr)] out string ppszInputString, [Out] /* ITokenCollection** */ out IntPtr ppTokens, [Out] out uint plcid, [Out] /* IUnknown** */ out IntPtr ppWordBreaker);
    }

    [ComImport,
    Guid(ShellIIDGuid.IQueryParser),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQueryParser
    {
        // Parse parses an input string, producing a query solution.
        // pCustomProperties should be an enumeration of IRichChunk objects, one for each custom property
        // the application has recognized. pCustomProperties may be NULL, equivalent to an empty enumeration.
        // For each IRichChunk, the position information identifies the character span of the custom property,
        // the string value should be the name of an actual property, and the PROPVARIANT is completely ignored.
        [PreserveSig]
        HRESULT Parse([In, MarshalAs(UnmanagedType.LPWStr)] string pszInputString, [In] IEnumUnknown pCustomProperties, [Out] out IQuerySolution ppSolution);

        // Set a single option. See STRUCTURED_QUERY_SINGLE_OPTION above.
        [PreserveSig]
        HRESULT SetOption([In] StructuredQuerySingleOption option, [In] ref PropVariant pOptionValue);

        [PreserveSig]
        HRESULT GetOption([In] StructuredQuerySingleOption option, [Out] out PropVariant pOptionValue);

        // Set a multi option. See STRUCTURED_QUERY_MULTIOPTION above.
        [PreserveSig]
        HRESULT SetMultiOption([In] StructuredQueryMultipleOption option, [In, MarshalAs(UnmanagedType.LPWStr)] string pszOptionKey, [In] PropVariant pOptionValue);

        // Get a schema provider for browsing the currently loaded schema.
        [PreserveSig]
        HRESULT GetSchemaProvider([Out] out /*ISchemaProvider*/ IntPtr ppSchemaProvider);

        // Restate a condition as a query string according to the currently selected syntax.
        // The parameter fUseEnglish is reserved for future use; must be FALSE.
        [PreserveSig]
        HRESULT RestateToString([In] ICondition pCondition, [In] bool fUseEnglish, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszQueryString);

        // Parse a condition for a given property. It can be anything that would go after 'PROPERTY:' in an AQS expession.
        [PreserveSig]
        HRESULT ParsePropertyValue([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName, [In, MarshalAs(UnmanagedType.LPWStr)] string pszInputString, [Out] out IQuerySolution ppSolution);

        // Restate a condition for a given property. If the condition contains a leaf with any other property name, or no property name at all,
        // E_INVALIDARG will be returned.
        [PreserveSig]
        HRESULT RestatePropertyValueToString([In] ICondition pCondition, [In] bool fUseEnglish, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszQueryString);
    }

    [ComImport,
    Guid(ShellIIDGuid.IQueryParserManager),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQueryParserManager
    {
        // Create a query parser loaded with the schema for a certain catalog localize to a certain language, and initialized with
        // standard defaults. One valid value for riid is IID_IQueryParser.
        [PreserveSig]
        HRESULT CreateLoadedParser([In, MarshalAs(UnmanagedType.LPWStr)] string pszCatalog, [In] ushort langidForKeywords, [In] ref Guid riid, [Out] out IQueryParser ppQueryParser);

        // In addition to setting AQS/NQS and automatic wildcard for the given query parser, this sets up standard named entity handlers and
        // sets the keyboard locale as locale for word breaking.
        [PreserveSig]
        HRESULT InitializeOptions([In] bool fUnderstandNQS, [In] bool fAutoWildCard, [In] IQueryParser pQueryParser);

        // Change one of the settings for the query parser manager, such as the name of the schema binary, or the location of the localized and unlocalized
        // schema binaries. By default, the settings point to the schema binaries used by Windows Shell.
        [PreserveSig]
        HRESULT SetOption([In] QueryParserManagerOption option, [In] PropVariant pOptionValue);

    };

    [ComImport,
    Guid(ShellIIDGuid.IQueryParserManager),
    CoClass(typeof(QueryParserManagerCoClass))]
    internal interface INativeQueryParserManager : IQueryParserManager
    {
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ShellCLSIDGuid.QueryParserManager)]
    internal class QueryParserManagerCoClass
    {
    }

    [ComImport,
    Guid("24264891-E80B-4fd3-B7CE-4FF2FAE8931F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEntity
    {
        // TODO
    }

    /// <summary>
    /// SYSTEMTIME structure with some useful methods
    /// </summary>
    internal struct SYSTEMTIME
    {
        internal ushort wYear;
        internal ushort wMonth;
        internal ushort wDayOfWeek;
        internal ushort wDay;
        internal ushort wHour;
        internal ushort wMinute;
        internal ushort wSecond;
        internal ushort wMilliseconds;

        /// <summary>
        /// Convert form System.DateTime
        /// </summary>
        /// <param name="time"></param>
        internal void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }

        /// <summary>
        /// Convert to System.DateTime
        /// </summary>
        /// <returns></returns>
        internal DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }

        /// <summary>
        /// Convert to System.DateTime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }

    #endregion

#pragma warning restore 108
}
