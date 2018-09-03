using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    public class HorizontalDivider : Control
    {
        [Browsable(false)]
        public new int Height
        {
            get { return Height; }
            set { base.Height = 1; }
        }

        public HorizontalDivider() : base()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(Pens.DarkGray, e.ClipRectangle.Left, e.ClipRectangle.Top,
                e.ClipRectangle.Left + e.ClipRectangle.Width, e.ClipRectangle.Top);
        }
    }
}
