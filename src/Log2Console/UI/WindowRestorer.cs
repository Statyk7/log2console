using System.Drawing;
using System.Windows.Forms;
using System.Linq;

/*
 * ==============================================================
 * @ID       $Id: WindowRestorer.cs 706 2009-08-14 14:37:56Z ww $
 * @created  2009-06-01
 * @project  http://cleancode.sourceforge.net/
 * @doc      /api/csharp/CleanCode.Forms.WindowRestorer.html
 * ==============================================================
 *
 * The official license for this file is shown next.
 * Unofficially, consider this e-postcardware as well:
 * if you find this module useful, let us know via e-mail, along with
 * where in the world you are and (if applicable) your website address.
 */

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is part of the CleanCode toolbox.
 *
 * The Initial Developer of the Original Code is Michael Sorens.
 * Portions created by the Initial Developer are Copyright (C) 2009-2009
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * ***** END LICENSE BLOCK *****
 */

namespace Log2Console.UI
{
    /// <summary>
    /// Tracks a form's window state and position
    /// and enables an application to restore it upon subsequent invocations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used in conjunction with application settings
    /// (see <see href="http://msdn.microsoft.com/en-us/library/0zszyc6e.aspx">Application Settings for Windows Forms</see>)
    /// this helper class accounts for multiple monitors, changes in monitors,
    /// and whether the window is minimized or maximized at the time the application is terminated.
    /// The code skeleton below illustrates the key points involved in wiring up this class.
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>Instantiate a WindowRestorer supplying values for the window state and position
    /// at the time the application was last terminated,
    /// which come from persistent application settings.
    /// The supplied values are used as a starting point. If the application was minimized, it is restored in a normal state.
    /// If it was normal or maximized, it is restored to that same state.
    /// If the monitor configuration has changed so that the window is no longer visible, it is moved so that it is visible.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Wire up the window <b>Move</b> and <b>Resize</b> events to track changes while you use the application.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Wire up the <b>FormClosing</b> event to save the window state and position at the time the application is terminated,
    /// for use during the following invocation.
    /// </description>
    /// </item>
    /// </list>
    /// <code>
    /// private WindowRestorer windowRestorer;
    /// 
    /// public MainForm()
    /// {
    ///     . . .
    ///     windowRestorer = new WindowRestorer(this,
    ///         Properties.Settings.Default.WindowPosition,
    ///         Properties.Settings.Default.WindowState);
    /// }
    /// 
    /// protected override void OnMove(EventArgs e)
    /// {
    ///     base.OnMove(e);
    ///     if (windowRestorer != null) windowRestorer.TrackWindow();
    /// }
    /// 
    /// protected override void OnResize(EventArgs e)
    /// {
    ///     base.OnResize(e);
    ///     if (windowRestorer != null) windowRestorer.TrackWindow();
    /// }
    /// 
    /// private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    /// {
    ///     SaveUserSettings();
    ///     . . .
    /// }
    /// 
    /// private void SaveUserSettings()
    /// {
    ///     Properties.Settings.Default.WindowPosition = windowRestorer.WindowPosition;
    ///     Properties.Settings.Default.WindowState = windowRestorer.WindowState;
    ///     . . .
    ///     Properties.Settings.Default.Save();
    /// }
    /// </code>
    /// <h2>Multiple Monitor Support</h2>
    /// <para>
    /// The WindowRestorer class also provides some facilities to support multiple monitor environments.
    /// First you may enhance the form maximize button so that when you depress Control
    /// while pressing the maximize button, the form maximizes across multiple monitors
    /// instead of just the current monitor.
    /// Enabling this functionality requires adding just one line to the above code.
    /// Change the <c>OnResize</c> method to this, adding the one highlighted line:
    /// </para>
    /// <code>
    /// protected override void OnResize(EventArgs e)
    /// {
    ///     base.OnResize(e);
    ///     if (windowRestorer != null)
    ///     {
    ///         <b>windowRestorer.MultipleMonitorMaximize();</b>
    ///         windowRestorer.TrackWindow();
    ///     }
    /// }
    /// </code>
    /// <para>
    /// WindowRestorer also provides a mechanism for automatically
    /// positioning your custom subforms of your main application window onto the same monitor
    /// (unfortunately, it does not do this for the native .NET open and save file dialogs).
    /// Just invoke the static <see cref="SetSubWindowRelativeLocation"/> method
    /// right before you display your subform.
    /// </para>
    /// <para>
    /// Since CleanCode 0.9.28.
    /// </para>
    /// </remarks>
    public class WindowRestorer
    {
        private Form form;
        private bool windowInitialized;

