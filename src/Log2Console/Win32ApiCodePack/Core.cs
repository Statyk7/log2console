using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Internal
{
  internal enum STPFLAG
  {
    STPF_NONE = 0x0,
    STPF_USEAPPTHUMBNAILALWAYS = 0x1,
    STPF_USEAPPTHUMBNAILWHENACTIVE = 0x2,
    STPF_USEAPPPEEKALWAYS = 0x4,
    STPF_USEAPPPEEKWHENACTIVE = 0x8
  }

  internal enum TBPFLAG
  {
    TBPF_NOPROGRESS = 0,
    TBPF_INDETERMINATE = 0x1,
    TBPF_NORMAL = 0x2,
    TBPF_ERROR = 0x4,
    TBPF_PAUSED = 0x8
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

  /// <summary>
  /// HRESULT Wrapper
  /// This is intended for Library Internal use only.
  /// </summary>
  public enum HRESULT : uint
  {
    /// <summary>
    /// S_FALSE
    /// </summary>
    S_FALSE = 0x0001,

    /// <summary>
    /// S_OK
    /// </summary>
    S_OK = 0x0000,

    /// <summary>
    /// E_INVALIDARG
    /// </summary>
    E_INVALIDARG = 0x80070057,

    /// <summary>
    /// E_OUTOFMEMORY
    /// </summary>
    E_OUTOFMEMORY = 0x8007000E,

    /// <summary>
    /// E_NOINTERFACE
    /// </summary>
    E_NOINTERFACE = 0x80004002,

    /// <summary>
    /// E_FAIL
    /// </summary>
    E_FAIL = 0x80004005,

    /// <summary>
    /// E_ELEMENTNOTFOUND
    /// </summary>
    E_ELEMENTNOTFOUND = 0x80070490,

    /// <summary>
    /// TYPE_E_ELEMENTNOTFOUND
    /// </summary>
    TYPE_E_ELEMENTNOTFOUND = 0x8002802B,

    /// <summary>
    /// NO_OBJECT
    /// </summary>
    NO_OBJECT = 0x800401E5,

    /// <summary>
    /// Win32 Error code: ERROR_CANCELLED
    /// </summary>
    ERROR_CANCELLED = 1223,

    /// <summary>
    /// ERROR_CANCELLED
    /// </summary>
    E_ERROR_CANCELLED = 0x800704C7,

    /// <summary>
    /// The requested resource is in use
    /// </summary>
    RESOURCE_IN_USE = 0x800700AA,
  }

  [ComImport]
  [Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface ITaskbarList4
  {
    // ITaskbarList
    [PreserveSig]
    void HrInit();

    [PreserveSig]
    void AddTab(IntPtr hwnd);

    [PreserveSig]
    void DeleteTab(IntPtr hwnd);

    [PreserveSig]
    void ActivateTab(IntPtr hwnd);

    [PreserveSig]
    void SetActiveAlt(IntPtr hwnd);

    // ITaskbarList2
    [PreserveSig]
    void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

    // ITaskbarList3
    [PreserveSig]
    void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);

    [PreserveSig]
    void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);

    [PreserveSig]
    void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

    [PreserveSig]
    void UnregisterTab(IntPtr hwndTab);

    [PreserveSig]
    void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

    [PreserveSig]
    void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);

    [PreserveSig]
    HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);

    [PreserveSig]
    HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);

    [PreserveSig]
    void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

    [PreserveSig]
    void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

    [PreserveSig]
    void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

    [PreserveSig]
    void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);

    // ITaskbarList4
    void SetTabProperties(IntPtr hwndTab, STPFLAG stpFlags);
  }

  [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
  [ClassInterface(ClassInterfaceType.None)]
  [ComImport]
  internal class CTaskbarList { }

  internal static class CoreErrorHelper
  {
    /// <summary>
    /// This is intended for Library Internal use only.
    /// </summary>
    /// <param name="hresult">The error code.</param>
    /// <returns>True if the error code indicates success.</returns>
    public static bool Succeeded(int hresult)
    {
      return (hresult >= 0);
    }
  }

  internal class CoreHelpers
  {
    /// <summary>
    /// Determines if the application is running on Windows Vista or later 7
    /// </summary>
    public static bool RunningOnVistaOrLater
    {
      get
      {
        return Environment.OSVersion.Version.Major >= 6;
      }
    }

    /// <summary>
    /// Determines if the application is running on Windows 7
    /// </summary>
    public static bool RunningOnWin7
    {
      get
      {
        return (Environment.OSVersion.Version.Major > 6) ||
            (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
      }
    }

    /// <summary>
    /// Throws PlatformNotSupportedException if the application is not running on Windows 7
    /// </summary>
    public static void ThrowIfNotWin7()
    {
      if (!RunningOnWin7)
        throw new PlatformNotSupportedException("Only supported on Windows 7 or newer.");
    }
  }

  internal static class CoreNativeMethods
  {
    public static int LOWORD(long dword)
    {
      return (short)(dword & 0xFFFF);
    }

    public static int HIWORD(long dword, int size)
    {
      return (short)(dword >> size);
    }
  }
}
