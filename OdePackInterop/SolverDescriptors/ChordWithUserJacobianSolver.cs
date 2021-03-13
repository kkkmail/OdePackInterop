using Softellect.OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
namespace Softellect.OdePackInterop.SolverDescriptors
{
    public record ChordWithUserJacobianSolver : SolverDescriptorBase
    {
        public ChordWithUserJacobianSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.ChordWithUserJacobian)
        {
        }

        public override int LRW => SolutionMethod.Switch(
            onAdams: () => 22 + 16 * NumberOfEquations + NumberOfEquations * NumberOfEquations,
            onBdf: () => 22 + 9 * NumberOfEquations + NumberOfEquations * NumberOfEquations);

        public override int LIW => 20 + NumberOfEquations;
    }
}
