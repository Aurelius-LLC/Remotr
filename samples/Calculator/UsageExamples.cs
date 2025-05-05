

namespace Remotr.Samples.Calculator;

public class SimpleExamples(IExternalCommandFactory commandFactory, IExternalQueryFactory queryFactory)
{
    private readonly IExternalCommandFactory commandFactory = commandFactory;
    private readonly IExternalQueryFactory queryFactory = queryFactory;

    // The below operation on the ICalculatorAggregate:
        // 1. Is sent as a single RPC
        // 2. Will be executed in the order specified
        // 3. Will all succeed or fail together (ACID)
    public async Task<double> SimpleArithmetic(string key) 
    {
        // CalculatorState starts at 0.
        // This algorithm returns 1 given an initial state of 0.
        var algorithm = commandFactory.GetAggregate<ICalculatorAggregate>()
            // Now 2.0
            .Add(2.0)
            // Now 4.0
            .Multiply(2.0);

        return await algorithm.Run(key);
    }

    public async Task<(double, double, double)> CachingChains(string target1Id, string target2Id) 
    {

        // CalculatorState starts at 0.
        // This algorithm returns 1 given an initial state of 0.
        var algorithm = commandFactory.GetAggregate<ICalculatorAggregate>()
            // Now 2
            .Add(2.0)
            // Now 1
            .Divide(2.0)
            // Returns 1
            .GetValue();

        // The algorithm is cached in a local variable.
        // This chain of commands can be reused for multiple targets.
        var result1 = await algorithm.Run(target1Id); // Returns 1
        var result2 = await algorithm.Run(target2Id); // Returns 1

        // Running the algorithm on the first target again will have a new result
        // Because it started at 1 instead of 0 due to the previous call.
        var result3 = await algorithm.Run(target1Id); // Returns 1.5

        return (result1, result2, result3);
    }

    public async Task<double> AdvancedChaining(string key) 
    {
        // This example demonstrates advanced chaining with forEach and reduce operations
        
        // Start with adding 120 to the calculator state (initially 0)
        // Then get its prime factors (for 120: [2, 2, 2, 3, 5])
        // Then multiply each factor by 10 ([20, 20, 20, 30, 50])
        // Then set the state to 10
        // Then sum them all together (140) and return this value.
        return await commandFactory.GetAggregate<ICalculatorAggregate>()
            // Add 120 to the initial state (0), making it 120
            .Add(120.0)
            // Get the prime factors of 120: [2, 2, 2, 3, 5]
            .GetPrimeFactors()
            // MergeSplit allows us to split the builder into two branches.
            // They must be merged back together at the end.
            // Several default strategies exist to merge the branches back together.
            // e.g. TakeFirst, TakeSecond, TakeGreater, TakeLesser, TakeTuple, etc.
            .MergeSplit(
                // Set the state to 100 by subtracting 20
                (b1) => b1.Add(-20.0),

                // "Then" keyword signals that we use the previous output as the input for "this" command/query.
                // However, we're not setting the state here.
                (b2) => 
                    b2.ThenForEach(
                        // Now, we're multiplying the state by each prime factor, one at a time
                        // 100*2*2*2*3*5 = 12000
                        (b) => b.ThenMultiply()
                    // Sum all the values from the previous result.
                    // 100*2 + 100*2*2 + 100*2*2*2 + 100*2*2*2*3 + 100*2*2*2*3*5 = 15800
                    ).ThenReduce(
                        new SumReducer()
                    ),
                
                // Take the second value, which is the sum of the multiplied factors, and should equal 15800.
                new TakeSecond<double, double>()
            )
            // Run for a given target.
            .Run(key);

        // The state is now 12000, however, the returned value is 15800.
    }


    public async Task<IEnumerable<double>> AdvancedChaining2(string key) 
    {
        // This example demonstrates advanced chaining with forEach and reduce operations
        
        // Start with adding 120 to the calculator state (initially 0)
        // Then get its prime factors (for 120: [2, 2, 2, 3, 5])
        // Then multiply each factor by 10 ([20, 20, 20, 30, 50])
        // Then set the state to 10
        // Then sum them all together (140) and return this value.
        return await commandFactory.GetAggregate<ICalculatorAggregate>()
            // Add 10 to the initial state (0), making it 10
            .Add(10.0)
            .ForEach(
                // For each value in the list, we will add it to the state, then multiply the state by it.
                new List<double>() {1.0, 2.0, 3.0 }, 
                (b) => b.MergeSplit(
                    // MergeSplit has an intuitive order of operations:
                    (b1) => b1.ThenAdd(), // The add command will execute first, as it's the first builder.
                    (b2) => b2.ThenMultiply(), // The multiply command will execute second.
                    new TakeSecond<double, double>()
                )

                // (10 + 1) * 1 = 11
                // (11 + 2) * 2 = 26
                // (26 + 3) * 3 = 87

                // Returned IEnumerable should be [11, 26, 87]
                // Final state value should be 87
            )
            // Run for a given target.
            .Run(key);
    }

    public async Task OnlyQueries() 
    {
        // Because we are using the query factory, we can only use queries.
        // This is infectious, meaning that queries run here can only run queries themselves.
        // Assuming the state is 120, the prime factors of 120: [2, 2, 2, 3, 5]
        var primeFactors = await queryFactory.GetAggregate<ICalculatorAggregate>()
            .GetPrimeFactors()
            .Run("Target1");
    }
}

