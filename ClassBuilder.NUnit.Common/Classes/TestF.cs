using ClassBuilder.Attributes;

namespace ClassBuilder.NUnit.Common.Classes
{
    [MapEquals]
    public class TestF
    {
        [MapField("A")]
        public string C { get; set; }

        [MapField("B")]
        public string D { get; set; }
    }
}