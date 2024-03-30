namespace Avalonia.MusicStore.Models

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

    let loadCoverBitmap album :Async<Stream> =
        let path = Path.Combine(CachePath, $"{album.Artist} - {album.Title}.bmp")
        if path |> File.Exists
        then async { return File.OpenRead(path) }
        else async {
            let! res =  http { GET album.CoverUrl } |> Request.sendAsync
            let! content = res.ToBytesAsync()
            return new MemoryStream(content)
        }