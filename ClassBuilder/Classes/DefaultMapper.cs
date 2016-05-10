using System;
using System.Linq;
using System.Reflection;
using ClassBuilder.Interfaces;

namespace ClassBuilder.Classes
{
    public class DefaultMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source) => Map(source, Activator.CreateInstance<TDestination>());

        public TDestination Map(TSource source, TDestination destination)
        {
            if (source.Equals(default(TSource))) return default(TDestination);

            var sourceType = source.GetType();
            var customProperties = sourceType.GetProperties().Where(t => t.CustomAttributes.Any()).ToList();

            if (sourceType.GetCustomAttributes(typeof(MapEqualsAttribute), true).Any())
            {
                foreach (var p in sourceType.GetProperties())
                    IterEqualsProperties(source, destination, p);
            }
            else
            {
                foreach (var p in customProperties.Where(t => Attribute.GetCustomAttribute(t, typeof(MapFieldAttribute)) != null))
                    IterEqualsProperties(source, destination, p);
            }

            foreach (var item in customProperties.Select(t => new { Property = t, Attribute = Attribute.GetCustomAttribute(t, typeof(MapFieldAttribute)) as MapFieldAttribute }))
                IterMapProperties(source, destination, item.Property, item.Attribute);

            return CustomMap(source, destination);
        }

        private void IterGeneric(TDestination destination, string name, object value) => destination.GetType().GetProperty(name)?.SetValue(destination, value);

        private void IterEqualsProperties(TSource source, TDestination destination, PropertyInfo propertyInfo) => IterGeneric(destination, propertyInfo.Name, propertyInfo.GetValue(source));
        private void IterMapProperties(TSource source, TDestination destination, PropertyInfo propertyInfo, MapFieldAttribute attribute) => IterGeneric(destination, attribute?.ToField, propertyInfo.GetValue(source));

        public virtual TDestination CustomMap(TSource source, TDestination destination) => destination;
    }
}
