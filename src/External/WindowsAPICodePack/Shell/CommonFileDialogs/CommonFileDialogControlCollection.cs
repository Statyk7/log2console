//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAPICodePack.Dialogs.Controls
{
    /// <summary>
    /// Provides a strongly typed collection for dialog controls.
    /// </summary>
    /// <typeparam name="T">DialogControl</typeparam>
    public sealed class CommonFileDialogControlCollection<T> : Collection<T> where T : DialogControl
    {
        private IDialogControlHost hostingDialog;

        internal CommonFileDialogControlCollection(IDialogControlHost host)
        {
            hostingDialog = host;
        }

        /// <summary>
        /// Inserts an dialog control at the specified index.
        /// </summary>
        /// <param name="index">The location to insert the control.</param>
        /// <param name="control">The item to insert.</param>
        /// <permission cref="System.InvalidOperationException">A control with 
        /// the same name already exists in this collection -or- 
        /// the control is being hosted by another dialog -or- the associated dialog is 
        /// showing and cannot be modified.</permission>
        protected override void InsertItem(int index, T control)
        {
            // Check for duplicates, lack of host, 
            // and during-show adds.
            if (Items.Contains(control))
                throw new InvalidOperationException(
                    "Dialog cannot have more than one control with the same name.");
            if (control.HostingDialog != null)
                throw new InvalidOperationException(
                    "Dialog control must be removed from current collections first.");
            if (!hostingDialog.IsCollectionChangeAllowed())
                throw new InvalidOperationException(
                    "Modifying controls collection while dialog is showing is not supported.");
            if (control is CommonFileDialogMenuItem)
                throw new InvalidOperationException(
                    "CommonFileDialogMenuItem controls can only be added to CommonFileDialogMenu controls.");

            // Reparent, add control.
            control.HostingDialog = hostingDialog;
            base.InsertItem(index, control);

            // Notify that we've added a control.
            hostingDialog.ApplyCollectionChanged();
        }

        /// <summary>
        /// Removes the control at the specified index.
        /// </summary>
        /// <param name="index">The location of the control to remove.</param>
        /// <permission cref="System.InvalidOperationException">
        /// The associated dialog is 
        /// showing and cannot be modified.</permission>
        protected override void RemoveItem(int index)
        {
            throw new NotSupportedException("Custom controls cannot be removed from a File dialog once added.");
        }
        
        /// <summary>
        /// Defines the indexer that supports accessing controls by name. 
        /// </summary>
        /// <remarks>
        /// <para>Control names are case sensitive.</para>
        /// <para>This indexer is useful when the dialog is created in XAML
        /// rather than constructed in code.</para></remarks>
        ///<exception cref="System.ArgumentException">
        /// The name cannot be null or a zero-length string.</exception>
        /// <remarks>If there is more than one control with the same name, only the <B>first control</B> will be returned.</remarks>
        public T this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException(
                        "Control name must not be null or zero length.");

                foreach (T control in base.Items)
                {
                    // NOTE: we don't ToLower() the strings - casing effects 
                    // hash codes, so we are case-sensitive.
                    if (control.Name == name)
                        return control;
                    else if (control is CommonFileDialogGroupBox)
                    {
                        foreach (T subControl in (control as CommonFileDialogGroupBox).Items)
                        {
                            if (subControl.Name == name)
                                return subControl;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Recursively searches for the control who's id matches the value
        /// passed in the <paramref name="id"/> parameter.
        /// </summary>
        /// 
        /// <param name="id">An integer containing the identifier of the 
        /// control being searched for.</param>
        /// 
        /// <returns>A DialogControl who's id matches the value of the
        /// <paramref name="id"/> parameter.</returns>
        /// 
        internal DialogControl GetControlbyId(int id)
        {
            return GetSubControlbyId(
                Items as IEnumerable<T>,
                id);
        }


        /// <summary>
        /// Recursively searches for a given control id in the 
        /// collection passed via the <paramref name="ctrlColl"/> parameter.
        /// </summary>
        /// 
        /// <param name="ctrlColl">A Collection&lt;CommonFileDialogControl&gt;</param>
        /// <param name="id">An int containing the identifier of the control 
        /// being searched for.</param>
        /// 
        /// <returns>A DialogControl who's Id matches the value of the
        /// <paramref name="id"/> parameter.</returns>
        /// 
        internal DialogControl GetSubControlbyId(IEnumerable ctrlColl,
            int id)
        {
            DialogControl foundControl = null;
            int iSubCtrlCount = 0;

            // if ctrlColl is null, it will throw in the foreach.
            if (ctrlColl == null)
                return null;

            foreach (DialogControl control in ctrlColl)
            {
                // Match?
                if (control.Id == id)
                {
                    return control;
                }

                // Search GroupBox child items
                if (control is CommonFileDialogGroupBox)
                {
                    CommonFileDialogGroupBox groupBox = control as CommonFileDialogGroupBox;

                    // recurse and search the GroupBox
                    iSubCtrlCount =
                        ((CommonFileDialogGroupBox)control).Items.Count;

                    if (iSubCtrlCount > 0)
                    {
                        foundControl = this.GetSubControlbyId(
                            groupBox.Items as IEnumerable,
                            id);

                        // make sure something was actually found
                        if (foundControl != null)
                        {
                            return foundControl;
                        }
                    }
                }
            }

            // Control id not found - likely an error, but the calling 
            // function should ultimately decide.
            return null;

        }

    }
}
