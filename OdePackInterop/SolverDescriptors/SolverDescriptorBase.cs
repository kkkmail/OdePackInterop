using OdePackInterop.Sets;

namespace OdePackInterop.SolverDescriptors
{
    public abstract record SolverDescriptorBase
    {
        public abstract int LRW { get; }
        public abstract int LIW { get; }

        public int NumberOfEquations { get; }
        public MethodFlag MethodFlag { get; }
        public CorrectorIteratorMethod IteratorMethod { get; }

        protected SolverDescriptorBase(
            int numberOfEquations,
            MethodFlag methodFlag,
            CorrectorIteratorMethod iteratorMethod
            )
        {
            NumberOfEquations = numberOfEquations;
            MethodFlag = methodFlag;
            IteratorMethod = iteratorMethod;
        }
    }
}
