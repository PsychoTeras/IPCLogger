using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public partial class frmManageApplication : Form
    {
        private ApplicationModel _applicationModel;

        public ApplicationRegDTO Result { get; private set; }

        public frmManageApplication()
        {
            InitializeComponent();
        }

        private void BtnOkClick(object sender, System.EventArgs e)
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

        public bool Execute(ApplicationModel existingApplication)
        {
            _applicationModel = existingApplication?.Clone<ApplicationModel>() ?? new ApplicationModel();
            BindModels();
            return ShowDialog() == DialogResult.OK;
        }

        private void BindModels()
        {
            Text = _applicationModel.Id == 0 ? "Register application" : "Modify application";
            tbApplicationName.DataBindings.Add("Text", _applicationModel, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
            tbDescription.DataBindings.Add("Text", _applicationModel, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
            tbConfigurationFile.DataBindings.Add("Text", _applicationModel, "ConfigurationFile", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void AssertChanges()
        {
            if (string.IsNullOrWhiteSpace(tbApplicationName.Text))
            {
                tbApplicationName.Focus();
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
            Result = new ApplicationRegDTO
            {
                Id = _applicationModel.Id,
                Name = _applicationModel.Name.Trim(),
                Description = _applicationModel.Description,
                ConfigurationFile = _applicationModel.ConfigurationFile
            };
        }

        private void SaveChanges()
        {
            Result.Id = Result.Id == 0
                ? ApplicationDAL.Instance.Create(Result)
                : ApplicationDAL.Instance.Update(Result);
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
