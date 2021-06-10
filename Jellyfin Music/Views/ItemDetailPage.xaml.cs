using Jellyfin_Music.ViewModels;
using Xamarin.Forms;

namespace Jellyfin_Music.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }

        private async void PlayMusic(object sender, ItemTappedEventArgs e)
        {
            var track = e.Item as Track;
            var audio = await DependencyService.Get<JellyfinService>().Audio.GetAudioStreamAsync(track.Id, audioCodec: "mp3").ConfigureAwait(false);
            await MediaManager.CrossMediaManager.Current.Play(audio.Stream, $"{track.Name}.mp3");
        }
    }
}