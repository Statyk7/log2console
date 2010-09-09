//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using MS.WindowsAPICodePack.Internal;

namespace MS.WindowsAPICodePack.Internal
{
    internal static class DWMMessages
    {
        internal const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        internal const int WM_DWMNCRENDERINGCHANGED = 0x031F;
    }
    
    [StructLayout( LayoutKind.Sequential )]
    internal struct MARGINS
    {
        public int cxLeftWidth;      // width of left border that retains its size
        public int cxRightWidth;     // width of right border that retains its size
        public int cyTopHeight;      // height of top border that retains its size
        public int cyBottomHeight;   // height of bottom border that retains its size

        public MARGINS( bool fullWindow )
        {
            cxLeftWidth = cxRightWidth = cyTopHeight = cyBottomHeight = (fullWindow ? -1 : 0);
        }
    };

    internal enum DwmBlurBehindDwFlags : uint
    {
        DWM_BB_ENABLE = 0x00000001,
        DWM_BB_BLURREGION = 0x00000002,
        DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004
    }

    [StructLayout( LayoutKind.Sequential )]
    internal struct DWM_BLURBEHIND
    {
        public DwmBlurBehindDwFlags dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    };

    internal enum CompositionEnable : uint
    {
        DWM_EC_DISABLECOMPOSITION = 0,
        DWM_EC_ENABLECOMPOSITION = 1
    }
    
    /// <summary>
    /// Internal class that contains interop declarations for 
    /// functions that are not benign and are performance critical. 
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class DesktopWindowManagerNativeMethods
    {
        [DllImport( "DwmApi.dll" )]
        internal static extern int DwmEnableBlurBehindWindow(
            IntPtr hwnd,
            ref DWM_BLURBEHIND bb );

        [DllImport( "DwmApi.dll" )]
        internal static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS m );

        [DllImport( "DwmApi.dll", PreserveSig = false )]
        internal static extern bool DwmIsCompositionEnabled( );

        [DllImport( "DwmApi.dll" )]
        internal static extern int DwmEnableComposition(
            CompositionEnable compositionAction );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern bool GetWindowRect( IntPtr hwnd, ref CoreNativeMethods.RECT rect );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern bool GetClientRect( IntPtr hwnd, ref CoreNativeMethods.RECT rect );
    }
}
