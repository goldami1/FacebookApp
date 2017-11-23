using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace FacebookApp_UI
{
    public class IconMenuContainer : IIconMenuItem
    {
        private const int k_Spacing = 15;
        private static readonly Size sr_ButtonBackSize;
        private Form m_MenuForm;
        private LinkedList<IIconMenuItem> m_MenuItems;
        private Point m_Offset;
        private Button m_ButtonBack;
        private PictureButton m_PictureButton;
        
        public static Size MenuSize { get; set; }

        public string MenuLabel
        {
            get { return m_PictureButton.Text; }
            set { m_PictureButton.Text = value; }
        }

        public string IconURL
        {
            get { return m_PictureButton.PictureURL; }
            set { m_PictureButton.PictureURL = value; }
        }

        public PictureButton MenuIcon
        {
            get { return m_PictureButton; }
            set { m_PictureButton = value; }
        }

        static IconMenuContainer()
        {
            sr_ButtonBackSize = new Size(60, 20);
        }

        public IconMenuContainer()
        {
            m_Offset = new Point(k_Spacing, k_Spacing);
            m_MenuForm = new Form();
            m_MenuForm.FormClosing += m_MenuForm_Closing;

            if (!MenuSize.IsEmpty)
            {
                m_MenuForm.Size = MenuSize;
            }

            m_MenuForm.FormBorderStyle = FormBorderStyle.None;
            m_MenuForm.StartPosition = FormStartPosition.CenterScreen;
            m_ButtonBack = new Button();
            m_ButtonBack.Size = sr_ButtonBackSize;
            m_ButtonBack.Text = "Back";
            m_MenuForm.Controls.Add(m_ButtonBack);
            m_ButtonBack.Location = m_Offset;
            m_ButtonBack.Click += buttonBack_Click;
            m_Offset.Y += m_ButtonBack.Size.Height + k_Spacing;
            m_PictureButton = new PictureButton();
            m_PictureButton.Click += menuIcon_Click;
            m_PictureButton.BackColor = Color.BlueViolet;
            m_PictureButton.LabelBackColor = Color.White;
            m_MenuItems = new LinkedList<IIconMenuItem>();
        }

        private void m_MenuForm_Closing(object sender, FormClosingEventArgs e)
        {
            m_MenuForm.Hide();
            e.Cancel = true;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            m_MenuForm.Hide();
        }

        private void menuIcon_Click(object sender, EventArgs e)
        {
            Show();
        }

        public void Show()
        {
            m_MenuForm.ShowDialog();
        }

        public void AddMenuItem(IIconMenuItem i_MenuItem)
        {
            m_MenuItems.AddLast(i_MenuItem);
            m_MenuForm.Controls.Add(i_MenuItem.MenuIcon);
            Point insertLocation = new Point();

            if (m_Offset.X + m_PictureButton.Width + k_Spacing < m_MenuForm.Size.Width)
            {
                insertLocation = m_Offset;
            }
            else
            {
                m_Offset.X = k_Spacing;
                m_Offset.Y += m_PictureButton.Height + k_Spacing;
                insertLocation = m_Offset;
            }

            PictureButton menuIcon = i_MenuItem.MenuIcon;
            menuIcon.Location = insertLocation;
            m_Offset.X += m_PictureButton.Width + k_Spacing;
        }
    }
}
