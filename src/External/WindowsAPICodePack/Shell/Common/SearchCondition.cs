//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Exposes properties and methods for retrieving information about a search condition.
    /// </summary>
    public class SearchCondition : IDisposable
    {
        internal SearchCondition(ICondition nativeSearchCondition)
        {
            if (nativeSearchCondition == null)
                throw new ArgumentNullException("nativeSearchCondition");

            NativeSearchCondition = nativeSearchCondition;

            HRESULT hr = NativeSearchCondition.GetConditionType(out conditionType);

            if (!CoreErrorHelper.Succeeded((int)hr))
                Marshal.ThrowExceptionForHR((int)hr);

            if (ConditionType == SearchConditionType.Leaf)
            {
                PropVariant propVar;
                hr = NativeSearchCondition.GetComparisonInfo(out canonicalName, out conditionOperation, out propVar);

                if (!CoreErrorHelper.Succeeded((int)hr))
                    Marshal.ThrowExceptionForHR((int)hr);

                try
                {
                    propertyValue = propVar.Value.ToString();
                }
                finally
                {
                    propVar.Clear();
                }

            }
        }

        private ICondition searchCondition = null;
        internal ICondition NativeSearchCondition
        {
            get
            {
                return searchCondition;
            }
            set
            {
                searchCondition = value;
            }
        }

        private string canonicalName;
        /// <summary>
        /// The name of a property to be compared or NULL for an unspecified property.
        /// </summary>
        public string PropertyCanonicalName
        {
            get { return canonicalName; }
            internal set { canonicalName = value; }
        }

        private PropertyKey propertyKey;
        private PropertyKey emptyPropertyKey = new PropertyKey();
        /// <summary>
        /// The property key for the property that is to be compared.
        /// </summary>
        public PropertyKey PropertyKey
        {
            get
            {
                if (propertyKey == emptyPropertyKey)
                {
                    int hr = PropertySystemNativeMethods.PSGetPropertyKeyFromName(PropertyCanonicalName, out propertyKey);

                    if (!CoreErrorHelper.Succeeded(hr))
                        Marshal.ThrowExceptionForHR(hr);
                }

                return propertyKey;
            }
            internal set { propertyKey = value; }
        }

        private string propertyValue;
        /// <summary>
        /// A value (in <see cref="System.String"/> format) to which the property is compared. 
        /// </summary>
        public string PropertyValue
        {
            get { return propertyValue; }
            internal set { propertyValue = value; }
        }

        private SearchConditionOperation conditionOperation = SearchConditionOperation.Implicit;
        /// <summary>
        /// Search condition operation to be performed on the property/value combination.
        /// See <see cref="Microsoft.WindowsAPICodePack.Shell.SearchConditionOperation"/> for more details.
        /// </summary>
        public SearchConditionOperation ConditionOperation
        {
            get { return conditionOperation; }
            internal set { conditionOperation = value; }
        }

        private SearchConditionType conditionType = SearchConditionType.Leaf;
        /// <summary>
        /// Represents the condition type for the given node. 
        /// </summary>
        public SearchConditionType ConditionType
        {
            get { return conditionType; }
            internal set { conditionType = value; }
        }

        /// <summary>
        /// Retrieves an array of the sub-conditions. 
        /// </summary>
        public IEnumerable<SearchCondition> GetSubConditions()
        {
            // Our list that we'll return
            List<SearchCondition> subConditionsList = new List<SearchCondition>();

            // Get the sub-conditions from the native API
            object subConditionObj;
            Guid guid = new Guid(ShellIIDGuid.IEnumUnknown);

            HRESULT hr = NativeSearchCondition.GetSubConditions(ref guid, out subConditionObj);

            if (!CoreErrorHelper.Succeeded((int)hr))
                Marshal.ThrowExceptionForHR((int)hr);

            // Convert each ICondition to SearchCondition
            if (subConditionObj != null)
            {
                IEnumUnknown enumUnknown = subConditionObj as IEnumUnknown;

                IntPtr buffer = IntPtr.Zero;
                uint fetched = 0;

                while (hr == HRESULT.S_OK)
                {
                    hr = enumUnknown.Next(1, ref buffer, ref fetched);

                    if (hr == HRESULT.S_OK && fetched == 1)
                    {
                        subConditionsList.Add(new SearchCondition((ICondition)Marshal.GetObjectForIUnknown(buffer)));
                    }
                }
            }

            return subConditionsList;
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        ~SearchCondition()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (NativeSearchCondition != null)
            {
                Marshal.ReleaseComObject(NativeSearchCondition);
                NativeSearchCondition = null;
            }
        }

        #endregion

    }
}
