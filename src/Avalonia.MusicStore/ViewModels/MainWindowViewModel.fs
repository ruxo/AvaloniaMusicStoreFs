namespace Avalonia.MusicStore.ViewModels

open ReactiveUI

type MainWindowViewModel() =
    inherit ViewModelBase()

    let buy_music_command = ReactiveCommand.Create(fun () -> printfn "Buy music!")

    member this.BuyMusicCommand = buy_music_command
