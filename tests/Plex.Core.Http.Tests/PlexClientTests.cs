using System.Net;
using FluentAssertions;
using Plex.Core.Http.Common;


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

    identity.Should().NotBeNull();
    identity!.MachineIdentifier.Should().Be("abcdef123456");
    identity.Version.Should().Be("1.40.2.8395");
    identity.Claimed.Should().BeTrue();

    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.RequestUri?.AbsolutePath.Should().Be("/identity");
    mock.LastRequest.Headers.GetValues("X-Plex-Token").First().Should().Be("secret-token-xyz");
    mock.LastRequest.Headers.GetValues("X-Plex-Client-Identifier").First().Should().Be("test-client-id-123");
    mock.LastRequest.Headers.GetValues("X-Plex-Product").First().Should().Be("PlexCoreHttpTests");
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

    sections.Should().NotBeNull();
    sections.Should().HaveCount(2);
    sections[0].Id.Should().Be(1);
    sections[0].Title.Should().Be("Movies");
    sections[0].Type.Should().Be("movie");
    sections[1].Id.Should().Be(3);
    sections[1].Title.Should().Be("Music");
    sections[1].Type.Should().Be("artist");
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

    collections.Should().NotBeNull();
    collections.Should().ContainSingle();
    collections[0].Id.Should().Be(901);
    collections[0].Title.Should().Be("Soundtracks");
    collections[0].Subtype.Should().Be("album");
    collections[0].ChildCount.Should().Be(5);

    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.RequestUri?.AbsolutePath.Should().Be("/library/sections/3/collections");
    mock.LastRequest.RequestUri?.Query.Should().Contain("subtype=album");
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

    albums.Should().NotBeNull();
    albums.Should().ContainSingle();
    albums[0].Id.Should().Be(456);
    albums[0].Title.Should().Be("Abbey Road");
    albums[0].ParentTitle.Should().Be("The Beatles");
    albums[0].Year.Should().Be(1969);

    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.RequestUri?.AbsolutePath.Should().Be("/library/sections/3/albums");
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

    items.Should().NotBeNull();
    items.Should().HaveCount(2);
    items[0].Title.Should().Be("Album 1");
    items[1].Title.Should().Be("Album 2");

    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.RequestUri?.AbsolutePath.Should().Be("/library/collections/901/items");
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

    created.Should().NotBeNull();
    created!.Id.Should().Be(999);
    created.Title.Should().Be("Rock Classics");

    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.Method.Should().Be(HttpMethod.Post);
    mock.LastRequest.RequestUri?.AbsolutePath.Should().Be("/library/collections");
    mock.LastRequest.RequestUri?.Query.Should().Contain("sectionId=3");
    mock.LastRequest.RequestUri?.Query.Should().Contain("title=Rock%20Classics");
    mock.LastRequest.RequestUri?.Query.Should().Contain("type=9");
  }

  [Fact]
  public async Task RefreshSectionAsync_ReturnsTrueOnSuccess()
  {
    var mock = new MockHttpMessageHandler("", HttpStatusCode.OK);
    using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
    using var client = new PlexClient(httpClient, _options);

    var result = await client.RefreshSectionAsync(1);

    result.Should().BeTrue();
    mock.LastRequest.Should().NotBeNull();
    mock.LastRequest!.RequestUri?.AbsolutePath.Should().Be("/library/sections/1/refresh");
  }

  [Fact]
  public async Task ServerError_ThrowsPlexException()
  {
    var mock = new MockHttpMessageHandler("Internal Server Error", HttpStatusCode.InternalServerError);
    using var httpClient = new HttpClient(mock) { BaseAddress = _options.ServerUri };
    using var client = new PlexClient(httpClient, _options);

    var act = () => client.GetIdentityAsync();
    var ex = await act.Should().ThrowAsync<PlexException>();
    ex.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    ex.Which.Message.Should().Contain("failed with status 500");
  }
}
