namespace Remotr.Testing;

public interface IStateMockBuilder
{
    public IStateMockBuilder WithState<TGrain>(Guid key, Dictionary<string, string> jsonState) where TGrain : ITransactionManagerGrain, IGrainWithGuidKey;
    public IStateMockBuilder WithState<TGrain>(string key, Dictionary<string, string> jsonState) where TGrain : ITransactionManagerGrain, IGrainWithStringKey;
    public IStateMockBuilder WithState<TGrain>(int key, Dictionary<string, string> jsonState) where TGrain : ITransactionManagerGrain, IGrainWithIntegerKey;
    public IStateMockBuilder WithState<TGrain>(Guid guidKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : ITransactionManagerGrain, IGrainWithGuidCompoundKey;
    public IStateMockBuilder WithState<TGrain>(int intKey, string stringKey, Dictionary<string, string> jsonState) where TGrain : ITransactionManagerGrain, IGrainWithIntegerCompoundKey;
}
