// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Stores information about how to sort a column that is displayed in the folder view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SortColumn
    {
        /// <summary>
        /// Creates a sort column with the specified direction for the given property.
        /// </summary>
        /// <param name="propertyKey">Property key for the property that the user will sort.</param>
        /// <param name="direction">The direction in which the items are sorted.</param>
        public SortColumn(PropertyKey propertyKey, SortDirection direction)
        {
            PropertyKey = propertyKey;
            Direction = direction;
        }

        /// <summary>
        /// The ID of the column by which the user will sort. A PropertyKey structure. 
        /// For example, for the "Name" column, the property key is PKEY_ItemNameDisplay or
        /// <see cref="Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System.ItemName"/>.
        /// </summary>
        public PropertyKey PropertyKey;

        /// <summary>
        /// The direction in which the items are sorted.
        /// </summary>
        public SortDirection Direction;
    };

}