        /// <summary>
        /// Gets the window position.
        /// </summary>
        /// <value>The window position.</value>
        public Rectangle WindowPosition { get; private set; }

        /// <summary>
        /// Gets the window state.
        /// </summary>
        /// <value>The window state.</value>
        public FormWindowState WindowState { get; private set; }

        private Rectangle saveRestoreBounds;
        private bool pseudoMaxActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowRestorer"/> class.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="persistedWindowPosition">The persisted window position.</param>
        /// <param name="persistedWindowState">The persisted window state.</param>
        public WindowRestorer(Form form, Rectangle persistedWindowPosition, FormWindowState persistedWindowState)
        {
            this.form = form;

            // initialize the tracking values
            WindowPosition = persistedWindowPosition;
            WindowState = persistedWindowState;

            // restore the window
            Restore();
        }

        /// <summary>
        /// Tracks a form's window state and position.
        /// </summary>
        /// <remarks>
        /// This remembers changes as they occur.
        /// (If you were instead to only record the state and position on closing the form,
        /// it would not work if the application was minimized or maximized.)
        /// At closing, simply transfer the <see cref="WindowPosition"/>
        /// and <see cref="WindowState"/> to your persistent setting store.
        /// </remarks>
        public void TrackWindow()
        {
            // Don't record the window setup, otherwise we lose the persistent values!
            if (!windowInitialized) { return; }

            if (form.WindowState == FormWindowState.Normal)
            {
                WindowPosition = form.DesktopBounds;
            }
            if (form.WindowState != FormWindowState.Minimized)
            {
                WindowState = form.WindowState;
            }
        }

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }

        private void Restore()
        {
            windowInitialized = false;

            // this is the default
            form.WindowState = FormWindowState.Normal;
            form.StartPosition = FormStartPosition.WindowsDefaultBounds;

            // check if the saved bounds are nonzero and visible on any screen
            if (WindowPosition != Rectangle.Empty && IsVisibleOnAnyScreen(WindowPosition))
            {
                // first set the bounds
                form.StartPosition = FormStartPosition.Manual;
                form.DesktopBounds = WindowPosition;

                // afterwards set the window state to the saved value (which could be Maximized)
                form.WindowState = WindowState;
            }
            else
            {
                // this resets the upper left corner of the window to windows standards
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;

                // we can still apply the saved size, if any
                if (WindowPosition != Rectangle.Empty)
                {
                    form.Size = WindowPosition.Size;
                }
            }

            // signal event handlers OK to process
            windowInitialized = true;
        }

