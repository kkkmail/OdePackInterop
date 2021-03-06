using System.Linq;
using OdePackInterop.SolverDescriptors;
using static OdePackInterop.Interop;

namespace OdePackInterop
{
    public record SolverParams
    {
        public static double DefaultRelativeTolerance = 1.0e-04;
        public static double DefaultAbsoluteTolerance = 1.0e-06;
        public SolverDescriptorBase SolverDescriptor { get; }
        public double StartTime { get; init; }
        public double EndTime { get; init; }
        public double[] InitialValues { get; }
        public double[] RelativeTolerance { get; init; }
        public double[] AbsoluteTolerance { get; init; }
        public F F { get; }
        public JAC? Jacobian { get; init; } = null;

        public SolverParams(
            SolverDescriptorBase solverDescriptor,
            double[] initialValues,
            F f)
        {
            SolverDescriptor = solverDescriptor;
            InitialValues = initialValues;
            F = f;

            RelativeTolerance =
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => DefaultRelativeTolerance)
                    .ToArray();

            AbsoluteTolerance =
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => DefaultAbsoluteTolerance)
                    .ToArray();
        }
    }
}
