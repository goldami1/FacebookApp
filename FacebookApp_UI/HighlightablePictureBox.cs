using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_Logic
{
    public class HighlightablePictureBox : PictureBoxDecorator
    {
        public HighlightablePictureBox(PictureBox i_CoreDecorated, int i_FrameWidth = 4) : base(i_CoreDecorated)
        {
            BackColor = Color.Transparent;
            this.Size = new Size(CoreDecorated.Size.Width + (i_FrameWidth * 2), CoreDecorated.Size.Height + (i_FrameWidth * 2));
            Controls.Add(CoreDecorated);
            CoreDecorated.Location = new Point(CoreDecorated.Location.X + i_FrameWidth, CoreDecorated.Location.Y + i_FrameWidth);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            BackColor = Color.Blue;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Point mousePosition = MousePosition;

            Rectangle square = RectangleToScreen(ClientRectangle);
            if (!square.Contains(mousePosition))
            {
                BackColor = Color.Transparent;
            }

            base.OnMouseLeave(e);
        }
    }
}
