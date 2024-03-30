namespace Avalonia.MusicStore.ViewModels

open System
open System.Collections.ObjectModel
open FSharp.Control.Reactive
open ReactiveUI
open Avalonia.MusicStore.Models

type ViewModelBase() =
    inherit ReactiveObject()

type AlbumViewModel(album: Album) =
    inherit ViewModelBase()

    member this.Artist = album.Artist
    member this.Title = album.Title

type MusicStoreViewModel() as this =
    inherit ViewModelBase()

    let search_results = ObservableCollection<AlbumViewModel>()

    let mutable search_text = String.Empty
    let mutable is_busy = false
    let mutable selected_album :AlbumViewModel option = None

    let notNull = Option.ofObj >> Option.filter (not << String.IsNullOrWhiteSpace)

    let doSearch s = async {
        is_busy <- true
        search_results.Clear()

        match notNull s with
        | Some v ->
            let! albums = v |> Album.searchAsync
            albums |> Seq.iter (AlbumViewModel >> search_results.Add)
        | None -> ()

        is_busy <- false
    }

    do this.Init()

    member this.SearchText with get() = search_text and set value = this.RaiseAndSetIfChanged(&search_text, value) |> ignore
    member this.IsBusy with get() = is_busy and set value = this.RaiseAndSetIfChanged(&is_busy, value) |> ignore

    member this.SelectedAlbum with get() = selected_album and set value = this.RaiseAndSetIfChanged(&selected_album, value) |> ignore
    member this.SearchResults with get() = search_results

    member private this.Init() =
        this.WhenAnyValue(fun x -> x.SearchText)
            |> Observable.throttle (400 |> TimeSpan.FromMilliseconds)
            |> Observable.subscribe (doSearch >> Async.Start)
            |> ignore