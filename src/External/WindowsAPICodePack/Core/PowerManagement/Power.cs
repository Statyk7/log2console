//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    internal static class Power
    {
        internal static PowerManagementNativeMethods.SystemPowerCapabilities 
            GetSystemPowerCapabilities()
        {
            IntPtr status = IntPtr.Zero;
            PowerManagementNativeMethods.SystemPowerCapabilities powerCap;

            try
            {
                status = Marshal.AllocCoTaskMem(
                Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemPowerCapabilities)));

                uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
                  4,  // SystemPowerCapabilities
                  (IntPtr)null,
                  0,
                  status,
                  (UInt32)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemPowerCapabilities))
                  );

                if (retval == CoreNativeMethods.STATUS_ACCESS_DENIED)
                {
                    throw new UnauthorizedAccessException("The caller had insufficient access rights to get the system power capabilities.");
                }

                powerCap = (PowerManagementNativeMethods.SystemPowerCapabilities)Marshal.PtrToStructure(status, typeof(PowerManagementNativeMethods.SystemPowerCapabilities));
            }
            finally
            {
                Marshal.FreeCoTaskMem(status);
            }

            return powerCap;
        }

        internal static PowerManagementNativeMethods.SystemBatteryState GetSystemBatteryState()
        {
            IntPtr status = IntPtr.Zero;
            PowerManagementNativeMethods.SystemBatteryState batt_status;
        
            try
            {
                status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemBatteryState)));
                uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
                  5,  // SystemBatteryState
                  (IntPtr)null,
                  0,
                  status,
                  (UInt32)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemBatteryState))
                  );

                if (retval == CoreNativeMethods.STATUS_ACCESS_DENIED)
                {
                    throw new UnauthorizedAccessException("The caller had insufficient access rights to get the system battery state.");
                }

                batt_status = (PowerManagementNativeMethods.SystemBatteryState)Marshal.PtrToStructure(status, typeof(PowerManagementNativeMethods.SystemBatteryState));
            }
            finally
            {
                Marshal.FreeCoTaskMem(status);
            }

            return batt_status;
        }

        /// <summary>
        /// Registers the application to receive power setting notifications 
        /// for the specific power setting event.
        /// </summary>
        /// <param name="handle">Handle indicating where the power setting 
        /// notifications are to be sent.</param>
        /// <param name="powerSetting">The GUID of the power setting for 
        /// which notifications are to be sent.</param>
        /// <returns>Returns a notification handle for unregistering 
        /// power notifications.</returns>
        internal static int RegisterPowerSettingNotification(
            IntPtr handle, Guid powerSetting)
        {
            int outHandle = PowerManagementNativeMethods.RegisterPowerSettingNotification(
                handle, 
                ref powerSetting, 
                0);

            return outHandle;
        }

        /// <summary>
        /// Allows an application to inform the system that it 
        /// is in use, thereby preventing the system from entering 
        /// the sleeping power state or turning off the display 
        /// while the application is running.
        /// </summary>
        /// <param name="flags">The thread's execution requirements.</param>
        /// <exception cref="Win32Exception">Thrown if the SetThreadExecutionState call fails.</exception>
        internal static void SetThreadExecutionState(ExecutionState flags)
        {
            ExecutionState? ret = PowerManagementNativeMethods.SetThreadExecutionState(flags);
            if (ret == null)
                throw new Win32Exception("SetThreadExecutionState call failed.");
        }
    }
}