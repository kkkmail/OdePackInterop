using System.Runtime.CompilerServices;

namespace Softellect.OdePackInterop.Sets
{
    public record CalculationState : ClosedDualSetBase<CalculationState, int, string>
    {
        private CalculationState(int key, [CallerMemberName] string? value = null) : base(key, value!)
        {
        }

        public static CalculationState FirstCall { get; } = new(1);
        public static CalculationState Continue { get; } = new(2);
        public static CalculationState ContinueWithParamChange { get; } = new(3);

        public static CalculationState DefaultValue { get; } = FirstCall;
    }
}
