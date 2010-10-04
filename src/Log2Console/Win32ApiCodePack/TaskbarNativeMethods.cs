using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Internal
{
  internal static class TaskbarNativeMethods
  {
    internal const int WM_COMMAND = 0x0111;
    
    [DllImport(CommonDllNames.Shell32)]
    internal static extern void GetCurrentProcessExplicitAppUserModelID([Out, MarshalAs(UnmanagedType.LPWStr)] out string AppID);

    [DllImport(CommonDllNames.Shell32)]
    internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    [DllImport(CommonDllNames.User32, EntryPoint = "RegisterWindowMessage", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

    internal static readonly uint WM_TASKBARBUTTONCREATED = RegisterWindowMessage("TaskbarButtonCreated");
  }
}
