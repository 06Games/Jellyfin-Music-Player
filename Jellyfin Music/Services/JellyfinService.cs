using Jellyfin.Sdk;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Jellyfin_Music
{
    /// <summary>
    /// Jellyfin service.
    /// </summary>
    public class JellyfinService
    {
        public readonly SdkClientSettings SdkClientSettings;
        private readonly HttpClient _httpClient;
        private UserDto _user;
        public UserDto UserInfo => _user;

        public readonly ISystemClient System;
        public readonly IUserClient User;
        public readonly IUserViewsClient UserViews;
        public readonly IUserLibraryClient UserLibrary;

        public readonly ILibraryClient Library;
        public readonly IImageClient Image;
        public readonly IItemsClient Items;
        public readonly IAudioClient Audio;


        /// <summary>
        /// Initializes a new instance of the <see cref="JellyfinService"/> class.
        /// </summary>
        public JellyfinService()
        {
            SdkClientSettings = new SdkClientSettings();
            SdkClientSettings.InitializeClientSettings(AppInfo.Name, AppInfo.VersionString, Device.RuntimePlatform, Guid.NewGuid().ToString("N"));

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(SdkClientSettings.ClientName, SdkClientSettings.ClientVersion));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            System = new SystemClient(SdkClientSettings, _httpClient);
            User = new UserClient(SdkClientSettings, _httpClient);
            UserViews = new UserViewsClient(SdkClientSettings, _httpClient);
            UserLibrary = new UserLibraryClient(SdkClientSettings, _httpClient);

            Library = new LibraryClient(SdkClientSettings, _httpClient);
            Image = new ImageClient(SdkClientSettings, _httpClient);
            Items = new ItemsClient(SdkClientSettings, _httpClient);
            Audio = new AudioClient(SdkClientSettings, _httpClient);
        }


        /// <summary>
        /// Run the sample.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Auth()
        {
            var validServer = false;
            do
            {
                var host = SdkClientSettings.BaseUrl = "https://demo.jellyfin.org/stable";
                try
                {
                    // Get public system info to verify that the url points to a Jellyfin server.
                    var systemInfo = await System.GetPublicSystemInfoAsync().ConfigureAwait(false);
                    validServer = true;
                    Debug.WriteLine($"Connected to {host}");
                    Debug.WriteLine($"Server Name: {systemInfo.ServerName}");
                    Debug.WriteLine($"Server Version: {systemInfo.Version}");
                }
                catch (InvalidOperationException ex) { Debug.Fail("Invalid url", ex.ToString()); }
                catch (Jellyfin.Sdk.SystemException ex) { Debug.Fail($"Error connecting to {host}", ex.ToString()); }
            }
            while (!validServer);

            var validUser = false;
            do
            {
                try
                {
                    var username = "demo";
                    var password = "";

                    Debug.WriteLine($"Logging into {SdkClientSettings.BaseUrl}");
                    var authenticationResult = await User.AuthenticateUserByNameAsync(new AuthenticateUserByName { Username = username, Pw = password }).ConfigureAwait(false); // Authenticate user.

                    SdkClientSettings.AccessToken = authenticationResult.AccessToken;
                    _user = authenticationResult.User;
                    Debug.WriteLine("Authentication success.");
                    Debug.WriteLine($"Welcome to Jellyfin - {_user.Name}");
                    validUser = true;
                }
                catch (UserException ex) { Debug.Fail("Error authenticating.", ex.ToString()); }
            }
            while (!validUser);
        }


        public async Task<ImageSource> TryGetArtwork(Guid album)
        {
            FileResponse img = null;
            try { img = await Image.GetItemImageAsync(album, ImageType.Primary).ConfigureAwait(false); }
            catch (Exception e) { Debug.Fail(e.Message, e.ToString()); }

            return ImageSource.FromStream(() =>
            {
                var ms = new System.IO.MemoryStream();
                if (img is null || img.IsPartial) return ms;
                img.Stream.CopyTo(ms);
                ms.Seek(0, global::System.IO.SeekOrigin.Begin);
                return ms;
            });
        }
    }
}
