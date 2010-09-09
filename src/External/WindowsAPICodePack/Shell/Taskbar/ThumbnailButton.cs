//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents a taskbar thumbnail button in the thumbnail toolbar.
    /// </summary>
    public sealed class ThumbnailToolbarButton: IDisposable
    {
        private static uint nextId = 101;
        private THUMBBUTTON win32ThumbButton;

        /// <summary>
        /// The event that occurs when the taskbar thumbnail button
        /// is clicked.
        /// </summary>
        public event EventHandler<ThumbnailButtonClickedEventArgs> Click;

        // Internal bool to track whether we should be updating the taskbar 
        // if any of our properties change or if it's just an internal update
        // on the properties (via the constructor)
        private bool internalUpdate = false;

        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        /// <param name="icon">The icon to use for this button</param>
        /// <param name="tooltip">The tooltip string to use for this button.</param>
        public ThumbnailToolbarButton(Icon icon, string tooltip)
        {
            // Start internal update (so we don't accidently update the taskbar
            // via the native API)
            internalUpdate = true;

            // Set our id
            Id = nextId;
            
            // increment the ID
            if (nextId == Int32.MaxValue)
                nextId = 101; // our starting point
            else
                nextId++;

            // Set user settings
            Icon = icon;
            Tooltip = tooltip;

            // Defaults
            Enabled = true;

            // Create a native 
            win32ThumbButton = new THUMBBUTTON();

            // End our internal update
            internalUpdate = false;
        }

        #region Public properties

        /// <summary>
        /// Gets thumbnail button's id.
        /// </summary>
        internal uint Id 
        { 
            get; 
            set; 
        }

        private Icon icon;
        /// <summary>
        /// Gets or sets the thumbnail button's icon.
        /// </summary>
        public Icon Icon 
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    UpdateThumbnailButton();
                }
            }
        }

        private string tooltip;
        /// <summary>
        /// Gets or sets the thumbnail button's tooltip.
        /// </summary>
        public string Tooltip 
        {
            get
            {
                return tooltip;
            }
            set
            {
                if (tooltip != value)
                {
                    tooltip = value;
                    UpdateThumbnailButton();
                }
            }
        }

        private bool visible = true;
        /// <summary>
        /// Gets or sets the thumbnail button's visibility. Default is true.
        /// </summary>
        public bool Visible
        {
            get
            {
                return (this.Flags & THBFLAGS.THBF_HIDDEN) == 0;
            }
            set
            {
                if (visible != value)
                {
                    visible = value;

                    if (value)
                    {
                        this.Flags &= ~(THBFLAGS.THBF_HIDDEN);
                    }
                    else
                    {
                        this.Flags |= THBFLAGS.THBF_HIDDEN;
                    }

                    UpdateThumbnailButton();
                }
                
            }
        }

        private bool enabled = true;
        /// <summary>
        /// Gets or sets the thumbnail button's enabled state. If the button is disabled, it is present, 
        /// but has a visual state that indicates that it will not respond to user action. Default is true.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (this.Flags & THBFLAGS.THBF_DISABLED) == 0;
            }
            set
            {
                if (value != enabled)
                {
                    enabled = value;

                    if (value)
                    {
                        this.Flags &= ~(THBFLAGS.THBF_DISABLED);
                    }
                    else
                    {
                        this.Flags |= THBFLAGS.THBF_DISABLED;
                    }

                    UpdateThumbnailButton();
                }
            }
        }

        private bool dismissOnClick = false;
        /// <summary>
        /// Gets or sets the property that describes the behavior when the button is clicked. 
        /// If set to true, the taskbar button's flyout will close immediately. Default is false.
        /// </summary>
        public bool DismissOnClick
        {
            get
            {
                return (this.Flags & THBFLAGS.THBF_DISMISSONCLICK) == 0;
            }
            set
            {
                if (value != dismissOnClick)
                {
                    dismissOnClick = value;

                    if (value)
                    {
                        this.Flags |= THBFLAGS.THBF_DISMISSONCLICK;
                    }
                    else
                    {
                        this.Flags &= ~(THBFLAGS.THBF_DISMISSONCLICK);
                    }

                    UpdateThumbnailButton();
                }
            }
        }

        private bool isInteractive = true;
        /// <summary>
        /// Gets or sets the property that describes whether the button is interactive with the user. Default is true.
        /// </summary>
        /// <remarks>
        /// Non-interactive buttons don't display any hover behavior nor do they raise click events.
        /// They are intended to be used as status icons. This is mostly similar to being not Enabled, 
        /// but the image is not desaturated.
        /// </remarks>
        public bool IsInteractive
        {
            get
            {
                return (this.Flags & THBFLAGS.THBF_NONINTERACTIVE) == 0;
            }
            set
            {
                if (value != isInteractive)
                {
                    isInteractive = value;

                    if (value)
                    {
                        this.Flags &= ~(THBFLAGS.THBF_NONINTERACTIVE);
                    }
                    else
                    {
                        this.Flags |= THBFLAGS.THBF_NONINTERACTIVE;
                    }

                    UpdateThumbnailButton();
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Native flags enum (used when creating the native button)
        /// </summary>
        internal THBFLAGS Flags { get; set; }

        /// <summary>
        /// Native representation of the thumbnail button
        /// </summary>
        internal THUMBBUTTON Win32ThumbButton
        {
            get
            {
                win32ThumbButton.iId = Id;
                win32ThumbButton.szTip = Tooltip;
                win32ThumbButton.dwFlags = Flags;

                win32ThumbButton.dwMask = THBMASK.THB_FLAGS;
                if (Tooltip != null)
                {
                    win32ThumbButton.dwMask |= THBMASK.THB_TOOLTIP;
                }
                if (Icon != null)
                {
                    win32ThumbButton.dwMask |= THBMASK.THB_ICON;
                    win32ThumbButton.hIcon = Icon.Handle;
                }

                return win32ThumbButton;
            }
        }

        /// <summary>
        /// The window manager should call this method to raise the public click event to all
        /// the subscribers.
        /// </summary>
        /// <param name="taskbarWindow">Taskbar Window associated with this button</param>
        internal void FireClick(TaskbarWindow taskbarWindow)
        {
            if (Click != null && taskbarWindow != null)
            {
                if (taskbarWindow.UserWindowHandle != IntPtr.Zero)
                    Click(this, new ThumbnailButtonClickedEventArgs(taskbarWindow.UserWindowHandle, this));
                else if (taskbarWindow.WindowsControl != null)
                    Click(this, new ThumbnailButtonClickedEventArgs(taskbarWindow.WindowsControl, this));
            }
        }

        /// <summary>
        /// Handle to the window to which this button is for (on the taskbar).
        /// </summary>
        internal IntPtr WindowHandle 
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// Indicates if this button was added to the taskbar. If it's not yet added,
        /// then we can't do any updates on it.
        /// </summary>
        internal bool AddedToTaskbar 
        { 
            get; 
            set; 
        }

        internal void UpdateThumbnailButton()
        {
            if (internalUpdate || !AddedToTaskbar)
                return;

            // Get the array of thumbnail buttons in native format
            THUMBBUTTON[] nativeButtons = { Win32ThumbButton };

            HRESULT hr = TaskbarManager.Instance.TaskbarList.ThumbBarUpdateButtons(WindowHandle, 1, nativeButtons);

            if (!CoreErrorHelper.Succeeded((int)hr))
                Marshal.ThrowExceptionForHR((int)hr);

        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        ~ThumbnailToolbarButton()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release the native objects.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                Icon.Dispose();
                tooltip = null;
            }
        }

        #endregion
    }

}
