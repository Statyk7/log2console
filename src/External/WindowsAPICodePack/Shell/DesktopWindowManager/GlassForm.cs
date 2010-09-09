using System;
using System.Windows.Forms;
using System.Drawing;
using MS.WindowsAPICodePack.Internal;


namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Windows Glass Form
    /// Inherit from this form to be able to enable glass on Windows Form
    /// </summary>
    public class GlassForm : Form
    {
        #region properties

        /// <summary>
        /// Get determines if AeroGlass is enabled on the desktop. Set enables/disables AreoGlass on the desktop.
        /// </summary>
        public bool AeroGlassCompositionEnabled
        {
            set
            {
                DesktopWindowManagerNativeMethods.DwmEnableComposition(
                    value ? CompositionEnable.DWM_EC_ENABLECOMPOSITION : CompositionEnable.DWM_EC_DISABLECOMPOSITION );
            }
            get
            {
                return DesktopWindowManagerNativeMethods.DwmIsCompositionEnabled( );
            }
        }

        #endregion

        #region events

        /// <summary>
        /// Fires when the availability of Glass effect changes.
        /// </summary>
        public event AeroGlassCompositionChangedEvent AeroGlassCompositionChanged;

        #endregion

        #region operations

        /// <summary>
        /// Makes the background of current window transparent
        /// </summary>
        public void SetAeroGlassTransparency( )
        {
            this.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Excludes a Control from the AeroGlass frame.
        /// </summary>
        /// <param name="control">The control to exclude.</param>
        /// <remarks>Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not 
        /// render properly on top of an AeroGlass frame. </remarks>
        public void ExcludeControlFromAeroGlass( Control control )
        {
            if( AeroGlassCompositionEnabled )
            {
                Rectangle clientScreen = this.RectangleToScreen( this.ClientRectangle );
                Rectangle controlScreen = control.RectangleToScreen( control.ClientRectangle );

                MARGINS margins = new MARGINS( );
                margins.cxLeftWidth = controlScreen.Left - clientScreen.Left;
                margins.cxRightWidth = clientScreen.Right - controlScreen.Right;
                margins.cyTopHeight = controlScreen.Top - clientScreen.Top;
                margins.cyBottomHeight = clientScreen.Bottom - controlScreen.Bottom;

                // Extend the Frame into client area
                DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea( Handle, ref margins );
            }
        }

        /// <summary>
        /// Resets the AeroGlass exclusion area.
        /// </summary>
        public void ResetAreoGlass( )
        {
            if( this.Handle != IntPtr.Zero )
            {
                MARGINS margins = new MARGINS( true );
                DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea( this.Handle, ref margins );
            }
        }
        #endregion

        #region implementation
        /// <summary>
        /// Catches the DWM messages to this window and fires the appropriate event.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc( ref Message m )
        {
            if( m.Msg == DWMMessages.WM_DWMCOMPOSITIONCHANGED
                || m.Msg == DWMMessages.WM_DWMNCRENDERINGCHANGED )
            {
                if( AeroGlassCompositionChanged != null )
                {
                    AeroGlassCompositionChanged.Invoke(
                        this,
                        new AeroGlassCompositionChangedEvenArgs( AeroGlassCompositionEnabled ) );
                }
            }
            
            base.WndProc( ref m );
        }

        /// <summary>
        /// Initializes the Form for AeroGlass
        /// </summary>
        /// <param name="e">The arguments for this event</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            ResetAreoGlass( );
        }

        /// <summary>
        /// Overide OnPaint to paint the background as black.
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );

            if( DesignMode == false )
            {
                if( AeroGlassCompositionEnabled )
                {
                    // Paint the all the regions black to enable glass
                    e.Graphics.FillRectangle( Brushes.Black, this.ClientRectangle );
                }
            }
        }

        #endregion
    }
}
