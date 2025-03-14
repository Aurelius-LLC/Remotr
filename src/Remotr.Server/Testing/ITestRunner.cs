namespace Remotr.Testing;

public interface ITestRunner
{
    Task RunTest(
        Func<IStateMockBuilder, IStateMockBuilder> stateMocker,
        Func<ICqMockBuilder, ICqMockBuilder> cqMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    );

    Task RunTest(
        Func<ICqMockBuilder, ICqMockBuilder> cqMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    );

    Task RunTest(
        Func<IStateMockBuilder, IStateMockBuilder> stateMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    );

    Task RunTest(
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    );
}

