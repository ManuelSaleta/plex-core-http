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

        result?.MediaContainer.Should().NotBeNull();
        result!.MediaContainer!.LibrarySectionID.Should().Be(3);
        result.MediaContainer.LibrarySectionTitle.Should().Be("Music");

        result.MediaContainer.Metadata.Should().NotBeNull();
        result.MediaContainer.Metadata.Should().ContainSingle();
        var item = result.MediaContainer.Metadata![0];
        item.Id.Should().Be(265);
        item.Title.Should().Be("Mandatory Fun");
        item.ParentTitle.Should().Be("\"Weird Al\" Yankovic");
        item.Year.Should().Be(2014);
        item.LeafCount.Should().Be(12);

        item.Genre.Should().NotBeNull();
        item.Genre.Should().ContainSingle();
        item.Genre![0].Tag.Should().Be("Comedy/Spoken");

        item.Media.Should().NotBeNull();
        item.Media.Should().ContainSingle();
        item.Media![0].AudioCodec.Should().Be("mp3");

        item.Media[0].Part.Should().NotBeNull();
        item.Media[0].Part.Should().ContainSingle();
        item.Media[0].Part![0].File.Should().Be("/music/Weird Al/Mandatory Fun/01 Handy.mp3");
    }

    [Fact]
    public void PlexJsonSerializer_RoundTrip()
    {
        var item = new PlexMetadataItem
        {
            RatingKey = "123",
            Title = "Test Album",
            Type = "album",
            Subtype = Common.PlexSubtype.Album,
            Year = 2026
        };

        var json = Serialization.PlexJsonSerializer.Serialize(item);
        json.Should().Contain("\"ratingKey\":\"123\"");
        json.Should().Contain("\"title\":\"Test Album\"");

        var deserialized = Serialization.PlexJsonSerializer.Deserialize<PlexMetadataItem>(json);
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(123);
        deserialized.Title.Should().Be("Test Album");
        deserialized.Subtype.Should().Be(Common.PlexSubtype.Album);
    }
}
