using System.Runtime.CompilerServices;

namespace OdePackInterop.Sets
{
    public record CorrectorIteratorMethod : ClosedDualSetBase<CorrectorIteratorMethod, int, string>
    {
        /// <summary>
        /// If true then either user supplied full or banded Jacobian is required OR it will be calculated.
        /// In a sense this is a measure of how many calls to F per step will be made.
        /// </summary>
        public bool UseJacobian { get; }

        public bool RequireUserJacobian { get; }

        private CorrectorIteratorMethod(
            int key,
            [CallerMemberName] string? value = null,
            bool useJacobian = false,
            bool requireUserJacobian = false) : base(key, value!)
        {
            UseJacobian = useJacobian;
            RequireUserJacobian = requireUserJacobian;
        }

        public static CorrectorIteratorMethod Functional { get; } = new(0);
        public static CorrectorIteratorMethod ChordWithUserJacobian { get; } = new(1, useJacobian: true, requireUserJacobian: true);
        public static CorrectorIteratorMethod ChordWithGeneratedJacobian { get; } = new(2, useJacobian: true);
        public static CorrectorIteratorMethod ChordWithDiagonalJacobian { get; } = new(3);
        public static CorrectorIteratorMethod ChordWithBandedUserJacobian { get; } = new(4, useJacobian: true, requireUserJacobian: true);
        public static CorrectorIteratorMethod ChordWithBandedGeneratedJacobian { get; } = new(5, useJacobian: true);
    }
}
