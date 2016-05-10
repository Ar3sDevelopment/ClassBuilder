using System;

namespace ClassBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class MapEqualsAttribute : Attribute
    {
    }
}
