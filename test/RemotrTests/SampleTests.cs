using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remotr;
using Remotr.Testing;
using Remotr.Samples.Calculator;
using Remotr.Samples.Calendar;

namespace RemotrTests.Tests;

[Collection(ClusterCollection.Name)]
public class RemotrTests(ClusterFixture fixture)
{
    [Fact]
    public async Task SimpleExamplesTest()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                SimpleExamples simpleExamples = new SimpleExamples(commandFactory, queryFactory);
                var simpleArithmeticTest = await simpleExamples.SimpleArithmetic("simpleArithmetic"); 
                var cachingChainsTest = await simpleExamples.CachingChains("target1", "target2");
                var advancedChainingTest = await simpleExamples.AdvancedChaining("advancedChaining");
                var advancedChaining2Test = await simpleExamples.AdvancedChaining2("advancedChaining2");

                // The value from the advanced chaining test should be different than what the state is.
                // This checks what the state's value is.
                var valueFromAdvancedChainingTest = await queryFactory.GetAggregate<ICalculatorAggregate>()
                    .GetValue()
                    .Run("advancedChaining");

                // The value from the advanced chaining test 2 should be different than what the state is.
                // This checks what the state's value is.
                var valueFromAdvancedChainingTest2 = await queryFactory.GetAggregate<ICalculatorAggregate>()
                    .GetValue()
                    .Run("advancedChaining2");

                simpleArithmeticTest.Should().Be(4.0);
                cachingChainsTest.Should().Be((1.0, 1.0, 1.5));
                advancedChainingTest.Should().Be(15800.0);
                valueFromAdvancedChainingTest.Should().Be(12000.0);

