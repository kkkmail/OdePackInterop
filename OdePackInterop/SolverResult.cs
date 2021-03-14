using Softellect.OdePackInterop.Sets;

// ReSharper disable InconsistentNaming
namespace Softellect.OdePackInterop
{
    public record SolverResult
    {
        public ResultState ResultState { get; init; } = ResultState.GlobalFailure;
        public double StartTime { get; init; }
        public double EndTime { get; init; }
        public double[] X { get; init; } = System.Array.Empty<double>();
        public int Steps { get; init; }
        public long FuncCalls { get; init; }
        public int JacobianCalls { get; init; }
        public int RequiredLRW { get; init; }
        public int RequiredLIW { get; init; }
        public int NumberOfEquations => X.Length;
    }
}
