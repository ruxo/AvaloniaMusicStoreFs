namespace Avalonia.MusicStore.ViewModels

open FSharp.Control.Reactive
open System
open System.Collections.ObjectModel
open System.Threading
open global.ReactiveUI
open Avalonia.MusicStore.Models

type MusicStoreViewModel() as this =
    inherit ViewModelBase()

    let search_results = ObservableCollection<AlbumViewModel>()

    let mutable search_text = String.Empty
    let mutable is_busy = false
    let mutable selected_album :obj = null

    let mutable cancellation_source = new CancellationTokenSource()
    let mutable buy_music_command: ReactiveCommand<Unit, AlbumViewModel option> = null

    let notNull = Option.ofObj >> Option.filter (not << String.IsNullOrWhiteSpace)

    let loadCovers () =
        search_results |> Seq.map _.LoadCover() |> Seq.toArray |> Async.Parallel |> Async.Ignore

    let doSearch s = async {
        is_busy <- true
        search_results.Clear()

        match notNull s with
        | Some v ->
            let! albums = v |> Album.searchAsync
            albums |> Seq.iter (AlbumViewModel >> search_results.Add)
            do! loadCovers()
        | None -> ()

        is_busy <- false
    }

    do this.Init()

    member this.SearchText with get() = search_text and set value = this.RaiseAndSetIfChanged(&search_text, value) |> ignore
    member this.IsBusy with get() = is_busy and set value = this.RaiseAndSetIfChanged(&is_busy, value) |> ignore

    member this.SelectedAlbum with get() = selected_album and set value = this.RaiseAndSetIfChanged(&selected_album, value) |> ignore
    member this.SearchResults = search_results

    member this.BuyMusicCommand = buy_music_command

    member private this.Init() =
        buy_music_command <- ReactiveCommand.Create<Unit, AlbumViewModel option>(fun _ ->
                               if isNull selected_album
                               then None
                               else selected_album :?> AlbumViewModel |> Some)
        this.WhenAnyValue(fun x -> x.SearchText)
            |> Observable.throttle (400 |> TimeSpan.FromMilliseconds)
            |> Observable.subscribe (fun s -> cancellation_source.Cancel()
                                              cancellation_source.Dispose()
                                              cancellation_source <- new CancellationTokenSource()
                                              Async.Start(doSearch(s), cancellation_source.Token))
            |> ignore
