using System.Runtime.CompilerServices;

namespace Softellect.OdePackInterop.Sets
{
    public record ResultState : ClosedDualSetBase<ResultState, int, string>
    {
        public bool HasSucceeded { get; }

        private ResultState(int key, bool hasSucceeded = false, [CallerMemberName] string? value = null) : base(key, value!)
        {
            HasSucceeded = hasSucceeded;
        }

        public static ResultState Success { get; } = new(2, hasSucceeded: true);

        public static ResultState NothingWasDone { get; } = new(1);
        public static ResultState ExcessWorkDone { get; } = new(-1);
        public static ResultState ExcessAccuracyRequested { get; } = new(-2);
        public static ResultState IllegalInputDetected { get; } = new(-3);
        public static ResultState RepeatedErrorTestFailures { get; } = new(-4);
        public static ResultState RepeatedConvergenceFailures { get; } = new(-5);
        public static ResultState ErrorWeightsBecameZero { get; } = new(-6);
        public static ResultState GlobalFailure { get; } = new(int.MinValue);
    }
}
