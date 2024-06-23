namespace YoutubeAutomation
{
    using Newtonsoft.Json;
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;
    using Google.Apis.Auth.OAuth2;
    using JsonSerializer = System.Text.Json.JsonSerializer;
    using Google.Apis.Util.Store;
    using Google.Apis.YouTube.v3;
    using Google.Apis.Services;
    using System.Net;

    public class YoutubeManager
    {
        public HttpClient ApiHttpClient { get; set; }

        public YoutubeManager()
        {
            ApiHttpClient = new HttpClient();
            //ApiHttpClient.BaseAddress = 
        }

        public void Main()
        {
            Test2();

            var httpClient = new HttpClient();
            var test = GetElibilityToken(httpClient);

        }

        private void Test2()
        {
            var cancel = new CancellationToken();

            // var clientSecrets = JsonSerializer.Deserialize<ClientSecrets>();

            var clientSecrets = new ClientSecrets
            {
                ClientId = "183231472236-cdugvs0ao2sq708benu167vm8726nbus.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            var scopeList = new List<string>()
            {
                YouTubeService.Scope.Youtube 
            };

            UserCredential credentials = null;

            using (var stream = new FileStream("C:\\users\\h\\downloads\\google-desktop.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "other_token.json";

                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                              GoogleClientSecrets.Load(stream).Secrets, 
                              scopeList, 
                              "h", 
                              cancel,
                              new FileDataStore(credPath, true)).Result;
            }

            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MyAutomations",
            });

            var test = service.Playlists.List;
        }

        private static Token GetElibilityToken(HttpClient client)
    {
        string baseAddress = @"https://accounts.google.com/o/oauth2/auth";

        string grant_type = "client_credentials";
        string client_id = "183231472236-cdugvs0ao2sq708benu167vm8726nbus.apps.googleusercontent.com";
        string client_secret = "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";

        var responseType  = "code";
        var scope = @"https://www.googleapis.com/auth/youtube";
        var redirectUrl = @"http://localhost:8080";

        var form = new Dictionary<string, string>
                {
                    //{"grant_type", grant_type},
                    {"client_id", client_id},
                    //{"client_secret", client_secret},
                    {"response_type", responseType },
                    {"scope", scope},
                    {"acess-type", "offline"},
                    {"redirect_uri", redirectUrl}          
                };

        var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(form)).Result;
        var jsonContent =  tokenResponse.Content.ReadAsStringAsync().Result;
        var tok = JsonConvert.DeserializeObject<Token>(jsonContent);
        return tok;
    }


    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

}
}