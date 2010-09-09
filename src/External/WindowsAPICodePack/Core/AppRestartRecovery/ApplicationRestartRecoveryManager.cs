//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    /// <summary>
    /// Provides access to the Application Restart and Recovery
    /// features available in Windows Vista or higher. Application Restart and Recovery lets an
    /// application do some recovery work to save data before the process exits.
    /// </summary>
    public static class ApplicationRestartRecoveryManager
    {
        /// <summary>
        /// Registers an application for recovery by Application Restart and Recovery.
        /// </summary>
        /// <param name="settings">An object that specifies
        /// the callback method, an optional parameter to pass to the callback
        /// method and a time interval.</param>
        /// <exception cref="System.ArgumentException">
        /// The registration failed due to an invalid parameter.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The registration failed.</exception>
        /// <remarks>The time interval is the period of time within 
        /// which the recovery callback method 
        /// calls the <see cref="ApplicationRecoveryInProgress"/> method to indicate
        /// that it is still performing recovery work.</remarks>
        public static void RegisterForApplicationRecovery(RecoverySettings settings)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            if (settings == null)
                throw new ArgumentNullException("settings");

            GCHandle handle = GCHandle.Alloc(settings.RecoveryData);

            HRESULT hr = AppRestartRecoveryNativeMethods.RegisterApplicationRecoveryCallback(AppRestartRecoveryNativeMethods.internalCallback, (IntPtr)handle, settings.PingInterval, (uint)0);

            if (!CoreErrorHelper.Succeeded((int)hr))
            {
                if (hr == HRESULT.E_INVALIDARG)
                    throw new ArgumentException("Application was not registered for recovery due to bad parameters.");
                else
                    throw new ExternalException("Application failed to register for recovery.");
            }
        }

        /// <summary>
        /// Removes an application's recovery registration.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The attempt to unregister for recovery failed.</exception>
        public static void UnregisterApplicationRecovery()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            HRESULT hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRecoveryCallback();

            if (hr == HRESULT.E_FAIL)
                throw new ExternalException("Unregister for recovery failed.");
        }

        /// <summary>
        /// Removes an application's restart registration.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The attempt to unregister for restart failed.</exception>
        public static void UnregisterApplicationRestart()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            HRESULT hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRestart();

            if (hr == HRESULT.E_FAIL)
                throw new ExternalException("Unregister for restart failed.");
        }

        /// <summary>
        /// Called by an application's <see cref="RecoveryCallback"/> method 
        /// to indicate that it is still performing recovery work.
        /// </summary>
        /// <returns>A <see cref="System.Boolean"/> value indicating whether the user
        /// canceled the recovery.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// This method must be called from a registered callback method.</exception>
        public static bool ApplicationRecoveryInProgress()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            bool canceled = false;

            HRESULT hr = AppRestartRecoveryNativeMethods.ApplicationRecoveryInProgress(out canceled);

            if (hr == HRESULT.E_FAIL)
                throw new InvalidOperationException("This method must be called from the registered callback method.");

            return canceled;
        }

        /// <summary>
        /// Called by an application's <see cref="RecoveryCallback"/> method to 
        /// indicate that the recovery work is complete.
        /// </summary>
        /// <remarks>
        /// This should
        /// be the last call made by the <see cref="RecoveryCallback"/> method because
        /// Windows Error Reporting will terminate the application
        /// after this method is invoked.
        /// </remarks>
        /// <param name="success"><b>true</b> to indicate the the program was able to complete its recovery
        /// work before terminating; otherwise <b>false</b>.</param>
        public static void ApplicationRecoveryFinished(bool success)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            AppRestartRecoveryNativeMethods.ApplicationRecoveryFinished(success);
        }

        /// <summary>
        /// Registers an application for automatic restart if 
        /// the application 
        /// is terminated by Windows Error Reporting.
        /// </summary>
        /// <param name="settings">An object that specifies
        /// the command line arguments used to restart the 
        /// application, and 
        /// the conditions under which the application should not be 
        /// restarted.</param>
        /// <exception cref="System.ArgumentException">Registration failed due to an invalid parameter.</exception>
        /// <exception cref="System.InvalidOperationException">The attempt to register failed.</exception>
        /// <remarks>A registered application will not be restarted if it executed for less than 60 seconds before terminating.</remarks>
        public static void RegisterForApplicationRestart(RestartSettings settings)
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            HRESULT hr = AppRestartRecoveryNativeMethods.RegisterApplicationRestart(settings.Command, settings.Restrictions);

            if (hr == HRESULT.E_FAIL)
                throw new InvalidOperationException("Application failed to registered for restart.");

            if (hr == HRESULT.E_INVALIDARG)
                throw new ArgumentException("Failed to register application for restart due to bad parameters.");
        }
       
    }
}

