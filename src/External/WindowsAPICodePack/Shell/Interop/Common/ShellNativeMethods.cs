//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsAPICodePack.Shell
{
    internal static class ShellNativeMethods
    {
        static ShellNativeMethods()
        {
            // Hide default constructor
        }

        #region TaskDialog Definitions

        // Identify button *return values* - note that, unfortunately, these are different
        // from the inbound button values.
        internal enum TASKDIALOG_COMMON_BUTTON_RETURN_ID
        {
            IDOK = 1,
            IDCANCEL = 2,
            IDABORT = 3,
            IDRETRY = 4,
            IDIGNORE = 5,
            IDYES = 6,
            IDNO = 7,
            IDCLOSE = 8
        }

        #endregion

        #region Shell Enums

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            // Ensure that items returned are filesystem items.
            FOS_FORCEFILESYSTEM = 0x00000040,
            // Allow choosing items that have no storage.
            FOS_ALLNONSTORAGEITEMS = 0x00000080,
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }
        internal enum CDCONTROLSTATE : uint
        {
            CDCS_INACTIVE = 0x00000000,
            CDCS_ENABLED = 0x00000001,
            CDCS_VISIBLE = 0x00000002
        }
        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
            SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
        }

        /// <summary>
        /// Indicate flags that modify the property store object retrieved by methods 
        /// that create a property store, such as IShellItem2::GetPropertyStore or 
        /// IPropertyStoreFactory::GetPropertyStore.
        /// </summary>
        [Flags]
        internal enum GETPROPERTYSTOREFLAGS : uint
        {
            /// <summary>
            /// Meaning to a calling process: Return a read-only property store that contains all 
            /// properties. Slow items (offline files) are not opened. 
            /// Combination with other flags: Can be overridden by other flags.
            /// </summary>
            GPS_DEFAULT = 0,

            /// <summary>
            /// Meaning to a calling process: Include only properties directly from the property
            /// handler, which opens the file on the disk, network, or device. Meaning to a file 
            /// folder: Only include properties directly from the handler.
            /// 
            /// Meaning to other folders: When delegating to a file folder, pass this flag on 
            /// to the file folder; do not do any multiplexing (MUX). When not delegating to a 
            /// file folder, ignore this flag instead of returning a failure code.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, 
            /// GPS_FASTPROPERTIESONLY, or GPS_BESTEFFORT.
            /// </summary>
            GPS_HANDLERPROPERTIESONLY = 0x1,

            /// <summary>
            /// Meaning to a calling process: Can write properties to the item. 
            /// Note: The store may contain fewer properties than a read-only store. 
            /// 
            /// Meaning to a file folder: ReadWrite.
            /// 
            /// Meaning to other folders: ReadWrite. Note: When using default MUX, 
            /// return a single unmultiplexed store because the default MUX does not support ReadWrite.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, 
            /// GPS_BESTEFFORT, or GPS_DELAYCREATION. Implies GPS_HANDLERPROPERTIESONLY.
            /// </summary>
            GPS_READWRITE = 0x2,

            /// <summary>
            /// Meaning to a calling process: Provides a writable store, with no initial properties, 
            /// that exists for the lifetime of the Shell item instance; basically, a property bag 
            /// attached to the item instance. 
            /// 
            /// Meaning to a file folder: Not applicable. Handled by the Shell item.
            /// 
            /// Meaning to other folders: Not applicable. Handled by the Shell item.
            /// 
            /// Combination with other flags: Cannot be combined with any other flag. Implies GPS_READWRITE
            /// </summary>
            GPS_TEMPORARY = 0x4,

            /// <summary>
            /// Meaning to a calling process: Provides a store that does not involve reading from the 
            /// disk or network. Note: Some values may be different, or missing, compared to a store 
            /// without this flag. 
            /// 
            /// Meaning to a file folder: Include the "innate" and "fallback" stores only. Do not load the handler.
            /// 
            /// Meaning to other folders: Include only properties that are available in memory or can 
            /// be computed very quickly (no properties from disk, network, or peripheral IO devices). 
            /// This is normally only data sources from the IDLIST. When delegating to other folders, pass this flag on to them.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, 
            /// GPS_HANDLERPROPERTIESONLY, or GPS_DELAYCREATION.
            /// </summary>
            GPS_FASTPROPERTIESONLY = 0x8,

            /// <summary>
            /// Meaning to a calling process: Open a slow item (offline file) if necessary. 
            /// Meaning to a file folder: Retrieve a file from offline storage, if necessary. 
            /// Note: Without this flag, the handler is not created for offline files.
            /// 
            /// Meaning to other folders: Do not return any properties that are very slow.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_FASTPROPERTIESONLY.
            /// </summary>
            GPS_OPENSLOWITEM = 0x10,

            /// <summary>
            /// Meaning to a calling process: Delay memory-intensive operations, such as file access, until 
            /// a property is requested that requires such access. 
            /// 
            /// Meaning to a file folder: Do not create the handler until needed; for example, either 
            /// GetCount/GetAt or GetValue, where the innate store does not satisfy the request. 
            /// Note: GetValue might fail due to file access problems.
            /// 
            /// Meaning to other folders: If the folder has memory-intensive properties, such as 
            /// delegating to a file folder or network access, it can optimize performance by 
            /// supporting IDelayedPropertyStoreFactory and splitting up its properties into a 
            /// fast and a slow store. It can then use delayed MUX to recombine them.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or 
            /// GPS_READWRITE
            /// </summary>
            GPS_DELAYCREATION = 0x20,

            /// <summary>
            /// Meaning to a calling process: Succeed at getting the store, even if some 
            /// properties are not returned. Note: Some values may be different, or missing,
            /// compared to a store without this flag. 
            /// 
            /// Meaning to a file folder: Succeed and return a store, even if the handler or 
            /// innate store has an error during creation. Only fail if substores fail.
            /// 
            /// Meaning to other folders: Succeed on getting the store, even if some properties 
            /// are not returned.
            /// 
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, 
            /// GPS_READWRITE, or GPS_HANDLERPROPERTIESONLY.
            /// </summary>
            GPS_BESTEFFORT = 0x40,

            /// <summary>
            /// Mask for valid GETPROPERTYSTOREFLAGS values.
            /// </summary>
            GPS_MASK_VALID = 0xff,
        }

        internal enum SIATTRIBFLAGS
        {
            // if multiple items and the attirbutes together.
            SIATTRIBFLAGS_AND = 0x00000001,
            // if multiple items or the attributes together.
            SIATTRIBFLAGS_OR = 0x00000002,
            // Call GetAttributes directly on the 
            // ShellFolder for multiple attributes.
            SIATTRIBFLAGS_APPCOMPAT = 0x00000003,
        }
        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }
        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }
        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,
            FDAP_TOP = 0x00000001,
        }

        [Flags]
        internal enum SIIGBF
        {
            SIIGBF_RESIZETOFIT = 0x00,
            SIIGBF_BIGGERSIZEOK = 0x01,
            SIIGBF_MEMORYONLY = 0x02,
            SIIGBF_ICONONLY = 0x04,
            SIIGBF_THUMBNAILONLY = 0x08,
            SIIGBF_INCACHEONLY = 0x10,
        }

        [Flags]
        internal enum WTS_FLAGS
        {
            WTS_EXTRACT = 0x00000000,
            WTS_INCACHEONLY = 0x00000001,
            WTS_FASTEXTRACT = 0x00000002,
            WTS_FORCEEXTRACTION = 0x00000004,
            WTS_SLOWRECLAIM = 0x00000008,
            WTS_EXTRACTDONOTCACHE = 0x00000020
        }

        [Flags]
        internal enum WTS_CACHEFLAGS
        {
            WTS_DEFAULT = 0x00000000,
            WTS_LOWQUALITY = 0x00000001,
            WTS_CACHED = 0x00000002,
        }

        internal enum WTS_ALPHATYPE
        {
            WTSAT_UNKNOWN = 0,
            WTSAT_RGB = 1,
            WTSAT_ARGB = 2,
        }

        [Flags]
        internal enum SFGAO : uint
        {
            /// <summary>
            /// The specified items can be copied.
            /// </summary>
            SFGAO_CANCOPY = 0x00000001,

            /// <summary>
            /// The specified items can be moved.
            /// </summary>
            SFGAO_CANMOVE = 0x00000002,

            /// <summary>
            /// Shortcuts can be created for the specified items. This flag has the same value as DROPEFFECT. 
            /// The normal use of this flag is to add a Create Shortcut item to the shortcut menu that is displayed 
            /// during drag-and-drop operations. However, SFGAO_CANLINK also adds a Create Shortcut item to the Microsoft 
            /// Windows Explorer's File menu and to normal shortcut menus. 
            /// If this item is selected, your application's IContextMenu::InvokeCommand is invoked with the lpVerb 
            /// member of the CMINVOKECOMMANDINFO structure set to "link." Your application is responsible for creating the link.
            /// </summary>
            SFGAO_CANLINK = 0x00000004,

            /// <summary>
            /// The specified items can be bound to an IStorage interface through IShellFolder::BindToObject.
            /// </summary>
            SFGAO_STORAGE = 0x00000008,

            /// <summary>
            /// The specified items can be renamed.
            /// </summary>
            SFGAO_CANRENAME = 0x00000010,

            /// <summary>
            /// The specified items can be deleted.
            /// </summary>
            SFGAO_CANDELETE = 0x00000020,

            /// <summary>
            /// The specified items have property sheets.
            /// </summary>
            SFGAO_HASPROPSHEET = 0x00000040,

            /// <summary>
            /// The specified items are drop targets.
            /// </summary>
            SFGAO_DROPTARGET = 0x00000100,

            /// <summary>
            /// This flag is a mask for the capability flags.
            /// </summary>
            SFGAO_CAPABILITYMASK = 0x00000177,

            /// <summary>
            /// Windows 7 and later. The specified items are system items.
            /// </summary>
            SFGAO_SYSTEM = 0x00001000,

            /// <summary>
            /// The specified items are encrypted.
            /// </summary>
            SFGAO_ENCRYPTED = 0x00002000,

            /// <summary>
            /// Indicates that accessing the object = through IStream or other storage interfaces, 
            /// is a slow operation. 
            /// Applications should avoid accessing items flagged with SFGAO_ISSLOW.
            /// </summary>
            SFGAO_ISSLOW = 0x00004000,

            /// <summary>
            /// The specified items are ghosted icons.
            /// </summary>
            SFGAO_GHOSTED = 0x00008000,

            /// <summary>
            /// The specified items are shortcuts.
            /// </summary>
            SFGAO_LINK = 0x00010000,

            /// <summary>
            /// The specified folder objects are shared.
            /// </summary>    
            SFGAO_SHARE = 0x00020000,

            /// <summary>
            /// The specified items are read-only. In the case of folders, this means 
            /// that new items cannot be created in those folders.
            /// </summary>
            SFGAO_READONLY = 0x00040000,

            /// <summary>
            /// The item is hidden and should not be displayed unless the 
            /// Show hidden files and folders option is enabled in Folder Settings.
            /// </summary>
            SFGAO_HIDDEN = 0x00080000,

            /// <summary>
            /// This flag is a mask for the display attributes.
            /// </summary>
            SFGAO_DISPLAYATTRMASK = 0x000FC000,

            /// <summary>
            /// The specified folders contain one or more file system folders.
            /// </summary>
            SFGAO_FILESYSANCESTOR = 0x10000000,

            /// <summary>
            /// The specified items are folders.
            /// </summary>
            SFGAO_FOLDER = 0x20000000,

            /// <summary>
            /// The specified folders or file objects are part of the file system 
            /// that is, they are files, directories, or root directories).
            /// </summary>
            SFGAO_FILESYSTEM = 0x40000000,

            /// <summary>
            /// The specified folders have subfolders = and are, therefore, 
            /// expandable in the left pane of Windows Explorer).
            /// </summary>
            SFGAO_HASSUBFOLDER = 0x80000000,

            /// <summary>
            /// This flag is a mask for the contents attributes.
            /// </summary>
            SFGAO_CONTENTSMASK = 0x80000000,

            /// <summary>
            /// When specified as input, SFGAO_VALIDATE instructs the folder to validate that the items 
            /// pointed to by the contents of apidl exist. If one or more of those items do not exist, 
            /// IShellFolder::GetAttributesOf returns a failure code. 
            /// When used with the file system folder, SFGAO_VALIDATE instructs the folder to discard cached 
            /// properties retrieved by clients of IShellFolder2::GetDetailsEx that may 
            /// have accumulated for the specified items.
            /// </summary>
            SFGAO_VALIDATE = 0x01000000,

            /// <summary>
            /// The specified items are on removable media or are themselves removable devices.
            /// </summary>
            SFGAO_REMOVABLE = 0x02000000,

            /// <summary>
            /// The specified items are compressed.
            /// </summary>
            SFGAO_COMPRESSED = 0x04000000,

            /// <summary>
            /// The specified items can be browsed in place.
            /// </summary>
            SFGAO_BROWSABLE = 0x08000000,

            /// <summary>
            /// The items are nonenumerated items.
            /// </summary>
            SFGAO_NONENUMERATED = 0x00100000,

            /// <summary>
            /// The objects contain new content.
            /// </summary>
            SFGAO_NEWCONTENT = 0x00200000,

            /// <summary>
            /// It is possible to create monikers for the specified file objects or folders.
            /// </summary>
            SFGAO_CANMONIKER = 0x00400000,

            /// <summary>
            /// Not supported.
            /// </summary>
            SFGAO_HASSTORAGE = 0x00400000,

            /// <summary>
            /// Indicates that the item has a stream associated with it that can be accessed 
            /// by a call to IShellFolder::BindToObject with IID_IStream in the riid parameter.
            /// </summary>
            SFGAO_STREAM = 0x00400000,

            /// <summary>
            /// Children of this item are accessible through IStream or IStorage. 
            /// Those children are flagged with SFGAO_STORAGE or SFGAO_STREAM.
            /// </summary>
            SFGAO_STORAGEANCESTOR = 0x00800000,

            /// <summary>
            /// This flag is a mask for the storage capability attributes.
            /// </summary>
            SFGAO_STORAGECAPMASK = 0x70C50008,

            /// <summary>
            /// Mask used by PKEY_SFGAOFlags to remove certain values that are considered 
            /// to cause slow calculations or lack context. 
            /// Equal to SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER.
            /// </summary>
            SFGAO_PKEYSFGAOMASK = 0x81044000,
        }

        [Flags]
        internal enum SHCONT : ushort
        {
            SHCONTF_CHECKING_FOR_CHILDREN = 0x0010,
            SHCONTF_FOLDERS = 0x0020,
            SHCONTF_NONFOLDERS = 0x0040,
            SHCONTF_INCLUDEHIDDEN = 0x0080,
            SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
            SHCONTF_NETPRINTERSRCH = 0x0200,
            SHCONTF_SHAREABLE = 0x0400,
            SHCONTF_STORAGE = 0x0800,
            SHCONTF_NAVIGATION_ENUM = 0x1000,
            SHCONTF_FASTITEMS = 0x2000,
            SHCONTF_FLATLIST = 0x4000,
            SHCONTF_ENABLE_ASYNC = 0x8000
        }

        #endregion

        #region Shell Structs

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;

            internal COMDLG_FILTERSPEC(string name, string spec)
            {
                pszName = name;
                pszSpec = spec;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WTS_THUMBNAILID
        {
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 16)]
            byte rgbKey;
        }


        #endregion

        #region Shell Helper Methods

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        internal static extern int SHCreateShellItemArrayFromDataObject(
            System.Runtime.InteropServices.ComTypes.IDataObject pdo,
            ref Guid riid,
            [MarshalAs( UnmanagedType.Interface )] out IShellItemArray iShellItemArray );

        [DllImport( "shell32.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            // The following parameter is not used - binding context.
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            // The following parameter is not used - binding context.
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int SHCreateShellItemArrayFromShellItem(IShellItem psi, 
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode,
            SetLastError = true)]
        internal static extern int PathParseIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] ref string pszIconFile);


        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromIDList(
            /*PCIDLIST_ABSOLUTE*/ IntPtr pidl,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 ppv);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHParseDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName,
            IntPtr pbc,
            out IntPtr ppidl,
            SFGAO sfgaoIn,
            out SFGAO psfgaoOut
        );

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetIDListFromObject(IntPtr iUnknown,
            out IntPtr ppidl
        );

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetDesktopFolder(
            [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf
        );

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateShellItem(
            IntPtr pidlParent,
            [In, MarshalAs(UnmanagedType.Interface)] IShellFolder psfParent,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi
        );

        [DllImport( "shell32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        internal static extern uint ILGetSize( IntPtr pidl );

        [DllImport( "shell32.dll", CharSet = CharSet.None )]
        public static extern void ILFree( IntPtr pidl );

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        #endregion

        #region Shell Library Enums

        internal enum LIBRARYFOLDERFILTER
        {
            LFF_FORCEFILESYSTEM = 1,
            LFF_STORAGEITEMS = 2,
            LFF_ALLITEMS = 3
        };

        [Flags]
        internal enum LIBRARYOPTIONFLAGS : uint
        {
            LOF_DEFAULT = 0,
            LOF_PINNEDTONAVPANE = 0x1,
            LOF_MASK_ALL = 0x1
        };

        internal enum DEFAULTSAVEFOLDERTYPE
        {
            DSFT_DETECT = 1,
            DSFT_PRIVATE = (DSFT_DETECT + 1),
            DSFT_PUBLIC = (DSFT_PRIVATE + 1)
        };

        internal enum LIBRARYSAVEFLAGS
        {
            LSF_FAILIFTHERE = 0,
            LSF_OVERRIDEEXISTING = 0x1,
            LSF_MAKEUNIQUENAME = 0x2
        };

        internal enum LIBRARYMANAGEDIALOGOPTIONS
        {
            LMD_DEFAULT = 0,
            LMD_NOUNINDEXABLELOCATIONWARNING = 0x1
        };

        /// <summary>
        /// The STGM constants are flags that indicate 
        /// conditions for creating and deleting the object and access modes 
        /// for the object. 
        /// 
        /// You can combine these flags, but you can only choose one flag 
        /// from each group of related flags. Typically one flag from each 
        /// of the access and sharing groups must be specified for all 
        /// functions and methods which use these constants. 
        /// </summary>
        [Flags]
        internal enum STGM : uint
        {
            /// <summary>
            /// Indicates that, in direct mode, each change to a storage 
            /// or stream element is written as it occurs.
            /// </summary>
            Direct = 0x00000000,

            /// <summary>
            /// Indicates that, in transacted mode, changes are buffered 
            /// and written only if an explicit commit operation is called. 
            /// </summary>
            Transacted = 0x00010000,

            /// <summary>
            /// Provides a faster implementation of a compound file 
            /// in a limited, but frequently used, case. 
            /// </summary>
            Simple = 0x08000000,



            /// <summary>
            /// Indicates that the object is read-only, 
            /// meaning that modifications cannot be made.
            /// </summary>
            Read = 0x00000000,

            /// <summary>
            /// Enables you to save changes to the object, 
            /// but does not permit access to its data. 
            /// </summary>
            Write = 0x00000001,

            /// <summary>
            /// Enables access and modification of object data.
            /// </summary>
            ReadWrite = 0x00000002,

            /// <summary>
            /// Specifies that subsequent openings of the object are 
            /// not denied read or write access. 
            /// </summary>
            ShareDenyNone = 0x00000040,

            /// <summary>
            /// Prevents others from subsequently opening the object in Read mode. 
            /// </summary>
            ShareDenyRead = 0x00000030,

            /// <summary>
            /// Prevents others from subsequently opening the object 
            /// for Write or ReadWrite access.
            /// </summary>
            ShareDenyWrite = 0x00000020,

            /// <summary>
            /// Prevents others from subsequently opening the object in any mode. 
            /// </summary>
            ShareExclusive = 0x00000010,

            /// <summary>
            /// Opens the storage object with exclusive access to the most 
            /// recently committed version.
            /// </summary>
            Priority = 0x00040000,

            /// <summary>
            /// Indicates that the underlying file is to be automatically destroyed when the root 
            /// storage object is released. This feature is most useful for creating temporary files. 
            /// </summary>
            DeleteOnRelease = 0x04000000,

            /// <summary>
            /// Indicates that, in transacted mode, a temporary scratch file is usually used 
            /// to save modifications until the Commit method is called. 
            /// Specifying NoScratch permits the unused portion of the original file 
            /// to be used as work space instead of creating a new file for that purpose. 
            /// </summary>
            NoScratch = 0x00100000,

            /// <summary>
            /// Indicates that an existing storage object 
            /// or stream should be removed before the new object replaces it. 
            /// </summary>
            Create = 0x00001000,

            /// <summary>
            /// Creates the new object while preserving existing data in a stream named "Contents". 
            /// </summary>
            Convert = 0x00020000,

            /// <summary>
            /// Causes the create operation to fail if an existing object with the specified name exists.
            /// </summary>
            FailIfThere = 0x00000000,

            /// <summary>
            /// This flag is used when opening a storage object with Transacted 
            /// and without ShareExclusive or ShareDenyWrite. 
            /// In this case, specifying NoSnapshot prevents the system-provided 
            /// implementation from creating a snapshot copy of the file. 
            /// Instead, changes to the file are written to the end of the file. 
            /// </summary>
            NoSnapshot = 0x00200000,

            /// <summary>
            /// Supports direct mode for single-writer, multireader file operations. 
            /// </summary>
            DirectSwmr = 0x00400000
        };
        #endregion

        #region Shell Library Helper Methods

        [DllImport("Shell32", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern int SHShowManageLibraryUI(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem library,
            [In] IntPtr hwndOwner,
            [In] string title,
            [In] string instruction,
            [In] LIBRARYMANAGEDIALOGOPTIONS lmdOptions);

        #endregion

        #region Command Link Definitions

        internal const int BS_COMMANDLINK = 0x0000000E;
        internal const uint BCM_SETNOTE = 0x00001609;
        internal const uint BCM_GETNOTE = 0x0000160A;
        internal const uint BCM_GETNOTELENGTH = 0x0000160B;
        internal const uint BCM_SETSHIELD = 0x0000160C;

        #endregion

    }
}
