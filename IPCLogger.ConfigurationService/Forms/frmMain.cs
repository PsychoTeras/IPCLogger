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
            _trayIcon.DoubleClick += MainFormOnShow;
            _trayIcon.Icon = Icon;
            _trayIcon.Text = Text;
            _trayIcon.Visible = true;

            RefreshRoles();
            RefreshUsers();
            RefreshLoggers();
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
            foreach (UserModel user in users)
            {
                ListViewItem item = new ListViewItem(user.UserName) { Tag = user };
                item.SubItems.Add(_roles[user.RoleId].ToString());
                item.SubItems.Add(user.Blocked ? "YES" : "NO");
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
            UserModel user = GetSelectedUser();
            if (user != null && new frmManageUser().Execute(_roles.Values.ToArray(), user))
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
            UserModel user = GetSelectedUser();
            if (user != null && MessageBox.Show($"Delete user \"{user.UserName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserDAL.Instance.Delete(user.Id);
                RefreshUsers();
            }
        }

        private void BtnUserDelete_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void BlockChangeUser()
        {
            UserModel user = GetSelectedUser();
            if (user == null) return;

            string action = user.Blocked ? "Unblock" : "Block";
            if (MessageBox.Show($"{action} user \"{user.UserName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserDAL.Instance.ChangeBlock(user.Id, !user.Blocked);
                RefreshUsers();
            }
        }

        private void BtnUserBlockChange_Click(object sender, EventArgs e)
        {
            BlockChangeUser();
        }

        private void UsersSelectedIndexChanged()
        {
            UserModel user = GetSelectedUser();
            btnUserModify.Enabled = btnUserDelete.Enabled = btnUserBlockChange.Enabled = user != null;
            bool userBlocked = user != null && user.Blocked;
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

#region Loggers

        private LoggerModel GetSelectedLogger()
        {
            ListViewItem item = lvLoggers.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            return item?.Tag as LoggerModel;
        }

        private void RefreshLoggers()
        {
            lvLoggers.BeginUpdate();

            lvLoggers.Items.Clear();
            List<LoggerModel> loggers = LoggerDAL.Instance.GetLoggers(true);
            foreach (LoggerModel logger in loggers)
            {
                ListViewItem item = new ListViewItem(logger.ApplicationName) { Tag = logger };
                item.SubItems.Add(logger.Description);
                item.SubItems.Add(logger.ConfigurationFile);
                item.SubItems.Add(logger.Visible ? "Visible" : "Hidden");
                lvLoggers.Items.Add(item);
            }

            lvLoggers.EndUpdate();
            LoggersSelectedIndexChanged();
        }

        private void BtnLoggersRefresh_Click(object sender, EventArgs e)
        {
            RefreshLoggers();
        }

        private void RegisterLogger()
        {
            if (new frmManageLogger().Execute())
            {
                RefreshLoggers();
            }
        }

        private void BtnLoggerRegister_Click(object sender, EventArgs e)
        {
            RegisterLogger();
        }

        private void ModifyLogger()
        {
            LoggerModel logger = GetSelectedLogger();
            if (logger != null && new frmManageLogger().Execute(logger))
            {
                RefreshLoggers();
            }
        }

        private void BtnLoggerModify_Click(object sender, EventArgs e)
        {
            ModifyLogger();
        }

        private void UnregisterLogger()
        {
            LoggerModel logger = GetSelectedLogger();
            if (logger != null && MessageBox.Show($"Unregister logger \"{logger.ApplicationName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LoggerDAL.Instance.Delete(logger.Id);
                RefreshLoggers();
            }
        }

        private void BtnLoggerUnregister_Click(object sender, EventArgs e)
        {
            UnregisterLogger();
        }

        private void VisibleChangeLogger()
        {
            LoggerModel logger = GetSelectedLogger();
            if (logger == null) return;

            string action = logger.Visible ? "Hide" : "Show";
            if (MessageBox.Show($"{action} logger \"{logger.ApplicationName}\"?", "Confirmation",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LoggerDAL.Instance.ChangeVisibility(logger.Id, !logger.Visible);
                RefreshLoggers();
            }
        }

        private void BtnLoggerVisibleChange_Click(object sender, EventArgs e)
        {
            VisibleChangeLogger();
        }

        private void LoggersSelectedIndexChanged()
        {
            LoggerModel logger = GetSelectedLogger();
            btnLoggerModify.Enabled = btnLoggerUnregister.Enabled = btnLoggerVisibleChange.Enabled = logger != null;
            bool loggerHidden = logger != null && !logger.Visible;
            btnLoggerVisibleChange.Image = loggerHidden
                ? Properties.Resources.show_app
                : Properties.Resources.hide_app;
            btnLoggerVisibleChange.Text = loggerHidden ? "Show" : "Hide";
        }

        private void LvLoggers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoggersSelectedIndexChanged();
        }

#endregion

    }
}
