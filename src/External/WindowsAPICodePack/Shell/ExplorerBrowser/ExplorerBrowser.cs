//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using MS.WindowsAPICodePack.Internal;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Controls.WindowsForms
{
    /// <summary>
    /// This class is a wrapper around the Windows Explorer Browser control.
    /// </summary>
    public class ExplorerBrowser :
        System.Windows.Forms.UserControl,
        Microsoft.WindowsAPICodePack.Controls.IServiceProvider,
        IExplorerPaneVisibility,
        IExplorerBrowserEvents,
        ICommDlgBrowser,
        IMessageFilter
    {
        #region properties
        /// <summary>
        /// Options that control how the ExplorerBrowser navigates
        /// </summary>
        public ExplorerBrowserNavigationOptions NavigationOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Options that control how the content of the ExplorerBorwser looks
        /// </summary>
        public ExplorerBrowserContentOptions ContentOptions
        {
            get;
            private set;
        }

        private IShellItemArray shellItemsArray;
        private ShellObjectCollection itemsCollection;
        /// <summary>
        /// The set of ShellObjects in the Explorer Browser
        /// </summary>
        public ShellObjectCollection Items
        {
            get
            {
                if (shellItemsArray != null)
                    Marshal.ReleaseComObject(shellItemsArray);

                if (itemsCollection != null)
                {
                    itemsCollection.Dispose();
                    itemsCollection = null;
                }

                shellItemsArray = GetItemsArray();
                itemsCollection = new ShellObjectCollection(shellItemsArray, true);

                return itemsCollection;
            }
        }

        private IShellItemArray selectedShellItemsArray;
        private ShellObjectCollection selectedItemsCollection;
        /// <summary>
        /// The set of selected ShellObjects in the Explorer Browser
        /// </summary>
        public ShellObjectCollection SelectedItems
        {
            get
            {
                if (selectedShellItemsArray != null)
                    Marshal.ReleaseComObject(selectedShellItemsArray);

                if (selectedItemsCollection != null)
                {
                    selectedItemsCollection.Dispose();
                    selectedItemsCollection = null;
                }

                selectedShellItemsArray = GetSelectedItemsArray();
                selectedItemsCollection = new ShellObjectCollection(selectedShellItemsArray, true);


                return selectedItemsCollection;
            }
        }

        /// <summary>
        /// Contains the navigation history of the ExplorerBrowser
        /// </summary>
        public ExplorerBrowserNavigationLog NavigationLog
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the property bag used to persist changes to the ExplorerBrowser's view state.
        /// </summary>
        public string PropertyBagName
        {
            get
            {
                return propertyBagName;
            }
            set
            {
                propertyBagName = value;
                if (explorerBrowserControl != null)
                    explorerBrowserControl.SetPropertyBag(propertyBagName);
            }
        }

        #endregion

        #region operations
        /// <summary>
        /// Clears the Explorer Browser of existing content, fills it with
        /// content from the specified container, and adds a new point to the Travel Log.
        /// </summary>
        /// <param name="shellObject">The shell container to navigate to.</param>
        /// <exception cref="System.Runtime.InteropServices.COMException">Will throw if navigation fails for any other reason.</exception>
        public void Navigate(ShellObject shellObject)
        {
            if (explorerBrowserControl == null)
            {
                antecreationNavigationTarget = shellObject;
            }
            else
            {
                HRESULT hr = explorerBrowserControl.BrowseToObject(shellObject.NativeShellItem, 0);
                if (hr != HRESULT.S_OK)
                {
                    if (hr == HRESULT.RESOURCE_IN_USE)
                    {
                        if (NavigationFailed != null)
                        {
                            NavigationFailedEventArgs args = new NavigationFailedEventArgs();
                            args.FailedLocation = shellObject;
                            NavigationFailed(this, args);
                        }
                    }
                    else
                        throw new COMException("BrowseToObject failed", (int)hr);
                }
            }
        }

        /// <summary>
        /// Navigates within the navigation log. This does not change the set of 
        /// locations in the navigation log.
        /// </summary>
        /// <param name="direction">Forward of Backward</param>
        /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
        public bool NavigateLogLocation(NavigationLogDirection direction)
        {
            return NavigationLog.NavigateLog(direction);
        }

        /// <summary>
        /// Navigate within the navigation log. This does not change the set of 
        /// locations in the navigation log.
        /// </summary>
        /// <param name="navigationLogIndex">An index into the navigation logs Locations collection.</param>
        /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
        public bool NavigateLogLocation(int navigationLogIndex)
        {
            return NavigationLog.NavigateLog(navigationLogIndex);
        }
        #endregion

        #region events

        /// <summary>
        /// Fires when the SelectedItems collection changes. 
        /// </summary>
        public event ExplorerBrowserSelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Fires when the Items colection changes. 
        /// </summary>
        public event ExplorerBrowserItemsChangedEventHandler ItemsChanged;

        /// <summary>
        /// Fires when a navigation has been initiated, but is not yet complete.
        /// </summary>
        public event ExplorerBrowserNavigationPendingEventHandler NavigationPending;

        /// <summary>
        /// Fires when a navigation has been 'completed': no NavigationPending listener 
        /// has cancelled, and the ExplorerBorwser has created a new view. The view 
        /// will be populated with new items asynchronously, and ItemsChanged will be 
        /// fired to reflect this some time later.
        /// </summary>
        public event ExplorerBrowserNavigationCompleteEventHandler NavigationComplete;

        /// <summary>
        /// Fires when either a NavigationPending listener cancels the navigation, or
        /// if the operating system determines that navigation is not possible.
        /// </summary>
        public event ExplorerBrowserNavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// Fires when the ExplorerBorwser view has finished enumerating files.
        /// </summary>
        public event ExplorerBrowserViewEnumerationCompleteHandler ViewEnumerationComplete;

        /// <summary>
        /// Fires when the item selected in the view has changed (i.e., a rename ).
        /// This is not the same as SelectionChanged.
        /// </summary>
        public event ExplorerBrowserViewSelectedItemChangedHandler ViewSelectedItemChanged;

        #endregion

        #region implementation

        #region construction
        internal ExplorerBrowserClass explorerBrowserControl = null;

        // for the IExplorerBrowserEvents Advise call
        internal uint eventsCookie = 0;

        // name of the property bag that contains the view state options of the browser
        string propertyBagName = typeof(ExplorerBrowser).FullName;

        /// <summary>
        /// Initializes the ExplorerBorwser WinForms wrapper.
        /// </summary>
        public ExplorerBrowser()
            : base()
        {
            NavigationOptions = new ExplorerBrowserNavigationOptions(this);
            ContentOptions = new ExplorerBrowserContentOptions(this);
            NavigationLog = new ExplorerBrowserNavigationLog(this);
        }

        #endregion

        #region message handlers

        /// <summary>
        /// Displays a placeholder for the explorer browser in design mode
        /// </summary>
        /// <param name="e">Contains information about the paint event.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                LinearGradientBrush linGrBrush = new LinearGradientBrush(
                    ClientRectangle,
                    Color.Aqua,
                    Color.CadetBlue,
                    LinearGradientMode.ForwardDiagonal);


                e.Graphics.FillRectangle(linGrBrush, ClientRectangle);

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(
                    "ExplorerBrowserControl",
                    new Font("Garamond", 30),
                    Brushes.White,
                    ClientRectangle,
                    sf);
            }

            base.OnPaint(e);
        }

        ShellObject antecreationNavigationTarget = null;
        ExplorerBrowserViewEvents viewEvents = null; 

        /// <summary>
        /// Creates and initializes the native ExplorerBrowser control
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            HRESULT hr = HRESULT.S_OK;

            if (this.DesignMode == false)
            {
                explorerBrowserControl = new ExplorerBrowserClass();

                // hooks up IExplorerPaneVisibility and ICommDlgBrowser event notifications
                hr = ExplorerBrowserNativeMethods.IUnknown_SetSite(explorerBrowserControl, this);

                // hooks up IExplorerBrowserEvents event notification
                hr = explorerBrowserControl.Advise(
                    Marshal.GetComInterfaceForObject(this, typeof(IExplorerBrowserEvents)),
                    out eventsCookie);

                // sets up ExplorerBrowser view connection point events
                viewEvents = new ExplorerBrowserViewEvents( this );

                CoreNativeMethods.RECT rect = new CoreNativeMethods.RECT();
                rect.top = ClientRectangle.Top;
                rect.left = ClientRectangle.Left;
                rect.right = ClientRectangle.Right;
                rect.bottom = ClientRectangle.Bottom;

                explorerBrowserControl.Initialize(this.Handle, ref rect, null);

                // Force an initial show frames so that IExplorerPaneVisibility works the first time it is set.
                // This also enables the control panel to be browsed to. If it is not set, then navigating to 
                // the control panel succeeds, but no items are visible in the view.
                explorerBrowserControl.SetOptions(EXPLORER_BROWSER_OPTIONS.EBO_SHOWFRAMES);

                explorerBrowserControl.SetPropertyBag(propertyBagName);

                if (antecreationNavigationTarget != null)
                {
                    BeginInvoke(new MethodInvoker(
                    delegate
                    {
                        Navigate(antecreationNavigationTarget);
                        antecreationNavigationTarget = null;
                    }));
                }
            }

            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// Sizes the native control to match the WinForms control wrapper.
        /// </summary>
        /// <param name="e">Contains information about the size changed event.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (explorerBrowserControl != null)
            {
                CoreNativeMethods.RECT rect = new CoreNativeMethods.RECT();
                rect.top = ClientRectangle.Top;
                rect.left = ClientRectangle.Left;
                rect.right = ClientRectangle.Right;
                rect.bottom = ClientRectangle.Bottom;
                IntPtr ptr = IntPtr.Zero;
                explorerBrowserControl.SetRect(ref ptr, rect);
            }

            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Cleans up the explorer browser events+object when the window is being taken down.
        /// </summary>
        /// <param name="e">An EventArgs that contains event data.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (explorerBrowserControl != null)
            {
                // unhook events
                viewEvents.DisconnectFromView( );
                HRESULT hr = explorerBrowserControl.Unadvise(eventsCookie);
                ExplorerBrowserNativeMethods.IUnknown_SetSite(explorerBrowserControl, null);

                // destroy the explorer browser control
                explorerBrowserControl.Destroy();

                // release com reference to it
                Marshal.ReleaseComObject(explorerBrowserControl);
                explorerBrowserControl = null;
            }

            base.OnHandleDestroyed(e);
        }
        #endregion

        #region object interfaces

        #region IServiceProvider
        HRESULT Microsoft.WindowsAPICodePack.Controls.IServiceProvider.QueryService(
            ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
            HRESULT hr = HRESULT.S_OK;

            if (guidService.CompareTo(new Guid(ExplorerBrowserIIDGuid.IExplorerPaneVisibility)) == 0)
            {
                // Responding to this SID allows us to control the visibility of the 
                // explorer browser panes
                ppvObject =
                    Marshal.GetComInterfaceForObject(this, typeof(IExplorerPaneVisibility));
                hr = HRESULT.S_OK;
            }
            else if (guidService.CompareTo(new Guid(ExplorerBrowserIIDGuid.ICommDlgBrowser)) == 0)
            {
                // Responding to this SID allows us to hook up our ICommDlgBrowser
                // implementation so we get selection change events from the view.
                if (riid.CompareTo(new Guid(ExplorerBrowserIIDGuid.ICommDlgBrowser)) == 0)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(ICommDlgBrowser));
                    hr = HRESULT.S_OK;
                }
                else
                {
                    ppvObject = IntPtr.Zero;
                    hr = HRESULT.E_NOINTERFACE;
                }
            }
            else
            {
                IntPtr nullObj = IntPtr.Zero;
                ppvObject = nullObj;
                hr = HRESULT.E_NOINTERFACE;
            }

            return hr;
        }
        #endregion

        #region IExplorerPaneVisibility
        /// <summary>
        /// Controls the visibility of the explorer borwser panes
        /// </summary>
        /// <param name="explorerPane">a guid identifying the pane</param>
        /// <param name="peps">the pane state desired</param>
        /// <returns></returns>
        HRESULT IExplorerPaneVisibility.GetPaneState(ref Guid explorerPane, out EXPLORERPANESTATE peps)
        {
            switch (explorerPane.ToString())
            {
                case ExplorerBrowserViewPanes.AdvancedQuery:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.AdvancedQuery);
                    break;
                case ExplorerBrowserViewPanes.Commands:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.Commands);
                    break;
                case ExplorerBrowserViewPanes.CommandsOrganize:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.CommandsOrganize);
                    break;
                case ExplorerBrowserViewPanes.CommandsView:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.CommandsView);
                    break;
                case ExplorerBrowserViewPanes.Details:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.Details);
                    break;
                case ExplorerBrowserViewPanes.Navigation:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.Navigation);
                    break;
                case ExplorerBrowserViewPanes.Preview:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.Preview);
                    break;
                case ExplorerBrowserViewPanes.Query:
                    peps = VisibilityToPaneState(NavigationOptions.PaneVisibility.Query);
                    break;
                default:
