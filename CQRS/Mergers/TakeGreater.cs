﻿namespace Remotr;

/// <summary>
/// Returns whichever result is the greater, or the first result if they are equal.
/// </summary>
[GenerateSerializer]
public class TakeGreater<T> : IMergeInputs<T, T, T>
    where T : IComparable<T>
{
    public T Execute(T dto1, T dto2)
    {
        return dto1.CompareTo(dto2) >= 0 ? dto1 : dto2;
    }
}
