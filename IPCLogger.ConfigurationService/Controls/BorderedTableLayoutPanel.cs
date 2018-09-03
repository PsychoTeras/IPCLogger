using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IPCLogger.ConfigurationService.Controls
{
    public class BorderedTableLayoutPanel : TableLayoutPanel
    {
        private readonly Pen _blackDarkGray = new Pen(Color.DarkGray, 1) { DashStyle = DashStyle.Dash };

        public BorderedTableLayoutPanel() : base()
        {
            DoubleBuffered = true;            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle r = new Rectangle(0, 0, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            e.Graphics.DrawRectangle(Pens.DarkGray, r);
        }

        protected override void OnCellPaint(TableLayoutCellPaintEventArgs e)
        {
            base.OnCellPaint(e);
            if (e.Row > 0)
            {
                e.Graphics.DrawLine(_blackDarkGray, e.CellBounds.Left, e.CellBounds.Top,
                    e.CellBounds.Left + e.CellBounds.Width - 1, e.CellBounds.Top);
            }
        }
    }
}
