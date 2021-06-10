using Jellyfin_Music.ViewModels;
using System.Linq;
using Xamarin.Forms;

namespace Jellyfin_Music.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
        public void SelectItem(object sender, System.EventArgs e) { _viewModel.SelectedItem = _viewModel.Items.FirstOrDefault(i => i.Id == (e as TappedEventArgs).Parameter as string); }
    }
}