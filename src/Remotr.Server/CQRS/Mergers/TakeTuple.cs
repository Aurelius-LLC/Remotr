namespace Remotr;

[GenerateSerializer]
public class TakeTuple<FirstT, SecondT> : IMergeInputs<FirstT, SecondT, (FirstT, SecondT)>
{
    public (FirstT, SecondT) Execute(FirstT dto1, SecondT dto2)
    {
        return (dto1, dto2);
    }
}
