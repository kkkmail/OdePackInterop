using System.Runtime.CompilerServices;

namespace OdePackInterop.Sets
{
    public record SolverTask : ClosedDualSetBase<SolverTask, int, string>
    {
        private SolverTask(int key, [CallerMemberName] string? value = null) : base(key, value!)
        {
        }

        public static SolverTask NormalComputation { get; } = new(1);

        public static SolverTask TakeOneStepOnly { get; } = new(2);
        public static SolverTask StopAtEndTime { get; } = new(3);
        public static SolverTask NormalComputationWithoutOvershooting { get; } = new(4);
        public static SolverTask TakeOneStepWithoutOvershooting { get; } = new(5);

        public static SolverTask DefaultValue { get; } = NormalComputation;
    }
}
