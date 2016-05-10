namespace ClassBuilder.Interfaces
{
    public interface IMapper
    {
    }

    public interface IMapper<in TSource, TDestination> : IMapper
    {
        TDestination Map(TSource source);
        TDestination Map(TSource source, TDestination destination);
    }
}
