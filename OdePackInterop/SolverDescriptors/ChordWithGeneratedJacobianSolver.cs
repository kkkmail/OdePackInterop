using OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithGeneratedJacobianSolver : SolverDescriptorBase
    {
        public ChordWithGeneratedJacobianSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.ChordWithGeneratedJacobian)
        {
        }

        public override int LRW => SolutionMethod.Switch(
            onAdams: () => 22 + 16 * NumberOfEquations + NumberOfEquations * NumberOfEquations,
            onBdf: () => 22 + 9 * NumberOfEquations + NumberOfEquations * NumberOfEquations);

        public override int LIW => 20 + NumberOfEquations;
    }
}
