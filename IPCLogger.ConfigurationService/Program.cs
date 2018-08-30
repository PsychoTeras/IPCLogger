using IPCLogger.ConfigurationService.Forms;
using IPCLogger.ConfigurationService.Web;
using System;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
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