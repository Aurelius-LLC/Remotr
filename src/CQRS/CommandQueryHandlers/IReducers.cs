namespace Remotr;

public interface IReduceInputs<IO>
{
    IO Execute(IO dto1, IO dto2);
}
