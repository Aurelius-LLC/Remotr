namespace Remotr;

[GenerateSerializer]
public class TakeFirst<FirstT, SecondT> : IMergeInputs<FirstT, SecondT, FirstT>
{
    public FirstT Execute(FirstT dto1, SecondT dto2)
    {
        return dto1;
    }
}
