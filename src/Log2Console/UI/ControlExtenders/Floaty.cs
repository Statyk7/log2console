// Copyright 2007 Herre Kuijpers - <herre@xs4all.nl>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ControlExtenders
{
    /// <summary>
    /// this is the publicly exposed interface of the floating window (floaty)
    /// add more methods/properties here for your own needs, so these are exposed to the client
    /// the main goal is to keep the floaty form internal
    /// </summary>
    public interface IFloaty
    {
        /// <summary>
        /// show the floaty 
        /// </summary>
        void Show();

        /// <summary>
        /// hide the floaty
        /// </summary>
        void Hide();

        /// <summary>
        /// set a caption for the floaty
        /// </summary>
        String Text {get; set; }

        /// <summary>
        /// indicates if a floaty may dock only on the host docking control (e.g. the form)
        /// and not inside other floaties
        /// </summary>
        bool DockOnHostOnly { get; set; }

        /// <summary>
        /// indicates if a floaty may dock on the inside or on the outside of a form/control
        /// default is true
        /// </summary>
        bool DockOnInside { get; set; }

        /// <summary>
        /// indicates that the handle of a floaty will not be hidden
        /// default is false
        /// </summary>
        bool DontHideHandle { get; set; }


        event EventHandler Docking;
    }

    /// <summary>
    /// this class contains basically all the logic for making a control floating and dockable
    /// note that it is an internal class, only it's IFloaty interface is exposed to the client
    /// </summary>
    internal sealed class Floaty : Form, IFloaty
    {
        #region a teeny weeny tiny bit of API functions used
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;

        // NOTE: I don't like using API's in .Net... so I try to avoid them if possible.
        // this time there was no way around it.

        // this function is used to be able to send some very specific (uncommon) messages
        // to the floaty forms. It is used particularly to switch between start dragging a docked panel
        // to dragging a floaty form.
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);
        #endregion private members

        #region private members

        // this is the orgigional state of the panel. This state is used to reset a control to its
        // origional state if it was floating
        private DockState _dockState;

        // this is a flag that indicates if a control can start floating
        private bool _startFloating;

        // indicates if the container is floating or docked
        private bool _isFloating; 

        // this is the dockmananger that manages this floaty.
        private DockExtender _dockExtender;

        private bool _dockOnHostOnly;
        private bool _dockOnInside;
        private bool _dontHideHandle;

        #endregion private members

        #region initialization
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="DockExtender">requires the DockExtender</param>
        public Floaty(DockExtender DockExtender)
        {
            _dockExtender = DockExtender;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Floaty
            // 
            this.ClientSize = new System.Drawing.Size(178, 122);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "Floaty";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);
            this._dockOnInside = true;
            this._dockOnHostOnly = true; // keep it simple for now
            this._dontHideHandle = false;
        }

        #endregion initialization

        #region properties
        internal DockState DockState 
        {
            get { return _dockState; }
        }

        public bool DockOnHostOnly
        {
            get { return _dockOnHostOnly; }
            set { _dockOnHostOnly = value; }
        }

        public bool DockOnInside
        {
            get { return _dockOnInside; }
            set { _dockOnInside = value; }
        }

        public bool DontHideHandle
        {
            get { return _dontHideHandle; }
            set { _dontHideHandle = value; }
        }
        
        #endregion properties

        #region overrides
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCLBUTTONDBLCLK) // doubleclicked on border, so reset.
            {
                DockFloaty();
            }
            base.WndProc(ref m);
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
        }

        protected override void OnResize(EventArgs e)
        {
            
            base.OnResize(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            
            if (_dockExtender.Overlay.Visible == true && _dockExtender.Overlay.DockHostControl != null) //ok found new docking position
            {
                _dockState.OrgDockingParent = _dockExtender.Overlay.DockHostControl;
                _dockState.OrgBounds = _dockState.Container.RectangleToClient(_dockExtender.Overlay.Bounds);
                _dockState.OrgDockStyle = _dockExtender.Overlay.Dock;
                _dockExtender.Overlay.Hide();
                DockFloaty(); // dock the container
            }
            _dockExtender.Overlay.DockHostControl = null;
            _dockExtender.Overlay.Hide();
            base.OnResizeEnd(e);
        }

        protected override void OnMove(EventArgs e)
        {
            if (IsDisposed) return;

            Point pt = Cursor.Position;
            Point pc = PointToClient(pt);
            if (pc.Y < -21 || pc.Y > 0) return;
            if (pc.X < -1 || pc.X > Width) return;

            Control t = _dockExtender.FindDockHost(this, pt);
            if (t == null) 
            {
                _dockExtender.Overlay.Hide();
            }
            else
            {
                SetOverlay(t, pt);
            }
            base.OnMove(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide(); // hide but don't close
            base.OnClosing(e);
        }

        #endregion overrides

        #region public methods (implements IFloaty)
        // override base method, this control only allows one way of showing.
        public new void Show()
        {
            if (!this.Visible && _isFloating)
                base.Show(_dockState.OrgDockHost);

            _dockState.Container.Show();
        }

        public new void Hide()
        {
            if (this.Visible)
                base.Hide();

            _dockState.Container.Hide();
        }

        // this this member
        public new void Show(IWin32Window win)
        {
            Show();
        }
        #endregion

        #region helper functions - this contains most of the logic

        /// <summary>
        /// determines the client area of the control. The area of docked controls are excluded
        /// </summary>
        /// <param name="c">the control to which to determine the client area</param>
        /// <returns>returns the docking area in screen coordinates</returns>
        private Rectangle GetDockingArea(Control c)
        {
            Rectangle r = c.Bounds;
           
            if (c.Parent != null)
                r = c.Parent.RectangleToScreen(r);

            Rectangle rc = c.ClientRectangle;

            int borderwidth = (r.Width - rc.Width) / 2;
            r.X += borderwidth;
            r.Y += (r.Height - rc.Height) - borderwidth;

            if (!_dockOnInside)
            {
                rc.X += r.X;
                rc.Y += r.Y;
                return rc;
            }

            foreach (Control cs in c.Controls)
            {
                if (!cs.Visible) continue;
                switch (cs.Dock)
                {
                    case DockStyle.Left:
                        rc.X += cs.Width;
                        rc.Width -= cs.Width;
                        break;
                    case DockStyle.Right:
                            rc.Width -= cs.Width;
                        break;
                    case DockStyle.Top:
                        rc.Y += cs.Height;
                        rc.Height -= cs.Height;
                        break;
                    case DockStyle.Bottom:
                            rc.Height -= cs.Height;
                        break;
                    default:
                        break;
                }
            }
            rc.X += r.X;
            rc.Y += r.Y;

            //Console.WriteLine("Client = " + c.Name + " " + rc.ToString());

            return rc;
        }

        /// <summary>
        /// This method will check if the overlay needs to be displayed or not
        /// for display it will position the overlay
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p">position of cursor in screen coordinates</param>
        private void SetOverlay(Control c, Point pc)
        {

            Rectangle r = GetDockingArea(c);
            Rectangle rc = r;

            //determine relative coordinates
            float rx = (pc.X - r.Left) / (float)(r.Width);
            float ry = (pc.Y - r.Top) / (float)(r.Height);

            //Console.WriteLine("Moving over " + c.Name + " " +  rx.ToString() + "," + ry.ToString());

            _dockExtender.Overlay.Dock = DockStyle.None; // keep floating

            // this section determines when the overlay is to be displayed.
            // it depends on the position of the mouse cursor on the client area.
            // the overlay is currently only shown if the mouse is moving over either the Northern, Western, 
            // Southern or Eastern parts of the client area.
            // when the mouse is in the center or in the NE, NW, SE or SW, no overlay preview is displayed, hence
            // allowing the user to dock the container.

            // dock to left, checks the Western area
            if (rx > 0 && rx < ry && rx < 0.25 && ry < 0.75 && ry > 0.25)
            {
                r.Width = r.Width / 2;
                if (r.Width > this.Width)
                    r.Width = this.Width;

                _dockExtender.Overlay.Dock = DockStyle.Left; // dock to left
            }

            // dock to the right, checks the Easter area
            if (rx < 1 && rx > ry && rx > 0.75 && ry < 0.75 && ry > 0.25)
            {
                r.Width = r.Width / 2;
                if (r.Width > this.Width)
                    r.Width = this.Width;
                r.X = rc.X + rc.Width - r.Width;
                _dockExtender.Overlay.Dock = DockStyle.Right;
            }

            // dock to top, checks the Northern area
            if (ry > 0 && ry < rx && ry < 0.25 && rx < 0.75 && rx > 0.25)
            {
                r.Height = r.Height / 2;
                if (r.Height > this.Height)
                    r.Height = this.Height;
                _dockExtender.Overlay.Dock = DockStyle.Top;
            }

            // dock to the bottom, checks the Southern area
            if (ry < 1 && ry > rx && ry > 0.75 && rx < 0.75 && rx > 0.25)
            {
                r.Height = r.Height / 2;
                if (r.Height > this.Height)
                    r.Height = this.Height;
                r.Y = rc.Y + rc.Height - r.Height;
                _dockExtender.Overlay.Dock = DockStyle.Bottom;
            }
            if (_dockExtender.Overlay.Dock != DockStyle.None)
                _dockExtender.Overlay.Bounds = r;
            else
                _dockExtender.Overlay.Hide();

            if (!_dockExtender.Overlay.Visible && _dockExtender.Overlay.Dock != DockStyle.None)
            {
                _dockExtender.Overlay.DockHostControl = c;
                _dockExtender.Overlay.Show(_dockState.OrgDockHost);
                BringToFront();
            }
        }

        internal void Attach(DockState dockState)
        {
            // track the handle's mouse movements
            _dockState = dockState;
            Text = _dockState.Handle.Text;
            _dockState.Handle.MouseMove += new MouseEventHandler(Handle_MouseMove);
            _dockState.Handle.MouseHover += new EventHandler(Handle_MouseHover);
            _dockState.Handle.MouseLeave += new EventHandler(Handle_MouseLeave);
        }

        /// <summary>
        /// makes the docked control floatable in this Floaty form
        /// </summary>
        /// <param name="dockState"></param>
        /// <param name="offsetx"></param>
        /// <param name="offsety"></param>
        private void MakeFloatable(DockState dockState, int offsetx, int offsety)
        {
            Point ps = Cursor.Position;
            _dockState = dockState;
            Text = _dockState.Handle.Text;

            Size sz = _dockState.Container.Size;
            if (_dockState.Container.Equals(_dockState.Handle))
            {
                sz.Width += 18;
                sz.Height += 28;
            }
            if (sz.Width > 600) sz.Width = 600;
            if (sz.Height > 600) sz.Height = 600;



            _dockState.OrgDockingParent = _dockState.Container.Parent;
            _dockState.OrgBounds = _dockState.Container.Bounds;
            _dockState.OrgDockStyle = _dockState.Container.Dock;
            //_dockState.OrgDockingParent.Controls.Remove(_dockState.Container);
            //Controls.Add(_dockState.Container);
            if (!_dontHideHandle)
                _dockState.Handle.Hide();
            _dockState.Container.Parent = this;
            _dockState.Container.Dock = DockStyle.Fill;
            //_dockState.Handle.Visible = false; // hide it for now
            if (_dockState.Splitter != null)
            {
                _dockState.Splitter.Visible = false; // hide splitter
                _dockState.Splitter.Parent = this;
            }
            // allow redraw of floaty and container
            //Application.DoEvents();  

            // this is kind of tricky
            // disable the mousemove events of the handle
            SendMessage(_dockState.Handle.Handle.ToInt32(), WM_LBUTTONUP, 0, 0);
            ps.X -= offsetx;
            ps.Y -= offsety;


            this.Bounds = new Rectangle(ps, sz);
            _isFloating = true;
            Show();
            // enable the mousemove events of the new floating form, start dragging the form immediately
            SendMessage(this.Handle.ToInt32(), WM_SYSCOMMAND, SC_MOVE | 0x02, 0);
        }

        /// <summary>
        /// this will dock the floaty control
        /// </summary>
        private void DockFloaty()
        {
            // bring dockhost to front first to prevent flickering
            _dockState.OrgDockHost.TopLevelControl.BringToFront();
            this.Hide();
            _dockState.Container.Visible = false; // hide it temporarely
            _dockState.Container.Parent = _dockState.OrgDockingParent;
            _dockState.Container.Dock = _dockState.OrgDockStyle;
            _dockState.Container.Bounds = _dockState.OrgBounds;
            _dockState.Handle.Visible = true; // show handle again
            _dockState.Container.Visible = true; // it's good, show it

            if (_dockOnInside)
                _dockState.Container.BringToFront(); // set to front

            //show splitter
            if (_dockState.Splitter != null && _dockState.OrgDockStyle != DockStyle.Fill && _dockState.OrgDockStyle != DockStyle.None)
            {
                _dockState.Splitter.Parent = _dockState.OrgDockingParent;
                _dockState.Splitter.Dock = _dockState.OrgDockStyle;
                _dockState.Splitter.Visible = true; // show splitter

                if (_dockOnInside)
                    _dockState.Splitter.BringToFront();
                else
                    _dockState.Splitter.SendToBack();
            }

            if (!_dockOnInside)
                _dockState.Container.SendToBack(); // set to back

            _isFloating = false;

            if (Docking != null)
                Docking.Invoke(this, new EventArgs());
        }

        private void DetachHandle()
        {
            _dockState.Handle.MouseMove -= new MouseEventHandler(Handle_MouseMove);
            _dockState.Handle.MouseHover -= new EventHandler(Handle_MouseHover);
            _dockState.Handle.MouseLeave -= new EventHandler(Handle_MouseLeave);
            _dockState.Container = null;
            _dockState.Handle = null;
        }

        #endregion helper functions

        #region Container Handle tracking methods
        void Handle_MouseHover(object sender, EventArgs e)
        {
            _startFloating = true;
        }

        void Handle_MouseLeave(object sender, EventArgs e)
        {
            _startFloating = false;
        }

        void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _startFloating)
            {
                Point ps = _dockState.Handle.PointToScreen(new Point(e.X, e.Y));
                MakeFloatable(_dockState, e.X, e.Y);
            }
        }
        #endregion Container Handle tracking methods


        #region events

        public event EventHandler Docking = null;

        #endregion
    }

    /// <summary>
    /// define a Floaty collection used for enumerating all defined floaties
    /// </summary>
    public class Floaties : List<IFloaty>
    {
        public IFloaty Find(Control container)
        {
            foreach (Floaty f in this)
            {
                if (f.DockState.Container.Equals(container))
                    return f;
            }
            return null;
        }
    }

}
