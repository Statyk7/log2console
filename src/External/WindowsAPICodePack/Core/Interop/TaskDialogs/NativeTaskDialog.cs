//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Encapsulates the native logic required to create, 
    /// configure, and show a TaskDialog, 
    /// via the TaskDialogIndirect() Win32 function.
    /// </summary>
    /// <remarks>A new instance of this class should 
    /// be created for each messagebox show, as
    /// the HWNDs for TaskDialogs do not remain constant 
    /// across calls to TaskDialogIndirect.
    /// </remarks>
    internal class NativeTaskDialog : IDisposable
    {
        private TaskDialogNativeMethods.TASKDIALOGCONFIG nativeDialogConfig;
        private NativeTaskDialogSettings settings;
        private IntPtr hWndDialog;
        private TaskDialog outerDialog;

        private IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(TaskDialogNativeMethods.TASKDIALOG_ELEMENTS)).Length];
        private IntPtr buttonArray, radioButtonArray;

        // Flag tracks whether our first radio 
        // button click event has come through.
        private bool firstRadioButtonClicked = true;

        #region Constructors

        // Configuration is applied at dialog creation time.
        internal NativeTaskDialog( 
            NativeTaskDialogSettings settings,
            TaskDialog outerDialog)
        {
            nativeDialogConfig = settings.NativeConfiguration;
            this.settings = settings;

            // Wireup dialog proc message loop for this instance.
            nativeDialogConfig.pfCallback = 
                new TaskDialogNativeMethods.PFTASKDIALOGCALLBACK(DialogProc);

            // Keep a reference to the outer shell, so we can notify.
            this.outerDialog = outerDialog;
        }

        #endregion

        #region Public Properties

        private DialogShowState showState = 
            DialogShowState.PreShow;

        public DialogShowState ShowState
        {
            get { return showState; }
        }

        private int selectedButtonID;
        internal int SelectedButtonID
        {
            get { return selectedButtonID; }
        }

        private int selectedRadioButtonID;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int SelectedRadioButtonID
        {
            get { return selectedRadioButtonID; }
        }

        private bool checkBoxChecked;
        internal bool CheckBoxChecked
        {
            get { return checkBoxChecked; }
        }

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        internal void NativeShow()
        {
            // Applies config struct and other settings, then
            // calls main Win32 function.
            if (settings == null)
                throw new InvalidOperationException(
                    "An error has occurred in dialog configuration.");

            // Do a last-minute parse of the various dialog control lists,  
            // and only allocate the memory at the last minute.

            MarshalDialogControlStructs();
            // Make the call and show the dialog.
            // NOTE: this call is BLOCKING, though the thread 
            // WILL re-enter via the DialogProc.
            try
            {
                showState = DialogShowState.Showing;

                // Here is the way we use "vanilla" P/Invoke to call 
                // TaskDialogIndirect().  
                HRESULT hresult = TaskDialogNativeMethods.TaskDialogIndirect(
                    nativeDialogConfig,
                    out selectedButtonID,
                    out selectedRadioButtonID,
                    out checkBoxChecked);

                if (CoreErrorHelper.Failed(hresult))
                {
                    string msg;
                    switch (hresult)
                    {
                        case HRESULT.E_INVALIDARG:
                            msg = "Invalid arguments to Win32 call.";
                            break;
                        case HRESULT.E_OUTOFMEMORY:
                            msg = "Dialog contents too complex.";
                            break;
                        default:
                            msg = String.Format(

                                "An unexpected internal error occurred in the Win32 call:{0:x}",
                                hresult);
                            break;
                    }
                    Exception e = Marshal.GetExceptionForHR((int)hresult);
                    throw new Win32Exception(msg, e);
                }
            }
            catch (EntryPointNotFoundException)
            {
                throw new NotSupportedException("TaskDialog feature needs to load version 6 of comctl32.dll but a different version is current loaded in memory.");
            }
            finally
            {
                showState = DialogShowState.Closed;
            }
        }

        // The new task dialog does not support the existing 
        // Win32 functions for closing (e.g. EndDialog()); instead,
        // a "click button" message is sent. In this case, we're 
        // abstracting out to say that the TaskDialog consumer can
        // simply call "Close" and we'll "click" the cancel button. 
        // Note that the cancel button doesn't actually
        // have to exist for this to work.
        internal void NativeClose(TaskDialogResult result)
        {
            showState = DialogShowState.Closing;

            int id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCANCEL;

            if(result == TaskDialogResult.Close)
                id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCLOSE;
            else if(result == TaskDialogResult.CustomButtonClicked)
                id = DialogsDefaults.MinimumDialogControlId; // custom buttons
            else if(result == TaskDialogResult.No)
                id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDNO;
            else if(result == TaskDialogResult.Ok)
                id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDOK;
            else if(result == TaskDialogResult.Retry)
                id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDRETRY;
            else if(result == TaskDialogResult.Yes)
                id = (int)TaskDialogNativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDYES;

            SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_CLICK_BUTTON, id, 0);
        }

        #region Main Dialog Proc

        private int DialogProc(
            IntPtr hwnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr lpRefData)
        {
            // Fetch the HWND - it may be the first time we're getting it.
            hWndDialog = hwnd;

            // Big switch on the various notifications the 
            // dialog proc can get.
            switch ((TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS)msg)
            {
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_CREATED:
                    int result = PerformDialogInitialization();
                    outerDialog.RaiseOpenedEvent();
                    return result;
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_BUTTON_CLICKED:
                    return HandleButtonClick((int)wParam);
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_RADIO_BUTTON_CLICKED:
                    return HandleRadioButtonClick((int)wParam);
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_HYPERLINK_CLICKED:
                    return HandleHyperlinkClick(lParam);
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_HELP:
                    return HandleHelpInvocation();
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_TIMER:
                    return HandleTick((int)wParam);
                case TaskDialogNativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_DESTROYED:
                    return PerformDialogCleanup();
                default:
                    break;
            }
            return (int)HRESULT.S_OK;
        }


        // Once the task dialog HWND is open, we need to send 
        // additional messages to configure it.
        private int PerformDialogInitialization()
        {
            // Initialize Progress or Marquee Bar.
            if (IsOptionSet(TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR))
            {
                UpdateProgressBarRange();
                
                // The order of the following is important - 
                // state is more important than value, 
                // and non-normal states turn off the bar value change 
                // animation, which is likely the intended
                // and preferable behavior.
                UpdateProgressBarState(settings.ProgressBarState);
                UpdateProgressBarValue(settings.ProgressBarValue);
                
                // Due to a bug that wasn't fixed in time for RTM of Vista,
                // second SendMessage is required if the state is non-Normal.
                UpdateProgressBarValue(settings.ProgressBarValue);
            }
            else if (IsOptionSet(TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR))
            {
                // TDM_SET_PROGRESS_BAR_MARQUEE is necessary 
                // to cause the marquee to start animating.
                // Note that this internal task dialog setting is 
                // round-tripped when the marquee is
                // is set to different states, so it never has to 
                // be touched/sent again.
                SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_MARQUEE, 1, 0);
                UpdateProgressBarState(settings.ProgressBarState);
            }

            if (settings.ElevatedButtons != null && settings.ElevatedButtons.Count > 0)
            {
                foreach (int id in settings.ElevatedButtons)
                {
                    UpdateElevationIcon(id, true);
                }
            }
            
            return CoreErrorHelper.IGNORED;
        }

        private int HandleButtonClick(int id)
        {
            // First we raise a Click event, if there is a custom button
            // However, we implement Close() by sending a cancel button, so 
            // we don't want to raise a click event in response to that.
            if (showState != DialogShowState.Closing)
                outerDialog.RaiseButtonClickEvent(id);

            // Once that returns, we raise a Closing event for the dialog
            // The Win32 API handles button clicking-and-closing 
            // as an atomic action,
            // but it is more .NET friendly to split them up.
            // Unfortunately, we do NOT have the return values at this stage.
            if(id <= 9)
                return outerDialog.RaiseClosingEvent(id);
            else
                return 1;
        }

        private int HandleRadioButtonClick(int id)
        {
            // When the dialog sets the radio button to default, 
            // it (somewhat confusingly)issues a radio button clicked event
            //  - we mask that out - though ONLY if
            // we do have a default radio button
            if (firstRadioButtonClicked && !IsOptionSet(TaskDialogNativeMethods.TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON))
                firstRadioButtonClicked = false;
            else
            {
                outerDialog.RaiseButtonClickEvent(id);
            }

            // Note: we don't raise Closing, as radio 
            // buttons are non-committing buttons
            return CoreErrorHelper.IGNORED;
        }

        private int HandleHyperlinkClick(IntPtr pszHREF)
        {
            string link = Marshal.PtrToStringUni(pszHREF);
            outerDialog.RaiseHyperlinkClickEvent(link);

            return CoreErrorHelper.IGNORED;
        }


        private int HandleTick(int ticks)
        {
            outerDialog.RaiseTickEvent(ticks);
            return CoreErrorHelper.IGNORED;
        }

        private int HandleHelpInvocation()
        {
            outerDialog.RaiseHelpInvokedEvent();
            return CoreErrorHelper.IGNORED;
        }

        // There should be little we need to do here, 
        // as the use of the NativeTaskDialog is
        // that it is instantiated for a single show, then disposed of.
        private int PerformDialogCleanup()
        {
            firstRadioButtonClicked = true;

            return CoreErrorHelper.IGNORED;
        }

        #endregion

        #region Update members

        internal void UpdateProgressBarValue(int i)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_POS, i, 0);
        }

        internal void UpdateProgressBarRange()
        {
            AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order.
            long range = NativeTaskDialog.MakeLongLParam(
                settings.ProgressBarMaximum, 
                settings.ProgressBarMinimum);

            SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_RANGE, 0, range);
        }

        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_STATE, (int)state, 0);
        }

        internal void UpdateText(string text)
        {
            UpdateTextCore(text, TaskDialogNativeMethods.TASKDIALOG_ELEMENTS.TDE_CONTENT);
        }

        internal void UpdateInstruction(string instruction)
        {
            UpdateTextCore(instruction, TaskDialogNativeMethods.TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION);
        }

        internal void UpdateFooterText(string footerText)
        {
            UpdateTextCore(footerText, TaskDialogNativeMethods.TASKDIALOG_ELEMENTS.TDE_FOOTER);
        }

        internal void UpdateExpandedText(string expandedText)
        {
            UpdateTextCore(expandedText, TaskDialogNativeMethods.TASKDIALOG_ELEMENTS.TDE_EXPANDED_INFORMATION);
        }

        private void UpdateTextCore(string s, TaskDialogNativeMethods.TASKDIALOG_ELEMENTS element)
        {
            AssertCurrentlyShowing();

            FreeOldString(element);
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_ELEMENT_TEXT,
                (int)element,
                (long)MakeNewString(s, element));
        }

        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            UpdateIconCore(mainIcon, TaskDialogNativeMethods.TASKDIALOG_ICON_ELEMENT.TDIE_ICON_MAIN);
        }

        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            UpdateIconCore(footerIcon, TaskDialogNativeMethods.TASKDIALOG_ICON_ELEMENT.TDIE_ICON_FOOTER);
        }

        private void UpdateIconCore(TaskDialogStandardIcon icon, TaskDialogNativeMethods.TASKDIALOG_ICON_ELEMENT element)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_UPDATE_ICON,
                (int)element,
                (long)icon);
        }

        internal void UpdateCheckBoxChecked(bool cbc)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_CLICK_VERIFICATION,
                (cbc ? 1 : 0),
                1);
        }

        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE,
                buttonId,
                Convert.ToInt32(showIcon));
        }

        internal void UpdateButtonEnabled(int buttonID, bool enabled)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_ENABLE_BUTTON, buttonID, enabled == true ? 1 : 0);
        }
        internal void UpdateRadioButtonEnabled(int buttonID, bool enabled)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                TaskDialogNativeMethods.TASKDIALOG_MESSAGES.TDM_ENABLE_RADIO_BUTTON, buttonID, enabled == true? 1 : 0);
        }

        internal void AssertCurrentlyShowing()
        {
            Debug.Assert(showState == DialogShowState.Showing, "Update*() methods should only be called while native dialog is showing");
        }

        #endregion

        #region Helpers

        private int SendMessageHelper(TaskDialogNativeMethods.TASKDIALOG_MESSAGES msg, int wParam, long lParam)
        {
            // Be sure to at least assert here - 
            // messages to invalid handles often just disappear silently
            Debug.Assert(hWndDialog != null, 
                "HWND for dialog is null during SendMessage");

            return (int)CoreNativeMethods.SendMessage(
                hWndDialog,
                (uint)msg,
                (IntPtr)wParam,
                new IntPtr(lParam));
        }

        private bool IsOptionSet(TaskDialogNativeMethods.TASKDIALOG_FLAGS flag)
        {
            return ((nativeDialogConfig.dwFlags & flag) == flag);
        }

        // Allocates a new string on the unmanaged heap, 
        // and stores the pointer so we can free it later.

        private IntPtr MakeNewString(string s,
            TaskDialogNativeMethods.TASKDIALOG_ELEMENTS element)
        {
            IntPtr newStringPtr = Marshal.StringToHGlobalUni(s);
            updatedStrings[(int)element] = newStringPtr;
            return newStringPtr;
        }

        // Checks to see if the given element already has an 
        // updated string, and if so, 
        // frees it. This is done in preparation for a call to 
        // MakeNewString(), to prevent
        // leaks from multiple updates calls on the same element 
        // within a single native dialog lifetime.

        private void FreeOldString(TaskDialogNativeMethods.TASKDIALOG_ELEMENTS element)
        {
            int elementIndex = (int)element;
            if (updatedStrings[elementIndex] != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(updatedStrings[elementIndex]);
                updatedStrings[elementIndex] = IntPtr.Zero;
            }
        }

        // Based on the following defines in WinDef.h and WinUser.h:
        // #define MAKELPARAM(l, h) ((LPARAM)(DWORD)MAKELONG(l, h))
        // #define MAKELONG(a, b) ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff)) | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
        private static long MakeLongLParam(int a, int b)
        {
            return (a << 16) + b;
        }

        // Builds the actual configuration that the 
        // NativeTaskDialog (and underlying Win32 API)
        // expects, by parsing the various control lists, 
        // marshaling to the unmanaged heap, etc.

        private void MarshalDialogControlStructs()
        {
            if (settings.Buttons != null && settings.Buttons.Length > 0)
            {
                buttonArray = AllocateAndMarshalButtons(settings.Buttons);
                settings.NativeConfiguration.pButtons = buttonArray;
                settings.NativeConfiguration.cButtons = (uint)settings.Buttons.Length;
            }
            if (settings.RadioButtons != null && settings.RadioButtons.Length > 0)
            {
                radioButtonArray = AllocateAndMarshalButtons(settings.RadioButtons);
                settings.NativeConfiguration.pRadioButtons = radioButtonArray;
                settings.NativeConfiguration.cRadioButtons = (uint)settings.RadioButtons.Length;
            }
        }

        private static IntPtr AllocateAndMarshalButtons(TaskDialogNativeMethods.TASKDIALOG_BUTTON[] structs)
        {
            IntPtr initialPtr = Marshal.AllocHGlobal(
                Marshal.SizeOf(typeof(TaskDialogNativeMethods.TASKDIALOG_BUTTON)) * structs.Length);

            IntPtr currentPtr = initialPtr;
            foreach (TaskDialogNativeMethods.TASKDIALOG_BUTTON button in structs)
            {
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }

            return initialPtr;
        }

        #endregion

        #region IDispose Pattern

        private bool disposed;

        // Finalizer and IDisposable implementation.
        public void Dispose() 
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~NativeTaskDialog() 
        { 
            Dispose(false); 
        }

        // Core disposing logic.
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true; 

                // Single biggest resource - make sure the dialog 
                // itself has been instructed to close.

                if (showState == DialogShowState.Showing)
                    NativeClose(TaskDialogResult.Cancel);
                
                // Clean up custom allocated strings that were updated
                // while the dialog was showing. Note that the strings
                // passed in the initial TaskDialogIndirect call will
                // be cleaned up automagically by the default 
                // marshalling logic.

                if (updatedStrings != null)
                {
                    for (int i = 0; i < updatedStrings.Length; i++)
                    {
                        if (updatedStrings[i] != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(updatedStrings[i]);
                            updatedStrings[i] = IntPtr.Zero;
                        }
                    }
                }

                // Clean up the button and radio button arrays, if any.
                if (buttonArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buttonArray);
                    buttonArray = IntPtr.Zero;
                }
                if (radioButtonArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(radioButtonArray);
                    radioButtonArray = IntPtr.Zero;
                }

                if (disposing)
                {
                    // Clean up managed resources - currently there are none
                    // that are interesting.
                }
            }
        }

        #endregion
    }
}
