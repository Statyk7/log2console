//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal class ProgressBarStateSettings
    {
        internal readonly int DefaultMaxValue = 100;
        internal readonly int DefaultMinValue = 0;
        
        /// <summary>
        /// Represents a collection of name/value pairs for each HWND and it’s 
        /// current progress bar value.
        /// </summary>
        internal IDictionary<IntPtr, int> CurrentValues;

        /// <summary>
        /// Represents a collection of name/value pairs for each HWND and it’s 
        /// current progress bar max values
        /// </summary>
        internal IDictionary<IntPtr, int> MaxValues;

        /// <summary>
        /// Represents a collection of name/value pairs for each HWND and it’s 
        /// current progress bar state.
        /// </summary>
        internal IDictionary<IntPtr, TaskbarProgressBarState> States;

        internal ProgressBarStateSettings()
        {
            CurrentValues = new Dictionary<IntPtr, int>();
            MaxValues = new Dictionary<IntPtr, int>();
            States = new Dictionary<IntPtr, TaskbarProgressBarState>();
        }

        private static ProgressBarStateSettings instance;
        /// <summary>
        /// Returns a singleton instance of the ProgressBarStateSettings class
        /// </summary>
        internal static ProgressBarStateSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProgressBarStateSettings();

                return instance;
            }
        }

        /// <summary>
        /// Represents the HWND for the application or default window. 
        /// </summary>
        internal IntPtr DefaultHandle
        {
            get
            {
                return TaskbarManager.Instance.OwnerHandle;
            }
        }

        /// <summary>
        /// Refreshes the native taskbar with the current progressbar values for the given HWND
        /// </summary>
        /// <param name="hwnd">Current window handle</param>
        /// <param name="currentValue">Current progress bar value</param>
        /// <param name="maxValue">Current progress bar max value</param>
        internal void RefreshValue(IntPtr hwnd, int currentValue, int maxValue)
        {
            TaskbarManager.Instance.TaskbarList.SetProgressValue(hwnd, (ulong)currentValue, (ulong)maxValue);
        }

        /// <summary>
        /// Refreshes the native taskbar with the current progressbar state for the given HWND
        /// </summary>
        /// <param name="hwnd">Current window handle</param>
        /// <param name="state">Current progress bar state</param>
        internal void RefreshState(IntPtr hwnd, TaskbarProgressBarState state)
        {
            TaskbarManager.Instance.TaskbarList.SetProgressState(hwnd, (TBPFLAG)state);
        }

    }
}
