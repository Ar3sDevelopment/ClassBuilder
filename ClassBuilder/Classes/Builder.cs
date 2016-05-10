using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ClassBuilder.Interfaces;

namespace ClassBuilder.Classes
{
    public static class Builder
    {
        private static readonly ObservableCollection<Assembly> Assemblies;
        private static IContainer Container;

        static Builder()
        {
            Assemblies = new ObservableCollection<Assembly>();

            var cb = new ContainerBuilder();

            cb.RegisterGeneric(typeof(DefaultMapper<,>)).As(typeof(IMapper<,>));
            cb.RegisterAssemblyTypes(Assemblies.ToArray()).Where(IsMapper).AsImplementedInterfaces();

            Container = cb.Build();
        }

        private static bool IsMapper(Type t) => typeof(IMapper).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition;

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
    }

    public sealed class Builder<TSource>
    {
        internal Builder(TSource source)
        {
        }
    }

    public sealed class ListBuilder<TSource>
    {
        internal ListBuilder(IEnumerable<TSource> sourceList)
        {
        }
    }
}

