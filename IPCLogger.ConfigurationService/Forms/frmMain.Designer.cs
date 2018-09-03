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
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pUsers = new System.Windows.Forms.Panel();
            this.pDivider1 = new System.Windows.Forms.Panel();
            this.tsUsers = new System.Windows.Forms.ToolStrip();
            this.btnUserAdd = new System.Windows.Forms.ToolStripButton();
            this.btnUserEdit = new System.Windows.Forms.ToolStripButton();
            this.btnUserDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUserBlockChange = new System.Windows.Forms.ToolStripButton();
            this.lvUsers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.tcMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pUsers.SuspendLayout();
            this.tsUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tabPage1);
            this.tcMain.Controls.Add(this.tabPage2);
            this.tcMain.Location = new System.Drawing.Point(12, 12);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(673, 459);
            this.tcMain.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pUsers);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(665, 433);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Users";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pUsers
            // 
            this.pUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pUsers.Controls.Add(this.pDivider1);
            this.pUsers.Controls.Add(this.tsUsers);
            this.pUsers.Controls.Add(this.lvUsers);
            this.pUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pUsers.Location = new System.Drawing.Point(3, 3);
            this.pUsers.Name = "pUsers";
            this.pUsers.Size = new System.Drawing.Size(659, 427);
            this.pUsers.TabIndex = 1;
            // 
            // pDivider1
            // 
            this.pDivider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pDivider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pDivider1.Location = new System.Drawing.Point(0, 24);
            this.pDivider1.Name = "pDivider1";
            this.pDivider1.Size = new System.Drawing.Size(658, 1);
            this.pDivider1.TabIndex = 4;
            // 
            // tsUsers
            // 
            this.tsUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tsUsers.AutoSize = false;
            this.tsUsers.Dock = System.Windows.Forms.DockStyle.None;
            this.tsUsers.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUserAdd,
            this.btnUserEdit,
            this.btnUserDelete,
            this.toolStripSeparator1,
            this.btnUserBlockChange});
            this.tsUsers.Location = new System.Drawing.Point(1, 0);
            this.tsUsers.Name = "tsUsers";
            this.tsUsers.Size = new System.Drawing.Size(660, 25);
            this.tsUsers.TabIndex = 3;
            // 
            // btnUserAdd
            // 
            this.btnUserAdd.Image = global::IPCLogger.ConfigurationService.Properties.Resources.add_user;
            this.btnUserAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(74, 22);
            this.btnUserAdd.Text = "Add user";
            this.btnUserAdd.Click += new System.EventHandler(this.btnUserAdd_Click);
            // 
            // btnUserEdit
            // 
            this.btnUserEdit.Image = global::IPCLogger.ConfigurationService.Properties.Resources.edit_user;
            this.btnUserEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserEdit.Name = "btnUserEdit";
            this.btnUserEdit.Size = new System.Drawing.Size(72, 22);
            this.btnUserEdit.Text = "Edit user";
            this.btnUserEdit.Click += new System.EventHandler(this.btnUserEdit_Click);
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnUserDelete.Image")));
            this.btnUserDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(85, 22);
            this.btnUserDelete.Text = "Delete user";
            this.btnUserDelete.Click += new System.EventHandler(this.btnUserDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUserBlockChange
            // 
            this.btnUserBlockChange.Image = global::IPCLogger.ConfigurationService.Properties.Resources.block_user;
            this.btnUserBlockChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserBlockChange.Name = "btnUserBlockChange";
            this.btnUserBlockChange.Size = new System.Drawing.Size(81, 22);
            this.btnUserBlockChange.Text = "Block user";
            this.btnUserBlockChange.Click += new System.EventHandler(this.btnUserBlockChange_Click);
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
            this.lvUsers.FullRowSelect = true;
            this.lvUsers.LabelWrap = false;
            this.lvUsers.Location = new System.Drawing.Point(2, 27);
            this.lvUsers.MultiSelect = false;
            this.lvUsers.Name = "lvUsers";
            this.lvUsers.Size = new System.Drawing.Size(653, 396);
            this.lvUsers.TabIndex = 1;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "User";
            this.columnHeader1.Width = 192;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Role";
            this.columnHeader2.Width = 238;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Active";
            this.columnHeader3.Width = 54;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(665, 433);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Logged Applications";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(598, 477);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 26);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 512);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tcMain);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration Service";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormOnClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormOnKeyDown);
            this.tcMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.pUsers.ResumeLayout(false);
            this.tsUsers.ResumeLayout(false);
            this.tsUsers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pUsers;
        private System.Windows.Forms.ListView lvUsers;
        private System.Windows.Forms.ToolStrip tsUsers;
        private System.Windows.Forms.ToolStripButton btnUserAdd;
        private System.Windows.Forms.ToolStripButton btnUserEdit;
        private System.Windows.Forms.Panel pDivider1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton btnUserDelete;
        private System.Windows.Forms.ToolStripButton btnUserBlockChange;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

