using System;

namespace OdePackInterop.Sets
{
    public abstract record ClosedSetBase<T, TV> : SetBase<T, TV>
        where T : ClosedSetBase<T, TV>
        where TV : IComparable<TV>
    {
        protected ClosedSetBase(TV value) : base(value)
        {
        }
    }
}
