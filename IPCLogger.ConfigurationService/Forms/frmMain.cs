using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Web;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Forms
{
    public sealed partial class frmMain : Form
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                SelfHost.Instance.Start();
                using (new frmMain())
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
        }

        private void OnOpenConsole(object sender, EventArgs e)
        {
            Process.Start(SelfHost.Instance.Url);
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

    }
}
