namespace Avalonia.MusicStore.Models

open System
open System.Text.Json
open FSharp.Control
open System.IO
open FsHttp
open iTunesSearch.Library

type Album = {
    Artist: string
    Title: string
    CoverUrl: string
}

module Album =
    let private SearchManager = iTunesSearchManager()

    let searchAsync search_term =
        async {
            let! query = search_term |> SearchManager.GetAlbumsAsync |> Async.AwaitTask
            return seq {
                for x in query.Albums do
                    { Artist = x.ArtistName
                      Title = x.CollectionName
                      CoverUrl = x.ArtworkUrl100 }
            }
        }

    [<Literal>]
    let private CachePath = "/temp/cache/"

    let getFileName album =
        $"{album.Artist} - {album.Title}"

    let getCachePath ext album =
        Path.Combine(CachePath, $"{album |> getFileName}{ext}")

    let loadCoverBitmap album :Async<Stream> =
        let path = album |> getCachePath ".bmp"
        if path |> File.Exists
        then async { return File.OpenRead(path) }
        else async {
            let! res =  http { GET album.CoverUrl } |> Request.sendAsync
            let! content = res.ToBytesAsync()
            return new MemoryStream(content)
        }

    let saveCoverBitmapStream = getCachePath ".bmp" >> File.OpenWrite

    let saveToStreamAsync stream (album: Album) =
        async { return! JsonSerializer.SerializeAsync(stream, album) }

    let saveAsync album =
        async {
            if not (Directory.Exists CachePath) then Directory.CreateDirectory CachePath |> ignore
            use fs = album |> getCachePath String.Empty |> File.OpenWrite
            do! album |> saveToStreamAsync fs
        }

    let loadFromStream stream =
        async { return! JsonSerializer.DeserializeAsync<Album>(stream) }

    let loadAlbum file =
        async {
            use fs = file |> File.OpenRead
            return! fs |> loadFromStream
        }

    let loadCachedAsync () =
        async {
            if not (Directory.Exists CachePath) then Directory.CreateDirectory CachePath |> ignore

            let cached_albums = Directory.EnumerateFiles(CachePath, "*.", SearchOption.TopDirectoryOnly)
            let all_albums = cached_albums |> Seq.map loadAlbum
            return! all_albums |> Async.Parallel
        }