namespace Jellyfin_Music.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new Jellyfin_Music.App());
        }
    }
}
