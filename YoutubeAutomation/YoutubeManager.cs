﻿namespace YoutubeAutomation
{
    using Newtonsoft.Json;
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Util.Store;
    using Google.Apis.YouTube.v3;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3.Data;
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

            //var httpClient = new HttpClient();
            //var test = GetElibilityToken(httpClient);

        }


        private void Test2()
        {
            var scopeList = new List<string>()
            {
                YouTubeService.Scope.Youtube
            };

            var credentials = GetMyCredentials(scopeList);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MyAutomations",
            });

            var selectedPlaylist = GetMyPlaylists(youtubeService).First(y => y.Snippet.Title == "dump-wl");  //.Select(x => x.Snippet)

            var playlistVideos = GetVideosFromPlaylist(youtubeService, selectedPlaylist.Id);
        }


        private static List<Playlist> GetMyPlaylists(YouTubeService service)
        {
            var getPlaylistsRequest = service.Playlists.List("snippet");

            getPlaylistsRequest.Mine = true;
            getPlaylistsRequest.MaxResults = 50;

            var playlistsFirstPage = getPlaylistsRequest.Execute();
            var allPlaylists = playlistsFirstPage.Items.ToList();


            var currentPlaylistPage = playlistsFirstPage;

            while (currentPlaylistPage.NextPageToken != null)
            {
                getPlaylistsRequest.PageToken = currentPlaylistPage.NextPageToken;
                var nextPlaylistPage = getPlaylistsRequest.Execute();

                allPlaylists.AddRange(nextPlaylistPage.Items);
                currentPlaylistPage = nextPlaylistPage;
            }

            return allPlaylists;
        }


        private static List<PlaylistItem> GetVideosFromPlaylist(YouTubeService service, string playlistId)
        {
            var getVideosRequest = service.PlaylistItems.List("snippet");
            getVideosRequest.PlaylistId = playlistId;
            getVideosRequest.MaxResults = 50;

            var videosFirstPage = getVideosRequest.Execute();

            var allVideos = videosFirstPage.Items.ToList();

            var currentVideoPage = videosFirstPage;

            while (currentVideoPage.NextPageToken != null)
            {
                getVideosRequest.PageToken = currentVideoPage.NextPageToken;
                var nextVideoPage = getVideosRequest.Execute();

                allVideos.AddRange(nextVideoPage.Items);
                currentVideoPage = nextVideoPage;
            }

            return allVideos;
        }


        private static UserCredential GetMyCredentials(List<string> scopeList)
        {
            var cancel = new CancellationToken();
        
            using var stream = new FileStream("C:\\users\\h\\downloads\\google-desktop.json", FileMode.Open, FileAccess.Read);
            var credPath = "other_token.json";

            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          GoogleClientSecrets.Load(stream).Secrets,
                          scopeList,
                          "h",
                          cancel,
                          new FileDataStore(credPath, true)).Result;

            return credentials;
        }


    //    private static Token GetElibilityToken(HttpClient client)
    //{
    //    string baseAddress = @"https://accounts.google.com/o/oauth2/auth";

    //    string grant_type = "client_credentials";
    //    string client_id = "REMOVED_CREDS";
    //    string client_secret = "REMOVED_CREDS";

    //    var responseType  = "code";
    //    var scope = @"https://www.googleapis.com/auth/youtube";
    //    var redirectUrl = @"http://localhost:8080";

    //    var form = new Dictionary<string, string>
    //            {
    //                //{"grant_type", grant_type},
    //                {"client_id", client_id},
    //                //{"client_secret", client_secret},
    //                {"response_type", responseType },
    //                {"scope", scope},
    //                {"acess-type", "offline"},
    //                {"redirect_uri", redirectUrl}          
    //            };

    //    var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(form)).Result;
    //    var jsonContent =  tokenResponse.Content.ReadAsStringAsync().Result;
    //    var tok = JsonConvert.DeserializeObject<Token>(jsonContent);
    //    return tok;
    //}


    //internal class Token
    //{
    //    [JsonProperty("access_token")]
    //    public string AccessToken { get; set; }

    //    [JsonProperty("token_type")]
    //    public string TokenType { get; set; }

    //    [JsonProperty("expires_in")]
    //    public int ExpiresIn { get; set; }

    //    [JsonProperty("refresh_token")]
    //    public string RefreshToken { get; set; }
    //}

}
}