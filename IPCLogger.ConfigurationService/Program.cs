using IPCLogger.ConfigurationService.Forms;
using IPCLogger.ConfigurationService.Web;
using System;
using System.Windows.Forms;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;

namespace IPCLogger.ConfigurationService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            UserDAL dal = new UserDAL();
            dal.Register(new UserAuthDTO { UserName = "a", PasswordHash = "c4ca4238a0b923820dcc509a6f75849b" });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                SelfHost.Instance.Start();
                Application.Run(new frmConfigurationService());
            }
            finally
            {
                SelfHost.Instance.Stop();
            }
        }
    }
}