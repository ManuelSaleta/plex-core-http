using System.Text.Json;
using Plex.Core.Http.Models;
using Xunit;

namespace Plex.Core.Http.Tests;

public class PlexSerializationTests
{
    [Fact]
    public void Deserializes_ComplexAlbumMetadata()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 1,
            "librarySectionID": 3,
            "librarySectionTitle": "Music",
            "Metadata": [
              {
                "ratingKey": "265",
                "key": "/library/metadata/265/children",
                "parentRatingKey": "251",
                "guid": "plex://album/5d07c894403c640290c0e196",
                "studio": "RCA",
                "type": "album",
                "title": "Mandatory Fun",
                "parentTitle": "\"Weird Al\" Yankovic",
                "index": 1,
                "rating": 8,
                "year": 2014,
                "leafCount": 12,
                "Genre": [
                  {
                    "tag": "Comedy/Spoken"
                  }
                ],
                "Media": [
                  {
                    "id": 827,
                    "duration": 5129000,
                    "bitrate": 320,
                    "audioCodec": "mp3",
                    "Part": [
                      {
                        "id": 827,
                        "key": "/library/parts/827/file.mp3",
                        "duration": 5129000,
                        "file": "/music/Weird Al/Mandatory Fun/01 Handy.mp3",
                        "size": 8420192
                      }
                    ]
                  }
                ]
              }
            ]
          }
        }
        """;

        var result = JsonSerializer.Deserialize<PlexResponse<PlexMediaContainer<PlexMetadataItem>>>(
            json,
            PlexClient.DefaultJsonOptions);

        Assert.NotNull(result?.MediaContainer);
        Assert.Equal(3, result.MediaContainer.LibrarySectionID);
        Assert.Equal("Music", result.MediaContainer.LibrarySectionTitle);

        var item = Assert.Single(result.MediaContainer.Metadata!);
        Assert.Equal(265, item.Id);
        Assert.Equal("Mandatory Fun", item.Title);
        Assert.Equal("\"Weird Al\" Yankovic", item.ParentTitle);
        Assert.Equal(2014, item.Year);
        Assert.Equal(12, item.LeafCount);

        var genre = Assert.Single(item.Genre!);
        Assert.Equal("Comedy/Spoken", genre.Tag);

        var media = Assert.Single(item.Media!);
        Assert.Equal("mp3", media.AudioCodec);

        var part = Assert.Single(media.Part!);
        Assert.Equal("/music/Weird Al/Mandatory Fun/01 Handy.mp3", part.File);
    }
}
