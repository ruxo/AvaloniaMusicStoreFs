namespace Avalonia.MusicStore.ViewModels

open System.Collections.ObjectModel
open Avalonia.Controls
open Avalonia.MusicStore.Models
open ReactiveUI

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let show_dialog = Interaction<MusicStoreViewModel, AlbumViewModel option>()
    let albums = ObservableCollection<AlbumViewModel>()

    let runDialog () =
        let store = MusicStoreViewModel()
        let result = show_dialog.Handle(store)
        result |> Observable.choose id
               |> Observable.subscribe (fun a -> albums.Add(a); a.SaveToDiskAsync() |> Async.Start)

    let buy_music_command = ReactiveCommand.Create(runDialog >> ignore)

    let loadAlbums() =
        async {
            let! loaded = Album.loadCachedAsync()
            let loaded_albums = loaded |> Array.map AlbumViewModel
            loaded_albums |> Array.iter albums.Add
            do! albums |> Seq.map _.LoadCover() |> Async.Parallel |> Async.Ignore
        }

    do this.Init()

    member this.BuyMusicCommand = buy_music_command
    member this.Albums = albums
    member this.ShowDialog = show_dialog

    member private this.Init() =
        if not Design.IsDesignMode then
            // If using Async.Start, the items shown will be duplicated! No idea why running on the thread pool would cause this.
            loadAlbums() |> Async.StartImmediate