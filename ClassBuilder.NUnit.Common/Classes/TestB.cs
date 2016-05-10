using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    [MapEquals]
    public class TestB
    {
        public string A { get; set; }

        [MapField("C")]
        public string B { get; set; }
    }
}