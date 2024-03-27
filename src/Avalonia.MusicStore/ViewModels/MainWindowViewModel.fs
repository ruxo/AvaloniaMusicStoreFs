namespace Avalonia.MusicStore.ViewModels

open ReactiveUI

type MainWindowViewModel() =
    inherit ViewModelBase()

    let show_dialog = Interaction<MusicStoreViewModel, AlbumViewModel option>()
    let run_dialog () =
            let store = MusicStoreViewModel()
            in show_dialog.Handle(store)

    let buy_music_command = ReactiveCommand.CreateFromObservable<AlbumViewModel option>(run_dialog)

    member this.BuyMusicCommand = buy_music_command
    member this.ShowDialog = show_dialog
