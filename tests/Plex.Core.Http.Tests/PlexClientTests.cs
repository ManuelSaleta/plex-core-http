using System.Net;
using Plex.Core.Http.Common;
using Plex.Core.Http.Models;
using Xunit;

namespace Plex.Core.Http.Tests;

public class PlexClientTests
{
    private readonly PlexClientOptions _options = new()
    {
        ServerUrl = "http://localhost:32400",
        Token = "secret-token-xyz",
        ClientIdentifier = "test-client-id-123",
        Product = "PlexCoreHttpTests",
        Version = "1.0.0"
    };

    [Fact]
    public async Task GetIdentityAsync_SendsCorrectHeaders_AndDeserializes()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 1,
            "claimed": true,
            "machineIdentifier": "abcdef123456",
            "version": "1.40.2.8395"
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var identity = await client.GetIdentityAsync();

        Assert.NotNull(identity);
        Assert.Equal("abcdef123456", identity.MachineIdentifier);
        Assert.Equal("1.40.2.8395", identity.Version);
        Assert.True(identity.Claimed);

        Assert.NotNull(mock.LastRequest);
        Assert.Equal("/identity", mock.LastRequest.RequestUri?.AbsolutePath);
        Assert.Equal("secret-token-xyz", mock.LastRequest.Headers.GetValues("X-Plex-Token").First());
        Assert.Equal("test-client-id-123", mock.LastRequest.Headers.GetValues("X-Plex-Client-Identifier").First());
        Assert.Equal("PlexCoreHttpTests", mock.LastRequest.Headers.GetValues("X-Plex-Product").First());
    }

    [Fact]
    public async Task GetSectionsAsync_ReturnsParsedSections()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 2,
            "Directory": [
              {
                "key": "1",
                "title": "Movies",
                "type": "movie",
                "agent": "tv.plex.agents.movie"
              },
              {
                "key": "3",
                "title": "Music",
                "type": "artist",
                "agent": "tv.plex.agents.music"
              }
            ]
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var sections = await client.GetSectionsAsync();

        Assert.NotNull(sections);
        Assert.Equal(2, sections.Count);
        Assert.Equal(1, sections[0].Id);
        Assert.Equal("Movies", sections[0].Title);
        Assert.Equal("movie", sections[0].Type);
        Assert.Equal(3, sections[1].Id);
        Assert.Equal("Music", sections[1].Title);
        Assert.Equal("artist", sections[1].Type);
    }

    [Fact]
    public async Task GetAlbumCollectionsAsync_AppendsSubtypeAlbumQuery()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 1,
            "Metadata": [
              {
                "ratingKey": "901",
                "key": "/library/metadata/901/children",
                "type": "collection",
                "subtype": "album",
                "title": "Soundtracks",
                "childCount": 5
              }
            ]
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var collections = await client.GetAlbumCollectionsAsync(3);

        Assert.NotNull(collections);
        Assert.Single(collections);
        Assert.Equal(901, collections[0].Id);
        Assert.Equal("Soundtracks", collections[0].Title);
        Assert.Equal("album", collections[0].Subtype);
        Assert.Equal(5, collections[0].ChildCount);

        Assert.NotNull(mock.LastRequest);
        Assert.Equal("/library/sections/3/collections", mock.LastRequest.RequestUri?.AbsolutePath);
        Assert.Contains("subtype=album", mock.LastRequest.RequestUri?.Query);
    }

    [Fact]
    public async Task GetAlbumsAsync_CallsCorrectEndpoint()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 1,
            "Metadata": [
              {
                "ratingKey": "456",
                "key": "/library/metadata/456/children",
                "type": "album",
                "title": "Abbey Road",
                "parentTitle": "The Beatles",
                "year": 1969
              }
            ]
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var albums = await client.GetAlbumsAsync(3);

        Assert.NotNull(albums);
        Assert.Single(albums);
        Assert.Equal(456, albums[0].Id);
        Assert.Equal("Abbey Road", albums[0].Title);
        Assert.Equal("The Beatles", albums[0].ParentTitle);
        Assert.Equal(1969, albums[0].Year);

        Assert.NotNull(mock.LastRequest);
        Assert.Equal("/library/sections/3/albums", mock.LastRequest.RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task GetCollectionItemsAsync_CallsCorrectPath()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 2,
            "Metadata": [
              {
                "ratingKey": "100",
                "type": "album",
                "title": "Album 1"
              },
              {
                "ratingKey": "101",
                "type": "album",
                "title": "Album 2"
              }
            ]
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var items = await client.GetCollectionItemsAsync(901);

        Assert.NotNull(items);
        Assert.Equal(2, items.Count);
        Assert.Equal("Album 1", items[0].Title);
        Assert.Equal("Album 2", items[1].Title);

        Assert.NotNull(mock.LastRequest);
        Assert.Equal("/library/collections/901/items", mock.LastRequest.RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task CreateCollectionAsync_ConstructsPostQueryAndParsesResult()
    {
        const string json = """
        {
          "MediaContainer": {
            "size": 1,
            "Metadata": [
              {
                "ratingKey": "999",
                "type": "collection",
                "subtype": "album",
                "title": "Rock Classics"
              }
            ]
          }
        }
        """;

        var mock = new MockHttpMessageHandler(json);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var created = await client.CreateCollectionAsync(3, "Rock Classics", PlexMetadataType.Album);

        Assert.NotNull(created);
        Assert.Equal(999, created.Id);
        Assert.Equal("Rock Classics", created.Title);

        Assert.NotNull(mock.LastRequest);
        Assert.Equal(HttpMethod.Post, mock.LastRequest.Method);
        Assert.Equal("/library/collections", mock.LastRequest.RequestUri?.AbsolutePath);
        Assert.Contains("sectionId=3", mock.LastRequest.RequestUri?.Query);
        Assert.Contains("title=Rock%20Classics", mock.LastRequest.RequestUri?.Query);
        Assert.Contains("type=9", mock.LastRequest.RequestUri?.Query);
    }

    [Fact]
    public async Task RefreshSectionAsync_ReturnsTrueOnSuccess()
    {
        var mock = new MockHttpMessageHandler("", HttpStatusCode.OK);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var result = await client.RefreshSectionAsync(1);

        Assert.True(result);
        Assert.NotNull(mock.LastRequest);
        Assert.Equal("/library/sections/1/refresh", mock.LastRequest.RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task ServerError_ThrowsPlexException()
    {
        var mock = new MockHttpMessageHandler("Internal Server Error", HttpStatusCode.InternalServerError);
        using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
        using var client = new PlexClient(httpClient, _options);

        var ex = await Assert.ThrowsAsync<PlexException>(() => client.GetIdentityAsync());
        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.Contains("failed with status 500", ex.Message);
    }
}
