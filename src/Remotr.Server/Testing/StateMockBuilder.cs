using Orleans.Runtime;

namespace Remotr.Testing;

public class StateMockBuilder : IStateMockBuilder
{
    private readonly IGrainFactory _grainFactory;
    private readonly TestPersistentStore _testStore;

    internal StateMockBuilder(IGrainFactory grainFactory, TestPersistentStore testStore)
    {
        _grainFactory = grainFactory;
        _testStore = testStore;
    }

    public IStateMockBuilder WithState(GrainId grainId, Dictionary<string, string> jsonState)
    {
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }

    public IStateMockBuilder WithState<TGrain>(Guid key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithGuidKey
    {
        var grainId = _grainFactory.GetGrain<TGrain>(key).GetGrainId();
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }

    public IStateMockBuilder WithState<TGrain>(string key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithStringKey
    {
        var grainId = _grainFactory.GetGrain<TGrain>(key).GetGrainId();
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }

    public IStateMockBuilder WithState<TGrain>(int key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithIntegerKey
    {
        var grainId = _grainFactory.GetGrain<TGrain>(key).GetGrainId();
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }

    public IStateMockBuilder WithState<TGrain>(Guid guidKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithGuidCompoundKey
    {
        var grainId = _grainFactory.GetGrain<TGrain>(guidKey, stringKey).GetGrainId();
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }

    public IStateMockBuilder WithState<TGrain>(int intKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithIntegerCompoundKey
    {
        var grainId = _grainFactory.GetGrain<TGrain>(intKey, stringKey).GetGrainId();
        _testStore.SetJsonState(grainId, jsonState);
        return this;
    }
}
