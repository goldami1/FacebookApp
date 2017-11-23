using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_Logic
{
    public class FramedPictureBox : PictureBoxDecorator
    {
        public FramedPictureBox(PictureBox i_CoreDecorated, int i_FrameWidth = 2) : base(i_CoreDecorated)
        {
            BackColor = Color.White;
            this.Size = new Size(CoreDecorated.Size.Width + (i_FrameWidth * 2), CoreDecorated.Size.Height + (i_FrameWidth * 2));
            Controls.Add(CoreDecorated);
            CoreDecorated.Location = new Point(CoreDecorated.Location.X + i_FrameWidth, CoreDecorated.Location.Y + i_FrameWidth);
        }
    }
}
