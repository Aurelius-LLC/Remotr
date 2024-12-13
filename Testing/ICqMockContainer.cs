namespace Remotr.Testing;

public interface ICqMockContainer
{
    bool Get<TActual, TRequired>(out TRequired? implementation);
}

