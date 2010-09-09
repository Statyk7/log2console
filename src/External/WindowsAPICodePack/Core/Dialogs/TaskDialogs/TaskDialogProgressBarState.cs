//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Sets the state of a task dialog progress bar.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum TaskDialogProgressBarState
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal = TaskDialogNativeMethods.PBST.PBST_NORMAL,

        /// <summary>
        /// An error occurred.
        /// </summary>
        Error = TaskDialogNativeMethods.PBST.PBST_ERROR,

        /// <summary>
        /// The progress is paused.
        /// </summary>
        Paused = TaskDialogNativeMethods.PBST.PBST_PAUSED,

        /// <summary>
        /// Displays marquee (indeterminate) style progress
        /// </summary>
        Marquee,
    }
}
