//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    /// <summary>
    /// A snapshot of the state of the battery.
    /// </summary>
    public class BatteryState
    {
        private bool acOnline;
        private int maxCharge;
        private int currentCharge;
        private int dischargeRate;
        private TimeSpan estimatedTimeRemaining;
        private int suggestedCriticalBatteryCharge;
        private int suggestedBatteryWarningCharge;

        internal BatteryState()
        {
            PowerManagementNativeMethods.SystemBatteryState battState = Power.GetSystemBatteryState();
            acOnline = battState.AcOnLine;
            maxCharge = (int)battState.MaxCapacity;
            currentCharge = (int)battState.RemainingCapacity;
            dischargeRate = (int)battState.Rate;
            long estimatedTime = (long)battState.EstimatedTime;
            int minutes = (int)(estimatedTime/60);
            int seconds = (int)(estimatedTime % 60);
            estimatedTimeRemaining = new TimeSpan(0, minutes, seconds);
            suggestedCriticalBatteryCharge = (int)battState.DefaultAlert1;
            suggestedBatteryWarningCharge = (int)battState.DefaultAlert2;
        }

        #region Public properties

        /// <summary>
        /// Gets a value that indicates whether the battery charger is 
        /// operating on external power.
        /// </summary>
        /// <value>A <see cref="System.Boolean"/> value. <b>True</b> indicates the battery charger is operating on AC power.</value>
        public bool ACOnline 
        { 
            get 
            {
                return acOnline;
            } 
        }

        /// <summary>
        /// Gets the maximum charge of the battery (in mW).
        /// </summary>
        /// <value>An <see cref="System.Int32"/> value.</value>
        public int MaxCharge 
        { 
            get 
            {
                return maxCharge;
            } 
        }

        /// <summary>
        /// Gets the current charge of the battery (in mW).
        /// </summary>
        /// <value>An <see cref="System.Int32"/> value.</value>
        public int CurrentCharge 
        { 
            get 
            {
                return currentCharge;
            } 
        }

        /// <summary>
        /// Gets the rate of discharge for the battery (in mW). 
        /// </summary>
        /// <remarks>
        /// A negative value indicates the
        /// charge rate. Not all batteries support charge rate.
        /// </remarks>
        /// <value>An <see cref="System.Int32"/> value.</value>
        public int DischargeRate 
        { 
            get 
            {
                return dischargeRate;
            } 
        }

        /// <summary>
        /// Gets the estimated time remaining until the battery is empty.
        /// </summary>
        /// <value>A <see cref="System.TimeSpan"/> object.</value>
        public TimeSpan EstimatedTimeRemaining 
        { 
            get 
            {
                return estimatedTimeRemaining;
            } 
        }

        /// <summary>
        /// Gets the manufacturer's suggested battery charge level 
        /// that should cause a critical alert to be sent to the user.
        /// </summary>
        /// <value>An <see cref="System.Int32"/> value.</value>
        public int SuggestedCriticalBatteryCharge 
        { 
            get 
            {
                return suggestedCriticalBatteryCharge;
            } 
        }

        /// <summary>
        /// Gets the manufacturer's suggested battery charge level
        /// that should cause a warning to be sent to the user.
        /// </summary>
        /// <value>An <see cref="System.Int32"/> value.</value>
        public int SuggestedBatteryWarningCharge 
        { 
            get 
            {
                return suggestedBatteryWarningCharge;
            } 
        }

        #endregion

        /// <summary>
        /// Generates a string that represents this <b>BatteryState</b> object.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this object's current state.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"
            , Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return string.Format(
                "ACOnline: {1}{0}Max Charge: {2} mWh{0}Current Charge: {3} mWh{0}Discharge Rate: {4} mWh{0}Estimated Time Remaining: {5}{0}Suggested Critical Battery Charge: {6} mWh{0}Suggested Battery Warning Charge: {7} mWh{0}",
                Environment.NewLine, 
                acOnline, 
                maxCharge, 
                currentCharge, 
                dischargeRate, 
                estimatedTimeRemaining, 
                suggestedCriticalBatteryCharge, 
                suggestedBatteryWarningCharge
                );
        }
    }
}
