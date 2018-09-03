using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    class MainTabControl : TabControl
    {
        private int _tabRigthSeparator = 7;

        private Color _backColor = Color.FromArgb(222, 222, 222);
        private Color _tabColorActive = Color.FromArgb(222, 222, 222);
        private Color _tabColorInactive = Color.FromArgb(240, 240, 240);

        private SolidBrush _tabBrushActive;
        private SolidBrush _tabBrushInactive;

        private readonly StringFormat _tabHeaderTxtFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        [Browsable(true), DefaultValue(15)]
        public int TabRigthSeparator
        {
            get { return _tabRigthSeparator; }
            set
            {
                _tabRigthSeparator = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        public Color TabColorActive
        {
            get { return _tabColorActive; }
            set
            {
                _tabColorActive = value;
                _backColor = value;
                InitializeGraphics();
                Invalidate();
            }
        }

        [Browsable(true)]
        public Color TabColorInactive
        {
            get { return _tabColorInactive; }
            set
            {
                _tabColorInactive = value;
                InitializeGraphics();
                Invalidate();
            }
        }

        public MainTabControl()
        {
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            InitializeGraphics();
        }

        private void InitializeGraphics()
        {
            if (_tabBrushActive != null)
            {
                _tabBrushActive.Dispose();
            }
            _tabBrushActive = new SolidBrush(_tabColorActive);
            if (_tabBrushInactive != null)
            {
                _tabBrushInactive.Dispose();
            }
            _tabBrushInactive = new SolidBrush(_tabColorInactive);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(_backColor);

            using (Brush parentBackBrush = new SolidBrush(Parent.BackColor))
            {
                Rectangle rectHeader = new Rectangle(0, 0, Width, ItemSize.Height + 2);
                g.FillRectangle(parentBackBrush, rectHeader);

                TabPage[] tabs = TabPages.OfType<TabPage>().ToArray();
                for (int i = 0; i < tabs.Length; i++)
                {
                    TabPage tabPage = tabs[i];

                    Rectangle rect = GetTabRect(i);
                    if (i == 0)
                    {
                        rect = new Rectangle(0, rect.Y, rect.Width + rect.X, rect.Height);
                    }
                    Brush backBrush = SelectedTab == tabPage
                        ? _tabBrushActive
                        : _tabBrushInactive;
                    g.FillRectangle(backBrush, rect);

                    Rectangle rectDivider = new Rectangle(rect.Left + rect.Width - _tabRigthSeparator, rect.Y, 
                        _tabRigthSeparator, rect.Height);
                    g.FillRectangle(parentBackBrush, rectDivider);

                    using (Brush brushText = new SolidBrush(ForeColor))
                    {
                        RectangleF rectText = new RectangleF(rect.Left, rect.Y, rect.Width - _tabRigthSeparator, rect.Height);
                        g.TextRenderingHint = DrawingHelper.ControlRenderingHint;
                        g.DrawString(tabPage.Text, Font, brushText, rectText, _tabHeaderTxtFormat);
                    }
                }
            }
        }
    }
}
