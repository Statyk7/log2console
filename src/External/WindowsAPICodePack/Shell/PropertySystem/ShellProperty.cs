//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    /// <summary>
    /// Defines a strongly-typed property object. 
    /// All writable property objects must be of this type 
    /// to be able to call the value setter.
    /// </summary>
    /// <typeparam name="T">The type of this property's value. 
    /// Because a property value can be empty, only nullable types 
    /// are allowed.</typeparam>
    public class ShellProperty<T> : IShellProperty
    {
        #region Private Fields

        private PropertyKey propertyKey;
        string imageReferencePath = null;
        int? imageReferenceIconIndex;
        private ShellPropertyDescription description = null;

        // The value was set but truncated in a string value or rounded if a numeric value.
        private const int InPlaceStringTruncated = 0x00401A0;

        #endregion

        #region Private Methods
         
        private ShellObject ParentShellObject { get; set; }

        private IPropertyStore NativePropertyStore { get; set; }

        private void GetImageReference()
        {
            PropVariant propVar;

            IPropertyStore store = ShellPropertyCollection.CreateDefaultPropertyStore(ParentShellObject);

            store.GetValue(ref propertyKey, out propVar);

            Marshal.ReleaseComObject(store);
            store = null;

            string refPath;
            ((IPropertyDescription2)Description.NativePropertyDescription).GetImageReferenceForValue(
                ref propVar, out refPath);

            if (refPath == null)
            {
                return;
            }

            int index = ShellNativeMethods.PathParseIconLocation(ref refPath);
            if (refPath != null)
            {
                imageReferencePath = refPath;
                imageReferenceIconIndex = index;
            }
        }

        private void StorePropVariantValue(PropVariant propVar)
        {
            Guid guid = new Guid(ShellIIDGuid.IPropertyStore);
            IPropertyStore writablePropStore = null;
            try
            {
                int hr = ParentShellObject.NativeShellItem2.GetPropertyStore(
                        ShellNativeMethods.GETPROPERTYSTOREFLAGS.GPS_READWRITE,
                        ref guid,
                        out writablePropStore);

                if (!CoreErrorHelper.Succeeded(hr))
                {
                    throw new ExternalException("Unable to get writable property store for this property.",
                        Marshal.GetExceptionForHR(hr));
                }

                int result = writablePropStore.SetValue(ref propertyKey, ref propVar);

                if (!AllowSetTruncatedValue && (result == InPlaceStringTruncated))
                {
                    throw new ArgumentOutOfRangeException("propVar", "A value had to be truncated in a string or rounded if a numeric value. Set AllowTruncatedValue to true to prevent this exception.");
                }

                if (!CoreErrorHelper.Succeeded(result))
                {
                    throw new ExternalException("Unable to set property.", Marshal.GetExceptionForHR(result));
                }

                writablePropStore.Commit();

            }
            catch (InvalidComObjectException e)
            {
                throw new ExternalException("Unable to get writable property store for this property.", e);
            }
            catch (InvalidCastException)
            {
                throw new ExternalException("Unable to get writable property store for this property.");
            }
            finally
            {
                if (writablePropStore != null)
                {
                    Marshal.ReleaseComObject(writablePropStore);
                    writablePropStore = null;
                }
            }
        }

        #endregion

        #region Internal Constructor

        /// <summary>
        /// Constructs a new Property object
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="description"></param>
        /// <param name="parent"></param>
        internal ShellProperty(
            PropertyKey propertyKey,
            ShellPropertyDescription description,
            ShellObject parent
            )
        {
            this.propertyKey = propertyKey;
            this.description = description;
            ParentShellObject = parent;
            AllowSetTruncatedValue = false;
        }

        /// <summary>
        /// Constructs a new Property object
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="description"></param>
        /// <param name="propertyStore"></param>
        internal ShellProperty(
            PropertyKey propertyKey,
            ShellPropertyDescription description,
            IPropertyStore propertyStore
            )
        {
            this.propertyKey = propertyKey;
            this.description = description;
            NativePropertyStore = propertyStore;
            AllowSetTruncatedValue = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the strongly-typed value of this property.
        /// The value of the property is cleared if the value is set to null.
        /// </summary>
        /// <exception cref="System.Runtime.InteropServices.COMException">
        /// If the property value cannot be retrieved or updated in the Property System</exception>
        /// <exception cref="NotSupportedException">If the type of this property is not supported; e.g. writing a binary object.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="AllowSetTruncatedValue"/> is false, and either 
        /// a string value was truncated or a numeric value was rounded.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "We are not currently handling globalization or localization")]
        public T Value
        {
            get
            {
                // Make sure we load the correct type
                Debug.Assert(ValueType == ShellPropertyDescription.VarEnumToSystemType(Description.VarEnumType));

                PropVariant propVar = new PropVariant();

                if (ParentShellObject.NativePropertyStore != null)
                {
                    // If there is a valid property store for this shell object, then use it.
                    ParentShellObject.NativePropertyStore.GetValue(ref propertyKey, out propVar);
                }
                else if (ParentShellObject != null)
                {
                    // Use IShellItem2.GetProperty instead of creating a new property store
                    // The file might be locked. This is probably quicker, and sufficient for what we need
                    ParentShellObject.NativeShellItem2.GetProperty(ref propertyKey, out propVar);
                }
                else if (NativePropertyStore != null)
                {
                    NativePropertyStore.GetValue(ref propertyKey, out propVar);
                }

                //Get the value
                T value;
                try
                {
                    value = (T)propVar.Value;
                }
                finally
                {
                    propVar.Clear();
                }

                return value;
            }


            set
            {
                // Make sure we use the correct type
                Debug.Assert(ValueType == ShellPropertyDescription.VarEnumToSystemType(Description.VarEnumType));

                if (typeof(T) != ValueType)
                {
                    throw new NotSupportedException(
                        String.Format("This property only accepts a value of type \"{0}\".", ValueType.Name));
                }

                if (value is Nullable)
                {
                    Type t = typeof(T);
                    PropertyInfo pi = t.GetProperty("HasValue");
                    if (pi != null)
                    {
                        bool hasValue = (bool)pi.GetValue(value, null);
                        if (!hasValue)
                        {
                            ClearValue();
                            return;
                        }
                    }
                }
                else if (value == null)
                {
                    ClearValue();
                    return;
                }

                if (ParentShellObject != null)
                {
                    using (ShellPropertyWriter propertyWriter = ParentShellObject.Properties.GetPropertyWriter())
                    {
                        propertyWriter.WriteProperty<T>(this, value, AllowSetTruncatedValue);
                    }
                }
                else if (NativePropertyStore != null)
                {
                    throw new InvalidOperationException("The value on this property cannot be set. To set the property value, use the ShellObject that is associated with this property.");
                }
            }
        }


        #endregion

        #region IProperty Members

        /// <summary>
        /// Gets the property key identifying this property.
        /// </summary>
        public PropertyKey PropertyKey
        {
            get
            {
                return propertyKey;
            }
        }
        /// <summary>
        /// Returns a formatted, Unicode string representation of a property value.
        /// </summary>
        /// <param name="format">One or more of the PropertyDescriptionFormat flags 
        /// that indicate the desired format.</param>
        /// <param name="formattedString">The formatted value as a string, or null if this property 
        /// cannot be formatted for display.</param>
        /// <returns>True if the method successfully locates the formatted string; otherwise 
        /// False.</returns>
        public bool TryFormatForDisplay(PropertyDescriptionFormat format, out string formattedString)
        {
            PropVariant propVar;

            if (Description == null || Description.NativePropertyDescription == null)
            {
                // We cannot do anything without a property description
                formattedString = null;
                return false;
            }

            IPropertyStore store = ShellPropertyCollection.CreateDefaultPropertyStore(ParentShellObject);

            store.GetValue(ref propertyKey, out propVar);

            // Release the Propertystore
            Marshal.ReleaseComObject(store);
            store = null;

            HRESULT hr = Description.NativePropertyDescription.FormatForDisplay(ref propVar, ref format, out formattedString);

            // Sometimes, the value cannot be displayed properly, such as for blobs
            // or if we get argument exception
            if (!CoreErrorHelper.Succeeded((int)hr))
            {
                formattedString = null;
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Returns a formatted, Unicode string representation of a property value.
        /// </summary>
        /// <param name="format">One or more of the PropertyDescriptionFormat flags 
        /// that indicate the desired format.</param>
        /// <returns>The formatted value as a string, or null if this property 
        /// cannot be formatted for display.</returns>
        public string FormatForDisplay(PropertyDescriptionFormat format)
        {
            string formattedString;
            PropVariant propVar;

            if (Description == null || Description.NativePropertyDescription == null)
            {
                // We cannot do anything without a property description
                return null;
            }

            IPropertyStore store = ShellPropertyCollection.CreateDefaultPropertyStore(ParentShellObject);

            store.GetValue(ref propertyKey, out propVar);

            // Release the Propertystore
            Marshal.ReleaseComObject(store);
            store = null;

            HRESULT hr = Description.NativePropertyDescription.FormatForDisplay(ref propVar, ref format, out formattedString);

            // Sometimes, the value cannot be displayed properly, such as for blobs
            // or if we get argument exception
            if (!CoreErrorHelper.Succeeded((int)hr))
                throw Marshal.GetExceptionForHR((int)hr);
            else
                return formattedString;
        }

        /// <summary>
        /// Get the property description object.
        /// </summary>
        public ShellPropertyDescription Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Gets the case-sensitive name of a property as it is known to the system,
        /// regardless of its localized name.
        /// </summary>
        public string CanonicalName
        {
            get
            {
                return Description.CanonicalName;
            }
        }

        /// <summary>
        /// Clears the value of the property.
        /// </summary>
        public void ClearValue()
        {
            PropVariant propVar = new PropVariant();
            propVar.SetEmptyValue();

            StorePropVariantValue(propVar);
        }

        /// <summary>
        /// Gets the value for this property using the generic Object type.
        /// To obtain a specific type for this value, use the more type strong
        /// Property&lt;T&gt; class.
        /// Also, you can only set a value for this type using Property&lt;T&gt;
        /// </summary>
        public object ValueAsObject
        {
            get
            {
                PropVariant propVar = new PropVariant();
                propVar.Clear();

                if (ParentShellObject != null)
                {

                    IPropertyStore store = ShellPropertyCollection.CreateDefaultPropertyStore(ParentShellObject);

                    store.GetValue(ref propertyKey, out propVar);

                    Marshal.ReleaseComObject(store);
                    store = null;
                }
                else if (NativePropertyStore != null)
                    NativePropertyStore.GetValue(ref propertyKey, out propVar);

                object value = null;

                try
                {
                    value = propVar.Value;
                }
                finally
                {
                    propVar.Clear();
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the associated runtime type.
        /// </summary>
        public Type ValueType
        {
            get
            {
                // The type for this object need to match that of the description
                Debug.Assert(Description.ValueType == typeof(T));

                return Description.ValueType;
            }
        }

        /// <summary>
        /// Gets the image reference path and icon index associated with a property value (Windows 7 only).
        /// </summary>
        public IconReference IconReference
        {
            get
            {
                if (!CoreHelpers.RunningOnWin7)
                {
                    throw new PlatformNotSupportedException("This Property is available on Windows 7 only.");
                }

                GetImageReference();
                int index = (imageReferenceIconIndex.HasValue ? imageReferenceIconIndex.Value : -1);

                return new IconReference(imageReferencePath, index);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if a value can be truncated. The default for this property is false.
        /// </summary>
        /// <remarks>
        /// An <see cref="ArgumentOutOfRangeException"/> will be thrown if
        /// this property is not set to true, and a property value was set
        /// but later truncated. 
        /// 
        /// </remarks>
        public bool AllowSetTruncatedValue
        {
            get;
            set;
        }

        #endregion
    }
}
