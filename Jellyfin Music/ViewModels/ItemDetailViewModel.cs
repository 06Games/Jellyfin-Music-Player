using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Jellyfin_Music.ViewModels
{
    public class Album
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public string ArtistsString => string.Join(", ", Artists);
        public ImageSource Artwork { get; set; }
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
    }
    public class Track
    {
        public Guid Id { get; set; }
        public Guid AlbumID { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public string ArtistsString => string.Join(", ", Artists);
        public string Codec { get; set; }
    }

    [QueryProperty(nameof(AlbumId), nameof(AlbumId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private Album album;
        public Album Album
        {
            get => album;
            set => SetProperty(ref album, value);
        }
        private string albumId;
        public string AlbumId
        {
            get => albumId;
            set
            {
                albumId = value;
                LoadItemId(albumId);
            }
        }
        public async void LoadItemId(string itemId)
        {
            try
            {
                var album = await Jellyfin.UserLibrary.GetItemAsync(Jellyfin.UserInfo.Id, new Guid(itemId)).ConfigureAwait(false);
                var artwork = await Jellyfin.TryGetArtwork(album.Id);
                var tracks = await Jellyfin.Items.GetItemsAsync(Jellyfin.UserInfo.Id, parentId: album.Id).ConfigureAwait(false);

                Album = new Album
                {
                    Id = album.Id,
                    Name = album.Name,
                    Artists = album.AlbumArtists.Select(a => a.Name),
                    Artwork = artwork,
                    Tracks = new ObservableCollection<Track>(tracks.Items.Select(track => new Track
                    {
                        Id = track.Id,
                        AlbumID = album.Id,
                        Number = track.IndexNumber?.ToString(),
                        Name = track.Name,
                        Artists = track.Artists,
                        Codec = track.MediaSources?.FirstOrDefault()?.Container ?? "mp3"
                    }))
                };
            }
            catch (Exception e) { Debug.Fail("Failed to Load Item", e.ToString()); }
        }
    }
}
