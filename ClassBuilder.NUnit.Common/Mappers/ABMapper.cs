using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassBuilder.Classes;
using ClassBuilder.NUnit.Common.Classes;

namespace ClassBuilder.NUnit.Common.Mappers
{
    public class ABMapper : DefaultMapper<TestA, TestB>
    {
        public override TestB CustomMap(TestA source, TestB dest)
        {
            var res = base.CustomMap(source, dest);

            res.B = res.B + " mapper";

            return res;
        }
    }
}
