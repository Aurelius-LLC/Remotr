using Remotr;
using Remotr.Testing;

namespace Remotrtests.TestStateBuilder;

public interface ITestStateBuilder
{
    Task SetupTestState(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory);

    ICqMockBuilder CreateMocks(ICqMockBuilder mocker);
}
