namespace Log2Console.Settings
{
    partial class ReceiversForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReceiversForm));
            this.receiversListView = new System.Windows.Forms.ListView();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addReceiverBtn = new System.Windows.Forms.ToolStripButton();
            this.removeReceiverBtn = new System.Windows.Forms.ToolStripButton();
            this.label2 = new System.Windows.Forms.Label();
            this.sampleClientConfigTextBox = new System.Windows.Forms.TextBox();
            this.receiverPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // receiversListView
            // 
            this.receiversListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.receiversListView.Location = new System.Drawing.Point(0, 0);
            this.receiversListView.MultiSelect = false;
            this.receiversListView.Name = "receiversListView";
            this.receiversListView.Size = new System.Drawing.Size(211, 376);
            this.receiversListView.TabIndex = 0;
            this.receiversListView.UseCompatibleStateImageBehavior = false;
            this.receiversListView.View = System.Windows.Forms.View.List;
            this.receiversListView.SelectedIndexChanged += new System.EventHandler(this.receiversListView_SelectedIndexChanged);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.receiversListView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(211, 376);
            this.toolStripContainer1.Location = new System.Drawing.Point(12, 12);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(211, 401);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addReceiverBtn,
            this.removeReceiverBtn});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(108, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // addReceiverBtn
            // 
            this.addReceiverBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addReceiverBtn.Image = ((System.Drawing.Image)(resources.GetObject("addReceiverBtn.Image")));
            this.addReceiverBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addReceiverBtn.Name = "addReceiverBtn";
            this.addReceiverBtn.Size = new System.Drawing.Size(42, 22);
            this.addReceiverBtn.Text = "Add...";
            this.addReceiverBtn.Click += new System.EventHandler(this.addReceiverBtn_Click);
            // 
            // removeReceiverBtn
            // 
            this.removeReceiverBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeReceiverBtn.Enabled = false;
            this.removeReceiverBtn.Image = ((System.Drawing.Image)(resources.GetObject("removeReceiverBtn.Image")));
            this.removeReceiverBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeReceiverBtn.Name = "removeReceiverBtn";
            this.removeReceiverBtn.Size = new System.Drawing.Size(54, 22);
            this.removeReceiverBtn.Text = "Remove";
            this.removeReceiverBtn.Click += new System.EventHandler(this.removeReceiverBtn_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 268);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Sample Client Configuration:";
            // 
            // sampleClientConfigTextBox
            // 
            this.sampleClientConfigTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sampleClientConfigTextBox.Location = new System.Drawing.Point(229, 284);
            this.sampleClientConfigTextBox.Multiline = true;
            this.sampleClientConfigTextBox.Name = "sampleClientConfigTextBox";
            this.sampleClientConfigTextBox.ReadOnly = true;
            this.sampleClientConfigTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sampleClientConfigTextBox.Size = new System.Drawing.Size(481, 129);
            this.sampleClientConfigTextBox.TabIndex = 6;
            // 
            // receiverPropertyGrid
            // 
            this.receiverPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.receiverPropertyGrid.Location = new System.Drawing.Point(229, 12);
            this.receiverPropertyGrid.Name = "receiverPropertyGrid";
            this.receiverPropertyGrid.Size = new System.Drawing.Size(481, 253);
            this.receiverPropertyGrid.TabIndex = 5;
            this.receiverPropertyGrid.ToolbarVisible = false;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(554, 419);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 8;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(635, 419);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 9;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // ReceiversForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(722, 454);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sampleClientConfigTextBox);
            this.Controls.Add(this.receiverPropertyGrid);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "ReceiversForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Receivers";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView receiversListView;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addReceiverBtn;
        private System.Windows.Forms.ToolStripButton removeReceiverBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sampleClientConfigTextBox;
        private System.Windows.Forms.PropertyGrid receiverPropertyGrid;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}