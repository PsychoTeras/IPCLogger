using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace IPCLogger.View
{
    public partial class frmSelectProcess : Form
    {

#region Ctor

        public frmSelectProcess()
        {
            InitializeComponent();
        }

#endregion

#region Properties

        public Process SelectedHost { get; private set; }

#endregion

#region Class methods

        public bool Execute(Process[] hosts)
        {
            Initialize(hosts);
            return ShowDialog() == DialogResult.OK;
        }

        private void Initialize(Process[] hosts)
        {
            lvHosts.BeginUpdate();
            lvHosts.Items.Clear();

            foreach (Process host in hosts)
            {
                string hostName = string.Format("{0} [{1}]", host.ProcessName, host.Id);
                ListViewItem item = new ListViewItem(hostName) {Tag = host};
                item.SubItems.Add(GetCommandLine(host));
                lvHosts.Items.Add(item);
            }

            lvHosts.EndUpdate();
            lvHosts.Items[0].Selected = true;
            LvHostsSelectedIndexChanged(null, null);
        }

        private string GetCommandLine(Process process)
        {
            StringBuilder commandLine = new StringBuilder();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher
                ("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            {
                foreach (var obj in searcher.Get())
                {
                    commandLine.Append(obj["CommandLine"] + " ");
                }
            }
            return commandLine.ToString();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if (lvHosts.SelectedItems.Count > 0)
            {
                SelectedHost = (Process)lvHosts.SelectedItems[0].Tag;
                DialogResult = DialogResult.OK;
            }
        }

        private void FrmSelectProcessKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    BtnOkClick(null, null);
                    break;
            }
        }

        private void LvHostsSelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = lvHosts.SelectedItems.Count > 0;
        }

#endregion

    }
}
