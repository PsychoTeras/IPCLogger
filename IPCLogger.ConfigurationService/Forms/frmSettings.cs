using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Web;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public sealed partial class frmSettings : Form
    {

#region Private fields

        private NotifyIcon _trayIcon;

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

            UserDAL dal = new UserDAL();
            dal.Register(new UserAuthDTO("a", "1"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                SelfHost.Instance.Start();
                using (new frmSettings())
                {
                    Application.Run();
                }
            }
            finally
            {
                SelfHost.Instance.Stop();
            }
        }

#endregion

#region Class methods

        public frmSettings()
        {
            InitializeComponent();


            //Create tray menu
            ContextMenu trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Configure", OnShowSettings);
            trayMenu.MenuItems.Add("Open console", OnOpenConsole);
            trayMenu.MenuItems.Add("-");
            //trayMenu.MenuItems.Add("About", OnShowAbout);
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayMenu.MenuItems[0].DefaultItem = true;

            //Create tray icon
            _trayIcon = new NotifyIcon();
            _trayIcon.ContextMenu = trayMenu;
            _trayIcon.DoubleClick += OnOpenConsole;
            _trayIcon.Icon = Icon;
            _trayIcon.Text = Text;
            _trayIcon.Visible = true;
        }

        private void OnOpenConsole(object sender, EventArgs e)
        {
            Process.Start(SelfHost.Instance.Url);
        }

        private void OnExit(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void OnShowSettings(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void ShowSettings()
        {
            Visible = ShowInTaskbar = true;
            BringToFront();
            Activate();
        }

        private void FrmSettingsClosing(object sender, FormClosingEventArgs e)
        {
            HideSettings();
            e.Cancel = true;
        }

        private void HideSettings()
        {
            Visible = ShowInTaskbar = false;
        }

        private void ExitApplication()
        {
            _trayIcon.Dispose();
            Application.Exit();
        }

        private void FrmSettingsKeyDown(object sender, KeyEventArgs e)
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

    }
}
