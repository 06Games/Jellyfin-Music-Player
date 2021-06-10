using System.Threading.Tasks;
using Xamarin.Forms;

namespace Jellyfin_Music
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            DependencyService.Register<JellyfinService>();
            Task.Run(DependencyService.Get<JellyfinService>().Auth).ConfigureAwait(false);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
