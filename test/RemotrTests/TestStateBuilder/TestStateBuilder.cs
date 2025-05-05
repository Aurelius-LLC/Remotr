using Remotr;
using Remotr.Testing;

namespace Remotrtests.TestStateBuilder;

public class TestStateBuilder : ITestStateBuilder
{

    public Task SetupTestState(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory)
    {
        return Task.CompletedTask;
    }

    public ICqMockBuilder CreateMocks(ICqMockBuilder mocker)
    {
        // Add mocks here.
        return mocker;
    }

    /*
    private readonly List<ICommand> _commands = [];

    public static TestStateBuilder CreateBuilder()
    {
        return new TestStateBuilder();
    }

    public async Task SetupTestState(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory)
    {
        for (int i = 0; i < _commands.Count; i++)
        {
            await _commands[i].Execute(grainFactory, externalCommandFactory, externalQueryFactory, _commands.Take(i));
        }
    }

    public TestStateBuilder CreateUser(Guid? userId = null)
    {
        var previousCreateUserCommandsCount = _commands.Count(c => c is CreateUserCommand);
        userId ??= Guid.NewGuid();
        string firebaseId = $"FIREBASE_ID_{previousCreateUserCommandsCount}";
        string phoneNumber = $"PHONE_NUMBER_{previousCreateUserCommandsCount}";
        string handle = $"HANDLE_{previousCreateUserCommandsCount}";
        string displayName = "Test";
        DateOnly birthDate = new(1990, 1, 1);
        string dateTimeZone = "America/New_York";
        string cultureInfoName = "en-US";
        string notificationDeviceId = userId.Value.ToString();
        bool isTestUser = true;
        _commands.Add(new CreateUserCommand(new AddUserInputWithFirebaseId(firebaseId, phoneNumber, handle, displayName, birthDate, dateTimeZone, cultureInfoName, isTestUser, notificationDeviceId), userId.Value));
        return this;
    }

    public TestStateBuilder FriendAllUsers()
    {
        _commands.Add(new FriendAllUsersCommand());
        return this;
    }

    public TestStateBuilder SendFriendMessage(Guid senderId, Guid receiverId)
    {
        _commands.Add(new SendFriendMessageCommand(senderId, receiverId));
        return this;
    }

    public TestStateBuilder UpdateFriendChatReadReciept(Guid userId, Guid friendId, DateTime timestamp)
    {
        _commands.Add(new UpdateFriendChatReadRecieptCommand(userId, friendId, timestamp));
        return this;
    }

    public TestStateBuilder SyncFriendChatSummary(Guid userId, Guid friendId)
    {
        _commands.Add(new SyncFriendChatSummaryCommand(userId, friendId));
        return this;
    }

    public ICqMockBuilder CreateMocks(ICqMockBuilder mocker)
    {
        // Add mocks here.
        return mocker;
    }
    */
}

