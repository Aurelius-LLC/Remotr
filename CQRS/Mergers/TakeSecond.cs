namespace Remotr;

[GenerateSerializer]
public class TakeSecond<FirstT, SecondT> : IMergeInputs<FirstT, SecondT, SecondT>
{
    public SecondT Execute(FirstT dto1, SecondT dto2)
    {
        return dto2;
    }
}
