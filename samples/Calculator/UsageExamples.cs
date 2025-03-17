

namespace Remotr.Samples.Calculator;

public class SimpleExamples(IExternalCommandFactory commandFactory, IExternalQueryFactory queryFactory)
{
    private readonly IExternalCommandFactory commandFactory = commandFactory;
    private readonly IExternalQueryFactory queryFactory = queryFactory;

    // The below operation on the ICalculatorManagerGrain:
        // 1. Is sent as a single RPC
        // 2. Will be executed in the order specified
        // 3. Will all succeed or fail together (ACID)
    public async Task SimpleArithmetic() {
        // CalculatorState starts at 0.
        // This algorithm returns 1 given an initial state of 0.
        var algorithm = commandFactory.GetManager<ICalculatorManagerGrain>()
            // Now 2.0
            .Add(2.0)
            // Now 4.0
            .Multiply(2.0)
            // Returns 4.0
            .GetValue();
    }

    public async Task CachingChains() {

        // CalculatorState starts at 0.
        // This algorithm returns 1 given an initial state of 0.
        var algorithm = commandFactory.GetManager<ICalculatorManagerGrain>()
            // Now 2
            .Add(2.0)
            // Now 1
            .Divide(2.0)
            // Returns 1
            .GetValue();

        // The algorithm is cached in a local variable.
        // This chain of commands can be reused for multiple targets.
        var result1 = await algorithm.Run("Target1"); // Returns 1
        var result2 = await algorithm.Run("Target2"); // Returns 1

        // Running the algorithm on the first target again will have a new result
        // Because it started at 1 instead of 0 due to the previous call.
        var result3 = await algorithm.Run("Target1"); // Returns 1.5
    }

    public async Task AdvancedChaining() {
        // This example demonstrates advanced chaining with forEach reduce operations
        
        // Start with adding 120 to the calculator state (initially 0)
        // Then get its prime factors (for 120: [2, 2, 2, 3, 5])
        // Then multiply each factor by 10 ([20, 20, 20, 30, 50])
        // Then set the state to 10
        // Then sum them all together (140) and return this value.
        await commandFactory.GetManager<ICalculatorManagerGrain>()
            // Add 120 to the initial state (0), making it 120
            .Add(120.0)
            // Get the prime factors of 120: [2, 2, 2, 3, 5]
            .GetPrimeFactors()
            // MergeSplit allows us to split the builder into two branches.
            // They must be merged back together at the end.
            // Several default strategies exist to merge the branches back together.
            // e.g. TakeFirst, TakeSecond, TakeGreater, TakeLesser, TakeTuple, etc.
            .MergeSplit(
                // Set the state to 10
                (b1) => b1.Set(10.0),

                // Multiply each factor by 10
                // Multiply each factor by 10
                // "Then" keyword signals that we use the previous output as the input for "this" command/query.
                // However, we're not setting the state here.
                (b2) => 
                    b2.ThenForEach(
                        (b) => b.Multiply(10.0)
                    // Sum all the values: 20 + 20 + 20 + 30 + 50 = 140
                    ).ThenReduce(
                        new SumReducer()
                    ),
                
                // Take the second value, which is the sum of the factors.
                new TakeSecond<double, double>()
            )
            // Run for a given target.
            .Run("Target1");

        // The state is now 10, however, the returned value is 140.
    }

    public async Task OnlyQueries() {
        // Because we are using the query factory, we can only use queries.
        // This is infectious, meaning that queries run here can only run queries themselves.
        // Assuming the state is 120, the prime factors of 120: [2, 2, 2, 3, 5]
        var primeFactors = await queryFactory.GetManager<ICalculatorManagerGrain>()
            .GetPrimeFactors()
            .Run("Target1");
    }
}

