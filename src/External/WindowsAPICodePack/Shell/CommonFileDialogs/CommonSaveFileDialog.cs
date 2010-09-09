//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Creates a Vista or Windows 7 Common File Dialog, allowing the user to select the filename and location for a saved file.
    /// </summary>
    /// <permission cref="System.Security.Permissions.FileDialogPermission">
    /// to save a file. Associated enumeration: <see cref="System.Security.Permissions.SecurityAction.LinkDemand"/>.
    /// </permission>
    [FileDialogPermissionAttribute(SecurityAction.LinkDemand, Save = true)]
    public sealed class CommonSaveFileDialog : CommonFileDialog
    {
        private NativeFileSaveDialog saveDialogCoClass;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public CommonSaveFileDialog() : base() { }
        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">The name of this dialog.</param>
        public CommonSaveFileDialog(string name) : base(name) { }

        #region Public API specific to Save

        private bool overwritePrompt = true;
        /// <summary>
        /// Gets or sets a value that controls whether to prompt before 
        /// overwriting an existing file of the same name. Default value is true.
        /// </summary>
        /// <permission cref="System.InvalidOperationException">
        /// This property cannot be changed when the dialog is showing.
        /// </permission>
        public bool OverwritePrompt
        {
            get { return overwritePrompt; }
            set
            {
                ThrowIfDialogShowing("OverwritePrompt" + IllegalPropertyChangeString);
                overwritePrompt = value;
            }
        }

        private bool createPrompt;
        /// <summary>
        /// Gets or sets a value that controls whether to prompt for creation if the item returned in the save dialog does not exist. 
        /// </summary>
        /// <remarks>Note that this does not actually create the item.</remarks>
        /// <permission cref="System.InvalidOperationException">
        /// This property cannot be changed when the dialog is showing.
        /// </permission>
        public bool CreatePrompt
        {
            get { return createPrompt; }
            set
            {
                ThrowIfDialogShowing("CreatePrompt" + IllegalPropertyChangeString);
                createPrompt = value;
            }
        }

        private bool isExpandedMode;
        /// <summary>
        /// Gets or sets a value that controls whether to the save dialog 
        /// displays in expanded mode. 
        /// </summary>
        /// <remarks>Expanded mode controls whether the dialog
        /// shows folders for browsing or hides them.</remarks>
        /// <permission cref="System.InvalidOperationException">
        /// This property cannot be changed when the dialog is showing.
        /// </permission>
        public bool IsExpandedMode
        {
            get { return isExpandedMode; }
            set
            {
                ThrowIfDialogShowing("IsExpandedMode" + IllegalPropertyChangeString);
                isExpandedMode = value;
            }
        }

        private bool alwaysAppendDefaultExtension;
        /// <summary>
        /// Gets or sets a value that controls whether the 
        /// returned file name has a file extension that matches the 
        /// currently selected file type.  If necessary, the dialog appends the correct 
        /// file extension.
        /// </summary>
        /// <permission cref="System.InvalidOperationException">
        /// This property cannot be changed when the dialog is showing.
        /// </permission>
        public bool AlwaysAppendDefaultExtension
        {
            get { return alwaysAppendDefaultExtension; }
            set
            {
                ThrowIfDialogShowing("AlwaysAppendDefaultExtension" + IllegalPropertyChangeString);
                alwaysAppendDefaultExtension = value;
            }
        }

        /// <summary>
        /// Sets an item to appear as the initial entry in a <b>Save As</b> dialog.
        /// </summary>
        /// <param name="item">The initial entry to be set in the dialog.</param>
        /// <remarks>The name of the item is displayed in the file name edit box, 
        /// and the containing folder is opened in the view. This would generally be 
        /// used when the application is saving an item that already exists.</remarks>
        public void SetSaveAsItem(ShellObject item)
        {
            IFileSaveDialog nativeDialog = null;

            if (nativeDialog == null)
            {
                InitializeNativeFileDialog();
                nativeDialog = GetNativeFileDialog() as IFileSaveDialog;
            }

            // Get the native IShellItem from ShellObject
            if (nativeDialog != null)
                nativeDialog.SetSaveAsItem(item.NativeShellItem);
        }

        /// <summary>
        /// Specifies which properties will be collected in the save dialog.
        /// </summary>
        /// <param name="appendDefault">True to show default properties for the currently selected 
        /// filetype in addition to the properties specified by propertyList. False to show only properties 
        /// specified by pList.
        /// <param name="propertyList">List of properties to collect. This parameter can be null.</param>
        /// </param>
        /// <remarks>
        /// SetCollectedPropertyKeys can be called at any time before the dialog is displayed or while it 
        /// is visible. If different properties are to be collected depending on the chosen filetype, 
        /// then SetCollectedProperties can be called in response to CommonFileDialog::FileTypeChanged event.
        /// Note: By default, no properties are collected in the save dialog.
        /// </remarks>
        public void SetCollectedPropertyKeys(bool appendDefault, params PropertyKey[] propertyList)
        {
            string propertyListStr = null;

            // Loop through all our property keys and create a semicolon-delimited property list string.
            if (propertyList != null && propertyList.Length > 0 && propertyList[0] != null)
            {
                foreach (PropertyKey key in propertyList)
                {
                    string canonicalName = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(key).CanonicalName;
                    
                    // The string we pass to PSGetPropertyDescriptionListFromString must
                    // start with "prop:", followed a list of canonical names for each 
                    // property that is to collected.
                    // 
                    // Add "prop:" at the start of the string if we are starting our for loop.
                    if (propertyListStr == null)
                        propertyListStr = "prop:";

                    // For each property, append the canonical name, followed by a semicolon
                    if (!string.IsNullOrEmpty(canonicalName))
                        propertyListStr += canonicalName + ";";
                }
            }

            // If the string was created correctly, get IPropertyDescriptionList for it
            if (!string.IsNullOrEmpty(propertyListStr))
            {
                Guid guid = new Guid(ShellIIDGuid.IPropertyDescriptionList);
                IPropertyDescriptionList propertyDescriptionList = null;

                try
                {
                    int hr = PropertySystemNativeMethods.PSGetPropertyDescriptionListFromString(propertyListStr, ref guid, out propertyDescriptionList);

                    // If we get a IPropertyDescriptionList, setit on the native dialog.
                    if (CoreErrorHelper.Succeeded(hr))
                    {
                        IFileSaveDialog nativeDialog = null;

                        if (nativeDialog == null)
                        {
                            InitializeNativeFileDialog();
                            nativeDialog = GetNativeFileDialog() as IFileSaveDialog;
                        }

                        if (nativeDialog != null)
                        {
                            hr = nativeDialog.SetCollectedProperties(propertyDescriptionList, appendDefault);

                            if (!CoreErrorHelper.Succeeded(hr))
                                Marshal.ThrowExceptionForHR(hr);
                        }
                    }
                }
                finally
                {
                    if (propertyDescriptionList != null)
                        Marshal.ReleaseComObject(propertyDescriptionList);
                }
            }
        }

        /// <summary>
        /// Retrieves the set of property values for a saved item or an item in the process of being saved.
        /// </summary>
        /// <returns>Collection of property values collected from the save dialog</returns>
        /// <remarks>This property can be called while the dialog is showing to retrieve the current 
        /// set of values in the metadata collection pane. It can also be called after the dialog 
        /// has closed, to retrieve the final set of values. The call to this method will fail 
        /// unless property collection has been turned on with a call to SetCollectedPropertyKeys method.
        /// </remarks>
        public ShellPropertyCollection CollectedProperties
        {
            get
            {
                IFileSaveDialog nativeDialog = null;

                if (nativeDialog == null)
                {
                    InitializeNativeFileDialog();
                    nativeDialog = GetNativeFileDialog() as IFileSaveDialog;
                }

                if (nativeDialog != null)
                {
                    IPropertyStore propertyStore;
                    HRESULT hr = nativeDialog.GetProperties(out propertyStore);

                    if (!CoreErrorHelper.Succeeded((int)hr))
                        throw Marshal.GetExceptionForHR((int)hr);

                    if (propertyStore != null)
                        return new ShellPropertyCollection(propertyStore);
                }

                return null;
            }
        }

        #endregion

        internal override void InitializeNativeFileDialog()
        {
            if (saveDialogCoClass == null)
                saveDialogCoClass = new NativeFileSaveDialog();
        }

        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(saveDialogCoClass != null,
                "Must call Initialize() before fetching dialog interface");
            return (IFileDialog)saveDialogCoClass;
        }

        internal override void PopulateWithFileNames(
            System.Collections.ObjectModel.Collection<string> names)
        {
            IShellItem item;
            saveDialogCoClass.GetResult(out item);

            if (item == null)
                throw new InvalidOperationException(
                    "Retrieved a null shell item from dialog");
            names.Clear();
            names.Add(GetFileNameFromShellItem(item));
        }

        internal override void PopulateWithIShellItems(
            System.Collections.ObjectModel.Collection<IShellItem> items)
        {
            IShellItem item;
            saveDialogCoClass.GetResult(out item);

            if (item == null)
                throw new InvalidOperationException(
                    "Retrieved a null shell item from dialog");
            items.Clear();
            items.Add(item);
        }

        internal override void CleanUpNativeFileDialog()
        {
            if (saveDialogCoClass != null)
                Marshal.ReleaseComObject(saveDialogCoClass);
        }

        internal override ShellNativeMethods.FOS GetDerivedOptionFlags(ShellNativeMethods.FOS flags)
        {
            if (overwritePrompt)
                flags |= ShellNativeMethods.FOS.FOS_OVERWRITEPROMPT;
            if (createPrompt)
                flags |= ShellNativeMethods.FOS.FOS_CREATEPROMPT;
            if (!isExpandedMode)
                flags |= ShellNativeMethods.FOS.FOS_DEFAULTNOMINIMODE;
            if (alwaysAppendDefaultExtension)
                flags |= ShellNativeMethods.FOS.FOS_STRICTFILETYPES;
            return flags;
        }
    }
}
