using System;
using System.Windows.Forms;

namespace FacebookApp_UI
{
    public partial class ThemeForm : Form
    {
        public event EventHandler ThemeSelected;

        public class ThemeSelectedEventArgs : EventArgs
        {
            public string ThemePictureURL { get; set; }
        }

        public ThemeForm()
        {
            InitializeComponent();
            pictureBoxTheme1.Click += pictureBoxTheme_Click;
            pictureBoxTheme2.Click += pictureBoxTheme_Click;
            pictureBoxTheme3.Click += pictureBoxTheme_Click;
            pictureBoxTheme4.Click += pictureBoxTheme_Click;
            pictureBoxTheme5.Click += pictureBoxTheme_Click;
            pictureBoxTheme6.Click += pictureBoxTheme_Click;
            setPictureBoxThemes();
        }

        private void setPictureBoxThemes()
        {
            pictureBoxTheme1.ImageLocation = "ThemeAaronBlaise.jpg";
            pictureBoxTheme2.ImageLocation = "ThemeDeepBlue.jpg";
            pictureBoxTheme3.ImageLocation = "ThemeFeathers.jpg";
            pictureBoxTheme4.ImageLocation = "ThemeMozaic.jpg";
            pictureBoxTheme5.ImageLocation = "ThemeSpace.jpg";
            pictureBoxTheme6.ImageLocation = "ThemeBricks.jpg";
        }

        private void onPictureBoxClicked(ThemeSelectedEventArgs i_ThemeSelectedEventArgs)
        {
             ThemeSelected(this, i_ThemeSelectedEventArgs);
        }

        private void pictureBoxTheme_Click(object sender, EventArgs e)
        {
            ThemeSelectedEventArgs eventArgs = new ThemeSelectedEventArgs();
            eventArgs.ThemePictureURL = (sender as PictureBox).ImageLocation;
            onPictureBoxClicked(eventArgs);
            this.Hide();
        }
    }
}
