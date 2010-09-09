using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;


namespace Microsoft.WindowsAPICodePack.Shell
{
    
    /// <summary>
    /// WPF Glass Window
    /// Inherit from this window class to enable glass on a WPF window
    /// </summary>
    public class GlassWindow : Window
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
        /// Makes the background of current window transparent from both Wpf and Windows Perspective
        /// </summary>
        public void SetAeroGlassTransparency( )
        {
            // Set the Background to transparent from Win32 perpective 
            HwndSource.FromHwnd( windowHandle ).CompositionTarget.BackgroundColor = System.Windows.Media.Colors.Transparent;

            // Set the Background to transparent from WPF perpective 
            this.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Excludes a UI element from the AeroGlass frame.
        /// </summary>
        /// <param name="element">The element to exclude.</param>
        /// <remarks>Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not 
        /// render properly on top of an AeroGlass frame. </remarks>
        public void ExcludeElementFromAeroGlass( FrameworkElement element )
        {
            if( AeroGlassCompositionEnabled )
            {
                // calculate total size of window nonclient area
                HwndSource hwndSource = PresentationSource.FromVisual( this ) as HwndSource;
                CoreNativeMethods.RECT windowRect = new CoreNativeMethods.RECT( );
                CoreNativeMethods.RECT clientRect = new CoreNativeMethods.RECT( );
                DesktopWindowManagerNativeMethods.GetWindowRect( hwndSource.Handle, ref windowRect );
                DesktopWindowManagerNativeMethods.GetClientRect( hwndSource.Handle, ref clientRect );
                Size nonClientSize =
                    new Size(
                        (double)(windowRect.right - windowRect.left) - (double)(clientRect.right - clientRect.left),
                        (double)(windowRect.bottom - windowRect.top) - (double)(clientRect.bottom - clientRect.top) );

                // calculate size of element relative to nonclient area
                GeneralTransform transform = element.TransformToAncestor( this );
                Point topLeftFrame =
                    transform.Transform( new Point( 0, 0 ) );
                Point bottomRightFrame =
                    transform.Transform(
                        new Point(
                            element.ActualWidth + nonClientSize.Width,
                            element.ActualHeight + nonClientSize.Height ) );

                // Create a margin structure
                MARGINS margins = new MARGINS( );
                margins.cxLeftWidth = (int)topLeftFrame.X;
                margins.cxRightWidth = (int)(this.ActualWidth - bottomRightFrame.X);
                margins.cyTopHeight = (int)(topLeftFrame.Y);
                margins.cyBottomHeight = (int)(this.ActualHeight - bottomRightFrame.Y);

                // Extend the Frame into client area
                DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea( windowHandle, ref margins );
            }
        }

        /// <summary>
        /// Resets the AeroGlass exclusion area.
        /// </summary>
        public void ResetAreoGlass()
        {
            MARGINS margins = new MARGINS( true );
            DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea( windowHandle, ref margins );
        }

        #endregion

        #region implementation
        private IntPtr windowHandle;

        private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            if( msg == DWMMessages.WM_DWMCOMPOSITIONCHANGED
                || msg == DWMMessages.WM_DWMNCRENDERINGCHANGED )
            {
                if( AeroGlassCompositionChanged != null )
                {
                    AeroGlassCompositionChanged.Invoke(
                        this,
                        new AeroGlassCompositionChangedEvenArgs( AeroGlassCompositionEnabled ) );
                }

                handled = true;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// OnSourceInitialized
        /// Override SourceInitialized to initialize windowHandle for this window.
        /// A valid windowHandle is available only after the sourceInitialized is completed
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            WindowInteropHelper interopHelper = new WindowInteropHelper( this );
            this.windowHandle = interopHelper.Handle;

            // add Window Proc hook to capture DWM messages
            HwndSource source = HwndSource.FromHwnd( windowHandle );
            source.AddHook( new HwndSourceHook( WndProc ) );

            ResetAreoGlass( );
        }

        #endregion
    }
}
