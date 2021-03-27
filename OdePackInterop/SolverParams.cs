using System.Linq;
using Softellect.OdePackInterop.SolverDescriptors;

// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Softellect.OdePackInterop
{
    public record SolverParams
    {
        public const double DefaultRelativeTolerance = 1.0e-05;
        public const double DefaultAbsoluteTolerance = 1.0e-07;
        public SolverDescriptorBase SolverDescriptor { get; }
        public double StartTime { get; init; }
        public double EndTime { get; init; }
        public double[] InitialValues { get; }
        public double[] RelativeTolerance { get; }
        public double[] AbsoluteTolerance { get; }

        public SolverParams(
            SolverDescriptorBase solverDescriptor,
            double[] initialValues,
            double[] absoluteTolerance,
            double[] relativeTolerance)
        {
            SolverDescriptor = solverDescriptor;
            InitialValues = initialValues;
            AbsoluteTolerance = absoluteTolerance;
            RelativeTolerance = relativeTolerance;
        }

        public SolverParams(
            SolverDescriptorBase solverDescriptor,
            double[] initialValues,
            double absoluteTolerance = DefaultAbsoluteTolerance,
            double relativeTolerance = 0.0 * DefaultRelativeTolerance) :
            this(solverDescriptor,
                initialValues,
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => absoluteTolerance)
                    .ToArray(),
                Enumerable.Range(0, solverDescriptor.NumberOfEquations)
                    .Select(_ => relativeTolerance)
                    .ToArray())
        {
        }
    }
}
