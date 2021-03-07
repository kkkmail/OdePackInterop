using System;
using System.Collections.Immutable;

namespace OdePackInterop.Sets
{
    public abstract record ClosedDualSetBase<T, TK, TV> : ClosedSetBase<T, TV>
        where T : ClosedDualSetBase<T, TK, TV>
        where TV : IComparable<TV>
        where TK : IComparable<TK>
    {
        public TK Key { get; }

        protected ClosedDualSetBase(TK key, TV value) : base(value) => Key = key;

        private static readonly Lazy<ImmutableDictionary<TK, T>> AllKeysDictionary =
            new(() => GetAllValues().ToImmutableDictionary(e => e.Key, e => e));

        public static ImmutableDictionary<TK, T> GetAllKeysDictionary() => AllKeysDictionary.Value;
        public static T? TryCreate(TK k) => GetAllKeysDictionary().TryGetValue(k, out var t) ? t : null;
    }
}
