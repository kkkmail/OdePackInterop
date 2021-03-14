using System.Runtime.CompilerServices;

namespace Softellect.OdePackInterop.Sets
{
    public record CorrectorIteratorMethod : ClosedDualSetBase<CorrectorIteratorMethod, int, string>
    {
        /// <summary>
        /// If true then either user supplied full or banded Jacobian is required OR it will be calculated.
        /// In a sense this is a measure of how many calls to F per step will be made.
        /// </summary>
        public bool UseJacobian { get; }

        /// <summary>
        /// User must supply a Jacobian function.
        /// </summary>
        public bool RequireUserJacobian { get; }

        private CorrectorIteratorMethod(
            int key,
            [CallerMemberName] string? value = null,
            bool useJacobian = false,
            bool requireUserJacobian = false) : base(key, value!)
        {
            RequireUserJacobian = requireUserJacobian;
            UseJacobian = useJacobian || requireUserJacobian;
        }

        public static CorrectorIteratorMethod Functional { get; } = new(0);
        public static CorrectorIteratorMethod ChordWithUserJacobian { get; } = new(1, requireUserJacobian: true);
        public static CorrectorIteratorMethod ChordWithGeneratedJacobian { get; } = new(2, useJacobian: true);
        public static CorrectorIteratorMethod ChordWithDiagonalJacobian { get; } = new(3);
        public static CorrectorIteratorMethod ChordWithBandedUserJacobian { get; } = new(4, requireUserJacobian: true);
        public static CorrectorIteratorMethod ChordWithBandedGeneratedJacobian { get; } = new(5, useJacobian: true);
    }
}
