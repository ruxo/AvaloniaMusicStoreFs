namespace Avalonia.MusicStore.Views

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

        this.WhenActivated (fun disposable ->
            this.DoShowDialogAsync |> this.ViewModel.ShowDialog.RegisterHandler |> disposeWith disposable
        ) |> ignore

    member this.DoShowDialogAsync (interaction: InteractionContext<MusicStoreViewModel, AlbumViewModel option>) =
        let dialog = MusicStoreWindow(DataContext = interaction.Input)

        task {
            let! result = dialog.ShowDialog<AlbumViewModel option>(this)
            interaction.SetOutput result
        }
