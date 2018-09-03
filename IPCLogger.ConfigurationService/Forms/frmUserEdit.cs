using IPCLogger.ConfigurationService.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public partial class frmUserEdit : Form
    {
        private IEnumerable<Role> _roles;
        private UserModel _userModel;

        public UserRegDTO Result { get; private set; }

        public frmUserEdit()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            Ok();
        }

        private void Ok()
        {
            try
            {
                AssertChanges();
                CreateResultDTO();
                SaveChanges();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Result = null;
            }
        }

        public bool Execute(IEnumerable<Role> roles)
        {
            return Execute(roles, null);
        }

        public bool Execute(IEnumerable<Role> roles, UserModel existingUser)
        {
            _roles = roles;
            _userModel = _userModel?.Clone<UserModel>() ?? new UserModel();
            BindModels();
            return ShowDialog() == DialogResult.OK;
        }

        private void BindModels()
        {
            tbUserName.DataBindings.Add("Text", _userModel, "UserName", false, DataSourceUpdateMode.OnPropertyChanged);
            cbRole.DataSource = _roles;
        }

        private void AssertChanges()
        {
        }

        private void CreateResultDTO()
        {
            Result = new UserRegDTO
            {
                Id = _userModel.Id,
                UserName = tbUserName.Text.Trim(),
                Password = cbDontChangePassword.Checked ? null : tbPassword1.Text,
                RoleId = ((Role)cbRole.SelectedItem).Id
            };
        }

        private void SaveChanges()
        {
        }
    }
}