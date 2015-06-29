using System.Windows.Forms;

namespace Log2Console
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.quitBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.levelComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.goToFirstLogBtn = new System.Windows.Forms.ToolStripButton();
            this.autoLogToggleBtn = new System.Windows.Forms.ToolStripButton();
            this.goToLastLogBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomOutLogListBtn = new System.Windows.Forms.ToolStripButton();
            this.zoomInLogListBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.clearBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.searchTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.logDetailsPanelToggleBtn = new System.Windows.Forms.ToolStripButton();
            this.loggersPanelToggleBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsBtn = new System.Windows.Forms.ToolStripButton();
            this.receiversBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.quickLoadBtn = new System.Windows.Forms.ToolStripButton();
            this.saveBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.pinOnTopBtn = new System.Windows.Forms.ToolStripButton();
            this.versionLabel = new System.Windows.Forms.ToolStripLabel();
            this.loggerTreeView = new System.Windows.Forms.TreeView();
            this.timeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.levelColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.threadColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.msgColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerPanel = new System.Windows.Forms.Panel();
            this.loggerInnerPanel = new System.Windows.Forms.Panel();
            this.loggersToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.closeLoggersPanelBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearLoggersBtn = new System.Windows.Forms.ToolStripButton();
            this.collapseAllBtn = new System.Windows.Forms.ToolStripButton();
            this.dactivateSourcesBtn = new System.Windows.Forms.ToolStripButton();
            this.keepHighlightBtn = new System.Windows.Forms.ToolStripButton();
            this.loggerSplitter = new System.Windows.Forms.Splitter();
            this.appNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logDetailPanel = new System.Windows.Forms.Panel();
            this.tabControlDetail = new System.Windows.Forms.TabControl();
            this.tabMessage = new System.Windows.Forms.TabPage();
            this.logDetailInnerPanel = new System.Windows.Forms.Panel();
            this.logDetailTextBox = new System.Windows.Forms.RichTextBox();
            this.logDetailToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.closeLogDetailPanelBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomOutLogDetailsBtn = new System.Windows.Forms.ToolStripButton();
            this.zoomInLogDetailsBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.copyLogDetailBtn = new System.Windows.Forms.ToolStripButton();
            this.tabExceptions = new System.Windows.Forms.TabPage();
            this.tbExceptions = new RichTextBoxLinks.RichTextBoxEx();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textEditorSourceCode = new ICSharpCode.TextEditor.TextEditorControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.lbFileName = new System.Windows.Forms.ToolStripLabel();
            this.btnOpenFileInVS = new System.Windows.Forms.ToolStripButton();
            this.logDetailSplitter = new System.Windows.Forms.Splitter();
            this.loggerTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteLoggerTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteAllLoggerTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.logListView = new Log2Console.UI.FlickerFreeListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainToolStrip.SuspendLayout();
            this.loggerPanel.SuspendLayout();
            this.loggerInnerPanel.SuspendLayout();
            this.loggersToolStrip.SuspendLayout();
            this.trayContextMenuStrip.SuspendLayout();
            this.logDetailPanel.SuspendLayout();
            this.tabControlDetail.SuspendLayout();
            this.tabMessage.SuspendLayout();
            this.logDetailInnerPanel.SuspendLayout();
            this.logDetailToolStrip.SuspendLayout();
            this.tabExceptions.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.loggerTreeContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitBtn,
            this.toolStripLabel3,
            this.levelComboBox,
            this.toolStripSeparator8,
            this.pauseBtn,
            this.toolStripSeparator15,
            this.goToFirstLogBtn,
            this.autoLogToggleBtn,
            this.goToLastLogBtn,
            this.toolStripSeparator13,
            this.zoomOutLogListBtn,
            this.zoomInLogListBtn,
            this.toolStripSeparator10,
            this.clearBtn,
            this.toolStripSeparator1,
            this.toolStripLabel4,
            this.searchTextBox,
            this.toolStripSeparator9,
            this.logDetailsPanelToggleBtn,
            this.loggersPanelToggleBtn,
            this.toolStripSeparator3,
            this.settingsBtn,
            this.receiversBtn,
            this.toolStripSeparator14,
            this.quickLoadBtn,
            this.saveBtn,
            this.toolStripSeparator4,
            this.aboutBtn,
            this.toolStripSeparator12,
            this.pinOnTopBtn,
            this.versionLabel});
            this.mainToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(1229, 25);
            this.mainToolStrip.TabIndex = 2;
            this.mainToolStrip.Text = "mainToolStrip";
            // 
            // quitBtn
            // 
            this.quitBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.quitBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quitBtn.Image = global::Log2Console.Properties.Resources.close16;
            this.quitBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.quitBtn.Name = "quitBtn";
            this.quitBtn.Size = new System.Drawing.Size(49, 22);
            this.quitBtn.Text = "Quit";
            this.quitBtn.Click += new System.EventHandler(this.quitBtn_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel3.Image = global::Log2Console.Properties.Resources.burn16;
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabel3.ToolTipText = "Log Level Filter";
            // 
            // levelComboBox
            // 
            this.levelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelComboBox.Items.AddRange(new object[] {
            "Trace",
            "Debug",
            "Info",
            "Warn",
            "Error",
            "Fatal"});
            this.levelComboBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.levelComboBox.Name = "levelComboBox";
            this.levelComboBox.Size = new System.Drawing.Size(75, 24);
            this.levelComboBox.ToolTipText = "Log Level Filter";
            this.levelComboBox.SelectedIndexChanged += new System.EventHandler(this.levelComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // pauseBtn
            // 
            this.pauseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseBtn.Image = global::Log2Console.Properties.Resources.Pause16;
            this.pauseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(23, 22);
            this.pauseBtn.ToolTipText = "Enable/Disable All Logs";
            this.pauseBtn.Click += new System.EventHandler(this.pauseBtn_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 25);
            // 
            // goToFirstLogBtn
            // 
            this.goToFirstLogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goToFirstLogBtn.Image = global::Log2Console.Properties.Resources.backward16;
            this.goToFirstLogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToFirstLogBtn.Name = "goToFirstLogBtn";
            this.goToFirstLogBtn.Size = new System.Drawing.Size(23, 22);
            this.goToFirstLogBtn.ToolTipText = "Go to First Log Message";
            this.goToFirstLogBtn.Click += new System.EventHandler(this.goToFirstLogBtn_Click);
            // 
            // autoLogToggleBtn
            // 
            this.autoLogToggleBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.autoLogToggleBtn.Image = global::Log2Console.Properties.Resources.movefile16;
            this.autoLogToggleBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.autoLogToggleBtn.Name = "autoLogToggleBtn";
            this.autoLogToggleBtn.Size = new System.Drawing.Size(23, 22);
            this.autoLogToggleBtn.ToolTipText = "Toggle Auto Log to Last Log Message";
            this.autoLogToggleBtn.Click += new System.EventHandler(this.autoLogToggleBtn_Click);
            // 
            // goToLastLogBtn
            // 
            this.goToLastLogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goToLastLogBtn.Image = global::Log2Console.Properties.Resources.forward16;
            this.goToLastLogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToLastLogBtn.Name = "goToLastLogBtn";
            this.goToLastLogBtn.Size = new System.Drawing.Size(23, 22);
            this.goToLastLogBtn.ToolTipText = "Go to Last Log Message";
            this.goToLastLogBtn.Click += new System.EventHandler(this.goToLastLogBtn_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 25);
            // 
            // zoomOutLogListBtn
            // 
            this.zoomOutLogListBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutLogListBtn.Image = global::Log2Console.Properties.Resources.zoomout16;
            this.zoomOutLogListBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutLogListBtn.Name = "zoomOutLogListBtn";
            this.zoomOutLogListBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomOutLogListBtn.ToolTipText = "Zoom Out Log List Font";
            this.zoomOutLogListBtn.Click += new System.EventHandler(this.zoomOutLogListBtn_Click);
            // 
            // zoomInLogListBtn
            // 
            this.zoomInLogListBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInLogListBtn.Image = global::Log2Console.Properties.Resources.zoomin16;
            this.zoomInLogListBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInLogListBtn.Name = "zoomInLogListBtn";
            this.zoomInLogListBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomInLogListBtn.ToolTipText = "Zoom In Log List Font";
            this.zoomInLogListBtn.Click += new System.EventHandler(this.zoomInLogListBtn_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // clearBtn
            // 
            this.clearBtn.Image = global::Log2Console.Properties.Resources.deletefile16;
            this.clearBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(54, 22);
            this.clearBtn.Text = "Clear";
            this.clearBtn.ToolTipText = "Clear Log Messages";
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel4.Image = global::Log2Console.Properties.Resources.find16;
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabel4.Text = "toolStripLabel4";
            this.toolStripLabel4.ToolTipText = "Search Text in Log Messages";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(100, 24);
            this.searchTextBox.ToolTipText = "Search Text in Log Messages";
            this.searchTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyUp);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // logDetailsPanelToggleBtn
            // 
            this.logDetailsPanelToggleBtn.Checked = true;
            this.logDetailsPanelToggleBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logDetailsPanelToggleBtn.Image = global::Log2Console.Properties.Resources.window16;
            this.logDetailsPanelToggleBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.logDetailsPanelToggleBtn.Name = "logDetailsPanelToggleBtn";
            this.logDetailsPanelToggleBtn.Size = new System.Drawing.Size(62, 22);
            this.logDetailsPanelToggleBtn.Text = "Details";
            this.logDetailsPanelToggleBtn.ToolTipText = "Show/Hide Log Details";
            this.logDetailsPanelToggleBtn.Click += new System.EventHandler(this.logDetailsPanelToggleBtn_Click);
            // 
            // loggersPanelToggleBtn
            // 
            this.loggersPanelToggleBtn.Checked = true;
            this.loggersPanelToggleBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loggersPanelToggleBtn.Image = global::Log2Console.Properties.Resources.window16;
            this.loggersPanelToggleBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loggersPanelToggleBtn.Name = "loggersPanelToggleBtn";
            this.loggersPanelToggleBtn.Size = new System.Drawing.Size(69, 22);
            this.loggersPanelToggleBtn.Text = "Loggers";
            this.loggersPanelToggleBtn.ToolTipText = "Show/Hide Loggers";
            this.loggersPanelToggleBtn.Click += new System.EventHandler(this.loggersPanelToggleBtn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // settingsBtn
            // 
            this.settingsBtn.Image = global::Log2Console.Properties.Resources.configure16;
            this.settingsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(78, 22);
            this.settingsBtn.Text = "Settings...";
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // receiversBtn
            // 
            this.receiversBtn.Image = global::Log2Console.Properties.Resources.configure16;
            this.receiversBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.receiversBtn.Name = "receiversBtn";
            this.receiversBtn.Size = new System.Drawing.Size(85, 22);
            this.receiversBtn.Text = "Receivers...";
            this.receiversBtn.Click += new System.EventHandler(this.receiversBtn_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // quickLoadBtn
            // 
            this.quickLoadBtn.Image = global::Log2Console.Properties.Resources.documentsorcopy16;
            this.quickLoadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.quickLoadBtn.Name = "quickLoadBtn";
            this.quickLoadBtn.Size = new System.Drawing.Size(100, 22);
            this.quickLoadBtn.Text = "Open Log File";
            this.quickLoadBtn.Click += new System.EventHandler(this.quickLoadBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveBtn.Image = global::Log2Console.Properties.Resources.saveas16;
            this.saveBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(23, 22);
            this.saveBtn.Text = "Export Logs...";
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // aboutBtn
            // 
            this.aboutBtn.Image = global::Log2Console.Properties.Resources.infoabout16;
            this.aboutBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(60, 22);
            this.aboutBtn.Text = "About";
            this.aboutBtn.ToolTipText = "About...";
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // pinOnTopBtn
            // 
            this.pinOnTopBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pinOnTopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pinOnTopBtn.Image = global::Log2Console.Properties.Resources.cd16;
            this.pinOnTopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pinOnTopBtn.Name = "pinOnTopBtn";
            this.pinOnTopBtn.Size = new System.Drawing.Size(23, 22);
            this.pinOnTopBtn.Text = "Pin on Top";
            this.pinOnTopBtn.Click += new System.EventHandler(this.pinOnTopBtn_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.versionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(42, 22);
            this.versionLabel.Text = "Version";
            // 
            // loggerTreeView
            // 
            this.loggerTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerTreeView.CheckBoxes = true;
            this.loggerTreeView.Indent = 19;
            this.loggerTreeView.Location = new System.Drawing.Point(0, 25);
            this.loggerTreeView.Name = "loggerTreeView";
            this.loggerTreeView.PathSeparator = ".";
            this.loggerTreeView.ShowRootLines = false;
            this.loggerTreeView.Size = new System.Drawing.Size(237, 532);
            this.loggerTreeView.TabIndex = 1;
            this.loggerTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.loggerTreeView_AfterCheck);
            this.loggerTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.loggerTreeView_AfterSelect);
            this.loggerTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.loggerTreeView_NodeMouseDoubleClick);
            this.loggerTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.loggerTreeView_MouseUp);
            // 
            // timeColumnHeader
            // 
            this.timeColumnHeader.Text = "Time";
            this.timeColumnHeader.Width = 120;
            // 
            // levelColumnHeader
            // 
            this.levelColumnHeader.Text = "Level";
            this.levelColumnHeader.Width = 48;
            // 
            // loggerColumnHeader
            // 
            this.loggerColumnHeader.Text = "Logger";
            this.loggerColumnHeader.Width = 92;
            // 
            // threadColumnHeader
            // 
            this.threadColumnHeader.Text = "Thread";
            this.threadColumnHeader.Width = 52;
            // 
            // msgColumnHeader
            // 
            this.msgColumnHeader.Text = "Message";
            this.msgColumnHeader.Width = 751;
            // 
            // loggerPanel
            // 
            this.loggerPanel.Controls.Add(this.loggerInnerPanel);
            this.loggerPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.loggerPanel.Location = new System.Drawing.Point(992, 25);
            this.loggerPanel.Name = "loggerPanel";
            this.loggerPanel.Size = new System.Drawing.Size(237, 557);
            this.loggerPanel.TabIndex = 5;
            // 
            // loggerInnerPanel
            // 
            this.loggerInnerPanel.Controls.Add(this.loggersToolStrip);
            this.loggerInnerPanel.Controls.Add(this.loggerTreeView);
            this.loggerInnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggerInnerPanel.Location = new System.Drawing.Point(0, 0);
            this.loggerInnerPanel.Name = "loggerInnerPanel";
            this.loggerInnerPanel.Size = new System.Drawing.Size(237, 557);
            this.loggerInnerPanel.TabIndex = 5;
            // 
            // loggersToolStrip
            // 
            this.loggersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.closeLoggersPanelBtn,
            this.toolStripSeparator2,
            this.clearLoggersBtn,
            this.collapseAllBtn,
            this.dactivateSourcesBtn,
            this.keepHighlightBtn});
            this.loggersToolStrip.Location = new System.Drawing.Point(0, 0);
            this.loggersToolStrip.Name = "loggersToolStrip";
            this.loggersToolStrip.Size = new System.Drawing.Size(237, 25);
            this.loggersToolStrip.TabIndex = 0;
            this.loggersToolStrip.Text = "Loggers";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel1.Text = "Loggers";
            // 
            // closeLoggersPanelBtn
            // 
            this.closeLoggersPanelBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeLoggersPanelBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeLoggersPanelBtn.Image = global::Log2Console.Properties.Resources.close16;
            this.closeLoggersPanelBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeLoggersPanelBtn.Name = "closeLoggersPanelBtn";
            this.closeLoggersPanelBtn.Size = new System.Drawing.Size(23, 22);
            this.closeLoggersPanelBtn.Text = "Close Loggers View";
            this.closeLoggersPanelBtn.Click += new System.EventHandler(this.closeLoggersPanelBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // clearLoggersBtn
            // 
            this.clearLoggersBtn.Image = global::Log2Console.Properties.Resources.delete16;
            this.clearLoggersBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearLoggersBtn.Name = "clearLoggersBtn";
            this.clearLoggersBtn.Size = new System.Drawing.Size(71, 22);
            this.clearLoggersBtn.Text = "Clear All";
            this.clearLoggersBtn.ToolTipText = "Clear All Loggers and Log Messages";
            this.clearLoggersBtn.Click += new System.EventHandler(this.clearLoggersBtn_Click);
            // 
            // collapseAllBtn
            // 
            this.collapseAllBtn.Image = global::Log2Console.Properties.Resources.collapse_all;
            this.collapseAllBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collapseAllBtn.Name = "collapseAllBtn";
            this.collapseAllBtn.Size = new System.Drawing.Size(72, 20);
            this.collapseAllBtn.Text = "Collapse";
            this.collapseAllBtn.ToolTipText = "Collapse all sources";
            this.collapseAllBtn.Click += new System.EventHandler(this.collapseAllBtn_Click);
            // 
            // dactivateSourcesBtn
            // 
            this.dactivateSourcesBtn.Image = global::Log2Console.Properties.Resources.unselect;
            this.dactivateSourcesBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dactivateSourcesBtn.Name = "dactivateSourcesBtn";
            this.dactivateSourcesBtn.Size = new System.Drawing.Size(82, 20);
            this.dactivateSourcesBtn.Text = "Deactivate";
            this.dactivateSourcesBtn.ToolTipText = "Deactivate selected sources";
            this.dactivateSourcesBtn.Click += new System.EventHandler(this.deactivatedsourcesBtn_Click);
            // 
            // keepHighlightBtn
            // 
            this.keepHighlightBtn.Image = global::Log2Console.Properties.Resources.pin;
            this.keepHighlightBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.keepHighlightBtn.Name = "keepHighlightBtn";
            this.keepHighlightBtn.Size = new System.Drawing.Size(99, 20);
            this.keepHighlightBtn.Text = "Keep selected";
            this.keepHighlightBtn.ToolTipText = "Deactivate all unhiglight sources";
            this.keepHighlightBtn.Click += new System.EventHandler(this.deactivatedUnselectSourcesBtn_Click);
            // 
            // loggerSplitter
            // 
            this.loggerSplitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.loggerSplitter.Location = new System.Drawing.Point(989, 25);
            this.loggerSplitter.Name = "loggerSplitter";
            this.loggerSplitter.Size = new System.Drawing.Size(3, 557);
            this.loggerSplitter.TabIndex = 6;
            this.loggerSplitter.TabStop = false;
            // 
            // appNotifyIcon
            // 
            this.appNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.appNotifyIcon.ContextMenuStrip = this.trayContextMenuStrip;
            this.appNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("appNotifyIcon.Icon")));
            this.appNotifyIcon.Text = "appNotifyIcon";
            this.appNotifyIcon.Visible = true;
            this.appNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.appNotifyIcon_MouseDoubleClick);
            // 
            // trayContextMenuStrip
            // 
            this.trayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreTrayMenuItem,
            this.toolStripSeparator5,
            this.settingsTrayMenuItem,
            this.aboutTrayMenuItem,
            this.toolStripSeparator6,
            this.exitTrayMenuItem});
            this.trayContextMenuStrip.Name = "trayContextMenuStrip";
            this.trayContextMenuStrip.Size = new System.Drawing.Size(189, 104);
            // 
            // restoreTrayMenuItem
            // 
            this.restoreTrayMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.restoreTrayMenuItem.Name = "restoreTrayMenuItem";
            this.restoreTrayMenuItem.Size = new System.Drawing.Size(188, 22);
            this.restoreTrayMenuItem.Text = "Restore";
            this.restoreTrayMenuItem.Click += new System.EventHandler(this.restoreTrayMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(185, 6);
            // 
            // settingsTrayMenuItem
            // 
            this.settingsTrayMenuItem.Name = "settingsTrayMenuItem";
            this.settingsTrayMenuItem.Size = new System.Drawing.Size(188, 22);
            this.settingsTrayMenuItem.Text = "Settings...";
            this.settingsTrayMenuItem.Click += new System.EventHandler(this.settingsTrayMenuItem_Click);
            // 
            // aboutTrayMenuItem
            // 
            this.aboutTrayMenuItem.Name = "aboutTrayMenuItem";
            this.aboutTrayMenuItem.Size = new System.Drawing.Size(188, 22);
            this.aboutTrayMenuItem.Text = "About Log2Console...";
            this.aboutTrayMenuItem.Click += new System.EventHandler(this.aboutTrayMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(185, 6);
            // 
            // exitTrayMenuItem
            // 
            this.exitTrayMenuItem.Name = "exitTrayMenuItem";
            this.exitTrayMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exitTrayMenuItem.Text = "Exit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // logDetailPanel
            // 
            this.logDetailPanel.Controls.Add(this.tabControlDetail);
            this.logDetailPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logDetailPanel.Location = new System.Drawing.Point(0, 388);
            this.logDetailPanel.Name = "logDetailPanel";
            this.logDetailPanel.Size = new System.Drawing.Size(989, 194);
            this.logDetailPanel.TabIndex = 7;
            // 
            // tabControlDetail
            // 
            this.tabControlDetail.Controls.Add(this.tabMessage);
            this.tabControlDetail.Controls.Add(this.tabExceptions);
            this.tabControlDetail.Controls.Add(this.tabSource);
            this.tabControlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDetail.Location = new System.Drawing.Point(0, 0);
            this.tabControlDetail.Name = "tabControlDetail";
            this.tabControlDetail.SelectedIndex = 0;
            this.tabControlDetail.Size = new System.Drawing.Size(989, 194);
            this.tabControlDetail.TabIndex = 2;
            // 
            // tabMessage
            // 
            this.tabMessage.Controls.Add(this.logDetailInnerPanel);
            this.tabMessage.Location = new System.Drawing.Point(4, 22);
            this.tabMessage.Name = "tabMessage";
            this.tabMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tabMessage.Size = new System.Drawing.Size(981, 168);
            this.tabMessage.TabIndex = 0;
            this.tabMessage.Text = "Message Details";
            this.tabMessage.UseVisualStyleBackColor = true;
            // 
            // logDetailInnerPanel
            // 
            this.logDetailInnerPanel.Controls.Add(this.logDetailTextBox);
            this.logDetailInnerPanel.Controls.Add(this.logDetailToolStrip);
            this.logDetailInnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logDetailInnerPanel.Location = new System.Drawing.Point(3, 3);
            this.logDetailInnerPanel.Name = "logDetailInnerPanel";
            this.logDetailInnerPanel.Size = new System.Drawing.Size(975, 162);
            this.logDetailInnerPanel.TabIndex = 1;
            // 
            // logDetailTextBox
            // 
            this.logDetailTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.logDetailTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logDetailTextBox.Location = new System.Drawing.Point(0, 25);
            this.logDetailTextBox.Name = "logDetailTextBox";
            this.logDetailTextBox.ReadOnly = true;
            this.logDetailTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logDetailTextBox.Size = new System.Drawing.Size(975, 137);
            this.logDetailTextBox.TabIndex = 0;
            this.logDetailTextBox.Text = "";
            // 
            // logDetailToolStrip
            // 
            this.logDetailToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.closeLogDetailPanelBtn,
            this.toolStripSeparator11,
            this.zoomOutLogDetailsBtn,
            this.zoomInLogDetailsBtn,
            this.toolStripSeparator7,
            this.copyLogDetailBtn});
            this.logDetailToolStrip.Location = new System.Drawing.Point(0, 0);
            this.logDetailToolStrip.Name = "logDetailToolStrip";
            this.logDetailToolStrip.Size = new System.Drawing.Size(975, 25);
            this.logDetailToolStrip.TabIndex = 1;
            this.logDetailToolStrip.Text = "toolStrip2";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(96, 22);
            this.toolStripLabel2.Text = "Message Details";
            // 
            // closeLogDetailPanelBtn
            // 
            this.closeLogDetailPanelBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeLogDetailPanelBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeLogDetailPanelBtn.Image = global::Log2Console.Properties.Resources.close16;
            this.closeLogDetailPanelBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeLogDetailPanelBtn.Name = "closeLogDetailPanelBtn";
            this.closeLogDetailPanelBtn.Size = new System.Drawing.Size(23, 22);
            this.closeLogDetailPanelBtn.Text = "Close Log Detail Panel";
            this.closeLogDetailPanelBtn.Click += new System.EventHandler(this.closeLogDetailPanelBtn_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // zoomOutLogDetailsBtn
            // 
            this.zoomOutLogDetailsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutLogDetailsBtn.Image = global::Log2Console.Properties.Resources.zoomout16;
            this.zoomOutLogDetailsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutLogDetailsBtn.Name = "zoomOutLogDetailsBtn";
            this.zoomOutLogDetailsBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomOutLogDetailsBtn.ToolTipText = "Zoom Out Log Details Font";
            this.zoomOutLogDetailsBtn.Click += new System.EventHandler(this.zoomOutLogDetailsBtn_Click);
            // 
            // zoomInLogDetailsBtn
            // 
            this.zoomInLogDetailsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInLogDetailsBtn.Image = global::Log2Console.Properties.Resources.zoomin16;
            this.zoomInLogDetailsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInLogDetailsBtn.Name = "zoomInLogDetailsBtn";
            this.zoomInLogDetailsBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomInLogDetailsBtn.ToolTipText = "Zoom In Log Details Font";
            this.zoomInLogDetailsBtn.Click += new System.EventHandler(this.zoomInLogDetailsBtn_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // copyLogDetailBtn
            // 
            this.copyLogDetailBtn.Image = global::Log2Console.Properties.Resources.documentsorcopy16;
            this.copyLogDetailBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyLogDetailBtn.Name = "copyLogDetailBtn";
            this.copyLogDetailBtn.Size = new System.Drawing.Size(55, 22);
            this.copyLogDetailBtn.Text = "Copy";
            this.copyLogDetailBtn.Click += new System.EventHandler(this.copyLogDetailBtn_Click);
            // 
            // tabExceptions
            // 
            this.tabExceptions.Controls.Add(this.tbExceptions);
            this.tabExceptions.Location = new System.Drawing.Point(4, 22);
            this.tabExceptions.Name = "tabExceptions";
            this.tabExceptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabExceptions.Size = new System.Drawing.Size(981, 168);
            this.tabExceptions.TabIndex = 2;
            this.tabExceptions.Text = "Exceptions";
            this.tabExceptions.UseVisualStyleBackColor = true;
            // 
            // tbExceptions
            // 
            this.tbExceptions.BackColor = System.Drawing.SystemColors.Window;
            this.tbExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbExceptions.Location = new System.Drawing.Point(3, 3);
            this.tbExceptions.Name = "tbExceptions";
            this.tbExceptions.ReadOnly = true;
            this.tbExceptions.Size = new System.Drawing.Size(975, 162);
            this.tbExceptions.TabIndex = 0;
            this.tbExceptions.Text = "";
            this.tbExceptions.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.TbExceptionsLinkClicked);
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.panel1);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabSource.Size = new System.Drawing.Size(981, 168);
            this.tabSource.TabIndex = 1;
            this.tabSource.Text = "Source Code";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textEditorSourceCode);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(975, 162);
            this.panel1.TabIndex = 1;
            // 
            // textEditorSourceCode
            // 
            this.textEditorSourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorSourceCode.IsReadOnly = false;
            this.textEditorSourceCode.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
            this.textEditorSourceCode.Location = new System.Drawing.Point(0, 25);
            this.textEditorSourceCode.Name = "textEditorSourceCode";
            this.textEditorSourceCode.Size = new System.Drawing.Size(975, 137);
            this.textEditorSourceCode.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.lbFileName,
            this.btnOpenFileInVS});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(975, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(29, 22);
            this.toolStripLabel5.Text = "File:";
            // 
            // lbFileName
            // 
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(0, 22);
            // 
            // btnOpenFileInVS
            // 
            this.btnOpenFileInVS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenFileInVS.Image = global::Log2Console.Properties.Resources.saveas16;
            this.btnOpenFileInVS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFileInVS.Name = "btnOpenFileInVS";
            this.btnOpenFileInVS.Size = new System.Drawing.Size(23, 22);
            this.btnOpenFileInVS.Text = "Export Logs...";
            this.btnOpenFileInVS.Click += new System.EventHandler(this.btnOpenFileInVS_Click);
            // 
            // logDetailSplitter
            // 
            this.logDetailSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logDetailSplitter.Location = new System.Drawing.Point(0, 385);
            this.logDetailSplitter.Name = "logDetailSplitter";
            this.logDetailSplitter.Size = new System.Drawing.Size(989, 3);
            this.logDetailSplitter.TabIndex = 8;
            this.logDetailSplitter.TabStop = false;
            // 
            // loggerTreeContextMenuStrip
            // 
            this.loggerTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteLoggerTreeMenuItem,
            this.toolStripSeparator16,
            this.deleteAllLoggerTreeMenuItem});
            this.loggerTreeContextMenuStrip.Name = "loggerTreeContextMenuStrip";
            this.loggerTreeContextMenuStrip.Size = new System.Drawing.Size(164, 54);
            // 
            // deleteLoggerTreeMenuItem
            // 
            this.deleteLoggerTreeMenuItem.Name = "deleteLoggerTreeMenuItem";
            this.deleteLoggerTreeMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteLoggerTreeMenuItem.Text = "Clear Logger";
            this.deleteLoggerTreeMenuItem.Click += new System.EventHandler(this.deleteLoggerTreeMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteAllLoggerTreeMenuItem
            // 
            this.deleteAllLoggerTreeMenuItem.Name = "deleteAllLoggerTreeMenuItem";
            this.deleteAllLoggerTreeMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteAllLoggerTreeMenuItem.Text = "Clear All Loggers";
            this.deleteAllLoggerTreeMenuItem.Click += new System.EventHandler(this.deleteAllLoggerTreeMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "*.csv";
            this.openFileDialog1.Filter = "CSV file|*.csv|All files|*.*";
            this.openFileDialog1.Title = "Open Log File";
            // 
            // logListView
            // 
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader5});
            this.logListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListView.FullRowSelect = true;
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logListView.HideSelection = false;
            this.logListView.Location = new System.Drawing.Point(0, 25);
            this.logListView.Name = "logListView";
            this.logListView.ShowItemToolTips = true;
            this.logListView.Size = new System.Drawing.Size(989, 360);
            this.logListView.TabIndex = 0;
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            this.logListView.SelectedIndexChanged += new System.EventHandler(this.logListView_SelectedIndexChanged);
            this.logListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logListView_KeyDown);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Nr";
            this.columnHeader6.Width = 40;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Level";
            this.columnHeader2.Width = 48;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Logger";
            this.columnHeader3.Width = 92;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Thread";
            this.columnHeader4.Width = 52;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "CallSiteClass";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "CallSiteMethod";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Message";
            this.columnHeader5.Width = 540;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1229, 582);
            this.Controls.Add(this.logListView);
            this.Controls.Add(this.logDetailSplitter);
            this.Controls.Add(this.logDetailPanel);
            this.Controls.Add(this.loggerSplitter);
            this.Controls.Add(this.loggerPanel);
            this.Controls.Add(this.mainToolStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Log2Console";
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.loggerPanel.ResumeLayout(false);
            this.loggerInnerPanel.ResumeLayout(false);
            this.loggerInnerPanel.PerformLayout();
            this.loggersToolStrip.ResumeLayout(false);
            this.loggersToolStrip.PerformLayout();
            this.trayContextMenuStrip.ResumeLayout(false);
            this.logDetailPanel.ResumeLayout(false);
            this.tabControlDetail.ResumeLayout(false);
            this.tabMessage.ResumeLayout(false);
            this.logDetailInnerPanel.ResumeLayout(false);
            this.logDetailInnerPanel.PerformLayout();
            this.logDetailToolStrip.ResumeLayout(false);
            this.logDetailToolStrip.PerformLayout();
            this.tabExceptions.ResumeLayout(false);
            this.tabSource.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.loggerTreeContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton clearBtn;
        private System.Windows.Forms.ToolStripButton quitBtn;
        private System.Windows.Forms.ColumnHeader timeColumnHeader;
        private System.Windows.Forms.ColumnHeader levelColumnHeader;
        private System.Windows.Forms.ColumnHeader loggerColumnHeader;
        private System.Windows.Forms.ColumnHeader threadColumnHeader;
        private System.Windows.Forms.ColumnHeader msgColumnHeader;
        private Log2Console.UI.FlickerFreeListView logListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TreeView loggerTreeView;
        private System.Windows.Forms.Panel loggerPanel;
        private System.Windows.Forms.Panel loggerInnerPanel;
        private System.Windows.Forms.Splitter loggerSplitter;
        private System.Windows.Forms.ToolStrip loggersToolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton closeLoggersPanelBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton loggersPanelToggleBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton clearLoggersBtn;
		private System.Windows.Forms.NotifyIcon appNotifyIcon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton aboutBtn;
        private System.Windows.Forms.ToolStripButton settingsBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip trayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem restoreTrayMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutTrayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsTrayMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem exitTrayMenuItem;
        private System.Windows.Forms.Panel logDetailPanel;
        private System.Windows.Forms.Splitter logDetailSplitter;
        private System.Windows.Forms.Panel logDetailInnerPanel;
        private System.Windows.Forms.RichTextBox logDetailTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton closeLogDetailPanelBtn;
        private System.Windows.Forms.ToolStripButton logDetailsPanelToggleBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton copyLogDetailBtn;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox levelComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton zoomOutLogListBtn;
        private System.Windows.Forms.ToolStripButton zoomInLogListBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton zoomOutLogDetailsBtn;
        private System.Windows.Forms.ToolStripButton zoomInLogDetailsBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton pinOnTopBtn;
        private System.Windows.Forms.ToolStripButton pauseBtn;
        private System.Windows.Forms.ToolStripButton receiversBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripButton saveBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripLabel versionLabel;
        private System.Windows.Forms.ToolStripButton autoLogToggleBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton goToFirstLogBtn;
        private System.Windows.Forms.ToolStripButton goToLastLogBtn;
        private System.Windows.Forms.ContextMenuStrip loggerTreeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteLoggerTreeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem deleteAllLoggerTreeMenuItem;
        private System.Windows.Forms.TabControl tabControlDetail;
        private System.Windows.Forms.TabPage tabMessage;
        private System.Windows.Forms.TabPage tabSource;
        private ICSharpCode.TextEditor.TextEditorControl textEditorSourceCode;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private ToolStripLabel lbFileName;
        private ToolStrip logDetailToolStrip;
        private ToolStripButton btnOpenFileInVS;
        private TabPage tabExceptions;
        private RichTextBoxLinks.RichTextBoxEx tbExceptions;
        private ToolStripButton quickLoadBtn;
        private OpenFileDialog openFileDialog1;
        private ToolStripButton dactivateSourcesBtn;
        private ToolStripButton collapseAllBtn;
        private ToolStripButton keepHighlightBtn;
    }
}

