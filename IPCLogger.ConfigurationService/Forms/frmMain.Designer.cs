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
            this.tpUsers = new System.Windows.Forms.TabPage();
            this.pUsers = new IPCLogger.ConfigurationService.Controls.BorderedPanel();
            this.hdUsers = new IPCLogger.ConfigurationService.Controls.HorizontalDivider();
            this.tsUsers = new System.Windows.Forms.ToolStrip();
            this.btnUserAdd = new System.Windows.Forms.ToolStripButton();
            this.btnUserEdit = new System.Windows.Forms.ToolStripButton();
            this.btnUserDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUserBlockChange = new System.Windows.Forms.ToolStripButton();
            this.btnUsersRefresh = new System.Windows.Forms.ToolStripButton();
            this.lvUsers = new IPCLogger.ConfigurationService.Controls.BorderedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pApps = new IPCLogger.ConfigurationService.Controls.BorderedPanel();
            this.hdApps = new IPCLogger.ConfigurationService.Controls.HorizontalDivider();
            this.tsApps = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.lvApps = new IPCLogger.ConfigurationService.Controls.BorderedListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tcMain.SuspendLayout();
            this.tpUsers.SuspendLayout();
            this.pUsers.SuspendLayout();
            this.tsUsers.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pApps.SuspendLayout();
            this.tsApps.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpUsers);
            this.tcMain.Controls.Add(this.tabPage2);
            this.tcMain.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tcMain.ItemSize = new System.Drawing.Size(170, 40);
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
            // tpUsers
            // 
            this.tpUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.tpUsers.Controls.Add(this.pUsers);
            this.tpUsers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tpUsers.Location = new System.Drawing.Point(4, 44);
            this.tpUsers.Name = "tpUsers";
            this.tpUsers.Padding = new System.Windows.Forms.Padding(8);
            this.tpUsers.Size = new System.Drawing.Size(728, 544);
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
            this.pUsers.Size = new System.Drawing.Size(712, 528);
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
            this.btnUserEdit,
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
            this.btnUserAdd.Image = global::IPCLogger.ConfigurationService.Properties.Resources.add_user;
            this.btnUserAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(58, 36);
            this.btnUserAdd.Text = "Add user";
            this.btnUserAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserAdd.Click += new System.EventHandler(this.BtnUserAdd_Click);
            // 
            // btnUserEdit
            // 
            this.btnUserEdit.Image = global::IPCLogger.ConfigurationService.Properties.Resources.edit_user;
            this.btnUserEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserEdit.Name = "btnUserEdit";
            this.btnUserEdit.Size = new System.Drawing.Size(56, 36);
            this.btnUserEdit.Text = "Edit user";
            this.btnUserEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUserEdit.Click += new System.EventHandler(this.BtnUserEdit_Click);
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnUserDelete.Image")));
            this.btnUserDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(69, 36);
            this.btnUserDelete.Text = "Delete user";
            this.btnUserDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            this.btnUserBlockChange.Size = new System.Drawing.Size(80, 36);
            this.btnUserBlockChange.Text = "Block user";
            this.btnUserBlockChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            this.lvUsers.Size = new System.Drawing.Size(710, 488);
            this.lvUsers.TabIndex = 1;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            this.lvUsers.SelectedIndexChanged += new System.EventHandler(this.LvUsers_SelectedIndexChanged);
            this.lvUsers.DoubleClick += new System.EventHandler(this.BtnUserEdit_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "User";
            this.columnHeader1.Width = 329;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Role";
            this.columnHeader2.Width = 321;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Blocked";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(222)))), ((int)(((byte)(222)))));
            this.tabPage2.Controls.Add(this.pApps);
            this.tabPage2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage2.Location = new System.Drawing.Point(4, 44);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(8);
            this.tabPage2.Size = new System.Drawing.Size(728, 544);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Logged Applications";
            // 
            // pApps
            // 
            this.pApps.BackColor = System.Drawing.Color.Transparent;
            this.pApps.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pApps.Controls.Add(this.hdApps);
            this.pApps.Controls.Add(this.tsApps);
            this.pApps.Controls.Add(this.lvApps);
            this.pApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pApps.Location = new System.Drawing.Point(8, 8);
            this.pApps.Name = "pApps";
            this.pApps.Size = new System.Drawing.Size(712, 528);
            this.pApps.TabIndex = 2;
            // 
            // hdApps
            // 
            this.hdApps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdApps.Location = new System.Drawing.Point(0, 38);
            this.hdApps.Name = "hdApps";
            this.hdApps.Size = new System.Drawing.Size(711, 1);
            this.hdApps.TabIndex = 4;
            // 
            // tsApps
            // 
            this.tsApps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tsApps.AutoSize = false;
            this.tsApps.Dock = System.Windows.Forms.DockStyle.None;
            this.tsApps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsApps.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsApps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.toolStripButton4,
            this.toolStripButton5});
            this.tsApps.Location = new System.Drawing.Point(-2, 0);
            this.tsApps.Name = "tsApps";
            this.tsApps.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tsApps.Size = new System.Drawing.Size(714, 39);
            this.tsApps.TabIndex = 3;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::IPCLogger.ConfigurationService.Properties.Resources.add_user;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(58, 36);
            this.toolStripButton1.Text = "Add user";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::IPCLogger.ConfigurationService.Properties.Resources.edit_user;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(56, 36);
            this.toolStripButton2.Text = "Edit user";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(69, 36);
            this.toolStripButton3.Text = "Delete user";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.Image = global::IPCLogger.ConfigurationService.Properties.Resources.block_user;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(80, 36);
            this.toolStripButton4.Text = "Block user";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton5.Image = global::IPCLogger.ConfigurationService.Properties.Resources.refresh;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(50, 36);
            this.toolStripButton5.Text = "Refresh";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // lvApps
            // 
            this.lvApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvApps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvApps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lvApps.CompactGridLines = true;
            this.lvApps.FullRowSelect = true;
            this.lvApps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvApps.LabelWrap = false;
            this.lvApps.Location = new System.Drawing.Point(0, 38);
            this.lvApps.MultiSelect = false;
            this.lvApps.Name = "lvApps";
            this.lvApps.OwnerDraw = true;
            this.lvApps.Size = new System.Drawing.Size(710, 488);
            this.lvApps.TabIndex = 1;
            this.lvApps.UseCompatibleStateImageBehavior = false;
            this.lvApps.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Application";
            this.columnHeader4.Width = 153;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 178;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Log file path";
            this.columnHeader6.Width = 319;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Visibility";
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
            this.tpUsers.ResumeLayout(false);
            this.pUsers.ResumeLayout(false);
            this.tsUsers.ResumeLayout(false);
            this.tsUsers.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.pApps.ResumeLayout(false);
            this.tsApps.ResumeLayout(false);
            this.tsApps.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MainTabControl tcMain;
        private System.Windows.Forms.TabPage tpUsers;
        private System.Windows.Forms.TabPage tabPage2;
        private BorderedPanel pUsers;
        private BorderedListView lvUsers;
        private System.Windows.Forms.ToolStrip tsUsers;
        private System.Windows.Forms.ToolStripButton btnUserAdd;
        private System.Windows.Forms.ToolStripButton btnUserEdit;
        private HorizontalDivider hdUsers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton btnUserDelete;
        private System.Windows.Forms.ToolStripButton btnUserBlockChange;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnUsersRefresh;
        private BorderedPanel pApps;
        private HorizontalDivider hdApps;
        private System.Windows.Forms.ToolStrip tsApps;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private BorderedListView lvApps;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}

