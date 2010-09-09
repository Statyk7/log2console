//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents a custom category on the taskbar's jump list
    /// </summary>
    public class JumpListCustomCategory
    {
        private string name;

        internal JumpListItemCollection<IJumpListItem> JumpListItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Category name
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String)", Justification="We are not currently handling globalization or localization")]
        public string Name
        {
            get { return name; }
            set
            {
                if (String.Compare(name, value) != 0)
                {
                    name = value;
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        
        /// <summary>
        /// Add JumpList items for this category
        /// </summary>
        /// <param name="items">The items to add to the JumpList.</param>
        public void AddJumpListItems(params IJumpListItem[] items)
        {
            foreach (IJumpListItem item in items)
                JumpListItems.Add(item);
        }

        /// <summary>
        /// Event that is triggered when the jump list collection is modified
        /// </summary>
        internal event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        /// <summary>
        /// Creates a new custom category instance
        /// </summary>
        /// <param name="categoryName">Category name</param>
        public JumpListCustomCategory(string categoryName)
        {
            Name = categoryName;

            JumpListItems = new JumpListItemCollection<IJumpListItem>();
            JumpListItems.CollectionChanged += OnJumpListCollectionChanged;
        }

        internal void OnJumpListCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            this.CollectionChanged(this, args);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)", Justification = "We are not currently handling globalization or localization")]
        internal void RemoveJumpListItem(string path)
        {
            List<IJumpListItem> itemsToRemove = new List<IJumpListItem>();

            // Check for items to remove
            foreach (IJumpListItem item in JumpListItems)
            {
                if (string.Compare(path, item.Path, true) == 0)
                {
                    itemsToRemove.Add(item);
                }
            }

            // Remove matching items
            for (int i = 0; i < itemsToRemove.Count; i++)
            {
                JumpListItems.Remove(itemsToRemove[i]);
            }
        }
    }
}
