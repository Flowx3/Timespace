using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TimeSpace
{
    public class CustomMaterialStyleComboBox : ComboBox
    {
        private int _comboboxHeight = 30;

        public int ComboboxHeight
        {
            get { return _comboboxHeight; }
            set
            {
                _comboboxHeight = value;
                this.ItemHeight = _comboboxHeight;
                this.Invalidate();
            }
        }

        public CustomMaterialStyleComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = Color.FromArgb(45, 45, 48); // Dark background color
            this.ForeColor = Color.White;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            // Determine if the item is selected
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Set background color
            Color backgroundColor = isSelected ? Color.FromArgb(80, 80, 80) : this.BackColor;

            // Fill background
            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            }

            // Draw text
            string text = this.Items[e.Index].ToString();
            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            {
                e.Graphics.DrawString(text, this.Font, textBrush, e.Bounds);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);

            // Draw text
            Rectangle rect = new Rectangle(2, 2, this.Width - 20, this.Height - 4);
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, rect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Draw custom dropdown arrow
            int arrowX = this.Width - 15;
            int arrowY = (this.Height - 5) / 2;
            using (Brush brush = new SolidBrush(Color.White))
            {
                Point[] points = {
                        new Point(arrowX, arrowY),
                        new Point(arrowX + 5, arrowY + 5),
                        new Point(arrowX - 5, arrowY + 5)
                    };
                e.Graphics.FillPolygon(brush, points);
            }

            // Draw custom border
            using (Pen pen = new Pen(Color.DarkGray))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_PAINT = 0x000F;
            const int WM_CTLCOLOR = 0x0133;
            const int WM_ERASEBKGND = 0x0014;

            if (m.Msg == WM_ERASEBKGND || m.Msg == WM_CTLCOLOR)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                using (Graphics g = Graphics.FromHwnd(this.Handle))
                {
                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                    ControlPaint.DrawBorder(g, rect, Color.DarkGray, ButtonBorderStyle.Solid);
                }
            }
        }
    }
}