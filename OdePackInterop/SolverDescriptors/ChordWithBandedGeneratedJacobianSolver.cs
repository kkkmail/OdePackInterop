using Softellect.OdePackInterop.Sets;

// ReSharper disable ArgumentsStyleAnonymousFunction
// ReSharper disable InconsistentNaming
namespace Softellect.OdePackInterop.SolverDescriptors
{
    public record ChordWithBandedGeneratedJacobianSolver : SolverDescriptorBase
    {
        public int ML { get; }
        public int MU { get; }

        public ChordWithBandedGeneratedJacobianSolver(
            int numberOfEquations,
            SolutionMethod solutionMethod,
            int ml,
            int mu)
            : base(numberOfEquations, solutionMethod, CorrectorIteratorMethod.ChordWithBandedGeneratedJacobian)
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
