using System;
using System.Windows.Forms;
using System.Drawing;
using timers = System.Timers;

namespace FacebookApp_Logic
{
    public class ZoomPictureBox : PictureBoxDecorator
    {
        private const int k_FrameInterval = 25;
        private const int k_AnimationInterval = k_FrameInterval * 8;
        private Size m_CoreDecoratedSize;
        private Size m_OriginalSize;
        private timers.Timer m_FrameTimer;
        private timers.Timer m_AnimationTimer;

        public ZoomPictureBox(PictureBox i_CoreDecorated) : base(i_CoreDecorated)
        {
            this.Size = new Size(i_CoreDecorated.Size.Width + 6, i_CoreDecorated.Size.Height + 6);
            m_OriginalSize = this.Size;
            Controls.Add(CoreDecorated);
            CoreDecorated.Location = new Point(CoreDecorated.Location.X + 2, CoreDecorated.Location.Y + 2);

            m_FrameTimer = new timers.Timer();
            m_FrameTimer.Interval = k_FrameInterval;
            m_FrameTimer.Elapsed += timer_Elapsed;
            m_AnimationTimer = new timers.Timer();
            m_AnimationTimer.Interval = k_AnimationInterval;
            m_AnimationTimer.Elapsed += AnimationTimer_Elapsed;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            m_CoreDecoratedSize = CoreDecorated.Size;
            this.Size = m_OriginalSize;
            grow();

            base.OnMouseEnter(e);
        }

        private void AnimationTimer_Elapsed(object sender, EventArgs e)
        {
            m_FrameTimer.Stop();
            m_AnimationTimer.Stop();
        }

        private void grow()
        {
            m_AnimationTimer.Start();
            m_FrameTimer.Start();
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => this.Size = new Size(this.Size.Width + 1, this.Size.Height + 1)));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Point locationOnScreen = PointToScreen(this.Location);
            Point mousePosition = MousePosition;

            Rectangle square = RectangleToScreen(ClientRectangle);
            if (!square.Contains(mousePosition))
            {
                this.Size = m_OriginalSize;

                m_AnimationTimer.Stop();
                m_FrameTimer.Stop();
            }

            base.OnMouseLeave(e);
        }
    }
}
