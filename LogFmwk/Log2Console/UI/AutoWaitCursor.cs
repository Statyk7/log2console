using System;
using System.Windows.Forms;


namespace Log2Console.UI
{
	/// <summary>
	/// Auto Wait Cursor utility class.
	/// Usage:
	/// using (new AutoWaitCursor())
	/// { ...long task... }
	/// </summary>
	public sealed class AutoWaitCursor : IDisposable
	{
		public AutoWaitCursor()
		{
			Enabled = true;
		}

		public void Dispose()
		{
			Enabled = false;
		}

		public static bool Enabled
		{
			get { return Application.UseWaitCursor; }
			set
			{
				if (value == Application.UseWaitCursor) return;
				Application.UseWaitCursor = value;
				Form f = Form.ActiveForm;
				if (f != null && f.Handle != null)   // Send WM_SETCURSOR
					SendMessage(f.Handle, 0x20, f.Handle, (IntPtr)1);
			}
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
	}
}
