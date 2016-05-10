using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassBuilder.Classes
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
