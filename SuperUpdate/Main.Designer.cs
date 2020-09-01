namespace SuperUpdate
{
    partial class Main
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnAction = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.wbAnimation = new System.Windows.Forms.WebBrowser();
            this.pnlGray = new System.Windows.Forms.Panel();
            this.lblMoreLessInfo = new System.Windows.Forms.Label();
            this.pbArrow = new System.Windows.Forms.PictureBox();
            this.lvDetails = new System.Windows.Forms.ListView();
            this.miLog = new System.Windows.Forms.ContextMenu();
            this.miSaveLog = new System.Windows.Forms.MenuItem();
            this.diSaveLog = new System.Windows.Forms.SaveFileDialog();
            this.pbMain = new System.Windows.Forms.ProgressBar();
            this.pnlGray.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.AutoEllipsis = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(82, 12);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(690, 64);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Initializing...";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAction
            // 
            this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Enabled = false;
            this.btnAction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAction.Location = new System.Drawing.Point(613, 11);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(75, 25);
            this.btnAction.TabIndex = 1;
            this.btnAction.Text = "Install";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(698, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // wbAnimation
            // 
            this.wbAnimation.AllowNavigation = false;
            this.wbAnimation.AllowWebBrowserDrop = false;
            this.wbAnimation.IsWebBrowserContextMenuEnabled = false;
            this.wbAnimation.Location = new System.Drawing.Point(12, 12);
            this.wbAnimation.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbAnimation.Name = "wbAnimation";
            this.wbAnimation.ScriptErrorsSuppressed = true;
            this.wbAnimation.ScrollBarsEnabled = false;
            this.wbAnimation.Size = new System.Drawing.Size(64, 64);
            this.wbAnimation.TabIndex = 4;
            this.wbAnimation.Url = new System.Uri("", System.UriKind.Relative);
            this.wbAnimation.WebBrowserShortcutsEnabled = false;
            this.wbAnimation.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.RefreshLargeIcon);
            // 
            // pnlGray
            // 
            this.pnlGray.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlGray.Controls.Add(this.lblMoreLessInfo);
            this.pnlGray.Controls.Add(this.pbArrow);
            this.pnlGray.Controls.Add(this.btnAction);
            this.pnlGray.Controls.Add(this.btnCancel);
            this.pnlGray.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlGray.Location = new System.Drawing.Point(0, 416);
            this.pnlGray.Name = "pnlGray";
            this.pnlGray.Size = new System.Drawing.Size(784, 45);
            this.pnlGray.TabIndex = 5;
            // 
            // lblMoreLessInfo
            // 
            this.lblMoreLessInfo.Location = new System.Drawing.Point(38, 15);
            this.lblMoreLessInfo.Name = "lblMoreLessInfo";
            this.lblMoreLessInfo.Size = new System.Drawing.Size(103, 14);
            this.lblMoreLessInfo.TabIndex = 4;
            this.lblMoreLessInfo.Text = "Initializing...";
            this.lblMoreLessInfo.Click += new System.EventHandler(this.ExpandContract);
            this.lblMoreLessInfo.MouseEnter += new System.EventHandler(this.pbArrow_MouseEnter);
            this.lblMoreLessInfo.MouseLeave += new System.EventHandler(this.pbArrow_MouseLeave);
            // 
            // pbArrow
            // 
            this.pbArrow.Location = new System.Drawing.Point(12, 12);
            this.pbArrow.Name = "pbArrow";
            this.pbArrow.Size = new System.Drawing.Size(20, 20);
            this.pbArrow.TabIndex = 3;
            this.pbArrow.TabStop = false;
            this.pbArrow.Click += new System.EventHandler(this.ExpandContract);
            this.pbArrow.Paint += new System.Windows.Forms.PaintEventHandler(this.pbArrow_Paint);
            this.pbArrow.MouseEnter += new System.EventHandler(this.pbArrow_MouseEnter);
            this.pbArrow.MouseLeave += new System.EventHandler(this.pbArrow_MouseLeave);
            // 
            // lvDetails
            // 
            this.lvDetails.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDetails.AutoArrange = false;
            this.lvDetails.FullRowSelect = true;
            this.lvDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDetails.HideSelection = false;
            this.lvDetails.Location = new System.Drawing.Point(12, 117);
            this.lvDetails.MultiSelect = false;
            this.lvDetails.Name = "lvDetails";
            this.lvDetails.Size = new System.Drawing.Size(760, 287);
            this.lvDetails.TabIndex = 6;
            this.lvDetails.UseCompatibleStateImageBehavior = false;
            this.lvDetails.View = System.Windows.Forms.View.Details;
            this.lvDetails.SelectedIndexChanged += new System.EventHandler(this.lvDetails_SelectedIndexChanged);
            this.lvDetails.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvDetails_MouseClick);
            // 
            // miLog
            // 
            this.miLog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSaveLog});
            // 
            // miSaveLog
            // 
            this.miSaveLog.Index = 0;
            this.miSaveLog.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.miSaveLog.Text = "Save log...";
            this.miSaveLog.Click += new System.EventHandler(this.miSaveLog_Click);
            // 
            // diSaveLog
            // 
            this.diSaveLog.Filter = "Text File|*.txt|Log File|*.log";
            this.diSaveLog.FilterIndex = 2;
            this.diSaveLog.Title = "Save Super Update Log...";
            // 
            // pbMain
            // 
            this.pbMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMain.Location = new System.Drawing.Point(12, 87);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(760, 15);
            this.pbMain.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbMain.TabIndex = 7;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.lvDetails);
            this.Controls.Add(this.pnlGray);
            this.Controls.Add(this.wbAnimation);
            this.Controls.Add(this.lblMessage);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update - Super Suite";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Main_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this.Resize += new System.EventHandler(this.CheckIfExpanded);
            this.pnlGray.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbArrow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblMessage;
        public System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.WebBrowser wbAnimation;
        private System.Windows.Forms.Panel pnlGray;
        private System.Windows.Forms.PictureBox pbArrow;
        private System.Windows.Forms.Label lblMoreLessInfo;
        public System.Windows.Forms.ListView lvDetails;
        private System.Windows.Forms.ContextMenu miLog;
        private System.Windows.Forms.MenuItem miSaveLog;
        private System.Windows.Forms.SaveFileDialog diSaveLog;
        public System.Windows.Forms.ProgressBar pbMain;
    }
}

