//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Controls.WindowsForms;

namespace Microsoft.WindowsAPICodePack.Controls
{
    /// <summary>
    /// These options control how the content of the Explorer Browser 
    /// is rendered.
    /// </summary>
    public class ExplorerBrowserContentOptions
    {
        #region construction
        ExplorerBrowser eb;
        internal ExplorerBrowserContentOptions( ExplorerBrowser eb )
        {
            this.eb = eb;
        }
        #endregion

        #region ViewMode property
        // This is a one-way property of the explorer browser. 
        // Keeping it around for the get implementations.
        internal FOLDERSETTINGS folderSettings = new FOLDERSETTINGS( );

        /// <summary>
        /// The viewing mode of the Explorer Browser
        /// </summary>
        public ExplorerBrowserViewMode ViewMode
        {
            get
            {
                return (ExplorerBrowserViewMode)folderSettings.ViewMode;
            }
            set
            {
                folderSettings.ViewMode = (FOLDERVIEWMODE)value;
                HRESULT hr = HRESULT.S_OK;
                if( eb.explorerBrowserControl != null )
                    hr = eb.explorerBrowserControl.SetFolderSettings( folderSettings );
            }
        }
        #endregion

        #region Flags property
        /// <summary>
        /// The binary representation of the ExplorerBrowser content flags
        /// </summary>
        public ExplorerBrowserContentFlags Flags
        {
            get
            {
                return (ExplorerBrowserContentFlags)folderSettings.fFlags;
            }
            set
            {
                folderSettings.fFlags = (FOLDERFLAGS)value | FOLDERFLAGS.FWF_USESEARCHFOLDER | FOLDERFLAGS.FWF_NOWEBVIEW;
                if( eb.explorerBrowserControl != null )
                    eb.explorerBrowserControl.SetFolderSettings( folderSettings );
            }
        }
        #endregion

        #region content flags to properties mapping
        /// <summary>
        /// The view should be left-aligned. 
        /// </summary>
        public bool AlignLeft
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.AlignLeft );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.AlignLeft, value );
            }
        }
        /// <summary>
        /// Automatically arrange the elements in the view. 
        /// </summary>
        public bool AutoArrange
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.AutoArrange );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.AutoArrange, value );
            }
        }
        /// <summary>
        /// Turns on check mode for the view
        /// </summary>
        public bool CheckSelect
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.CheckSelect );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.CheckSelect, value );
            }
        }
        /// <summary>
        /// When the view is in "tile view mode" the layout of a single item should be extended to the width of the view.
        /// </summary>
        public bool ExtendedTiles
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.ExtendedTiles );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.ExtendedTiles, value );
            }
        }
        /// <summary>
        /// When an item is selected, the item and all its sub-items are highlighted.
        /// </summary>
        public bool FullRowSelect
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.FullRowSelect );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.FullRowSelect, value );
            }
        }
        /// <summary>
        /// The view should not display file names
        /// </summary>
        public bool HideFileNames
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.HideFileNames );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.HideFileNames, value );
            }
        }
        /// <summary>
        /// The view should not save view state in the browser.
        /// </summary>
        public bool NoBrowserViewState
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.NoBrowserViewState );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.NoBrowserViewState, value );
            }
        }
        /// <summary>
        /// Do not display a column header in the view in any view mode.
        /// </summary>
        public bool NoColumnHeader
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.NoColumnHeader );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.NoColumnHeader, value );
            }
        }
        /// <summary>
        /// Only show the column header in details view mode.
        /// </summary>
        public bool NoHeaderInAllViews
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.NoHeaderInAllViews );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.NoHeaderInAllViews, value );
            }
        }
        /// <summary>
        /// The view should not display icons. 
        /// </summary>
        public bool NoIcons
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.NoIcons );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.NoIcons, value );
            }
        }
        /// <summary>
        /// Do not show subfolders. 
        /// </summary>
        public bool NoSubfolders
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.NoSubfolders );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.NoSubfolders, value );
            }
        }
        /// <summary>
        /// Navigate with a single click
        /// </summary>
        public bool SingleClickActivate
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.SingleClickActivate );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.SingleClickActivate, value );
            }
        }
        /// <summary>
        /// Do not allow more than a single item to be selected.
        /// </summary>
        public bool SingleSelection
        {
            get
            {
                return IsFlagSet( ExplorerBrowserContentFlags.SingleSelection );
            }
            set
            {
                SetFlag( ExplorerBrowserContentFlags.SingleSelection, value );
            }
        }

        private bool IsFlagSet( ExplorerBrowserContentFlags flag )
        {
            return ( folderSettings.fFlags & (FOLDERFLAGS)flag ) != 0;
        }

        private void SetFlag( ExplorerBrowserContentFlags flag, bool value )
        {
            if( value )
                folderSettings.fFlags |= (FOLDERFLAGS)flag;
            else
                folderSettings.fFlags = folderSettings.fFlags & ~(FOLDERFLAGS)flag;

            if( eb.explorerBrowserControl != null )
                eb.explorerBrowserControl.SetFolderSettings( folderSettings );
        }

        #endregion

        #region thumbnail size
        /// <summary>
        /// The size of the thumbnails in pixels
        /// </summary>
        public int ThumbnailSize
        {
            get
            {
                int iconSize = 0;
                IFolderView2 iFV2 = eb.GetFolderView2( );
                if( iFV2 != null )
                {
                    try
                    {
                        int fvm = 0;
                        HRESULT hr = iFV2.GetViewModeAndIconSize( out fvm, out iconSize );
                        if( hr != HRESULT.S_OK )
                            throw new COMException("unable to get icon size", (int)hr );
                    }
                    finally
                    {
                        Marshal.ReleaseComObject( iFV2 );
                        iFV2 = null;
                    }
                }

                return iconSize;
            }
            set
            {
                IFolderView2 iFV2 = eb.GetFolderView2( );
                if( iFV2 != null )
                {
                    try
                    {
                        int fvm = 0;
                        int iconSize = 0;
                        HRESULT hr = iFV2.GetViewModeAndIconSize( out fvm, out iconSize );
                        if( hr != HRESULT.S_OK )
                            throw new COMException( "unable to get icon size", (int)hr );
                        hr = iFV2.SetViewModeAndIconSize( fvm, value );
                        if( hr != HRESULT.S_OK )
                            throw new COMException( "unable to set icon size", (int)hr );
                    }
                    finally
                    {
                        Marshal.ReleaseComObject( iFV2 );
                        iFV2 = null;
                    }
                }
            }
        }
        #endregion
    }

}
