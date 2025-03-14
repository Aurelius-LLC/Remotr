using Orleans.Runtime;
using Trackr.Backend.Core.Remotr.CQRS.BuilderFactories;

namespace Remotr.Testing;

public sealed class TestRunner : ITestRunner
{
    private readonly IGrainFactory _grainFactory;
    private readonly TestPersistentStore _persistentStore;
    private readonly CqMockContainerFactory _cqMockContainerFactory;

    public TestRunner(
        IGrainFactory grainFactory,
        TestPersistentStore persistentStore,
        CqMockContainerFactory cqMockContainerFactory
    )
    {
        _grainFactory = grainFactory;
        _persistentStore = persistentStore;
        _cqMockContainerFactory = cqMockContainerFactory;
    }

    public async Task RunTest(
        Func<IStateMockBuilder, IStateMockBuilder> stateMocker,
        Func<ICqMockBuilder, ICqMockBuilder> cqMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    )
    {
        var testId = GetNewTestId();
        RunStateMocker(stateMocker);
        RunCqMocker(testId, cqMocker);
        await RunTestWithFactories(test);
    }

    public async Task RunTest(
        Func<ICqMockBuilder, ICqMockBuilder> cqMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    )
    {
        var testId = GetNewTestId();
        RunCqMocker(testId, cqMocker);
        await RunTestWithFactories(test);
    }

    public async Task RunTest(
        Func<IStateMockBuilder, IStateMockBuilder> stateMocker,
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    )
    {
        var testId = GetNewTestId();
        RunStateMocker(stateMocker);
        await RunTestWithFactories(test);
    }

    public async Task RunTest(
        Func<IExternalCommandFactory, IExternalQueryFactory, Task> test
    )
    {
        var testId = GetNewTestId();
        await RunTestWithFactories(test);
    }

    private Guid GetNewTestId()
    {
        Guid testId = Guid.NewGuid();
        RequestContext.Set("remotrTestId", testId);
        return testId;
    }

    private void RunStateMocker(Func<IStateMockBuilder, IStateMockBuilder> stateMocker)
    {
        var stateMockBuilder = new StateMockBuilder(_grainFactory, _persistentStore);
        stateMocker(stateMockBuilder);
    }

    private void RunCqMocker(Guid testId, Func<ICqMockBuilder, ICqMockBuilder> cqMocker)
    {
        var cqMockBuilder = new CqMockBuilder();
        cqMocker(cqMockBuilder);
        var cqMocks = cqMockBuilder.GetMocks();
        var mockContainer = new CqMockContainer();
        mockContainer.AddOrUpdateTypes(cqMocks);
        _cqMockContainerFactory.SetContainer(testId, mockContainer);
    }

    private async Task RunTestWithFactories(Func<IExternalCommandFactory, IExternalQueryFactory, Task> test)
    {
        await test(
            new ExternalCommandFactory(_grainFactory),
            new ExternalQueryFactory(_grainFactory)
        );
    }
}


