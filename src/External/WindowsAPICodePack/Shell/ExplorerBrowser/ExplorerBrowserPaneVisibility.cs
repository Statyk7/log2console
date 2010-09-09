//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Microsoft.WindowsAPICodePack.Controls
{
    /// <summary>
    /// Controls the visibility of the various ExplorerBrowser panes on subsequent navigation
    /// </summary>
    public class ExplorerBrowserPaneVisibility
    {
        /// <summary>
        /// The pane on the left side of the Windows Explorer window that hosts the folders tree and Favorites.
        /// </summary>
        public PaneVisibilityState Navigation
        {
            get
            {
                return _Navigation;
            }
            set
            {
                _Navigation = value;
            }
        }
        private PaneVisibilityState _Navigation = PaneVisibilityState.DontCare;

        /// <summary>
        /// Commands module along the top of the Windows Explorer window.
        /// </summary>
        public PaneVisibilityState Commands
        {
            get
            {
                return _Commands;
            }
            set
            {
                _Commands = value;
            }
        }
        private PaneVisibilityState _Commands = PaneVisibilityState.DontCare;

        /// <summary>
        /// Organize menu within the commands module.
        /// </summary>
        public PaneVisibilityState CommandsOrganize
        {
            get
            {
                return _CommandsOrganize;
            }
            set
            {
                _CommandsOrganize = value;
            }
        }
        private PaneVisibilityState _CommandsOrganize = PaneVisibilityState.DontCare;


        /// <summary>
        /// View menu within the commands module.
        /// </summary>
        public PaneVisibilityState CommandsView
        {
            get
            {
                return _CommandsView;
            }
            set
            {
                _CommandsView = value;
            }
        }
        private PaneVisibilityState _CommandsView = PaneVisibilityState.DontCare;


        /// <summary>
        /// Pane showing metadata along the bottom of the Windows Explorer window.
        /// </summary>
        public PaneVisibilityState Details
        {
            get
            {
                return _Details;
            }
            set
            {
                _Details = value;
            }
        }
        private PaneVisibilityState _Details = PaneVisibilityState.DontCare;


        /// <summary>
        /// Pane on the right of the Windows Explorer window that shows a large reading preview of the file.
        /// </summary>
        public PaneVisibilityState Preview
        {
            get
            {
                return _Preview;
            }
            set
            {
                _Preview = value;
            }
        }
        private PaneVisibilityState _Preview = PaneVisibilityState.DontCare;


        /// <summary>
        /// Quick filter buttons to aid in a search.
        /// </summary>
        public PaneVisibilityState Query
        {
            get
            {
                return _Query;
            }
            set
            {
                _Query = value;
            }
        }
        private PaneVisibilityState _Query = PaneVisibilityState.DontCare;


        /// <summary>
        /// Additional fields and options to aid in a search.
        /// </summary>
        public PaneVisibilityState AdvancedQuery
        {
            get
            {
                return _AdvancedQuery;
            }
            set
            {
                _AdvancedQuery = value;
            }
        }
        private PaneVisibilityState _AdvancedQuery = PaneVisibilityState.DontCare;

    }
}
