using System;

namespace OdePackInterop.Sets
{
    public abstract record ClosedDualSetBase<T, TK, TV> : ClosedSetBase<T, TV>
        where T : ClosedDualSetBase<T, TK, TV>
        where TV : IComparable<TV>
    {
        public TK Key { get; }

        protected ClosedDualSetBase(TK key, TV value) : base(value) => Key = key;
    }
}
