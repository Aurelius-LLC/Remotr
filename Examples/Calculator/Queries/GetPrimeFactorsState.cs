
namespace Remotr.Example.Calculator;

[RemotrGen]
public class GetPrimeFactorsState : StatefulQueryHandler<CalculatorState, IEnumerable<double>>
{
    public override async Task<IEnumerable<double>> Execute()
    {
        var number = (await GetState()).Value;

        // Must be an integer.
        if (number % 1.0 > 0) {
            return [];
        }

        var primes = new List<int>();

        for (int div = 2; div <= number; div++)
            while (number % div == 0)
            {
                primes.Add(div);
                number = number / div;
            }
        
        return primes.Select(p => (double)p);
    }
}