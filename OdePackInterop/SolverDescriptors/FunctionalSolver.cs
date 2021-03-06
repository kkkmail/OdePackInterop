using OdePackInterop.Sets;

namespace OdePackInterop.SolverDescriptors
{
    public record FunctionalSolver : SolverDescriptorBase
    {
        public FunctionalSolver(
            int numberOfEquations,
            MethodFlag methodFlag)
            : base(numberOfEquations, methodFlag, CorrectorIteratorMethod.Functional)
        {
        }

        public override int LRW => 0;
        public override int LIW => 0;
    }
}