        /// <summary>
        /// Sets the location of a sub-window relative to its base form
        /// and optionally a parent control on that base form,
        /// allowing for multiple monitors.
        /// </summary>
        /// <param name="targetForm">The target form.</param>
        /// <param name="parentForm">The parent (base) form.</param>
        /// <param name="parentRelativeLocation">The relative offset from the base form.</param>
        /// <param name="xOffset">An additional fixed x offset.</param>
        /// <param name="yOffset">An additional fixed y offset.</param>
        /// <remarks>
        /// <para>
        /// One can set the location relative to a specific parent control of a form
        /// or, by passing <c>parentRelativeLocation</c> as (0, 0), relative just to a form.
        /// The <c>parentRelativeLocation</c> is typically the <c>Location</c> property of the
        /// control displaying the subform, or it may be the <c>Location</c> property
        /// of one of its parents--and not necessarily the immediate parent.
        /// </para>
        /// <para>
        /// A typical usage of this method is inside your subform class.
        /// Override the <c>OnVisibleChanged</c> method so that just as
        /// the form is being displayed you set its position.
        /// That way, the current monitor showing the application
        /// will always be the monitor the subform is displayed on.
        /// Just before you display the subform with Show or ShowDialog
        /// you should set the standard <c>Form.Owner</c> property
        /// and optionally the custom <c>ParentRelativeLocation</c> property.
        /// </para>
        /// <code>
        /// public Point ParentRelativeLocation { get; set; }
        ///
        /// protected override void OnVisibleChanged(EventArgs e)
        /// {
        ///     if (Visible)
        ///     {
        ///         WindowRestorer.SetSubWindowRelativeLocation(
        ///             this, Owner, ParentRelativeLocation, 20, 50);
        ///     }
        ///     base.OnVisibleChanged(e);
        /// }
        /// </code>
        /// <para>
        /// Note that this method is used quite independently of other methods in this class
        /// but it is in this class due to being close conceptually.
        /// </para>
        /// </remarks>
        public static void SetSubWindowRelativeLocation(Form targetForm, Form parentForm, Point parentRelativeLocation, int xOffset, int yOffset)
        {
            if (parentForm == null) return;
            targetForm.Location = new Point(
                parentForm.Location.X + parentRelativeLocation.X + xOffset,
                parentForm.Location.Y + parentRelativeLocation.Y + yOffset);
            targetForm.StartPosition = FormStartPosition.Manual;
        }


        // From http://www.c-sharpcorner.com/Forums/ShowMessages.aspx?ThreadID=52
        //[DllImport("user32.dll")]
        //private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        //private const int WM_SETREDRAW = 11; 

        /// <summary>
        /// Maximizes a form over multiple monitors.
        /// </summary>
        /// <remarks>
        /// Allows for the main monitor being the leftmost
        /// (in which case its top-left corner is the top-left corner of the spread)
        /// or not the leftmost (in which case the top-left corner is negative).
        /// Assumes that monitors are laid out in a single horizontal line.
        /// Allows for differing monitor height, using the smallest so that
        /// the entire form window is visible.
        /// </remarks>
        /// <returns>true if form was resized to multiple monitors</returns>
        public bool MultipleMonitorMaximize()
        {
            bool retVal = false;
            if (form.WindowState == FormWindowState.Maximized
                && Control.ModifierKeys == Keys.Control
                && Screen.AllScreens.Length > 1)
            {
                // This reduces flicker a bit but also
                // leaves screen artifacts behind sometimes so not using it.
                //SendMessage(form.Handle, WM_SETREDRAW, false, 0);

                if (pseudoMaxActive)
                { // currently pseudo-maximized; restore down to normal
                    form.WindowState = FormWindowState.Normal;
                    form.DesktopBounds = saveRestoreBounds;
                    pseudoMaxActive = false;
                }
                else
                {
                    // pseudo-maximize to multiple monitors
                    saveRestoreBounds = form.RestoreBounds;
                    pseudoMaxActive = true;

                    Point startPoint = new Point(
                        Screen.AllScreens.Min(screen => screen.WorkingArea.Left),
                        Screen.AllScreens.Max(screen => screen.WorkingArea.Top));
                    Point endPoint = new Point(
                        Screen.AllScreens.Max(screen => screen.WorkingArea.Right),
                        Screen.AllScreens.Min(screen => screen.WorkingArea.Bottom));
                    Size newSize = new Size(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);

                    form.WindowState = FormWindowState.Normal;
                    form.Location = startPoint;
                    form.Size = newSize;
                }
                //SendMessage(form.Handle, WM_SETREDRAW, true, 0);
                retVal = true;
            }
            return retVal;
        }

    }
}