//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace MS.WindowsAPICodePack.Internal
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    /// This class is intended for internal use only.
    /// </summary>
    /// <remarks>
    /// Must call Clear when finished to avoid memory leaks. If you get the value of
    /// a VT_UNKNOWN prop, an implicit AddRef is called, thus your reference will
    /// be active even after the PropVariant struct is cleared.
    /// Correct usage:
    /// 
    ///     PropVariant propVar;
    ///     GetProp(out propVar);
    ///     try
    ///     {
    ///         object value = propVar.Value;
    ///     }
    ///     finally { propVar.Clear(); }
    ///     
    /// Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx
    /// and modified to support additional types inculding vectors and ability to set values
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct PropVariant
    {
        #region Fields

        // The layout of these elements needs to be maintained.
        //
        // NOTE: We could use LayoutKind.Explicit, but we want
        //       to maintain that the IntPtr may be 8 bytes on
        //       64-bit architectures, so we'll let the CLR keep
        //       us aligned.
        //

        // This is actually a VarEnum value, but the VarEnum type
        // requires 4 bytes instead of the expected 2.
        ushort valueType;
        
        // Reserved Fields
        ushort wReserved1;
        ushort wReserved2;
        ushort wReserved3;


        // In order to allow x64 compat, we need to allow for
        // expansion of the IntPtr. However, the BLOB struct
        // uses a 4-byte int, followed by an IntPtr, so
        // although the valueData field catches most pointer values,
        // we need an additional 4-bytes to get the BLOB
        // pointer. The valueDataExt field provides this, as well as
        // the last 4-bytes of an 8-byte value on 32-bit
        // architectures.
        IntPtr valueData;
        Int32 valueDataExt;        
        #endregion // struct fields

        #region public Methods

        /// <summary>
        /// Creates a PropVariant from an object
        /// </summary>
        /// <param name="value">The object containing the data.</param>
        /// <returns>An initialized PropVariant</returns>
        public static PropVariant FromObject( object value )
        {
            PropVariant propVar = new PropVariant( );

            if( value == null )
            {
                propVar.Clear( );
                return propVar;
            }

            if( value.GetType( ) == typeof( string ) )
            {
                //Strings require special consideration, because they cannot be empty as well
                if( String.IsNullOrEmpty( value as string ) || String.IsNullOrEmpty( (value as string).Trim( ) ) )
                    throw new ArgumentException( "String argument cannot be null or empty." );
                propVar.SetString( value as string );
            }
            else if( value.GetType( ) == typeof( bool? ) )
            {
                propVar.SetBool( (value as bool?).Value );
            }
            else if( value.GetType( ) == typeof( bool ) )
            {
                propVar.SetBool( (bool)value );
            }
            else if( value.GetType( ) == typeof( byte? ) )
            {
                propVar.SetByte( (value as byte?).Value );
            }
            else if( value.GetType( ) == typeof( byte ) )
            {
                propVar.SetByte( (byte)value );
            }
            else if( value.GetType( ) == typeof( sbyte? ) )
            {
                propVar.SetSByte( (value as sbyte?).Value );
            }
            else if( value.GetType( ) == typeof( sbyte ) )
            {
                propVar.SetSByte( (sbyte)value );
            }
            else if( value.GetType( ) == typeof( short? ) )
            {
                propVar.SetShort( (value as short?).Value );
            }
            else if( value.GetType( ) == typeof( short ) )
            {
                propVar.SetShort( (short)value );
            }
            else if( value.GetType( ) == typeof( ushort? ) )
            {
                propVar.SetUShort( (value as ushort?).Value );
            }
            else if( value.GetType( ) == typeof( ushort ) )
            {
                propVar.SetUShort( (ushort)value );
            }
            else if( value.GetType( ) == typeof( int? ) )
            {
                propVar.SetInt( (value as int?).Value );
            }
            else if( value.GetType( ) == typeof( int ) )
            {
                propVar.SetInt( (int)value );
            }
            else if( value.GetType( ) == typeof( uint? ) )
            {
                propVar.SetUInt( (value as uint?).Value );
            }
            else if( value.GetType( ) == typeof( uint ) )
            {
                propVar.SetUInt( (uint)value );
            }
            else if( value.GetType( ) == typeof( long? ) )
            {
                propVar.SetLong( (value as long?).Value );
            }
            else if( value.GetType( ) == typeof( long ) )
            {
                propVar.SetLong( (long)value );
            }
            else if( value.GetType( ) == typeof( ulong? ) )
            {
                propVar.SetULong( (value as ulong?).Value );
            }
            else if( value.GetType( ) == typeof( ulong ) )
            {
                propVar.SetULong( (ulong)value );
            }
            else if( value.GetType( ) == typeof( double? ) )
            {
                propVar.SetDouble( (value as double?).Value );
            }
            else if( value.GetType( ) == typeof( double ) )
            {
                propVar.SetDouble( (double)value );
            }
            else if( value.GetType( ) == typeof( float? ) )
            {
                propVar.SetDouble( (double)((value as float?).Value) );
            }
            else if( value.GetType( ) == typeof( float ) )
            {
                propVar.SetDouble( (double)(float)value );
            }
            else if( value.GetType( ) == typeof( DateTime? ) )
            {
                propVar.SetDateTime( (value as DateTime?).Value );
            }
            else if( value.GetType( ) == typeof( DateTime ) )
            {
                propVar.SetDateTime( (DateTime)value );
            }
            else if( value.GetType( ) == typeof( string[ ] ) )
            {
                propVar.SetStringVector( (value as string[ ]) );
            }
            else if( value.GetType( ) == typeof( short[ ] ) )
            {
                propVar.SetShortVector( (value as short[ ]) );
            }
            else if( value.GetType( ) == typeof( ushort[ ] ) )
            {
                propVar.SetUShortVector( (value as ushort[ ]) );
            }
            else if( value.GetType( ) == typeof( int[ ] ) )
            {
                propVar.SetIntVector( (value as int[ ]) );
            }
            else if( value.GetType( ) == typeof( uint[ ] ) )
            {
                propVar.SetUIntVector( (value as uint[ ]) );
            }
            else if( value.GetType( ) == typeof( long[ ] ) )
            {
                propVar.SetLongVector( (value as long[ ]) );
            }
            else if( value.GetType( ) == typeof( ulong[ ] ) )
            {
                propVar.SetULongVector( (value as ulong[ ]) );
            }
            else if( value.GetType( ) == typeof( DateTime[ ] ) )
            {
                propVar.SetDateTimeVector( (value as DateTime[ ]) );
            }
            else if( value.GetType( ) == typeof( bool[ ] ) )
            {
                propVar.SetBoolVector( (value as bool[ ]) );
            }
            else
            {
                throw new ArgumentException( "This Value type is not supported." );
            }

            return propVar;
        }


        /// <summary>
        /// Called to clear the PropVariant's referenced and local memory.
        /// </summary>
        /// <remarks>
        /// You must call Clear to avoid memory leaks.
        /// </remarks>
        public void Clear()
        {
            // Can't pass "this" by ref, so make a copy to call PropVariantClear with
            PropVariant var = this;
            PropVariantNativeMethods.PropVariantClear(ref var);

            // Since we couldn't pass "this" by ref, we need to clear the member fields manually
            // NOTE: PropVariantClear already freed heap data for us, so we are just setting
            //       our references to null.
            valueType = (ushort)VarEnum.VT_EMPTY;
            wReserved1 = wReserved2 = wReserved3 = 0;
            valueData = IntPtr.Zero;
            valueDataExt = 0;
        }

        /// <summary>
        /// Set a string value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetString(string value)
        {
            valueType = (ushort)VarEnum.VT_LPWSTR;
            valueData = Marshal.StringToCoTaskMemUni(value);
        }

        /// <summary>
        /// Set a string vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetStringVector(string[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromStringVector(value, (uint) value.Length, out propVar);
            CopyData(propVar);

        }

        /// <summary>
        /// Set a bool vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetBoolVector(bool[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromBooleanVector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetShortVector(short[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt16Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetUShortVector(ushort[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt16Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set an int vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetIntVector(int[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt32Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set an uint vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetUIntVector(uint[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt32Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a long vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetLongVector(long[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt64Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a ulong vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetULongVector(ulong[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a double vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetDoubleVector(double[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromDoubleVector(value, (uint)value.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a DateTime vector
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetDateTimeVector(DateTime[] value)
        {
            System.Runtime.InteropServices.ComTypes.FILETIME[] fileTimeArr = 
                new System.Runtime.InteropServices.ComTypes.FILETIME[value.Length];

            for (int i = 0; i < value.Length; i++ )
            {
                fileTimeArr[i] = DateTimeTotFileTime(value[i]);
            }

            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromFileTimeVector(fileTimeArr, (uint)fileTimeArr.Length, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set an IUnknown value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetIUnknown(object value)
        {
            valueType = (ushort)VarEnum.VT_UNKNOWN;
            valueData = Marshal.GetIUnknownForObject(value);
        }

        /// <summary>
        /// Set a bool value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetBool(bool value)
        {
            valueType = (ushort)VarEnum.VT_BOOL;
            valueData = (value == true) ? (IntPtr)(-1) : (IntPtr)0;
        }

        /// <summary>
        /// Set a DateTime value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetDateTime(DateTime value)
        {
            valueType = (ushort)VarEnum.VT_FILETIME;

            PropVariant propVar;
            System.Runtime.InteropServices.ComTypes.FILETIME ft = DateTimeTotFileTime(value);
            PropVariantNativeMethods.InitPropVariantFromFileTime(ref ft, out propVar);
            CopyData(propVar);
        }

        /// <summary>
        /// Set a safe array value
        /// </summary>
        /// <param name="array">The new value to set.</param>
        public void SetSafeArray(Array array)
        {
            const ushort vtUnknown = 13;
            IntPtr psa = PropVariantNativeMethods.SafeArrayCreateVector(vtUnknown, 0, (uint)array.Length);

            IntPtr pvData = PropVariantNativeMethods.SafeArrayAccessData(psa);
            try // to remember to release lock on data
            {
                for (int i = 0; i < array.Length; ++i)
                {
                    object obj = array.GetValue(i);
                    IntPtr punk = (obj != null) ? Marshal.GetIUnknownForObject(obj) : IntPtr.Zero;
                    Marshal.WriteIntPtr(pvData, i * IntPtr.Size, punk);
                }
            }
            finally
            {
                PropVariantNativeMethods.SafeArrayUnaccessData(psa);
            }

            this.valueType = (ushort)VarEnum.VT_ARRAY | (ushort)VarEnum.VT_UNKNOWN;
            this.valueData = psa;
        }

        /// <summary>
        /// Set a byte value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetByte(byte value)
        {
            valueType = (ushort)VarEnum.VT_UI1;
            valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set a sbyte value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetSByte(sbyte value)
        {
            valueType = (ushort)VarEnum.VT_I1;
            valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set a short value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetShort(short value)
        {
            valueType = (ushort)VarEnum.VT_I2;
            valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an unsigned short value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetUShort(ushort value)
        {
            valueType = (ushort)VarEnum.VT_UI2;
            valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an int value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetInt(int value)
        {
            valueType = (ushort)VarEnum.VT_I4;
            valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an unsigned int value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetUInt(uint value)
        {
            valueType = (ushort)VarEnum.VT_UI4;
            valueData = (IntPtr)(int)value;
        }

        /// <summary>
        /// Set a decimal  value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetDecimal(decimal value)
        {
            int[] bits = Decimal.GetBits(value);
            valueData = (IntPtr)bits[0];
            valueDataExt = bits[1];
            wReserved3 = (ushort)(bits[2] >> 16);
            wReserved2 = (ushort)(bits[2] & 0x0000FFFF);
            wReserved1 = (ushort)(bits[3] >> 16);
            valueType = (ushort)VarEnum.VT_DECIMAL;
        }

        /// <summary>
        /// Set a long
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetLong(long value)
        {
            long[] valueArr = new long[] { value };
            
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt64Vector(valueArr, 1, out propVar);

            CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Set a ulong
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetULong(ulong value)
        {
            PropVariant propVar;
            ulong[] valueArr = new ulong[] { value };
            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(valueArr, 1, out propVar);

            CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Set a double
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetDouble(double value)
        {
            double[] valueArr = new double[] { value };

            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromDoubleVector(valueArr, 1, out propVar);

            CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Sets the value type to empty
        /// </summary>
        public void SetEmptyValue()
        {
            valueType = (ushort) VarEnum.VT_EMPTY;
        }

        /// <summary>
        /// Checks if this has an empty or null value
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty
        {
            get
            {
                return (valueType == (ushort)VarEnum.VT_EMPTY || valueType == (ushort)VarEnum.VT_NULL);
            }
        }
        
        #endregion

        #region public Properties

        /// <summary>
        /// Gets or sets the variant type.
        /// </summary>
        public VarEnum VarType
        {
            get { return (VarEnum)valueType; }
            set { valueType = (ushort) value; }
        }

        /// <summary>
        /// Gets the variant value.
        /// </summary>
        public object Value
        {
            get
            {
                switch ((VarEnum)valueType)
                {
                    case VarEnum.VT_I1:
                        return cVal;
                    case VarEnum.VT_UI1:
                        return bVal;
                    case VarEnum.VT_I2:
                        return iVal;
                    case VarEnum.VT_UI2:
                        return uiVal;
                    case VarEnum.VT_I4:
                    case VarEnum.VT_INT:
                        return lVal;
                    case VarEnum.VT_UI4:
                    case VarEnum.VT_UINT:
                        return ulVal;
                    case VarEnum.VT_I8:
                        return hVal;
                    case VarEnum.VT_UI8:
                        return uhVal;
                    case VarEnum.VT_R4:
                        return fltVal;
                    case VarEnum.VT_R8:
                        return dblVal;
                    case VarEnum.VT_BOOL:
                        return boolVal;
                    case VarEnum.VT_ERROR:
                        return scode;
                    case VarEnum.VT_CY:
                        return cyVal;
                    case VarEnum.VT_DATE:
                        return date;
                    case VarEnum.VT_FILETIME:
                        return DateTime.FromFileTime(hVal);
                    case VarEnum.VT_BSTR:
                        return Marshal.PtrToStringBSTR(valueData);
                    case VarEnum.VT_BLOB:
                        return GetBlobData();
                    case VarEnum.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(valueData);
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(valueData);
                    case VarEnum.VT_UNKNOWN:
                        return Marshal.GetObjectForIUnknown(valueData);
                    case VarEnum.VT_DISPATCH:
                        return Marshal.GetObjectForIUnknown(valueData);
                    case VarEnum.VT_DECIMAL:
                        return decVal;
                    case VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN:
                        return CrackSingleDimSafeArray(valueData);
                    case (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR):
                        return GetStringVector();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I2):
                        return GetVector<Int16>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI2):
                            return GetVector<UInt16>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I4):
                            return GetVector<Int32>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI4):
                            return GetVector<UInt32>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I8):
                            return GetVector<Int64>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI8):
                            return GetVector<UInt64>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_R8):
                            return GetVector<Double>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_BOOL):
                            return GetVector<Boolean>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME):
                            return GetVector<DateTime>();
                    default:
                        // if the value cannot be marshaled
                        return null;
                }
            }
        }

        #endregion

        #region PropVariant Simple Data types

        sbyte cVal // CHAR cVal;
        {
            get { return (sbyte)GetDataBytes()[0]; }
        }

        byte bVal // UCHAR bVal;
        {
            get { return GetDataBytes()[0]; }
        }

        short iVal // SHORT iVal;
        {
            get { return BitConverter.ToInt16(GetDataBytes(), 0); }
        }

        ushort uiVal // USHORT uiVal;
        {
            get { return BitConverter.ToUInt16(GetDataBytes(), 0); }
        }

        int lVal // LONG lVal;
        {
            get { return BitConverter.ToInt32(GetDataBytes(), 0); }
        }

        uint ulVal // ULONG ulVal;
        {
            get { return BitConverter.ToUInt32(GetDataBytes(), 0); }
        }

        long hVal // LARGE_INTEGER hVal;
        {
            get { return BitConverter.ToInt64(GetDataBytes(), 0); }
        }

        ulong uhVal // ULARGE_INTEGER uhVal;
        {
            get { return BitConverter.ToUInt64(GetDataBytes(), 0); }
        }

        float fltVal // FLOAT fltVal;
        {
            get { return BitConverter.ToSingle(GetDataBytes(), 0); }
        }

        double dblVal // DOUBLE dblVal;
        {
            get { return BitConverter.ToDouble(GetDataBytes(), 0); }
        }

        bool boolVal // VARIANT_BOOL boolVal;
        {
            get { return (iVal == 0 ? false : true); }
        }

        int scode // SCODE scode;
        {
            get { return lVal; }
        }

        decimal cyVal // CY cyVal;
        {
            get { return decimal.FromOACurrency(hVal); }
        }

        DateTime date // DATE date;
        {
            get { return DateTime.FromOADate(dblVal); }
        }

        Decimal decVal  // Decimal value
        {
            get
            {
                int[] bits = new int[4];
                bits[0] = (int)valueData;
                bits[1] = valueDataExt;
                bits[2] = (wReserved3 << 16) | wReserved2;
                bits[3] = (wReserved1 << 16);
                return new decimal(bits);
            }
        }

        #endregion 

        #region Private Methods

        private void CopyData(PropVariant propVar)
        {
            this.valueType = propVar.valueType;
            this.valueData = propVar.valueData;
            this.valueDataExt = propVar.valueDataExt;
        }

        private void CreatePropVariantFromVectorElement(PropVariant propVar)
        {
            //Copy the first vector element to a new PropVariant
            CopyData(propVar);
            PropVariantNativeMethods.InitPropVariantFromPropVariantVectorElem(ref this, 0, out propVar);

            //Overwrite the existing data
            CopyData(propVar);
        }

        private static long FileTimeToDateTime(ref System.Runtime.InteropServices.ComTypes.FILETIME val)
        {
            return (((long)val.dwHighDateTime) << 32) + val.dwLowDateTime;
        }

        private static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeTotFileTime(DateTime value)
        {
            long hFT = value.ToFileTime();
            System.Runtime.InteropServices.ComTypes.FILETIME ft =
                new System.Runtime.InteropServices.ComTypes.FILETIME();
            ft.dwLowDateTime = (int)(hFT & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(hFT >> 32);
            return ft;
        }

        private object GetBlobData()
        {
            byte[] blobData = new byte[lVal];
            IntPtr pBlobData;
            if (IntPtr.Size == 4)
            {
                pBlobData = new IntPtr(valueDataExt);
            }
            else if( IntPtr.Size == 8 )
            {
                // In this case, we need to derive a pointer at offset 12,
                // because the size of the blob is represented as a 4-byte int
                // but the pointer is immediately after that.
                pBlobData = new IntPtr(
                    (Int64)(BitConverter.ToInt32( GetDataBytes( ), sizeof( int ) )) +
                    (Int64)(BitConverter.ToInt32( GetDataBytes( ), 2 * sizeof( int ) ) << 32) );
            }
            else
            {
                throw new NotSupportedException( );
            }
            Marshal.Copy(pBlobData, blobData, 0, lVal);
 
            return blobData;
        }

        private Array GetVector<T>() where T : struct
        {
            int count = PropVariantNativeMethods.PropVariantGetElementCount(ref this);
            if (count <= 0)
                return null;

            Array arr = new T[count];

            for (uint i = 0; i < count; i++)
            {
                if (typeof(T) == typeof(Int16))
                {
                    short val;
                    PropVariantNativeMethods.PropVariantGetInt16Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt16))
                {
                    ushort val;
                    PropVariantNativeMethods.PropVariantGetUInt16Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Int32))
                {
                    int val;
                    PropVariantNativeMethods.PropVariantGetInt32Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt32))
                {
                    uint val;
                    PropVariantNativeMethods.PropVariantGetUInt32Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Int64))
                {
                    long val;
                    PropVariantNativeMethods.PropVariantGetInt64Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt64))
                {
                    ulong val;
                    PropVariantNativeMethods.PropVariantGetUInt64Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    System.Runtime.InteropServices.ComTypes.FILETIME val;
                    PropVariantNativeMethods.PropVariantGetFileTimeElem(ref this, i, out val);

                    long fileTime = FileTimeToDateTime(ref val);


                    arr.SetValue(DateTime.FromFileTime(fileTime), i);
                }
                else if (typeof(T) == typeof(Boolean))
                {
                    bool val;
                    PropVariantNativeMethods.PropVariantGetBooleanElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Double))
                {
                    double val;
                    PropVariantNativeMethods.PropVariantGetDoubleElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(String))
                {
                    string val;
                    PropVariantNativeMethods.PropVariantGetStringElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
            }

            return arr;
        }

        // A string requires a special case because it's not a struct or value type
        private string[] GetStringVector()
        {
            int count = PropVariantNativeMethods.PropVariantGetElementCount(ref this);
            if (count <= 0)
                return null;

            string[] strArr = new string[count];
            for (uint i = 0; i < count; i++)
            {
                PropVariantNativeMethods.PropVariantGetStringElem(ref this, i, out strArr[i]);
            }

            return strArr;
        }

        /// <summary>
        /// Gets a byte array containing the data bits of the struct.
        /// </summary>
        /// <returns>A byte array that is the combined size of the data bits.</returns>
        private byte[] GetDataBytes()
        {
            byte[] ret = new byte[IntPtr.Size + sizeof(int)];
            if (IntPtr.Size == 4)
                BitConverter.GetBytes(valueData.ToInt32()).CopyTo(ret, 0);
            else if (IntPtr.Size == 8)
                BitConverter.GetBytes(valueData.ToInt64()).CopyTo(ret, 0);
            BitConverter.GetBytes(valueDataExt).CopyTo(ret, IntPtr.Size);
            return ret;
        }

        Array CrackSingleDimSafeArray(IntPtr psa)
        {
            uint cDims = PropVariantNativeMethods.SafeArrayGetDim(psa);
            if (cDims != 1) 
                throw new ArgumentException("Multi-dimensional SafeArrays not supported.");

            int lBound = PropVariantNativeMethods.SafeArrayGetLBound(psa, 1U);
            int uBound = PropVariantNativeMethods.SafeArrayGetUBound(psa, 1U);

            int n = uBound - lBound + 1; // uBound is inclusive

            object[] array = new object[n];
            for (int i = lBound; i <= uBound; ++i)
            {
                array[i] = PropVariantNativeMethods.SafeArrayGetElement(psa, ref i);
            }

            return array;
        }

        #endregion

    }
}
