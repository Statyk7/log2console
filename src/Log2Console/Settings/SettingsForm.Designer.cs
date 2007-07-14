namespace Log2Console.Settings
{
    partial class SettingsForm
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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.settingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.settingsTabControl = new System.Windows.Forms.TabControl();
            this.uiSettingsTabPage = new System.Windows.Forms.TabPage();
            this.receiverSettingsTabPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.sampleClientConfigTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.receiverTypeComboBox = new System.Windows.Forms.ComboBox();
            this.receiverPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.settingsTabControl.SuspendLayout();
            this.uiSettingsTabPage.SuspendLayout();
            this.receiverSettingsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Enabled = false;
            this.cancelBtn.Location = new System.Drawing.Point(416, 422);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 0;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Visible = false;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(497, 422);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 0;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.Size = new System.Drawing.Size(553, 372);
            this.settingsPropertyGrid.TabIndex = 1;
            this.settingsPropertyGrid.ToolbarVisible = false;
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsTabControl.Controls.Add(this.uiSettingsTabPage);
            this.settingsTabControl.Controls.Add(this.receiverSettingsTabPage);
            this.settingsTabControl.Location = new System.Drawing.Point(12, 12);
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.SelectedIndex = 0;
            this.settingsTabControl.Size = new System.Drawing.Size(567, 404);
            this.settingsTabControl.TabIndex = 2;
            // 
            // uiSettingsTabPage
            // 
            this.uiSettingsTabPage.Controls.Add(this.settingsPropertyGrid);
            this.uiSettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.uiSettingsTabPage.Name = "uiSettingsTabPage";
            this.uiSettingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.uiSettingsTabPage.Size = new System.Drawing.Size(559, 378);
            this.uiSettingsTabPage.TabIndex = 0;
            this.uiSettingsTabPage.Text = "Settings";
            this.uiSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // receiverSettingsTabPage
            // 
            this.receiverSettingsTabPage.Controls.Add(this.label2);
            this.receiverSettingsTabPage.Controls.Add(this.sampleClientConfigTextBox);
            this.receiverSettingsTabPage.Controls.Add(this.label1);
            this.receiverSettingsTabPage.Controls.Add(this.receiverTypeComboBox);
            this.receiverSettingsTabPage.Controls.Add(this.receiverPropertyGrid);
            this.receiverSettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.receiverSettingsTabPage.Name = "receiverSettingsTabPage";
            this.receiverSettingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.receiverSettingsTabPage.Size = new System.Drawing.Size(559, 378);
            this.receiverSettingsTabPage.TabIndex = 1;
            this.receiverSettingsTabPage.Text = "Receiver";
            this.receiverSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sample Client Configuration:";
            // 
            // sampleClientConfigTextBox
            // 
            this.sampleClientConfigTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sampleClientConfigTextBox.Location = new System.Drawing.Point(6, 243);
            this.sampleClientConfigTextBox.Multiline = true;
            this.sampleClientConfigTextBox.Name = "sampleClientConfigTextBox";
            this.sampleClientConfigTextBox.ReadOnly = true;
            this.sampleClientConfigTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sampleClientConfigTextBox.Size = new System.Drawing.Size(547, 129);
            this.sampleClientConfigTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Receiver Type";
            // 
            // receiverTypeComboBox
            // 
            this.receiverTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.receiverTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.receiverTypeComboBox.FormattingEnabled = true;
            this.receiverTypeComboBox.Location = new System.Drawing.Point(147, 6);
            this.receiverTypeComboBox.Name = "receiverTypeComboBox";
            this.receiverTypeComboBox.Size = new System.Drawing.Size(406, 21);
            this.receiverTypeComboBox.TabIndex = 1;
            // 
            // receiverPropertyGrid
            // 
            this.receiverPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.receiverPropertyGrid.Location = new System.Drawing.Point(6, 33);
            this.receiverPropertyGrid.Name = "receiverPropertyGrid";
            this.receiverPropertyGrid.Size = new System.Drawing.Size(547, 182);
            this.receiverPropertyGrid.TabIndex = 0;
            this.receiverPropertyGrid.ToolbarVisible = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(591, 457);
            this.Controls.Add(this.settingsTabControl);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Log2Console Settings";
            this.settingsTabControl.ResumeLayout(false);
            this.uiSettingsTabPage.ResumeLayout(false);
            this.receiverSettingsTabPage.ResumeLayout(false);
            this.receiverSettingsTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.PropertyGrid settingsPropertyGrid;
		private System.Windows.Forms.TabControl settingsTabControl;
		private System.Windows.Forms.TabPage uiSettingsTabPage;
		private System.Windows.Forms.TabPage receiverSettingsTabPage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox receiverTypeComboBox;
		private System.Windows.Forms.PropertyGrid receiverPropertyGrid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sampleClientConfigTextBox;
    }
}