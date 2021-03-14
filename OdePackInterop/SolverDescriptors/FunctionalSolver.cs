using Softellect.OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
namespace Softellect.OdePackInterop.SolverDescriptors
{
    public record FunctionalSolver : SolverDescriptorBase
    {
        public FunctionalSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.Functional)
        {
        }

        public override int LRW => SolutionMethod.Switch(
            onAdams: () => 20 + 16 * NumberOfEquations,
            onBdf: () => 20 + 9 * NumberOfEquations);

        public override int LIW => 20;
    }
}
