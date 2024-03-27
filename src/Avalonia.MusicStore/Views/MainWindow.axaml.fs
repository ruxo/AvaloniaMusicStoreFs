namespace Avalonia.MusicStore.Views

open System
open Avalonia
open Avalonia.Markup.Xaml
open Avalonia.MusicStore
open Avalonia.MusicStore.ViewModels
open Avalonia.ReactiveUI
open global.ReactiveUI

type MainWindow () as this =
    inherit ReactiveWindow<MainWindowViewModel>()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)

        let registerDialog (action: Action<IDisposable>) =
            action.Invoke <| this.ViewModel.ShowDialog.RegisterHandler this.DoShowDialogAsync

        this.WhenActivated registerDialog |> ignore

    member this.DoShowDialogAsync (interaction: InteractionContext<MusicStoreViewModel, AlbumViewModel option>) =
        let dialog = MusicStoreWindow()
        dialog.DataContext <- interaction.Input

        task {
            let! result = dialog.ShowDialog<AlbumViewModel option>(this)
            interaction.SetOutput result
        }
