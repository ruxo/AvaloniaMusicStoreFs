namespace Avalonia.MusicStore.ViewModels

open ReactiveUI

type ViewModelBase() =
    inherit ReactiveObject()

type AlbumViewModel() =
    inherit ViewModelBase()

type MusicStoreViewModel() =
    inherit ViewModelBase()