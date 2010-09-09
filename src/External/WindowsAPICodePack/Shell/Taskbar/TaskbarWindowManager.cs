// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Shell;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal enum TaskbarProxyWindowType
    {
        TabbedThumbnail,
        ThumbnailToolbar,
    }

    internal class TaskbarWindowManager
    {
        internal IList<TaskbarWindow> taskbarWindowList;
        private bool buttonsAdded = false;

        private static TaskbarWindowManager instance = null;
        internal static TaskbarWindowManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TaskbarWindowManager();

                return instance;
            }
        }

        internal TaskbarWindowManager()
        {
            taskbarWindowList = new List<TaskbarWindow>();
        }

        internal void AddThumbnailButtons(IntPtr userWindowHandle, params ThumbnailToolbarButton[] buttons)
        {
            // Try to get an existing taskbar window for this user windowhandle,
            // or get one that is created for us
            TaskbarWindow taskbarWindow = GetTaskbarWindow(userWindowHandle, TaskbarProxyWindowType.ThumbnailToolbar);

            if (taskbarWindow == null)
            {
                taskbarWindow = new TaskbarWindow(userWindowHandle, buttons);
                taskbarWindowList.Add(taskbarWindow);
            }
            else if (taskbarWindow.ThumbnailButtons == null)
                taskbarWindow.ThumbnailButtons = buttons;
            else
            {
                // We already have buttons assigned
                throw new InvalidOperationException("Toolbar buttons for this window are already added. Once added, neither the order of the buttons cannot be changed, nor can they be added or removed.");
            }
        }

        internal void AddThumbnailButtons(System.Windows.UIElement control, params ThumbnailToolbarButton[] buttons)
        {
            // Try to get an existing taskbar window for this user windowhandle,
            // or get one that is created for us
            TaskbarWindow taskbarWindow = GetTaskbarWindow(control, TaskbarProxyWindowType.ThumbnailToolbar);

            if (taskbarWindow == null)
            {
                taskbarWindow = new TaskbarWindow(control, buttons);
                taskbarWindowList.Add(taskbarWindow);
            }
            else if (taskbarWindow.ThumbnailButtons == null)
                taskbarWindow.ThumbnailButtons = buttons;
            else
            {
                // We already have buttons assigned
                throw new InvalidOperationException("Toolbar buttons for this window are already added. Please refer to the Remarks section of the AddButtons method for more information on updating the properties or hiding existing buttons.");
            }
        }

        internal void AddTabbedThumbnail(TabbedThumbnail preview)
        {
            // Create a TOP-LEVEL proxy window for the user's source window/control
            TaskbarWindow taskbarWindow = null;

            if (preview.WindowHandle != IntPtr.Zero)
                taskbarWindow = GetTaskbarWindow(preview.WindowHandle, TaskbarProxyWindowType.TabbedThumbnail);
            else
                taskbarWindow = GetTaskbarWindow(preview.WindowsControl, TaskbarProxyWindowType.TabbedThumbnail);

            if (taskbarWindow == null)
            {
                taskbarWindow = new TaskbarWindow(preview);
                taskbarWindowList.Add(taskbarWindow);
            }
            else if (taskbarWindow.TabbedThumbnail == null)
                taskbarWindow.TabbedThumbnail = preview;

            //
            preview.TaskbarWindow = taskbarWindow;

            // Listen for Title changes
            preview.TitleChanged += new EventHandler(thumbnailPreview_TitleChanged);
            preview.TooltipChanged += new EventHandler(thumbnailPreview_TooltipChanged);

            // Get/Set properties for proxy window
            IntPtr hwnd = taskbarWindow.WindowToTellTaskbarAbout;

            // Register this new tab and set it as being active.
            TaskbarManager.Instance.TaskbarList.RegisterTab(hwnd, preview.ParentWindowHandle);
            TaskbarManager.Instance.TaskbarList.SetTabOrder(hwnd, IntPtr.Zero);
            TaskbarManager.Instance.TaskbarList.SetTabActive(hwnd, preview.ParentWindowHandle, 0);

            // We need to make sure we can set these properties even when
            // running with admin 
            TabbedThumbnailNativeMethods.ChangeWindowMessageFilter(TabbedThumbnailNativeMethods.WM_DWMSENDICONICTHUMBNAIL, TabbedThumbnailNativeMethods.MSGFLT_ADD);
            TabbedThumbnailNativeMethods.ChangeWindowMessageFilter(TabbedThumbnailNativeMethods.WM_DWMSENDICONICLIVEPREVIEWBITMAP, TabbedThumbnailNativeMethods.MSGFLT_ADD);

            TabbedThumbnailNativeMethods.EnableCustomWindowPreview(hwnd, true);

            // Make sure we use the initial title set by the user
            // Trigger a "fake" title changed event, so the title is set on the taskbar thumbnail.
            // Empty/null title will be ignored.
            thumbnailPreview_TitleChanged(preview, EventArgs.Empty);
            thumbnailPreview_TooltipChanged(preview, EventArgs.Empty);

            // Indicate to the preview that we've added it on the taskbar
            preview.AddedToTaskbar = true;
        }

        internal TaskbarWindow GetTaskbarWindow(System.Windows.UIElement windowsControl, TaskbarProxyWindowType taskbarProxyWindowType)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("userWindowHandle");

            TaskbarWindow toReturn = null;

            foreach (TaskbarWindow window in taskbarWindowList)
            {
                if ((window.TabbedThumbnail != null && window.TabbedThumbnail.WindowsControl == windowsControl)
                    || (window.ThumbnailToolbarProxyWindow != null && window.ThumbnailToolbarProxyWindow.WindowsControl == windowsControl))
                {
                    toReturn = window;
                    break;
                }
            }

            // It's not in our list. Create a new TaskbarWindow, set the correct properties,
            // return the window to the caller.
            if (toReturn == null)
                return null;

            if (taskbarProxyWindowType == TaskbarProxyWindowType.ThumbnailToolbar)
                toReturn.EnableThumbnailToolbars = true;
            else
                toReturn.EnableTabbedThumbnails = true;

            return toReturn;
        }

        internal TaskbarWindow GetTaskbarWindow(IntPtr userWindowHandle, TaskbarProxyWindowType taskbarProxyWindowType)
        {
            if (userWindowHandle == IntPtr.Zero)
                throw new ArgumentException("userWindowHandle");

            TaskbarWindow toReturn = null;

            foreach (TaskbarWindow window in taskbarWindowList)
            {
                if (window.UserWindowHandle == userWindowHandle)
                {
                    toReturn = window;
                    break;
                }
            }

            // It's not in our list. Create a new TaskbarWindow, set the correct properties,
            // return the window to the caller.
            if (toReturn == null)
                return null;

            if (taskbarProxyWindowType == TaskbarProxyWindowType.ThumbnailToolbar)
                toReturn.EnableThumbnailToolbars = true;
            else
                toReturn.EnableTabbedThumbnails = true;

            return toReturn;
        }

        void thumbnailPreview_TooltipChanged(object sender, EventArgs e)
        {
            TabbedThumbnail preview = sender as TabbedThumbnail;

            TaskbarWindow taskbarWindow = null;

            if (preview.WindowHandle == IntPtr.Zero)
                taskbarWindow = GetTaskbarWindow(preview.WindowsControl, TaskbarProxyWindowType.TabbedThumbnail);
            else
                taskbarWindow = GetTaskbarWindow(preview.WindowHandle, TaskbarProxyWindowType.TabbedThumbnail);

            // Update the proxy window for the tabbed thumbnail
            if (taskbarWindow != null)
                TaskbarManager.Instance.TaskbarList.SetThumbnailTooltip(taskbarWindow.WindowToTellTaskbarAbout, preview.Tooltip);
        }

        void thumbnailPreview_TitleChanged(object sender, EventArgs e)
        {
            TabbedThumbnail preview = sender as TabbedThumbnail;

            TaskbarWindow taskbarWindow = null;

            if (preview.WindowHandle == IntPtr.Zero)
                taskbarWindow = GetTaskbarWindow(preview.WindowsControl, TaskbarProxyWindowType.TabbedThumbnail);
            else
                taskbarWindow = GetTaskbarWindow(preview.WindowHandle, TaskbarProxyWindowType.TabbedThumbnail);

            // Update the proxy window for the tabbed thumbnail
            if (taskbarWindow != null)
                taskbarWindow.Title = preview.Title;
        }

        /// <summary>
        /// Dispatches a window message so that the appropriate events
        /// can be invoked. This is used for the Taskbar's thumbnail toolbar feature.
        /// </summary>
        /// <param name="m">The window message, typically obtained
        /// from a Windows Forms or WPF window procedure.</param>
        /// <param name="taskbarWindow">Taskbar window for which we are intercepting the messages</param>
        /// <returns>Returns true if this method handles the window message</returns>
        internal bool DispatchMessage(ref Message m, TaskbarWindow taskbarWindow)
        {
            if (taskbarWindow.EnableThumbnailToolbars)
            {
                if (m.Msg == (int)TaskbarNativeMethods.WM_TASKBARBUTTONCREATED)
                {
                    AddButtons(taskbarWindow);
                }
                else
                {
                    if (!buttonsAdded)
                        AddButtons(taskbarWindow);

                    switch (m.Msg)
                    {

                        case TaskbarNativeMethods.WM_COMMAND:
                            if (CoreNativeMethods.HIWORD(m.WParam.ToInt64(), 16) == THUMBBUTTON.THBN_CLICKED)
                            {
                                int buttonId = CoreNativeMethods.LOWORD(m.WParam.ToInt64());

                                var buttonsFound =
                                    from b in taskbarWindow.ThumbnailButtons
                                    where b.Id == buttonId
                                    select b;

                                foreach (ThumbnailToolbarButton button in buttonsFound)
                                {
                                    button.FireClick(taskbarWindow);
                                }
                            }
                            break;
                    } // End switch
                } // End else
            } // End if


            // If we are removed from the taskbar, ignore all the messages
            if (taskbarWindow.EnableTabbedThumbnails)
            {
                if (taskbarWindow.TabbedThumbnail.RemovedFromTaskbar)
                    return false;
                else if (m.Msg == (int)TabbedThumbnailNativeMethods.WM_ACTIVATE)
                {
                    // Raise the event
                    taskbarWindow.TabbedThumbnail.OnTabbedThumbnailActivated();

                    SetActiveTab(taskbarWindow);

                    return true;
                }
                else if (m.Msg == (int)TaskbarNativeMethods.WM_DWMSENDICONICTHUMBNAIL)
                {
                    int width = (int)((long)m.LParam >> 16);
                    int height = (int)(((long)m.LParam) & (0xFFFF));
                    Size requestedSize = new Size(width, height);

                    // Fire an event to let the user update their bitmap
                    // Raise the event
                    taskbarWindow.TabbedThumbnail.OnTabbedThumbnailBitmapRequested();

                    IntPtr hBitmap = IntPtr.Zero;

                    // Default size for the thumbnail
                    Size realWindowSize = new Size(200, 200);

                    if (taskbarWindow.TabbedThumbnail.WindowHandle != IntPtr.Zero)
                        TabbedThumbnailNativeMethods.GetClientSize(taskbarWindow.TabbedThumbnail.WindowHandle, out realWindowSize);
                    else if (taskbarWindow.TabbedThumbnail.WindowsControl != null)
                        realWindowSize = new Size(
                            Convert.ToInt32(taskbarWindow.TabbedThumbnail.WindowsControl.RenderSize.Width),
                            Convert.ToInt32(taskbarWindow.TabbedThumbnail.WindowsControl.RenderSize.Height));

                    if ((realWindowSize.Height == -1) && (realWindowSize.Width == -1))
                        realWindowSize.Width = realWindowSize.Height = 199;

                    // capture the bitmap for the given control
                    // If the user has already specified us a bitmap to use, use that.
                    if (taskbarWindow.TabbedThumbnail.ClippingRectangle != null && taskbarWindow.TabbedThumbnail.ClippingRectangle.Value != Rectangle.Empty)
                    {
                        if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                            hBitmap = GrabBitmap(taskbarWindow, realWindowSize);
                        else
                            hBitmap = taskbarWindow.TabbedThumbnail.CurrentHBitmap;

                        // Clip the bitmap we just got
                        Bitmap bmp = Bitmap.FromHbitmap(hBitmap);

                        Rectangle clippingRectangle = taskbarWindow.TabbedThumbnail.ClippingRectangle.Value;

                        // If our clipping rect is out of bounds, update it
                        if (clippingRectangle.Height > requestedSize.Height)
                            clippingRectangle.Height = requestedSize.Height;
                        if (clippingRectangle.Width > requestedSize.Width)
                            clippingRectangle.Width = requestedSize.Width;

                        bmp = bmp.Clone(clippingRectangle, bmp.PixelFormat);

                        // Make sure we dispose the bitmap before assigning, otherwise we'll have a memory leak
                        if (hBitmap != IntPtr.Zero && taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                        {
                            ShellNativeMethods.DeleteObject(hBitmap);
                        }
                        hBitmap = bmp.GetHbitmap();
                    }
                    else
                    {
                        // Else, user didn't want any clipping, if they haven't provided us a bitmap,
                        // use the screencapture utility and capture it.

                        hBitmap = taskbarWindow.TabbedThumbnail.CurrentHBitmap;

                        // If no bitmap, capture one using the utility
                        if (hBitmap == IntPtr.Zero)
                            hBitmap = GrabBitmap(taskbarWindow, realWindowSize);
                    }

                    // Only set the thumbnail if it's not null. 
                    // If it's null (either we didn't get the bitmap or size was 0),
                    // let DWM handle it
                    if (hBitmap != IntPtr.Zero)
                    {
                        Bitmap temp = TabbedThumbnailScreenCapture.ResizeImageWithAspect(hBitmap, requestedSize.Width, requestedSize.Height, true);

                        if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                            ShellNativeMethods.DeleteObject(hBitmap);
                        
                        hBitmap = temp.GetHbitmap();
                        TabbedThumbnailNativeMethods.SetIconicThumbnail(taskbarWindow.WindowToTellTaskbarAbout, hBitmap);
                        temp.Dispose();
                    }

                    // If the bitmap we have is not coming from the user (i.e. we created it here),
                    // then make sure we delete it as we don't need it now.
                    if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                        ShellNativeMethods.DeleteObject(hBitmap);

                    return true;
                }
                else if (m.Msg == (int)TaskbarNativeMethods.WM_DWMSENDICONICLIVEPREVIEWBITMAP)
                {
                    // Try to get the width/height
                    int width = (int)(((long)m.LParam) >> 16);
                    int height = (int)(((long)m.LParam) & (0xFFFF));

                    // Default size for the thumbnail
                    Size realWindowSize = new Size(200, 200);

                    if (taskbarWindow.TabbedThumbnail.WindowHandle != IntPtr.Zero)
                        TabbedThumbnailNativeMethods.GetClientSize(taskbarWindow.TabbedThumbnail.WindowHandle, out realWindowSize);
                    else if (taskbarWindow.TabbedThumbnail.WindowsControl != null)
                        realWindowSize = new Size(
                            Convert.ToInt32(taskbarWindow.TabbedThumbnail.WindowsControl.RenderSize.Width),
                            Convert.ToInt32(taskbarWindow.TabbedThumbnail.WindowsControl.RenderSize.Height));

                    // If we don't have a valid height/width, use the original window's size
                    if (width <= 0)
                        width = realWindowSize.Width;
                    if (height <= 0)
                        height = realWindowSize.Height;

                    // Fire an event to let the user update their bitmap
                    // Raise the event
                    taskbarWindow.TabbedThumbnail.OnTabbedThumbnailBitmapRequested();

                    // capture the bitmap for the given control
                    // If the user has already specified us a bitmap to use, use that.
                    IntPtr hBitmap = taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero ? GrabBitmap(taskbarWindow, realWindowSize) : taskbarWindow.TabbedThumbnail.CurrentHBitmap;

                    // If we have a valid parent window handle,
                    // calculate the offset so we can place the "peek" bitmap
                    // correctly on the app window
                    if (taskbarWindow.TabbedThumbnail.ParentWindowHandle != IntPtr.Zero && taskbarWindow.TabbedThumbnail.WindowHandle != IntPtr.Zero)
                    {
                        Point offset = new Point();

                        // if we don't have a offset specified already by the user...
                        if (!taskbarWindow.TabbedThumbnail.PeekOffset.HasValue)
                        {
                            offset = WindowUtilities.GetParentOffsetOfChild(taskbarWindow.TabbedThumbnail.WindowHandle, taskbarWindow.TabbedThumbnail.ParentWindowHandle);
                        }
                        else
                        {
                            offset = new Point(Convert.ToInt32(taskbarWindow.TabbedThumbnail.PeekOffset.Value.X),
                                Convert.ToInt32(taskbarWindow.TabbedThumbnail.PeekOffset.Value.Y));
                        }

                        // Only set the peek bitmap if it's not null. 
                        // If it's null (either we didn't get the bitmap or size was 0),
                        // let DWM handle it
                        if (hBitmap != IntPtr.Zero)
                        {
                            if (offset.X >= 0 && offset.Y >= 0)
                                TabbedThumbnailNativeMethods.SetPeekBitmap(taskbarWindow.WindowToTellTaskbarAbout, hBitmap, offset, taskbarWindow.TabbedThumbnail.DisplayFrameAroundBitmap);
                        }

                        // If the bitmap we have is not coming from the user (i.e. we created it here),
                        // then make sure we delete it as we don't need it now.
                        if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                            ShellNativeMethods.DeleteObject(hBitmap);

                        return true;
                    }
                    // Else, we don't have a valid window handle from the user. This is mostly likely because
                    // we have a WPF UIElement control. If that's the case, use a different screen capture method
                    // and also couple of ways to try to calculate the control's offset w.r.t it's parent.
                    else if (taskbarWindow.TabbedThumbnail.ParentWindowHandle != IntPtr.Zero &&
                        taskbarWindow.TabbedThumbnail.WindowsControl != null)
                    {
                        System.Windows.Point offset;

                        if (!taskbarWindow.TabbedThumbnail.PeekOffset.HasValue)
                        {
                            // Calculate the offset for a WPF UIElement control
                            // For hidden controls, we can't seem to perform the transform.
                            GeneralTransform objGeneralTransform = taskbarWindow.TabbedThumbnail.WindowsControl.TransformToVisual(taskbarWindow.TabbedThumbnail.WindowsControlParentWindow);
                            offset = objGeneralTransform.Transform(new System.Windows.Point(0, 0));
                        }
                        else
                        {
                            offset = new System.Windows.Point(taskbarWindow.TabbedThumbnail.PeekOffset.Value.X, taskbarWindow.TabbedThumbnail.PeekOffset.Value.Y);
                        }

                        // Only set the peek bitmap if it's not null. 
                        // If it's null (either we didn't get the bitmap or size was 0),
                        // let DWM handle it
                        if (hBitmap != IntPtr.Zero)
                        {
                            if (offset.X >= 0 && offset.Y >= 0)
                                TabbedThumbnailNativeMethods.SetPeekBitmap(taskbarWindow.WindowToTellTaskbarAbout, hBitmap, new Point((int)offset.X, (int)offset.Y), taskbarWindow.TabbedThumbnail.DisplayFrameAroundBitmap);
                            else
                                TabbedThumbnailNativeMethods.SetPeekBitmap(taskbarWindow.WindowToTellTaskbarAbout, hBitmap, taskbarWindow.TabbedThumbnail.DisplayFrameAroundBitmap);
                        }

                        // If the bitmap we have is not coming from the user (i.e. we created it here),
                        // then make sure we delete it as we don't need it now.
                        if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                            ShellNativeMethods.DeleteObject(hBitmap);

                        return true;
                    }
                    else
                    {
                        // Else (no parent specified), just set the bitmap. It would take over the entire 
                        // application window (would work only if you are a MDI app)

                        // Only set the peek bitmap if it's not null. 
                        // If it's null (either we didn't get the bitmap or size was 0),
                        // let DWM handle it
                        if (hBitmap != null)
                            TabbedThumbnailNativeMethods.SetPeekBitmap(taskbarWindow.WindowToTellTaskbarAbout, hBitmap, taskbarWindow.TabbedThumbnail.DisplayFrameAroundBitmap);

                        // If the bitmap we have is not coming from the user (i.e. we created it here),
                        // then make sure we delete it as we don't need it now.
                        if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                            ShellNativeMethods.DeleteObject(hBitmap);

                        return true;
                    }
                }
                else if (m.Msg == (int)TabbedThumbnailNativeMethods.WM_DESTROY)
                {
                    TaskbarManager.Instance.TaskbarList.UnregisterTab(taskbarWindow.WindowToTellTaskbarAbout);

                    taskbarWindow.TabbedThumbnail.RemovedFromTaskbar = true;

                    return true;
                }
                else if (m.Msg == (int)TabbedThumbnailNativeMethods.WM_NCDESTROY)
                {
                    // Raise the event
                    taskbarWindow.TabbedThumbnail.OnTabbedThumbnailClosed();

                    // Remove the taskbar window from our internal list
                    if (taskbarWindowList.Contains(taskbarWindow))
                        taskbarWindowList.Remove(taskbarWindow);

                    taskbarWindow.Dispose();
                    taskbarWindow = null;

                    return true;
                }
                else if (m.Msg == (int)TabbedThumbnailNativeMethods.WM_SYSCOMMAND)
                {
                    if (((int)m.WParam) == TabbedThumbnailNativeMethods.SC_CLOSE)
                    {
                        // Raise the event
                        taskbarWindow.TabbedThumbnail.OnTabbedThumbnailClosed();

                        // Remove the taskbar window from our internal list
                        if (taskbarWindowList.Contains(taskbarWindow))
                            taskbarWindowList.Remove(taskbarWindow);

                        taskbarWindow.Dispose();
                        taskbarWindow = null;
                    }
                    else if (((int)m.WParam) == TabbedThumbnailNativeMethods.SC_MAXIMIZE)
                    {
                        // Raise the event
                        taskbarWindow.TabbedThumbnail.OnTabbedThumbnailMaximized();
                    }
                    else if (((int)m.WParam) == TabbedThumbnailNativeMethods.SC_MINIMIZE)
                    {
                        // Raise the event
                        taskbarWindow.TabbedThumbnail.OnTabbedThumbnailMinimized();
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Helper function to capture a bitmap for a given window handle or incase of WPF app,
        /// an UIElement.
        /// </summary>
        /// <param name="taskbarWindow">The proxy window for which a bitmap needs to be created</param>
        /// <param name="requestedSize">Size for the requested bitmap image</param>
        /// <returns>Bitmap captured from the window handle or UIElement. Null if the window is hidden or it's size is zero.</returns>
        private IntPtr GrabBitmap(TaskbarWindow taskbarWindow, System.Drawing.Size requestedSize)
        {
            IntPtr hBitmap = IntPtr.Zero;

            if (taskbarWindow.TabbedThumbnail.WindowHandle != IntPtr.Zero)
            {
                if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                {
                    Bitmap bmp = TabbedThumbnailScreenCapture.GrabWindowBitmap(taskbarWindow.TabbedThumbnail.WindowHandle, requestedSize);
                    hBitmap = bmp != null ? bmp.GetHbitmap() : IntPtr.Zero;

                    if (bmp != null)
                    {
                        bmp.Dispose();
                        bmp = null;
                    }
                }
                else
                {
                    System.Drawing.Image img = System.Drawing.Image.FromHbitmap(taskbarWindow.TabbedThumbnail.CurrentHBitmap);
                    System.Drawing.Bitmap bmp = new Bitmap(img, requestedSize);

                    hBitmap = bmp != null ? bmp.GetHbitmap() : IntPtr.Zero;

                    // Dipose the image
                    bmp.Dispose();
                    bmp = null;

                    img.Dispose();
                    img = null;
                }
            }
            else if (taskbarWindow.TabbedThumbnail.WindowsControl != null)
            {
                if (taskbarWindow.TabbedThumbnail.CurrentHBitmap == IntPtr.Zero)
                {
                    Bitmap bmp = TabbedThumbnailScreenCapture.GrabWindowBitmap(taskbarWindow.TabbedThumbnail.WindowsControl, 96, 96, requestedSize.Width, requestedSize.Height);
                    hBitmap = bmp != null ? bmp.GetHbitmap() : IntPtr.Zero;

                    if (bmp != null)
                    {
                        bmp.Dispose();
                        bmp = null;
                    }
                }
                else
                {
                    System.Drawing.Image img = System.Drawing.Image.FromHbitmap(taskbarWindow.TabbedThumbnail.CurrentHBitmap);
                    System.Drawing.Bitmap bmp = new Bitmap(img, requestedSize);

                    hBitmap = bmp != null ? bmp.GetHbitmap() : IntPtr.Zero;

                    // Dipose the image
                    bmp.Dispose();
                    bmp = null;

                    img.Dispose();
                    img = null;
                }
            }

            return hBitmap;
        }

        internal void SetActiveTab(TaskbarWindow taskbarWindow)
        {
            if (taskbarWindow != null)
                TaskbarManager.Instance.TaskbarList.SetTabActive(taskbarWindow.WindowToTellTaskbarAbout, taskbarWindow.TabbedThumbnail.ParentWindowHandle, 0);
        }

        internal void UnregisterTab(TaskbarWindow taskbarWindow)
        {
            if (taskbarWindow != null)
                TaskbarManager.Instance.TaskbarList.UnregisterTab(taskbarWindow.WindowToTellTaskbarAbout);
        }

        internal void InvalidatePreview(TaskbarWindow taskbarWindow)
        {
            if (taskbarWindow != null)
                TabbedThumbnailNativeMethods.DwmInvalidateIconicBitmaps(taskbarWindow.WindowToTellTaskbarAbout);
        }

        private void AddButtons(TaskbarWindow taskbarWindow)
        {
            // Add the buttons
            // Get the array of thumbnail buttons in native format
            THUMBBUTTON[] nativeButtons = (from thumbButton in taskbarWindow.ThumbnailButtons
                                           select thumbButton.Win32ThumbButton).ToArray();

            // Add the buttons on the taskbar
            HRESULT hr = TaskbarManager.Instance.TaskbarList.ThumbBarAddButtons(taskbarWindow.WindowToTellTaskbarAbout, (uint)taskbarWindow.ThumbnailButtons.Length, nativeButtons);

            if (!CoreErrorHelper.Succeeded((int)hr))
                Marshal.ThrowExceptionForHR((int)hr);

            // Set the window handle on the buttons (for future updates)
            buttonsAdded = true;
            Array.ForEach(taskbarWindow.ThumbnailButtons, new Action<ThumbnailToolbarButton>(UpdateHandle));
        }

        private void UpdateHandle(ThumbnailToolbarButton button)
        {
            button.AddedToTaskbar = buttonsAdded;
        }
    }
}
