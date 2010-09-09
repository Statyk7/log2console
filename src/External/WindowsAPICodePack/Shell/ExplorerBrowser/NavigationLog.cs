//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Controls.WindowsForms;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Controls
{

    /// <summary>
    /// The navigation log is a history of the locations visited by the explorer browser. 
    /// </summary>
    public class ExplorerBrowserNavigationLog
    {
        #region operations
        /// <summary>
        /// Clears the contents of the navigation log.
        /// </summary>
        public void ClearLog( )
        {
            // nothing to do
            if( Locations.Count == 0 )
                return;

            bool oldCanNavigateBackward = CanNavigateBackward;
            bool oldCanNavigateForward = CanNavigateForward;

            Locations.Clear( );
            this.currentLocationIndex = -1;

            NavigationLogEventArgs args = new NavigationLogEventArgs( );
            args.LocationsChanged = true;
            args.CanNavigateBackwardChanged = ( oldCanNavigateBackward != CanNavigateBackward );
            args.CanNavigateForwardChanged = ( oldCanNavigateForward != CanNavigateForward );
            if( NavigationLogChanged != null )
                NavigationLogChanged( this, args );
        }
        #endregion

        #region properties
        /// <summary>
        /// Indicates the presence of locations in the log that can be 
        /// reached by calling Navigate(Forward)
        /// </summary>
        public bool CanNavigateForward
        {
            get
            {
                return (CurrentLocationIndex < (Locations.Count - 1));
            }
        }

        /// <summary>
        /// Indicates the presence of locations in the log that can be 
        /// reached by calling Navigate(Backward)
        /// </summary>
        public bool CanNavigateBackward
        {
            get
            {
                return ( CurrentLocationIndex >= 1 );
            }
        }

        /// <summary>
        /// The navigation log
        /// </summary>
        public List<ShellObject> Locations
        {
            get;
            private set;
        }

        /// <summary>
        /// An index into the Locations collection. The ShellObject pointed to 
        /// by this index is the current location of the ExplorerBrowser.
        /// </summary>
        public int CurrentLocationIndex
        {
            get
            {
                return currentLocationIndex;
            }
        }


        /// <summary>
        /// Gets the shell object in the Locations collection pointed to
        /// by CurrentLocationIndex.
        /// </summary>
        public ShellObject CurrentLocation
        { 
            get
            {
                if( currentLocationIndex < 0 )
                    return null;

                return Locations[currentLocationIndex];
            }
        }
        #endregion

        #region events
        /// <summary>
        /// Fires when the navigation log changes or 
        /// the current navigation position changes
        /// </summary>
        public event NavigationLogChangedEventHandler NavigationLogChanged;
        #endregion

        #region implementation

        private ExplorerBrowser parent = null;

        /// <summary>
        /// The pending navigation log action. null if the user is not navigating 
        /// via the navigation log.
        /// </summary>
        private PendingNavigation pendingNavigation = null;
        
        /// <summary>
        /// The index into the Locations collection. -1 if the Locations colleciton 
        /// is empty.
        /// </summary>
        private int currentLocationIndex = -1;
 
        internal ExplorerBrowserNavigationLog( ExplorerBrowser parent )
        {
            if( parent == null )
                throw new ArgumentException( "parent can not be null!" );

            //
            Locations = new List<ShellObject>();

            // Hook navigation events from the parent to distinguish between
            // navigation log induced navigation, and other navigations.
            this.parent = parent;
            this.parent.NavigationComplete += new ExplorerBrowserNavigationCompleteEventHandler( OnNavigationComplete );
            this.parent.NavigationFailed += new ExplorerBrowserNavigationFailedEventHandler( OnNavigationFailed );
        }

        private void OnNavigationFailed( object sender, NavigationFailedEventArgs args )
        {
            pendingNavigation = null;
        }

        private void OnNavigationComplete( object sender, NavigationCompleteEventArgs args )
        {
            NavigationLogEventArgs eventArgs = new NavigationLogEventArgs( );
            bool oldCanNavigateBackward = CanNavigateBackward;
            bool oldCanNavigateForward = CanNavigateForward;

            if( ( pendingNavigation != null ) )
            {
                // navigation log traversal in progress

                // determine if new location is the same as the traversal request
                int result = 0;
                pendingNavigation.location.NativeShellItem.Compare( 
                    args.NewLocation.NativeShellItem, SICHINTF.SICHINT_ALLFIELDS, out result );
                bool shellItemsEqual = ( result == 0 );
                if( shellItemsEqual == false )
                {
                    // new location is different than traversal request, 
                    // behave is if it never happened!
                    // remove history following currentLocationIndex, append new item
                    if( currentLocationIndex < ( Locations.Count - 1 ) )
                        Locations.RemoveRange( (int)currentLocationIndex + 1, (int)( Locations.Count - ( currentLocationIndex + 1 ) ) );
                    Locations.Add( args.NewLocation );
                    currentLocationIndex = (Locations.Count - 1);
                    eventArgs.LocationsChanged = true;
                }
                else
                {
                    // log traversal successful, update index
                    currentLocationIndex = (int)pendingNavigation.index;
                    eventArgs.LocationsChanged = false;
                }
                pendingNavigation = null;
            }
            else
            {
                // remove history following currentLocationIndex, append new item
                if( currentLocationIndex < (Locations.Count - 1) )
                    Locations.RemoveRange( (int)currentLocationIndex + 1, (int)( Locations.Count - (currentLocationIndex + 1)) );
                Locations.Add( args.NewLocation );
                currentLocationIndex = (Locations.Count - 1);
                eventArgs.LocationsChanged = true;
            }

            // update event args
            eventArgs.CanNavigateBackwardChanged = ( oldCanNavigateBackward != CanNavigateBackward );
            eventArgs.CanNavigateForwardChanged = ( oldCanNavigateForward != CanNavigateForward );

            if( NavigationLogChanged != null )
                NavigationLogChanged( this, eventArgs );
        }

        internal bool NavigateLog( NavigationLogDirection direction )
        {
            // determine proper index to navigate to
            int locationIndex = 0;
            if( (direction == NavigationLogDirection.Backward)  && CanNavigateBackward )
            {
                locationIndex = (currentLocationIndex - 1);
            }
            else if( (direction == NavigationLogDirection.Forward)  && CanNavigateForward )
            {
                locationIndex = (currentLocationIndex + 1);
            }
            else
            {
                return false;
            }

            // initiate traversal request
            ShellObject location = Locations[ (int)locationIndex ];
            pendingNavigation = new PendingNavigation( location, locationIndex );
            parent.Navigate( location );
            return true;
        }

        internal bool NavigateLog( int index )
        {
            // can't go anywhere
            if( index >= Locations.Count ||
                index < 0 )
                return false;

            // no need to re navigate to the same location
            if( index == currentLocationIndex )
                return false;

            // initiate traversal request
            ShellObject location = Locations[ (int)index ];
            pendingNavigation = new PendingNavigation( location, index );
            parent.Navigate( location );
            return true;
        }

        #endregion
    }

    /// <summary>
    /// A navigation traversal request
    /// </summary>
    internal class PendingNavigation
    {
        internal PendingNavigation( ShellObject location, int index )
        {
            this.location = location;
            this.index = index;
        }

        internal ShellObject location;
        internal int index;
    }
}