//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Taskbar
{
    /// <summary>
    /// Represents the main class for adding and removing tabbed thumbnails on the Taskbar
    /// for child windows and controls.
    /// </summary>
    public class TabbedThumbnailManager
    {
        /// <summary>
        /// Internal dictionary to keep track of the user's window handle and its 
        /// corresponding thumbnail preview objects.
        /// </summary>
        private IDictionary<IntPtr, TabbedThumbnail> tabbedThumbnailList;
        private IDictionary<UIElement, TabbedThumbnail> tabbedThumbnailListWPF; // list for WPF controls

        /// <summary>
        /// Internal constructor that creates a new dictionary for keeping track of the window handles
        /// and their corresponding thumbnail preview objects.
        /// </summary>
        internal TabbedThumbnailManager()
        {
            tabbedThumbnailList = new Dictionary<IntPtr, TabbedThumbnail>();
            tabbedThumbnailListWPF = new Dictionary<UIElement, TabbedThumbnail>();
        }

        /// <summary>
        /// Adds a new tabbed thumbnail to the taskbar.
        /// </summary>
        /// <param name="preview">Thumbnail preview for a specific window handle or control. The preview
        /// object can be initialized with specific properties for the title, bitmap, and tooltip.</param>
        /// <exception cref="System.ArgumentException">If the tabbed thumbnail has already been added</exception>
        public void AddThumbnailPreview(TabbedThumbnail preview)
        {
            if (preview.WindowHandle == IntPtr.Zero) // it's most likely a UI Element
            {
                if (tabbedThumbnailListWPF.ContainsKey(preview.WindowsControl))
                    throw new ArgumentException("This preview has already been added");
            }
            else
            {
                // Regular control with a valid handle
                if (tabbedThumbnailList.ContainsKey(preview.WindowHandle))
                    throw new ArgumentException("This preview has already been added");
            }

            TaskbarWindowManager.Instance.AddTabbedThumbnail(preview);

            // Add the preview and window manager to our cache

            // Probably a UIElement control
            if (preview.WindowHandle == IntPtr.Zero)
                tabbedThumbnailListWPF.Add(preview.WindowsControl, preview);
            else
                tabbedThumbnailList.Add(preview.WindowHandle, preview);

            preview.InvalidatePreview();
        }

        /// <summary>
        /// Gets the TabbedThumbnail object associated with the given window handle
        /// </summary>
        /// <param name="windowHandle">Window handle for the control/window</param>
        /// <returns>TabbedThumbnail associated with the given window handle</returns>
        public TabbedThumbnail GetThumbnailPreview(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("Window handle is invalid", "windowHandle");

            if (tabbedThumbnailList.ContainsKey(windowHandle))
                return tabbedThumbnailList[windowHandle];
            else
                return null;
        }

        /// <summary>
        /// Gets the TabbedThumbnail object associated with the given control
        /// </summary>
        /// <param name="control">Specific control for which the preview object is requested</param>
        /// <returns>TabbedThumbnail associated with the given control</returns>
        public TabbedThumbnail GetThumbnailPreview(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            return GetThumbnailPreview(control.Handle);
        }

        /// <summary>
        /// Gets the TabbedThumbnail object associated with the given WPF Window
        /// </summary>
        /// <param name="windowsControl">WPF Control (UIElement) for which the preview object is requested</param>
        /// <returns>TabbedThumbnail associated with the given WPF Window</returns>
        public TabbedThumbnail GetThumbnailPreview(UIElement windowsControl)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("windowsControl");

            if (tabbedThumbnailListWPF.ContainsKey(windowsControl))
                return tabbedThumbnailListWPF[windowsControl];
            else
                return null;

        }

        /// <summary>
        /// Remove the tabbed thumbnail from the taskbar.
        /// </summary>
        /// <param name="preview">TabbedThumbnail associated with the control/window that 
        /// is to be removed from the taskbar</param>
        public void RemoveThumbnailPreview(TabbedThumbnail preview)
        {
            if (preview == null)
                throw new ArgumentNullException("preview");

            if (tabbedThumbnailList.ContainsKey(preview.WindowHandle))
                RemoveThumbnailPreview(preview.WindowHandle);
            else if (tabbedThumbnailListWPF.ContainsKey(preview.WindowsControl))
                RemoveThumbnailPreview(preview.WindowsControl);
        }

        /// <summary>
        /// Remove the tabbed thumbnail from the taskbar.
        /// </summary>
        /// <param name="windowHandle">TabbedThumbnail associated with the window handle that 
        /// is to be removed from the taskbar</param>
        public void RemoveThumbnailPreview(IntPtr windowHandle)
        {
            if (tabbedThumbnailList.ContainsKey(windowHandle))
            {
                TaskbarWindowManager.Instance.UnregisterTab(tabbedThumbnailList[windowHandle].TaskbarWindow);

                tabbedThumbnailList.Remove(windowHandle);

                TaskbarWindow taskbarWindow = TaskbarWindowManager.Instance.GetTaskbarWindow(windowHandle, TaskbarProxyWindowType.TabbedThumbnail);

                if (taskbarWindow != null)
                {
                    if (TaskbarWindowManager.Instance.taskbarWindowList.Contains(taskbarWindow))
                        TaskbarWindowManager.Instance.taskbarWindowList.Remove(taskbarWindow);
                    taskbarWindow.Dispose();
                    taskbarWindow = null;
                }
            }
            else
                throw new ArgumentException("The given control has not been added to the taskbar.");
        }

        /// <summary>
        /// Remove the tabbed thumbnail from the taskbar.
        /// </summary>
        /// <param name="control">TabbedThumbnail associated with the control that 
        /// is to be removed from the taskbar</param>
        public void RemoveThumbnailPreview(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            IntPtr handle = control.Handle;

            RemoveThumbnailPreview(handle);
        }

        /// <summary>
        /// Remove the tabbed thumbnail from the taskbar.
        /// </summary>
        /// <param name="windowsControl">TabbedThumbnail associated with the WPF Control (UIElement) that 
        /// is to be removed from the taskbar</param>
        public void RemoveThumbnailPreview(UIElement windowsControl)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("windowsControl");

            if (tabbedThumbnailListWPF.ContainsKey(windowsControl))
            {
                TaskbarWindowManager.Instance.UnregisterTab(tabbedThumbnailListWPF[windowsControl].TaskbarWindow);

                tabbedThumbnailListWPF.Remove(windowsControl);

                TaskbarWindow taskbarWindow = TaskbarWindowManager.Instance.GetTaskbarWindow(windowsControl, TaskbarProxyWindowType.TabbedThumbnail);

                if (taskbarWindow != null)
                {
                    if (TaskbarWindowManager.Instance.taskbarWindowList.Contains(taskbarWindow))
                        TaskbarWindowManager.Instance.taskbarWindowList.Remove(taskbarWindow);
                    taskbarWindow.Dispose();
                    taskbarWindow = null;
                }
            }
            else
                throw new ArgumentException("The given control has not been added to the taskbar.");
        }

        /// <summary>
        /// Sets the given tabbed thumbnail preview object as being active on the taskbar tabbed thumbnails list.
        /// Call this method to keep the application and the taskbar in sync as to which window/control
        /// is currently active (or selected, in the case of tabbed application).
        /// </summary>
        /// <param name="preview">TabbedThumbnail for the specific control/indow that is currently active in the application</param>
        /// <exception cref="System.ArgumentException">If the control/window is not yet added to the tabbed thumbnails list</exception>
        public void SetActiveTab(TabbedThumbnail preview)
        {
            if (preview.WindowHandle != IntPtr.Zero)
            {
                if (tabbedThumbnailList.ContainsKey(preview.WindowHandle))
                    TaskbarWindowManager.Instance.SetActiveTab(tabbedThumbnailList[preview.WindowHandle].TaskbarWindow);
                else
                    throw new ArgumentException("The given preview has not been added to the taskbar.");
            }
            else if (preview.WindowsControl != null)
            {
                if (tabbedThumbnailListWPF.ContainsKey(preview.WindowsControl))
                    TaskbarWindowManager.Instance.SetActiveTab(tabbedThumbnailListWPF[preview.WindowsControl].TaskbarWindow);
                else
                    throw new ArgumentException("The given preview has not been added to the taskbar.");
            }
        }

        /// <summary>
        /// Sets the given window handle as being active on the taskbar tabbed thumbnails list.
        /// Call this method to keep the application and the taskbar in sync as to which window/control
        /// is currently active (or selected, in the case of tabbed application).
        /// </summary>
        /// <param name="windowHandle">Window handle for the control/window that is currently active in the application</param>
        /// <exception cref="System.ArgumentException">If the control/window is not yet added to the tabbed thumbnails list</exception>
        public void SetActiveTab(IntPtr windowHandle)
        {
            if (tabbedThumbnailList.ContainsKey(windowHandle))
                TaskbarWindowManager.Instance.SetActiveTab(tabbedThumbnailList[windowHandle].TaskbarWindow);
            else
                throw new ArgumentException("The given control has not been added to the taskbar.");
        }

        /// <summary>
        /// Sets the given Control/Form window as being active on the taskbar tabbed thumbnails list.
        /// Call this method to keep the application and the taskbar in sync as to which window/control
        /// is currently active (or selected, in the case of tabbed application).
        /// </summary>
        /// <param name="control">Control/Form that is currently active in the application</param>
        /// <exception cref="System.ArgumentException">If the control/window is not yet added to the tabbed thumbnails list</exception>
        public void SetActiveTab(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            SetActiveTab(control.Handle);
        }

        /// <summary>
        /// Sets the given WPF window as being active on the taskbar tabbed thumbnails list.
        /// Call this method to keep the application and the taskbar in sync as to which window/control
        /// is currently active (or selected, in the case of tabbed application).
        /// </summary>
        /// <param name="windowsControl">WPF control that is currently active in the application</param>
        /// <exception cref="System.ArgumentException">If the control/window is not yet added to the tabbed thumbnails list</exception>
        public void SetActiveTab(UIElement windowsControl)
        {
            if (windowsControl == null)
                throw new ArgumentNullException("windowsControl");

            if (tabbedThumbnailListWPF.ContainsKey(windowsControl))
                TaskbarWindowManager.Instance.SetActiveTab(tabbedThumbnailListWPF[windowsControl].TaskbarWindow);
            else
                throw new ArgumentException("The given control has not been added to the taskbar.");
        }

        /// <summary>
        /// Determines whether the given preview has been added to the taskbar's tabbed thumbnail list.
        /// </summary>
        /// <param name="preview">The preview to locate on the taskbar's tabbed thumbnail list</param>
        /// <returns>true if the tab is already added on the taskbar; otherwise, false.</returns>
        public bool IsThumbnailPreviewAdded(TabbedThumbnail preview)
        {
            if (preview == null)
                throw new ArgumentNullException("preview");

            if (preview.WindowHandle != IntPtr.Zero && tabbedThumbnailList.ContainsKey(preview.WindowHandle))
                return true;
            else if (preview.WindowsControl != null && tabbedThumbnailListWPF.ContainsKey(preview.WindowsControl))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether the given window has been added to the taskbar's tabbed thumbnail list.
        /// </summary>
        /// <param name="windowHandle">The window to locate on the taskbar's tabbed thumbnail list</param>
        /// <returns>true if the tab is already added on the taskbar; otherwise, false.</returns>
        public bool IsThumbnailPreviewAdded(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("windowHandle");

            if (tabbedThumbnailList.ContainsKey(windowHandle))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether the given control has been added to the taskbar's tabbed thumbnail list.
        /// </summary>
        /// <param name="control">The preview to locate on the taskbar's tabbed thumbnail list</param>
        /// <returns>true if the tab is already added on the taskbar; otherwise, false.</returns>
        public bool IsThumbnailPreviewAdded(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (tabbedThumbnailList.ContainsKey(control.Handle))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether the given control has been added to the taskbar's tabbed thumbnail list.
        /// </summary>
        /// <param name="control">The preview to locate on the taskbar's tabbed thumbnail list</param>
        /// <returns>true if the tab is already added on the taskbar; otherwise, false.</returns>
        public bool IsThumbnailPreviewAdded(UIElement control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (tabbedThumbnailListWPF.ContainsKey(control))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Invalidates all the tabbed thumbnails. This will force the Desktop Window Manager
        /// to not use the cached thumbnail or preview or aero peek and request a new one next time.
        /// </summary>
        /// <remarks>This method should not be called frequently. 
        /// Doing so can lead to poor performance as new bitmaps are created and retrieved.</remarks>
        public void InvalidateThumbnails()
        {
            // Invalidate all the previews currently in our cache.
            // This will ensure we get updated bitmaps next time
            tabbedThumbnailList.Values.ToList<TabbedThumbnail>().ForEach(thumbPreview => TaskbarWindowManager.Instance.InvalidatePreview(tabbedThumbnailList[thumbPreview.WindowHandle].TaskbarWindow));
            tabbedThumbnailListWPF.Values.ToList<TabbedThumbnail>().ForEach(thumbPreview => TaskbarWindowManager.Instance.InvalidatePreview(tabbedThumbnailListWPF[thumbPreview.WindowsControl].TaskbarWindow));

            tabbedThumbnailList.Values.ToList<TabbedThumbnail>().ForEach(thumbPreview => thumbPreview.SetImage(IntPtr.Zero));
            tabbedThumbnailListWPF.Values.ToList<TabbedThumbnail>().ForEach(thumbPreview => thumbPreview.SetImage(IntPtr.Zero));
        }

        /// <summary>
        /// Clear a clip that is already in place and return to the default display of the thumbnail.
        /// </summary>
        /// <param name="windowHandle">The handle to a window represented in the taskbar. This has to be a top-level window.</param>
        public void ClearThumbnailClip(IntPtr windowHandle)
        {
            TaskbarManager.Instance.TaskbarList.SetThumbnailClip(windowHandle, IntPtr.Zero);
        }

        /// <summary>
        /// Selects a portion of a window's client area to display as that window's thumbnail in the taskbar.
        /// </summary>
        /// <param name="windowHandle">The handle to a window represented in the taskbar. This has to be a top-level window.</param>
        /// <param name="clippingRectangle">Rectangle structure that specifies a selection within the window's client area,
        /// relative to the upper-left corner of that client area.
        /// <para>If this parameter is null, the clipping area will be cleared and the default display of the thumbnail will be used instead.</para></param>
        public void SetThumbnailClip(IntPtr windowHandle, Rectangle? clippingRectangle)
        {
            if (clippingRectangle == null)
            {
                TaskbarManager.Instance.TaskbarList.SetThumbnailClip(windowHandle, IntPtr.Zero);
                return;
            }

            CoreNativeMethods.RECT rect = new CoreNativeMethods.RECT();
            rect.left = clippingRectangle.Value.Left;
            rect.top = clippingRectangle.Value.Top;
            rect.right = clippingRectangle.Value.Right;
            rect.bottom = clippingRectangle.Value.Bottom;

            IntPtr rectPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(rect));
            try
            {
                Marshal.StructureToPtr(rect, rectPtr, true);
                TaskbarManager.Instance.TaskbarList.SetThumbnailClip(windowHandle, rectPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(rectPtr);
            }
        }

        /// <summary>
        /// Moves an existing thumbnail to a new position in the application's group.
        /// </summary>
        /// <param name="previewToChange">Preview for the window whose order is being changed. 
        /// This value is required, must already be added via AddThumbnailPreview method, and cannot be null.</param>
        /// <param name="insertBeforePreview">The preview of the tab window whose thumbnail that previewToChange is inserted to the left of. 
        /// This preview must already be added via AddThumbnailPreview. If this value is null, the previewToChange tab is added to the end of the list.
        /// </param>
        public void SetTabOrder(TabbedThumbnail previewToChange, TabbedThumbnail insertBeforePreview)
        {
            if (previewToChange == null)
                throw new ArgumentNullException("previewToChange");

            IntPtr handleToReorder = previewToChange.TaskbarWindow.WindowToTellTaskbarAbout;

            if (insertBeforePreview == null)
                TaskbarManager.Instance.TaskbarList.SetTabOrder(handleToReorder, IntPtr.Zero);
            else
            {
                IntPtr handleBefore = insertBeforePreview.TaskbarWindow.WindowToTellTaskbarAbout;

                TaskbarManager.Instance.TaskbarList.SetTabOrder(handleToReorder, handleBefore);
            }
        }
    }
}
