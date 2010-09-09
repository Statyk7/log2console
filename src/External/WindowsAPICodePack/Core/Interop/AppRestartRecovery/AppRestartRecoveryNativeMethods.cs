//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    internal static class AppRestartRecoveryNativeMethods
    {
        #region Application Restart and Recovery Definitions

        internal delegate UInt32 InternalRecoveryCallback(IntPtr state); 
        
        internal static InternalRecoveryCallback internalCallback;

        static AppRestartRecoveryNativeMethods()
        {
            internalCallback = new InternalRecoveryCallback(InternalRecoveryHandler);
        }

        private static UInt32 InternalRecoveryHandler(IntPtr parameter)
        {
            bool cancelled = false;
            ApplicationRecoveryInProgress(out cancelled);

            GCHandle handle = GCHandle.FromIntPtr(parameter);
            RecoveryData data = handle.Target as RecoveryData;
            data.Invoke();
            handle.Free();

            return (0);
        }



        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished(
           [MarshalAs(UnmanagedType.Bool)] bool success);

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT ApplicationRecoveryInProgress(
            [Out, MarshalAs(UnmanagedType.Bool)] out bool canceled);

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT GetApplicationRecoveryCallback(
            IntPtr processHandle,
            out RecoveryCallback recoveryCallback,
            out object state,
            out uint pingInterval,
            out uint flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [PreserveSig]
        internal static extern HRESULT RegisterApplicationRecoveryCallback(
            InternalRecoveryCallback callback, IntPtr param,
            uint pingInterval,
            uint flags); // Unused.


        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT RegisterApplicationRestart(
            [MarshalAs(UnmanagedType.BStr)] string commandLineArgs,
            RestartRestrictions flags);

        [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [PreserveSig]
        internal static extern HRESULT GetApplicationRestartSettings(
            IntPtr process,
            IntPtr commandLine,
            ref uint size,
            out RestartRestrictions flags);

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT UnregisterApplicationRecoveryCallback();

        [DllImport("kernel32.dll")]
        [PreserveSig]
        internal static extern HRESULT UnregisterApplicationRestart();

        #endregion
    }
}
