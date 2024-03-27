module ReactiveUI

open System.Reactive.Disposables

let disposeWith (disposable: CompositeDisposable) item =
    ignore <| item.DisposeWith(disposable)
