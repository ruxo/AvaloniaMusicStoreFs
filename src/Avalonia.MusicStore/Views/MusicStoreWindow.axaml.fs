namespace Avalonia.MusicStore

open Avalonia.Controls
open global.ReactiveUI
open Avalonia
open Avalonia.Markup.Xaml
open Avalonia.MusicStore.ViewModels
open Avalonia.ReactiveUI

type MusicStoreWindow () as this =
    inherit ReactiveWindow<MusicStoreViewModel>()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)

        if not Design.IsDesignMode then
            this.WhenActivated(fun disposables ->
                this.ViewModel.BuyMusicCommand.Subscribe(this.Close) |> disposeWith disposables
            ) |> ignore