                advancedChaining2Test.ToList().Should().BeEquivalentTo([11, 26, 87]);
                valueFromAdvancedChainingTest2.Should().Be(87.0);
            }
        );
    }

    /*

    private static ICqMockBuilder MockCreateOrUpdateReminder(ICqMockBuilder mocker)
    {
        var createOrUpdateReminderMock = new Mock<IAsyncCommandHandler<ITransactionChildGrain<FriendChatState>, CreateOrUpdateReminderInput, bool>>();
        createOrUpdateReminderMock.Setup(m => m.Execute(It.IsAny<CreateOrUpdateReminderInput>()))
            .ReturnsAsync(true);

        return mocker.MockChild
                <FriendChatState,
                CreateOrUpdateReminder<FriendChatState>,
                CreateOrUpdateReminderInput,
                bool>(createOrUpdateReminderMock.Object);
    }

    private static ICqMockBuilder MockUseSkipDayBoosterTrackrCard(ICqMockBuilder mocker)
    {
        var mock = new Mock<IAsyncCommandHandler<ITransactionChildGrain<TrackrPacksManagerState>, Guid, bool>>();
        mock.Setup(m => m.Execute(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        return mocker.MockChild<TrackrPacksManagerState, Trackr.Backend.Services.TrackrPackService.TrackrPacksManager.Commands.UseSkipDayBoosterTrackrCard, Guid, bool>(mock.Object);
    }

    [Fact]
    public async Task Send20MessagesTest()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => MockCreateOrUpdateReminder(mocker),
            async (commandFactory, queryFactory) =>
            {
                var utcNow = DateTime.UtcNow;
                var userId1 = Guid.NewGuid();
                var userId2 = Guid.NewGuid();
                var chatId = userId1 < userId2 ? $"{userId1}_{userId2}" : $"{userId2}_{userId1}";
                var messageIds = new List<Guid>();
                for (var i = 0; i < 20; i++)
                {
                    messageIds.Add(Guid.NewGuid());
                }
                for (var i = 0; i < 20; i++)
                {
                    await commandFactory.GetManager<IFriendChatApiGrain>()
                        .Tell<SendMessage, SendMessageInput, Message>(new SendMessageInput(messageIds[i], userId1, utcNow + TimeSpan.FromSeconds(i), $"{i}", [], [], [], [], [], false, string.Empty, string.Empty))
                        .Run(chatId);
                }

                var queriedMessage = await queryFactory.GetManager<IFriendChatApiGrain>()
                    .Ask<GetMessages, GetMessagesInput, MessagesWithReadReceipts>(new GetMessagesInput(20))
                    .Run(chatId);
                queriedMessage.Messages.Should().HaveCount(20);
                queriedMessage.Messages.Select(m => m.Text).Should().BeEquivalentTo(Enumerable.Range(0, 20).Select(i => $"{i}"));
            }
        );
    }

    [Fact]
    public async Task UpdateReadReceipt()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => MockCreateOrUpdateReminder(mocker),
            async (commandFactory, queryFactory) =>
            {
                var utcNow = DateTime.UtcNow;
                var userId1 = Guid.NewGuid();
                var userId2 = Guid.NewGuid();
                var chatId = userId1 < userId2 ? $"{userId1}_{userId2}" : $"{userId2}_{userId1}";
                var returnedValue = await commandFactory.GetManager<IFriendChatApiGrain>()
                       .Tell<UpdateReadReceipt, UpdateReadReceiptInput, DateTime>(new UpdateReadReceiptInput(userId1, utcNow))
                       .Run(chatId);

                var readReceipts = await queryFactory.GetManager<IFriendChatApiGrain>()
                    .Ask<GetReadReceipts, ImmutableSortedDictionary<Guid, DateTime>>()
                    .Run(chatId);
                readReceipts.Should().ContainKey(userId1);
                readReceipts[userId1].Should().Be(returnedValue);
            }
        );
    }

    [Fact]
    public async Task CreateNewUser()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            async (commandFactory, queryFactory) =>
            {
                var userId = Guid.NewGuid();
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<Trackr.Backend.Services.UserApiService.UserApi.Commands.AddUser, AddUserInputWithFirebaseId, bool>(new AddUserInputWithFirebaseId("jjjjj", "919-481-7112", "jack", "Jack", new DateOnly(2002, 4, 3), "America/New_York", "en-us", true, "notificationDeviceId"))
                    .Run(userId);
                var user = await queryFactory.GetManager<IUserApiGrain>()
                    .Ask<GetUser, User>()
                    .Run(userId);

                user.Id.Should().Be(userId);
                user.DisplayName.Should().Be("Jack");
            }
        );
    }

    [Fact]
    public async Task CreateGoal()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [],
                    })
                    .Run(userId);
                var goal = pointsEarned.Entity!;
                goal.Id.Should().Be(goalId);
                goal.Title.Should().Be("Test Goal");
                goal.Deadline.Should().Be(deadline);
            }
        );
    }

    [Fact]
    public async Task CreateGoalWithRoutine()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var routineId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [
                            new DayRoutineInput
                            {
                                Id = routineId,
                                Title = "Test Routine",
                                Description = "Test Description",
                                DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Tuesday]
                            }
                        ],
                    })
                    .Run(userId);
                var goal = pointsEarned.Entity!;
                var routine = goal.Routines[0];
                routine.Id.Should().Be(routineId);
                routine.Title.Should().Be("Test Routine");
                routine.Description.Should().Be("Test Description");
                routine.ActivityDays.Select(x => x.Day).Should().BeEquivalentTo([DayOfWeek.Monday, DayOfWeek.Tuesday]);
            }
        );
    }

    [Fact]
    public async Task CreateGoalWithMilestone()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var milestoneId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [
                            new MilestoneInput
                            {
                                Id = milestoneId,
                                Title = "Test Milestone",
                            }
                        ],
                        Routines = [],
                    })
                    .Run(userId);
                var goal = pointsEarned.Entity!;
                var milestone = goal.Milestones[0];
                milestone.Id.Should().Be(milestoneId);
                milestone.Title.Should().Be("Test Milestone");
            }
        );
    }

    [Fact]
    public async Task CompleteMilestone()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var milestoneId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [
                            new MilestoneInput
                            {
                                Id = milestoneId,
                                Title = "Test Milestone",
                            }
                        ],
                        Routines = [],
                    })
                    .Run(userId);
                var completeMilestonePointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CompleteMilestone, (Guid GoalId, Guid MilestoneId), PointsEarned<Milestone>>((goalId, milestoneId))
                    .Run(userId);
                completeMilestonePointsEarned.Points.Should().BePositive();
            }
        );
    }

    [Fact]
    public async Task CheckInToRoutine()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var routineId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [
                            new DayRoutineInput
                            {
                                Id = routineId,
                                Title = "Test Routine",
                                Description = "Test Description",
                                DaysOfWeek = [.. Enum.GetValues<DayOfWeek>()]
                            }
                        ],
                    })
                    .Run(userId);
                var checkInToRoutinePointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, DateTime.UtcNow.DayOfWeek))
                    .Run(userId);
                var routine = checkInToRoutinePointsEarned.Entity!;
                routine.Id.Should().Be(routineId);
                checkInToRoutinePointsEarned.Points.Should().BePositive();
            }
        );
    }

    [Fact]
    public async Task CheckInToRoutineAfterLosingStreak()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            testStateBuilder.CreateMocks,
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var routineId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [
                            new DayRoutineInput
                            {
                                Id = routineId,
                                Title = "Test Routine",
                                Description = "Test Description",
                                DaysOfWeek = [.. Enum.GetValues<DayOfWeek>()]
                            }
                        ],
                    })
                    .Run(userId);
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, DateTime.UtcNow.DayOfWeek))
                    .Run(userId);
                var threeDaysFromNow = DateTime.UtcNow.AddDays(3);
                UtcDateService.DateOverride = threeDaysFromNow;
                var checkInToRoutinePointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, threeDaysFromNow.DayOfWeek))
                    .Run(userId);
                var routine = checkInToRoutinePointsEarned.Entity!;
                routine.LastPositiveStreak.Should().Be(1);
                routine.Streak.Should().Be(1);
            }
        );
    }

    [Fact]
    public async Task UseSkipDayCard()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(m =>
            {
                testStateBuilder.CreateMocks(m);
                MockUseSkipDayBoosterTrackrCard(m);
                return m;
            },
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var routineId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [
                            new DayRoutineInput
                            {
                                Id = routineId,
                                Title = "Test Routine",
                                Description = "Test Description",
                                DaysOfWeek = [.. Enum.GetValues<DayOfWeek>()]
                            }
                        ],
                    })
                    .Run(userId);

                // Check in to routine
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, DateTime.UtcNow.DayOfWeek))
                    .Run(userId);

                // Force the date to be one day from now
                var oneDayFromNow = DateTime.UtcNow.AddDays(1);
                UtcDateService.DateOverride = oneDayFromNow;

                // Use the skip day card
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<UseSkipDayBoosterTrackrCard, Guid, bool>(Guid.Empty)
                    .Run(userId);

                // Force the date to be two days from now
                var twoDaysFromNow = oneDayFromNow.AddDays(1);
                UtcDateService.DateOverride = twoDaysFromNow;

                // Get routine due today
                var routines = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<GetRoutinesDueToday, List<RoutineSummary>>()
                    .Run(userId);
                routines.Should().ContainSingle();
                routines[0].Streak.Should().Be(1);
            }
        );
    }

    [Fact]
    public async Task CheckInAfterSkipDay()
    {
        var userId = Guid.NewGuid();
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(m =>
        {
            testStateBuilder.CreateMocks(m);
            MockUseSkipDayBoosterTrackrCard(m);
            return m;
        },
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);
                var goalId = Guid.NewGuid();
                var routineId = Guid.NewGuid();
                var deadline = DateTime.UtcNow.AddDays(30);
                var pointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<AddGoal, GoalInput, PointsEarned<Goal>>(new GoalInput
                    {
                        Id = goalId,
                        Title = "Test Goal",
                        Deadline = deadline,
                        Milestones = [],
                        Routines = [
                            new DayRoutineInput
                            {
                                Id = routineId,
                                Title = "Test Routine",
                                Description = "Test Description",
                                DaysOfWeek = [.. Enum.GetValues<DayOfWeek>()]
                            }
                        ],
                    })
                    .Run(userId);

                // Check in to routine
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, DateTime.UtcNow.DayOfWeek))
                    .Run(userId);

                // Force the date to be one day from now
                var oneDayFromNow = DateTime.UtcNow.AddDays(1);
                UtcDateService.DateOverride = oneDayFromNow;

                // Use the skip day card
                await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<UseSkipDayBoosterTrackrCard, Guid, bool>(Guid.Empty)
                    .Run(userId);

                // Force the date to be two days from now
                var twoDaysFromNow = oneDayFromNow.AddDays(1);
                UtcDateService.DateOverride = twoDaysFromNow;

                // Check in to routine
                var checkInToRoutinePointsEarned = await commandFactory.GetManager<IUserApiGrain>()
                    .Tell<CheckRoutineIn, (Guid GoalId, Guid RoutineId, DayOfWeek DayOfWeek), PointsEarned<Routine>>((goalId, routineId, twoDaysFromNow.DayOfWeek))
                    .Run(userId);
                var routine = checkInToRoutinePointsEarned.Entity!;
                routine.Streak.Should().Be(2);
            }
        );
    }

    [Fact]
    public async Task DenormalizedChatsSyncing()
    {
        var utcNow = DateTime.UtcNow;
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var chatId = userId1 < userId2 ? $"{userId1}_{userId2}" : $"{userId2}_{userId1}";
        var timestamp = DateTime.UtcNow.AddHours(1);
        var testStateBuilder = TestStateBuilder.TestStateBuilder.CreateBuilder()
            .CreateUser(userId1)
            .CreateUser(userId2)
            .FriendAllUsers()
            .SendFriendMessage(userId1, userId2)
            .UpdateFriendChatReadReciept(userId2, userId1, timestamp)
            .SyncFriendChatSummary(userId1, userId2);

        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(m =>
        {
            testStateBuilder.CreateMocks(m);
            return m;
        },
            async (commandFactory, queryFactory) =>
            {
                await testStateBuilder.SetupTestState(fixture.Cluster.GrainFactory, commandFactory, queryFactory);

                var latestMessages = await commandFactory
                    .GetManager<IUserApiGrain>()
                    .Tell<LatestMessages, DateTime, MessageUpdates?>(utcNow.AddHours(-1))
                    .Run(userId1);
                latestMessages.Should().NotBeNull();
                latestMessages!.ChatsWithUpdates.Should().ContainSingle();
                latestMessages!.ChatsWithUpdates[0].ChatType.Should().Be(ChatType.Friend);
                latestMessages!.ChatsWithUpdates[0].ChatId.Should().Be(chatId);
                latestMessages!.ChatsWithUpdates[0].NewMessages.Should().ContainSingle();
                latestMessages!.ChatsWithUpdates[0].NewMessages[0].Text.Should().Be("Test");
                latestMessages!.ChatsWithUpdates[0].UpdatedReadReceipts.Should().ContainSingle();
                latestMessages!.ChatsWithUpdates[0].UpdatedReadReceipts[0].UserId.Should().Be(userId2);
                latestMessages!.ChatsWithUpdates[0].UpdatedReadReceipts[0].Timestamp.Should().Be(timestamp);
            }
        );
    }
    */
}
