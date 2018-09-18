using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public partial class frmManageLogger : Form
    {
        private LoggerModel _loggerModel;

        public LoggerRegDTO Result { get; private set; }

        public frmManageLogger()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            Ok();
        }

        private void FrmKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    break;
                case Keys.Enter:
                    Ok();
                    break;
            }
        }

        private void LockWindow()
        {
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            Application.DoEvents();
        }

        private void UnlockWindow()
        {
            Cursor = Cursors.Default;
            Enabled = true;
            Application.DoEvents();
        }

        private void Ok()
        {
            try
            {
                AssertChanges();
                CreateResultDTO();
                try
                {
                    LockWindow();
                    SaveChanges();
                }
                finally
                {
                    UnlockWindow();
                }
                DialogResult = DialogResult.OK;
            }
            catch (ValidationException vex)
            {
                MessageBox.Show(vex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Result = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Result = null;
            }
        }

        public bool Execute()
        {
            return Execute(null);
        }

        public bool Execute(LoggerModel existingLogger)
        {
            _loggerModel = existingLogger?.Clone<LoggerModel>() ?? new LoggerModel();
            BindModels();
            return ShowDialog() == DialogResult.OK;
        }

        private void BindModels()
        {
            Text = _loggerModel.Id == 0 ? "Register logger" : "Modify logger";
            tbAppName.DataBindings.Add("Text", _loggerModel, "ApplicationName", false, DataSourceUpdateMode.OnPropertyChanged);
            tbDescription.DataBindings.Add("Text", _loggerModel, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
            tbConfigurationFile.DataBindings.Add("Text", _loggerModel, "ConfigurationFile", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void AssertChanges()
        {
            if (string.IsNullOrWhiteSpace(tbAppName.Text))
            {
                tbAppName.Focus();
                throw new ValidationException("Application name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(tbConfigurationFile.Text) || !File.Exists(tbConfigurationFile.Text))
            {
                tbConfigurationFile.Focus();
                throw new ValidationException("Invalid configuration file");
            }
        }

        private void CreateResultDTO()
        {
            Result = new LoggerRegDTO
            {
                Id = _loggerModel.Id,
                ApplicationName = _loggerModel.ApplicationName.Trim(),
                Description = _loggerModel.Description,
                ConfigurationFile = _loggerModel.ConfigurationFile
            };
        }

        private void SaveChanges()
        {
            Result.Id = Result.Id == 0
                ? LoggerDAL.Instance.Create(Result)
                : LoggerDAL.Instance.Update(Result);
        }

        private void TbConfigurationFile_ButtonClick(object sender, EventArgs e)
        {
            if (odLogFile.ShowDialog() == DialogResult.OK)
            {
                tbConfigurationFile.Text = odLogFile.FileName;
            }
        }
    }
}
