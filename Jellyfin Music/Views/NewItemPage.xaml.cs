using Jellyfin_Music.Models;
using Jellyfin_Music.ViewModels;
using Xamarin.Forms;

namespace Jellyfin_Music.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}