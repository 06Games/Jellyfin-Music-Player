using Jellyfin_Music.Models;
using Jellyfin_Music.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Jellyfin_Music.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }

        public string ItemType { get; set; } = "MusicAlbum";
        public IEnumerable<string> SortBy { get; set; } = new[] { "AlbumArtist", "SortName" };

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (Jellyfin.UserInfo is null) return;
            IsBusy = true;

            Items.Clear();
            var items = await Jellyfin.Items.GetItemsAsync(Jellyfin.UserInfo.Id, includeItemTypes: new[] { ItemType }, recursive: true, sortBy: SortBy).ConfigureAwait(false);

            foreach (var item in items.Items)
            {
                Jellyfin.Sdk.FileResponse img = null;
                try { img = await Jellyfin.Image.GetItemImageAsync(item.Id, global::Jellyfin.Sdk.ImageType.Primary).ConfigureAwait(false); } catch (System.Exception e) { System.Diagnostics.Debug.Fail(e.Message, e.ToString()); }
                await Device.InvokeOnMainThreadAsync(() => Items.Add(new Item
                {
                    Id = item.Id.ToString(),
                    Text = item.Name,
                    Artwork = ImageSource.FromStream(() =>
                    {
                        var ms = new System.IO.MemoryStream();
                        if (img is null || img.IsPartial) return ms;
                        img.Stream.CopyTo(ms);
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        return ms;
                    })
                })).ConfigureAwait(false);
            }

            IsBusy = false;
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj) => await Shell.Current.GoToAsync(nameof(NewItemPage));
        async void OnItemSelected(Item item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.AlbumId)}={item.Id}"); // This will push the ItemDetailPage onto the navigation stack
        }
    }
}