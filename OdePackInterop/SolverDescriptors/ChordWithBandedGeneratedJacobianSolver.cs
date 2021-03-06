
using OdePackInterop.Sets;

namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithBandedGeneratedJacobianSolver : SolverDescriptorBase
    {
        public ChordWithBandedGeneratedJacobianSolver(
            int numberOfEquations,
            MethodFlag methodFlag)
            : base(numberOfEquations, methodFlag, CorrectorIteratorMethod.ChordWithBandedGeneratedJacobian)
        {

        }
    }
}
