using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    public class TestD
    {
        [MapEquals]
        public string A { get; set; }

        [MapEquals]
        [MapField("C")]
        public string B { get; set; }
    }
}