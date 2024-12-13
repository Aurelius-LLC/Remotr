namespace Remotr;

public interface IMergeInputs<Input1, Input2, Output>
{
    Output Execute(Input1 dto1, Input2 dto2);
}
