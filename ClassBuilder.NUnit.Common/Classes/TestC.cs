using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    public class TestC
    {
        [MapEquals]
        public string A { get; set; }

        [MapEquals]
        [MapField("B")]
        public string C { get; set; }
    }
}