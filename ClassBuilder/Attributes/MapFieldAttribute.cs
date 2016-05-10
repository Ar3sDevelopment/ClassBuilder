using System;

namespace ClassBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MapFieldAttribute : Attribute
    {
        public string ToField { get; set; }
        public MapFieldAttribute(string toField)
        {
            ToField = toField;
        }
    }
}
