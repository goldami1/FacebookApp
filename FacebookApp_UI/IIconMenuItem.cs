namespace FacebookApp_UI
{
    public interface IIconMenuItem
    {
        string IconURL { get; set; }

        string MenuLabel { get; set; }

        PictureButton MenuIcon { get; set; }

        void Show();
    }
}
