using System.Runtime.CompilerServices;
using DiffEngine;

namespace NetEscapades.EnumGenerators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        DiffRunner.MaxInstancesToLaunch(1);
        DiffRunner.Disabled = true;
    }
}