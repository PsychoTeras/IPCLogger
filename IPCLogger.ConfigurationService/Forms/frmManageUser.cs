using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public partial class frmManageUser : Form
    {
        private IEnumerable<RoleModel> _roles;
        private UserModel _userModel;

        public UserRegDTO Result { get; private set; }

        public frmManageUser()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
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

        private void CbDontChangePassword_CheckedChanged(object sender, EventArgs e)
        {
            tbPassword1.Enabled = tbPassword2.Enabled = !cbDontChangePassword.Checked;
        }

        public bool Execute(IEnumerable<RoleModel> roles)
        {
            return Execute(roles, null);
        }

        public bool Execute(IEnumerable<RoleModel> roles, UserModel existingUser)
        {
            _roles = roles;
            _userModel = existingUser?.Clone<UserModel>() ?? new UserModel();
            BindModels();
            return ShowDialog() == DialogResult.OK;
        }

        private void BindModels()
        {
            Text = _userModel.Id == 0 ? "Create new user" : "Modify user";
            cbDontChangePassword.Visible = _userModel.Id != 0;
            tbUserName.DataBindings.Add("Text", _userModel, "UserName", false, DataSourceUpdateMode.OnPropertyChanged);
            cbRole.DataSource = _roles;
            cbRole.SelectedItem = _roles.FirstOrDefault(r => r.Id == _userModel.RoleId);
        }

        private void AssertChanges()
        {
            if (string.IsNullOrWhiteSpace(tbUserName.Text))
            {
                tbUserName.Focus();
                throw new ValidationException("User name cannot be empty");
            }

            if (cbRole.SelectedItem == null)
            {
                cbRole.Focus();
                throw new ValidationException("Role is not selected");
            }

            if (!cbDontChangePassword.Checked)
            {
                if (tbPassword1.Text.Length == 0)
                {
                    tbPassword1.Focus();
                    throw new ValidationException("Password cannot be empty");
                }
                if (tbPassword1.Text != tbPassword2.Text)
                {
                    tbPassword1.Focus();
                    throw new ValidationException("Password does not match");
                }
            }
        }

        private void CreateResultDTO()
        {
            Result = new UserRegDTO
            {
                Id = _userModel.Id,
                UserName = tbUserName.Text.Trim(),
                Password = cbDontChangePassword.Checked ? null : tbPassword1.Text,
                RoleId = ((RoleModel)cbRole.SelectedItem).Id
            };
        }

        private void SaveChanges()
        {
            Result.Id = Result.Id == 0
                ? UserDAL.Instance.Create(Result)
                : UserDAL.Instance.Update(Result);
        }
    }
}