//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Controls
{
    /// <summary>
    /// Fires when the SelectedItems collection changes. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExplorerBrowserSelectionChangedEventHandler( object sender, EventArgs e );
    
    /// <summary>
    /// Fires when the Items colection changes. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExplorerBrowserItemsChangedEventHandler( object sender, EventArgs e ) ;
    
    /// <summary>
    /// Fires when a navigation has been initiated, but is not yet complete.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExplorerBrowserNavigationPendingEventHandler( object sender, NavigationPendingEventArgs e );
    
    /// <summary>
    /// Fires when a navigation has been 'completed': no NavigationPending listener 
    /// has cancelled, and the ExplorerBorwser has created a new view. The view 
    /// will be populated with new items asynchronously, and ItemsChanged will be 
    /// fired to reflect this some time later.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExplorerBrowserNavigationCompleteEventHandler( object sender, NavigationCompleteEventArgs e );
    
    /// <summary>
    /// Fires when either a NavigationPending listener cancels the navigation, or
    /// if the operating system determines that navigation is not possible.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExplorerBrowserNavigationFailedEventHandler( object sender, NavigationFailedEventArgs e );

    /// <summary>
    /// Fires when the ExplorerBorwser view has finished enumerating files.
    /// </summary>
    /// <param name="sender">the explorer borwser</param>
    /// <param name="e">empty</param>
    public delegate void ExplorerBrowserViewEnumerationCompleteHandler( object sender, EventArgs e );

    /// <summary>
    /// Fires when the item selected in the view has changed (i.e., a rename ).
    /// This is not the same as SelectionChanged.
    /// </summary>
    /// <param name="sender">the explorer borwser</param>
    /// <param name="e">empty</param>
    public delegate void ExplorerBrowserViewSelectedItemChangedHandler( object sender, EventArgs e );



    /// <summary>
    /// Event argument for The NavigationPending event
    /// </summary>
    public class NavigationPendingEventArgs : EventArgs
    {
        /// <summary>
        /// The location being navigated to
        /// </summary>
        public ShellObject PendingLocation
        {
            get;
            set;
        }
        
        /// <summary>
        /// Set to 'True' to cancel the navigation.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Event argument for The NavigationComplete event
    /// </summary>
    public class NavigationCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// The new location of the explorer browser
        /// </summary>
        public ShellObject NewLocation
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Event argument for the NavigatinoFailed event
    /// </summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>
        /// The location the the browser would have navigated to.
        /// </summary>
        public ShellObject FailedLocation
        {
            get;
            set;
        }
    }
}