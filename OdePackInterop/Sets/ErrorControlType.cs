using System.Runtime.CompilerServices;

namespace Softellect.OdePackInterop.Sets
{
    public record ErrorControlType : ClosedDualSetBase<ErrorControlType, int, string>
    {
        private ErrorControlType(int key, [CallerMemberName] string? value = null) : base(key, value!)
        {
        }

        public static ErrorControlType ErrorControlTypeOne { get; } = new(1);
        public static ErrorControlType ErrorControlTypeTwo { get; } = new(2);
        public static ErrorControlType ErrorControlTypeThree { get; } = new(3);
        public static ErrorControlType ErrorControlTypeFour { get; } = new(4);
    }
}
