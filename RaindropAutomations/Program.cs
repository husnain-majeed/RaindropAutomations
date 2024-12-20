﻿using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Options;
using RaindropAutomations.Tools;
using RainDropAutomations.Youtube.Models;
using YoutubeAutomation;

namespace RaindropAutomations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                         //.AddJsonFile("appsettings.json")
                        .AddUserSecrets<Program>()
                        .Build();

            GlobalConfig.Config = config;

            var googleApiConfig = config.GetSection("GoogleApi");

            var applicationName = googleApiConfig.GetSection("ApplicationName").Value;
            var credentialsPath = googleApiConfig.GetSection("CredentialsPath").Value;
            var tokenPath = googleApiConfig.GetSection("TokenFilePath").Value;


            if (true)
            {
                var saveCollectionId = int.Parse(config.GetFromRaindropConfig("VideosSaveInboxId").Value);
                var checkParentCollectionId = int.Parse(config.GetFromRaindropConfig("VidoesCheckRootId").Value);

                var youtubeManager = new YoutubeManager(config, applicationName, credentialsPath, tokenPath);
                var raindropManager = new RaindropManager(config);

                var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

                YoutubePlaylistToRaindrop(youtubeManager, raindropManager, playlistUrl, saveCollectionId, checkParentCollectionId);
            }
        }

        private static void YoutubePlaylistToRaindrop(YoutubeManager youtubeManager, RaindropManager raindropManager, string playlistUrl, int saveCollectionId, int checkParentCollectionId)
        {
            // for ViaApi version of GetVideos method
            // var youtubePlaylistName = "dump-wl";

            var videoUrls = youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl).Select(x => x.UrlAndFirstPram);
            
            var targetCollection = new CollectionIdSaveModel { Id = saveCollectionId };

            var videosAsBookmarks = videoUrls.Select
                (
                   x => new BookmarkSaveModel { Link = x, Collection = targetCollection, PleaseParse = new() }
                ).ToList();

            videosAsBookmarks.RemoveMatchesFromDescendants(checkParentCollectionId, SelfInclusionOptions.IncludeSelf, UrlOptions.UrlAndFirstParamOnly);

            raindropManager.CreateMultipleBookmarks(videosAsBookmarks);
        }

        private static void SaveYoutubeVideosInRaindropCollection()
        {

        }
    }
}

//var bookmarks = new List<Bookmark>();

//var bookmark1 = new Bookmark()
//{
//    Link = @"https://www.youtube.com/watch?v=j5q9t4hXZz4",
//    Collection = new Collection {Id = 43166517},
//    PleaseParse = new(),
//};

//var bookmark2 = new Bookmark()
//{
//    Link = @"https://www.youtube.com/watch?v=5rSU21PXTGE",
//    Collection = new Collection {Id = 43166517},
//    PleaseParse = new(),
//};

//bookmarks.Add(bookmark1);
//bookmarks.Add(bookmark2);