#if LOG_UNKNOWN_PANES
                    System.Diagnostics.Debugger.Log( 4, "ExplorerBrowser", "unknown pane view state. id=" + explorerPane.ToString( ) );
#endif
                    peps = VisibilityToPaneState(PaneVisibilityState.Show);
                    break;
            }

            return HRESULT.S_OK;
        }

        private EXPLORERPANESTATE VisibilityToPaneState(PaneVisibilityState visibility)
        {
            switch (visibility)
            {
                case PaneVisibilityState.DontCare:
                    return EXPLORERPANESTATE.EPS_DONTCARE;

                case PaneVisibilityState.Hide:
                    return EXPLORERPANESTATE.EPS_DEFAULT_OFF | EXPLORERPANESTATE.EPS_FORCE;

                case PaneVisibilityState.Show:
                    return EXPLORERPANESTATE.EPS_DEFAULT_ON | EXPLORERPANESTATE.EPS_FORCE;

                default:
                    throw new ArgumentException("unexpected PaneVisibilityState");
            }
        }

        #endregion

        #region IExplorerBrowserEvents
        HRESULT IExplorerBrowserEvents.OnNavigationPending(IntPtr pidlFolder)
        {
            bool canceled = false;

            if (NavigationPending != null)
            {
                NavigationPendingEventArgs args = new NavigationPendingEventArgs();

                // For some special items (like network machines), ShellObject.FromIDList
                // might return null
                args.PendingLocation = ShellObjectFactory.Create(pidlFolder);

                if (args.PendingLocation != null)
                {
                    foreach (Delegate del in NavigationPending.GetInvocationList())
                    {
                        del.DynamicInvoke(new object[] { this, args });
                        if (args.Cancel)
                        {
                            canceled = true;
                        }
                    }
                }
            }

            return canceled ? HRESULT.E_FAIL : HRESULT.S_OK;
        }

        HRESULT IExplorerBrowserEvents.OnViewCreated( object psv )
        {
            viewEvents.ConnectToView( (IShellView)psv );

            return HRESULT.S_OK;
        }

        HRESULT IExplorerBrowserEvents.OnNavigationComplete(IntPtr pidlFolder)
        {
            // view mode may change 
            ContentOptions.folderSettings.ViewMode = GetCurrentViewMode();

            if (NavigationComplete != null)
            {
                NavigationCompleteEventArgs args = new NavigationCompleteEventArgs();
                args.NewLocation = ShellObjectFactory.Create(pidlFolder);
                NavigationComplete(this, args);
            }
            return HRESULT.S_OK;
        }

        HRESULT IExplorerBrowserEvents.OnNavigationFailed(IntPtr pidlFolder)
        {
            if (NavigationFailed != null)
            {
                NavigationFailedEventArgs args = new NavigationFailedEventArgs();
                args.FailedLocation = ShellObjectFactory.Create(pidlFolder);
                NavigationFailed(this, args);
            }
            return HRESULT.S_OK;
        }
        #endregion

        #region ICommDlgBrowser
        HRESULT ICommDlgBrowser.OnDefaultCommand(IntPtr ppshv)
        {
            return HRESULT.S_FALSE;
        }

        HRESULT ICommDlgBrowser.OnStateChange(IntPtr ppshv, CommDlgBrowserStateChange uChange)
        {
            if( uChange == CommDlgBrowserStateChange.CDBOSC_SELCHANGE )
                FireSelectionChanged( );

            return HRESULT.S_OK;
        }

        HRESULT ICommDlgBrowser.IncludeObject(IntPtr ppshv, IntPtr pidl)
        {
            // items in the view have changed, so the collections need updating
            FireContentChanged( );

            return HRESULT.S_OK;
        }

        #endregion

        #region IMessageFilter Members

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            HRESULT hr = HRESULT.S_FALSE;
            if (explorerBrowserControl != null)
            {
                // translate keyboard input
                hr = ((IInputObject)explorerBrowserControl).TranslateAcceleratorIO(ref m);
            }
            return (hr == HRESULT.S_OK);
        }

        #endregion

        #endregion

        #region utilities

        /// <summary>
        /// Returns the current view mode of the browser
        /// </summary>
        /// <returns></returns>
        internal FOLDERVIEWMODE GetCurrentViewMode()
        {
            IFolderView2 ifv2 = GetFolderView2();
            uint viewMode = 0;
            if (ifv2 != null)
            {
                try
                {
                    HRESULT hr = ifv2.GetCurrentViewMode(out viewMode);
                    if (hr != HRESULT.S_OK)
                        throw Marshal.GetExceptionForHR((int)hr);
                }
                finally
                {
                    Marshal.ReleaseComObject(ifv2);
                    ifv2 = null;
                }
            }
            return (FOLDERVIEWMODE)viewMode;
        }

        /// <summary>
        /// Gets the IFolderView2 interface from the explorer browser.
        /// </summary>
        /// <returns></returns>
        internal IFolderView2 GetFolderView2()
        {
            Guid iid = new Guid(ExplorerBrowserIIDGuid.IFolderView2);
            IntPtr view = IntPtr.Zero;
            if (this.explorerBrowserControl != null)
            {
                HRESULT hr = this.explorerBrowserControl.GetCurrentView(ref iid, out view);
                switch (hr)
                {
                    case HRESULT.S_OK:
                        break;

                    case HRESULT.E_NOINTERFACE:
                    case HRESULT.E_FAIL:
#if LOG_KNOWN_COM_ERRORS
                        Debugger.Log( 2, "ExplorerBrowser", "Unable to obtain view. Error=" + e.ToString( ) );
#endif
                        return null;

                    default:
                        throw new COMException("ExplorerBrowser failed to get current view.", (int)hr);
                }

                return (IFolderView2)Marshal.GetObjectForIUnknown(view);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the selected items in the explorer browser as an IShellItemArray
        /// </summary>
        /// <returns></returns>
        internal IShellItemArray GetSelectedItemsArray()
        {
            IShellItemArray iArray = null;
            IFolderView2 iFV2 = GetFolderView2();
            if (iFV2 != null)
            {
                try
                {
                    Guid iidShellItemArray = new Guid(ShellIIDGuid.IShellItemArray);
                    object oArray = null;
                    HRESULT hr = iFV2.Items((uint)SVGIO.SVGIO_SELECTION, ref iidShellItemArray, out oArray);
                    iArray = oArray as IShellItemArray;
                    if (hr != HRESULT.S_OK &&
                        hr != HRESULT.E_ELEMENTNOTFOUND &&
                        hr != HRESULT.E_FAIL)
                    {
                        throw new COMException("unexpected error retrieving selection", (int)hr);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(iFV2);
                    iFV2 = null;
                }
            }

            return iArray;
        }

        internal int GetItemsCount()
        {
            int itemsCount = 0;

            IFolderView2 iFV2 = GetFolderView2();
            if (iFV2 != null)
            {
                try
                {
                    HRESULT hr = iFV2.ItemCount((uint)SVGIO.SVGIO_ALLVIEW, out itemsCount);

                    if (hr != HRESULT.S_OK &&
                        hr != HRESULT.E_ELEMENTNOTFOUND &&
                        hr != HRESULT.E_FAIL)
                    {
                        throw new COMException("unexpected error retrieving item count", (int)hr);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(iFV2);
                    iFV2 = null;
                }
            }

            return itemsCount;
        }

        internal int GetSelectedItemsCount()
        {
            int itemsCount = 0;

            IFolderView2 iFV2 = GetFolderView2();
            if (iFV2 != null)
            {
                try
                {
                    HRESULT hr = iFV2.ItemCount((uint)SVGIO.SVGIO_SELECTION, out itemsCount);

                    if( hr != HRESULT.S_OK &&
                        hr != HRESULT.E_ELEMENTNOTFOUND &&
                        hr != HRESULT.E_FAIL )
                    {
                        throw new COMException( "unexpected error retrieving selected item count", (int)hr );
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(iFV2);
                    iFV2 = null;
                }
            }

            return itemsCount;
        }

        /// <summary>
        /// Gets the items in the ExplorerBrowser as an IShellItemArray
        /// </summary>
        /// <returns></returns>
        internal IShellItemArray GetItemsArray()
        {
            IShellItemArray iArray = null;
            IFolderView2 iFV2 = GetFolderView2();
            if (iFV2 != null)
            {
                try
                {
                    Guid iidShellItemArray = new Guid(ShellIIDGuid.IShellItemArray);
                    object oArray = null;
                    HRESULT hr = iFV2.Items((uint)SVGIO.SVGIO_ALLVIEW, ref iidShellItemArray, out oArray);
                    if (hr != HRESULT.S_OK &&
                        hr != HRESULT.E_FAIL &&
                        hr != HRESULT.E_ELEMENTNOTFOUND &&
                        hr != HRESULT.E_INVALIDARG)
                    {
                        throw new COMException("unexpected error retrieving view items", (int)hr);
                    }

                    iArray = oArray as IShellItemArray;
                }
                finally
                {
                    Marshal.ReleaseComObject(iFV2);
                    iFV2 = null;
                }
            }
            return iArray;
        }

        #endregion

        #region view event forwarding
            internal void FireSelectionChanged( )
            {
                if( SelectionChanged != null )
                    SelectionChanged.Invoke( this, EventArgs.Empty );
            }

            internal void FireContentChanged( )
            {
                if( ItemsChanged != null )
                    ItemsChanged.Invoke( this, EventArgs.Empty );
            }

            internal void FireContentEnumerationComplete( )
            {
                if( ViewEnumerationComplete != null )
                    ViewEnumerationComplete.Invoke( this, EventArgs.Empty );
            }

            internal void FireSelectedItemChanged( )
            {
                if( ViewSelectedItemChanged != null )
                    ViewSelectedItemChanged.Invoke( this, EventArgs.Empty );
            }
        #endregion

        #endregion
    }

}
