using System;
using System.Drawing;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    public class TextBoxWithButton : TextBox
    {
        private bool _buttonCreated;

        public TextBoxWithButton()
        {
        }

        private void CreteButton()
        {
            if (!_buttonCreated)
            {
                _buttonCreated = true;
                Button btn = new Button();
                btn.Size = new Size(25, ClientSize.Height + 2);
                btn.Location = new Point(ClientSize.Width - btn.Width, -1);
                //btn.Image = Properties.Resources.star;
                Controls.Add(btn);
                DrawingHelper.SendMessage(Handle, 0xd3, 2, btn.Width << 16);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            CreteButton();
            base.OnVisibleChanged(e);
        }
    }
}
