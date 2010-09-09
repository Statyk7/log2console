//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    ///<summary>
    /// Encapsulates additional configuration needed by NativeTaskDialog
    /// that it can't get from the TASKDIALOGCONFIG struct.
    ///</summary>
    internal class NativeTaskDialogSettings
    {
        internal NativeTaskDialogSettings()
        {
            nativeConfiguration = new TaskDialogNativeMethods.TASKDIALOGCONFIG();

            // Apply standard settings.
            nativeConfiguration.cbSize = (uint)Marshal.SizeOf(nativeConfiguration);
            nativeConfiguration.hwndParent = IntPtr.Zero;
            nativeConfiguration.hInstance = IntPtr.Zero;
            nativeConfiguration.dwFlags = TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
            nativeConfiguration.dwCommonButtons = TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON;
            nativeConfiguration.MainIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION(0);
            nativeConfiguration.FooterIcon = new TaskDialogNativeMethods.TASKDIALOGCONFIG_ICON_UNION(0);
            nativeConfiguration.cxWidth = TaskDialogDefaults.IdealWidth;

            // Zero out all the custom button fields.
            nativeConfiguration.cButtons = 0;
            nativeConfiguration.cRadioButtons = 0;
            nativeConfiguration.pButtons = IntPtr.Zero;
            nativeConfiguration.pRadioButtons = IntPtr.Zero;
            nativeConfiguration.nDefaultButton = 0;
            nativeConfiguration.nDefaultRadioButton = 0;

            // Various text defaults.
            nativeConfiguration.pszWindowTitle = TaskDialogDefaults.Caption;
            nativeConfiguration.pszMainInstruction = TaskDialogDefaults.MainInstruction;
            nativeConfiguration.pszContent = TaskDialogDefaults.Content;
            nativeConfiguration.pszVerificationText = null;
            nativeConfiguration.pszExpandedInformation = null;
            nativeConfiguration.pszExpandedControlText = null;
            nativeConfiguration.pszCollapsedControlText = null;
            nativeConfiguration.pszFooter = null;
        }

        private int progressBarMinimum;
        public int ProgressBarMinimum
        {
            get { return progressBarMinimum; }
            set { progressBarMinimum = value; }
        }

        private int progressBarMaximum;
        public int ProgressBarMaximum
        {
            get { return progressBarMaximum; }
            set { progressBarMaximum = value; }
        }

        private int progressBarValue;
        public int ProgressBarValue
        {
            get { return progressBarValue; }
            set { this.progressBarValue = value; }
        }

        private TaskDialogProgressBarState progressBarState;
        public TaskDialogProgressBarState ProgressBarState
        {
            get { return progressBarState; }
            set { progressBarState = value; }
        }

        private bool invokeHelp;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool InvokeHelp
        {
            get { return invokeHelp; }
            set { invokeHelp = value; }
        }

        private TaskDialogNativeMethods.TASKDIALOGCONFIG nativeConfiguration;
        public TaskDialogNativeMethods.TASKDIALOGCONFIG NativeConfiguration
        {
            get { return nativeConfiguration; }
        }

        private TaskDialogNativeMethods.TASKDIALOG_BUTTON[] buttons;
        public TaskDialogNativeMethods.TASKDIALOG_BUTTON[] Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        private TaskDialogNativeMethods.TASKDIALOG_BUTTON[] radioButtons;
        public TaskDialogNativeMethods.TASKDIALOG_BUTTON[] RadioButtons
        {
            get { return radioButtons; }
            set { radioButtons = value; }
        }

        private List<int> elevatedButtons;
        public List<int> ElevatedButtons
        {
            get { return elevatedButtons; }
            set { elevatedButtons = value; }
        }
    }
}
