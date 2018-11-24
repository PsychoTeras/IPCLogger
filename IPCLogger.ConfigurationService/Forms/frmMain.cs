using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.ConfigurationService.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public sealed partial class frmMain : Form
    {

#region Private fields

        private NotifyIcon _trayIcon;
        private Dictionary<int, RoleModel> _roles;

#endregion

#region Entry point

        private static bool CheckSingleApplicationInstance()
        {
            Process curProcess = Process.GetCurrentProcess();
            string curProcessName = curProcess.ProcessName;
            return Process.GetProcessesByName(curProcessName).Length == 1;
        }

        [STAThread]
        public static void Main()
        {
            if (!CheckSingleApplicationInstance()) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                SelfHosted.Instance.Start();
                using (new frmMain())
                {
                    Application.Run();
                }
            }
            finally
            {
                SelfHosted.Instance.Stop();
            }
        }

#endregion

#region Class methods

        public frmMain()
        {
            InitializeComponent();

            //Create tray menu
            ContextMenu trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Configure", MainFormOnShow);
            trayMenu.MenuItems.Add("Open console", OnOpenConsole);
            trayMenu.MenuItems.Add("-");
            //trayMenu.MenuItems.Add("About", OnShowAbout);
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayMenu.MenuItems[0].DefaultItem = true;

            //Create tray icon
            _trayIcon = new NotifyIcon();
            _trayIcon.ContextMenu = trayMenu;
            _trayIcon.DoubleClick += OnOpenConsole;// MainFormOnShow;
            _trayIcon.Icon = Icon;
            _trayIcon.Text = Text;
            _trayIcon.Visible = true;

            RefreshRoles();
            RefreshUsers();
            RefreshApplications();
        }

        private void OnOpenConsole(object sender, EventArgs e)
        {
            Process.Start(SelfHosted.Instance.Url);
        }

        private void OnExit(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void MainFormOnShow(object sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void ShowMainForm()
        {
            Visible = ShowInTaskbar = true;
            BringToFront();
            Activate();
        }

        private void MainFormOnClosing(object sender, FormClosingEventArgs e)
        {
            HideMainForm();
            e.Cancel = true;
        }

        private void HideMainForm()
        {
            Visible = ShowInTaskbar = false;
        }

        private void ExitApplication()
        {
            _trayIcon.Dispose();
            Application.Exit();
        }

        private void MainFormOnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    //BtnOkClick(null, null);
                    break;
                case Keys.Escape:
                    //BtnCancelClick(null, null);
                    break;
            }
        }

#endregion

#region Roles

        private void RefreshRoles()
        {
            _roles = UserDAL.Instance.GetRoles(true).ToDictionary(r => r.Id, r => r);
        }

#endregion

