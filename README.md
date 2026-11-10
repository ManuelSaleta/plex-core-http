# Plex.Core.Http

[![NuGet Version](https://img.shields.io/nuget/v/Plex.Core.Http.svg?style=flat-square)](https://www.nuget.org/packages/Plex.Core.Http/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![.NET Compatibility](https://img.shields.io/badge/.NET-%3E%3D%2010.0-512BD4.svg?style=flat-square)](https://dotnet.microsoft.com/)

A modern, minimal, lightweight, and high-performance .NET `HttpClient` library for communicating with [Plex Media Server](https://www.plex.tv/), based on the official [Plex OpenAPI Specification v1.23.3](https://developer.plex.tv/pms/).

Built with modern C# and `System.Text.Json`, `Plex.Core.Http` provides strongly-typed models, idiomatic `IHttpClientFactory` / Dependency Injection support, and automatic Plex header management.

---

## Features

- **Modern & Lightweight:** Built specifically for modern .NET (`>= 10.0`), minimal transitive dependencies, non-blocking asynchronous APIs with `CancellationToken` support throughout.
- **First-Class DI Integration:** Registers cleanly with `IServiceCollection.AddPlexClient()` and `IHttpClientFactory`.
- **Plex Header Management:** Transparently handles required Plex headers (`X-Plex-Token`, `X-Plex-Client-Identifier`, `X-Plex-Product`, `X-Plex-Version`, etc.).
- **Rich Library & Music Support:** Full support for Library Sections, Media metadata, Scanning/Refresh, Playlists, Sessions, and **Album Collections** (`subtype=album`).
- **Robust Serialization:** Custom-tailored `JsonSerializerOptions` that handle Plex's dynamic string/integer IDs and payload structures.

---

## Installation

Install the package via NuGet CLI:

```bash
dotnet add package Plex.Core.Http
```

Or via the Visual Studio Package Manager:

```powershell
Install-Package Plex.Core.Http
```

---

## Quickstart

### 1. Dependency Injection (`Microsoft.Extensions.DependencyInjection`)

In your `Program.cs` or service configuration:

```csharp
using Plex.Core.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register PlexClient with IHttpClientFactory
builder.Services.AddPlexClient(options =>
{
    options.ServerUrl = builder.Configuration["Plex:ServerUrl"] ?? "http://192.168.1.100:32400";
    options.Token = builder.Configuration["Plex:Token"] ?? "YOUR_PLEX_TOKEN";
    options.ClientIdentifier = "my-homelab-app";
    options.Product = "MyHomelabApp";
    options.Version = "1.0.0";
});

var app = builder.Build();
```

Inject `IPlexClient` into your services or controllers:

```csharp
public class MediaSyncService(IPlexClient plexClient)
{
    public async Task SyncMusicAsync(CancellationToken ct)
    {
        var identity = await plexClient.GetIdentityAsync(ct);
        Console.WriteLine($"Connected to PMS: {identity?.MachineIdentifier} (v{identity?.Version})");
    }
}
```

### 2. Standalone Usage (Without DI)

```csharp
using Plex.Core.Http;

var options = new PlexClientOptions
{
    ServerUrl = "http://192.168.1.100:32400",
    Token = "YOUR_PLEX_TOKEN",
    ClientIdentifier = "standalone-console-app",
    Product = "PlexConsoleApp",
    Version = "1.0.0"
};

using var client = new PlexClient(options);
var sections = await client.GetSectionsAsync();

foreach (var section in sections)
{
    Console.WriteLine($"Section [{section.Id}]: {section.Title} ({section.Type})");
}
```

---

## Usage Examples

### Listing Library Sections

```csharp
var sections = await plexClient.GetSectionsAsync();

foreach (var section in sections)
{
    Console.WriteLine($"[{section.Id}] {section.Title} - Type: {section.Type}");
}
```

### Querying Album Collections

In Plex Media Server, collections in a Music library have `type: "collection"` and `subtype: "album"`. `Plex.Core.Http` provides dedicated methods to work with them:

```csharp
int musicSectionId = 3;

// 1. Get all Album Collections in the music section
var albumCollections = await plexClient.GetAlbumCollectionsAsync(musicSectionId);

foreach (var collection in albumCollections)
{
    Console.WriteLine($"Collection: {collection.Title} (ID: {collection.Id}, Items: {collection.ChildCount})");

    // 2. Fetch the albums inside this collection
    var albums = await plexClient.GetCollectionItemsAsync(collection.Id);
    foreach (var album in albums)
    {
        Console.WriteLine($"  - {album.Title} by {album.ParentTitle} ({album.Year})");
    }
}
```

### Creating an Album Collection

```csharp
using Plex.Core.Http.Common;

// Create a new Album collection (type = 9) in section 3
var newCollection = await plexClient.CreateCollectionAsync(
    sectionId: 3,
    title: "90s Alternative Gems",
    type: PlexMetadataType.Album
);

Console.WriteLine($"Created collection: {newCollection?.Title} (ID: {newCollection?.Id})");
```

### Triggering a Library Refresh / Scan

```csharp
// Refresh a library section after new files are transferred
bool initiated = await plexClient.RefreshSectionAsync(sectionId: 3);
if (initiated)
{
    Console.WriteLine("Plex scan initiated successfully.");
}
```

### Active Sessions Monitoring

```csharp
var activeSessions = await plexClient.GetActiveSessionsAsync();
Console.WriteLine($"Currently active streams: {activeSessions.Count}");
```

---

## API Overview

| Method                                  | Description                                                             |
| :-------------------------------------- | :---------------------------------------------------------------------- |
| `GetIdentityAsync()`                    | Retrieves server machine identifier, version, and claimed state.        |
| `GetServerCapabilitiesAsync()`          | Retrieves root `/` server details, friendly name, and features.         |
| `GetSectionsAsync()`                    | Lists all library sections.                                             |
| `GetSectionAsync(id)`                   | Retrieves details for a specific library section.                       |
| `RefreshSectionAsync(id)`               | Triggers a background scan/refresh of the specified section.            |
| `EmptySectionTrashAsync(id)`            | Empties trash items from the library section.                           |
| `GetSectionItemsAsync(id, type?)`       | Gets all items in a section, optionally filtered by `PlexMetadataType`. |
| `GetAlbumsAsync(sectionId)`             | Gets all albums in a music library section.                             |
| `GetCollectionsAsync(id, subtype?)`     | Gets collections in a section, optionally filtered by subtype.          |
| `GetAlbumCollectionsAsync(sectionId)`   | Dedicated helper to get album collections (`subtype=album`).            |
| `GetCollectionItemsAsync(collectionId)` | Retrieves items contained in a collection.                              |
| `CreateCollectionAsync(...)`            | Creates a new standard or smart collection.                             |
| `DeleteCollectionAsync(...)`            | Deletes a collection from a section.                                    |
| `AddItemToCollectionAsync(...)`         | Adds a media item URI to a collection.                                  |
| `RemoveItemFromCollectionAsync(...)`    | Removes a media item from a collection.                                 |
| `GetMetadataAsync(ratingKey)`           | Retrieves full metadata for an item.                                    |
| `GetMetadataChildrenAsync(ratingKey)`   | Gets child items (e.g. tracks of an album, episodes of a season).       |
| `SearchSectionAsync(id, query)`         | Searches within a library section.                                      |
| `GetActiveSessionsAsync()`              | Lists currently active playback streams.                                |
| `GetAsync<T>(relativeUri)`              | Low-level generic GET helper for arbitrary endpoints.                   |

---

## Publishing to NuGet

### 1. Pack with .NET CLI

```bash
dotnet pack -c Release -o ./nupkg
```

### 2. Publish Manually

```bash
dotnet nuget push ./nupkg/Plex.Core.Http.*.nupkg \
  --api-key <YOUR_NUGET_API_KEY> \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

### 3. Automated via GitHub Actions

This repository includes automated GitHub Actions workflows:

- **`ci.yml`**: Automatically builds and tests pull requests and pushes to `main`.
- **`publish.yml`**: On creating a GitHub Release (or running manually via `workflow_dispatch`), packages the project and pushes to NuGet using the `NUGET_API_KEY` repository secret.

---

## License

This project is licensed under the [MIT License](LICENSE).
