using System;
using System.Drawing;
using System.Windows.Forms;

using ControlExtenders;

using Log2Console.Log;
using Log2Console.Receiver;
using Log2Console.Settings;
using Log2Console.UI;
using System.Collections;


// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace Log2Console
{
    public partial class MainForm : Form, ILogMessageNotifiable
    {
        private DockExtender _dockExtender = null;
        private IFloaty _logDetailsPanelFloaty = null;
        private IFloaty _loggersPanelFloaty = null;

		private string _msgDetailText = String.Empty;
		private LoggerItem _lastHighlightedLogger = null;
		private LoggerItem _lastHighlightedLogMsgs = null;

		delegate void NotifyLogMsgCallback(LogMessage logMsg);
		delegate void NotifyLogMsgsCallback(LogMessage[] logMsgs);



        public MainForm()
        {
            InitializeComponent();

			// Set titles
			this.Text = AboutForm.AssemblyTitle + " - v" + AboutForm.AssemblyVersion;
            appNotifyIcon.Text = AboutForm.AssemblyTitle;

            levelComboBox.SelectedIndex = 0;


			// Init Log Manager Singleton
			LogManager.Instance.Initialize(loggerTreeView, logListView);


            _dockExtender = new DockExtender(this);

            // Dockable Log Detail View
            _logDetailsPanelFloaty = _dockExtender.Attach(logDetailPanel, logDetailToolStrip, logDetailSplitter);
            _logDetailsPanelFloaty.DontHideHandle = true;
            _logDetailsPanelFloaty.Docking += new EventHandler(floaty_Docking);

            // Dockable Logger Tree
            _loggersPanelFloaty = _dockExtender.Attach(loggerPanel, loggersToolStrip, loggerSplitter);
            _loggersPanelFloaty.DontHideHandle = true;
            _loggersPanelFloaty.Docking += new EventHandler(floaty_Docking);


            // Settings
			UserSettings.Load();
            ApplySettings(true, true);
        }

        private void floaty_Docking(object sender, EventArgs e)
        {
            // make sure the ZOrder remains intact
            logListView.BringToFront();
            this.BringToFront();
        }


        private void ApplySettings(bool noCheck, bool initReceiver)
        {
            this.Opacity = (double)UserSettings.Instance.Transparency / 100;
            this.ShowInTaskbar = !UserSettings.Instance.HideTaskbarIcon;

			this.TopMost = UserSettings.Instance.AlwaysOnTop;
            pinOnTopBtn.Checked = UserSettings.Instance.AlwaysOnTop;

			logListView.Font = UserSettings.Instance.LogListFont;
			logDetailTextBox.Font = UserSettings.Instance.LogDetailFont;
			loggerTreeView.Font = UserSettings.Instance.LoggerTreeFont;

			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Trace].Color = UserSettings.Instance.TraceLevelColor;
			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Debug].Color = UserSettings.Instance.DebugLevelColor;
			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Info].Color = UserSettings.Instance.InfoLevelColor;
			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Warn].Color = UserSettings.Instance.WarnLevelColor;
			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Error].Color = UserSettings.Instance.ErrorLevelColor;
			LogLevels.Instance.LogLevelInfos[(int)LogLevel.Fatal].Color = UserSettings.Instance.FatalLevelColor;

			levelComboBox.SelectedIndex = (int) UserSettings.Instance.LogLevelInfo.Level;

			if (logListView.ShowGroups != UserSettings.Instance.GroupLogMessages)
			{
				if (noCheck)
				{
					logListView.ShowGroups = UserSettings.Instance.GroupLogMessages;
				}
				else
				{
					DialogResult res = MessageBox.Show(
						this,
						"You changed the Message Grouping setting, the Log Message List must be cleared, OK?",
						this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

					if (res == DialogResult.OK)
					{
						ClearAll();
						logListView.ShowGroups = UserSettings.Instance.GroupLogMessages;
					}
					else
					{
						UserSettings.Instance.GroupLogMessages = !UserSettings.Instance.GroupLogMessages;
					}
				}
			}

            if (initReceiver)
            {
                if (UserSettings.Instance.Receiver == null)
                {
                    ShowErrorBox("No Receiver has been set yet!\nGo to the Settings dialog and configure one.");
                }
                else 
                {
                    try
                    {
                        UserSettings.Instance.Receiver.Initialize();
                        UserSettings.Instance.Receiver.Attach(this);
                        LogManager.Instance.SetRootLoggerName(
                            String.Format("Root [{0}]", UserSettings.Instance.Receiver));
                    }
                    catch (Exception ex)
                    {
                        try {
                            UserSettings.Instance.Receiver.Terminate();
                        } catch {}
                        UserSettings.Instance.Receiver = null;

                        ShowErrorBox("Failed to Initialize Receiver: " + ex.Message);
                    }
                }
            }
        }

        private void Quit()
		{
            UserSettings.Instance.Save();
            UserSettings.Instance.Close();

            Close();
        }

		private void ClearLogMessages()
		{
		    SetLogMessageDetail(null);
			LogManager.Instance.ClearLogMessages();
		}

		private void ClearLoggers()
        {
            SetLogMessageDetail(null);
			LogManager.Instance.ClearAll();
		}

		private void ClearAll()
		{
			ClearLogMessages();
			ClearLoggers();
		}

        protected void ShowBalloonTip(string msg)
        {
            appNotifyIcon.BalloonTipTitle = AboutForm.AssemblyTitle;
            appNotifyIcon.BalloonTipText = msg;
            appNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            appNotifyIcon.ShowBalloonTip(3000);
        }

		private void ShowErrorBox(string msg)
		{
			MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

        private void ShowSettingsForm()
        {
            IReceiver prevReceiver = UserSettings.Instance.Receiver;

			SettingsForm form = new SettingsForm(UserSettings.Instance);
			if (form.ShowDialog(this) != DialogResult.OK)
				return;

			// TODO: Manage Cancel!! (copy UserSettings obj)

            // Terminate previous receiver
            bool receiverHasChanged = (prevReceiver != UserSettings.Instance.Receiver);
            if (receiverHasChanged && (prevReceiver != null))
			{
				try
				{
					prevReceiver.Detach();
					prevReceiver.Terminate();
				}
				catch (Exception ex)
				{
					ShowErrorBox("Failed to Terminate Receiver: " + ex.Message);
				}

				prevReceiver = null;
			}

			//UserSettings.Instance = form.UserSettings;
			UserSettings.Instance.Save();
            ApplySettings(false, receiverHasChanged);
		}

        private void ShowAboutForm()
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog(this);
        }

        private void RestoreWindow()
        {
            this.Visible = true;
            this.Activate();
		}



		#region ILogMessageNotifiable Members

		/// <summary>
		/// Transforms the notification into an asynchronous call.
		/// The actual method called to add log messages is 'AddLogMessages'.
		/// </summary>
		public void Notify(LogMessage[] logMsgs)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (logListView.InvokeRequired)
			{
				NotifyLogMsgsCallback d = AddLogMessages;
				Invoke(d, new object[] { logMsgs });
			}
			else
			{
				AddLogMessages(logMsgs);
			}
		}

		/// <summary>
		/// Transforms the notification into an asynchronous call.
		/// The actual method called to add a log message is 'AddLogMessage'.
		/// </summary>
		public void Notify(LogMessage logMsg)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (logListView.InvokeRequired)
            {
				NotifyLogMsgCallback d = AddLogMessage;
                Invoke(d, new object[] { logMsg });
            }
            else
            {
				AddLogMessage(logMsg);
            }
        }

        #endregion


		/// <summary>
		/// Adds a new log message, synchronously.
		/// </summary>
		private void AddLogMessages(LogMessage[] logMsgs)
		{
			logListView.BeginUpdate();

			foreach (LogMessage msg in logMsgs)
				AddLogMessage(msg);

			logListView.EndUpdate();
		}


		/// <summary>
		/// Adds a new log message, synchronously.
		/// </summary>
		private void AddLogMessage(LogMessage logMsg)
		{
			RemovedLogMsgsHighlight();

			LogManager.Instance.ProcessLogMessage(logMsg);

			if (!this.Visible && UserSettings.Instance.NotifyNewLogWhenHidden)
				ShowBalloonTip("A new message has been received...");
		}


        private void quitBtn_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void logListView_SelectedIndexChanged(object sender, EventArgs e)
        {
			RemovedLoggerHighlight();

            LogMessageItem logMsgItem = null;
            if (logListView.SelectedItems.Count > 0)
                logMsgItem = logListView.SelectedItems[0].Tag as LogMessageItem;

            SetLogMessageDetail(logMsgItem);

			// Highlight Logger in the Tree View
			if ((logMsgItem != null) && (UserSettings.Instance.HighlightLogger))
			{
				logMsgItem.Parent.Highlight = true;
				_lastHighlightedLogger = logMsgItem.Parent;
			}
        }

        private void SetLogMessageDetail(LogMessageItem logMsgItem)
        {
            // Store the text to avoid editing without settings the control
            // as readonly... kind of ugly trick...

            if (logMsgItem == null)
            {
                _msgDetailText = String.Empty;
            }
            else
            {
                _msgDetailText = logMsgItem.Message.Message;
                logDetailTextBox.ForeColor = logMsgItem.Message.Level.Color;
            }

            logDetailTextBox.Text = _msgDetailText;
        }

		private void logDetailTextBox_TextChanged(object sender, EventArgs e)
		{
			// Disable Edition without making it Read Only (better rendering...), see above
			logDetailTextBox.Text = _msgDetailText;
		}


        private void clearBtn_Click(object sender, EventArgs e)
        {
			ClearLogMessages();
        }

        private void closeLoggersPanelBtn_Click(object sender, EventArgs e)
        {
            _dockExtender.Hide(loggerPanel);
            loggersPanelToggleBtn.Checked = false;
        }

        private void loggersPanelToggleBtn_Click(object sender, EventArgs e)
        {
            // Toggle check state
            loggersPanelToggleBtn.Checked = !loggersPanelToggleBtn.Checked;

            if (loggersPanelToggleBtn.Checked)
                _dockExtender.Show(loggerPanel);
            else
                _dockExtender.Hide(loggerPanel);
        }

        private void clearLoggersBtn_Click(object sender, EventArgs e)
        {
			ClearLoggers();
        }

        private void closeLogDetailPanelBtn_Click(object sender, EventArgs e)
        {
            _dockExtender.Hide(logDetailPanel);
            logDetailsPanelToggleBtn.Checked = false;
        }

        private void logDetailsPanelToggleBtn_Click(object sender, EventArgs e)
        {
            // Toggle check state
            logDetailsPanelToggleBtn.Checked = !logDetailsPanelToggleBtn.Checked;

            if (logDetailsPanelToggleBtn.Checked)
                _dockExtender.Show(logDetailPanel);
            else
                _dockExtender.Hide(logDetailPanel);
        }

        private void copyLogDetailBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(logDetailTextBox.Text))
                return;

            Clipboard.SetText(logDetailTextBox.Text);
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void appNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreWindow();
        }

        private void restoreTrayMenuItem_Click(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        private void settingsTrayMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void aboutTrayMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            RemovedLogMsgsHighlight();

            LogManager.Instance.HighlightSearchedText(searchTextBox.Text);
        }

        private void zoomOutLogListBtn_Click(object sender, EventArgs e)
		{
			ZoomControlFont(logListView, false);
        }

        private void zoomInLogListBtn_Click(object sender, EventArgs e)
        {
			ZoomControlFont(logListView, true);
        }

        private void zoomOutLogDetailsBtn_Click(object sender, EventArgs e)
		{
			ZoomControlFont(logDetailTextBox, false);
        }

        private void zoomInLogDetailsBtn_Click(object sender, EventArgs e)
		{
			ZoomControlFont(logDetailTextBox, true);
        }

        private void pinOnTopBtn_Click(object sender, EventArgs e)
        {
            // Toggle check state
            pinOnTopBtn.Checked = !pinOnTopBtn.Checked;

            // Save and apply setting
            UserSettings.Instance.AlwaysOnTop = pinOnTopBtn.Checked;
            this.TopMost = pinOnTopBtn.Checked;
        }

		private static void ZoomControlFont(Control ctrl, bool zoomIn)
		{
			// Limit to a minimum size
			float newSize = Math.Max(0.5f, ctrl.Font.SizeInPoints + (zoomIn ? +1 : -1));
			ctrl.Font = new Font(ctrl.Font.FontFamily, newSize);
		}

		private void loggerTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			using (new AutoWaitCursor())
			{
				(e.Node.Tag as LoggerItem).Enabled = e.Node.Checked;
			}
		}

		private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.IsHandleCreated)
				return;

			using (new AutoWaitCursor())
			{
				UserSettings.Instance.LogLevelInfo =
					LogUtils.GetLogLevelInfo((LogLevel)levelComboBox.SelectedIndex);
				LogManager.Instance.UpdateLogLevel();
			}
		}

		private void loggerTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if ((e.Node == null) || ((e.Node.Tag as LoggerItem) == null))
				return;

			if (UserSettings.Instance.HighlightLogMessages)
			{
				_lastHighlightedLogMsgs = e.Node.Tag as LoggerItem;
				_lastHighlightedLogMsgs.HighlightLogMessages = true;
			}
		}

		private void loggerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RemovedLogMsgsHighlight();
		}

		private void RemovedLogMsgsHighlight()
		{
			if (_lastHighlightedLogMsgs != null)
			{
				_lastHighlightedLogMsgs.HighlightLogMessages = false;
				_lastHighlightedLogMsgs = null;
			}
		}

		private void RemovedLoggerHighlight()
		{
			if (_lastHighlightedLogger != null)
			{
				_lastHighlightedLogger.Highlight = false;
				_lastHighlightedLogger = null;
			}
		}

    }

}
