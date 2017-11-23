using System;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_Logic
{
    public class TextBoxProxy : TextBox
    {
        public Size SizeLimit { get; set; }

        public new Size Size { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Text = this.Text;
            base.Size = Size;
            using (Graphics graphics = this.CreateGraphics())
            {
                using (SolidBrush brush = new SolidBrush(this.ForeColor))
                {
                    StringFormat format = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                    int numberOfLettersInRow = (int)(Size.Width / 8);
                    string res = Text.CropWholeWords(numberOfLettersInRow);

                    if (RightToLeft == RightToLeft.Yes)
                    {
                        graphics.DrawString(res, Font, brush, new PointF(Size.Width - 2, 0), format);
                    }
                    else
                    {
                        graphics.DrawString(res, Font, brush, new PointF(0, 0));
                    }
                }
            }
        }
        
        public TextBoxProxy(string i_TextBoxText, Size i_SizeLimit)
        {
            SetStyle(ControlStyles.UserPaint, true);
            Text = i_TextBoxText;
            Enabled = false;
            SizeLimit = i_SizeLimit;
            SetTextBoxProxySize();
            Multiline = true;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular);
            ForeColor = Color.Black;
            if (i_TextBoxText != null && i_TextBoxText[0] >= 'א' && i_TextBoxText[0] <= 'ת')
            {
                RightToLeft = RightToLeft.Yes;
            }
        }

        public void SetTextBoxProxySize()
        {
            int numOfLinesInTextBox = 0;
            System.Drawing.Size defaultOrActualSize = new Size(Math.Min((int)(SizeLimit.Width * Font.Size), (int)(TextLength * Font.Size)), SizeLimit.Height);
            if (defaultOrActualSize.Width > SizeLimit.Width)
            {
                numOfLinesInTextBox = ((int)(defaultOrActualSize.Width / SizeLimit.Width)) + 1;
                Size = new Size(SizeLimit.Width, numOfLinesInTextBox * SizeLimit.Height);
            }
            else
            {
                Size = defaultOrActualSize;
            }
        }
    }  
}