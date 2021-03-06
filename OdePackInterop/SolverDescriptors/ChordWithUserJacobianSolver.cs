using OdePackInterop.Sets;

namespace OdePackInterop.SolverDescriptors
{
    public record ChordWithUserJacobianSolver : SolverDescriptorBase
    {
        public ChordWithUserJacobianSolver(
            int numberOfEquations,
            MethodFlag methodFlag)
            : base(numberOfEquations, methodFlag, CorrectorIteratorMethod.ChordWithUserJacobian)
        {

        }
    }
}
