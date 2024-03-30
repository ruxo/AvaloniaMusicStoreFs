namespace Avalonia.MusicStore.Models

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
