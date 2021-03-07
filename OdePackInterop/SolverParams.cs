using System.Linq;
using OdePackInterop.SolverDescriptors;

namespace OdePackInterop
{
    public record SolverParams
    {
        public static double DefaultRelativeTolerance = 1.0e-05;
        public static double DefaultAbsoluteTolerance = 1.0e-07;
        public SolverDescriptorBase SolverDescriptor { get; }
        public double StartTime { get; init; }
        public double EndTime { get; init; }
        public double[] InitialValues { get; }
        public double[] RelativeTolerance { get; init; }
        public double[] AbsoluteTolerance { get; init; }

        public SolverParams(
            SolverDescriptorBase solverDescriptor,
            double[] initialValues)
        {
            SolverDescriptor = solverDescriptor;
            InitialValues = initialValues;

            RelativeTolerance =
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => 0.0 * DefaultRelativeTolerance)
                    .ToArray();

            AbsoluteTolerance =
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => DefaultAbsoluteTolerance)
                    .ToArray();
        }
    }
}
