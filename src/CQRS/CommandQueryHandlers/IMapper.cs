namespace Remotr;

public interface IMapInput<Input, Output>
{
    Output Execute(Input data);
}
