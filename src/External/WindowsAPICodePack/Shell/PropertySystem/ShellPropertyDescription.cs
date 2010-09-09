//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    /// <summary>
    /// Defines the shell property description information for a property.
    /// </summary>
    public class ShellPropertyDescription : IDisposable
    {
        #region Private Fields

        private IPropertyDescription nativePropertyDescription = null;
        private string canonicalName = null;
        private PropertyKey propertyKey;
        private string displayName = null;
        private string editInvitation = null;
        private VarEnum? varEnumType = null;
        private PropertyDisplayType? displayType;
        private PropertyAggregationType? aggregationTypes;
        private uint? defaultColumWidth;
        private PropertyTypeFlags? propertyTypeFlags;
        private PropertyViewFlags? propertyViewFlags;
        private Type valueType = null;
        private ReadOnlyCollection<ShellPropertyEnumType> propertyEnumTypes = null;
        private PropertyColumnState? columnState;
        private PropertyConditionType? conditionType;
        private PropertyConditionOperation? conditionOperation;
        private PropertyGroupingRange? groupingRange;
        private PropertySortDescription? sortDescription;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the case-sensitive name of a property as it is known to the system, 
        /// regardless of its localized name.
        /// </summary>
        public string CanonicalName
        {
            get
            {
                if (canonicalName == null)
                {
                    PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);
                }

                return canonicalName;
            }
        }

        /// <summary>
        /// Gets the property key identifying the underlying property.
        /// </summary>
        public PropertyKey PropertyKey
        {
            get
            {
                return propertyKey;
            }
        }

        /// <summary>
        /// Gets the display name of the property as it is shown in any user interface (UI).
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (NativePropertyDescription != null && displayName == null)
                {
                    IntPtr dispNameptr = IntPtr.Zero;

                    HRESULT hr = NativePropertyDescription.GetDisplayName(out dispNameptr);

                    if (CoreErrorHelper.Succeeded((int)hr) && dispNameptr != IntPtr.Zero)
                    {
                        displayName = Marshal.PtrToStringUni(dispNameptr);

                        // Free the string
                        Marshal.FreeCoTaskMem(dispNameptr);
                    }
                }

                return displayName;
            }
        }

        /// <summary>
        /// Gets the text used in edit controls hosted in various dialog boxes.
        /// </summary>
        public string EditInvitation
        {
            get
            {
                if (NativePropertyDescription != null && editInvitation == null)
                {
                    // EditInvitation can be empty, so ignore the HR value, but don't throw an exception
                    IntPtr ptr = IntPtr.Zero;

                    HRESULT hr = NativePropertyDescription.GetEditInvitation(out ptr);

                    if (CoreErrorHelper.Succeeded((int)hr) && ptr != IntPtr.Zero)
                    {
                        editInvitation = Marshal.PtrToStringUni(ptr);
                        // Free the string
                        Marshal.FreeCoTaskMem(ptr);
                    }
                }

                return editInvitation;
            }
        }

        /// <summary>
        /// Gets the VarEnum OLE type for this property.
        /// </summary>
        public VarEnum VarEnumType
        {
            get
            {
                if (NativePropertyDescription != null && varEnumType == null)
                {
                    VarEnum tempType;

                    HRESULT hr = NativePropertyDescription.GetPropertyType(out tempType);
                    
                    if(CoreErrorHelper.Succeeded((int)hr))
                        varEnumType = tempType;
                }

                return varEnumType.HasValue ? varEnumType.Value : default(VarEnum);
            }
        }

        /// <summary>
        /// Gets the .NET system type for a value of this property, or
        /// null if the value is empty.
        /// </summary>
        public Type ValueType
        {
            get
            {
                if (valueType == null)
                    valueType = VarEnumToSystemType(VarEnumType);

                return valueType;
            }
        }

        /// <summary>
        /// Gets the current data type used to display the property.
        /// </summary>
        public PropertyDisplayType DisplayType
        {
            get
            {
                if (NativePropertyDescription != null && displayType == null)
                {
                    PropertyDisplayType tempDisplayType;

                    HRESULT hr = NativePropertyDescription.GetDisplayType(out tempDisplayType);

                    if (CoreErrorHelper.Succeeded((int)hr))
                        displayType =  tempDisplayType;
                }

                return displayType.HasValue ? displayType.Value : default(PropertyDisplayType);
            }
        }

        /// <summary>
        /// Gets the default user interface (UI) column width for this property.
        /// </summary>
        public uint DefaultColumWidth
        {
            get
            {
                if (NativePropertyDescription != null && !defaultColumWidth.HasValue)
                {
                    uint tempDefaultColumWidth;

                    HRESULT hr = NativePropertyDescription.GetDefaultColumnWidth(out tempDefaultColumWidth);

                    if(CoreErrorHelper.Succeeded((int)hr))
                        defaultColumWidth = tempDefaultColumWidth;
                }

                return defaultColumWidth.HasValue ? defaultColumWidth.Value : default(uint);
            }
        }

        /// <summary>
        /// Gets a value that describes how the property values are displayed when 
        /// multiple items are selected in the user interface (UI).
        /// </summary>
        public PropertyAggregationType AggregationTypes
        {
            get
            {
                if (NativePropertyDescription != null && aggregationTypes == null)
                {
                    PropertyAggregationType tempAggregationTypes;   
                 
                    HRESULT hr = NativePropertyDescription.GetAggregationType(out tempAggregationTypes);
                    
                    if (CoreErrorHelper.Succeeded((int)hr))
                        aggregationTypes =  tempAggregationTypes;
                }

                return aggregationTypes.HasValue ? aggregationTypes.Value : default(PropertyAggregationType);
            }
        }

        /// <summary>
        /// Gets a list of the possible values for this property.
        /// </summary>
        public ReadOnlyCollection<ShellPropertyEnumType> PropertyEnumTypes
        {
            get
            {
                if (NativePropertyDescription != null && propertyEnumTypes == null)
                {
                    List<ShellPropertyEnumType> propEnumTypeList = new List<ShellPropertyEnumType>();

                    Guid guid = new Guid(ShellIIDGuid.IPropertyEnumTypeList);
                    IPropertyEnumTypeList nativeList;
                    HRESULT hr = NativePropertyDescription.GetEnumTypeList(ref guid, out nativeList);

                    if (nativeList != null && CoreErrorHelper.Succeeded((int)hr))
                    {

                        uint count;
                        nativeList.GetCount(out count);
                        guid = new Guid(ShellIIDGuid.IPropertyEnumType);

                        for (uint i = 0; i < count; i++)
                        {
                            IPropertyEnumType nativeEnumType;
                            nativeList.GetAt(i, ref guid, out nativeEnumType);
                            propEnumTypeList.Add(new ShellPropertyEnumType(nativeEnumType));
                        }
                    }

                    propertyEnumTypes = new ReadOnlyCollection<ShellPropertyEnumType>(propEnumTypeList);
                }

                return propertyEnumTypes;

            }
        }

        /// <summary>
        /// Gets the column state flag, which describes how the property 
        /// should be treated by interfaces or APIs that use this flag.
        /// </summary>
        public PropertyColumnState ColumnState
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && columnState == null)
                {
                    PropertyColumnState state;

                    HRESULT hr = NativePropertyDescription.GetColumnState(out state);

                    if (CoreErrorHelper.Succeeded((int)hr))
                        columnState = state;
                }

                return columnState.HasValue ? columnState.Value : default(PropertyColumnState);
            }
        }

        /// <summary>
        /// Gets the condition type to use when displaying the property in 
        /// the query builder user interface (UI). This influences the list 
        /// of predicate conditions (for example, equals, less than, and 
        /// contains) that are shown for this property.
        /// </summary>
        /// <remarks>For more information, see the <c>conditionType</c> attribute 
        /// of the <c>typeInfo</c> element in the property's .propdesc file.</remarks>
        public PropertyConditionType ConditionType
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && conditionType == null)
                {
                    PropertyConditionType tempConditionType;
                    PropertyConditionOperation tempConditionOperation;

                    HRESULT hr = NativePropertyDescription.GetConditionType(out tempConditionType, out tempConditionOperation);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        conditionOperation = tempConditionOperation;
                        conditionType = tempConditionType;
                    }
                }

                return conditionType.HasValue ? conditionType.Value : default(PropertyConditionType);
            }
        }

        /// <summary>
        /// Gets the default condition operation to use 
        /// when displaying the property in the query builder user 
        /// interface (UI). This influences the list of predicate conditions 
        /// (for example, equals, less than, and contains) that are shown 
        /// for this property.
        /// </summary>
        /// <remarks>For more information, see the <c>conditionType</c> attribute of the 
        /// <c>typeInfo</c> element in the property's .propdesc file.</remarks>
        public PropertyConditionOperation ConditionOperation
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && conditionOperation == null)
                {
                    PropertyConditionType tempConditionType;
                    PropertyConditionOperation tempConditionOperation;

                    HRESULT hr = NativePropertyDescription.GetConditionType(out tempConditionType, out tempConditionOperation);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        conditionOperation = tempConditionOperation;
                        conditionType = tempConditionType;
                    }
                }

                return conditionOperation.HasValue ? conditionOperation.Value : default(PropertyConditionOperation);
            }
        }

        /// <summary>
        /// Gets the method used when a view is grouped by this property.
        /// </summary>
        /// <remarks>The information retrieved by this method comes from 
        /// the <c>groupingRange</c> attribute of the <c>typeInfo</c> element in the 
        /// property's .propdesc file.</remarks>
        public PropertyGroupingRange GroupingRange
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && groupingRange == null)
                {
                    PropertyGroupingRange tempGroupingRange;

                    HRESULT hr = NativePropertyDescription.GetGroupingRange(out tempGroupingRange);

                    if (CoreErrorHelper.Succeeded((int)hr))
                        groupingRange = tempGroupingRange;
                }

                return groupingRange.HasValue ? groupingRange.Value : default(PropertyGroupingRange);
            }
        }

        /// <summary>
        /// Gets the current sort description flags for the property, 
        /// which indicate the particular wordings of sort offerings.
        /// </summary>
        /// <remarks>The settings retrieved by this method are set 
        /// through the <c>sortDescription</c> attribute of the <c>labelInfo</c> 
        /// element in the property's .propdesc file.</remarks>
        public PropertySortDescription SortDescription
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && sortDescription == null)
                {
                    PropertySortDescription tempSortDescription;

                    HRESULT hr = NativePropertyDescription.GetSortDescription(out tempSortDescription);

                    if (CoreErrorHelper.Succeeded((int)hr))
                        sortDescription = tempSortDescription;
                }

                return sortDescription.HasValue ? sortDescription.Value : default(PropertySortDescription);
            }
        }

        /// <summary>
        /// Gets the localized display string that describes the current sort order.
        /// </summary>
        /// <param name="descending">Indicates the sort order should 
        /// reference the string "Z on top"; otherwise, the sort order should reference the string "A on top".</param>
        /// <returns>The sort description for this property.</returns>
        /// <remarks>The string retrieved by this method is determined by flags set in the 
        /// <c>sortDescription</c> attribute of the <c>labelInfo</c> element in the property's .propdesc file.</remarks>
        public string GetSortDescriptionLabel(bool descending)
        {
            IntPtr ptr = IntPtr.Zero;
            string label = String.Empty;

            if (NativePropertyDescription != null)
            {
                HRESULT hr = NativePropertyDescription.GetSortDescriptionLabel(descending, out ptr);

                if (CoreErrorHelper.Succeeded((int)hr) && ptr != IntPtr.Zero)
                {
                    label = Marshal.PtrToStringUni(ptr);
                    // Free the string
                    Marshal.FreeCoTaskMem(ptr);
                }
            }

            return label;
        }

        /// <summary>
        /// Gets a set of flags that describe the uses and capabilities of the property.
        /// </summary>
        public PropertyTypeFlags TypeFlags
        {
            get
            {
                if (NativePropertyDescription != null && propertyTypeFlags == null)
                {
                    PropertyTypeFlags tempFlags;
                    
                    HRESULT hr = NativePropertyDescription.GetTypeFlags(PropertyTypeFlags.MaskAll, out tempFlags);

                    propertyTypeFlags = CoreErrorHelper.Succeeded((int)hr) ? tempFlags : default(PropertyTypeFlags);
                }

                return propertyTypeFlags.HasValue ? propertyTypeFlags.Value : default(PropertyTypeFlags);
            }
        }

        /// <summary>
        /// Gets the current set of flags governing the property's view.
        /// </summary>
        public PropertyViewFlags ViewFlags
        {
            get
            {
                if (NativePropertyDescription != null && propertyViewFlags == null)
                {
                    PropertyViewFlags tempFlags;
                    HRESULT hr = NativePropertyDescription.GetViewFlags(out tempFlags);

                    propertyViewFlags = CoreErrorHelper.Succeeded((int)hr) ? tempFlags : default(PropertyViewFlags);
                }

                return propertyViewFlags.HasValue ? propertyViewFlags.Value : default(PropertyViewFlags);
            }
        }

        /// <summary>
        /// Gets a value that determines if the native property description is present on the system.
        /// </summary>
        public bool HasSystemDescription
        {
            get { return NativePropertyDescription != null; }
        }

        #endregion

        #region Internal Constructor

        internal ShellPropertyDescription(PropertyKey key)
        {
            this.propertyKey = key;
        }

        #endregion

        #region Internal Methods

        static internal Type VarEnumToSystemType(VarEnum VarEnumType)
        {
            switch (VarEnumType)
            {
                case (VarEnum.VT_EMPTY):
                case (VarEnum.VT_NULL):

                    return typeof(Object);

                case (VarEnum.VT_UI1):

                    return typeof(Byte?);

                case (VarEnum.VT_I2):

                    return typeof(Int16?);

                case (VarEnum.VT_UI2):

                    return typeof(UInt16?);

                case (VarEnum.VT_I4):

                    return typeof(Int32?);

                case (VarEnum.VT_UI4):

                    return typeof(UInt32?);

                case (VarEnum.VT_I8):

                    return typeof(Int64?);

                case (VarEnum.VT_UI8):

                    return typeof(UInt64?);

                case (VarEnum.VT_R8):

                    return typeof(Double?);

                case (VarEnum.VT_BOOL):

                    return typeof(Boolean?);

                case (VarEnum.VT_FILETIME):

                    return typeof(DateTime?);

                case (VarEnum.VT_CLSID):

                    return typeof(IntPtr?);

                case (VarEnum.VT_CF):

                    return typeof(IntPtr?);

                case (VarEnum.VT_BLOB):

                    return typeof(Byte[]);

                case (VarEnum.VT_LPWSTR):

                    return typeof(String);

                case (VarEnum.VT_UNKNOWN):

                    return typeof(IntPtr?);

                case (VarEnum.VT_STREAM):

                    return typeof(IStream);

                case (VarEnum.VT_VECTOR | VarEnum.VT_UI1):

                    return typeof(Byte[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_I2):

                    return typeof(Int16[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_UI2):

                    return typeof(UInt16[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_I4):

                    return typeof(Int32[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_UI4):

                    return typeof(UInt32[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_I8):

                    return typeof(Int64[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_UI8):

                    return typeof(UInt64[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_R8):

                    return typeof(Double[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_BOOL):

                    return typeof(Boolean[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME):

                    return typeof(DateTime[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_CLSID):

                    return typeof(IntPtr[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_CF):

                    return typeof(IntPtr[]);

                case (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR):

                    return typeof(String[]);

                default:

                    return typeof(Object);
            }
        }

        /// <summary>
        /// Get the native property description COM interface
        /// </summary>
        internal IPropertyDescription NativePropertyDescription
        {
            get
            {
                if (nativePropertyDescription == null)
                {
                    Guid guid = new Guid(ShellIIDGuid.IPropertyDescription);
                    PropertySystemNativeMethods.PSGetPropertyDescription(ref propertyKey, ref guid, out nativePropertyDescription);
                }

                return nativePropertyDescription;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release the native objects
        /// </summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        public void Dispose(bool disposing)
        {
            if (nativePropertyDescription != null)
            {
                Marshal.ReleaseComObject(nativePropertyDescription);
                nativePropertyDescription = null;
            }

            if (disposing)
            {
                // and the managed ones
                canonicalName = null;
                displayName = null;
                editInvitation = null;
                defaultColumWidth = null;
                valueType = null;
                propertyEnumTypes = null;
            }
        }

        /// <summary>
        /// Release the native objects
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release the native objects
        /// </summary>
        ~ShellPropertyDescription()
        {
            Dispose(false);
        }

        #endregion
    }
}