#region Users

        private UserModel GetSelectedUser()
        {
            ListViewItem item = lvUsers.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            return item?.Tag as UserModel;
        }

        private void RefreshUsers()
        {
            lvUsers.BeginUpdate();

            lvUsers.Items.Clear();
            List<UserModel> users = UserDAL.Instance.GetUsers();
            foreach (UserModel model in users)
            {
                ListViewItem item = new ListViewItem(model.UserName) { Tag = model };
                item.SubItems.Add(_roles[model.RoleId].ToString());
                item.SubItems.Add(model.Blocked ? "YES" : "NO");
                lvUsers.Items.Add(item);
            }

            lvUsers.EndUpdate();
            UsersSelectedIndexChanged();
        }

        private void BtnUsersRefresh_Click(object sender, EventArgs e)
        {
            RefreshUsers();
        }

        private void AddUser()
        {
            if (new frmManageUser().Execute(_roles.Values.ToArray()))
            {
                RefreshUsers();
            }
        }

        private void BtnUserAdd_Click(object sender, EventArgs e)
        {
            AddUser();
        }

        private void ModifyUser()
        {
            UserModel userModel = GetSelectedUser();
            if (userModel != null && new frmManageUser().Execute(_roles.Values.ToArray(), userModel))
            {
                RefreshUsers();
            }
        }

        private void BtnUserModify_Click(object sender, EventArgs e)
        {
            ModifyUser();
        }

        private void DeleteUser()
        {
            UserModel userModel = GetSelectedUser();
            if (userModel != null && MessageBox.Show($"Delete user \"{userModel.UserName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserDAL.Instance.Delete(userModel.Id);
                RefreshUsers();
            }
        }

        private void BtnUserDelete_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void BlockChangeUser()
        {
            UserModel userModel = GetSelectedUser();
            if (userModel == null) return;

            string action = userModel.Blocked ? "Unblock" : "Block";
            if (MessageBox.Show($"{action} user \"{userModel.UserName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserDAL.Instance.ChangeBlock(userModel.Id, !userModel.Blocked);
                RefreshUsers();
            }
        }

        private void BtnUserBlockChange_Click(object sender, EventArgs e)
        {
            BlockChangeUser();
        }

        private void UsersSelectedIndexChanged()
        {
            UserModel userModel = GetSelectedUser();
            btnUserModify.Enabled = btnUserDelete.Enabled = btnUserBlockChange.Enabled = userModel != null;
            bool userBlocked = userModel != null && userModel.Blocked;
            btnUserBlockChange.Image = userBlocked
                ? Properties.Resources.unblock_user
                : Properties.Resources.block_user;
            btnUserBlockChange.Text = userBlocked ? "Unblock" : "Block";
        }

        private void LvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UsersSelectedIndexChanged();
        }

#endregion

#region Applications

        private ApplicationModel GetSelectedApplication()
        {
            ListViewItem item = lvApplications.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            return item?.Tag as ApplicationModel;
        }

        private void RefreshApplications()
        {
            lvApplications.BeginUpdate();

            lvApplications.Items.Clear();
            List<ApplicationModel> applications = ApplicationDAL.Instance.GetApplications(true);
            foreach (ApplicationModel model in applications)
            {
                ListViewItem item = new ListViewItem(model.Name) { Tag = model };
                item.SubItems.Add(model.Description);
                item.SubItems.Add(model.ConfigurationFile);
                item.SubItems.Add(model.Visible ? "Visible" : "Hidden");
                lvApplications.Items.Add(item);
            }

            lvApplications.EndUpdate();
            ApplicationsSelectedIndexChanged();
        }

        private void BtnApplicationsRefreshClick(object sender, EventArgs e)
        {
            RefreshApplications();
        }

        private void RegisterApplication()
        {
            if (new frmManageApplication().Execute())
            {
                RefreshApplications();
            }
        }

        private void BtnApplicationRegisterClick(object sender, EventArgs e)
        {
            RegisterApplication();
        }

        private void ModifyApplication()
        {
            ApplicationModel applicationModel = GetSelectedApplication();
            if (applicationModel != null && new frmManageApplication().Execute(applicationModel))
            {
                RefreshApplications();
            }
        }

        private void BtnApplicationModifyClick(object sender, EventArgs e)
        {
            ModifyApplication();
        }

        private void UnregisterApplication()
        {
            ApplicationModel applicationModel = GetSelectedApplication();
            if (applicationModel != null && MessageBox.Show($"Unregister application \"{applicationModel.Name}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ApplicationDAL.Instance.Delete(applicationModel.Id);
                RefreshApplications();
            }
        }

        private void BtnApplicationUnregisterClick(object sender, EventArgs e)
        {
            UnregisterApplication();
        }

        private void VisibleChangeApplication()
        {
            ApplicationModel applicationModel = GetSelectedApplication();
            if (applicationModel == null) return;

            string action = applicationModel.Visible ? "Hide" : "Show";
            if (MessageBox.Show($"{action} application \"{applicationModel.Name}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ApplicationDAL.Instance.ChangeVisibility(applicationModel.Id, !applicationModel.Visible);
                RefreshApplications();
            }
        }

        private void BtnApplicationVisibleChangeClick(object sender, EventArgs e)
        {
            VisibleChangeApplication();
        }

        private void ApplicationsSelectedIndexChanged()
        {
            ApplicationModel applicationModel = GetSelectedApplication();
            btnApplicationModify.Enabled = btnApplicationUnregister.Enabled = 
                btnApplicationVisibleChange.Enabled = applicationModel != null;
            bool applicationHidden = applicationModel != null && !applicationModel.Visible;
            btnApplicationVisibleChange.Image = applicationHidden
                ? Properties.Resources.show_app
                : Properties.Resources.hide_app;
            btnApplicationVisibleChange.Text = applicationHidden ? "Show" : "Hide";
        }

        private void LvApplicationsSelectedIndexChanged(object sender, EventArgs e)
        {
            ApplicationsSelectedIndexChanged();
        }

#endregion

    }
}
