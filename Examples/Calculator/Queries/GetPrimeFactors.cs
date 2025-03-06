namespace Remotr.Example.Calculator;

// [RemotrGen]
// public class GetPrimeFactors : StatelessQueryHandler<ICalculatorManagerGrain, IEnumerable<double>>
// {
//     public override Task<IEnumerable<double>> Execute()
//     {
//         return QueryFactory.GetChild<CalculatorState>()
//             .GetPrimeFactorsState()
//             .Run(GetPrimaryKey().ToString());
//     }
// }