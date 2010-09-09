//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;

namespace Microsoft.WindowsAPICodePack.Dialogs.Controls
{
    /// <summary>
    /// Represents a radio button list for the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public class CommonFileDialogRadioButtonList : CommonFileDialogControl, ICommonFileDialogIndexedControls
    {
        private Collection<CommonFileDialogRadioButtonListItem> items;
        /// <summary>
        /// Gets the collection of CommonFileDialogRadioButtonListItem objects
        /// </summary>
        public Collection<CommonFileDialogRadioButtonListItem> Items
        {
            get { return items; }
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogRadioButtonList()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">The name of this control.</param>
        public CommonFileDialogRadioButtonList(string name): base (name, String.Empty)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            items = new Collection<CommonFileDialogRadioButtonListItem>();
        }

        #region ICommonFileDialogIndexedControls Members

        private int selectedIndex = -1;
        /// <summary>
        /// Gets or sets the current index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                // Don't update this property if it hasn't changed
                if (selectedIndex == value)
                    return;

                // If the native dialog has not been created yet
                if (HostingDialog == null)
                {
                    selectedIndex = value;
                    return;
                }

                // Check for valid index
                if (value >= 0 && value < items.Count)
                {
                    selectedIndex = value;
                    ApplyPropertyChange("SelectedIndex");
                }
                else
                {
                    throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");
                }
            }
        }

        /// <summary>
        /// Occurs when the user changes the SelectedIndex.
        /// </summary>
        /// 
        /// <remarks>
        /// By initializing the SelectedIndexChanged event with an empty
        /// delegate, we can skip the test to determine
        /// if the SelectedIndexChanged is null.
        /// test.
        /// </remarks>
        public event EventHandler SelectedIndexChanged = delegate { };

        /// <summary>
        /// Occurs when the user changes the SelectedIndex.
        /// </summary>
        /// <remarks>Because this method is defined in an interface, we can either
        /// have it as public, or make it private and explicitly implement (like below).
        /// Making it public doesn't really help as its only internal (but can't have this 
        /// internal because of the interface)
        /// </remarks>
        void ICommonFileDialogIndexedControls.RaiseSelectedIndexChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Attach the RadioButtonList control to the dialog object
        /// </summary>
        /// <param name="dialog">The target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogRadioButtonList.Attach: dialog parameter can not be null");
            
            // Add the radio button list control
            dialog.AddRadioButtonList(this.Id);

            // Add the radio button list items
            for (int index = 0; index < items.Count; index++)
                dialog.AddControlItem(this.Id, index, items[index].Text);

            // Set the currently selected item
            if (selectedIndex >= 0 && selectedIndex < items.Count)
            {
                dialog.SetSelectedControlItem(this.Id, this.selectedIndex);
            }
            else if (selectedIndex != -1)
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");
            }


            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }

    /// <summary>
    /// Represents a list item for the CommonFileDialogRadioButtonList object.
    /// </summary>
    public class CommonFileDialogRadioButtonListItem
    {
        private string text;
        /// <summary>
        /// Gets or sets the string that will be displayed for this list item.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogRadioButtonListItem()
        {
            this.text = String.Empty;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">The string that you want to display for this list item.</param>
        public CommonFileDialogRadioButtonListItem(string text)
        {
            this.text = text;
        }
    }
}
