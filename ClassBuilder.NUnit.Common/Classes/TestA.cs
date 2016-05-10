using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    [MapEquals]
    public class TestA
    {
        public string A { get; set; }

        [MapField("B")]
        public string C { get; set; }
    }
}
