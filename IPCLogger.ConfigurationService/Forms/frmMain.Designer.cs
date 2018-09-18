using IPCLogger.ConfigurationService.Controls;

namespace IPCLogger.ConfigurationService.Forms
{
    sealed partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tcMain = new IPCLogger.ConfigurationService.Controls.MainTabControl();
            this.tpLoggers = new System.Windows.Forms.TabPage();
            this.pLoggers = new IPCLogger.ConfigurationService.Controls.BorderedPanel();
            this.hdLoggers = new IPCLogger.ConfigurationService.Controls.HorizontalDivider();
            this.tsLoggers = new System.Windows.Forms.ToolStrip();
            this.btnLoggerRegister = new System.Windows.Forms.ToolStripButton();
            this.btnLoggerModify = new System.Windows.Forms.ToolStripButton();
            this.btnLoggerUnregister = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoggerVisibleChange = new System.Windows.Forms.ToolStripButton();
            this.btnLoggersRefresh = new System.Windows.Forms.ToolStripButton();
            this.lvLoggers = new IPCLogger.ConfigurationService.Controls.BorderedListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpUsers = new System.Windows.Forms.TabPage();
            this.pUsers = new IPCLogger.ConfigurationService.Controls.BorderedPanel();
            this.hdUsers = new IPCLogger.ConfigurationService.Controls.HorizontalDivider();
            this.tsUsers = new System.Windows.Forms.ToolStrip();
            this.btnUserAdd = new System.Windows.Forms.ToolStripButton();
            this.btnUserModify = new System.Windows.Forms.ToolStripButton();
            this.btnUserDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUserBlockChange = new System.Windows.Forms.ToolStripButton();
            this.btnUsersRefresh = new System.Windows.Forms.ToolStripButton();
            this.lvUsers = new IPCLogger.ConfigurationService.Controls.BorderedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tcMain.SuspendLayout();
            this.tpLoggers.SuspendLayout();
            this.pLoggers.SuspendLayout();
            this.tsLoggers.SuspendLayout();
            this.tpUsers.SuspendLayout();
            this.pUsers.SuspendLayout();
            this.tsUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpLoggers);
            this.tcMain.Controls.Add(this.tpUsers);
            this.tcMain.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tcMain.ItemSize = new System.Drawing.Size(110, 35);
            this.tcMain.Location = new System.Drawing.Point(12, 11);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(736, 592);
            this.tcMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcMain.TabColorActive = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.tcMain.TabColorInactive = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tcMain.TabIndex = 0;
            this.tcMain.TabRigthSeparator = 1;
            // 
            // tpLoggers
            // 
            this.tpLoggers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.tpLoggers.Controls.Add(this.pLoggers);
            this.tpLoggers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tpLoggers.Location = new System.Drawing.Point(4, 39);
            this.tpLoggers.Name = "tpLoggers";
            this.tpLoggers.Padding = new System.Windows.Forms.Padding(8);
            this.tpLoggers.Size = new System.Drawing.Size(728, 549);
            this.tpLoggers.TabIndex = 1;
            this.tpLoggers.Text = "Loggers";
            // 
            // pLoggers
            // 
            this.pLoggers.BackColor = System.Drawing.Color.Transparent;
            this.pLoggers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pLoggers.Controls.Add(this.hdLoggers);
            this.pLoggers.Controls.Add(this.tsLoggers);
            this.pLoggers.Controls.Add(this.lvLoggers);
            this.pLoggers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pLoggers.Location = new System.Drawing.Point(8, 8);
            this.pLoggers.Name = "pLoggers";
            this.pLoggers.Size = new System.Drawing.Size(712, 533);
            this.pLoggers.TabIndex = 2;
            // 
            // hdLoggers
            // 
            this.hdLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdLoggers.Location = new System.Drawing.Point(0, 38);
            this.hdLoggers.Name = "hdLoggers";
            this.hdLoggers.Size = new System.Drawing.Size(711, 1);
            this.hdLoggers.TabIndex = 4;
            // 
            // tsLoggers
            // 
            this.tsLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tsLoggers.AutoSize = false;
            this.tsLoggers.Dock = System.Windows.Forms.DockStyle.None;
            this.tsLoggers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsLoggers.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsLoggers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoggerRegister,
            this.btnLoggerModify,
            this.btnLoggerUnregister,
            this.toolStripSeparator2,
            this.btnLoggerVisibleChange,
            this.btnLoggersRefresh});
            this.tsLoggers.Location = new System.Drawing.Point(-2, 0);
            this.tsLoggers.Name = "tsLoggers";
            this.tsLoggers.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tsLoggers.Size = new System.Drawing.Size(714, 39);
            this.tsLoggers.TabIndex = 3;
            // 
            // btnLoggerRegister
            // 
            this.btnLoggerRegister.AutoSize = false;
            this.btnLoggerRegister.Image = global::IPCLogger.ConfigurationService.Properties.Resources.add_app;
            this.btnLoggerRegister.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoggerRegister.Name = "btnLoggerRegister";
            this.btnLoggerRegister.Size = new System.Drawing.Size(62, 36);
            this.btnLoggerRegister.Text = "Register";
            this.btnLoggerRegister.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLoggerRegister.ToolTipText = "Register logger";
            this.btnLoggerRegister.Click += new System.EventHandler(this.BtnLoggerRegister_Click);
            // 
            // btnLoggerModify
            // 
            this.btnLoggerModify.AutoSize = false;
            this.btnLoggerModify.Image = global::IPCLogger.ConfigurationService.Properties.Resources.edit_app;
            this.btnLoggerModify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoggerModify.Name = "btnLoggerModify";
            this.btnLoggerModify.Size = new System.Drawing.Size(62, 36);
            this.btnLoggerModify.Text = "Modify";
            this.btnLoggerModify.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLoggerModify.ToolTipText = "Modify selected logger";
            this.btnLoggerModify.Click += new System.EventHandler(this.BtnLoggerModify_Click);
            // 
            // btnLoggerUnregister
            // 
            this.btnLoggerUnregister.AutoSize = false;
            this.btnLoggerUnregister.Image = global::IPCLogger.ConfigurationService.Properties.Resources.delete_app;
            this.btnLoggerUnregister.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoggerUnregister.Name = "btnLoggerUnregister";
            this.btnLoggerUnregister.Size = new System.Drawing.Size(62, 36);
            this.btnLoggerUnregister.Text = "Unregister";
            this.btnLoggerUnregister.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLoggerUnregister.ToolTipText = "Unregister selected logger";
            this.btnLoggerUnregister.Click += new System.EventHandler(this.BtnLoggerUnregister_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // btnLoggerVisibleChange
            // 
            this.btnLoggerVisibleChange.AutoSize = false;
            this.btnLoggerVisibleChange.Image = global::IPCLogger.ConfigurationService.Properties.Resources.hide_app;
            this.btnLoggerVisibleChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoggerVisibleChange.Name = "btnLoggerVisibleChange";
            this.btnLoggerVisibleChange.Size = new System.Drawing.Size(62, 36);
            this.btnLoggerVisibleChange.Text = "Hide";
            this.btnLoggerVisibleChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLoggerVisibleChange.ToolTipText = "Hide/show selected logger";
            this.btnLoggerVisibleChange.Click += new System.EventHandler(this.BtnLoggerVisibleChange_Click);
            // 
            // btnLoggersRefresh
            // 
            this.btnLoggersRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnLoggersRefresh.Image = global::IPCLogger.ConfigurationService.Properties.Resources.refresh;
            this.btnLoggersRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoggersRefresh.Name = "btnLoggersRefresh";
            this.btnLoggersRefresh.Size = new System.Drawing.Size(50, 36);
            this.btnLoggersRefresh.Text = "Refresh";
            this.btnLoggersRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLoggersRefresh.ToolTipText = "Refresh list";
            this.btnLoggersRefresh.Click += new System.EventHandler(this.BtnLoggersRefresh_Click);
            // 
            // lvLoggers
            // 
            this.lvLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLoggers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvLoggers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lvLoggers.CompactGridLines = true;
            this.lvLoggers.FullRowSelect = true;
            this.lvLoggers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvLoggers.LabelWrap = false;
            this.lvLoggers.Location = new System.Drawing.Point(0, 38);
            this.lvLoggers.MultiSelect = false;
            this.lvLoggers.Name = "lvLoggers";
            this.lvLoggers.OwnerDraw = true;
            this.lvLoggers.Size = new System.Drawing.Size(710, 493);
            this.lvLoggers.TabIndex = 1;
            this.lvLoggers.UseCompatibleStateImageBehavior = false;
            this.lvLoggers.View = System.Windows.Forms.View.Details;
            this.lvLoggers.SelectedIndexChanged += new System.EventHandler(this.LvLoggers_SelectedIndexChanged);
            this.lvLoggers.DoubleClick += new System.EventHandler(this.BtnLoggerModify_Click);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Application name";
            this.columnHeader4.Width = 153;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 178;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Configuration file";
            this.columnHeader6.Width = 319;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Visibility";
            // 
            // tpUsers
            // 
            this.tpUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.tpUsers.Controls.Add(this.pUsers);
            this.tpUsers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tpUsers.Location = new System.Drawing.Point(4, 39);
            this.tpUsers.Name = "tpUsers";
            this.tpUsers.Padding = new System.Windows.Forms.Padding(8);
            this.tpUsers.Size = new System.Drawing.Size(728, 549);
            this.tpUsers.TabIndex = 0;
            this.tpUsers.Text = "Users";
            // 
            // pUsers
            // 
            this.pUsers.BackColor = System.Drawing.Color.Transparent;
            this.pUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pUsers.Controls.Add(this.hdUsers);
            this.pUsers.Controls.Add(this.tsUsers);
            this.pUsers.Controls.Add(this.lvUsers);
            this.pUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pUsers.Location = new System.Drawing.Point(8, 8);
            this.pUsers.Name = "pUsers";
            this.pUsers.Size = new System.Drawing.Size(712, 533);
            this.pUsers.TabIndex = 1;
            // 
            // hdUsers
            // 
            this.hdUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdUsers.Location = new System.Drawing.Point(0, 38);
            this.hdUsers.Name = "hdUsers";
            this.hdUsers.Size = new System.Drawing.Size(711, 1);
            this.hdUsers.TabIndex = 4;
            // 
            // tsUsers
            // 
            this.tsUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tsUsers.AutoSize = false;
            this.tsUsers.Dock = System.Windows.Forms.DockStyle.None;
            this.tsUsers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsUsers.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUserAdd,
            this.btnUserModify,
            this.btnUserDelete,
            this.toolStripSeparator1,
            this.btnUserBlockChange,
            this.btnUsersRefresh});
            this.tsUsers.Location = new System.Drawing.Point(-2, 0);
            this.tsUsers.Name = "tsUsers";
            this.tsUsers.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tsUsers.Size = new System.Drawing.Size(714, 39);
            this.tsUsers.TabIndex = 3;
            // 
            // btnUserAdd
            // 
            this.btnUserAdd.AutoSize = false;
            this.btnUserAdd.Image = global::IPCLogger.ConfigurationService.Properties.Resources.add_user;
            this.btnUserAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(62, 36);
            this.btnUserAdd.Text = "Create";
            this.btnUserAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserAdd.ToolTipText = "Create new user";
            this.btnUserAdd.Click += new System.EventHandler(this.BtnUserAdd_Click);
            // 
            // btnUserModify
            // 
            this.btnUserModify.AutoSize = false;
            this.btnUserModify.Image = global::IPCLogger.ConfigurationService.Properties.Resources.edit_user;
            this.btnUserModify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserModify.Name = "btnUserModify";
            this.btnUserModify.Size = new System.Drawing.Size(62, 36);
            this.btnUserModify.Text = "Modify";
            this.btnUserModify.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserModify.ToolTipText = "Modify selected user";
            this.btnUserModify.Click += new System.EventHandler(this.BtnUserModify_Click);
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.AutoSize = false;
            this.btnUserDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnUserDelete.Image")));
            this.btnUserDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(62, 36);
            this.btnUserDelete.Text = "Delete";
            this.btnUserDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserDelete.ToolTipText = "Delete selected user";
            this.btnUserDelete.Click += new System.EventHandler(this.BtnUserDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnUserBlockChange
            // 
            this.btnUserBlockChange.AutoSize = false;
            this.btnUserBlockChange.Image = global::IPCLogger.ConfigurationService.Properties.Resources.block_user;
            this.btnUserBlockChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserBlockChange.Name = "btnUserBlockChange";
            this.btnUserBlockChange.Size = new System.Drawing.Size(62, 36);
            this.btnUserBlockChange.Text = "Block";
            this.btnUserBlockChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserBlockChange.ToolTipText = "Block/unblock selected user";
            this.btnUserBlockChange.Click += new System.EventHandler(this.BtnUserBlockChange_Click);
            // 
            // btnUsersRefresh
            // 
            this.btnUsersRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnUsersRefresh.Image = global::IPCLogger.ConfigurationService.Properties.Resources.refresh;
            this.btnUsersRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUsersRefresh.Name = "btnUsersRefresh";
            this.btnUsersRefresh.Size = new System.Drawing.Size(50, 36);
            this.btnUsersRefresh.Text = "Refresh";
            this.btnUsersRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUsersRefresh.ToolTipText = "Refresh list";
            this.btnUsersRefresh.Click += new System.EventHandler(this.BtnUsersRefresh_Click);
            // 
            // lvUsers
            // 
            this.lvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvUsers.CompactGridLines = true;
            this.lvUsers.FullRowSelect = true;
            this.lvUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUsers.LabelWrap = false;
            this.lvUsers.Location = new System.Drawing.Point(0, 38);
            this.lvUsers.MultiSelect = false;
            this.lvUsers.Name = "lvUsers";
            this.lvUsers.OwnerDraw = true;
            this.lvUsers.Size = new System.Drawing.Size(710, 493);
            this.lvUsers.TabIndex = 1;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            this.lvUsers.SelectedIndexChanged += new System.EventHandler(this.LvUsers_SelectedIndexChanged);
            this.lvUsers.DoubleClick += new System.EventHandler(this.BtnUserModify_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "User";
            this.columnHeader1.Width = 331;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Role";
            this.columnHeader2.Width = 319;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Blocked";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 615);
            this.Controls.Add(this.tcMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration Service";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormOnClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormOnKeyDown);
            this.tcMain.ResumeLayout(false);
            this.tpLoggers.ResumeLayout(false);
            this.pLoggers.ResumeLayout(false);
            this.tsLoggers.ResumeLayout(false);
            this.tsLoggers.PerformLayout();
            this.tpUsers.ResumeLayout(false);
            this.pUsers.ResumeLayout(false);
            this.tsUsers.ResumeLayout(false);
            this.tsUsers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MainTabControl tcMain;
        private System.Windows.Forms.TabPage tpUsers;
        private System.Windows.Forms.TabPage tpLoggers;
        private BorderedPanel pUsers;
        private BorderedListView lvUsers;
        private System.Windows.Forms.ToolStrip tsUsers;
        private System.Windows.Forms.ToolStripButton btnUserAdd;
        private System.Windows.Forms.ToolStripButton btnUserModify;
        private HorizontalDivider hdUsers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton btnUserDelete;
        private System.Windows.Forms.ToolStripButton btnUserBlockChange;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnUsersRefresh;
        private BorderedPanel pLoggers;
        private HorizontalDivider hdLoggers;
        private System.Windows.Forms.ToolStrip tsLoggers;
        private System.Windows.Forms.ToolStripButton btnLoggerRegister;
        private System.Windows.Forms.ToolStripButton btnLoggerModify;
        private System.Windows.Forms.ToolStripButton btnLoggerUnregister;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnLoggerVisibleChange;
        private System.Windows.Forms.ToolStripButton btnLoggersRefresh;
        private BorderedListView lvLoggers;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}

