//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
        internal static class PropVariantNativeMethods
        {
            [DllImport("Ole32.dll", PreserveSig = false)] // returns hresult
            internal extern static void PropVariantClear([In, Out] ref PropVariant pvar);

            [DllImport("OleAut32.dll", PreserveSig = true)] // psa is actually returned, not hresult
            internal extern static IntPtr SafeArrayCreateVector(ushort vt, int lowerBound, uint cElems);

            [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
            internal extern static IntPtr SafeArrayAccessData(IntPtr psa);

            [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
            internal extern static void SafeArrayUnaccessData(IntPtr psa);

            [DllImport("OleAut32.dll", PreserveSig = true)] // retuns uint32
            internal extern static uint SafeArrayGetDim(IntPtr psa);
            
            [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
            internal extern static int SafeArrayGetLBound(IntPtr psa, uint nDim);
            
            [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
            internal extern static int SafeArrayGetUBound(IntPtr psa, uint nDim);

            // This decl for SafeArrayGetElement is only valid for cDims==1!
            [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
            [return: MarshalAs(UnmanagedType.IUnknown)]
            internal extern static object SafeArrayGetElement(IntPtr psa, ref int rgIndices);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int InitPropVariantFromPropVariantVectorElem([In] ref PropVariant propvarIn, uint iElem, [Out] out PropVariant ppropvar);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern uint InitPropVariantFromFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME pftIn, out PropVariant ppropvar);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.I4)]
            internal static extern int PropVariantGetElementCount([In] ref PropVariant propVar);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetBooleanElem([In] ref PropVariant propVar, [In]uint iElem, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetInt16Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out short pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetUInt16Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out ushort pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetInt32Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out int pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetUInt32Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out uint pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetInt64Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out Int64 pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetUInt64Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out UInt64 pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetDoubleElem([In] ref PropVariant propVar, [In] uint iElem, [Out] out double pnVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetFileTimeElem([In] ref PropVariant propVar, [In] uint iElem, [Out, MarshalAs(UnmanagedType.Struct)] out System.Runtime.InteropServices.ComTypes.FILETIME pftVal);

            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void PropVariantGetStringElem([In] ref PropVariant propVar, [In]  uint iElem, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszVal);


            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromBooleanVector([In, Out] bool [] prgf, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromInt16Vector([In, Out] Int16 [] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromUInt16Vector([In, Out] UInt16[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromInt32Vector([In, Out] Int32[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromUInt32Vector([In, Out] UInt32[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromInt64Vector([In, Out] Int64[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromUInt64Vector([In,Out] UInt64[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromDoubleVector([In, Out] double[] prgn, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromFileTimeVector([In, Out] System.Runtime.InteropServices.ComTypes.FILETIME[] prgft, uint cElems, out PropVariant ppropvar);
            
            [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern void InitPropVariantFromStringVector([In, Out] string[] prgsz, uint cElems, out PropVariant ppropvar);
        }
}
