namespace Jellyfin_Music.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public Xamarin.Forms.ImageSource Artwork { get; set; }
    }
}