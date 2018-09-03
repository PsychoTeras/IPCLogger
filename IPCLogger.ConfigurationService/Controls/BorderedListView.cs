using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace IPCLogger.ConfigurationService.Controls
{
    public class BorderedListView : ListView
    {
        private bool _compactGridLines;
        private bool _forceResizeColumn;
        private bool _canResizeLastColumn;

        private static readonly Pen _penBorder = new Pen(DrawingHelper.BorderColor);
        private static readonly Brush _brushSelected = new SolidBrush(Color.FromArgb(235, 235, 235));
        private static readonly Brush _brushChecked = new SolidBrush(Color.FromArgb(125, 125, 125));

        public new ListViewItemCollectionNewEvent Items;

        [Browsable(true), DefaultValue(false)]
        public bool CompactGridLines
        {
            get { return _compactGridLines; }
            set
            {
                _compactGridLines = value;
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (!DesignMode && m.Msg == 0x0030) //WM_SETFONT
            {
                Font f = new Font(Font.Name, Font.SizeInPoints + 3);
                m.WParam = f.ToHfont();
            }
            if (DesignMode || BorderStyle == BorderStyle.None || !DrawingHelper.WndProcTextBoxControl(this, m, false))
            {
                base.WndProc(ref m);
            }
        }

        public BorderedListView()
        {
            Items = new ListViewItemCollectionNewEvent(this);

            OwnerDraw = true;
            DoubleBuffered = true;
            ResizeColumns();
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            if (View != View.Details)
            {
                base.OnDrawColumnHeader(e);
                return;
            }

            Graphics g = e.Graphics;
            int width = e.ColumnIndex == Columns.Count - 1 ? Width : e.Bounds.Width;
            Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, width, e.Bounds.Height);
            g.FillRectangle(Brushes.SteelBlue, rect);

            if (e.ColumnIndex != Columns.Count - 1)
            {
                g.DrawLine(_penBorder, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y,
                    e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height);
            }
            g.DrawLine(_penBorder, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1,
                e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height - 1);

            using (Brush brush = new SolidBrush(ForeColor))
            {
                StringFormat tf = new StringFormat
                {
                    FormatFlags = StringFormatFlags.NoWrap,
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.TextRenderingHint = DrawingHelper.ControlRenderingHint;
                g.DrawString(e.Header.Text, Font, Brushes.White, e.Bounds, tf);
            }
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (View != View.Details)
            {
                base.OnDrawItem(e);
                return;
            }

            Graphics g = e.Graphics;
            if (e.Item.Selected)
            {
                g.FillRectangle(_brushSelected, e.Bounds);
            }
            else
            {
                if (e.Item.Checked)
                {
                    g.FillRectangle(_brushChecked, e.Bounds);
                }
                else
                {
                    e.DrawBackground();
                }
            }

            int xShiftFirstCol = 0;
            if (CheckBoxes)
            {
                int y = e.Item.Bounds.Y + (e.Item.Bounds.Height - 14) / 2 + 1;
                CheckBoxState state = e.Item.Checked
                    ? CheckBoxState.CheckedNormal
                    : CheckBoxState.UncheckedNormal;
                CheckBoxRenderer.DrawCheckBox(g, new Point(e.Item.Bounds.X + 5, y), state);
                xShiftFirstCol = 22;
            }

            for (int i = 0; i < e.Item.SubItems.Count; i++)
            {
                ListViewItem.ListViewSubItem item = e.Item.SubItems[i];
                Rectangle rect = new Rectangle(item.Bounds.X + xShiftFirstCol, item.Bounds.Y,
                    Columns[i].Width - xShiftFirstCol, item.Bounds.Height);

                Color foreColor = e.Item.Checked ? Color.White : ForeColor;
                using (Brush brush = new SolidBrush(foreColor))
                {
                    StringFormat tf = new StringFormat
                    {
                        FormatFlags = StringFormatFlags.NoWrap,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.TextRenderingHint = DrawingHelper.ControlRenderingHint;
                    g.DrawString(item.Text, Font, brush, rect, tf);
                }

                xShiftFirstCol = 0;

                if (_compactGridLines && i > 0)
                {
                    g.DrawLine(_penBorder, rect.Left - 1, rect.Top, rect.Left - 1, rect.Bottom);
                }
            }

            if (_compactGridLines)
            {
                Rectangle rect = new Rectangle(e.Item.Bounds.X - 1, e.Item.Bounds.Y - 1,
                    e.Item.Bounds.Width + 1, e.Item.Bounds.Height);
                g.DrawRectangle(_penBorder, rect);
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeColumns();
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            if (View != View.Details) return;

            bool forceResizeColumn = _forceResizeColumn;
            _forceResizeColumn = false;
            if (e.ColumnIndex == Columns.Count - 1 && !forceResizeColumn)
            {
                e.Cancel = !_canResizeLastColumn;
                return;
            }

            int width = ClientRectangle.Width;
            if (Columns.Count > 1)
            {
                width -= Columns.Cast<ColumnHeader>().
                    Take(Columns.Count - 1).
                    Sum(c => c.Index == e.ColumnIndex ? e.NewWidth : c.Width);
            }

            _canResizeLastColumn = true;
            Columns[Columns.Count - 1].Width = width;
            _canResizeLastColumn = false;
        }

        public void ResizeColumns()
        {
            if (Columns.Count > 0 && View == View.Details)
            {
                _forceResizeColumn = true;
                OnColumnWidthChanging(new ColumnWidthChangingEventArgs(0, Columns[0].Width));
            }
        }

        public class ListViewItemCollectionNewEvent : ListViewItemCollection
        {
            private BorderedListView _owner;

            public ListViewItemCollectionNewEvent(BorderedListView owner) : base(owner)
            {
                _owner = owner;
            }

            public new ListViewItem Add(ListViewItem item)
            {
                base.Add(item);
                _owner.ResizeColumns();
                return item;
            }

            public new void AddRange(ListViewItem[] items)
            {
                base.AddRange(items);
                _owner.ResizeColumns();
            }

            public new void Remove(ListViewItem item)
            {
                base.Remove(item);
                _owner.ResizeColumns();
            }

            public new void RemoveAt(int index)
            {
                base.RemoveAt(index);
                _owner.ResizeColumns();
            }
        }
    }
}
