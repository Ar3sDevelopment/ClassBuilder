using ClassBuilder.Classes;

namespace ClassBuilder.NUnit.Common.Mappers
{
    public class IntFloatMapper : DefaultMapper<int, float>
    {
        public override float CustomMap(int source, float dest)
        {
            base.CustomMap(source, dest);
            return source;
        }
    }
}