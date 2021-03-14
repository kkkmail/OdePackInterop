using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace Softellect.OdePackInterop.Sets
{
    public abstract record SetBase<T, TV> : IComparable<T>, IEquatable<T>
        where T : SetBase<T, TV>
        where TV : IComparable<TV>
    {
        public TV Value { get; }

        protected SetBase(TV value) => Value = value;

        public static implicit operator TV?(SetBase<T, TV>? other) =>
            other != null
                ? other.Value
                : typeof(TV).IsValueType && Nullable.GetUnderlyingType(typeof(TV)) == null
                    ? throw new NullReferenceException($"Cannot cast null {typeof(T).Name} to {typeof(TV).Name}.")
                    : default(TV);

        public int CompareTo(T? other) => Value?.CompareTo(other) ?? (other == null ? 0 : 1);
        public bool Equals(T? other) => other != null && EqualityComparer<TV>.Default.Equals(Value, other.Value);
        public override string ToString() => $"{Value}";

        public static InvalidDataException ToInvalidDataException(SetBase<T, TV> value) =>
            new($"Invalid {typeof(T)}: '{value}'.");
    }
}
