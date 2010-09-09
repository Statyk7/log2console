//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Provides a strongly typed collection for file dialog filters.
    /// </summary>
    public class CommonFileDialogFilterCollection : Collection<CommonFileDialogFilter>
    {
        internal CommonFileDialogFilterCollection()
            : base()
        {
            // Make the default constructor internal so users can't instantiate this 
            // collection by themselves.
        }

        internal ShellNativeMethods.COMDLG_FILTERSPEC[] GetAllFilterSpecs()
        {
            ShellNativeMethods.COMDLG_FILTERSPEC[] filterSpecs = 
                new ShellNativeMethods.COMDLG_FILTERSPEC[this.Count];

            for (int i = 0; i < this.Count; i++)
                filterSpecs[i] = this[i].GetFilterSpec();

            return filterSpecs;
        }
    }
}
