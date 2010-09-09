//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    /// <summary>
    /// This class generates .NET events based on Windows messages.  
    /// The PowerRegWindow class processes the messages from Windows.
    /// </summary>
    internal static class MessageManager
    {
        private static object lockObject = new object();
        private static PowerRegWindow window;

        #region Internal static methods

        /// <summary>
        /// Registers a callback for a power event.
        /// </summary>
        /// <param name="eventId">Guid for the event.</param>
        /// <param name="eventToRegister">Event handler for the specified event.</param>
        internal static void RegisterPowerEvent(Guid eventId, EventHandler eventToRegister)
        {
            EnsureInitialized();
            window.RegisterPowerEvent(eventId, eventToRegister);
        }

        /// <summary>
        /// Unregisters an event handler for a power event.
        /// </summary>
        /// <param name="eventId">Guid for the event.</param>
        /// <param name="eventToUnregister">Event handler to unregister.</param>
        internal static void UnregisterPowerEvent(Guid eventId, EventHandler eventToUnregister)
        {
            EnsureInitialized();
            window.UnregisterPowerEvent(eventId, eventToUnregister);
        }

        #endregion

        /// <summary>
        /// Ensures that the hidden window is initialized and 
        /// listening for messages.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (window == null)
            {
                lock (lockObject)
                {
                    if (window == null)
                    {
                        // Create a new hidden window to listen
                        // for power management related window messages.
                        window = new PowerRegWindow();
                    }
                }
            }
        }

        /// <summary>
        /// Catch Windows messages and generates events for power specific
        /// messages.
        /// </summary>
        internal class PowerRegWindow : Form
        {
            private Hashtable eventList = new Hashtable();
            private ReaderWriterLock readerWriterLock = new ReaderWriterLock();

            internal PowerRegWindow()
                : base()
            {

            }

            #region Internal Methods

            /// <summary>
            /// Adds an event handler to call when Windows sends 
            /// a message for an evebt.
            /// </summary>
            /// <param name="eventId">Guid for the event.</param>
            /// <param name="eventToRegister">Event handler for the event.</param>
            internal void RegisterPowerEvent(Guid eventId, EventHandler eventToRegister)
            {
                readerWriterLock.AcquireWriterLock(Timeout.Infinite);
                if (!eventList.Contains(eventId))
                {
                    Power.RegisterPowerSettingNotification(this.Handle, eventId);
                    ArrayList newList = new ArrayList();
                    newList.Add(eventToRegister);
                    eventList.Add(eventId, newList);
                }
                else
                {
                    ArrayList currList = (ArrayList)eventList[eventId];
                    currList.Add(eventToRegister);
                }
                readerWriterLock.ReleaseWriterLock();
            }

            /// <summary>
            /// Removes an event handler.
            /// </summary>
            /// <param name="eventId">Guid for the event.</param>
            /// <param name="eventToUnregister">Event handler to remove.</param>
            /// <exception cref="InvalidOperationException">Cannot unregister 
            /// a function that is not registered.</exception>
            internal void UnregisterPowerEvent(Guid eventId, EventHandler eventToUnregister)
            {
                readerWriterLock.AcquireWriterLock(Timeout.Infinite);
                if (eventList.Contains(eventId))
                {
                    ArrayList currList = (ArrayList)eventList[eventId];
                    currList.Remove(eventToUnregister);
                }
                else
                {
                    throw new InvalidOperationException(
                        "The specified event handler has not been registered.");
                }
                readerWriterLock.ReleaseWriterLock();
            }

            #endregion

            /// <summary>
            /// Executes any registered event handlers.
            /// </summary>
            /// <param name="eventHandlerList">ArrayList of event handlers.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            private static void ExecuteEvents(ArrayList eventHandlerList)
            {
                ArrayList tempList = (ArrayList)eventHandlerList.Clone();
                foreach (EventHandler handler in tempList)
                {
                    try
                    {
                        if (handler != null)
                            handler.Invoke(null, new EventArgs());
                    }
                    // Don't crash if an event handler throws an exception.
                    catch { ;}
                }
            }

            /// <summary>
            /// This method is called when a Windows message 
            /// is sent to this window.
            /// The method calls the registered event handlers.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                // Make sure it is a Power Management message.
                if (m.Msg == PowerManagementNativeMethods.WM_POWERBROADCAST && (int)m.WParam == PowerManagementNativeMethods.PBT_POWERSETTINGCHANGE)
                {
                    PowerManagementNativeMethods.PowerBroadcastSetting ps =
                         (PowerManagementNativeMethods.PowerBroadcastSetting)Marshal.PtrToStructure(
                             m.LParam, typeof(PowerManagementNativeMethods.PowerBroadcastSetting));
                    IntPtr pData = new IntPtr(m.LParam.ToInt64() + Marshal.SizeOf(ps));
                    Guid currentEvent = ps.PowerSetting;

                    // Update the appropriate Property.
                    // Power Personality
                    if (ps.PowerSetting == EventManager.PowerPersonalityChange &&
                        ps.DataLength == Marshal.SizeOf(typeof(Guid)))
                    {
                        Guid newPersonality =
                            (Guid)Marshal.PtrToStructure(pData, typeof(Guid));

                        PowerManager.powerPersonality = PersonalityGuids.GuidToEnum(newPersonality);
                        // Tell PowerManager that is now safe to 
                        // read the powerPersonality member.
                        EventManager.personalityReset.Set();
                    }
                    // Power Source
                    else if (ps.PowerSetting == EventManager.PowerSourceChange &&
                         ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                    {
                        Int32 powerSrc = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                        PowerManager.powerSource = (PowerSource)powerSrc;
                        EventManager.powerSrcReset.Set();
                    }
                    // Battery capacity
                    else if (ps.PowerSetting == EventManager.BatteryCapacityChange &&
                        ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                    {
                        Int32 battCapacity = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                        PowerManager.batteryLifePercent = battCapacity;
                        EventManager.batteryLifeReset.Set();
                    }
                    // IsMonitorOn
                    else if (ps.PowerSetting == EventManager.MonitorPowerStatus &&
                        ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                    {
                        Int32 monitorStatus = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                        PowerManager.isMonitorOn = monitorStatus == 0 ? false : true;
                        EventManager.monitorOnReset.Set();
                    }

                    if (!EventManager.IsMessageCaught(currentEvent))
                        ExecuteEvents((ArrayList)eventList[currentEvent]);
                }
                else
                    base.WndProc(ref m);

            }

        }
    }
}
