using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    public class TextBoxWithButton : TextBox
    {
        private Button _button;
        private bool _buttonCreated;

        [Browsable(true)]
        public event EventHandler ButtonClick;

        [Browsable(true)]
        public Image ButtonImage
        {
            get { return _button.Image; }
            set { _button.Image = value; }
        }

        [Browsable(true)]
        public string ButtonText
        {
            get { return _button.Text; }
            set { _button.Text = value; }
        }

        [Browsable(true)]
        public Font ButtonFont
        {
            get { return _button.Font; }
            set { _button.Font = value; }
        }

        [Browsable(true), DefaultValue(typeof(Padding), "0, 0, 0, 0")]
        public Padding ButtonPadding
        {
            get { return _button.Padding; }
            set { _button.Padding = value; }
        }

        public TextBoxWithButton()
        {
            CreteButton();
        }

        private void CreteButton()
        {
            if (!_buttonCreated)
            {
                _buttonCreated = true;
                _button = new Button();
                _button.TabStop = false;
                _button.Cursor = Cursors.Default;
                _button.Click += (s, e) =>
                {
                    if (Enabled)
                    {
                        Focus();
                        ButtonClick?.Invoke(this, e);
                    }
                };
                Controls.Add(_button);
            }
        }

        private void ResizeButton()
        {
            _button.Size = new Size(25, ClientSize.Height + 2);
            _button.Location = new Point(ClientSize.Width - _button.Width + 1, -1);
            DrawingHelper.SendMessage(Handle, 0xd3, 2, _button.Width << 16);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            _button.Enabled = Enabled;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeButton();
        }
    }
}
