//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    #region Enums
    internal enum KNOWNDESTCATEGORY
    {
        KDC_FREQUENT = 1,
        KDC_RECENT
    }

    internal enum SHARD
    {
        SHARD_PIDL = 0x1,
        SHARD_PATHA = 0x2,
        SHARD_PATHW = 0x3,
        SHARD_APPIDINFO = 0x4,       // indicates the data type is a pointer to a SHARDAPPIDINFO structure
        SHARD_APPIDINFOIDLIST = 0x5, // indicates the data type is a pointer to a SHARDAPPIDINFOIDLIST structure
        SHARD_LINK = 0x6,            // indicates the data type is a pointer to an IShellLink instance
        SHARD_APPIDINFOLINK = 0x7,   // indicates the data type is a pointer to a SHARDAPPIDINFOLINK structure 
    }
    
    internal enum TBPFLAG
    {
        TBPF_NOPROGRESS = 0,
        TBPF_INDETERMINATE = 0x1,
        TBPF_NORMAL = 0x2,
        TBPF_ERROR = 0x4,
        TBPF_PAUSED = 0x8
    }

    internal enum TBATFLAG
    {
        TBATF_USEMDITHUMBNAIL = 0x1,
        TBATF_USEMDILIVEPREVIEW = 0x2
    }

    internal enum THBMASK
    {
        THB_BITMAP = 0x1,
        THB_ICON = 0x2,
        THB_TOOLTIP = 0x4,
        THB_FLAGS = 0x8
    }

    [Flags]
    internal enum THBFLAGS
    {
        THBF_ENABLED = 0x00000000,
        THBF_DISABLED = 0x00000001,
        THBF_DISMISSONCLICK = 0x00000002,
        THBF_NOBACKGROUND = 0x00000004,
        THBF_HIDDEN = 0x00000008,
        THBF_NONINTERACTIVE = 0x00000010
    }

    internal enum STPFLAG
    {
        STPF_NONE = 0x0,
        STPF_USEAPPTHUMBNAILALWAYS = 0x1,
        STPF_USEAPPTHUMBNAILWHENACTIVE = 0x2,
        STPF_USEAPPPEEKALWAYS = 0x4,
        STPF_USEAPPPEEKWHENACTIVE = 0x8
    }

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Explicit)]
    internal struct CALPWSTR
    {
        [FieldOffset(0)]
        internal uint cElems;
        [FieldOffset(4)]
        internal IntPtr pElems;
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct THUMBBUTTON
    {
        /// <summary>
        /// WPARAM value for a THUMBBUTTON being clicked.
        /// </summary>
        internal const int THBN_CLICKED = 0x1800;

        [MarshalAs(UnmanagedType.U4)]
        internal THBMASK dwMask;
        internal uint iId;
        internal uint iBitmap;
        internal IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szTip;
        [MarshalAs(UnmanagedType.U4)]
        internal THBFLAGS dwFlags;
    }
    #endregion;

    internal class TaskbarNativeMethods
    {
        internal static readonly uint DWM_SIT_DISPLAYFRAME = 0x00000001;

        internal static Guid IID_IObjectArray = new Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9");
        internal static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        internal const int WM_COMMAND = 0x0111;

        // Register Window Message used by Shell to notify that the corresponding taskbar button has been added to the taskbar.
        internal static readonly uint WM_TASKBARBUTTONCREATED = RegisterWindowMessage("TaskbarButtonCreated");

        internal static readonly uint WM_DWMSENDICONICTHUMBNAIL = 0x0323;
        internal static readonly uint WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;

        private TaskbarNativeMethods()
        {
            // Hide the default constructor as this is a static class and we don't want to instantiate it.
        }


        #region Methods

        [DllImport(CommonDllNames.Shell32,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        internal static extern uint SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            // The following parameter is not used - binding context.
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport(CommonDllNames.Shell32)]
        internal static extern void SetCurrentProcessExplicitAppUserModelID(
            [MarshalAs(UnmanagedType.LPWStr)] string AppID);

        [DllImport(CommonDllNames.Shell32)]
        internal static extern void GetCurrentProcessExplicitAppUserModelID(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] out string AppID);

        [DllImport(CommonDllNames.Shell32)]
        internal static extern void SHAddToRecentDocs(
            SHARD flags,
            [MarshalAs(UnmanagedType.LPWStr)] string path);

        internal static void SHAddToRecentDocs(string path)
        {
            SHAddToRecentDocs(SHARD.SHARD_PATHW, path);
        }

        [DllImport(CommonDllNames.User32)]
        internal static extern int GetWindowText(
            IntPtr hwnd, StringBuilder str, int maxCount);

        [DllImport(CommonDllNames.User32, EntryPoint = "RegisterWindowMessage", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);


        [DllImport(CommonDllNames.Shell32)]
        public static extern int SHGetPropertyStoreForWindow(
            IntPtr hwnd,
            ref Guid iid /*IID_IPropertyStore*/,
            [Out(), MarshalAs(UnmanagedType.Interface)]
                out IPropertyStore propertyStore);

        /// <summary>
        /// Sets the window's application id by its window handle.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="appId">The application id.</param>
        internal static void SetWindowAppId(IntPtr hwnd, string appId)
        {
            SetWindowProperty(hwnd, SystemProperties.System.AppUserModel.ID, appId);
        }

        internal static void SetWindowProperty(IntPtr hwnd, PropertyKey propkey, string value)
        {
            // Get the IPropertyStore for the given window handle
            IPropertyStore propStore = GetWindowPropertyStore(hwnd);
            
            // Set the value
            PropVariant pv = new PropVariant();
            propStore.SetValue(ref propkey, ref pv);

            // Dispose the IPropertyStore and PropVariant
            Marshal.ReleaseComObject(propStore);
            pv.Clear();
        }

        internal static IPropertyStore GetWindowPropertyStore(IntPtr hwnd)
        {
            IPropertyStore propStore;
            Guid guid = new Guid(ShellIIDGuid.IPropertyStore);
            int rc = SHGetPropertyStoreForWindow(
                hwnd,
                ref guid,
                out propStore);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
            return propStore;
        }

        #endregion
    }
}
