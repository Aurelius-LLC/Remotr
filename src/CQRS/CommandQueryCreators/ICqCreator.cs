namespace Remotr;

public interface ICqCreator
{
    TRequired InstantiateQuery<TActual, TRequired>()
        where TActual : notnull, ICanRead, TRequired;

    TRequired InstantiateCommand<TActual, TRequired>()
        where TActual : notnull, ICanReadAndWrite, TRequired;

    T InstantiateMapper<T>() where T : notnull;
    T InstantiateMerger<T>() where T : notnull;
    T InstantiateReducer<T>() where T : notnull;
}
