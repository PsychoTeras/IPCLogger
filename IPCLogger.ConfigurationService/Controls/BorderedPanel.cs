using System.Drawing;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    public class BorderedPanel : Panel
    {
        public BorderedPanel() : base()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle r = new Rectangle(0, 0, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            e.Graphics.DrawRectangle(Pens.DarkGray, r);
        }
    }
}
