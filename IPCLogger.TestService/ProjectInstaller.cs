using System.ComponentModel;
using System.Configuration.Install;

namespace IPCLogger.TestService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