/*
public interface ICommand
{
    public Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands);
}

public interface ICommand<T> : ICommand
{
    public T Input { get; }
}

public class CreateUserCommand : ICommand<AddUserInputWithFirebaseId>
{
    private readonly AddUserInputWithFirebaseId _input;

    private readonly Guid _userId;

    public CreateUserCommand(AddUserInputWithFirebaseId input, Guid userId)
    {
        _input = input;
        _userId = userId;
    }

    public AddUserInputWithFirebaseId Input => _input;

    public Guid UserId => _userId;

    public async Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands)
    {
        List<Guid> userIds = [];
        foreach (var command in previousCommands)
        {
            if (command is CreateUserCommand u)
            {
                userIds.Add(u.UserId);
            }
        }
        if (userIds.Contains(UserId))
        {
            throw new InvalidOperationException("User already exists");
        }
        await externalCommandFactory
            .GetManager<IUserApiGrain>()
            .Tell<Trackr.Backend.Services.UserApiService.UserApi.Commands.AddUser, AddUserInputWithFirebaseId, bool>(_input)
            .Run(UserId);
    }
}

public class FriendAllUsersCommand : ICommand
{
    public async Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands)
    {
        List<Guid> userIds = [];
        foreach (var command in previousCommands)
        {
            if (command is CreateUserCommand u)
            {
                userIds.Add(u.UserId);
            }
        }
        List<(Guid, Guid)> friendRequests = [];
        for (int i = 0; i < userIds.Count; i++)
        {
            for (int j = i + 1; j < userIds.Count; j++)
            {
                var sender = userIds[i];
                var receiver = userIds[j];
                var relationshipId = sender < receiver ? $"{sender}_{receiver}" : $"{receiver}_{sender}";
                var relationshipGrain = grainFactory.GetGrain<IRelationshipGrain>(relationshipId);
                var relationshipState = await relationshipGrain.GetRelationshipStatus(sender);
                // Handle relationship state
                friendRequests.Add((sender, receiver));
            }
        }
        foreach (var (userId, friendId) in friendRequests)
        {
            await externalCommandFactory
                .GetManager<IUserApiGrain>()
                .Tell<Trackr.Backend.Services.UserApiService.UserApi.Commands.SendFriendRequest, Guid, bool>(friendId)
                .Run(userId);
            await externalCommandFactory
                .GetManager<IUserApiGrain>()
                .Tell<Trackr.Backend.Services.UserApiService.UserApi.Commands.AcceptFriendRequest, Guid, bool>(userId)
                .Run(friendId);
        }
    }
}

public class SendFriendMessageCommand : ICommand
{
    private readonly Guid _senderId;
    private readonly Guid _receiverId;

    public SendFriendMessageCommand(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId)
        {
            throw new InvalidOperationException("Cannot send message to self");
        }
        _senderId = senderId;
        _receiverId = receiverId;
    }

    public async Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands)
    {
        var chatId = _senderId < _receiverId ? $"{_senderId}_{_receiverId}" : $"{_receiverId}_{_senderId}";
        await externalCommandFactory
            .GetManager<IFriendChatApiGrain>()
            .Tell<Trackr.Backend.Services.ChatService.FriendChatApi.Commands.SendMessage, Trackr.Backend.Services.ChatService.FriendChatApi.Commands.SendMessageInput, Trackr.Backend.Services.ChatService.ViewModels.Message>(new(
                    Guid.NewGuid(),
                    _senderId,
                    DateTime.UtcNow,
                    "Test",
                    [],
                    [],
                    [],
                    [],
                    [],
                    false,
                    string.Empty,
                    string.Empty))
            .Run(chatId);
    }
}

public class UpdateFriendChatReadRecieptCommand : ICommand
{
    private readonly Guid _userId;
    private readonly Guid _friendId;
    private readonly DateTime _timestamp;

    public UpdateFriendChatReadRecieptCommand(Guid userId, Guid friendId, DateTime timestamp)
    {
        if (userId == friendId)
        {
            throw new InvalidOperationException("Cannot update read receipt for self");
        }
        _userId = userId;
        _friendId = friendId;
        _timestamp = timestamp;
    }
    public async Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands)
    {
        var chatId = _userId < _friendId ? $"{_userId}_{_friendId}" : $"{_friendId}_{_userId}";
        await externalCommandFactory
            .GetManager<IFriendChatApiGrain>()
            .Tell<Trackr.Backend.Services.ChatService.FriendChatApi.Commands.UpdateReadReceipt, Trackr.Backend.Services.ChatService.FriendChatApi.Commands.UpdateReadReceiptInput, DateTime>(new(_userId, _timestamp))
            .Run(chatId);
    }
}

public class SyncFriendChatSummaryCommand : ICommand
{
    private readonly Guid _userId;
    private readonly Guid _friendId;

    public SyncFriendChatSummaryCommand(Guid userId, Guid friendId)
    {
        if (userId == friendId)
        {
            throw new InvalidOperationException("Cannot sync chat summary for self");
        }
        _userId = userId;
        _friendId = friendId;
    }

    public async Task Execute(IGrainFactory grainFactory, IExternalCommandFactory externalCommandFactory, IExternalQueryFactory externalQueryFactory, IEnumerable<ICommand> previousCommands)
    {
        var chatId = _userId < _friendId ? $"{_userId}_{_friendId}" : $"{_friendId}_{_userId}";
        await externalCommandFactory
            .GetManager<IUserApiGrain>()
            .Tell<Trackr.Backend.Services.UserApiService.UserApi.Commands.SyncChatSummary, Trackr.Backend.Services.ChatService.DenormalizedChats.Commands.SyncChatSummaryInput, bool>(new(ChatType.Friend, chatId))
            .Run(_userId);
    }
}

*/