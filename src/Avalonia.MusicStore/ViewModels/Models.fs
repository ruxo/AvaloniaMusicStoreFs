namespace Avalonia.MusicStore.ViewModels

open System.Collections.ObjectModel
open ReactiveUI

type ViewModelBase() =
    inherit ReactiveObject()

type AlbumViewModel() =
    inherit ViewModelBase()

type MusicStoreViewModel() =
    inherit ViewModelBase()

    let search_results = ObservableCollection<AlbumViewModel>()
    do search_results.Add <| AlbumViewModel()
    do search_results.Add <| AlbumViewModel()
    do search_results.Add <| AlbumViewModel()

    let mutable search_text :string option = None
    let mutable is_busy = false
    let mutable selected_album :AlbumViewModel option = None

    member this.SearchText with get() = search_text and set value = this.RaiseAndSetIfChanged(&search_text, value) |> ignore
    member this.IsBusy with get() = is_busy and set value = this.RaiseAndSetIfChanged(&is_busy, value) |> ignore

    member this.SelectedAlbum with get() = selected_album and set value = this.RaiseAndSetIfChanged(&selected_album, value) |> ignore
    member this.SearchResults with get() = search_results