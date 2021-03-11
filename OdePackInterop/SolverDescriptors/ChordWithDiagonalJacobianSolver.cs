using OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithDiagonalJacobianSolver : SolverDescriptorBase
    {
        public ChordWithDiagonalJacobianSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.ChordWithDiagonalJacobian)
        {
        }

        public override int LRW => SolutionMethod.Switch(
            onAdams: () => 22 + 17 * NumberOfEquations,
            onBdf: () => 22 + 10 * NumberOfEquations);

        public override int LIW => 20;
    }
}
