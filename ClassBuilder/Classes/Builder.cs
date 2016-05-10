using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Autofac;
using ClassBuilder.Interfaces;

namespace ClassBuilder.Classes
{
    public static class Builder
    {
        private static readonly ObservableCollection<Assembly> Assemblies;
        private static readonly IContainer Container;

        static Builder()
        {
            Assemblies = new ObservableCollection<Assembly>();

            var cb = new ContainerBuilder();

            cb.RegisterGeneric(typeof(DefaultMapper<,>)).As(typeof(IMapper<,>));
            cb.RegisterAssemblyTypes(Assemblies.ToArray()).Where(IsMapper).AsImplementedInterfaces();

            Container = cb.Build();

            Assemblies.CollectionChanged += (sender, args) =>
            {
                RegisterAssemblies(args.NewItems.Cast<Assembly>().Where(t => t != null).ToArray());
            };

            RegisterAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        internal static void RegisterAssemblies(params Assembly[] assemblies)
        {
            var cb = new ContainerBuilder();
            cb.RegisterAssemblyTypes(assemblies).Where(IsMapper).AsImplementedInterfaces();
            cb.Update(Container);

            foreach (var a in assemblies.SelectMany(t => t.GetReferencedAssemblies()).Select(AssemblyLoadSecure).Where(t => t != null && !assemblies.Contains(t) && ContainsMapper(t)))
            {
                Assemblies.Add(a);
            }
        }

        internal static IMapper<TSource, TDestination> GetMapper<TSource, TDestination>()
        {
            IMapper<TSource, TDestination> mapper;

            using (var scope = Container.BeginLifetimeScope())
            {
                scope.TryResolve(out mapper);
            }

            return mapper;
        }

        private static bool IsMapper(Type t) => typeof(IMapper).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition;

        private static Assembly AssemblyLoadSecure(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static bool ContainsMapper(Assembly a)
        {
            try
            {
                return a.GetTypes().Any(IsMapper);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void RegisterMapper<TMapper>() where TMapper : IMapper
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<TMapper>().AsImplementedInterfaces();
            cb.Update(Container);
        }

        public static Builder<TSource> Build<TSource>(TSource source) => new Builder<TSource>(source);

        public static ListBuilder<TSource> BuildList<TSource>(IEnumerable<TSource> sourceList) => new ListBuilder<TSource>(sourceList);
    }

    public sealed class Builder<TSource>
    {
        private readonly TSource _source;

        internal Builder(TSource source)
        {
            _source = source;
        }

        public TDestination To<TDestination>()
        {
            Builder.RegisterAssemblies(typeof(TDestination).Assembly);

            return To(Activator.CreateInstance<TDestination>(), Builder.GetMapper<TSource, TDestination>());
        }

        public TDestination To<TDestination>(TDestination destination)
        {
            Builder.RegisterAssemblies(typeof(TDestination).Assembly);

            return To(destination, Builder.GetMapper<TSource, TDestination>());
        }

        public TDestination To<TDestination>(IMapper<TSource, TDestination> mapper)
        {
            Builder.RegisterAssemblies(typeof(TDestination).Assembly);

            return To(Activator.CreateInstance<TDestination>(), mapper);
        }

        public TDestination To<TDestination>(TDestination destination, IMapper<TSource, TDestination> mapper)
        {
            return _source?.Equals(default(TSource)) ?? true ? default(TDestination) : mapper.Map(_source, destination);
        }
    }

    public sealed class ListBuilder<TSource>
    {
        private readonly IEnumerable<TSource> _sourceList;

        internal ListBuilder(IEnumerable<TSource> sourceList)
        {
            _sourceList = sourceList;
        }

        public IEnumerable<TDestination> ToList<TDestination>()
        {
            Builder.RegisterAssemblies(typeof(TDestination).Assembly);

            return ToList(Builder.GetMapper<TSource, TDestination>());
        }

        public IEnumerable<TDestination> ToList<TDestination>(IMapper<TSource, TDestination> mapper)
        {
            Builder.RegisterAssemblies(typeof(TDestination).Assembly);

            return _sourceList.Select(mapper.Map);
        }
    }
}

