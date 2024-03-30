namespace Avalonia.MusicStore.ViewModels

open global.ReactiveUI
open Avalonia.Media.Imaging
open Avalonia.MusicStore.Models

type ViewModelBase() =
    inherit ReactiveObject()

type AlbumViewModel(album: Album) =
    inherit ViewModelBase()

    let mutable nullable_cover: Bitmap = null

    member this.Artist = album.Artist
    member this.Title = album.Title
    member this.Cover with get() = nullable_cover and set v = this.RaiseAndSetIfChanged(&nullable_cover, v) |> ignore

    member this.LoadCover() =
        async {
            let! stream = album |> Album.loadCoverBitmap
            this.Cover <- Bitmap.DecodeToWidth(stream, 400)
            do! stream.DisposeAsync()
        }

    member this.SaveToDiskAsync() =
        async {
            do! album |> Album.saveAsync

            if not (nullable_cover |> isNull) then
                use fs = album |> Album.saveCoverBitmapStream
                nullable_cover.Save(fs)
        }