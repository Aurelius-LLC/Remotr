using Orleans.Runtime.GrainDirectory;

namespace Remotr.Example.Calculator;

public class Examples(IExternalCommandFactory factory)
{
    private readonly IExternalCommandFactory factory = factory;

    public async Task SimpleArithmetic() {

        // CalculatorState starts at 0.
        // This algorithm returns 1 given an initial state of 0.
        var algorithm = factory.GetManager<ICalculatorManagerGrain>()
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
        // Then sum them all together (140)
        // Finally set the state to this result
        await factory.GetManager<ICalculatorManagerGrain>()
            // Add 120 to the initial state (0), making it 120
            .Add(120.0)
            // Get the prime factors of 120: [2, 2, 2, 3, 5]
            .GetPrimeFactors()
            // Multiply each factor by 10
            // "Then" keyword signals that we use the previous output as the input for "this" command/query.
            .ThenForEach(
                (b) => b.Multiply(10.0)
            )
            // Sum all the values: 20 + 20 + 20 + 30 + 50 = 140
            .ThenReduce(
                new SumReducer()
            )
            // State = 140
            .ThenSet()
            // Run for a given target.
            .Run("target1");
    }
}