﻿namespace ClassBuilder.Classes

open Autofac
open Caelan.Frameworks.Common.Helpers
open ClassBuilder
open ClassBuilder.Interfaces
open System
open System.Collections.ObjectModel
open System.Linq
open System.Reflection

module Builder = 
    let private assemblies = ObservableCollection<Assembly>()
    let private isMapper (t : Type) = typeof<IMapper>.IsAssignableFrom(t) && not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition
    
    let private containsMapper (t : Assembly) = 
        try 
            t.GetTypes() |> Array.exists isMapper
        with _ -> false
    
    let mutable private container = 
        let cb = ContainerBuilder()
        cb.RegisterGeneric(typedefof<DefaultMapper<_, _>>).As(typedefof<IMapper<_, _>>) |> ignore
        cb.RegisterAssemblyTypes(assemblies.AsEnumerable() |> Array.ofSeq).Where(fun t -> t |> isMapper).AsImplementedInterfaces() |> ignore
        cb.Build()
    
    let RegisterMapper<'TMapper when 'TMapper :> IMapper>() = 
        let cb = ContainerBuilder()
        cb.RegisterType<'TMapper>().AsImplementedInterfaces() |> ignore
        cb.Update(container) |> ignore
    
    let internal registerAssemblies (allAssemblies : Assembly []) = 
        let cb = ContainerBuilder()
        cb.RegisterAssemblyTypes(allAssemblies).Where(fun t -> t |> isMapper).AsImplementedInterfaces() |> ignore
        cb.Update(container) |> ignore
        allAssemblies
        |> Array.collect (fun t -> t.GetReferencedAssemblies())
        |> Array.map (fun t -> try
                                    t |> Assembly.Load
                                with _ -> null)
        |> Array.filter (isNull >> not)
        |> Array.filter (assemblies.Contains >> not)
        |> Array.filter containsMapper
        |> Array.iter assemblies.Add
    
    assemblies.CollectionChanged.Add(fun t -> 
        registerAssemblies (t.NewItems.Cast<Assembly>()
                            |> Seq.filter (isNull >> not)
                            |> Array.ofSeq))
    AppDomain.CurrentDomain.GetAssemblies() |> registerAssemblies
    
    let internal getMapper<'TSource, 'TDestination>() = 
        let mutable mapper = Unchecked.defaultof<IMapper<'TSource, 'TDestination>>
        use scope = container.BeginLifetimeScope()
        container.TryResolve<IMapper<'TSource, 'TDestination>>(&mapper) |> ignore
        mapper
    
    [<Sealed>]
    type Builder<'T> internal (source) = 
        
        /// <summary>
        /// 
        /// </summary>
        member this.To<'TDestination>() = 
            let destination = Activator.CreateInstance<'TDestination>()
            
            let mapper = 
                if typeof<'TDestination>.Assembly
                   |> (isNull >> not)
                   && typeof<'TDestination>.Assembly |> (assemblies.Contains >> not)
                   && typeof<'TDestination>.Assembly |> containsMapper then assemblies.Add(typeof<'TDestination>.Assembly)
                getMapper<'T, 'TDestination>()
            (destination, mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        member this.To<'TDestination>(mapper : IMapper<'T, 'TDestination>) = (Activator.CreateInstance<'TDestination>(), mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        member this.To<'TDestination>(destination : 'TDestination) = 
            let mapper = 
                if typeof<'TDestination>.Assembly
                   |> (isNull >> not)
                   && typeof<'TDestination>.Assembly |> (assemblies.Contains >> not)
                   && typeof<'TDestination>.Assembly |> containsMapper then assemblies.Add(typeof<'TDestination>.Assembly)
                getMapper<'T, 'TDestination>()
            (destination, mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="mapper"></param>
        member __.To<'TDestination>(destination, mapper : IMapper<'T, 'TDestination>) = 
            match source :> obj |> Option.ofObj with
            | None -> Unchecked.defaultof<'TDestination>
            | Some(s) -> (source, destination) |> mapper.Map
    
    [<Sealed>]
    type ListBuilder<'T> internal (sourceList) = 
        
        /// <summary>
        /// 
        /// </summary>
        member this.ToList<'TDestination>() = 
            if typeof<'TDestination>.Assembly
               |> (isNull >> not)
               && typeof<'TDestination>.Assembly |> (assemblies.Contains >> not)
               && typeof<'TDestination>.Assembly |> containsMapper then assemblies.Add(typeof<'TDestination>.Assembly)
            getMapper<'T, 'TDestination>() |> this.ToList
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        member __.ToList<'TDestination>(mapper : IMapper<'T, 'TDestination>) = sourceList |> Seq.map mapper.Map
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    let Build<'T>(source : 'T) = Builder<'T>(source)
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceList"></param>
    let BuildList<'T>(sourceList : seq<'T>) = ListBuilder<'T>(sourceList)
