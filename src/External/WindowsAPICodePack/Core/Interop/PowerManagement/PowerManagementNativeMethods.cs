//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    internal class PowerManagementNativeMethods
    {
        private PowerManagementNativeMethods()
        {

        }

        #region Power Management

        internal const uint WM_POWERBROADCAST = 536;
        internal const uint PBT_POWERSETTINGCHANGE = 32787;
        internal const uint SPI_SETSCREENSAVEACTIVE = 0x0011;
        internal const uint SPIF_UPDATEINIFILE = 0x0001;
        internal const uint SPIF_SENDCHANGE = 0x0002;

        // This structure is sent when the PBT_POWERSETTINGSCHANGE message is sent.
        // It describes the power setting that has changed and 
        // contains data about the change.
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PowerBroadcastSetting
        {
            internal Guid PowerSetting;
            internal Int32 DataLength;
        }

        // This structure is used when calling CallNtPowerInformation 
        // to retrieve SystemPowerCapabilities
        [StructLayout(LayoutKind.Sequential)]
        internal struct SystemPowerCapabilities
        {
            [MarshalAs(UnmanagedType.I1)]
            internal bool PowerButtonPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SleepButtonPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool LidPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemS1;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemS2;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemS3;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemS4;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemS5;
            [MarshalAs(UnmanagedType.I1)]
            internal bool HiberFilePresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool FullWake;
            [MarshalAs(UnmanagedType.I1)]
            internal bool VideoDimPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool ApmPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool UpsPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool ThermalControl;
            [MarshalAs(UnmanagedType.I1)]
            internal bool ProcessorThrottle;
            internal byte ProcessorMinThrottle;
            internal byte ProcessorMaxThrottle;
            [MarshalAs(UnmanagedType.I1)]
            internal bool FastSystemS4;
            internal byte spare2_1;
            internal byte spare2_2;
            internal byte spare2_3;
            [MarshalAs(UnmanagedType.I1)]
            internal bool DiskSpinDown;
            internal byte spare3_1;
            internal byte spare3_2;
            internal byte spare3_3;
            internal byte spare3_4;
            internal byte spare3_5;
            internal byte spare3_6;
            internal byte spare3_7;
            internal byte spare3_8;
            [MarshalAs(UnmanagedType.I1)]
            internal bool SystemBatteriesPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool BatteriesAreShortTerm;
            internal int granularity;
            internal int capacity;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SystemBatteryState
        {
            [MarshalAs(UnmanagedType.I1)]
            internal bool AcOnLine;
            [MarshalAs(UnmanagedType.I1)]
            internal bool BatteryPresent;
            [MarshalAs(UnmanagedType.I1)]
            internal bool Charging;
            [MarshalAs(UnmanagedType.I1)]
            internal bool Discharging;
            internal byte spare1;
            internal byte spare2;
            internal byte spare3;
            internal byte spare4;
            internal uint MaxCapacity;
            internal uint RemainingCapacity;
            internal uint Rate;
            internal uint EstimatedTime;
            internal uint DefaultAlert1;
            internal uint DefaultAlert2;
        }
        [DllImport("powrprof.dll", SetLastError = true)]
        internal static extern UInt32 CallNtPowerInformation(
             Int32 InformationLevel,
             IntPtr lpInputBuffer,
             UInt32 nInputBufferSize,
             IntPtr lpOutputBuffer,
             UInt32 nOutputBufferSize
        );

        [DllImport("User32", SetLastError = true,
            EntryPoint = "RegisterPowerSettingNotification",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern int RegisterPowerSettingNotification(
                IntPtr hRecipient,
                ref Guid PowerSettingGuid,
                Int32 Flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        #endregion
    }
}
