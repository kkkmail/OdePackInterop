using Softellect.OdePackInterop.Sets;

namespace Softellect.OdePackInterop.SolverDescriptors
{
    public abstract record SolverDescriptorBase
    {
        public abstract int LRW { get; }
        public abstract int LIW { get; }

        public int NumberOfEquations { get; }
        public SolutionMethod SolutionMethod { get; }
        public CorrectorIteratorMethod IteratorMethod { get; }
        public int MethodFlag => SolutionMethod.Key * 10 + IteratorMethod.Key;

        protected SolverDescriptorBase(
            int numberOfEquations,
            SolutionMethod solutionMethod,
            CorrectorIteratorMethod iteratorMethod
            )
        {
            NumberOfEquations = numberOfEquations;
            SolutionMethod = solutionMethod;
            IteratorMethod = iteratorMethod;
        }
    }
}
