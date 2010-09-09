//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    internal static class DialogsDefaults
    {
        internal const string Caption = "Application";
        internal const string MainInstruction = "";
        internal const string Content = "";

        internal const int ProgressBarStartingValue = 0;
        internal const int ProgressBarMinimumValue = 0;
        internal const int ProgressBarMaximumValue = 100;

        internal const int IdealWidth = 0;

        // For generating control ID numbers that won't 
        // collide with the standard button return IDs.
        internal const int MinimumDialogControlId =
            (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCLOSE + 1;
    }
}
