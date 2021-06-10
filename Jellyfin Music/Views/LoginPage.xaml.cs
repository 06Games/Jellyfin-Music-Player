using Jellyfin_Music.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jellyfin_Music.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }
    }
}