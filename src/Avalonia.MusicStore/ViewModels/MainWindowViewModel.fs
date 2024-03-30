namespace Avalonia.MusicStore.ViewModels

open System.Collections.ObjectModel
open ReactiveUI

type MainWindowViewModel() =
    inherit ViewModelBase()

    let show_dialog = Interaction<MusicStoreViewModel, AlbumViewModel option>()
    let albums = ObservableCollection<AlbumViewModel>()

    let run_dialog () =
            let store = MusicStoreViewModel()
            let result = show_dialog.Handle(store)
            result |> Observable.choose id
                   |> Observable.subscribe albums.Add

    let buy_music_command = ReactiveCommand.Create(run_dialog >> ignore)

    member this.BuyMusicCommand = buy_music_command
    member this.Albums = albums
    member this.ShowDialog = show_dialog
