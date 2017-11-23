using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_Logic
{
    public abstract class PictureBoxDecorator : PictureBox
    {
        private Size m_CurrentSize;

        protected PictureBox CoreDecorated { get; set; }

        protected PictureBoxDecorator(PictureBox i_CoreDecorated)
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SizeMode = PictureBoxSizeMode.StretchImage;
            CoreDecorated = i_CoreDecorated;
            m_CurrentSize = Size;
        }
    }
}
