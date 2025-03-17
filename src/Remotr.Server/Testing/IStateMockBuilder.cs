namespace Remotr.Testing;

public interface IStateMockBuilder
{
    public IStateMockBuilder WithState<TGrain>(Guid key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithGuidKey;
    public IStateMockBuilder WithState<TGrain>(string key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithStringKey;
    public IStateMockBuilder WithState<TGrain>(int key, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithIntegerKey;
    public IStateMockBuilder WithState<TGrain>(Guid guidKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithGuidCompoundKey;
    public IStateMockBuilder WithState<TGrain>(int intKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : IAggregateRoot, IGrainWithIntegerCompoundKey;
}
