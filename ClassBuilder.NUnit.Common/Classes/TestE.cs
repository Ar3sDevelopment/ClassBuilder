using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    [MapEquals]
    public class TestE
    {
        [MapField("C")]
        public string A { get; set; }
        [MapField("D")]
        public string B { get; set; }
        [MapField("E")]
        public string F { get; set; }
    }
}