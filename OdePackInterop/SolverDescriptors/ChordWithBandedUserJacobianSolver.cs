using OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
// ReSharper disable InconsistentNaming
namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithBandedUserJacobianSolver : SolverDescriptorBase
    {
        public int ML { get; }
        public int MU { get; }

        public ChordWithBandedUserJacobianSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod,
            int ml,
            int mu)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.ChordWithBandedUserJacobian)
        {
            ML = ml;
            MU = mu;
        }

        public override int LRW => SolutionMethod.Switch(
            onAdams: () => 22 + 17 * NumberOfEquations + (2 * ML + MU) * NumberOfEquations,
            onBdf: () => 22 + 10 * NumberOfEquations + (2 * ML + MU) * NumberOfEquations);

        public override int LIW => 20 + NumberOfEquations;
    }
}
