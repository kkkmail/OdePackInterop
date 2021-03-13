using System.Runtime.CompilerServices;

namespace Softellect.OdePackInterop.Sets
{
    public record SolutionMethod : ClosedDualSetBase<SolutionMethod, int, string>
    {
        private SolutionMethod(int key, [CallerMemberName] string? value = null) : base(key, value!)
        {
        }

        public static SolutionMethod Adams { get; } = new(1);
        public static SolutionMethod Bdf { get; } = new(2);
    }
}
