using System;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_UI
{
    public class PictureButton : Button
    {
        private const int k_Spacing = 6;
        private static readonly Size sr_DefaultSize;
        private Label m_ButtonLabel;
        private PictureBox m_ButtonPictureBox;

        public new string Text
        {
            get { return m_ButtonLabel.Text; }
            set { m_ButtonLabel.Text = value; }
        }

        public string PictureURL
        {
            get { return m_ButtonPictureBox.ImageLocation; }
            set
            {
                m_ButtonPictureBox.ImageLocation = value;
            }
        }

        private void adjustSize(Size i_NewSize)
        {
            m_ButtonPictureBox.Size = new Size(i_NewSize.Width / 2, i_NewSize.Height / 2);
            m_ButtonLabel.Size = new Size(i_NewSize.Width - (k_Spacing * 2), k_Spacing * 2);
        }

        static PictureButton()
        {
            sr_DefaultSize = new Size(78, 78);
        }
        
        private void buttonComponent_Click(object sender, EventArgs e)
        {
            OnClick(new EventArgs());
        }

        private void buttonComponent_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(new EventArgs());
        }

        public Color LabelBackColor
        {
            get { return m_ButtonLabel.BackColor; }
            set { m_ButtonLabel.BackColor = value; }
        }

        public Color LabelTextColor
        {
            get { return m_ButtonLabel.ForeColor; }
            set { m_ButtonLabel.ForeColor = value; }
        }
      
        public PictureButton()
        {
            m_ButtonLabel = new Label();
            m_ButtonPictureBox = new PictureBox();
            m_ButtonPictureBox.BackColor = Color.Red;
            m_ButtonLabel.BackColor = Color.Yellow;
            m_ButtonPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            m_ButtonLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Size = sr_DefaultSize;
            Size size = this.Size;
            m_ButtonLabel.Text = string.Empty;
            adjustSize(sr_DefaultSize);
            Controls.Add(m_ButtonLabel);
            Controls.Add(m_ButtonPictureBox);
            m_ButtonPictureBox.Location = new Point(this.Size.Width / 4, (this.Size.Height / 4) + k_Spacing);
            m_ButtonLabel.Location = new Point(k_Spacing, k_Spacing);
            m_ButtonPictureBox.Click += buttonComponent_Click;
            m_ButtonPictureBox.MouseEnter += buttonComponent_MouseEnter;
            m_ButtonLabel.Click += buttonComponent_Click;
            m_ButtonLabel.MouseEnter += buttonComponent_MouseEnter;
        }
    }
}
