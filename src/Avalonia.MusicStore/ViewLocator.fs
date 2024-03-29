namespace Avalonia.MusicStore

open System
open System.Collections.Generic
open Avalonia.Controls
open Avalonia.Controls.Templates
open Avalonia.MusicStore.ViewModels

module private ViewLocator =
    let private located_types = Dictionary<Type, Type>()

    let getViewType data_type =
        let found, t = located_types.TryGetValue data_type
        in if found then ValueSome t else ValueNone

    let inline findType name =
        name |> Type.GetType |> ValueOption.ofObj

    let getViewTypeName (vm_type: Type) =
        vm_type.FullName.Replace("ViewModel", "View", StringComparison.Ordinal)

    let findAndCacheType vm_type =
        vm_type |> getViewTypeName
                |> findType
                |> ValueOption.map (fun t -> located_types.Add(vm_type, t); t)

    let getAndCacheType vm_type =
        vm_type |> getViewType |> ValueOption.orElseWith (fun _ -> vm_type |> findAndCacheType)

type ViewLocator() =

    interface IDataTemplate with
        member this.Build(data) =
            if isNull data then
                null
            else
                let data_type = data.GetType()

                match data_type |> ViewLocator.getAndCacheType with
                | ValueSome typ ->
                    let view = Activator.CreateInstance(typ) :?> Control
                    view.DataContext <- data
                    view
                | ValueNone -> upcast TextBlock(Text = $"Not Found: %s{data_type |> ViewLocator.getViewTypeName}")

        member this.Match(data) = data :? ViewModelBase
