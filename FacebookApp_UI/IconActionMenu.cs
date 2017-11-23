using System;
using System.Drawing;

namespace FacebookApp_UI
{
    public class IconActionMenu : IIconMenuItem
    {
        private PictureButton m_MenuIcon;
        private Action m_Callback;

        public IconActionMenu()
        {
            m_MenuIcon = new PictureButton();
            m_MenuIcon.BackColor = System.Drawing.Color.BlueViolet;
            m_MenuIcon.LabelBackColor = Color.White;
            m_MenuIcon.Click += menuIcon_Click;
        }

        public Action Callback
        {
            get { return m_Callback; }
            set { m_Callback = value; }
        }

        public string IconURL
        {
            get { return m_MenuIcon.PictureURL; }
            set { m_MenuIcon.PictureURL = value; }
        }

        public PictureButton MenuIcon
        {
            get { return m_MenuIcon; }
            set { m_MenuIcon = value; }
        }

        public string MenuLabel
        {
            get { return m_MenuIcon.Text; }
            set { m_MenuIcon.Text = value; }
        }

        private void menuIcon_Click(object sender, EventArgs e)
        {
            Show();
        }

        public void Show()
        {
            if(m_Callback != null)
            {
                m_Callback.Invoke();
            }
        }
    }
}
