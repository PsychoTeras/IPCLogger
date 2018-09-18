using IPCLogger.ConfigurationService.Controls;

namespace IPCLogger.ConfigurationService.Forms
{
    partial class frmManageLogger
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
            this.tlpMain = new IPCLogger.ConfigurationService.Controls.BorderedTableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tlpInner = new System.Windows.Forms.TableLayoutPanel();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbAppName = new System.Windows.Forms.TextBox();
            this.tbConfigurationFile = new IPCLogger.ConfigurationService.Controls.TextBoxWithButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.odLogFile = new System.Windows.Forms.OpenFileDialog();
            this.tlpMain.SuspendLayout();
            this.tlpInner.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.Controls.Add(this.btnCancel, 0, 1);
            this.tlpMain.Controls.Add(this.tlpInner, 0, 0);
            this.tlpMain.Controls.Add(this.btnOk, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(12, 14);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tlpMain.Size = new System.Drawing.Size(540, 233);
            this.tlpMain.TabIndex = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(446, 195);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 8, 7, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tlpInner
            // 
            this.tlpInner.ColumnCount = 2;
            this.tlpMain.SetColumnSpan(this.tlpInner, 2);
            this.tlpInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInner.Controls.Add(this.tbDescription, 1, 1);
            this.tlpInner.Controls.Add(this.label5, 0, 0);
            this.tlpInner.Controls.Add(this.label6, 0, 1);
            this.tlpInner.Controls.Add(this.label7, 0, 2);
            this.tlpInner.Controls.Add(this.tbAppName, 1, 0);
            this.tlpInner.Controls.Add(this.tbConfigurationFile, 1, 2);
            this.tlpInner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpInner.Location = new System.Drawing.Point(3, 3);
            this.tlpInner.Name = "tlpInner";
            this.tlpInner.Padding = new System.Windows.Forms.Padding(0, 3, 2, 0);
            this.tlpInner.RowCount = 4;
            this.tlpInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpInner.Size = new System.Drawing.Size(534, 97);
            this.tlpInner.TabIndex = 9;
            // 
            // tbDescription
            // 
            this.tbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDescription.Location = new System.Drawing.Point(113, 37);
            this.tbDescription.MaxLength = 39;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(416, 23);
            this.tbDescription.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 31);
            this.label5.TabIndex = 1;
            this.label5.Text = "Applicatoin name:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 31);
            this.label6.TabIndex = 5;
            this.label6.Text = "Description:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 31);
            this.label7.TabIndex = 6;
            this.label7.Text = "Configuration file:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbAppName
            // 
            this.tbAppName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAppName.Location = new System.Drawing.Point(113, 6);
            this.tbAppName.MaxLength = 39;
            this.tbAppName.Name = "tbAppName";
            this.tbAppName.Size = new System.Drawing.Size(416, 23);
            this.tbAppName.TabIndex = 1;
            // 
            // tbConfigurationFile
            // 
            this.tbConfigurationFile.ButtonFont = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbConfigurationFile.ButtonImage = null;
            this.tbConfigurationFile.ButtonPadding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.tbConfigurationFile.ButtonText = "...";
            this.tbConfigurationFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbConfigurationFile.Location = new System.Drawing.Point(113, 68);
            this.tbConfigurationFile.MaxLength = 100;
            this.tbConfigurationFile.Name = "tbConfigurationFile";
            this.tbConfigurationFile.Size = new System.Drawing.Size(416, 23);
            this.tbConfigurationFile.TabIndex = 3;
            this.tbConfigurationFile.ButtonClick += new System.EventHandler(this.TbConfigurationFile_ButtonClick);
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Location = new System.Drawing.Point(354, 195);
            this.btnOk.Margin = new System.Windows.Forms.Padding(5, 8, 3, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(87, 30);
            this.btnOk.TabIndex = 14;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // odLogFile
            // 
            this.odLogFile.Filter = "Configuration files (*.config)|*.config|All files|*.*";
            // 
            // frmManageLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 261);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(580, 300);
            this.Name = "frmManageLogger";
            this.Padding = new System.Windows.Forms.Padding(12, 14, 12, 14);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register/Manage logger";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmKeyDown);
            this.tlpMain.ResumeLayout(false);
            this.tlpInner.ResumeLayout(false);
            this.tlpInner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BorderedTableLayoutPanel tlpMain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tlpInner;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbAppName;
        private TextBoxWithButton tbConfigurationFile;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.OpenFileDialog odLogFile;
    }
}