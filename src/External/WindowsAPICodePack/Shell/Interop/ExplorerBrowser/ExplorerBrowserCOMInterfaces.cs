//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Controls
{
    internal enum SVGIO : uint
    {
        SVGIO_BACKGROUND = 0x00000000,
        SVGIO_SELECTION = 0x00000001,
        SVGIO_ALLVIEW = 0x00000002,
        SVGIO_CHECKED = 0x00000003,
        SVGIO_TYPE_MASK = 0x0000000F,
        SVGIO_FLAG_VIEWORDER = 0x80000000
    }
    
    internal enum FOLDERFLAGS : uint
    {
        FWF_AUTOARRANGE = 0x00000001,
        FWF_ABBREVIATEDNAMES = 0x00000002,
        FWF_SNAPTOGRID = 0x00000004,
        FWF_OWNERDATA = 0x00000008,
        FWF_BESTFITWINDOW = 0x00000010,
        FWF_DESKTOP = 0x00000020,
        FWF_SINGLESEL = 0x00000040,
        FWF_NOSUBFOLDERS = 0x00000080,
        FWF_TRANSPARENT = 0x00000100,
        FWF_NOCLIENTEDGE = 0x00000200,
        FWF_NOSCROLL = 0x00000400,
        FWF_ALIGNLEFT = 0x00000800,
        FWF_NOICONS = 0x00001000,
        FWF_SHOWSELALWAYS = 0x00002000,
        FWF_NOVISIBLE = 0x00004000,
        FWF_SINGLECLICKACTIVATE = 0x00008000,
        FWF_NOWEBVIEW = 0x00010000,
        FWF_HIDEFILENAMES = 0x00020000,
        FWF_CHECKSELECT = 0x00040000,
        FWF_NOENUMREFRESH = 0x00080000,
        FWF_NOGROUPING = 0x00100000,
        FWF_FULLROWSELECT = 0x00200000,
        FWF_NOFILTERS = 0x00400000,
        FWF_NOCOLUMNHEADER = 0x00800000,
        FWF_NOHEADERINALLVIEWS = 0x01000000,
        FWF_EXTENDEDTILES = 0x02000000,
        FWF_TRICHECKSELECT = 0x04000000,
        FWF_AUTOCHECKSELECT = 0x08000000,
        FWF_NOBROWSERVIEWSTATE = 0x10000000,
        FWF_SUBSETGROUPS = 0x20000000,
        FWF_USESEARCHFOLDER = 0x40000000,
        FWF_ALLOWRTLREADING = 0x80000000
    }

    internal enum FOLDERVIEWMODE
    {
        FVM_AUTO = -1,
        FVM_FIRST = 1,
        FVM_ICON = 1,
        FVM_SMALLICON = 2,
        FVM_LIST = 3,
        FVM_DETAILS = 4,
        FVM_THUMBNAIL = 5,
        FVM_TILE = 6,
        FVM_THUMBSTRIP = 7,
        FVM_CONTENT = 8,
        FVM_LAST = 8
    }

    internal enum EXPLORERPANESTATE
    {
        EPS_DONTCARE	 = 0x00000000,
	    EPS_DEFAULT_ON	 = 0x00000001,
	    EPS_DEFAULT_OFF	 = 0x00000002,
	    EPS_STATEMASK	 = 0x0000ffff,
	    EPS_INITIALSTATE = 0x00010000,
	    EPS_FORCE	     = 0x00020000
    }

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    internal class FOLDERSETTINGS
    {
        public FOLDERVIEWMODE ViewMode;
        public FOLDERFLAGS fFlags;
    }

    internal enum EXPLORER_BROWSER_FILL_FLAGS
    {
        EBF_NODROPTARGET = 0x200,
        EBF_NONE = 0,
        EBF_SELECTFROMDATAOBJECT = 0x100
    }

    internal enum EXPLORER_BROWSER_OPTIONS
    {
        EBO_NAVIGATEONCE = 0x00000001,
        EBO_SHOWFRAMES = 0x00000002,
        EBO_ALWAYSNAVIGATE = 0x00000004,
        EBO_NOTRAVELLOG = 0x00000008,
        EBO_NOWRAPPERWINDOW = 0x00000010,
        EBO_HTMLSHAREPOINTVIEW = 0x00000020
    }

    internal enum CommDlgBrowserStateChange : uint
    {
        CDBOSC_SETFOCUS = 0x00000000,
        CDBOSC_KILLFOCUS = 0x00000001,
        CDBOSC_SELCHANGE = 0x00000002,
        CDBOSC_RENAME = 0x00000003,
        CDBOSC_STATECHANGE = 0x00000004
    }

    internal enum CommDlgBrowserNotifyType : uint
    {
        CDB2N_CONTEXTMENU_DONE = 0x00000001,
        CDB2N_CONTEXTMENU_START = 0x00000002
    }

    internal enum CommDlgBrowser2ViewFlags : uint
    {
        CDB2GVF_SHOWALLFILES = 0x00000001,
        CDB2GVF_ISFILESAVE = 0x00000002,
        CDB2GVF_ALLOWPREVIEWPANE = 0x00000004,
        CDB2GVF_NOSELECTVERB = 0x00000008,
        CDB2GVF_NOINCLUDEITEM = 0x00000010,
        CDB2GVF_ISFOLDERPICKER = 0x00000020
    }

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 108


    [ComImport,
     TypeLibType( TypeLibTypeFlags.FCanCreate ),
     ClassInterface( ClassInterfaceType.None ),
     Guid( ExplorerBrowserCLSIDGuid.ExplorerBrowser )]
    internal class ExplorerBrowserClass : IExplorerBrowser
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void Initialize( IntPtr hwndParent, [In]ref CoreNativeMethods.RECT prc, [In] FOLDERSETTINGS pfs );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void Destroy( );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void SetRect( [In, Out] ref IntPtr phdwp, CoreNativeMethods.RECT rcBrowser );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void SetPropertyBag( [MarshalAs( UnmanagedType.LPWStr )] string pszPropertyBag );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void SetEmptyText( [MarshalAs( UnmanagedType.LPWStr )] string pszEmptyText );
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern HRESULT SetFolderSettings( FOLDERSETTINGS pfs );
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern HRESULT Advise( IntPtr psbe, out uint pdwCookie );
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern HRESULT Unadvise( uint dwCookie );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void SetOptions( [In]EXPLORER_BROWSER_OPTIONS dwFlag );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void GetOptions( out EXPLORER_BROWSER_OPTIONS pdwFlag );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void BrowseToIDList( IntPtr pidl, uint uFlags );
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern HRESULT BrowseToObject( [MarshalAs( UnmanagedType.IUnknown )] object punk, uint uFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void FillFromObject( [MarshalAs( UnmanagedType.IUnknown )] object punk, int dwFlags );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern void RemoveAll( );
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        public virtual extern HRESULT GetCurrentView( ref Guid riid, out IntPtr ppv );
    }


    [ComImport,
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),
     Guid( ExplorerBrowserIIDGuid.IExplorerBrowser )]
    internal interface IExplorerBrowser
    {
        /// <summary>
        /// Prepares the browser to be navigated.
        /// </summary>
        /// <param name="hwndParent">A handle to the owner window or control.</param>
        /// <param name="prc">A pointer to a RECT containing the coordinates of the bounding rectangle 
        /// the browser will occupy. The coordinates are relative to hwndParent. If this parameter is NULL,
        /// then method IExplorerBrowser::SetRect should subsequently be called.</param>
        /// <param name="pfs">A pointer to a FOLDERSETTINGS structure that determines how the folder will be
        /// displayed in the view. If this parameter is NULL, then method IExplorerBrowser::SetFolderSettings
        /// should be called, otherwise, the default view settings for the folder are used.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Initialize( IntPtr hwndParent, [In] ref CoreNativeMethods.RECT prc, [In] FOLDERSETTINGS pfs );

        /// <summary>
        /// Destroys the browser.
        /// </summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Destroy( );

        /// <summary>
        /// Sets the size and position of the view windows created by the browser.
        /// </summary>
        /// <param name="phdwp">A pointer to a DeferWindowPos handle. This paramater can be NULL.</param>
        /// <param name="rcBrowser">The coordinates that the browser will occupy.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetRect( [In, Out] ref IntPtr phdwp, CoreNativeMethods.RECT rcBrowser );

        /// <summary>
        /// Sets the name of the property bag.
        /// </summary>
        /// <param name="pszPropertyBag">A pointer to a constant, null-terminated, Unicode string that contains
        /// the name of the property bag. View state information that is specific to the application of the 
        /// client is stored (persisted) using this name.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetPropertyBag( [MarshalAs( UnmanagedType.LPWStr )] string pszPropertyBag );

        /// <summary>
        /// Sets the default empty text.
        /// </summary>
        /// <param name="pszEmptyText">A pointer to a constant, null-terminated, Unicode string that contains 
        /// the empty text.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetEmptyText( [MarshalAs( UnmanagedType.LPWStr )] string pszEmptyText );

        /// <summary>
        /// Sets the folder settings for the current view.
        /// </summary>
        /// <param name="pfs">A pointer to a FOLDERSETTINGS structure that contains the folder settings 
        /// to be applied.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT SetFolderSettings( FOLDERSETTINGS pfs );

        /// <summary>
        /// Initiates a connection with IExplorerBrowser for event callbacks.
        /// </summary>
        /// <param name="psbe">A pointer to the IExplorerBrowserEvents interface of the object to be 
        /// advised of IExplorerBrowser events</param>
        /// <param name="pdwCookie">When this method returns, contains a token that uniquely identifies 
        /// the event listener. This allows several event listeners to be subscribed at a time.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT Advise( IntPtr psbe, out uint pdwCookie );

        /// <summary>
        /// Terminates an advisory connection.
        /// </summary>
        /// <param name="dwCookie">A connection token previously returned from IExplorerBrowser::Advise.
        /// Identifies the connection to be terminated.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT Unadvise( [In] uint dwCookie );

        /// <summary>
        /// Sets the current browser options.
        /// </summary>
        /// <param name="dwFlag">One or more EXPLORER_BROWSER_OPTIONS flags to be set.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetOptions( [In]EXPLORER_BROWSER_OPTIONS dwFlag );

        /// <summary>
        /// Gets the current browser options.
        /// </summary>
        /// <param name="pdwFlag">When this method returns, contains the current EXPLORER_BROWSER_OPTIONS 
        /// for the browser.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetOptions( out EXPLORER_BROWSER_OPTIONS pdwFlag );

        /// <summary>
        /// Browses to a pointer to an item identifier list (PIDL)
        /// </summary>
        /// <param name="pidl">A pointer to a const ITEMIDLIST (item identifier list) that specifies an object's 
        /// location as the destination to navigate to. This parameter can be NULL.</param>
        /// <param name="uFlags">A flag that specifies the category of the pidl. This affects how 
        /// navigation is accomplished</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void BrowseToIDList( IntPtr pidl, uint uFlags );

        /// <summary>
        /// Browse to an object
        /// </summary>
        /// <param name="punk">A pointer to an object to browse to. If the object cannot be browsed, 
        /// an error value is returned.</param>
        /// <param name="uFlags">A flag that specifies the category of the pidl. This affects how 
        /// navigation is accomplished. </param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT BrowseToObject( [MarshalAs( UnmanagedType.IUnknown )] object punk, uint uFlags );

        /// <summary>
        /// Creates a results folder and fills it with items.
        /// </summary>
        /// <param name="punk">An interface pointer on the source object that will fill the IResultsFolder</param>
        /// <param name="dwFlags">One of the EXPLORER_BROWSER_FILL_FLAGS</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void FillFromObject( [MarshalAs( UnmanagedType.IUnknown )] object punk, int dwFlags );

        /// <summary>
        /// Removes all items from the results folder.
        /// </summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void RemoveAll( );

        /// <summary>
        /// Gets an interface for the current view of the browser.
        /// </summary>
        /// <param name="riid">A reference to the desired interface ID.</param>
        /// <param name="ppv">When this method returns, contains the interface pointer requested in riid. 
        /// This will typically be IShellView or IShellView2. </param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetCurrentView( ref Guid riid, out IntPtr ppv );
    }

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IServiceProvider ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IServiceProvider
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall )]
        HRESULT QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject );
    };

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IFolderView ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IFolderView
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetCurrentViewMode( [Out] out uint pViewMode );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetCurrentViewMode( uint ViewMode );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFolder( ref Guid riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Item( int iItemIndex, out IntPtr ppidl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void ItemCount( uint uFlags, out int pcItems );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Items( uint uFlags, ref Guid riid, [Out, MarshalAs( UnmanagedType.IUnknown )] out object ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectionMarkedItem( out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFocusedItem( out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetItemPosition( IntPtr pidl, out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSpacing( [Out] out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetDefaultSpacing( out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetAutoArrange( );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SelectItem( int iItem, uint dwFlags );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SelectAndPositionItems( uint cidl, IntPtr apidl, ref CoreNativeMethods.POINT apt, uint dwFlags );
    }

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IFolderView2 ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IFolderView2 : IFolderView
    {
        // IFolderView
        [PreserveSig]        
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetCurrentViewMode( out uint pViewMode );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetCurrentViewMode( uint ViewMode );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFolder( ref Guid riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Item( int iItemIndex, out IntPtr ppidl );

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT ItemCount( uint uFlags, out int pcItems );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT Items( uint uFlags, ref Guid riid, [Out, MarshalAs( UnmanagedType.IUnknown )] out object ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectionMarkedItem( out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFocusedItem( out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetItemPosition( IntPtr pidl, out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSpacing( [Out] out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetDefaultSpacing( out CoreNativeMethods.POINT ppt );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetAutoArrange( );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SelectItem( int iItem, uint dwFlags );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SelectAndPositionItems( uint cidl, IntPtr apidl, ref CoreNativeMethods.POINT apt, uint dwFlags );
        
        // IFolderView2
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetGroupBy( IntPtr key, bool fAscending );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetGroupBy( ref IntPtr pkey, ref bool pfAscending );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetViewProperty( IntPtr pidl, IntPtr propkey, object propvar );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetViewProperty( IntPtr pidl, IntPtr propkey, out object ppropvar );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetTileViewProperties( IntPtr pidl, [MarshalAs( UnmanagedType.LPWStr )] string pszPropList );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetExtendedTileViewProperties( IntPtr pidl, [MarshalAs( UnmanagedType.LPWStr )] string pszPropList );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetText( int iType, [MarshalAs( UnmanagedType.LPWStr )] string pwszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetCurrentFolderFlags( uint dwMask, uint dwFlags );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetCurrentFolderFlags( out uint pdwFlags );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSortColumnCount( out int pcColumns );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetSortColumns( IntPtr rgSortColumns, int cColumns );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSortColumns( out IntPtr rgSortColumns, int cColumns );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetItem( int iItem, ref Guid riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetVisibleItem( int iStart, bool fPrevious, out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectedItem( int iStart, out int piItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelection( bool fNoneImpliesFolder, out IShellItemArray ppsia );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectionState( IntPtr pidl, out uint pdwFlags );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void InvokeVerbOnSelection( [In, MarshalAs( UnmanagedType.LPWStr )] string pszVerb );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT SetViewModeAndIconSize( int uViewMode, int iImageSize );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetViewModeAndIconSize( out int puViewMode, out int piImageSize );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetGroupSubsetCount( uint cVisibleRows );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetGroupSubsetCount( out uint pcVisibleRows );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetRedraw( bool fRedrawOn );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void IsMoveInSameFolder( );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void DoRename( );
    }

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IExplorerPaneVisibility ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IExplorerPaneVisibility
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetPaneState( ref Guid explorerPane, out EXPLORERPANESTATE peps);
    };

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IExplorerBrowserEvents ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IExplorerBrowserEvents
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnNavigationPending( IntPtr pidlFolder );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnViewCreated( [MarshalAs(UnmanagedType.IUnknown)]  object psv );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnNavigationComplete( IntPtr pidlFolder);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnNavigationFailed( IntPtr pidlFolder);
    }

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.ICommDlgBrowser ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface ICommDlgBrowser
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnDefaultCommand( IntPtr ppshv );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT OnStateChange(
            IntPtr ppshv,
            CommDlgBrowserStateChange uChange );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT IncludeObject(
            IntPtr ppshv,
            IntPtr pidl );
    }


    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IInputObject ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IInputObject
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT UIActivateIO( bool fActivate, ref System.Windows.Forms.Message pMsg );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT HasFocusIO( );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT TranslateAcceleratorIO( ref System.Windows.Forms.Message pMsg );
        
    };

    [ComImport,
     Guid( ExplorerBrowserIIDGuid.IShellView ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IShellView
    {
        // IOleWindow
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetWindow( 
            out IntPtr phwnd);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT ContextSensitiveHelp( 
            bool fEnterMode);

        // IShellView
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT TranslateAccelerator( 
            IntPtr pmsg);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT EnableModeless( 
            bool fEnable);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT UIActivate( 
            uint uState);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT Refresh( );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT CreateViewWindow( 
            [MarshalAs( UnmanagedType.IUnknown )] object psvPrevious,
            IntPtr pfs,
            [MarshalAs( UnmanagedType.IUnknown )] object psb,
            IntPtr prcView,
            out IntPtr phWnd);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT DestroyViewWindow( );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetCurrentInfo( 
            out IntPtr pfs);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT AddPropertySheetPages( 
            uint dwReserved,
            IntPtr pfn,
            uint lparam);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT SaveViewState( );
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT SelectItem( 
            IntPtr pidlItem,
            uint uFlags);
        
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        HRESULT GetItemObject(
            SVGIO uItem,
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv );
    }

#pragma warning restore 108

}
